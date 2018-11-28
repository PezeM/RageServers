using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RageServers.Database;
using RageServers.Database.Service;
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
            services.AddTransient<RavenRageServerService, RavenRageServerService>();
            services.AddSingleton<RageClient, RageClient>();

            // add app
            services.AddTransient<App>();
        }
    }
}
