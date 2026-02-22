using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using Building_Permits.Infrastructure;
using Building_Permits.Infrastructure.Migrations;
using Building_Permits.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Building_Permits.MVC.Controllers
{
	[Authorize]
	public class CostController : Controller
	{
		private readonly IPermitRepo permitService;
		private readonly IClientRepo clientService;
        private readonly AppDbContext _context;

        public ICostRepo costService { get; }

		public CostController(IPermitRepo permitService, IClientRepo clientService,ICostRepo costService,AppDbContext _context)
		{
			this.permitService = permitService;
			this.clientService = clientService;
			this.costService = costService;
            this._context = _context;
        }
		public IActionResult searchCosts(string q)
		{
			var allpermits = permitService.GetAllPermits();
			List<PermitCostVm> list = new List<PermitCostVm>();
			foreach (var item in allpermits)
			{
				if (costService.GetAllCosts().Any(x => x.PermitId == item.Id))
				{
					list.Add(new PermitCostVm
					{
						clientName = clientService.GetById(item.ClientId).FullName,
						total = costService.GetTotalPermitCosts(item.Id),
						permitcosts = costService.GetPermitCosts(item.Id)
					});
				}
			}
			var result = string.IsNullOrEmpty(q) ? list: 
				list.Where(x=>x.clientName.ToLower().Contains(q.ToLower())||
			x.permitcosts.Any(p => p.AmountFor != null && p.AmountFor.ToLower().Contains(q.ToLower())));
			return PartialView("_costsTable", result);
		}
		[HttpGet]
		[Authorize(Roles ="Admin")]
		public IActionResult Index()
		{
			var allpermits = permitService.GetAllPermits();
			List<PermitCostVm> list = new List<PermitCostVm>();
			foreach (var item in allpermits)
			{
				if (costService.GetAllCosts().Any(x => x.PermitId == item.Id))
				{
					list.Add(new PermitCostVm
					{
						clientName = clientService.GetById(item.ClientId).FullName,
						total = costService.GetTotalPermitCosts(item.Id),
						permitcosts = costService.GetPermitCosts(item.Id),
						PermitId = item.Id
                    });
				}
			}
			return View(list);
		}

		[HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult edit(int PermitId, int costId)
		{
			Cost cost = _context.Costs.FirstOrDefault(x => x.PermitId == PermitId && x.Id == costId);
			CostVM VM = new CostVM
			{
				PermitId = PermitId,
				Amount = cost.Amount,
				AmountFor  =cost.AmountFor,
				ClientName  = clientService.GetByPermitId(PermitId).FullName,
				CostId = costId
            };
			return View(VM);
		}
		[HttpPost]
        public IActionResult edit(CostVM viewmodel)
        {
            if(ModelState.IsValid)
            {
				Cost costDB = _context.Costs.FirstOrDefault(x => x.PermitId == viewmodel.PermitId && x.Id == viewmodel.CostId);
				costDB.Amount = viewmodel.Amount;
				costDB.AmountFor = viewmodel.AmountFor;
				_context.SaveChanges();
                TempData["editcost"] = $"تم تعديل المصاريف الخاصة برخصة العميل {viewmodel.ClientName} بنجاح";
				return RedirectToAction("Index");
            }
            return View(viewmodel);
		}
		[HttpGet]
		[Authorize]
		public IActionResult AddCost(int id)
		{
			CostVM costVM = new CostVM()
			{
				PermitId = id,
				ClientName = clientService.GetAllClients().FirstOrDefault(x => x.PermitId == id).FullName
			};
			return View(costVM);
		}
		[HttpPost]
        [Authorize]
        public IActionResult AddCost(CostVM viewmodel)
		{ 
			if(ModelState.IsValid)
			{
				Cost cost = new Cost()
				{
					Amount = viewmodel.Amount,
					AddedAt = DateTime.Now,
					AmountFor = viewmodel.AmountFor,
					PermitId = viewmodel.PermitId,
					AddedBy= User.Claims.First(x=>x.Type== "Name").Value != null ? User.Claims.First(x => x.Type == "Name").Value : "" ,
				};
				if (cost.AddedBy == "")
					return RedirectToAction("Login", "Account");

				costService.AddCost(cost);
				costService.Save();
				TempData["AddCost"] = $"{viewmodel.ClientName}:تم إضافة المصاريف الخاصة برخصة العميل ";
				if(User.IsInRole("Admin"))
				{
					return RedirectToAction("PermitCosts", new {id=viewmodel.PermitId,ClientName = viewmodel.ClientName});
				}else
				{
					return RedirectToAction("Index","Client");
				}
			}
			return View(viewmodel);
		}
		[HttpGet]
		[Authorize(Roles ="Admin")]
        public IActionResult PermitCosts(int id,string ClientName)
		{
			var permitcosts = costService.GetPermitCosts(id);
			var total = costService.GetTotalPermitCosts(id);
			return View(new PermitCostVm
			{
				total = total,
				permitcosts = permitcosts,
				clientName=ClientName
			});
		}
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int PermitId, int costId)
        {
            Cost cost = _context.Costs.FirstOrDefault(x => x.PermitId == PermitId && x.Id == costId);
			costService.Delete(cost.Id);
			costService.Save();
			TempData["DeleteCost"] = "تم حذف المصروف بنجاح";
			return RedirectToAction("Index");
        }
    }
}