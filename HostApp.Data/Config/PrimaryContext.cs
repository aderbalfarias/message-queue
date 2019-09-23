using HostApp.Data.Mappings;
using HostApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HostApp.Data.Config
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
            modelBuilder.ApplyConfiguration(new TestMap());
        }

        public DbSet<TestEntity> Test { get; set; }
    }
}
