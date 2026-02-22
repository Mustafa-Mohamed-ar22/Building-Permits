namespace Building_Permits.MVC.ViewModels
{
    public class NotifVM
    {
        public string EngineerConsidered { get; set; }
        public string ClientName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool wasRead { get; set; }
        public string PermitStageName { get; set; }
        public DateTime stageStartedAt { get; set; }
        public DateTime ExpectedCompletedAt { get; set;}
        public int daysOverDue { get; set; }
        public string Title { get; set; }
        public DateTime PermitCreatedAt {  get; set; }
    }
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; } // "متأخر", "أرشيف", "إهمال"

        // Permit Information
        public int PermitId { get; set; }
        public string ClientName { get; set; }
        public string EngineerName { get; set; }

        // Stage Information (for overdue notifications)
        public int? PermitStageId { get; set; }
        public string StageName { get; set; }
        public DateTime? StageDueDate { get; set; }
        public int? StageDefaultDaysLimit { get; set; }
        public int? DaysOverdue { get; set; }

        // UI Helper Properties
        public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string StageDueDateFormatted => StageDueDate?.ToString("dd/MM/yyyy") ?? "N/A";
        public string NotificationTypeClass => NotificationType switch
        {
            "متأخر" => "danger",
            "أرشيف" => "info",
            "إهمال" => "warning",
            _ => "secondary"
        };
        public string NotificationIcon => NotificationType switch
        {
            "متأخر" => "fas fa-clock",
            "أرشيف" => "fas fa-archive",
            "إهمال" => "fas fa-exclamation-triangle",
            _ => "fas fa-bell"
        };
    }

    public class NotificationsPageViewModel
    {
        public List<NotificationViewModel> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public int OverdueCount { get; set; }
        public int ArchivedCount { get; set; }
        public int NeglectedCount { get; set; }
        public int FulfilCount { get; set; }
        public string FilterType { get; set; } = "all"; // all, unread, overdue, archived, neglected
    }
}
