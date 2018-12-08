using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RageServers.Database.Service;
using RageServers.Models;

namespace RageServers
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly AppSettings _appSettings;
        private IRageDatabaseServerService _ravenRageDatabaseServerService;
        private RageClient _rageClient;

        public App(IOptions<AppSettings> appSettings, ILogger<App> logger, IRageDatabaseServerService ravenRageDatabaseServerService, RageClient rageClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _ravenRageDatabaseServerService = ravenRageDatabaseServerService;
            _rageClient = rageClient;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Program started.");
            _rageClient.StartGettingInformation();
            Console.ReadKey();
        }
    }
}
