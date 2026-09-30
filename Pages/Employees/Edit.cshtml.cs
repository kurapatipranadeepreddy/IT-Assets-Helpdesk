using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Employees;

public class EditModel(EmployeeService svc) : PageModel
{
    [BindProperty] public Employee Employee { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        var e = await svc.GetByIdAsync(id);
        if (e == null) return NotFound();
        Employee = e;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        if (!ModelState.IsValid) return Page();
        var (ok, msg) = await svc.UpdateAsync(Employee);
        if (!ok) { ModelState.AddModelError(string.Empty, msg); return Page(); }
        TempData["Success"] = msg;
        return RedirectToPage("./Index");
    }
}
