using System;
using GlumOrigins.Common.Networking;
using Lidgren.Network;
using UnityUtilities.Math;

namespace GlumOrigins.Common.Game
{
    public class TileEventArgs : EventArgs
    {
        public Tile Tile { get; }
        public TileEventArgs(Tile tile)
        {
            Tile = tile;
        }
    }

    public struct Tile : INetworked<Tile>
    {
        public Vector2i Position { get; private set; }
        public Tile(Vector2i position) : this()
        {
            Position = position;
        }

        public void Serialize(NetBuffer packet)
        {
            packet.Write(Position.X);
            packet.Write(Position.Y);
        }

        public void Deserialize(NetBuffer packet)
        {
            Position = new Vector2i(packet.ReadInt32(), packet.ReadInt32());
        }
    }
}
