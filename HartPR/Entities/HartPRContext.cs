using Microsoft.EntityFrameworkCore;

namespace HartPR.Entities
{
    public class HartPRContext : DbContext
    {
        public HartPRContext(DbContextOptions<HartPRContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Player> Players { get; set; }

        public DbSet<Tournament> Tournaments { get; set; }
    }
}
