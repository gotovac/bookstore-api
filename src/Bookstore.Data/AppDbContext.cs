using Bookstore.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Review> Reviews { get; set; }


        public override int SaveChanges()
        {
            NormalizeEntityNames();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            NormalizeEntityNames();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void NormalizeEntityNames()
        {
            foreach (var entry in ChangeTracker.Entries()
                         .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                switch (entry.Entity)
                {
                    case Book b: b.NormalizedTitle = Book.NormalizeTitle(b.Title); break;
                    case Author a: a.NormalizedName = Author.NormalizeName(a.Name); break;
                    case Genre g: g.NormalizedName = Genre.NormalizeName(g.Name); break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexes

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.NormalizedTitle)
                .IsUnique();

            modelBuilder.Entity<Author>()
                .HasIndex(a => a.NormalizedName)
                .IsUnique();

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.NormalizedName)
                .IsUnique();

            // Relationships

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books);

            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired();
            modelBuilder.Entity<Author>().Property(a => a.Name).IsRequired();
            modelBuilder.Entity<Genre>().Property(g => g.Name).IsRequired();
            modelBuilder.Entity<Review>().Property(r => r.Rating).IsRequired();

            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("money");

            modelBuilder.Entity<Book>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }

    }
}
