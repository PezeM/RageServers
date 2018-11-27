using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RageServers.Models;

namespace RageServers
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly AppSettings _appSettings;

        public App(IOptions<AppSettings> appSettings, ILogger<App> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Program started.");
            Console.ReadKey();
            //var client = new RageClient(_appSettings.RavenSettings.Url
            //    , _appSettings.Configuration.ServersToDisplayInformationAbout
            //    , _appSettings.Configuration.Interval
            //    , _appSettings.Configuration.DisplayInformation);
            //await client.StartGettingInformationAsync();
        }
    }
}
