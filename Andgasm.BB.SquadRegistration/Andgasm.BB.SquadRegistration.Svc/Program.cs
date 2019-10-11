using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Andgasm.BB.SquadRegistration.Svc
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "SquadRegistrationExtractor.Svc";
            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            Console.Title = "SquadRegistrationExtractor.Svc";
            var boot = new Startup();
            await boot.Host.RunConsoleAsync();
        }
    }
}
