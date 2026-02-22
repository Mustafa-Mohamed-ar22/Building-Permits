namespace Building_Permits.MVC.ViewModels
{
    public class ExpenseDetailsVM
    {
        public string clientName { get; set; }
        public decimal AgreedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal? RemainingAmound { get; set; }
        public DateTime? createdAt { get; set; }
    }
    }
