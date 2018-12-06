using System.Collections.Generic;
using RageServers.Entity;

namespace RageServers.Web.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ServerEntity> Servers { get; set; }
    }
}
