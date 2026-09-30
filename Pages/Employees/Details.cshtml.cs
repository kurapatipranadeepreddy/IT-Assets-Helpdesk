using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Employees;

public class DetailsModel(EmployeeService svc) : PageModel
{
    public Employee Employee { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        var e = await svc.GetByIdAsync(id);
        if (e == null) return NotFound();
        Employee = e;
        return Page();
    }
}
