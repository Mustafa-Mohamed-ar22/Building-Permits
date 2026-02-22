namespace Building_Permits.MVC.ViewModels
{
	public class PermitDetailsVM
	{
		public int PermitId { get; set; }
		public string ClientName { get; set; }
		public DateTime PermitStartedAt { get; set; }
		
		public int DaysOnPermitGenerally { get; set; }
		public decimal TotolCosts { get; set; }
		public List<permitstageDetails> permitstages { get; set; } = new List<permitstageDetails>();
	}
	public class permitstageDetails
	{
		public int stageId { get; set; }

		public string stageName { get; set; }
		public string PathPhoto { get; set; }
		public DateTime StageStartedAt { get; set; }
		public DateTime StageCompletedAt { get; set; }
		public int DaysSpendOnthisStage { get; set; }
	}
}
