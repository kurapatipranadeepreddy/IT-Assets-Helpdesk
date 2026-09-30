using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Assets;

public class IndexModel(AssetService svc) : PageModel
{
    public List<Asset> Assets { get; set; } = new();
    [BindProperty(SupportsGet=true)] public string? Search   { get; set; }
    [BindProperty(SupportsGet=true)] public string? Category { get; set; }
    [BindProperty(SupportsGet=true)] public string? Status   { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        Assets = await svc.GetAllAsync(Search, Category, Status);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        var (ok, msg) = await svc.DeleteAsync(id);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUnassignAsync(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        var (ok, msg) = await svc.UnassignAsync(id);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToPage();
    }
}
