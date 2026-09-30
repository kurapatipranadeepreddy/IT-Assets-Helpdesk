using ITAssetHelpdesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITAssetHelpdesk.Pages;

public class LoginModel(AuthService auth) : PageModel
{
    [BindProperty] public string Username { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("UserId") != null) return RedirectToPage("/Dashboard/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await auth.ValidateAsync(Username, Password);
        if (user == null) { ErrorMessage = "Invalid username or password."; return Page(); }
        HttpContext.Session.SetString("UserId",   user.Id.ToString());
        HttpContext.Session.SetString("UserName", user.FullName);
        HttpContext.Session.SetString("UserRole", user.Role);
        return RedirectToPage("/Dashboard/Index");
    }
}
