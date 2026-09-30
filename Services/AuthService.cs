using ITAssetHelpdesk.Data;
using ITAssetHelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAssetHelpdesk.Services;

public class AuthService(AppDbContext db)
{
    public async Task<AppUser?> ValidateAsync(string username, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        if (user == null) return null;
        return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
    }
}
