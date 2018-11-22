using System.Collections.Generic;

namespace RageServers.Models
{
    public class Configuration
    {
        /// <summary>
        /// Time interval between request in ms, default: 60000 ms = 1min
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// If set to true, will display information about servers from http response
        /// </summary>
        public bool DisplayInformation { get; set; }

        /// <summary>
        /// List of servers ip to display information about
        /// </summary>
        public List<string> ServersToDisplayInformationAbout { get; set; }
    }
}
