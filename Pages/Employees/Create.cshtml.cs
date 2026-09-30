using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Employees;

public class CreateModel(EmployeeService svc) : PageModel
{
    [BindProperty] public Employee Employee { get; set; } = new() { DateJoined = DateTime.Today };
    public string? SuggestedCode { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        SuggestedCode = await svc.NextCodeAsync();
        Employee.EmployeeCode = SuggestedCode;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Administrator") return Forbid();
        if (!ModelState.IsValid) return Page();
        var (ok, msg) = await svc.AddAsync(Employee);
        if (!ok) { ModelState.AddModelError(string.Empty, msg); return Page(); }
        TempData["Success"] = msg;
        return RedirectToPage("./Index");
    }
}
