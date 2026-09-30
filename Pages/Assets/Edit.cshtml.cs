using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Assets;

public class EditModel(AssetService svc) : PageModel
{
    [BindProperty] public Asset Asset { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        var a = await svc.GetByIdAsync(id);
        if (a == null) return NotFound();
        Asset = a;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        ModelState.Remove("Asset.Employee");
        ModelState.Remove("Asset.AssetAssignments");
        if (!ModelState.IsValid) return Page();
        var (ok, msg) = await svc.UpdateAsync(Asset);
        if (!ok) { ModelState.AddModelError(string.Empty, msg); return Page(); }
        TempData["Success"] = msg;
        return RedirectToPage("./Details", new { id = Asset.Id });
    }
}
