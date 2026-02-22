using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using Building_Permits.Infrastructure;
using Building_Permits.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace Building_Permits.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExpenseController : Controller
	{
		private AppDbContext _context;
		private readonly IClientRepo clientService;
		private readonly IPermitRepo permitService;
		private readonly IExpenseRepo expenseService;

		public ExpenseController(AppDbContext _context, IClientRepo clientService,IPermitRepo permitService,IExpenseRepo expenseService)
		{
			this._context = _context;
			this.clientService = clientService;
			this.permitService = permitService;
			this.expenseService = expenseService;
		}
		public IActionResult searchExpenses(string q)
		{
            var allexpenses = expenseService.GetAllExpenses().Where(x=>x.PermitId!=null).Select(x => new ExpenseVM
            {
                expenseId = x.Id,
                clientName = clientService.GetByPermitId(x.PermitId).FullName,
                AgreedAmount = x.AgreementAmount,
                ReceivedAmount = x.ReceivedAmount,
                RemainingAmound = x.RemainingAmount,
                createdAt = x.AddedAt,
                heHAs = _context.Expenses.Sum(c => c.RemainingAmount),
                debt = _context.Expenses.Sum(c => c.ReceivedAmount > c.AgreementAmount ? c.ReceivedAmount - c.AgreementAmount : 0)
            }).ToList();
			var model = string.IsNullOrEmpty(q)?allexpenses:
				allexpenses.Where(x=>x.clientName.ToLower().Contains(q.ToLower())).ToList();
			return PartialView("_ExpensesTable", model);
        }

        public IActionResult Index()
		{
			var allexpenses = expenseService.GetAllExpenses().Where(x => x.PermitId != null).Select(x => new ExpenseVM
			{
				expenseId=x.Id,
				clientName = clientService.GetByPermitId(x.PermitId).FullName,
				AgreedAmount=x.AgreementAmount,
				ReceivedAmount=x.ReceivedAmount,
				RemainingAmound = x.RemainingAmount,
				createdAt = x.AddedAt,
				heHAs=_context.Expenses.Sum(c => c.RemainingAmount),
				debt=_context.Expenses.Sum(c => c.ReceivedAmount > c.AgreementAmount ? c.ReceivedAmount - c.AgreementAmount : 0)
			});
			return View(allexpenses);
		}
		[HttpGet]
		public IActionResult ExpenseDetails(int id)
		{
			Expense x = expenseService.GetPermitById(id);
			ExpenseDetailsVM vm = new ExpenseDetailsVM
			{
				clientName = clientService.GetByPermitId(x.PermitId).FullName,
				AgreedAmount = x.AgreementAmount,
				ReceivedAmount = x.ReceivedAmount,
				RemainingAmound = x.RemainingAmount,
				createdAt = x.AddedAt,
			};

            return View(vm);
		}
        [HttpGet]
		public IActionResult AddExpense(int id)
		{
            if (expenseService.GetPermitById(id) != null)
            {
                TempData["AddExpense"] = $"بالفعل يوجد حساب للرخصة الخاصة بالعميل {clientService.GetAllClients().FirstOrDefault(x => x.PermitId == id)?.FullName}";
                return RedirectToAction("Index", "Client");
            }
            ExpenseModel model = new ExpenseModel
			{
				PermitId = id,
				PermitOwner = clientService.GetAllClients().FirstOrDefault(x => x.PermitId == id).FullName,
			};
            string token = Guid.NewGuid().ToString();
            TempData["FormToken"] = token;
            ViewBag.FormToken = token;
            return View(model);
		}
		[ValidateAntiForgeryToken]
		[HttpPost]
        public IActionResult AddExpense(ExpenseModel viewmmodel, string FormToken)
        {
            string expectedToken = TempData.Peek("FormToken").ToString();
            if (expectedToken == null || expectedToken != FormToken)
            {
                TempData["AddExpense"] = $"بالفعل تم إضافة حساب للرخصة الخاصة بالعميل  {viewmmodel.PermitOwner}";
                return RedirectToAction("Index", "Client");
            }
            if (!ModelState.IsValid)
            {
                return View(viewmmodel);
            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (expenseService.GetPermitById(viewmmodel.PermitId) != null)
                    {
                        TempData["AddExpense"] = $"بالفعل يوجد حساب للرخصة الخاصة بالعميل {viewmmodel.PermitOwner}";
                        return RedirectToAction("Index", "Client");
                    }
                    Expense expense = new Expense()
                    {
                        AgreementAmount = viewmmodel.AgreementAmount,
                        ReceivedAmount = viewmmodel.Received,
                        PermitId = viewmmodel.PermitId,
                        AddedAt = DateTime.Now
                    };
                    expenseService.AddExpense(expense);
                    expenseService.Save();
                    Permit permit = permitService.GetById(viewmmodel.PermitId);
                    if (permit != null)
                    {
                        permit.ExpenseId = expense.Id;
                        permitService.Save();
                    }
                    transaction.Commit();

                    TempData["AddExpense"] = $"تم إضافة حساب للرخصة الخاصة بالعميل {viewmmodel.PermitOwner}";
                    return RedirectToAction("Index", "Client");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["AddExpense"] = "حدث خطأ أثناء إضافة الحساب";
                    return RedirectToAction("Index", "Client");
                }
            }
        }
        [HttpGet]
		public IActionResult Edit(int id)
		{
			Expense expense = expenseService.GetById(id);
            ExpenseModel model = new ExpenseModel
            {
                PermitId = expense.PermitId,
                PermitOwner = clientService.GetAllClients().FirstOrDefault(x => x.PermitId == expense.PermitId).FullName,
				AgreementAmount= expense.AgreementAmount,
				Received= expense.ReceivedAmount,
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(ExpenseModel viewmmodel)
        {
            if (ModelState.IsValid)
            {
                Expense expenseDB = expenseService.GetPermitById(viewmmodel.PermitId);
				expenseDB.AgreementAmount=viewmmodel.AgreementAmount;
				expenseDB.ReceivedAmount = viewmmodel.Received;
                _context.Expenses.Update(expenseDB);
				_context.SaveChanges();
                TempData["EditExpense"] = $"تم تعديل الحساب الخاص برخصة العميل  {viewmmodel.PermitOwner}";
                return RedirectToAction("Index", "Expense");
            }
            return View(viewmmodel);
        }
        public IActionResult Delete(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Permit permit = permitService.GetAllPermits().FirstOrDefault(x => x.ExpenseId == id);
                    if (permit == null)
                    {
                        ModelState.AddModelError("", "حدث خطأ");
                        return RedirectToAction("Index", "Expense");
                    }

                    permit.Expense = null;
                    permitService.Save();

                    Expense expense = expenseService.GetById(id);
                    expenseService.Delete(id);
                    expenseService.Save();

                    transaction.Commit();

                    TempData["deleteexpens"] = "تم حذف الحساب بنجاح";
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "حدث خطأ أثناء الحذف");
                }
            }

            return RedirectToAction("Index", "Expense");
        }
    }
}
