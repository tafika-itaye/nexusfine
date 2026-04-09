using NexusFine.Mobile.Services;

namespace NexusFine.Mobile.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly ApiService     _api;
    private readonly SessionService _session;

    public DashboardPage(ApiService api, SessionService session)
    {
        InitializeComponent();
        _api     = api;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        SetGreeting();
        await LoadDataAsync();
    }

    private void SetGreeting()
    {
        var hour = DateTime.Now.Hour;
        var greeting = hour < 12 ? "Good morning,"
                     : hour < 17 ? "Good afternoon,"
                     : "Good evening,";

        GreetingLabel.Text    = greeting;
        OfficerNameLabel.Text = _session.CurrentOfficer?.FullName ?? "Officer";
        BadgeLabel.Text       = $"Badge #{_session.CurrentOfficer?.BadgeNumber}";
    }

    private async Task LoadDataAsync()
    {
        LoadingIndicator.IsVisible = true;
        RecentFinesView.IsVisible  = false;

        var kpisTask   = _api.GetKpisAsync();
        var recentTask = _api.GetRecentFinesAsync(8);

        await Task.WhenAll(kpisTask, recentTask);

        var kpis   = kpisTask.Result;
        var recent = recentTask.Result;

        if (kpis is not null)
        {
            IssuedTodayLabel.Text = kpis.IssuedToday.ToString("N0");
            RevenueLabel.Text     = $"MK {kpis.RevenueMonth:N0}";
            CollectionLabel.Text  = $"{kpis.CollectionRate:N1}%";
            OnDutyLabel.Text      = kpis.OfficersOnDuty.ToString();
        }

        RecentFinesView.ItemsSource = recent;
        LoadingIndicator.IsVisible  = false;
        RecentFinesView.IsVisible   = true;
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadDataAsync();
        Refresher.IsRefreshing = false;
    }

    private async void OnIssueFineClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//issuefine");
    }

    private async void OnLookupClicked(object sender, EventArgs e)
    {
        var plate = await DisplayPromptAsync(
            "Plate Lookup",
            "Enter vehicle registration plate:",
            placeholder: "e.g. MWK 1234 A",
            keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(plate)) return;

        var fine = await _api.LookupFineAsync("plate", plate.Trim().ToUpper());

        if (fine is null)
        {
            await DisplayAlert("Not Found", "No fine found for that plate number.", "OK");
            return;
        }

        await DisplayAlert(
            $"Fine: {fine.ReferenceNumber}",
            $"Plate: {fine.PlateNumber}\n" +
            $"Driver: {fine.DriverName}\n" +
            $"Offence: {fine.OffenceCode?.Name}\n" +
            $"Amount: {fine.FormattedAmount}\n" +
            $"Status: {fine.Status}",
            "Close");
    }
}
