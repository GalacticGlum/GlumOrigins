using Lidgren.Network;

namespace GlumOrigins.Common.Helpers
{
    public static class CopyHelpers
    {
        public static NetBuffer Clone(this NetBuffer netBuffer) => new NetBuffer
        {
            Data = netBuffer.Data
        };

        public static void CopyFrom(this NetOutgoingMessage destination, NetBuffer source)
        {
            if (destination == null || source == null) return;
            destination.Data = source.Data;
        }
    }
}
