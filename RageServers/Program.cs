using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RageServers.Models;

namespace RageServers
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static AppSettings AppSettings { get; set; }

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            var client = new RageClient(AppSettings.ConnectionString
                , new List<string> { "51.136.12.2:22005", "37.59.35.134:22005", "51.68.154.84:22005" }
                , AppSettings.Configuration.Interval
                , displayInformation: false);
            // client.StartGettingInformation();

            Console.ReadKey();
        }

        private static void GetAppSettings()
        {
            AppSettings.ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("LiteDb").Value;
            AppSettings.Configuration.Interval = Int32.Parse(Configuration.GetSection("Configuration").GetSection("Interval").Value);
        }
    }
}
