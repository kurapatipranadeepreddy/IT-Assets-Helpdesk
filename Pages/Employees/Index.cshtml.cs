using ITAssetHelpdesk.Models;
using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages.Employees;

public class IndexModel(EmployeeService svc) : PageModel
{
    public List<Employee> Employees { get; set; } = new();
    public List<string> Departments { get; set; } = new();
    [BindProperty(SupportsGet=true)] public string? Search { get; set; }
    [BindProperty(SupportsGet=true)] public string? Dept { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        Employees   = await svc.GetAllAsync(Search, Dept);
        Departments = await svc.GetDepartmentsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null) return RedirectToPage("/Login");
        var (ok, msg) = await svc.DeleteAsync(id);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToPage();
    }
}
