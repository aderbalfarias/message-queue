using HostApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostApp.Data.Mappings
{
    public class TestMap : IEntityTypeConfiguration<TestEntity>
    {
        public void Configure(EntityTypeBuilder<TestEntity> builder)
        {
            builder.ToTable("Test", "dbo");

            builder.HasKey(k => k.Id);

            builder.Property(p => p.Text)
                .HasMaxLength(5000)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(500)
                .IsRequired();
        }
    }
}
