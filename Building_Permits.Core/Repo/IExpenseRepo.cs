using Building_Permits.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Building_Permits.Core.Repo
{
	public interface IExpenseRepo
    {
        public Expense GetById(int id);
        public Expense GetPermitById(int id);
        public bool AddExpense(Expense expense);
        public bool DeleteExpense(int id);
        public List<Expense> GetAllExpenses();
        public void Save();
        public void Delete(int id);
    }
}
