using System;
using System.Collections.Generic;

namespace RageServers.Web.ViewModels
{
    public class HomeDetailsViewModel
    {
        public string IP { get; set; }
        public Dictionary<DateTime, int> PeakPlayers { get; set; }
        //public List<List<double>> PeakPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public int Slots { get; set; }
        public string Lang { get; set; }
        public string Gamemode { get; set; }
    }
}
