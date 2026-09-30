using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Tickets;

public class DetailsModel(TicketService svc) : PageModel
{
    public Ticket Ticket { get; set; } = null!;
    public List<AppUser> Technicians { get; set; } = new();
    [BindProperty] public int TechId { get; set; }
    [BindProperty] public string NewStatus { get; set; } = string.Empty;
    [BindProperty] public string ResNotes { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        var t = await svc.GetByIdAsync(id);
        if (t == null) return NotFound();
        Ticket = t;
        Technicians = await svc.GetTechniciansAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAssignTechAsync(int id)
    {
        var (ok, msg) = await svc.AssignTechAsync(id, TechId);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(int id)
    {
        var (ok, msg) = await svc.ChangeStatusAsync(id, NewStatus, ResNotes);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToPage(new { id });
    }
}
