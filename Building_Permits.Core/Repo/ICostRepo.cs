using Building_Permits.Core.Entities;

namespace Building_Permits.Core.Repo
{
	public interface ICostRepo
    {
        public bool AddCost(Cost cost);
        public void Save();
        public void Delete(int id);
        public decimal GetTotalPermitCosts(int permitId);
        public List<Cost> GetPermitCosts (int PermitId);
        public decimal GetTotalCosts();
        public decimal GetTotalUserCosts(string UserId);
        public List<Cost> GetAllCosts();
        public Cost GetCostByPermitId (int PermitId);

	}
}
