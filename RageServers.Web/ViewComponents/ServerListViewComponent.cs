using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RageServers.Services.Requests;

namespace RageServers.Web.ViewComponents
{
    public class ServerListViewComponent : ViewComponent
    {
        private HtmlRequest _request;

        public ServerListViewComponent(HtmlRequest request)
        {
            _request = request;
        }

        public IViewComponentResult Invoke()
        {
            var servers = GetOnlineServersAsync().Result.OrderByDescending(x => x.Value.Players).ToDictionary(x => x.Key, x => x.Value);
            return View("Default", servers);
        }

        private async Task<Dictionary<string, ServerInfo>> GetOnlineServersAsync()
        {
            return await _request.GetServersAsync();
        }
    }
}
