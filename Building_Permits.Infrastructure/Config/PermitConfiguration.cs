using Building_Permits.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building_Permits.Infrastructure.Config
{
    public class PermitConfiguration : IEntityTypeConfiguration<Permit>
    {
        public void Configure(EntityTypeBuilder<Permit> builder)
        {
            builder.ToTable("Permits").HasKey(x => x.Id);
            builder.Property(e => e.Status).HasConversion<string>();
            builder.Property(e => e.ArchivedAt).HasColumnType("datetime");
            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            builder.Property(x => x.PermitType).HasColumnType("NVARCHAR").HasMaxLength(60);

            builder.HasOne(x => x.Client).WithOne(x => x.Permit).
                HasForeignKey<Client>(x => x.PermitId).OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(x=>x.ApplicationUser).WithMany(x=>x.Permits).HasForeignKey(x=>x.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
