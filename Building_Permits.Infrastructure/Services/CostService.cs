using Building_Permits.Core.Entities;
using Building_Permits.Core.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Building_Permits.Infrastructure.Services
{
	public class CostService : ICostRepo
	{
		private AppDbContext _context;
        public CostService(AppDbContext _context)
        {
            this._context = _context;
        }
        public bool AddCost(Cost cost)
		{
			if (cost == null) return false;
			else
			{
				_context.Costs.Add(cost);
				return true;
			}
		}

		public decimal GetTotalUserCosts(string UserId)
		{
			var permitsForUser=_context.Permits.Where(x=>x.CreatedBy==UserId).ToList();
			var res = permitsForUser.Sum(x=>GetTotalPermitCosts(x.Id));
			return res;
			decimal totalUserCosts = 0;
			foreach (var item in permitsForUser)
			{
				totalUserCosts += GetTotalPermitCosts(item.Id);
			}
			return totalUserCosts;
		}

		public List<Cost> GetPermitCosts(int PermitId)
		{
			var result = _context.Costs.Where(x=>x.PermitId==PermitId).ToList();
			return result;
		}
		public List<Cost> GetAllCosts ()
		{
			return _context.Costs.ToList();
		}

		public decimal GetTotalCosts()
		{
			return _context.Costs.Sum(x=>x.Amount);	
		}

		public decimal GetTotalPermitCosts(int permitId)
		{
			return _context.Costs.Where(x=>x.PermitId==permitId).Sum(x => x.Amount);
		}
		public void Save()
		{
			_context.SaveChanges();
		}
        public void Delete(int id)
        {
			_context.Costs.Remove(_context.Costs.FirstOrDefault(x => x.Id == id));
        }
        public Cost GetCostByPermitId(int PermitId)
		{
			return _context.Costs.FirstOrDefault(x => x.PermitId == PermitId)!;
		}
	}
}
