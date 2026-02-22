namespace Building_Permits.MVC.ViewModels
{
    public class IndexGeneralCostsVM
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string AmountFor { get; set; }
        public DateTime? AddedAt { get; set; }
        public string? AddedBy { get; set; }
    }
}