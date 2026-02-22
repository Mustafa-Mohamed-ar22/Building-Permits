using Building_Permits.Core.Entities;

namespace Building_Permits.MVC.ViewModels
{
    public class ExpenseVM
    {
        public int expenseId { get; set; }
        public string clientName { get; set; }
        public decimal AgreedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal? RemainingAmound { get; set; }
        public DateTime? createdAt { get; set; }
        public decimal? heHAs { get; set; }
        public decimal debt { get; set; }
    }
}
