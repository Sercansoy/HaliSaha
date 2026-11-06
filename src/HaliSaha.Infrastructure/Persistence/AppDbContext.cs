using HaliSaha.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaliSaha.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Pitch> Pitches { get; set; }
    }
}
