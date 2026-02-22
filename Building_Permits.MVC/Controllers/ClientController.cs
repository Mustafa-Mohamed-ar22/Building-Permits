using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using Building_Permits.Infrastructure;
using Building_Permits.Infrastructure.Services;
using Building_Permits.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace Building_Permits.MVC.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly IClientRepo clientService;
        private readonly IPermitRepo permitService;
        private readonly IExpenseRepo expenseService;
        private readonly IStageRepo stageService;
        private readonly IPermitStageRepo permitStageService;
		private readonly AppDbContext context;
		private readonly ICostRepo costService;

		public ClientController(IClientRepo clientService, IPermitRepo permitService,
            IExpenseRepo expenseService, IStageRepo stageService, IPermitStageRepo permitStageService
            ,AppDbContext context,ICostRepo costService)
        {
            this.clientService = clientService;
            this.permitService = permitService;
            this.expenseService = expenseService;
            this.stageService = stageService;
            this.permitStageService = permitStageService;
			this.context = context;
			this.costService = costService;
		}
       
		public IActionResult search(string q)
        {
            var result = string.IsNullOrEmpty(q) ?
                clientService.GetAllClients().Where(x => x.Permit.Status != Core.Enums.PermitStatus.Archieved && !x.Permit.isPaused
                &&x.PermitId!=null).ToList() :

                clientService.GetAllClients().Where(x => x.FullName.ToLower().Contains(q.ToLower()) ||
                x.NationalId.Contains(q.ToLower()) || x.Address.ToLower().Contains(q.ToLower())
                || x.Permit.PermitType.ToString().ToLower().Contains(q.ToLower()) || x.Phone.Contains(q))
                .Where(x => x.Permit.Status != Core.Enums.PermitStatus.Archieved).ToList();
            return PartialView("_clientTable", result);
        }
        [HttpGet]
        public IActionResult Index()
        {
            var res = clientService.GetAllClients().
                Where(x => permitService.GetById((int)x.PermitId).Status != Core.Enums.PermitStatus.Archieved && !x.Permit.isPaused && x.PermitId != null);
            return View(res);
        }
        [HttpGet]
        public IActionResult Create()
        {
            string token = Guid.NewGuid().ToString();
            TempData["FormToken"] = token;
            ViewBag.FormToken = token;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientWithPermitWithExpense dataFromReq,string FormToken)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Claims.First(x => x.Type == "Id").Value;
                string expectedToken = TempData.Peek("FormToken").ToString();

                if (expectedToken == null || expectedToken != FormToken)
                {
                    TempData["CreateClient"] = "تم حفظ البيانات بالفعل";

                    var existingPermit = await context.Permits
                        .Where(p => p.CreatedBy == userId)
                        .OrderByDescending(p => p.CreatedAt)
                        .FirstOrDefaultAsync();

                    return RedirectToAction("StageGroup1", "Permit", new { PermitId = existingPermit?.Id });
                }
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var alreadyPerocessed = await context.Clients.AnyAsync(x => dataFromReq.NationalId == x.NationalId);
                        if (alreadyPerocessed)
                        {
                            TempData["CreateClient"] = "تم حفظ البيانات بالفعل";

                            var existingPermit = await context.Permits
                                .Where(p => p.CreatedBy == userId)
                                .OrderByDescending(p => p.CreatedAt)
                                .FirstOrDefaultAsync();

                            return RedirectToAction("StageGroup1", "Permit", new { PermitId = existingPermit?.Id });
                        }
                        // Create Client
                        Client client = new Client()
                        {
                            Address = dataFromReq.Address,
                            FullName = dataFromReq.FullName,
                            NationalId = dataFromReq.NationalId,
                            Phone = dataFromReq.Phone,
                        };
                        clientService.AddClient(client);
                        await context.SaveChangesAsync();

                        // Create Permit
                        Permit permit = new Permit()
                        {
                            PermitType = dataFromReq.PermitType,
                            ClientId = client.Id,
                            CreatedAt = DateTime.Now,
                            Status = Core.Enums.PermitStatus.InProgrss,
                            Current_Execution_Stage = 1,
                            CreatedBy = User.Claims.First(x => x.Type == "Id").Value,
                            isChecked = false
                        };
                        permitService.AddPermit(permit);
                        await context.SaveChangesAsync();

                        // Update Client with PermitId
                        client.PermitId = permit.Id;
                        await clientService.SaveAsync();

                        // Create PermitStages
                        var stages = await stageService.GetFirst_6_StagesAsync();
                        foreach (var stage in stages)
                        {
                            permitStageService.AddPermitStage(new PermitStage()
                            {
                                StageId = stage.Id,
                                PermitId = permit.Id,
                                StartedAt = DateTime.Now,
                                DueAt = DateTime.Now.AddDays(stage.DefaultDaysLimit),
                                isCheckedForNeglect = false,
								isCheckedForOverDue=false,
							});
                        }
                        await permitStageService.SaveAsync();

                        await transaction.CommitAsync();

                        TempData["CreateClient"] = "تم حفط البيانات .. من فضلك استكمل الإجراءات";
                        return RedirectToAction("StageGroup1", "Permit", new { PermitId = permit.Id });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "حدث خطأ أثناء حفظ البيانات. يرجى المحاولة مرة أخرى.");
                        return View(dataFromReq);
                    }
                }
            }
            return View(dataFromReq);
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var client = clientService.GetById(id);
            updateClientVM model = new updateClientVM
            {
                FullName = client.FullName,
                NationalId = client.NationalId,
                id = client.Id,
                Phone = client.Phone,
                Address = client.Address
            };
            return View(model);
        }
        [HttpPost]
		public IActionResult Update(updateClientVM model)
		{
            if(ModelState.IsValid)
            {
                Client newclient = new Client
                {
                    FullName = model.FullName,
                    NationalId = model.NationalId,
                    Address = model.Address,
                    Id = model.id,
                    Phone = model.Phone,
                };
                if(clientService.UpdateClient(newclient))
                {
                    clientService.Save();
                    TempData["Update"] = $"بنجاح {model.FullName} : تم تعديل بيانات العميل ";
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
    }
}
