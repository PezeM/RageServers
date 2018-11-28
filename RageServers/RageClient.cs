using RageServers.Database.Service;
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
        private readonly RavenRageServerService _ravenRage;
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
        /// Starting point for application
        /// </summary>
        /// <param name="connectionString">Connection string for database.</param>
        /// <param name="serversToDisplayInformationAbout">List of servers to diplay informations about</param>
        /// <param name="interval">Time interval between requests</param>
        /// <param name="displayInformation">Set to true if allowed to display informations about servers</param>
        public RageClient(RavenRageServerService ravenRage, IEnumerable<string> serversToDisplayInformationAbout,
            double interval = 60000, bool displayInformation = true)
        {
            DisplayInformation = displayInformation;
            ServersToDisplayInformationAbout = serversToDisplayInformationAbout;
            _ravenRage = ravenRage;

            Interval = interval;
            _timer = new Timer(Interval);
            _timer.Elapsed += TimerElapsedAsync;
            //ShowPeakPlayers();
            //DisplayPeakPlayers();
        }

        //private void ShowPeakPlayers()
        //{
        //    var timer = new Stopwatch();
        //    timer.Start();
        //    var peak = _serversDb.GetPeakPlayersForServerInDateRange("51.68.154.84:22005", new DateTime(2018, 11, 22), DateTime.Now);
        //    timer.Stop();
        //    Console.WriteLine($"ShowPeakPlayers completed in {timer.ElapsedMilliseconds} ms, {timer.Elapsed}");
        //}

        //private void DisplayPeakPlayers()
        //{
        //    var timer = new Stopwatch();
        //    timer.Start();
        //    var peakPlayers = _serversDb.GetPeakForAllServers();
        //    timer.Stop();
        //    Console.WriteLine($"DisplayPeakPlayers completed in {timer.ElapsedMilliseconds} ms, {timer.Elapsed}");
        //    //foreach (var server in peakPlayers)
        //    //{
        //    //    Console.WriteLine($"{server.Key} had maximum {server.Value} players.");
        //    //}
        //}

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

                await AddToDatabaseAsync(servers);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"EXCEPTION FOUND MESSAGE: {e.Message}");
            }
        }

        private async Task AddToDatabaseAsync(Dictionary<string, ServerInfo> servers)
        {
            foreach (var serverInfo in servers)
            {
                await _ravenRage.InsertAsync(serverInfo.Key, serverInfo.Value);
            }
        }
    }
}
