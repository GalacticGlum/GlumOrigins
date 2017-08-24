using GlumOrigins.Common;
using GlumOrigins.Common.Networking;
using Lidgren.Network;

namespace GlumOrigins.Client
{
    public class GameClient : GamePeer<NetClient>
    {
        public override NetPeerConfiguration NetConfiguration { get; } = new NetPeerConfiguration("game");

        public GameClient()
        {
            Validate();
        }

        protected override NetClient ConstructPeer()
        {
            return new NetClient(NetConfiguration);
        }

        public void Connect(string ip, int port)
        {
            NetPeer.Start();
            NetPeer.Connect(ip, port);
        }

        public void Disconnect()
        {
            NetPeer.Disconnect("");
        }
            
        public void Send(Packet packet, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable) => 
            NetPeer.SendMessage(GetOutgoingMessageFromPacket(packet), deliveryMethod);

        public void Send(ClientOutgoingPacketType packetHeader, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.Unreliable) => 
            NetPeer.SendMessage(CreateMessageWithHeader((int)packetHeader), deliveryMethod);
    }
}
