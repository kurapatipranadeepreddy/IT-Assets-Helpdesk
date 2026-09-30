using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Tickets;

public class EditModel(TicketService svc) : PageModel
{
    [BindProperty] public Ticket Ticket { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        var t = await svc.GetByIdAsync(id);
        if (t == null) return NotFound();
        Ticket = t;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        ModelState.Remove("Ticket.CreatedBy");
        ModelState.Remove("Ticket.AssignedTechnician");
        ModelState.Remove("Ticket.TicketNumber");
        if (!ModelState.IsValid) return Page();
        var (ok, msg) = await svc.UpdateAsync(Ticket);
        if (!ok) { ModelState.AddModelError(string.Empty, msg); return Page(); }
        TempData["Success"] = msg;
        return RedirectToPage("./Details", new { id = Ticket.Id });
    }
}
