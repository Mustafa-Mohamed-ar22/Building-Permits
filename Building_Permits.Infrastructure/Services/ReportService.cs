using Building_Permits.Core.Entities;
using Building_Permits.Core.Enums;
using Building_Permits.Core.Repo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Building_Permits.Infrastructure.Services
{
    public class ReportService : IReportRepo
    {
        private readonly AppDbContext _context;
        private readonly INotificationRepo _notificationService;
        private readonly ILogger<ReportService> _logger;

        public ReportService(AppDbContext context, INotificationRepo notificationService, ILogger<ReportService> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task SendWeeklyReport()
        {
            try
            {
                _logger.LogInformation("Starting weekly report generation at {Time}", DateTime.Now);

                var weekStart = DateTime.Now.AddDays(-7).Date;
                var weekEnd = DateTime.Now.Date;

                var reportData = GenerateWeeklyReport(weekStart, weekEnd);
                var reportMessage = BuildReportMessage(reportData);

                var report = new Report
                {
                    Title = "التقرير الأسبوعي",
                    Content = reportMessage,
                    CreatedAt = DateTime.Now,
                    Period = reportData.ReportPeriod,
                    HasCriticalIssues = reportData.HasCriticalIssues,
                    IsRead = false
                };

                _context.Reports.Add(report);
                _context.SaveChanges();

                _logger.LogInformation("Weekly report generated and saved successfully");
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("ghbzcc@gmail.com", "ftcq urtr itnt gwke");
                var message = new MailMessage("ghbzcc@gmail.com", "mahmoudabdelhameed483@gmail.com", "التقرير الأسبوعى", report.Content);
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating weekly report");
                throw;
            }
        }

        private WeeklyReportData GenerateWeeklyReport(DateTime weekStart, DateTime weekEnd)
        {
            var report = new WeeklyReportData
            {
                ReportPeriod = $"{weekStart:dd/MM/yyyy} - {weekEnd:dd/MM/yyyy}"
            };

            // 1. New Permits Created This Week
            report.NewPermits = _context.Permits
                .Include(p => p.Client)
                .Include(p => p.ApplicationUser)
                .Where(p => p.CreatedAt >= weekStart && p.CreatedAt <= weekEnd)
                .Select(p => new PermitSummary
                {
                    Id = p.Id,
                    ClientName = p.Client.FullName,
                    EngineerName = p.ApplicationUser != null ? p.ApplicationUser.FullName : "غير محدد",
                    Status = p.Status.ToString(),
                    PermitType = p.PermitType,
                    CurrentStage = p.Current_Execution_Stage,
                    CreatedAt = p.CreatedAt
                })
                .ToList();

            // 2. Archived Permits This Week (using ArchivedAt instead of CompletedAt)
            report.ArchivedPermits = _context.Permits
                .Include(p => p.Client)
                .Include(p => p.ApplicationUser)
                .Where(p => p.Status == PermitStatus.Archieved &&
                           p.ArchivedAt >= weekStart &&
                           p.ArchivedAt <= weekEnd)
                .Select(p => new PermitSummary
                {
                    Id = p.Id,
                    ClientName = p.Client.FullName,
                    EngineerName = p.ApplicationUser != null ? p.ApplicationUser.FullName : "غير محدد",
                    Status = p.Status.ToString(),
                    PermitType = p.PermitType,
                    ArchivedAt = p.ArchivedAt
                })
                .ToList();

            // 3. Permits by Status This Week (for better insights)
            report.PermitsByStatus = _context.Permits
                .Where(p => p.CreatedAt >= weekStart && p.CreatedAt <= weekEnd)
                .GroupBy(p => p.Status)
                .Select(g => new StatusSummary
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            // 4. Stages Completed This Week
            report.CompletedStages = _context.PermitStages
                .Include(ps => ps.Stage)
                .Include(ps => ps.Permit)
                    .ThenInclude(p => p.Client)
                .Include(ps => ps.Permit)
                    .ThenInclude(p => p.ApplicationUser)
                .Where(ps => ps.CompletedAt >= weekStart && ps.CompletedAt <= weekEnd)
                .Select(ps => new StageSummary
                {
                    Id = ps.Id,
                    StageName = ps.Stage != null ? ps.Stage.Name : "غير محدد",
                    PermitId = ps.PermitId,
                    ClientName = ps.Permit.Client.FullName,
                    EngineerName = ps.Permit.ApplicationUser != null ? ps.Permit.ApplicationUser.FullName : "غير محدد",
                    PermitType = ps.Permit.PermitType,
                    CompletedAt = ps.CompletedAt.Value,
                    DaysToComplete = ps.CompletedAt.HasValue && ps.StartedAt != null
                        ? (ps.CompletedAt.Value - ps.StartedAt).Days
                        : 0
                })
                .ToList();

            // 5. Overdue Stages (current)
            report.OverdueStages = _context.PermitStages
                .Include(ps => ps.Stage)
                .Include(ps => ps.Permit)
                    .ThenInclude(p => p.Client)
                .Include(ps => ps.Permit)
                    .ThenInclude(p => p.ApplicationUser)
                .Where(ps => ps.DueAt < DateTime.Now && ps.CompletedAt == null)
                .Select(ps => new StageSummary
                {
                    Id = ps.Id,
                    StageName = ps.Stage != null ? ps.Stage.Name : "غير محدد",
                    PermitId = ps.PermitId,
                    ClientName = ps.Permit.Client.FullName,
                    EngineerName = ps.Permit.ApplicationUser != null ? ps.Permit.ApplicationUser.FullName : "غير محدد",
                    PermitType = ps.Permit.PermitType,
                    DueDate = ps.DueAt,
                    DaysOverdue = ps.DueAt != null ? (DateTime.Now - ps.DueAt).Days : 0
                })
                .ToList();

            // 6. Financial Summary - New Expenses This Week
            report.NewExpenses = _context.Expenses
                .Include(e => e.Permit)
                    .ThenInclude(p => p.Client)
                .Where(e => e.AddedAt >= weekStart && e.AddedAt <= weekEnd)
                .Select(e => new ExpenseSummary
                {
                    Id = e.Id,
                    PermitId = e.PermitId,
                    ClientName = e.Permit.Client.FullName,
                    PermitType = e.Permit.PermitType,
                    AgreementAmount = e.AgreementAmount,
                    ReceivedAmount = e.ReceivedAmount,
                    RemainingAmount = e.RemainingAmount ?? (e.AgreementAmount - e.ReceivedAmount),
                    AddedAt = e.AddedAt
                })
                .ToList();

            // 7. New Costs This Week
            report.NewCosts = _context.Costs
                .Include(c => c.Permit)
                    .ThenInclude(p => p.Client)
                .Where(c => c.AddedAt >= weekStart && c.AddedAt <= weekEnd)
                .Select(c => new CostSummary
                {
                    Id = c.Id,
                    PermitId = c.PermitId,
                    ClientName = c.Permit.Client.FullName,
                    PermitType = c.Permit.PermitType,
                    Amount = c.Amount,
                    AmountFor = c.AmountFor,
                    AddedAt = c.AddedAt
                })
                .ToList();

            // 8. Permits by Type Analysis
            report.PermitsByType = _context.Permits
                .Where(p => p.CreatedAt >= weekStart && p.CreatedAt <= weekEnd)
                .GroupBy(p => p.PermitType)
                .Select(g => new TypeSummary
                {
                    PermitType = g.Key,
                    Count = g.Count(),
                    TotalValue = g.Sum(p => p.Expense != null ? p.Expense.AgreementAmount : 0)
                })
                .ToList();

            // 9. Current Stage Distribution
            report.StageDistribution = _context.Permits
                .Where(p => p.Status != PermitStatus.Archieved)
                .GroupBy(p => p.Current_Execution_Stage)
                .Select(g => new StageDistribution
                {
                    StageNumber = g.Key,
                    Count = g.Count()
                })
                .OrderBy(sd => sd.StageNumber)
                .ToList();

            // 10. Financial Totals
            report.TotalNewAgreements = report.NewExpenses.Sum(e => e.AgreementAmount);
            report.TotalReceivedThisWeek = report.NewExpenses.Sum(e => e.ReceivedAmount);
            report.TotalCostsThisWeek = report.NewCosts.Sum(c => c.Amount);
            report.TotalOutstandingAmount = _context.Expenses
                .Sum(e => e.RemainingAmount ?? (e.AgreementAmount - e.ReceivedAmount));

            // 11. Performance Metrics
            report.AverageStageCompletionTime = report.CompletedStages.Any()
                ? report.CompletedStages.Average(s => s.DaysToComplete)
                : 0;

            // 12. Top Performing Engineers
            report.TopEngineers = _context.PermitStages
     .Include(ps => ps.Permit)
         .ThenInclude(p => p.ApplicationUser)
     .Where(ps => ps.CompletedAt >= weekStart &&
                  ps.CompletedAt <= weekEnd &&
                  ps.Permit.ApplicationUser != null &&
                  ps.StartedAt != null) // Filter out null StartedAt in database
     .ToList() // Force client evaluation
     .GroupBy(ps => new { ps.Permit.ApplicationUser.Id, ps.Permit.ApplicationUser.FullName })
     .Select(g => new EngineerPerformance
     {
         EngineerName = g.Key.FullName,
         CompletedStages = g.Count(),
         AverageCompletionTime = g.Average(ps => (ps.CompletedAt.Value - ps.StartedAt).Days)
     })
     .OrderByDescending(e => e.CompletedStages)
     .Take(5)
     .ToList();
            // 13. Unchecked Permits (using isChecked property)
            report.UncheckedPermits = _context.Permits
                .Include(p => p.Client)
                .Include(p => p.ApplicationUser)
                .Where(p => !p.isChecked)
                .Count();

            return report;
        }

        private string BuildReportMessage(WeeklyReportData report)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"📊 التقرير الأسبوعي للفترة من {report.ReportPeriod}");
            sb.AppendLine("──────────");

            // Summary
            sb.AppendLine("📈 ملخص الأنشطة:");
            sb.AppendLine($"   • رخص جديدة: {report.NewPermits.Count}");
            sb.AppendLine($"   • رخص مؤرشفة: {report.ArchivedPermits.Count}");
            sb.AppendLine($"   • مراحل مكتملة: {report.CompletedStages.Count}");
            sb.AppendLine($"   • مراحل متأخرة حالياً: {report.OverdueStages.Count}");
            sb.AppendLine($"   • رخص غير مراجعة: {report.UncheckedPermits}");
            sb.AppendLine();

            // By Type
            sb.AppendLine("📋 تحليل الرخص حسب النوع:");
            foreach (var type in report.PermitsByType)
                sb.AppendLine($"   • {type.PermitType}: {type.Count} رخصة (قيمة: {type.TotalValue:N2} ج.م)");
            sb.AppendLine();

            // Stage Distribution
            sb.AppendLine("📊 توزيع الرخص حسب المرحلة الحالية:");
            foreach (var stage in report.StageDistribution)
                sb.AppendLine($"   • المرحلة {stage.StageNumber}: {stage.Count} رخصة");
            sb.AppendLine();

            // Status Analysis
            sb.AppendLine("📈 تحليل حسب الحالة:");
            foreach (var status in report.PermitsByStatus)
                sb.AppendLine($"   • {status.Status}: {status.Count} رخصة");
            sb.AppendLine();

            // Finance
            sb.AppendLine("💰 الملخص المالي:");
            sb.AppendLine($"   • إجمالي الاتفاقيات الجديدة: {report.TotalNewAgreements:N2} ج.م");
            sb.AppendLine($"   • إجمالي المبالغ المستلمة: {report.TotalReceivedThisWeek:N2} ج.م");
            sb.AppendLine($"   • إجمالي المصروفات: {report.TotalCostsThisWeek:N2} ج.م");
            sb.AppendLine($"   • إجمالي المبالغ المستحقة: {report.TotalOutstandingAmount:N2} ج.م");
            sb.AppendLine();

            // KPIs
            sb.AppendLine("⏱️ مؤشرات الأداء:");
            sb.AppendLine($"   • متوسط وقت إنجاز المراحل: {report.AverageStageCompletionTime:F1} يوم");
            sb.AppendLine();

            // Top engineers
            if (report.TopEngineers.Any())
            {
                sb.AppendLine("🏆 أفضل المهندسين هذا الأسبوع:");
                foreach (var engineer in report.TopEngineers)
                    sb.AppendLine($"   • {engineer.EngineerName}: {engineer.CompletedStages} مرحلة (متوسط {engineer.AverageCompletionTime:F1} يوم)");
                sb.AppendLine();
            }

            // New agreements
            if (report.NewExpenses.Any())
            {
                sb.AppendLine("💳 تفاصيل الاتفاقيات الجديدة:");
                foreach (var expense in report.NewExpenses.Take(5))
                    sb.AppendLine($"   • {expense.ClientName} ({expense.PermitType}): {expense.AgreementAmount:N2} ج.م");
                if (report.NewExpenses.Count > 5)
                    sb.AppendLine($"   ... و {report.NewExpenses.Count - 5} اتفاقيات أخرى");
                sb.AppendLine();
            }

            // New costs
            if (report.NewCosts.Any())
            {
                sb.AppendLine("💸 المصروفات الجديدة:");
                foreach (var cost in report.NewCosts.Take(5))
                    sb.AppendLine($"   • {cost.ClientName}: {cost.Amount:N2} ج.م ({cost.AmountFor})");
                if (report.NewCosts.Count > 5)
                    sb.AppendLine($"   ... و {report.NewCosts.Count - 5} مصروفات أخرى");
                sb.AppendLine();
            }

            // Alerts
            if (report.OverdueStages.Any())
            {
                sb.AppendLine("⚠️ تنبيهات مهمة:");
                sb.AppendLine($"   يوجد {report.OverdueStages.Count} مرحلة متأخرة تحتاج للمتابعة العاجلة:");
                foreach (var overdue in report.OverdueStages.Take(3))
                    sb.AppendLine($"   • {overdue.ClientName} - {overdue.StageName} (متأخر {overdue.DaysOverdue} يوم)");
                if (report.OverdueStages.Count > 3)
                    sb.AppendLine($"   ... و {report.OverdueStages.Count - 3} مراحل أخرى متأخرة");
                sb.AppendLine();
            }

            // Unchecked reminder
            if (report.UncheckedPermits > 0)
                sb.AppendLine($"📋 تذكير: يوجد {report.UncheckedPermits} رخصة تحتاج للمراجعة");

            return sb.ToString();
        }

    }
    public class WeeklyReportData
    {
        public string ReportPeriod { get; set; } = string.Empty;

        // Collections
        public List<PermitSummary> NewPermits { get; set; } = new();
        public List<PermitSummary> ArchivedPermits { get; set; } = new();
        public List<StageSummary> CompletedStages { get; set; } = new();
        public List<StageSummary> OverdueStages { get; set; } = new();
        public List<ExpenseSummary> NewExpenses { get; set; } = new();
        public List<CostSummary> NewCosts { get; set; } = new();
        public List<EngineerPerformance> TopEngineers { get; set; } = new();
        public List<StatusSummary> PermitsByStatus { get; set; } = new();
        public List<TypeSummary> PermitsByType { get; set; } = new();
        public List<StageDistribution> StageDistribution { get; set; } = new();

        // Financial Totals
        public decimal TotalNewAgreements { get; set; }
        public decimal TotalReceivedThisWeek { get; set; }
        public decimal TotalCostsThisWeek { get; set; }
        public decimal TotalOutstandingAmount { get; set; }

        // Performance Metrics
        public double AverageStageCompletionTime { get; set; }
        public int UncheckedPermits { get; set; }

        // Calculated Properties
        public decimal NetProfit => TotalReceivedThisWeek - TotalCostsThisWeek;
        public int TotalActivePermits => NewPermits.Count - ArchivedPermits.Count;
        public bool HasCriticalIssues => OverdueStages.Count > 0 || UncheckedPermits > 0;
    }

    /// <summary>
    /// Summary information for permit records
    /// </summary>
    public class PermitSummary
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string EngineerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PermitType { get; set; } = string.Empty;
        public int CurrentStage { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }

        // Helper Properties
        public string FormattedCreatedAt => CreatedAt?.ToString("dd/MM/yyyy") ?? "غير محدد";
        public string FormattedArchivedAt => ArchivedAt?.ToString("dd/MM/yyyy") ?? "غير محدد";
        public bool IsNewThisWeek => CreatedAt?.Date >= DateTime.Now.AddDays(-7).Date;
        public bool IsArchivedThisWeek => ArchivedAt?.Date >= DateTime.Now.AddDays(-7).Date;
    }

    /// <summary>
    /// Summary information for permit stage records
    /// </summary>
    public class StageSummary
    {
        public int Id { get; set; }
        public string StageName { get; set; } = string.Empty;
        public int PermitId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string EngineerName { get; set; } = string.Empty;
        public string PermitType { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public int DaysToComplete { get; set; }
        public int DaysOverdue { get; set; }

        // Helper Properties
        public string FormattedCompletedAt => CompletedAt?.ToString("dd/MM/yyyy") ?? "غير مكتمل";
        public string FormattedDueDate => DueDate?.ToString("dd/MM/yyyy") ?? "غير محدد";
        public bool IsOverdue => DaysOverdue > 0;
        public bool IsCompletedOnTime => CompletedAt.HasValue && DaysOverdue == 0;
        public string PerformanceStatus => IsOverdue ? "متأخر" : IsCompletedOnTime ? "في الوقت" : "قيد التنفيذ";
        public string PerformanceClass => IsOverdue ? "danger" : IsCompletedOnTime ? "success" : "warning";
    }

    /// <summary>
    /// Summary information for expense records
    /// </summary>
    public class ExpenseSummary
    {
        public int Id { get; set; }
        public int PermitId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string PermitType { get; set; } = string.Empty;
        public decimal AgreementAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime? AddedAt { get; set; }

        // Helper Properties
        public string FormattedAddedAt => AddedAt?.ToString("dd/MM/yyyy") ?? "غير محدد";
        public decimal PaymentProgress => AgreementAmount > 0 ? (ReceivedAmount / AgreementAmount) * 100 : 0;
        public bool IsFullyPaid => RemainingAmount <= 0;
        public bool HasOutstandingAmount => RemainingAmount > 0;
        public string PaymentStatus => IsFullyPaid ? "مكتمل" : HasOutstandingAmount ? "متبقي" : "غير محدد";
        public string PaymentStatusClass => IsFullyPaid ? "success" : HasOutstandingAmount ? "warning" : "secondary";
    }

    /// <summary>
    /// Summary information for cost records
    /// </summary>
    public class CostSummary
    {
        public int Id { get; set; }
        public int PermitId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string PermitType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AmountFor { get; set; } = string.Empty;
        public DateTime? AddedAt { get; set; }

        // Helper Properties
        public string FormattedAddedAt => AddedAt?.ToString("dd/MM/yyyy") ?? "غير محدد";
        public string FormattedAmount => $"{Amount:N2} ج.م";
        public bool IsHighCost => Amount > 10000; // Configurable threshold
        public bool IsRecentCost => AddedAt?.Date >= DateTime.Now.AddDays(-7).Date;
    }

    /// <summary>
    /// Performance metrics for engineers
    /// </summary>
    public class EngineerPerformance
    {
        public string EngineerName { get; set; } = string.Empty;
        public int CompletedStages { get; set; }
        public double AverageCompletionTime { get; set; }
        public int OverdueStages { get; set; }
        public int OnTimeStages { get; set; }
        public decimal TotalRevenue { get; set; }

        // Helper Properties
        public double SuccessRate => (CompletedStages + OverdueStages) > 0
            ? (double)OnTimeStages / (OnTimeStages + OverdueStages) * 100
            : 0;
        public string PerformanceGrade => SuccessRate >= 90 ? "ممتاز" :
                                        SuccessRate >= 80 ? "جيد جداً" :
                                        SuccessRate >= 70 ? "جيد" :
                                        SuccessRate >= 60 ? "مقبول" : "يحتاج تحسين";
        public string PerformanceClass => SuccessRate >= 90 ? "success" :
                                        SuccessRate >= 70 ? "primary" :
                                        SuccessRate >= 60 ? "warning" : "danger";
        public bool IsTopPerformer => CompletedStages >= 10 && SuccessRate >= 85;
    }

    /// <summary>
    /// Summary of permits by status
    /// </summary>
    public class StatusSummary
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }

        // Helper Properties
        public string StatusDisplayName => Status switch
        {
            "Active" => "نشط",
            "Archived" => "مؤرشف",
            "Pending" => "معلق",
            "InProgress" => "قيد التنفيذ",
            "Completed" => "مكتمل",
            "Cancelled" => "ملغي",
            _ => Status
        };

        public string StatusClass => Status switch
        {
            "Active" or "InProgress" => "primary",
            "Completed" => "success",
            "Archived" => "info",
            "Pending" => "warning",
            "Cancelled" => "danger",
            _ => "secondary"
        };
    }

    /// <summary>
    /// Summary of permits by type
    /// </summary>
    public class TypeSummary
    {
        public string PermitType { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AverageValue { get; set; }

        // Helper Properties
        public string FormattedTotalValue => $"{TotalValue:N2} ج.م";
        public string FormattedAverageValue => $"{AverageValue:N2} ج.م";
        public bool IsHighValueType => TotalValue > 100000; // Configurable threshold
        public string ValueCategory => TotalValue > 200000 ? "عالي القيمة" :
                                     TotalValue > 50000 ? "متوسط القيمة" : "منخفض القيمة";
    }

    /// <summary>
    /// Distribution of permits by current execution stage
    /// </summary>
    public class StageDistribution
    {
        public int StageNumber { get; set; }
        public int Count { get; set; }
        public string StageName { get; set; } = string.Empty;
        public decimal Percentage { get; set; }

        // Helper Properties
        public string StageDisplayName => StageName.IsNullOrEmpty() ? $"المرحلة {StageNumber}" : StageName;
        public bool IsBottleneck => Count > 20; // Configurable threshold
        public string DistributionStatus => IsBottleneck ? "اختناق" : Count > 10 ? "عادي" : "قليل";
        public string DistributionClass => IsBottleneck ? "danger" : Count > 10 ? "primary" : "secondary";
    }

    /// <summary>
    /// Financial analysis summary
    /// </summary>
    public class FinancialAnalysis
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public int ProfitablePermits { get; set; }
        public int LossPermits { get; set; }

        // Helper Properties
        public string FormattedRevenue => $"{TotalRevenue:N2} ج.م";
        public string FormattedCosts => $"{TotalCosts:N2} ج.م";
        public string FormattedProfit => $"{NetProfit:N2} ج.م";
        public string ProfitStatus => NetProfit > 0 ? "ربح" : NetProfit < 0 ? "خسارة" : "متوازن";
        public string ProfitClass => NetProfit > 0 ? "success" : NetProfit < 0 ? "danger" : "warning";
        public bool IsHealthyProfit => ProfitMargin > 20;
    }

    /// <summary>
    /// Comparison with previous period
    /// </summary>
    public class PeriodComparison
    {
        public string MetricName { get; set; } = string.Empty;
        public decimal CurrentValue { get; set; }
        public decimal PreviousValue { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercentage { get; set; }

        // Helper Properties
        public bool IsImprovement => Change > 0;
        public string ChangeDirection => Change > 0 ? "زيادة" : Change < 0 ? "انخفاض" : "ثابت";
        public string ChangeClass => Change > 0 ? "success" : Change < 0 ? "danger" : "secondary";
        public string FormattedChange => $"{Math.Abs(ChangePercentage):F1}%";
    }
}

// Extension methods for helper functionality
public static class ReportExtensions
{
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static string ToArabicNumber(this int number)
    {
        return number.ToString("N0");
    }

    public static string ToArabicCurrency(this decimal amount)
    {
        return $"{amount:N2} ج.م";
    }
}
