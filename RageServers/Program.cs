using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RageServers
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            var url = "https://cdn.rage.mp/master/";
            await GetHtmlAsync(url);
            Console.ReadKey();
            Console.WriteLine("Main async");
        }

        private static void DisplayInformations(Dictionary<string, ServerInfo> servers)
        {
            foreach (var server in servers)
            {
                Console.WriteLine($"Server name: {server.Value.Name} has {server.Value.Players} players. With peak {server.Value.Peak}.");
            }
        }

        public static async Task GetHtmlAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var html = await httpClient.GetStringAsync(url);
                var json = FromJson(html);
                DisplayInformations(json);
            }
        }

        public static Dictionary<string, ServerInfo> FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, ServerInfo>>(json);
        }
    }
}
