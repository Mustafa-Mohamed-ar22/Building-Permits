using Building_Permits.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building_Permits.Infrastructure.Config
{
	public class CostConfiguraion : IEntityTypeConfiguration<Cost>
	{
		public void Configure(EntityTypeBuilder<Cost> builder)
		{
			builder.ToTable("Costs").HasKey(x => x.Id);
			builder.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
			builder.Property(x => x.AmountFor).HasColumnType("NVARCHAR").HasMaxLength(200);
			builder.HasOne(x => x.Permit).WithMany(x => x.Costs).HasForeignKey(x => x.PermitId).OnDelete(DeleteBehavior.ClientSetNull);
		}
	}
}
