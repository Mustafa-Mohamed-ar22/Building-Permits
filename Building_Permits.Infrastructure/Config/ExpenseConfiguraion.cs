using Building_Permits.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building_Permits.Infrastructure.Config
{
	public class ExpenseConfiguraion : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses").HasKey(x => x.Id);
            builder.Property(e => e.AgreementAmount).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.ReceivedAmount).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.RemainingAmount)
                .HasComputedColumnSql("([AgreementAmount]-[ReceivedAmount])", true)
                .HasColumnType("decimal(19, 2)");
            builder.HasOne(x=>x.Permit).WithOne(x=>x.Expense).HasForeignKey<Permit>(x=>x.ExpenseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasIndex(x => x.PermitId).IsUnique();
        }
    }
}
