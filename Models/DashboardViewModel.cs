namespace ITAssetHelpdesk.Models;

public class DashboardViewModel
{
    // KPI
    public int TotalAssets { get; set; }
    public int AssignedAssets { get; set; }
    public int AvailableAssets { get; set; }
    public int UnderRepairAssets { get; set; }
    public int OpenTickets { get; set; }
    public int HighPriorityTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int CriticalTickets { get; set; }
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int TicketsResolvedThisMonth { get; set; }
    public int WarrantyExpiringSoon { get; set; }

    // Chart data
    public Dictionary<string, int> AssetsByCategory { get; set; } = new();
    public Dictionary<string, int> AssetsByStatus { get; set; } = new();
    public Dictionary<string, int> TicketsByPriority { get; set; } = new();
    public Dictionary<string, int> TicketsByStatus { get; set; } = new();

    // Recent activity
    public List<RecentActivityItem> RecentActivity { get; set; } = new();

    // Warranty alerts
    public List<WarrantyAlertItem> WarrantyAlerts { get; set; } = new();
}

public class RecentActivityItem
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string BadgeClass { get; set; } = string.Empty;
}

public class WarrantyAlertItem
{
    public string AssetTag { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public int DaysRemaining { get; set; }
}
