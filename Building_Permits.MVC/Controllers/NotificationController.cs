using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using Building_Permits.Infrastructure;
using Building_Permits.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Building_Permits.MVC.Controllers
{
    [Authorize(Roles ="Admin")]
	public class NotificationController : Controller
	{
		private readonly AppDbContext _context;
        private readonly IClientRepo _clientRepo;
        private readonly IStageRepo _stageRepo;

        public NotificationController(AppDbContext _context, IClientRepo clientRepo, IStageRepo stageRepo)
		{
			this._context = _context;
            _clientRepo = clientRepo;
            _stageRepo = stageRepo;
        }
       
        public async Task<IActionResult> GetAllNotifications(string filterType = "all")
        {
			//var notificationsQuery = _context.Notifications
			//    .Include(n => n.Permit)
			//        .ThenInclude(p => p.ApplicationUser)
			//    .Include(n => n.PermitStage)
			//        .ThenInclude(ps => ps.Stage)
			//    .OrderByDescending(n => n.CreatedAt)
			//    .AsQueryable();
			//var notificationsQuery = _context.Notifications
			//    .Include(n => n.Permit)
			//     .ThenInclude(p => p.ApplicationUser)
			//    .Include(n => n.PermitStage)
			//     .ThenInclude(ps => ps.Stage)
			//    .OrderByDescending(n => n.CreatedAt)
			//    .GroupBy(n => n.Message)
			//    .Select(g => g.First())
			//    .AsQueryable();
			var allNotificationsWithIncludes = await _context.Notifications
			  .Include(n => n.Permit)
				  .ThenInclude(p => p.ApplicationUser)
			  .Include(n => n.PermitStage)
				  .ThenInclude(ps => ps.Stage)
			  .OrderByDescending(n => n.CreatedAt)
			  .ToListAsync();

			// Apply distinct and filtering in memory
			var notificationsQuery = allNotificationsWithIncludes
				.GroupBy(n => n.Message)
				.Select(g => g.First())  // Gets most recent per message
				.AsQueryable();

			// Apply filters
			switch (filterType.ToLower())
            {
                case "unread":
                    notificationsQuery = notificationsQuery.Where(n => !n.IsRead);
                    break;
                case "overdue":
                    notificationsQuery = notificationsQuery.Where(n => n.Title == "متأخر");
                    break;
                case "archived":
                    notificationsQuery = notificationsQuery.Where(n => n.Title == "أرشيف");
                    break;
                case "neglected":
                    notificationsQuery = notificationsQuery.Where(n => n.Title == "إهمال");
                    break;
				case "fulfil":
					notificationsQuery = notificationsQuery.Where(n => n.Title == "استيفاء");
					break;
            }

			var notifications =  notificationsQuery.ToList();
		        //var notifications = await notificationsQuery
	        // .OrderByDescending(n => n.CreatedAt)
	        // .ToListAsync();
			var viewModel = new NotificationsPageViewModel
            {
                FilterType = filterType,
                Notifications = notifications.Select(n => MapToViewModel(n)).ToList()
            };

			// Get counts for statistics
			var allNotifications = _context.Notifications
	            .ToList() 
	            .DistinctBy(x => x.Message)
	            .ToList();
			viewModel.TotalCount = allNotifications.Count();
            viewModel.UnreadCount = allNotifications.Count(n => !n.IsRead);
            viewModel.OverdueCount = allNotifications.Count(n => n.Title == "متأخر");
            viewModel.ArchivedCount = allNotifications .Count(n => n.Title == "أرشيف");
            viewModel.NeglectedCount = allNotifications .Count(n => n.Title == "إهمال");
            viewModel.FulfilCount = allNotifications .Count(n => n.Title == "استيفاء");

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, count = unreadNotifications.Count });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _context.Notifications.CountAsync(n => !n.IsRead);
            return Json(new { count });
        }

        private NotificationViewModel MapToViewModel(Notification notification)
        {
            var client = _clientRepo.GetAllClients()
                .FirstOrDefault(c => c.PermitId == notification.PermitId);

            var viewModel = new NotificationViewModel
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                NotificationType = notification.Title,
                PermitId = (int)notification.PermitId,
                ClientName = client?.FullName ?? "Unknown Client",
                EngineerName = notification.Permit?.ApplicationUser?.FullName ?? "Unknown Engineer",
                PermitStageId = notification.PermitStageId
            };

            if (notification.PermitStage != null)
            {
                viewModel.StageName = notification.PermitStage.Stage?.Name ?? "Unknown Stage";
                viewModel.StageDueDate = notification.PermitStage.DueAt;

                if (notification.PermitStage.Stage != null)
                {
                    viewModel.StageDefaultDaysLimit = notification.PermitStage.Stage.DefaultDaysLimit;
                }

                // Calculate days overdue
                if (notification.PermitStage.DueAt != null && DateTime.Now > notification.PermitStage.DueAt)
                {
                    viewModel.DaysOverdue = (DateTime.Now - notification.PermitStage.DueAt).Days;
                }
            }

            return viewModel;
        }
        public IActionResult GetUnRead()
        {
			//var notifications = _context.Notifications.Where(x => x.IsRead == false).OrderBy(x => x.CreatedAt).ToList();
			//var notifications = _context.Notifications
	  //      .Where(x => x.IsRead == false)
	  //      .GroupBy(x => x.Message)
	  //      .Select(g => g.OrderByDescending(n => n.CreatedAt).First())  // Keep oldest per message
	  //      .OrderByDescending(x => x.CreatedAt)
	  //      .ToList();
			var notifications = _context.Notifications
		        .Where(x => x.IsRead == false)
		        .OrderByDescending(x => x.CreatedAt)
		        .AsEnumerable()  // ✅ Switch to client-side
		        .DistinctBy(x => x.Message)
		        .ToList();
			return Json(new
            {
                count = notifications.Count,
                notifications
            });
        }
    }
}