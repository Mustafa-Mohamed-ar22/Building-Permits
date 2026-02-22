namespace Building_Permits.MVC.ViewModels
{
    public class fulfilDisplayVM
    {
        public string clientName { get; set; }
        public int PermitId { get; set; }
        public DateTime? pausedAt { get; set; }
        public DateTime? rememberDate { get; set; }
        public string fulfilReason { get; set; }
    }
}