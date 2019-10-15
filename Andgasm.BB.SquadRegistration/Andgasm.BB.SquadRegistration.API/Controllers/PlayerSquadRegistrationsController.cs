using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Andgasm.BB.SquadRegistration.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerSquadRegistrationsController : Controller
    {
        #region Fields
        SquadRegistrationDb _context;
        ILogger _logger;
        #endregion

        #region Constructors
        public PlayerSquadRegistrationsController(SquadRegistrationDb context, ILogger<PlayerSquadRegistrationsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        [HttpGet(Name = "GetAllPlayers")]
        public async Task<IActionResult> GetAll()
        {
            var d = await _context.Players.Select(x => new PlayerSquadRegistrationModel()
            {
                Surname = x.Surname,
                Name = x.Name,
                DoB = x.DoB,
                Height = x.Height,
                Weight = x.Weight,
                CountryKey = x.CountryKey,
                PlayerKey = x.Key,
                Positions = x.Positions,
            }).ToListAsync();
            return Ok(d);
        }

        [HttpPost(Name = "CreatePlayerSquadRegistration")]
        public async Task<IActionResult> Create([FromBody]List<PlayerSquadRegistrationModel> model)
        {
            try
            {
                bool dochange = false;
                foreach (var p in model)
                {
                    if (!_context.Players.Any(x => x.Key == p.PlayerKey))
                    {
                        dochange = true;
                        var player = new Player()
                        {
                            Key = p.PlayerKey,
                            Name = p.Name,
                            Surname = p.Surname,
                            DoB = p.DoB,
                            Height = p.Height,
                            Weight = p.Weight,
                            Positions = p.Positions,
                            CountryKey = p.CountryKey,
                        };
                        _context.Players.Add(player);
                    }
                    if (!_context.PlayerClubSeasonAssociations.Any(x => x.ClubKey == p.ClubKey && 
                                                                    x.SeasonKey == p.SeasonKey && 
                                                                    x.PlayerKey == p.PlayerKey))
                    {
                        dochange = true;
                        var association = new PlayerClubSeasonAssociation()
                        {
                            ClubKey = p.ClubKey,
                            SeasonKey = p.SeasonKey,
                            PlayerKey = p.PlayerKey,
                        };
                        _context.PlayerClubSeasonAssociations.Add(association);
                    }
                }
                if (dochange) await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException pkex)
            {
                // TODO: we are seeing this occaisionally due to async processing from multiple instances
                //       its ok to swallow as we dont support data updates and if the key exists there is no need for dupe store
                return Conflict($"A primary key violation occured while saving player data: { pkex.Message }");
            }
        }
    }
}
