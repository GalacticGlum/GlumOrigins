using System.Collections.Generic;
using GlumOrigins.Common.Networking;
using Lidgren.Network;

namespace GlumOrigins.Server
{
    public class GameServer : GamePeer<NetServer>
    {
        public bool IsRunning { get; private set; }

        private const int ServerPort = 7777;
        private const int MaximumConnections = 100;

        public GameServer() 
        {
            Validate();
            HandleMessageType(NetIncomingMessageType.ConnectionApproval, (sender, args) => args.Message.SenderConnection.Approve());
        }

        public override NetPeerConfiguration NetConfiguration { get; } = new NetPeerConfiguration("game")
        {
            Port = ServerPort,
            MaximumConnections = MaximumConnections
        };

        protected override NetServer ConstructPeer()
        {
            NetConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            return new NetServer(NetConfiguration);
        }

        public void Start()
        {
            NetPeer.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            NetPeer.Shutdown(string.Empty);
            IsRunning = false;
        }

        public void Send(Packet packet, NetConnection connection, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendMessage(GetOutgoingMessageFromPacket(packet), connection, deliveryMethod);
        }

        public void Send(Packet packet, IList<NetConnection> connections, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            foreach (NetConnection netConnection in connections)
            {
                NetPeer.SendMessage(GetOutgoingMessageFromPacket(packet), netConnection, deliveryMethod);
            }
        }

        public void Send(ServerOutgoingPacketType header, NetConnection connection, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendMessage(CreateMessageWithHeader((int)header), connection, deliveryMethod);
        }

        public void Send(ServerOutgoingPacketType header, IList<NetConnection> connections, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            foreach (NetConnection netConnection in connections)
            {
                NetPeer.SendMessage(CreateMessageWithHeader((int)header), netConnection, deliveryMethod);
            }
        }

        public void SendToAll(Packet packet, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable)
        {
            NetPeer.SendToAll(GetOutgoingMessageFromPacket(packet), deliveryMethod);
        }
    }
}
