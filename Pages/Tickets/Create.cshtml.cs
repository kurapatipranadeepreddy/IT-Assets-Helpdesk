using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Tickets;

public class CreateModel(TicketService svc) : PageModel
{
    [BindProperty] public Ticket Ticket { get; set; } = new();

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        ModelState.Remove("Ticket.TicketNumber");
        ModelState.Remove("Ticket.CreatedBy");
        ModelState.Remove("Ticket.AssignedTechnician");
        if (!ModelState.IsValid) return Page();
        Ticket.CreatedById = int.Parse(HttpContext.Session.GetString("UserId")!);
        var (ok, msg) = await svc.CreateAsync(Ticket);
        if (!ok) { ModelState.AddModelError(string.Empty, msg); return Page(); }
        TempData["Success"] = msg;
        return RedirectToPage("./Index");
    }
}
