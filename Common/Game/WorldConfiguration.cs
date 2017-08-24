using Newtonsoft.Json;

namespace GlumOrigins.Common.Game
{
    public class WorldConfiguration
    {
        private static WorldConfiguration worldConfiguration;

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }
}
