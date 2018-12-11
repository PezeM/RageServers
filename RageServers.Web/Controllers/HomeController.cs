﻿using System.Diagnostics;
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
        private IRageDatabaseServerService _ravenRageDatabase;

        public HomeController(IRageDatabaseServerService ravenRageDatabase)
        {
            _ravenRageDatabase = ravenRageDatabase;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
