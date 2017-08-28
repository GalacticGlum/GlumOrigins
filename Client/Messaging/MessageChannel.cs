using System.Collections.Generic;

namespace GlumOrigins.Client.Messaging
{
    public interface IMessage
    {
    }

    public delegate void MessageHandler<in TArgs>(TArgs args);
    public static class MessageChannel<TMessage> where TMessage : class, IMessage
    {
        private static readonly Queue<TMessage> messageQueue;
        private static event MessageHandler<TMessage> messageHandlers;

        static MessageChannel()
        {
            messageQueue = new Queue<TMessage>();
            MessageController.Instance.Subscribe(Invoke);
        }

        public static void Subscribe(MessageHandler<TMessage> handler)
        {
            messageHandlers += handler;
        }

        public static void Unsubscribe(MessageHandler<TMessage> handler)
        {
            if (messageHandlers == null) return;
            messageHandlers -= handler;
        }

        public static void Broadcast(TMessage message)
        {
            messageQueue.Enqueue(message);
        }

        private static void Invoke()
        {
            while (messageQueue.Count > 0)
            {
                messageHandlers?.Invoke(messageQueue.Dequeue());
            }
        }
    }

}
