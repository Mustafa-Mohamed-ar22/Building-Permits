using Building_Permits.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building_Permits.Infrastructure.Config
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications").HasKey(x => x.Id);
            builder.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            builder.Property(e => e.Message).HasMaxLength(500);
            builder.Property(e => e.UserId).HasMaxLength(450);

            builder.HasOne(x => x.Permit).WithMany(x => x.Notifications).HasForeignKey(x => x.PermitId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasOne(d => d.PermitStage).WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.PermitStageId).OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(x => x.applicationUser).WithMany(p => p.Notifications).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
