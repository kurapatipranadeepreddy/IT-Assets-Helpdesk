using ITAssetHelpdesk.Data;
using ITAssetHelpdesk.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ITAssetHelpdesk.Pages.Reports;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<Asset>    Assets    { get; set; } = new();
    public List<Ticket>   Tickets   { get; set; } = new();
    public List<Employee> Employees { get; set; } = new();
    [BindProperty(SupportsGet=true)] public string Tab      { get; set; } = "assets";
    [BindProperty(SupportsGet=true)] public string? AStatus { get; set; }
    [BindProperty(SupportsGet=true)] public string? ACat    { get; set; }
    [BindProperty(SupportsGet=true)] public string? TStatus { get; set; }
    [BindProperty(SupportsGet=true)] public string? TPrio   { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        var aq = db.Assets.Include(a => a.Employee).AsQueryable();
        if (!string.IsNullOrEmpty(AStatus) && AStatus!="All") aq = aq.Where(a=>a.Status==AStatus);
        if (!string.IsNullOrEmpty(ACat)    && ACat!="All")    aq = aq.Where(a=>a.Category==ACat);
        Assets = await aq.OrderBy(a=>a.AssetTag).ToListAsync();

        var tq = db.Tickets.Include(t=>t.AssignedTechnician).AsQueryable();
        if (!string.IsNullOrEmpty(TStatus) && TStatus!="All") tq = tq.Where(t=>t.Status==TStatus);
        if (!string.IsNullOrEmpty(TPrio)   && TPrio!="All")   tq = tq.Where(t=>t.Priority==TPrio);
        Tickets = await tq.OrderByDescending(t=>t.CreatedDate).ToListAsync();

        Employees = await db.Employees.Include(e=>e.Assets).OrderBy(e=>e.FullName).ToListAsync();
        return Page();
    }
}
