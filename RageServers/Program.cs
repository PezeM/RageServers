using System;
using System.IO;
using System.Linq;
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

            GetAppSettings();

            var client = new RageClient(AppSettings.ConnectionString
                , AppSettings.Configuration.ServersToDisplayInformationAbout
                , AppSettings.Configuration.Interval
                , AppSettings.Configuration.DisplayInformation);
            client.StartGettingInformation();

            Console.ReadKey();
        }

        private static void GetAppSettings()
        {
            AppSettings.ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("LiteDb").Value;
            AppSettings.Configuration.Interval = Int32.Parse(Configuration.GetSection("Configuration").GetSection("Interval").Value);
            AppSettings.Configuration.DisplayInformation = Convert.ToBoolean(Configuration.GetSection("Configuration").GetSection("DisplayInformation").Value);

            AppSettings.Configuration.ServersToDisplayInformationAbout = Configuration.GetSection("Configuration")
                .GetSection("ServersToDisplayInformationAbout").GetChildren().Select(x => x.Value).ToList();
        }
    }
}
