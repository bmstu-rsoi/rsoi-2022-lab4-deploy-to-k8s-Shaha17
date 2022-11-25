using LibrarySystem.RatingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.RatingSystem.Context
{
    public class RatingsContext : DbContext
    {
        public RatingsContext()
        {
            Database.EnsureCreated();
        }

        public RatingsContext(DbContextOptions<RatingsContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Rating> Ratings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var databaseHost = Environment.GetEnvironmentVariable("DB_HOST");
                var databasePort = Environment.GetEnvironmentVariable("DB_PORT");
                var database = Environment.GetEnvironmentVariable("DATABASE");
                var username = Environment.GetEnvironmentVariable("USERNAME");
                var password = Environment.GetEnvironmentVariable("PASSWORD");
                optionsBuilder.UseNpgsql(
                    $"Host={databaseHost};Port={databasePort};Database={database};Username={username};Password={password}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("rating");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Stars).HasColumnName("stars");

                entity.Property(e => e.Username)
                    .HasMaxLength(80)
                    .HasColumnName("username");
            });
        }
    }
}