namespace Building_Permits.Core.Entities
{
	public class Cost
	{
		public int Id { get; set; }
		public int PermitId { get; set; }
		public Permit Permit { get; set; }
		public decimal Amount { get; set; }
		public string AmountFor { get; set; }
		public DateTime? AddedAt { get; set; }
		public string? AddedBy { get; set; }
	}
	public class GeneralCost
	{
		public int Id { get; set; }
        public decimal Amount { get; set; }
        public string AmountFor { get; set; }
        public DateTime? AddedAt { get; set; }
        public string? AddedBy { get; set; }
    }
}
