using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Building_Permits.Infrastructure.Services
{
    public class NotificationService : INotificationRepo
	{
		private AppDbContext _context;
        private readonly IConfiguration configuration;
		private readonly IPermitRepo _permitService;
		private readonly IClientRepo _clientRepo;
		private readonly IPermitStageRepo _permitStageRepo;
		private readonly IStageRepo stageRepo;
        public NotificationService(AppDbContext _context,IConfiguration configuration,
            IPermitRepo permitService, IClientRepo clientRepo , IPermitStageRepo _permitStageRepo, IStageRepo stageRepo)
        {
            this._context = _context;
            this.configuration = configuration;
            _permitService = permitService;
            _clientRepo = clientRepo;
			this._permitStageRepo = _permitStageRepo;
			this.stageRepo = stageRepo;

        }
		public void SendStageOverdue(PermitStage permitStage)
		{
			int daysOverdue = (DateTime.Now - permitStage.DueAt).Days;
			DateTime stageStartedAr = permitStage.StartedAt;
			DateTime StageExpectedTocomplete = permitStage.DueAt;
			Notification notification = new Notification()
			{
				PermitId = permitStage.PermitId,
				CreatedAt = DateTime.Now,
				IsRead = false,
                //Message = $"نحيطكم علماً بأن المرحلة \"{stageRepo.GetStageById(permitStage.StageId).Name}\" الخاصة بالعميل \"{_clientRepo.GetById(_permitService.GetById(permitStage.PermitId).Id).FullName}\" والتابعة للمهندس \"{_context.Users.FirstOrDefault(x => x.Id == (_permitService.GetById(permitStage.PermitId).CreatedBy))}\" قد تأخرت وتجاوزت الحد الأقصى المسموح به وهو {stageRepo.GetStageById(permitStage.StageId).DefaultDaysLimit} يوماً، حيث بلغت مدة التأخير {daysOverdue} يوماً، وذلك وفقاً للمدة المحددة للرخصة.",
                Message = $"نحيطكم علماً بأن المرحلة \"{stageRepo.GetStageById(permitStage.StageId).Name}\" الخاصة بالعميل \"{_clientRepo.GetById(_permitService.GetById(permitStage.PermitId).ClientId).FullName}\" والتابعة للمهندس \"{_context.Users.FirstOrDefault(x => x.Id == (_permitService.GetById(permitStage.PermitId).CreatedBy))}\" قد تأخرت وتجاوزت الحد الأقصى المسموح به وهو {stageRepo.GetStageById(permitStage.StageId).DefaultDaysLimit} يوماً، حيث بدأت بتاريخ {stageStartedAr:yyyy/MM/dd} وكان من المتوقع الانتهاء منها بتاريخ {StageExpectedTocomplete:yyyy/MM/dd}، وقد بلغت مدة التأخير {daysOverdue} يوماً، وذلك وفقاً للمدة المحددة للرخصة.",
				UserId = "4386ff1f-8d45-41cc-9fd7-1ed9150e7b31",// configuration.GetSection("AdminId").Value!,
				PermitStageId = permitStage.Id,
				Title = "متأخر"
			};
			_context.Notifications.Add(notification);
			_context.SaveChanges();
		}
		public void sendArhcievedPermit(Permit permit)
		{
            var client = _clientRepo.GetAllClients().FirstOrDefault(x => x.PermitId == permit.Id);
            Notification notification = new Notification()
			{
				PermitId = permit.Id,
				CreatedAt = DateTime.Now,
				IsRead = false,
				Message = $"تم حفظ رخصة العميل \"{client?.FullName}\" فى الأرشيف.",
				Title = "أرشيف",
				UserId = "4386ff1f-8d45-41cc-9fd7-1ed9150e7b31"// configuration.GetSection("AdminId").Value!,
			};
			_context.Notifications.Add(notification);
			_context.SaveChanges();
		}

		public void Sendneglected(Permit permit)
		{
			Notification notification = new Notification()
			{
				PermitId = permit.Id,
				CreatedAt = DateTime.Now,
				IsRead = false,
				Message = $"فى الرخصة الخاصة بالعميل {_clientRepo.GetAllClients().FirstOrDefault(x => x.PermitId == permit.Id).FullName} .. تم إنجاز جميع البنود الأربعة الأولى يتبقى فقط تقديم وتسليم الملف",
				Title = "إهمال",
				UserId = "4386ff1f-8d45-41cc-9fd7-1ed9150e7b31"
			};
			_context.Notifications.Add(notification);
			_context.SaveChanges();
		}
		public void SendReminder(Permit permit)
		{
			Notification notification = new Notification()
			{
				PermitId = permit.Id,
				CreatedAt = DateTime.Now,
				IsRead = false,
				Message = $"الرخصة {_context.Clients.FirstOrDefault(x => x.PermitId == permit.Id).FullName} وصلت إلى موعد الاستيفاء، الرجاء المتابعة.",
				Title = "استيفاء",
				UserId = "4386ff1f-8d45-41cc-9fd7-1ed9150e7b31"
			};
			_context.Notifications.Add(notification);
			_context.SaveChanges();
		}
	}
}

