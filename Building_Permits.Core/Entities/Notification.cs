namespace Building_Permits.Core.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser? applicationUser { get; set; }

        public int? PermitId { get; set; }

        public int? PermitStageId { get; set; }

        public string Message { get; set; } = null!;
        public string Title { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        public virtual Permit? Permit { get; set; }

        public virtual PermitStage? PermitStage { get; set; }
    }


}
