using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SamuraiModel
{
    public partial class SamuraiappdatacoreContext : DbContext
    {
        public SamuraiappdatacoreContext()
        {
        }

        public SamuraiappdatacoreContext(DbContextOptions<SamuraiappdatacoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Battles> Battles { get; set; }
        public virtual DbSet<Quotes> Quotes { get; set; }
        public virtual DbSet<SamuraiBattle> SamuraiBattle { get; set; }
        public virtual DbSet<Samurais> Samurais { get; set; }
        public virtual DbSet<SecretIdentity> SecretIdentity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Quotes>(entity =>
            {
                entity.HasIndex(e => e.SamuraiId);

                entity.HasOne(d => d.Samurai)
                    .WithMany(p => p.Quotes)
                    .HasForeignKey(d => d.SamuraiId);
            });

            modelBuilder.Entity<SamuraiBattle>(entity =>
            {
                entity.HasKey(e => new { e.SamuraiId, e.BattleId });

                entity.HasIndex(e => e.BattleId);

                entity.HasOne(d => d.Battle)
                    .WithMany(p => p.SamuraiBattle)
                    .HasForeignKey(d => d.BattleId);

                entity.HasOne(d => d.Samurai)
                    .WithMany(p => p.SamuraiBattle)
                    .HasForeignKey(d => d.SamuraiId);
            });

            modelBuilder.Entity<SecretIdentity>(entity =>
            {
                entity.HasIndex(e => e.SamuraiId)
                    .IsUnique();

                entity.HasOne(d => d.Samurai)
                    .WithOne(p => p.SecretIdentity)
                    .HasForeignKey<SecretIdentity>(d => d.SamuraiId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}