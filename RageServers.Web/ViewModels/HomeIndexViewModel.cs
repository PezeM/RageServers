using System.Collections.Generic;
using RageServers.Models;

namespace RageServers.Web.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ServerEntity> Servers { get; set; }
    }
}
