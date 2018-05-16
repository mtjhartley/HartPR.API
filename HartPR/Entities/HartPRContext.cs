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

        public DbSet<Set> Sets { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<TrueskillHistory> TrueskillHistories { get; set; }
    }
}
