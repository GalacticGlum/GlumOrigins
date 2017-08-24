using UnityUtilities.Math;

namespace GlumOrigins.Server
{
    public struct Tile
    {
        public Vector2i Position { get; set; }
        public Tile(Vector2i position) : this()
        {
            Position = position;
        }
    }
}
