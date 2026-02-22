using Building_Permits.Core.Enums;

namespace Building_Permits.Core.Entities
{
    public class Permit
    {
        public int Id { get; set; }

        public PermitStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ArchivedAt { get; set; }

        public int ClientId { get; set; }
        public string PermitType { get; set; }
        public int Current_Execution_Stage { get; set; }
        public  Client Client { get; set; } 
        public Expense? Expense { get; set; }
        public int? ExpenseId { get; set; }
        public virtual ICollection<Cost>? Costs { get; set; } = new List<Cost>();
        public virtual ICollection<Notification>? Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<PermitStage>? PermitStages { get; set; } = new List<PermitStage>();

        public string? CreatedBy { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }


        public bool isChecked { get; set; }

        public bool isPaused { get; set; }
		public DateTime? FulfilmentDate { get; set; }
		public string? FulfilmentReason { get; set; }
        public DateTime? PausedAt { get; set; }
	}
}
