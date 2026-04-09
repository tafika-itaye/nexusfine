using NexusFine.Mobile.Models;
using NexusFine.Mobile.Services;

namespace NexusFine.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly SessionService _session;

    public LoginPage(SessionService session)
    {
        InitializeComponent();
        _session = session;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var badge = BadgeEntry.Text?.Trim();
        var name  = NameEntry.Text?.Trim();

        if (string.IsNullOrEmpty(badge) || string.IsNullOrEmpty(name))
        {
            ShowError("Please enter your badge number and full name.");
            return;
        }

        LoginButton.IsEnabled = false;
        LoginButton.Text = "Signing in...";

        // Demo auth — real system will POST to /api/auth/login and receive JWT
        var officer = new Officer
        {
            Id          = 1, // real system: from JWT claim
            BadgeNumber = badge.ToUpper(),
            FullName    = name,
            Rank        = "Constable",
            Status      = "Active"
        };

        _session.Login(officer);

        // Navigate to shell
        Application.Current!.MainPage = new AppShell();

        LoginButton.IsEnabled = true;
        LoginButton.Text = "Sign In →";
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text    = message;
        ErrorFrame.IsVisible = true;
    }
}
