using LibrarySystem.LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.LibrarySystem.Context
{
    public class LibrariesContext : DbContext
    {
        public LibrariesContext()
        {
            Database.EnsureCreated();
        }

        public LibrariesContext(DbContextOptions<LibrariesContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<Library> Libraries { get; set; } = null!;
        public virtual DbSet<LibraryBook> LibraryBooks { get; set; } = null!;

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
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("books");

                entity.HasIndex(e => e.BookUid, "books_book_uid_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Author)
                    .HasMaxLength(255)
                    .HasColumnName("author");

                entity.Property(e => e.BookUid).HasColumnName("book_uid");

                entity.Property(e => e.Condition)
                    .HasMaxLength(20)
                    .HasColumnName("condition")
                    .HasDefaultValueSql("'EXCELLENT'::character varying");

                entity.Property(e => e.Genre)
                    .HasMaxLength(255)
                    .HasColumnName("genre");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Library>(entity =>
            {
                entity.ToTable("library");

                entity.HasIndex(e => e.LibraryUid, "library_library_uid_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .HasColumnName("city");

                entity.Property(e => e.LibraryUid).HasColumnName("library_uid");

                entity.Property(e => e.Name)
                    .HasMaxLength(80)
                    .HasColumnName("name");
            });


            modelBuilder.Entity<LibraryBook>(entity =>
            {
                entity.HasKey(lb => new {lb.BookId, lb.LibraryId});

                entity.ToTable("library_books");

                entity.Property(e => e.AvailableCount).HasColumnName("available_count");

                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.LibraryId).HasColumnName("library_id");

                entity.HasOne(d => d.Book)
                    .WithMany()
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("library_books_book_id_fkey");

                entity.HasOne(d => d.Library)
                    .WithMany()
                    .HasForeignKey(d => d.LibraryId)
                    .HasConstraintName("library_books_library_id_fkey");
            });
        }
    }
}