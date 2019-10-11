using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using Andgasm.BB.Harvest;
using System.Dynamic;
using Andgasm.Http;

namespace Andgasm.BB.SquadRegistration.Core
{
    public class SquadRegistrationHarvester : DataHarvest
    {
        #region Fields
        ILogger<SquadRegistrationHarvester> _logger;
        ApiSettings _apisettings;

        string _playersapiroot;
        string _registrationsApiPath;
        #endregion

        #region Properties
        public string StageCode { get; set; }
        public string SeasonCode { get; set; }
        public string ClubCode { get; set; }
        #endregion

        #region Contructors
        public SquadRegistrationHarvester(ApiSettings settings, ILogger<SquadRegistrationHarvester> logger, HarvestRequestManager requestmanager)
        {
            _logger = logger;
            _requestmanager = requestmanager;

            _playersapiroot = settings.PlayersDbApiRootKey;
            _registrationsApiPath = settings.PlayerSquadRegistrationsApiPath;
            _apisettings = settings;
        }
        #endregion

        #region Execution Operations
        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            if (string.IsNullOrWhiteSpace(StageCode)) return false;
            if (string.IsNullOrWhiteSpace(SeasonCode)) return false;
            if (string.IsNullOrWhiteSpace(ClubCode)) return false;
            return true;
        }

        public async override Task Execute()
        {
            if (CanExecute())
            {
                _timer.Start();
                var lastmodekey = await DetermineLastModeKey();
                HtmlDocument responsedoc = await ExecuteRequest(lastmodekey);
                if (responsedoc != null)
                {
                    var players = new List<ExpandoObject>();
                    foreach (var pl in ParsePlayersFromResponse(responsedoc))
                    {
                        players.Add(CreateSquadPlayer(pl));
                    };
                    await HttpRequestFactory.Post(players, _playersapiroot, _registrationsApiPath);
                    _logger.LogDebug(string.Format("Stored new player data to database for club key '{0}' and season key '{1}'", ClubCode, SeasonCode));
                }
                else
                {
                    _logger.LogDebug(string.Format("Failed to store & commit player squad registration for season '{0}' in data store.", SeasonCode));
                }
                HarvestHelper.FinaliseTimer(_timer);
            };
        }
        #endregion

        #region Entity Creation Helpers
        private string CreateRequestUrl()
        {
            return string.Format(WhoScoredConstants.PlayerStatisticsFeedUrl, StageCode, ClubCode);
        }

        private string CreateRefererUrl()
        {
            return string.Format(WhoScoredConstants.ClubsUrl, ClubCode);
        }

        private async Task<string> DetermineLastModeKey()
        {
            var referer = CreateRefererUrl();
            var ctx = HarvestHelper.ConstructRequestContext(null, "text/html, application/xhtml+xml, image/jxr, */*", WhoScoredConstants.RootUrl,
                                                            CookieString,
                                                            "en-GB,en;q=0.9,en-US;q=0.8,th;q=0.7", false, false, true);
            var parentresponse = await _requestmanager.MakeRequest(referer, ctx);
            if (parentresponse != null)
            {
                var r =  GetLastModeKey(parentresponse.DocumentNode.InnerHtml);
                return r;
            }
            return null;
        }

        private async Task<HtmlDocument> ExecuteRequest(string lastmodekey)
        {
            var url = CreateRequestUrl();
            var referer = CreateRefererUrl();
            var ctx = HarvestHelper.ConstructRequestContext(lastmodekey, "application/json,text/javascript,*/*; q=0.01", referer,
                                                           CookieString,
                                                            "en-GB", true, false, false);
            var p = await _requestmanager.MakeRequest(url, ctx);
            CookieString = ctx.Cookies["Cookie"];
            return p;
        }

        private JArray ParsePlayersFromResponse(HtmlDocument response)
        {
            var rawdata = response.DocumentNode.InnerHtml;
            var startIndex = rawdata.IndexOf("{     \"playerTableStats\" : ");
            var endIndex = rawdata.IndexOf(",  \"paging\"");
            rawdata = rawdata.Substring(startIndex + 26, (endIndex - (startIndex + 26)));
            return JsonConvert.DeserializeObject<JArray>(rawdata);
        }

        private ExpandoObject CreateSquadPlayer(JToken playerdata)
        {
            dynamic player = new ExpandoObject();
            player.Name = playerdata["firstName"].ToString();
            player.Surname = playerdata["lastName"].ToString();
            player.CountryKey = playerdata["regionCode"].ToString();
            player.PlayerCode = playerdata["playerId"].ToString();
            player.Height = playerdata["height"].ToString();
            player.Weight = playerdata["weight"].ToString();
            player.Positions = playerdata["playedPositionsShort"].ToString();
            player.ClubKey = ClubCode;
            player.SeasonKey = SeasonCode;
            return player;
        }
        #endregion
    }
}
