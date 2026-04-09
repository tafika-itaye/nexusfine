using NexusFine.Mobile.Pages;
using NexusFine.Mobile.Services;

namespace NexusFine.Mobile;

public partial class App : Application
{
    public App(SessionService session, LoginPage loginPage)
    {
        InitializeComponent();

        // Try to restore saved session
        if (session.TryRestoreSession())
            MainPage = new AppShell();
        else
            MainPage = loginPage;
    }
}
