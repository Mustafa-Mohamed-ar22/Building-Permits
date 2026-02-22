using Building_Permits.Core.Repo;
using Building_Permits.Infrastructure;
using Building_Permits.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Building_Permits.MVC.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ReportController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IReportRepo reportService;

        public ReportController(AppDbContext context,IReportRepo reportService)
        {
            _context = context;
            this.reportService = reportService;
        }
        public IActionResult Index()
        {
            var reports = _context.Reports
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(reports);
        }
        [HttpGet]
        public IActionResult WeeklyReport()
        {
            reportService.SendWeeklyReport();
            TempData["sendreport"] = "تم إنشاء التقرير الأسبوعى بنجاح .. من فضلك راجع التقارير";
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Details(int id)
        {
            var report =  _context.Reports.Find(id);

            if (report == null)
            {
                return NotFound();
            }
            report.IsRead = true;
            _context.SaveChanges();
            return View(report);
        }
		public IActionResult GetUnRead()
		{
			var notifications = _context.Reports.Where(x => x.IsRead == false).OrderBy(x => x.CreatedAt).ToList();

			return Json(new
			{
				count = notifications.Count,
				notifications
			});
		}
	}
}
