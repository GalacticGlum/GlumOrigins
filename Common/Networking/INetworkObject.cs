namespace GlumOrigins.Common.Networking
{
    public interface INetworkObject
    {
        void Write(Packet packet);
        void Read(Packet packet);
    }
}
