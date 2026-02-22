using Building_Permits.Core.Entities;
using Building_Permits.Infrastructure;
using Building_Permits.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Building_Permits.MVC.Controllers
{
    [Authorize]
    public class GeneralCostController : Controller
    {
        private readonly AppDbContext _context;
        public GeneralCostController(AppDbContext _context)
        {
            this._context = _context;
        }

        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            var model = _context.GeneralCosts.Select(x => new IndexGeneralCostsVM
            {
                Id = x.Id,
                AddedAt = x.AddedAt,
                AddedBy = _context.Users.FirstOrDefault(y=>y.Id==x.AddedBy).FullName,
                Amount = x.Amount,
                AmountFor  = x.AmountFor
            });
            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(GeneralCostVM model)
        {
            if(ModelState.IsValid)
            {
                GeneralCost generalCostVM = new GeneralCost
                {
                    Amount = model.Amount,
                    AmountFor = model.AmountFor,
                    AddedAt = DateTime.Now,
                    AddedBy = User.Claims.First(x => x.Type == "Id").Value
                };
                _context.GeneralCosts.Add(generalCostVM);
                _context.SaveChanges();
                TempData["addgncost"] = "تم إضافة المصروف بنجاح";
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index","Home");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit (int id)
        {
            var dbCost = _context.GeneralCosts.Find(id);
            GeneralCostVM model = new GeneralCostVM
            {
                AmountFor = dbCost.AmountFor,
                Amount = dbCost.Amount,
                Id = dbCost.Id,
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(GeneralCostVM model)
        {
            if(ModelState.IsValid)
            {
                var dbCost = _context.GeneralCosts.FirstOrDefault(x=>x.Id==model.Id);
                if (dbCost == null)
                    return RedirectToAction("Index", "Home");
                dbCost.Amount = model.Amount;
                dbCost.AmountFor = model.AmountFor;
                _context.SaveChanges();
                TempData["editgncost"] = "تم تعديل المصروف بنجاح";
                return RedirectToAction("Index");   
            }
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            var dbCost = _context.GeneralCosts.Find(id);
            _context.GeneralCosts.Remove(dbCost);
            _context.SaveChanges();
            TempData["DeleteGNCOst"] = "تم حذف المصروف بنجاح";
            return RedirectToAction("Index");
        }
    }
}
