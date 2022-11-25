using LibrarySystem.ReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.ReservationSystem.Context
{
    public class ReservationsContext : DbContext
    {
        public ReservationsContext()
        {
            Database.EnsureCreated();
        }

        public ReservationsContext(DbContextOptions<ReservationsContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Reservation> Reservations { get; set; } = null!;

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
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("reservation");

                entity.HasIndex(e => e.ReservationUid, "reservation_reservation_uid_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BookUid).HasColumnName("book_uid");

                entity.Property(e => e.LibraryUid).HasColumnName("library_uid");

                entity.Property(e => e.ReservationUid).HasColumnName("reservation_uid");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("status");

                entity.Property(e => e.TillDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("till_date");

                entity.Property(e => e.Username)
                    .HasMaxLength(80)
                    .HasColumnName("username");
            });
        }
    }
}