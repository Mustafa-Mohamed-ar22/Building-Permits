using Building_Permits.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building_Permits.Infrastructure.Config
{
    public class StageConfiguration : IEntityTypeConfiguration<Stage>
    {
        public void Configure(EntityTypeBuilder<Stage> builder)
        {
            builder.ToTable("Stages").HasKey(x => x.Id);

            builder.Property(e => e.Name).HasMaxLength(200);
        }
    }
}
