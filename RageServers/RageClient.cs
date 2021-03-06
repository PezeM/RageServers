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
using RageServers.Services.Requests;

namespace RageServers
{
    /// <summary>
    /// Entry point in application
    /// </summary>
    public class RageClient
    {
        private static HtmlRequest _client;
        private readonly string mainUrl = "https://cdn.rage.mp/master/";
        private IRageDatabaseServerService _ravenRageDatabase;
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
        /// <param name="ravenRageDatabase">Raven database service.</param>
        public RageClient(IRageDatabaseServerService ravenRageDatabase, IOptions<AppSettings> appSettings, ILogger<RageClient> logger, HtmlRequest client)
        {
            _logger = logger;
            _ravenRageDatabase = ravenRageDatabase;
            _client = client;
            var clientSettings = appSettings.Value.Configuration;
            DisplayInformation = clientSettings.DisplayInformation;
            ServersToDisplayInformationAbout = clientSettings.ServersToDisplayInformationAbout;

            Interval = clientSettings.Interval;
            _timer = new Timer(Interval);
            _timer.Elapsed += TimerElapsedAsync;
            // GetAllServersAsync();
            //GetPeakPlayersForServer("51.68.154.84:22005");
            //GetPeakPlayersForServerInDateRange("51.68.154.84:22005");
            GetPeakPlayersForServerInDay("51.68.154.84:22005");
        }

        private async Task GetPeakPlayersForServerInDay(string ip)
        {
            var timer = new Stopwatch();
            timer.Start();
            var players = await _ravenRageDatabase.GetPeakPlayersForServerForEachDayAsync(ip);
            timer.Stop();
            _logger.LogInformation($"GetPeakPlayersForServerInDay for {ip} completed in {timer.Elapsed} s.");
            foreach (var peak in players)
            {
                Console.WriteLine($"{peak.Key}: {peak.Value}");
            }
        }

        private void GetPeakPlayersForServerInDateRange(string id)
        {
            var timer = new Stopwatch();
            timer.Start();
            var peak = _ravenRageDatabase.GetPeakPlayersForServerInDateRange(id, new DateTime(2018, 11, 27), DateTime.Now);
            timer.Stop();
            _logger.LogInformation($"GetPeakPlayersForServerInDateRange for {id} completed in {timer.Elapsed} s. Returned {peak} players.");
        }

        private void GetPeakPlayersForServer(string id)
        {
            var timer = new Stopwatch();
            timer.Start();
            var peak = _ravenRageDatabase.GetPeakPlayersForServer(id);
            timer.Stop();
            _logger.LogInformation($"GetPeakPlayersForServer for {id} completed in {timer.Elapsed} s. Returned {peak} players.");
        }

        public async Task GetAllServersAsync()
        {
            var timer = new Stopwatch();
            timer.Start();
            await _ravenRageDatabase.GetAllServersAsync();
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
            await GetServerInfoAsync();
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

        private async Task GetServerInfoAsync()
        {
            // Get all servers info from rage cdn
            var servers = await _client.GetServersAsync();

            if (DisplayInformation)
                DisplayInformations(servers);

            await AddToDatabaseAsync(servers);
        }

        private async Task AddToDatabaseAsync(Dictionary<string, ServerInfo> servers)
        {
            foreach (var serverInfo in servers)
            {
                await _ravenRageDatabase.InsertAsync(serverInfo.Key, serverInfo.Value);
            }
            _logger.LogInformation($"RageClient finished adding {servers.Count} entities to database.");
        }
    }
}
