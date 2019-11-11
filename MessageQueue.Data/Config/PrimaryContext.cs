using MessageQueue.Data.Mappings;
using MessageQueue.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessageQueue.Data.Config
{
    public class PrimaryContext : DbContext
    {
        public PrimaryContext(DbContextOptions<PrimaryContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TrackerMap());
        }

        public DbSet<TrackerEntity> Tracker { get; set; }
    }
}
