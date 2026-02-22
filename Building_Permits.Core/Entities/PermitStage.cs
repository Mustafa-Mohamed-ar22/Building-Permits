namespace Building_Permits.Core.Entities
{
    public class PermitStage
    {
        public int Id { get; set; }

        public int PermitId { get; set; }

        public int StageId { get; set; }

        public string? Notes { get; set; }

        public string? AttachmentPath { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime DueAt { get; set; }

        public string? TransactionNumber { get; set; }

        public virtual Permit Permit { get; set; } 

        public virtual Stage Stage { get; set; } 
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();


        public bool isCheckedForOverDue { get; set; }
        public bool isCheckedForNeglect { get; set; }

    }


}
