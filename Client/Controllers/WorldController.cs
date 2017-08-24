using GlumOrigins.Common;
using GlumOrigins.Common.Game;
using GlumOrigins.Common.Networking;
using Newtonsoft.Json;
using UnityEngine;
using Logger = GlumOrigins.Common.Logging.Logger;

namespace GlumOrigins.Client.Controllers
{
    public class WorldController : MonoBehaviour
    {
        public WorldConfiguration Configuration { get; private set; }

        private void Start()
        {
            NetworkController.Instance.Client.Packets[ServerOutgoingPacketType.SendWorldConfiguration] += HandleWorldData;
        }

        private void HandleWorldData(object sender, PacketRecievedEventArgs args)
        {
            Configuration = JsonConvert.DeserializeObject<WorldConfiguration>(args.Buffer.ReadString());

            Logger.Log(Configuration.Width);
            Logger.Log(Configuration.Height);
        }

        public void RequestWorldData()
        {
            NetworkController.Instance.Client.Send(ClientOutgoingPacketType.RequestWorldConfiguration);
        }
    }
}
