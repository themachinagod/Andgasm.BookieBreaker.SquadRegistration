using System.ComponentModel.DataAnnotations;

namespace Andgasm.BookieBreaker.SquadRegistration.API
{
    public class PlayerClubSeasonAssociation
    {
        [Key]
        public string Key
        {
            get
            {
                return $"{ClubKey}-{SeasonKey}-{PlayerKey}";
            }
            set { }
        }
        public string ClubKey { get; set; }
        public string SeasonKey { get; set; }
        public string PlayerKey { get; set; }
    }
}
