using Lidgren.Network;

namespace GlumOrigins.Common.Networking
{
    // ReSharper disable once UnusedTypeParameter
    public interface INetworked<T> where T : new()
    {
        void Serialize(NetBuffer packet);
        void Deserialize(NetBuffer packet);
    }
}
