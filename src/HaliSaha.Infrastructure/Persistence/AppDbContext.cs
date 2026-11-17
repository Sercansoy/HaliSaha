using HaliSaha.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaliSaha.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Facility> Facilities => Set<Facility>();
        public DbSet<Pitch> Pitches => Set<Pitch>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.FullName).HasMaxLength(100);
                e.Property(u => u.Email).HasMaxLength(254);
            });

            modelBuilder.Entity<Facility>(e =>
            {
                e.Property(f => f.Name).HasMaxLength(100);
                e.Property(f => f.City).HasMaxLength(50);
                e.Property(f => f.District).HasMaxLength(50);
                e.Property(f => f.Address).HasMaxLength(200);
                e.Property(f => f.Phone).HasMaxLength(20);
            });

            modelBuilder.Entity<Pitch>(e =>
            {
                e.Property(p => p.Name).HasMaxLength(100);
            });
        }
    }
}
