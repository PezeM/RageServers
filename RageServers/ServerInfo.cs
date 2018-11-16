using Newtonsoft.Json;

namespace RageServers
{
    public class ServerInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("gamemode")]
        public string Gamemode { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("players")]
        public long Players { get; set; }

        [JsonProperty("peak")]
        public long Peak { get; set; }

        [JsonProperty("maxplayers")]
        public long Maxplayers { get; set; }
    }
}
