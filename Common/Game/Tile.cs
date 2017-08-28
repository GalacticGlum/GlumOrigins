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

    public class Tile : INetworked<Tile>
    {
        public Vector2i Position { get; private set; }

        public Tile()
        {
            Position = new Vector2i(-1);
        }

        public Tile(Vector2i position) 
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
