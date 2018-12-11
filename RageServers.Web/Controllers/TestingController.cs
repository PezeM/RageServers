using Microsoft.AspNetCore.Mvc;
using RageServers.Database.Service;
using RageServers.Web.ViewModels;

namespace RageServers.Web.Controllers
{
    public class TestingController : Controller
    {
        private IRageDatabaseServerService _ravenRageDatabase;

        public TestingController(IRageDatabaseServerService ravenRageDatabase)
        {
            _ravenRageDatabase = ravenRageDatabase;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(string ip, int currentPlayers, int slots, string lang, string gamemode)
        {
            var peakPlayers = _ravenRageDatabase.GetPeakPlayersForServerForEachDayAsync(ip).Result;

            if (peakPlayers == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new HomeDetailsViewModel
            {
                PeakPlayers = peakPlayers,
                IP = ip,
                Gamemode = gamemode,
                Lang = lang,
                CurrentPlayers = currentPlayers,
                Slots = slots
            };

            return PartialView("_TestingList", model);
        }
    }
}