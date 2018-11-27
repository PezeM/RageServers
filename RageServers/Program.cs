using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RageServers.Database;
using RageServers.Models;

namespace RageServers
{
    internal class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static AppSettings AppSettings { get; set; } = new AppSettings();

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            // create service collection
            var services = new ServiceCollection();
            ConfigureServices(services);

            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            // entry to run app
            await serviceProvider.GetService<App>().RunAsync();

            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // add logging
            services.AddSingleton(new LoggerFactory().AddConsole().AddDebug());
            services.AddLogging();

            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddOptions();
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            // Configure services
            services.AddSingleton<IDocumentStoreHolder, DocumentStoreHolder>();

            // add app
            services.AddTransient<App>();
        }

        //public static async Task MainAsync2(string[] args)
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json");

        //    Configuration = builder.Build();
        //    AppSettings.Configuration = new Configuration();
        //    GetAppSettings();

        //    var client = new RageClient(AppSettings.ConnectionString
        //        , AppSettings.Configuration.ServersToDisplayInformationAbout
        //        , AppSettings.Configuration.Interval
        //        , AppSettings.Configuration.DisplayInformation);
        //    client.StartGettingInformationAsync();

        //    Console.ReadKey();
        //}

        //private static void GetAppSettings()
        //{
        //    AppSettings.ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("LiteDb").Value;
        //    AppSettings.Configuration.Interval = Int32.Parse(Configuration.GetSection("Configuration").GetSection("Interval").Value);
        //    AppSettings.Configuration.DisplayInformation = Convert.ToBoolean(Configuration.GetSection("Configuration").GetSection("DisplayInformation").Value);

        //    AppSettings.Configuration.ServersToDisplayInformationAbout = Configuration.GetSection("Configuration")
        //        .GetSection("ServersToDisplayInformationAbout").GetChildren().Select(x => x.Value).ToList();
        //}
    }
}
