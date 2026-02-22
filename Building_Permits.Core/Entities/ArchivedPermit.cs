namespace Building_Permits.Core.Entities
{
    public class ArchivedPermit
    {
        public int PermitId { get; set; }

        public string Title { get; set; } = null!;

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? ArchivedAt { get; set; }
    }
}
