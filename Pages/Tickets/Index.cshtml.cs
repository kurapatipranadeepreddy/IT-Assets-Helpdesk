using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Tickets;

public class IndexModel(TicketService svc) : PageModel
{
    public List<Ticket> Tickets { get; set; } = new();
    [BindProperty(SupportsGet=true)] public string? Search   { get; set; }
    [BindProperty(SupportsGet=true)] public string? Status   { get; set; }
    [BindProperty(SupportsGet=true)] public string? Priority { get; set; }
    [BindProperty(SupportsGet=true)] public string? Category { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        Tickets = await svc.GetAllAsync(Search, Status, Priority, Category);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        var (ok, msg) = await svc.DeleteAsync(id);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToPage();
    }
}
