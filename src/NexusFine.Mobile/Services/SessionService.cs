using NexusFine.Mobile.Models;

namespace NexusFine.Mobile.Services;

public class SessionService
{
    public Officer? CurrentOfficer { get; private set; }
    public bool     IsLoggedIn     => CurrentOfficer is not null;

    public void Login(Officer officer)
    {
        CurrentOfficer = officer;
        Preferences.Set("officer_badge",  officer.BadgeNumber);
        Preferences.Set("officer_name",   officer.FullName);
        Preferences.Set("officer_id",     officer.Id);
    }

    public void Logout()
    {
        CurrentOfficer = null;
        Preferences.Remove("officer_badge");
        Preferences.Remove("officer_name");
        Preferences.Remove("officer_id");
    }

    public bool TryRestoreSession()
    {
        var badge = Preferences.Get("officer_badge", string.Empty);
        var name  = Preferences.Get("officer_name",  string.Empty);
        var id    = Preferences.Get("officer_id",    0);

        if (string.IsNullOrEmpty(badge)) return false;

        CurrentOfficer = new Officer
        {
            Id          = id,
            BadgeNumber = badge,
            FullName    = name,
            Status      = "Active"
        };
        return true;
    }
}
