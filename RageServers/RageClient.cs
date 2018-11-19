using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using LiteDB;
using RageServers.Entity;

namespace RageServers
{
    public class RageClient
    {
        private static HttpClient _client = new HttpClient();
        private readonly string mainUrl = "https://cdn.rage.mp/master/";
        private ServersDatabase _serversDb;

        private Timer _timer;
        private double _interval;
        private int _iteration = 1;

        private IEnumerable<string> _serversToDisplayInformationAbout;
        private bool _displayInformation = true;

        public Dictionary<string, ServerInfo> ServerInfos { get; set; }

        public RageClient(string connectionString, IEnumerable<string> serversToDisplayInformationAbout, double interval = 60000, bool displayInformation = true)
        {
            _displayInformation = displayInformation;
            _serversToDisplayInformationAbout = serversToDisplayInformationAbout;

            _serversDb = new ServersDatabase(connectionString);
            ServerInfos = new Dictionary<string, ServerInfo>();

            _interval = interval;
            _timer = new Timer(_interval);
            _timer.Elapsed += TimerElapsedAsync;
            DisplayPeakPlayers();
        }

        private void DisplayPeakPlayers()
        {
            var peakPlayers = _serversDb.GetPeakForAllServers();
            foreach (var server in peakPlayers)
            {
                Console.WriteLine($"{server.Key} had maximum {server.Value} players.");
            }
        }

        public void StartGettingInformation()
        {
            // Return if timer is already enabled
            if (_timer.Enabled)
                return;

            _timer.Enabled = true;
        }

        private async void TimerElapsedAsync(object sender, ElapsedEventArgs e)
        {
            await GetHtmlAsync(mainUrl);
            _iteration++;
        }

        private void DisplayInformations(Dictionary<string, ServerInfo> servers)
        {
            Console.WriteLine($"===================== Iteration: {_iteration} {DateTime.Now} ============================");
            // Display informations only about servers with decleared IP inside _serversToDisplayInformationAbout
            if (_serversToDisplayInformationAbout.Any())
            {
                foreach (var server in servers)
                {
                    if (_serversToDisplayInformationAbout.Contains(server.Key))
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
                var response = await _client.GetStringAsync(mainUrl);
                var servers = JsonService.DeserializeRageServerInfos<string, ServerInfo>(response);

                if (_displayInformation)
                    DisplayInformations(servers);

                //InsertToDatabase(servers);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"EXCEPTION FOUND MESSAGE: {e.Message}");
            }
        }

        private void InsertToDatabase(Dictionary<string, ServerInfo> servers)
        {
            _serversDb.Insert(servers);
        }
    }
}
