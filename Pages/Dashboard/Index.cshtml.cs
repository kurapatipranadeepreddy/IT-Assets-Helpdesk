using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Dashboard;

public class IndexModel(DashboardService dash) : PageModel
{
    public DashboardViewModel Stats { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        Stats = await dash.GetDashboardAsync();
        return Page();
    }
}
