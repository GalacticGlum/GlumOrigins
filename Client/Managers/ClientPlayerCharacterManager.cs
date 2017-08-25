using System;
using System.Collections;
using System.Collections.Generic;
using GlumOrigins.Client.Controllers;
using GlumOrigins.Common.Game;
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

        public void Create(int id, string name, Tile tile)
        {
            PlayerCharacter playerCharacter = new PlayerCharacter(id, name, tile);
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
