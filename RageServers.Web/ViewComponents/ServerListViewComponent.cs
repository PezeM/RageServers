using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RageServers.Web.ViewComponents
{
    public class ServerListViewComponent : ViewComponent
    {
        private static HttpClient _client = new HttpClient();
        private ILogger<ServerListViewComponent> _logger;

        public ServerListViewComponent(ILogger<ServerListViewComponent> logger)
        {
            _logger = logger;
        }

        public IViewComponentResult Invoke()
        {
            var servers = GetServersAsync().Result.OrderByDescending(x => x.Value.Players).ToDictionary(x => x.Key, x => x.Value);
            return View("Default", servers);
        }

        private async Task<Dictionary<string, ServerInfo>> GetServersAsync()
        {
            try
            {
                var response = await _client.GetStringAsync("https://cdn.rage.mp/master/");
                return JsonService.DeserializeRageServerInfos<string, ServerInfo>(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"HttpRequestException: {e.Message}");
                return null;
            }
        }
    }
}
