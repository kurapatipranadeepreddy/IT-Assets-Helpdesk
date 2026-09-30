using ITAssetHelpdesk.Data;
using ITAssetHelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAssetHelpdesk.Services;

public class DashboardService(AppDbContext db)
{
    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var vm = new DashboardViewModel
        {
            TotalAssets          = await db.Assets.CountAsync(),
            AssignedAssets       = await db.Assets.CountAsync(a => a.Status == "Assigned"),
            AvailableAssets      = await db.Assets.CountAsync(a => a.Status == "Available"),
            UnderRepairAssets    = await db.Assets.CountAsync(a => a.Status == "Under Repair"),
            TotalEmployees       = await db.Employees.CountAsync(),
            ActiveEmployees      = await db.Employees.CountAsync(e => e.IsActive),
            OpenTickets          = await db.Tickets.CountAsync(t => t.Status != "Resolved" && t.Status != "Closed"),
            HighPriorityTickets  = await db.Tickets.CountAsync(t => (t.Priority == "High" || t.Priority == "Critical") && t.Status != "Closed"),
            CriticalTickets      = await db.Tickets.CountAsync(t => t.Priority == "Critical" && t.Status != "Closed"),
            ResolvedTickets      = await db.Tickets.CountAsync(t => t.Status == "Resolved" || t.Status == "Closed"),
        };

        var firstOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        vm.TicketsResolvedThisMonth = await db.Tickets
            .CountAsync(t => (t.Status == "Resolved" || t.Status == "Closed") && t.ResolvedDate >= firstOfMonth);

        var thirtyDays = DateTime.Today.AddDays(30);
        vm.WarrantyExpiringSoon = await db.Assets
            .CountAsync(a => a.WarrantyExpiryDate.HasValue && a.WarrantyExpiryDate > DateTime.Today && a.WarrantyExpiryDate <= thirtyDays);

        vm.AssetsByCategory = await db.Assets.GroupBy(a => a.Category).Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        vm.AssetsByStatus = await db.Assets.GroupBy(a => a.Status).Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        vm.TicketsByPriority = await db.Tickets.GroupBy(t => t.Priority).Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        vm.TicketsByStatus = await db.Tickets.GroupBy(t => t.Status).Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        // Recent activity
        var assignments = await db.AssetAssignments.Include(a => a.Asset).Include(a => a.Employee)
            .OrderByDescending(a => a.AssignedDate).Take(5).ToListAsync();
        foreach (var aa in assignments)
            vm.RecentActivity.Add(new() { Type="Assignment", Description=$"'{aa.Asset.Name}' assigned to {aa.Employee.FullName}", Timestamp=aa.AssignedDate, Icon="bi-box", BadgeClass="bg-primary" });

        var tickets = await db.Tickets.OrderByDescending(t => t.UpdatedDate).Take(7).ToListAsync();
        foreach (var t in tickets)
            vm.RecentActivity.Add(new() { Type="Ticket", Description=$"[{t.TicketNumber}] {t.Title} — {t.Status}", Timestamp=t.UpdatedDate, Icon="bi-ticket", BadgeClass="bg-secondary" });

        vm.RecentActivity = vm.RecentActivity.OrderByDescending(x => x.Timestamp).Take(10).ToList();

        // Warranty alerts
        vm.WarrantyAlerts = await db.Assets
            .Where(a => a.WarrantyExpiryDate.HasValue && a.WarrantyExpiryDate > DateTime.Today && a.WarrantyExpiryDate <= thirtyDays)
            .OrderBy(a => a.WarrantyExpiryDate)
            .Select(a => new WarrantyAlertItem { AssetTag=a.AssetTag, AssetName=a.Name, ExpiryDate=a.WarrantyExpiryDate, DaysRemaining=(int)((a.WarrantyExpiryDate!.Value - DateTime.Today).TotalDays) })
            .ToListAsync();

        return vm;
    }
}
