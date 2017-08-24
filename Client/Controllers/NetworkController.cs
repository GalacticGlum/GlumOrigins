using UnityEngine;
using Logger = GlumOrigins.Common.Logging.Logger;

namespace GlumOrigins.Client.Controllers
{
    public class NetworkController : MonoBehaviour
    {
        public static NetworkController Instance { get; private set; }
        public GameClient Client { get; private set; }

        [Header("Connection Settings")]
        [SerializeField]
        private string ipAddress;
        [SerializeField]
        private int port;

        private void OnEnable()
        {
            Logger.Initialize(true);

            Instance = this;
            Client = new GameClient();
            Client.Connect(ipAddress, port);
        }

        private void Update()
        {
            Client.Listen();
        }
    }
}
