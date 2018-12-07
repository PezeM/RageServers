using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RageServers.Database.Service;
using RageServers.Web.Models;

namespace RageServers.Web.Controllers
{
    public class HomeController : Controller
    {
        private RavenRageServerService _ravenRage;
        private ILogger<HomeController> _logger;

        public HomeController(RavenRageServerService ravenRage, ILogger<HomeController> logger)
        {
            _ravenRage = ravenRage;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(string ip)
        {
            var server = _ravenRage.GetServerEntitiesByIpAsync(ip).Result;
            var model = server.FirstOrDefault();

            if (model == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
