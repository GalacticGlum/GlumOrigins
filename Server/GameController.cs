using GlumOrigins.Common.Logging;
using GlumOrigins.Common.Networking;
using Newtonsoft.Json;

namespace GlumOrigins.Server
{
    public class GameController
    {
        public static GameController Instance { get; private set; }
        public NetworkWorld NetworkWorld { get; }

        private GameController()
        {
            NetworkWorld = new NetworkWorld();
            CoreApp.Server.Packets[ClientOutgoingPacketType.RequestWorldConfiguration] += SendClientWorldConfiguration;
        }

        private void SendClientWorldConfiguration(object sender, PacketRecievedEventArgs args)
        {
            Packet packet = CoreApp.Server.CreatePacket(ServerOutgoingPacketType.SendWorldConfiguration);
            packet.Write(JsonConvert.SerializeObject(NetworkWorld.Configuration, Formatting.None));
            CoreApp.Server.Send(packet, args.SenderConnection);

            Logger.LogDestination = LoggerDestination.All;
            Logger.Log("Sending world config");
        } 

        public static void Create()
        {
            Instance = new GameController();
        }
    }
}
