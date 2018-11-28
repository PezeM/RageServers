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
        private RavenRageServerService _ravenRageServerService;
        private RageClient _rageClient;

        public App(IOptions<AppSettings> appSettings, ILogger<App> logger, RavenRageServerService ravenRageServerService, RageClient rageClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _ravenRageServerService = ravenRageServerService;
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
