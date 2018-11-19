using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            var connectionString = configuration.GetSection("ConnectionStrings").GetSection("LiteDb").Value;
            var client = new RageClient(connectionString
                , new List<string> { "51.136.12.2:22005", "37.59.35.134:22005", "51.68.154.84:22005" }
                , 2000,
                displayInformation: false);
            // client.StartGettingInformation();

            Console.ReadKey();
        }
    }
}
