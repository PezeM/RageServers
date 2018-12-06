using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RageServers.Database.Service;
using RageServers.Web.Models;
using RageServers.Web.ViewModels;

namespace RageServers.Web.Controllers
{
    public class HomeController : Controller
    {
        private RavenRageServerService _ravenRage;
        private ILogger<HomeController> _logger;
        private static HttpClient _client = new HttpClient();

        public HomeController(RavenRageServerService ravenRage, ILogger<HomeController> logger)
        {
            _ravenRage = ravenRage;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new HomeIndexViewModel();
            model.Servers = GetServersAsync().Result;
            return View(model);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
