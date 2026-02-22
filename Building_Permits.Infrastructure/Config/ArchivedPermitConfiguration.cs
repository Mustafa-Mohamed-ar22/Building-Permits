using Building_Permits.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building_Permits.Infrastructure.Config
{
    public class ArchivedPermitConfiguration : IEntityTypeConfiguration<ArchivedPermit>
    {
        public void Configure(EntityTypeBuilder<ArchivedPermit> builder)
        {
            builder
            .HasNoKey()
            .ToView("ArchivedPermits");

            builder.Property(e => e.ArchivedAt).HasColumnType("datetime");
            builder.Property(e => e.CreatedAt).HasColumnType("datetime");
            builder.Property(e => e.PermitId).ValueGeneratedOnAdd();
            builder.Property(e => e.Status).HasMaxLength(50);
            builder.Property(e => e.Title).HasMaxLength(200);
        }
    }
}
 