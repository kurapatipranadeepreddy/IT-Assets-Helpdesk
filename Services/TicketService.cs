using ITAssetHelpdesk.Data;
using ITAssetHelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAssetHelpdesk.Services;

public class TicketService(AppDbContext db)
{
    public async Task<List<Ticket>> GetAllAsync(string? search = null, string? status = null, string? priority = null, string? category = null)
    {
        var q = db.Tickets.Include(t => t.CreatedBy).Include(t => t.AssignedTechnician).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) { var s = search.ToLower(); q = q.Where(t => t.TicketNumber.ToLower().Contains(s) || t.Title.ToLower().Contains(s) || t.Description.ToLower().Contains(s)); }
        if (!string.IsNullOrWhiteSpace(status)   && status   != "All") q = q.Where(t => t.Status   == status);
        if (!string.IsNullOrWhiteSpace(priority) && priority != "All") q = q.Where(t => t.Priority == priority);
        if (!string.IsNullOrWhiteSpace(category) && category != "All") q = q.Where(t => t.Category == category);
        return await q.OrderByDescending(t => t.UpdatedDate).ToListAsync();
    }

    public async Task<Ticket?> GetByIdAsync(int id) =>
        await db.Tickets.Include(t => t.CreatedBy).Include(t => t.AssignedTechnician).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<string> NextNumberAsync()
    {
        var count = await db.Tickets.CountAsync();
        var n = $"TKT-{count + 1:D4}";
        while (await db.Tickets.AnyAsync(t => t.TicketNumber == n)) { count++; n = $"TKT-{count + 1:D4}"; }
        return n;
    }

    public async Task<(bool ok, string msg)> CreateAsync(Ticket t)
    {
        t.TicketNumber = await NextNumberAsync();
        t.CreatedDate = t.UpdatedDate = DateTime.Now;
        db.Tickets.Add(t); await db.SaveChangesAsync();
        return (true, $"Ticket {t.TicketNumber} created.");
    }

    public async Task<(bool ok, string msg)> UpdateAsync(Ticket t)
    {
        t.UpdatedDate = DateTime.Now;
        if (t.Status == "Resolved" && !t.ResolvedDate.HasValue) t.ResolvedDate = DateTime.Now;
        db.Tickets.Update(t); await db.SaveChangesAsync();
        return (true, "Ticket updated.");
    }

    public async Task<(bool ok, string msg)> ChangeStatusAsync(int id, string status, string? notes = null)
    {
        var t = await db.Tickets.FindAsync(id);
        if (t == null) return (false, "Ticket not found.");
        t.Status = status; t.UpdatedDate = DateTime.Now;
        if (status == "Resolved") { t.ResolvedDate = DateTime.Now; if (!string.IsNullOrWhiteSpace(notes)) t.ResolutionNotes = notes; }
        await db.SaveChangesAsync();
        return (true, $"Status changed to {status}.");
    }

    public async Task<(bool ok, string msg)> AssignTechAsync(int id, int techId)
    {
        var t = await db.Tickets.FindAsync(id);
        var u = await db.Users.FindAsync(techId);
        if (t == null || u == null) return (false, "Not found.");
        t.AssignedTechnicianId = techId; t.UpdatedDate = DateTime.Now;
        if (t.Status == "Open") t.Status = "In Progress";
        await db.SaveChangesAsync();
        return (true, $"Assigned to {u.FullName}.");
    }

    public async Task<(bool ok, string msg)> DeleteAsync(int id)
    {
        var t = await db.Tickets.FindAsync(id);
        if (t == null) return (false, "Not found.");
        db.Tickets.Remove(t); await db.SaveChangesAsync();
        return (true, "Ticket deleted.");
    }

    public async Task<List<AppUser>> GetTechniciansAsync() =>
        await db.Users.Where(u => u.IsActive).OrderBy(u => u.FullName).ToListAsync();
}
