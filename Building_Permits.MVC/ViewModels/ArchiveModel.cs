namespace Building_Permits.MVC.ViewModels
{
    public class ArchiveModel
    {
        public string clientName { get; set; }
        public string EngineerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? finishedAt { get; set; }
        public int NumOfDaysSpent { get; set; }
        public string PermitType { get; set; }
        public decimal Costs { get; set; }
        public decimal AgreedOn { get; set; }
        public decimal Received { get; set; }
        public decimal? Remaining { get; set; }
        public int? id { get; set; }
    }
}