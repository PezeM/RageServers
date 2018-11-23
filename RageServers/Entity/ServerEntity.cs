using System;

namespace RageServers.Entity
{
    public class ServerEntity
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public DateTime Datetime { get; set; }
        public ServerInfo ServerInfo { get; set; }
    }
}
