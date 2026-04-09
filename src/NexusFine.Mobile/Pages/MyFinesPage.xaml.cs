using NexusFine.Mobile.Services;

namespace NexusFine.Mobile.Pages;

public partial class MyFinesPage : ContentPage
{
    private readonly ApiService     _api;
    private readonly SessionService _session;

    public MyFinesPage(ApiService api, SessionService session)
    {
        InitializeComponent();
        _api     = api;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadFinesAsync();
    }

    private async Task LoadFinesAsync()
    {
        LoadingIndicator.IsVisible = true;
        FinesView.IsVisible        = false;

        var officerId = _session.CurrentOfficer?.Id ?? 1;
        var result    = await _api.GetOfficerFinesAsync(officerId);
        var fines     = result?.Data ?? new();

        TotalLabel.Text  = $"{fines.Count} total";
        UnpaidCount.Text = fines.Count(f => f.Status == "Unpaid").ToString();
        PaidCount.Text   = fines.Count(f => f.Status == "Paid").ToString();
        TotalAmount.Text = $"{fines.Sum(f => f.Amount):N0}";

        FinesView.ItemsSource     = fines;
        LoadingIndicator.IsVisible = false;
        FinesView.IsVisible        = true;
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadFinesAsync();
        Refresher.IsRefreshing = false;
    }
}
