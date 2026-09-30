using System.Security.Cryptography;
using System.Text;

namespace ITAssetHelpdesk.Services;

public static class PasswordHasher
{
    private const string Salt = "ITAssetHelpdesk_2026_Salt";

    public static string Hash(string password)
    {
        var combined = $"{Salt}:{password}";
        var bytes = Encoding.UTF8.GetBytes(combined);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    public static bool Verify(string password, string hash) =>
        Hash(password).Equals(hash, StringComparison.OrdinalIgnoreCase);
}
