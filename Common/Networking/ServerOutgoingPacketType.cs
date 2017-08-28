namespace GlumOrigins.Common.Networking
{
    /// <summary>
    /// A packet which leaves from the server and arrives at a client.
    /// </summary>
    public enum ServerOutgoingPacketType
    {
        SendWorldConfiguration,
        SendNewPlayer,
        SendPlayerList,
        SendPlayerDisconnect,
        UpdatePlayerPositions
    }
}
