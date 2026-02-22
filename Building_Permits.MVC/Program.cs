using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using Building_Permits.Infrastructure;
using Building_Permits.Infrastructure.Migrations;
using Building_Permits.Infrastructure.Services;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
namespace Building_Permits.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            string cs  = builder.Configuration.GetSection("constr").Value;
            builder.Services.AddDbContextPool<AppDbContext>(x=>x.UseSqlServer(cs));

            builder.Services.AddScoped<IClientRepo, CleintService>();
            builder.Services.AddScoped<IPermitRepo, PermitService>();
            builder.Services.AddScoped<IExpenseRepo, ExpenseService>();
            builder.Services.AddScoped<IStageRepo, StageService>();
            builder.Services.AddScoped<INotificationRepo, NotificationService>();
            builder.Services.AddScoped<ICostRepo, CostService>();
            builder.Services.AddScoped<IReportRepo, ReportService>();
            builder.Services.AddScoped<IPermitStageRepo, PermitStageService>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            
            builder.Services.AddHangfire(x => x.UseSqlServerStorage(cs));
            builder.Services.AddHangfireServer();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(1); 
                options.SlidingExpiration = false;  
                options.LoginPath = "/Account/Login"; 
            });
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
			string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Console.WriteLine(uploadsPath);
            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsPath))
			{
				Directory.CreateDirectory(uploadsPath);
			}
			app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
                RequestPath = "/Uploads"
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
           
			
            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<StageDeadlineService>("check-deadline-service", x => x.CheckOverdueStages(),
               Cron.Hourly);
            RecurringJob.AddOrUpdate<StageDeadlineService>("check-archieved-service", x => x.checkArchived(),
            Cron.Hourly);

            RecurringJob.AddOrUpdate<StageDeadlineService>("check-neglect-service", x => x.checkNeglect(),
               Cron.Hourly);
            RecurringJob.AddOrUpdate<ReportService>("weekly-Report", x => x.SendWeeklyReport(),
               Cron.Weekly(DayOfWeek.Friday, 14));
            RecurringJob.AddOrUpdate<StageDeadlineService>("Fulfilments", x => x.CheckFulfilments(),
              Cron.Daily);
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }

}