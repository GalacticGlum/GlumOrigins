using System;
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

    public struct Tile
    {
        public Vector2i Position { get; private set; }
        public Tile(Vector2i position) : this()
        {
            Position = position;
        }
    }
}
