namespace Building_Permits.Core.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // The report message text
        public DateTime CreatedAt { get; set; }
        public string Period { get; set; } = string.Empty; // "15/09/2025 - 22/09/2025"
        public bool HasCriticalIssues { get; set; }
        public bool IsRead { get; set; }

	}

}
