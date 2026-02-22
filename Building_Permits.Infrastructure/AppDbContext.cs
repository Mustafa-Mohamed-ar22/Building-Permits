using Building_Permits.Core.Entities;
using Building_Permits.Infrastructure.Config;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Building_Permits.Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {

        public virtual DbSet<ArchivedPermit> ArchivedPermits { get; set; }

        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<Expense> Expenses { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Permit> Permits { get; set; }

        public virtual DbSet<PermitStage> PermitStages { get; set; }
        public virtual DbSet<Cost> Costs { get; set; }
        public virtual DbSet<GeneralCost> GeneralCosts { get; set; }

        public virtual DbSet<Stage> Stages { get; set; }
        public DbSet<Report> Reports { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientConfiguraion).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
