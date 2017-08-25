using System;
using System.Collections;
using System.Collections.Generic;
using GlumOrigins.Client.Controllers;
using GlumOrigins.Common.Game;
using GlumOrigins.Common.Logging;
using GlumOrigins.Common.Networking;
using UnityUtilities.Math;

namespace GlumOrigins.Client.Managers
{
    public sealed class ClientPlayerCharacterManager : IEnumerable<PlayerCharacter>
    {
        public event PlayerCharacterEventHandler Created;
        private void OnCreated(PlayerCharacter character)
        {
            Created?.Invoke(this, new PlayerCharacterEventArgs(character));
        }

        public event PlayerCharacterEventHandler Removed;
        private void OnRemoved(PlayerCharacter character)
        {
            Removed?.Invoke(this, new PlayerCharacterEventArgs(character));
        }

        private readonly Dictionary<int, PlayerCharacter> playerCharacters;

        public ClientPlayerCharacterManager()
        {
            playerCharacters = new Dictionary<int, PlayerCharacter>();
            NetworkController.Instance.Client.Packets[ServerOutgoingPacketType.SendNewPlayer] += HandleNewPlayer;
            NetworkController.Instance.Client.Packets[ServerOutgoingPacketType.SendPlayerDisconnect] += HandlePlayerDisconnect;
            NetworkController.Instance.Client.Packets[ServerOutgoingPacketType.SendAllPlayers] += HandleAllPlayers;
        }

        private void HandleAllPlayers(object sender, PacketRecievedEventArgs args)
        {
            int count = args.Buffer.ReadInt32() - 1;
            for (int i = 0; i < count; i++)
            {
                Logger.Log(i);
                PlayerCharacter character = args.Buffer.Read<PlayerCharacter>();
                Logger.Log(character.Id);
                if (playerCharacters.ContainsKey(character.Id)) return;
                Create(character);
            }
        }

        private void HandlePlayerDisconnect(object sender, PacketRecievedEventArgs args)
        {
            int id = args.Buffer.ReadInt32();
            if (!playerCharacters.ContainsKey(id)) return;

            PlayerCharacter playerCharacter = playerCharacters[id];
            playerCharacters.Remove(id);

            OnRemoved(playerCharacter);
        }

        private void HandleNewPlayer(object sender, PacketRecievedEventArgs args)
        {
            int id = args.Buffer.ReadInt32();
            string name = args.Buffer.ReadString();
            int x = args.Buffer.ReadInt32();
            int y = args.Buffer.ReadInt32();

            Create(id, name, new Tile(new Vector2i(x, y)));
        }

        private void Create(int id, string name, Tile tile)
        {
            if (playerCharacters.ContainsKey(id)) return;
            PlayerCharacter playerCharacter = new PlayerCharacter(id, name, tile);
            playerCharacters.Add(playerCharacter.Id, playerCharacter);

            OnCreated(playerCharacter);
        }

        private void Create(PlayerCharacter playerCharacter)
        {
            if (playerCharacters.ContainsKey(playerCharacter.Id)) return;
            playerCharacters.Add(playerCharacter.Id, playerCharacter);
            OnCreated(playerCharacter);
        }

        public IEnumerator<PlayerCharacter> GetEnumerator()
        {
            return playerCharacters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
