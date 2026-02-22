using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;

namespace Building_Permits.Infrastructure.Services
{
    public class StageDeadlineService
    {
        private readonly IPermitStageRepo _permitStageRepo;
        private readonly INotificationRepo _notificationService;
        private readonly IPermitRepo _permitService;
        private readonly AppDbContext _context;
        public StageDeadlineService(IPermitStageRepo permitStageRepo, INotificationRepo notificationService, IPermitRepo _permitService, AppDbContext _context)
        {
            _permitStageRepo = permitStageRepo;
            _notificationService = notificationService;
            this._permitService = _permitService;
            this._context = _context;
        }
        public void CheckOverdueStages()
        {
            var overdueStages = _permitStageRepo.GetAllPermitStages()
                .Where(s => !s.Permit.isPaused && !s.isCheckedForOverDue && s.DueAt < DateTime.Now && s.CompletedAt == null)
                .ToList();
            foreach (var stage in overdueStages)
            {
                _notificationService.SendStageOverdue(stage);
                stage.isCheckedForOverDue = true;
            }
            _context.SaveChanges();
        }
        public void checkArchived()
        {
            var permits = _permitService.GetAllPermits().Where(x=>!x.isChecked&&!x.isPaused).ToList();
            foreach (var item in permits)
            {
                if(item.Status.ToString()== "Archieved")
                {
                    _notificationService.sendArhcievedPermit(item);
                    item.isChecked = true;
                }
            }
            _context.SaveChanges();
        }
        public void checkNeglect()
        {
            var Stages = _permitStageRepo.GetAllPermitStages().Where(x => x.Permit.Current_Execution_Stage == 1 && !x.Permit.isPaused && !x.isCheckedForNeglect)
                .OrderBy(x => x.Permit.Id).GroupBy(x => x.PermitId).ToList();
            List<PermitStage> temp = new List<PermitStage>();
            foreach (var permit in Stages)
            {
                foreach (PermitStage item in permit)
                {
                    temp.Add(item);
                }
                if (temp[0].CompletedAt != null && temp[1].CompletedAt != null && temp[2].CompletedAt != null && temp[3].CompletedAt != null)
                {
                    if (temp[4].CompletedAt == null || temp[5].CompletedAt == null)
                    {
				        _notificationService.Sendneglected(_context.Permits.FirstOrDefault(x => x.Id == temp[0].PermitId));
						foreach (var s in temp)
						{
							s.isCheckedForNeglect = true;
						}
                        _context.SaveChanges();
					}
				}
                temp.Clear();
            }
            
        //    foreach (var stage in Stages)
        //    {
        //        Console.WriteLine(stage.Permit.Id +" : " + stage.StageId);
        //    }
        //    bool IsFinished = false;
        //    List<PermitStage> temp = new List<PermitStage>();
            
        //    for (int i=0;i<Stages.Count()-1;i++)
        //    {
        //        if (Stages[i].PermitId == Stages[i+1].PermitId )
        //        {
        //            temp.Add(Stages[i]);
        //            if (temp.Count() == 5 && Stages.Count() == 6)
        //            {
        //                IsFinished = true;
        //                temp.Add(Stages[i + 1]);
        //            }
        //        }
        //        else
        //        {
        //            IsFinished=true;
        //            temp.Add(Stages[i+1]);
        //        }
        //        if(IsFinished)
        //        {
        //            if (_context.Permits.FirstOrDefault(x => x.Id == temp[0].PermitId).Current_Execution_Stage==1)
        //            {
        //                if (temp[0].CompletedAt != null&& temp[1].CompletedAt != null && temp[2].CompletedAt != null && temp[3].CompletedAt != null)
        //                {
        //                    if (temp[4].CompletedAt == null|| temp[5].CompletedAt == null)
        //                    {
        //                        _notificationService.Sendneglected(_context.Permits.FirstOrDefault(x => x.Id == temp[0].PermitId));
        //                        IsFinished = false;
								//foreach (var s in temp)
								//{
								//	s.isCheckedForNeglect = true;
								//}
								//_context.SaveChanges();
        //                        temp.Clear();
        //                    }
        //                }else
        //                {
        //                    temp.Clear();
        //                    IsFinished = false;
        //                }
        //            }else
        //            {
        //                temp.Clear();
        //                IsFinished = false;
        //            }
        //        }
        //    }
        //    _context.SaveChanges();
        }
		public void CheckFulfilments()
		{
			var duePermits = _permitService.GetAllPermits()
				.Where(p => p.isPaused && p.FulfilmentDate <= DateTime.Now)
				.ToList();

			foreach (var permit in duePermits)
			{
                _notificationService.SendReminder(permit);
			}
		}
	}

}