namespace ITAssetHelpdesk.Pages;

public static class PageHelper
{
    public static bool IsAdmin(Microsoft.AspNetCore.Http.ISession session) =>
        session.GetString("UserRole") == "Administrator";

    public static bool IsLoggedIn(Microsoft.AspNetCore.Http.ISession session) =>
        session.GetString("UserId") != null;
}
