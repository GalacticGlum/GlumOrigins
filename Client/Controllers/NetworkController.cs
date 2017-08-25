using GlumOrigins.Common.Networking;
using Lidgren.Network;
using Logger = GlumOrigins.Common.Logging.Logger;

namespace GlumOrigins.Client.Controllers
{
    public class NetworkController
    {
        public static NetworkController Instance { get; private set; }
        public GameClient Client { get; }

        public NetworkController()
        {
            Logger.Initialize(true);

            Instance = this;
            Client = new GameClient();
        }

        public void Update()
        {
            Client.Listen();
        }

        public void Login(string name)
        {
            Packet packet = Client.CreatePacket(ClientOutgoingPacketType.SendLogin);
            packet.Write(name);
            Client.Send(packet, NetDeliveryMethod.ReliableUnordered);
        }

        public void Connect(string ipAddress, int port)
        {
            Client.Connect(ipAddress, port);
        }
    }
}
