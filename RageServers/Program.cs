using System;
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
            var client = new RageClient(2000);
            client.StartGettingInformation();
            Console.ReadKey();
        }
    }
}
