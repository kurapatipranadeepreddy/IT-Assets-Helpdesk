using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Assets;

public class CreateModel(AssetService svc) : PageModel
{
    [BindProperty] public Asset Asset { get; set; } = new() { PurchaseDate = DateTime.Today };

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        Asset.AssetTag = await svc.NextTagAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        ModelState.Remove("Asset.Employee");
        if (!ModelState.IsValid) return Page();
        var (ok, msg) = await svc.AddAsync(Asset);
        if (!ok) { ModelState.AddModelError(string.Empty, msg); return Page(); }
        TempData["Success"] = msg;
        return RedirectToPage("./Index");
    }
}
