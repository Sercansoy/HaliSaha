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

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }       
    }
}
