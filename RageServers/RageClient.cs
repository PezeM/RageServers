using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;

namespace RageServers
{
    public class RageClient
    {
        private readonly string mainUrl = "https://cdn.rage.mp/master/";
        private Timer _timer;
        private double _interval;
        private int _iteration;

        public Dictionary<string, ServerInfo> ServerInfos { get; set; }

        public RageClient(double interval = 60000)
        {
            ServerInfos = new Dictionary<string, ServerInfo>();
            _interval = interval;
            _timer = new Timer(_interval);
            _timer.Elapsed += TimerElapsedAsync;
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
            Console.WriteLine($"===================== Iteration: {_iteration} ============================");
            foreach (var server in servers)
            {
                Console.WriteLine($"Server name: {server.Value.Name} has {server.Value.Players} players. With peak {server.Value.Peak}.");
            }
        }

        private async Task GetHtmlAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var html = await httpClient.GetStringAsync(url);
                ServerInfos = JsonService.DeserializeRageServerInfos<string, ServerInfo>(html);
                DisplayInformations(ServerInfos);
            }
        }
    }
}
