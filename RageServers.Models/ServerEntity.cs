using System;
using Newtonsoft.Json;

namespace RageServers.Entity
{
    public class ServerEntity
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string IP { get; set; }
        public DateTime Datetime { get; set; }
        public ServerInfo ServerInfo { get; set; }
    }
}
