using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace RageServers
{
    public class RageClient
    {
        private static HttpClient _client = new HttpClient();
        private readonly string mainUrl = "https://cdn.rage.mp/master/";
        private ServersDatabase _serversDb;

        private Timer _timer;
        private double _interval;
        public int Iteration { get; private set; } = 1;

        public IEnumerable<string> ServersToDisplayInformationAbout { get; private set; }
        public Dictionary<string, ServerInfo> ServerInfos { get; set; }
        public bool DisplayInformation { get; private set; }

        public RageClient(string connectionString, IEnumerable<string> serversToDisplayInformationAbout,
            double interval = 60000, bool displayInformation = true)
        {
            DisplayInformation = displayInformation;
            ServersToDisplayInformationAbout = serversToDisplayInformationAbout;

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
            //foreach (var server in peakPlayers)
            //{
            //    Console.WriteLine($"{server.Key} had maximum {server.Value} players.");
            //}
        }

        public void StartGettingInformation()
        {
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
                var response = await _client.GetStringAsync(mainUrl);
                var servers = JsonService.DeserializeRageServerInfos<string, ServerInfo>(response);

                if (DisplayInformation)
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
