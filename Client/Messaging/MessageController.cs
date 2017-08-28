using UnityEngine;

namespace GlumOrigins.Client.Messaging
{
    public delegate void MessageHandler();
    public class MessageController : MonoBehaviour
    {
        public static MessageController Instance { get; private set; }
        private event MessageHandler messageHandler;

        private void OnEnable()
        {
            Instance = this;
        }

        private void Update()
        {
            messageHandler?.Invoke();
        }

        public void Subscribe(MessageHandler handler)
        {
            messageHandler += handler;
        }

        public void Unsubscribe(MessageHandler handler)
        {
            if (messageHandler == null) return;
            messageHandler -= handler;
        }
    }
}
