using MessageQueue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageQueue.Data.Mappings
{
    public class TrackerMap : IEntityTypeConfiguration<TrackerEntity>
    {
        public void Configure(EntityTypeBuilder<TrackerEntity> builder)
        {
            builder.ToTable("Tracker", "dbo");

            builder.HasKey(k => k.Id);

            builder.Property(p => p.ProjectName)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
