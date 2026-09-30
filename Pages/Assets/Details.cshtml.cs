using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Assets;

public class DetailsModel(AssetService assetSvc, EmployeeService empSvc) : PageModel
{
    public Asset Asset { get; set; } = null!;
    public List<Employee> ActiveEmployees { get; set; } = new();
    [BindProperty] public int AssignEmployeeId { get; set; }
    [BindProperty] public string AssignNotes { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        var a = await assetSvc.GetByIdAsync(id);
        if (a == null) return NotFound();
        Asset = a;
        ActiveEmployees = (await empSvc.GetAllAsync()).Where(e => e.IsActive).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostAssignAsync(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        var (ok, msg) = await assetSvc.AssignAsync(id, AssignEmployeeId, AssignNotes);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnassignAsync(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        var (ok, msg) = await assetSvc.UnassignAsync(id);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToPage(new { id });
    }
}
