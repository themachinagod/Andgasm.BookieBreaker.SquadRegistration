using System;
using System.Threading.Tasks;

namespace Andgasm.BookieBreaker.SquadRegistration.Extractor.Svc
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "SeasonParticipantExtractor.Svc";
            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            Console.Title = "SeasonParticipantExtractor.Svc";
            var boot = new Startup();
            await boot.Host.RunConsoleAsync();
        }
    }
}
