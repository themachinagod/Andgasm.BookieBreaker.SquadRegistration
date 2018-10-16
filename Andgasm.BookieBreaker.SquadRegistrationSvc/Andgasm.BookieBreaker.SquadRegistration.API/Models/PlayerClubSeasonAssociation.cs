using System.ComponentModel.DataAnnotations;

namespace Andgasm.BookieBreaker.SquadRegistration.API
{
    public class PlayerClubSeasonAssociation
    {
        [Key]
        public string ClubKey { get; set; }

        [Key]
        public string SeasonKey { get; set; }

        [Key]
        public string PlayerKey { get; set; }
    }
}
