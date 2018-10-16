using Microsoft.EntityFrameworkCore;

namespace Andgasm.BookieBreaker.SquadRegistration.API
{
    public class SquadRegistrationDb : DbContext
    {
        public SquadRegistrationDb() : base()
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerClubSeasonAssociation> PlayerClubSeasonAssociations { get; set; }
        
        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}