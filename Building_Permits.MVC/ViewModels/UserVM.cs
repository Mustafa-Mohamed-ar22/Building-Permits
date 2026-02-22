namespace Building_Permits.MVC.ViewModels
{
    public class UserVM
	{
		public string Id { get; set; }
		public string username{ get; set; }
		public DateTime? createdAt{ get; set; }
		public string status{ get; set; }
		public string phone{ get; set; }

		public string FullName { get;  set; }
		public DateTime? LastLogin { get;  set; }
		public int NoOfPermits { get;  set; }
		public decimal TotalCosts { get; set; }
	}
}
