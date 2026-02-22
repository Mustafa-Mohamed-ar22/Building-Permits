using Building_Permits.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building_Permits.Infrastructure.Config
{
    public class PermitStageConfiguration : IEntityTypeConfiguration<PermitStage>
    {
        public void Configure(EntityTypeBuilder<PermitStage> builder)
        {
            builder.ToTable("PermitStages").HasKey(x => x.Id);

            builder.Property(e => e.AttachmentPath).HasMaxLength(500);
            builder.Property(e => e.CompletedAt).HasColumnType("datetime");
            builder.Property(e => e.DueAt).HasColumnType("datetime");
            builder.Property(e => e.StartedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            builder.Property(e => e.TransactionNumber)
                .HasMaxLength(50)
                .HasColumnName("Transaction_Number");
            builder.HasOne(d => d.Permit).WithMany(p => p.PermitStages)
            .HasForeignKey(d => d.PermitId)
            .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Stage).WithMany(p => p.PermitStages)
            .HasForeignKey(d => d.StageId)
            .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
  