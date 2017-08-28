using System;
using GlumOrigins.Client.Managers;
using GlumOrigins.Common.Game;
using UnityUtilities.Math;

namespace GlumOrigins.Client
{
    public delegate void TileCreatedEventHandler(object sender, TileEventArgs args);

    public class World
    {
        public static World Current { get; private set; }

        public WorldConfiguration Configuration { get; private set; }
        public ClientPlayerCharacterManager PlayerCharacterManager { get; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public event TileCreatedEventHandler TileCreated;
        private void OnTileCreated(Tile tile)
        {
            TileCreated?.Invoke(this, new TileEventArgs(tile));
        }

        private Tile[,] tiles;  

        public World()
        {
            Current = this;
            PlayerCharacterManager = new ClientPlayerCharacterManager();
        }

        public void Initialize(WorldConfiguration worldConfiguration)
        {
            Configuration = worldConfiguration;

            Width = Configuration.Width;
            Height = Configuration.Height;

            tiles = new Tile[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tileAt = new Tile(new Vector2i(x, y));
                    tiles[x, y] = tileAt;
                    OnTileCreated(tileAt);
                }
            }
        }

        public Tile GetTileAt(int x, int y) => GetTileAt(new Vector2i(x, y));
        public Tile GetTileAt(Vector2i position)
        {
            if (position.X < 0 || position.X >= Width || position.Y < 0 || position.Y >= Height) throw new ArgumentOutOfRangeException(position.ToString());
            return tiles[position.X, position.Y];
        }
    }
}
