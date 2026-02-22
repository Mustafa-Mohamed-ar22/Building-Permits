using Building_Permits.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building_Permits.Infrastructure.Config
{
	public class ClientConfiguraion : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients").HasKey(t => t.Id);
            builder.Property(e => e.Address).HasMaxLength(300);
            builder.Property(e => e.FullName).HasMaxLength(200);
            builder.Property(e => e.NationalId).HasMaxLength(50);
            builder.Property(e => e.Phone).HasMaxLength(50);

            builder.HasOne(x=>x.Permit).WithOne(x=>x.Client).HasForeignKey<Permit>(x=>x.ClientId).OnDelete(DeleteBehavior.ClientSetNull); ;
        }
    }
}
