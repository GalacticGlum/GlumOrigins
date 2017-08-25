using GlumOrigins.Common.Game;
using GlumOrigins.Common.Networking;
using Newtonsoft.Json;

namespace GlumOrigins.Client.Controllers
{
    public class WorldController 
    {
        public static WorldController Instance { get; private set; }
        public World World { get; }

        public WorldController()
        {
            World = new World();
            Instance = this;

            // Request world config from the server when the client connects.
            NetworkController.Instance.Client.PeerConnected += (sender, args) => NetworkController.Instance.Client.Send(ClientOutgoingPacketType.RequestWorldConfiguration);
            NetworkController.Instance.Client.Packets[ServerOutgoingPacketType.SendWorldConfiguration] += HandleWorldData;
        }

        private void HandleWorldData(object sender, PacketRecievedEventArgs args)
        {
            WorldConfiguration configuration = JsonConvert.DeserializeObject<WorldConfiguration>(args.Buffer.ReadString());
            World.Initialize(configuration);

            NetworkController.Instance.Login("temp_name");
        }
    }
}
