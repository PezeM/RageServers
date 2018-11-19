using System;

namespace RageServers.Entity
{
    public class ServerEntity
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public DateTime Datetime { get; set; }
        //public string Name { get; set; }
        //public string Gamemode { get; set; }
        //public string Url { get; set; }
        //public string Lang { get; set; }
        //public int Players { get; set; }
        //public int Peak { get; set; }
        //public int Maxplayers { get; set; }
        public ServerInfo ServerInfo { get; set; }
    }
}
