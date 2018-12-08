using System;
using System.Collections.Generic;

namespace RageServers.Web.ViewModels
{
    public class HomeDetailsViewModel
    {
        public Dictionary<DateTime, int> PeakPlayers { get; set; }
        public string IP { get; set; }
        public int CurrentPlayers { get; set; }
        public int Slots { get; set; }
        public string Lang { get; set; }
        public string Gamemode { get; set; }
    }
}
