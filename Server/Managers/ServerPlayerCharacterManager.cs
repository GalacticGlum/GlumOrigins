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
        private readonly Dictionary<NetConnection, int> playerConnections;
        private readonly Dictionary<int, PlayerCharacter> playerCharacters;

        public ServerPlayerCharacterManager()
        {
            playerConnections = new Dictionary<NetConnection, int>();
            playerCharacters = new Dictionary<int, PlayerCharacter>();

            CoreApp.Server.Packets[ClientOutgoingPacketType.SendLogin] += HandleLogin;
            CoreApp.Server.Packets[ClientOutgoingPacketType.SendMovement] += HandleMovement;
            CoreApp.Server.PeerDisconnected += HandlePlayerDisconnect;  
        }

        private void HandleMovement(object sender, PacketRecievedEventArgs args)
        {
            int id = args.Buffer.ReadInt32();
            if (!playerCharacters.ContainsKey(id)) return;

            Tile destination = args.Buffer.Read<Tile>();
            playerCharacters[id].Tile = destination;

            Packet packet = CoreApp.Server.CreatePacket(ServerOutgoingPacketType.UpdatePlayerPositions);
            packet.Write(id);
            packet.Write(destination);
            CoreApp.Server.SendToAll(packet, NetDeliveryMethod.ReliableUnordered);
        }

        private void HandlePlayerDisconnect(object sender, ConnectionEventArgs args)
        {
            if (!playerConnections.ContainsKey(args.Connection)) return;

            int id = playerConnections[args.Connection];
            if (!playerCharacters.ContainsKey(id)) return;

            Packet packet = CoreApp.Server.CreatePacket(ServerOutgoingPacketType.SendPlayerDisconnect);
            packet.Write(id);
            CoreApp.Server.SendToAll(packet, NetDeliveryMethod.ReliableUnordered);

            playerCharacters.Remove(id);
            playerConnections.Remove(args.Connection);
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
            packet.Write(playerCharacters[id].Tile.Position.X);
            packet.Write(playerCharacters[id].Tile.Position.Y);

            CoreApp.Server.SendToAll(packet, NetDeliveryMethod.ReliableOrdered);
            SendAllPlayers(args.SenderConnection);
        }

        private void SendAllPlayers(NetConnection target)
        {
            Packet packet = CoreApp.Server.CreatePacket(ServerOutgoingPacketType.SendPlayerList);
            packet.Write(playerCharacters.Count); // The amount of players we are sending, basically how much we'll loop for (to read).
            foreach (PlayerCharacter playerCharacter in playerCharacters.Values)
            {
                packet.Write(playerCharacter);
            }

            CoreApp.Server.Send(packet, target, NetDeliveryMethod.ReliableOrdered);
        }

        public void Create(NetConnection connection, int id, string name, Tile tile)
        {
            if (playerCharacters.ContainsKey(id)) return;
            playerCharacters.Add(id, new PlayerCharacter(id, name, tile));
            playerConnections.Add(connection, id);
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
