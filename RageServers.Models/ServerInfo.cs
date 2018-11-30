using System;
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
        public int Players { get; set; }

        [JsonProperty("peak")]
        public int Peak { get; set; }

        [JsonProperty("maxplayers")]
        public int Maxplayers { get; set; }
    }
}
