﻿using RageServers.Database.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using RageServers.Models;
using Microsoft.Extensions.Options;

namespace RageServers
{
    /// <summary>
    /// Entry point in application
    /// </summary>
    public class RageClient
    {
        private static HttpClient _client = new HttpClient();
        private readonly string mainUrl = "https://cdn.rage.mp/master/";
        private RavenRageServerService _ravenRage;
        private readonly ILogger<RageClient> _logger;
        private Timer _timer;

        /// <summary>
        /// List of servers ip to display information about
        /// </summary>
        public IEnumerable<string> ServersToDisplayInformationAbout { get; private set; }

        /// <summary>
        /// If set to true, will display information about every server from current response 
        /// </summary>
        public bool DisplayInformation { get; private set; }

        /// <summary>
        /// Number of requests
        /// </summary>
        public int Iteration { get; private set; } = 1;

        /// <summary>
        /// Time interval between requests, in ms
        /// </summary>
        public double Interval { get; private set; }

        /// <summary>
        /// Starting point in application
        /// </summary>
        /// <param name="ravenRage">Raven database service.</param>
        public RageClient(RavenRageServerService ravenRage, IOptions<AppSettings> appSettings, ILogger<RageClient> logger)
        {
            _logger = logger;
            _ravenRage = ravenRage;
            var clientSettings = appSettings.Value.Configuration;
            DisplayInformation = clientSettings.DisplayInformation;
            ServersToDisplayInformationAbout = clientSettings.ServersToDisplayInformationAbout;

            Interval = clientSettings.Interval;
            _timer = new Timer(Interval);
            _timer.Elapsed += TimerElapsedAsync;
            // GetAllServersAsync();
            //GetPeakPlayersForServer("51.68.154.84:22005");
            //GetPeakPlayersForServerInDateRange("51.68.154.84:22005");
        }

        private void GetPeakPlayersForServerInDateRange(string id)
        {
            var timer = new Stopwatch();
            timer.Start();
            var peak = _ravenRage.GetPeakPlayersForServerInDateRange(id, new DateTime(2018, 11, 27), DateTime.Now);
            timer.Stop();
            _logger.LogInformation($"GetPeakPlayersForServerInDateRange for {id} completed in {timer.Elapsed} s. Returned {peak} players.");
        }

        private void GetPeakPlayersForServer(string id)
        {
            var timer = new Stopwatch();
            timer.Start();
            var peak = _ravenRage.GetPeakPlayersForServer(id);
            timer.Stop();
            _logger.LogInformation($"GetPeakPlayersForServer for {id} completed in {timer.Elapsed} s. Returned {peak} players.");
        }

        public async Task GetAllServersAsync()
        {
            var timer = new Stopwatch();
            timer.Start();
            await _ravenRage.GetAllServersAsync();
            timer.Stop();
            _logger.LogInformation($"GetAllServers completed in {timer.ElapsedMilliseconds} ms, {timer.Elapsed} s.");
        }

        public void StartGettingInformation()
        {
            _logger.LogInformation("RageClient started getting information.");
            _timer.Enabled = true;
        }

        private async void TimerElapsedAsync(object sender, ElapsedEventArgs e)
        {
            await GetHtmlAsync(mainUrl);
            Iteration++;
        }

        private void DisplayInformations(Dictionary<string, ServerInfo> servers)
        {
            Console.WriteLine($"===================== Iteration: {Iteration} {DateTime.Now} ============================");
            // Display informations only about servers with decleared IP inside ServersToDisplayInformationAbout
            if (ServersToDisplayInformationAbout.Any())
            {
                foreach (var server in servers)
                {
                    if (ServersToDisplayInformationAbout.Contains(server.Key))
                    {
                        Console.WriteLine($"Server name: {server.Value.Name} has {server.Value.Players} players. With peak {server.Value.Peak}.");
                    }
                }
            }
        }

        private async Task GetHtmlAsync(string url)
        {
            try
            {
                var response = await _client.GetStringAsync(url);
                var servers = JsonService.DeserializeRageServerInfos<string, ServerInfo>(response);

                if (DisplayInformation)
                    DisplayInformations(servers);

                await AddToDatabaseAsync(servers);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"HttpRequestException: {e.Message}");
            }
        }

        private async Task AddToDatabaseAsync(Dictionary<string, ServerInfo> servers)
        {
            foreach (var serverInfo in servers)
            {
                await _ravenRage.InsertAsync(serverInfo.Key, serverInfo.Value);
            }
            _logger.LogInformation($"RageClient finished adding {servers.Count} entities to database.");
        }
    }
}
