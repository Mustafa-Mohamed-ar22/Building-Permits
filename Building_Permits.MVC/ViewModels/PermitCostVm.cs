using Building_Permits.Core.Entities;

namespace Building_Permits.MVC.ViewModels
{
    public class PermitCostVm
    {
        public decimal total { get; set; }
        public List<Cost> permitcosts { get; set; }
        public string clientName { get; set; }
        public int? PermitId { get; set; }
    }
}
