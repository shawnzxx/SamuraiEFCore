using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Contexts
{
    public class SamuraiContext : DbContext
    {
        //we add SamuraiContext to the ASP.net Core IoC container with options, so that we can use it in Startup.cs
        public SamuraiContext(DbContextOptions<SamuraiContext> options) : base(options)
        {

        }
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //explicity tell EF core this is many to many relationship
            modelBuilder.Entity<SamuraiBattle>()
                .HasKey(s => new { s.SamuraiId, s.BattleId });

            //seed the database with dummy data
            modelBuilder.Entity<Samurai>().HasData(
                new Samurai()
                {
                    Id = 1,
                    Name="Shawnzxx"
                },
                new Samurai()
                {
                    Id = 2,
                    Name = "TuZi"
                },
                new Samurai()
                {
                    Id = 3,
                    Name = "BaBa"
                },
                new Samurai()
                {
                    Id = 4,
                    Name = "BabaTu"
                },
                new Samurai()
                {
                    Id = 5,
                    Name = "TuBaba"
                }
                );
            modelBuilder.Entity<Quote>().HasData(
                new Quote()
                {
                    Id = 1,
                    SamuraiId = 1,
                    Text = "I told you watch out my shape sword, ohh well!"
                },
                new Quote()
                {
                    Id = 2,
                    SamuraiId = 1,
                    Text = "I am here to defeat you"
                },
                new Quote()
                {
                    Id = 3,
                    SamuraiId = 1,
                    Text = "I am happy now"
                },
                new Quote()
                {
                    Id = 4,
                    SamuraiId = 2,
                    Text = "Trash, Trash, Trash!!"
                },
                new Quote()
                {
                    Id = 5,
                    SamuraiId = 2,
                    Text = "Come, I will show you the way"
                },
                new Quote()
                {
                    Id = 6,
                    SamuraiId = 3,
                    Text = "Roger that"
                },
                new Quote()
                {
                    Id = 7,
                    SamuraiId = 3,
                    Text = "It's a fun game, isn't it?"
                },
                new Quote()
                {
                    Id = 8,
                    SamuraiId = 4,
                    Text = "Dame it! Dame it!! Dame it!!!"
                },
                new Quote()
                {
                    Id = 9,
                    SamuraiId = 5,
                    Text = "OMG"
                },
                new Quote()
                {
                    Id = 10,
                    SamuraiId = 5,
                    Text = "Let's get it to work now"
                },
                new Quote()
                {
                    Id = 11,
                    SamuraiId = 5,
                    Text = "Shiock! We finaly did that, Let's have a chicken soup tonight"
                }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
