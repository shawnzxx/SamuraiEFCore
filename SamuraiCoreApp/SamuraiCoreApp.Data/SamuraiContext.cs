using Microsoft.EntityFrameworkCore;
using SaumraiCoreApp.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace SamuraiCoreApp.Data
{
    public class SamuraiContext : DbContext
    {
        //we can let webapp to determing which provider and connectiong string to use
        public SamuraiContext(DbContextOptions<SamuraiContext> options) : base(options)
        {

        }
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }

        //explicity tell EF core this is many to many relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<SamuraiBattle>()
                .HasKey(s => new { s.SamuraiId, s.BattleId });
        }
    }
}
