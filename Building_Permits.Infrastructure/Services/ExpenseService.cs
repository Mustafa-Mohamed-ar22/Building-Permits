using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Building_Permits.Infrastructure.Services
{
    public class ExpenseService : IExpenseRepo
    {
        private AppDbContext _context;
        public ExpenseService(AppDbContext _context)
        {
            this._context = _context;
        }
        public bool AddExpense(Expense expense)
        {
            if (expense is not null)
            {
                _context.Expenses.Add(expense);
                return true;
            }
            return false;
        }

        public bool DeleteExpense(int id)
        {
            Expense expense = _context.Expenses.FirstOrDefault(x => x.Id == id);
            if (expense is not null)
            {
                _context.Expenses.Remove(expense);
                return true;
            }
            return false;
        }

        public List<Expense> GetAllExpenses() => _context.Expenses.ToList();

        
        public Expense GetById(int id) => _context.Expenses.FirstOrDefault(x => x.Id == id);

		public Expense GetPermitById(int id)
		{
            return _context.Expenses.FirstOrDefault(x => x.PermitId == id)!;
		}

		public void Save()
        {
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            _context.Expenses.Remove(GetById(id));
        }
    }
}
