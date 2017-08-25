using System.IO;
using GlumOrigins.Common.Game;
using Newtonsoft.Json;
using UnityUtilities.Math;
using GlumOrigins.Common.Logging;
using GlumOrigins.Server.Managers;

namespace GlumOrigins.Server
{
    public class NetworkWorld
    {
        private WorldConfiguration configuration;
        public WorldConfiguration Configuration
        {
            get
            {
                if (configuration != null) return configuration;

                string filePath = Path.Combine("Data", "World.config.json");
                Logger.Assert(File.Exists(filePath), "Could not find world configuration file! Configuration file path: \"./Data/World.config.json\"");

                configuration = JsonConvert.DeserializeObject<WorldConfiguration>(File.ReadAllText(filePath));
                return configuration;
            }
        }

        private readonly Tile[,] tiles;
        private readonly int width;
        private readonly int height;

        private readonly ServerPlayerCharacterManager playerCharacterManager;

        public NetworkWorld()
        {
            width = Configuration.Width;
            height = Configuration.Height;

            tiles = new Tile[width, height];
            playerCharacterManager = new ServerPlayerCharacterManager();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = new Tile(new Vector2i(x, y));
                }
            }
        }
    }
}
