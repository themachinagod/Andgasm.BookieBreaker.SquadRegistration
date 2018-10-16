using Andgasm.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Andgasm.BookieBreaker.SeasonParticipant.Core
{
    public class SquadRegistrationExtractorSvc : IHostedService
    {
        static ILogger<SquadRegistrationExtractorSvc> _logger;
        static IBusClient _newClubSeasonAssociationBus;
        static SquadRegistrationHarvester _harvester;

        public SquadRegistrationExtractorSvc(ILogger<SquadRegistrationExtractorSvc> logger, IBusClient newClubSeasonBus, SquadRegistrationHarvester harvester)
        {
            _harvester = harvester;
            _logger = logger;
            _newClubSeasonAssociationBus = newClubSeasonBus;
        }

        public void Run()
        {
            //using (var init = new CookieInitialiser(FiddlerVersion.Fiddler2))
            //{
            //    init.Execute();
            //    _harvester.CookieString = init.RealisedCookie;
            //}

            _logger.LogDebug("SquadRegistrationExtractor.Svc is registering to new season participant events...");
            _newClubSeasonAssociationBus.RecieveEvents(ExceptionReceivedHandler, ProcessMessagesAsync);
            _logger.LogDebug("SquadRegistrationExtractor.Svc is now listening for new season participant events...");
        }

        static async Task ProcessMessagesAsync(IBusEvent message, CancellationToken c)
        {
            var payload = Encoding.UTF8.GetString(message.Body);
            _logger.LogDebug($"Received message: Body:{payload}");

            dynamic payloadvalues = JsonConvert.DeserializeObject<ExpandoObject>(payload);
            _harvester.ClubKey = payloadvalues["clubkey"];
            _harvester.SeasonKey = payloadvalues["seasonkey"];
            _harvester.StageCode = payloadvalues["stagecode"];
            _harvester.ClubSeasonCode = payloadvalues["clubseasoncode"];
            await _harvester.Execute();
            await _newClubSeasonAssociationBus.CompleteEvent(message.LockToken);
        }

        static Task ExceptionReceivedHandler(IExceptionArgs exceptionReceivedEventArgs)
        {
            _logger.LogDebug($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.Exception;
            _logger.LogDebug("Exception context for troubleshooting:");
            _logger.LogDebug($"- Message: {context.Message}");
            _logger.LogDebug($"- Stack: {context.StackTrace}");
            _logger.LogDebug($"- Source: {context.Source}");
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            _logger.LogDebug("SquadRegistrationExtractor.Svc is closing...");
            await _newClubSeasonAssociationBus.Close();
            _logger.LogDebug("SquadRegistrationExtractor.Svc has successfully shut down...");
        }

        // scratch code to manually invoke new season - invoke from startasync to debug without bus
        //public static BusEventBase BuildNewSeasonEvent(string tournamentcode, string seasoncode, string stagecode, string regioncode, string countrycode)
        //{
        //    dynamic jsonpayload = new ExpandoObject();
        //    jsonpayload.TournamentCode = tournamentcode;
        //    jsonpayload.SeasonCode = seasoncode;
        //    jsonpayload.StageCode = stagecode;
        //    jsonpayload.RegionCode = regioncode;
        //    jsonpayload.CountryCode = countrycode;
        //    var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonpayload));
        //    return new BusEventBase(payload);
        //}
    }
}
