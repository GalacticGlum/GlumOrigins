using System;
using System.Collections;
using System.Collections.Generic;
using GlumOrigins.Common.Game;
using GlumOrigins.Common.Logging;
using GlumOrigins.Common.Networking;
using Lidgren.Network;
using UnityUtilities.Math;

namespace GlumOrigins.Server.Managers
{
    public sealed class ServerPlayerCharacterManager : IEnumerable<PlayerCharacter>
    {
        private readonly Dictionary<NetConnection, PlayerCharacter> playerCharacters;

        public ServerPlayerCharacterManager()
        {
            playerCharacters = new Dictionary<NetConnection, PlayerCharacter>();
            CoreApp.Server.Packets[ClientOutgoingPacketType.SendLogin] += HandleLogin;
            CoreApp.Server.PeerDisconnected += HandlePlayerDisconnect;
        }

        private void HandlePlayerDisconnect(object sender, ConnectionEventArgs args)
        {
            if (!playerCharacters.ContainsKey(args.Connection)) return;

            Packet packet = CoreApp.Server.CreatePacket(ServerOutgoingPacketType.SendPlayerDisconnect);
            packet.Write(playerCharacters[args.Connection].Id);
            CoreApp.Server.SendToAll(packet, NetDeliveryMethod.ReliableUnordered);

            playerCharacters.Remove(args.Connection);
        }

        private void HandleLogin(object sender, PacketRecievedEventArgs args)
        {
            Logger.Log("Receiving login packet");

            int id = playerCharacters.Count + 1;
            string name = args.Buffer.ReadString();
            Create(args.SenderConnection, id, name, new Tile(new Vector2i(0, id)));

            Packet packet = CoreApp.Server.CreatePacket(ServerOutgoingPacketType.SendNewPlayer);
            packet.Write(id);
            packet.Write(name);
            packet.Write(playerCharacters[args.SenderConnection].Tile.Position.X);
            packet.Write(playerCharacters[args.SenderConnection].Tile.Position.Y);

            CoreApp.Server.SendToAll(packet, NetDeliveryMethod.ReliableOrdered);
            SendAllPlayers();
        }

        private void SendAllPlayers()
        {
            Packet packet = CoreApp.Server.CreatePacket(ServerOutgoingPacketType.SendAllPlayers);
            packet.Write(playerCharacters.Count); // The amount of players we are sending, basically how much we'll loop for (to read).
            foreach (PlayerCharacter playerCharacter in playerCharacters.Values)
            {
                packet.Write(playerCharacter);
            }

            CoreApp.Server.SendToAll(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void Create(NetConnection connection, int id, string name, Tile tile)
        {
            if (playerCharacters.ContainsKey(connection)) return;
            playerCharacters.Add(connection, new PlayerCharacter(id, name, tile));
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
