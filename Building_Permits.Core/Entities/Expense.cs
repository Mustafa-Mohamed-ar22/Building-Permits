namespace Building_Permits.Core.Entities
{
	public class Expense
    {
        public int Id { get; set; }

        public int PermitId { get; set; }

        public decimal AgreementAmount { get; set; }

        public decimal ReceivedAmount { get; set; }

        public decimal? RemainingAmount { get; set; }
        public virtual Permit Permit { get; set; } = null!;
        public DateTime? AddedAt { get; set; }  
    }
}
