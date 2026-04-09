using NexusFine.Mobile.Models;
using NexusFine.Mobile.Services;

namespace NexusFine.Mobile.Pages;

public partial class IssuFinePage : ContentPage
{
    private readonly ApiService     _api;
    private readonly SessionService _session;
    private List<OffenceCodeDto>    _offenceCodes = new();
    private double? _lat, _lng;

    public IssuFinePage(ApiService api, SessionService session)
    {
        InitializeComponent();
        _api     = api;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOffenceCodesAsync();
        ClearForm();
    }

    private async Task LoadOffenceCodesAsync()
    {
        _offenceCodes = await _api.GetOffenceCodesAsync();

        OffencePicker.ItemsSource  = _offenceCodes.Select(o => $"{o.Code} — {o.Name}").ToList();
        OffencePicker.SelectedIndex = -1;
    }

    private void OnOffenceSelected(object sender, EventArgs e)
    {
        var idx = OffencePicker.SelectedIndex;
        if (idx < 0 || idx >= _offenceCodes.Count) return;

        var code = _offenceCodes[idx];
        AmountLabel.Text    = $"MK {code.DefaultFineAmount:N0}";
        AmountFrame.IsVisible = true;
    }

    private async void OnGetLocationClicked(object sender, EventArgs e)
    {
        try
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout         = TimeSpan.FromSeconds(10)
            });

            if (location is not null)
            {
                _lat = location.Latitude;
                _lng = location.Longitude;
                LocationEntry.Text = $"GPS: {_lat:F4}, {_lng:F4}";
            }
        }
        catch
        {
            await DisplayAlert("Location Error", "Could not get GPS location. Enter manually.", "OK");
        }
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        HideMessages();

        // Validate
        if (string.IsNullOrWhiteSpace(PlateEntry.Text))   { ShowError("Vehicle plate is required."); return; }
        if (string.IsNullOrWhiteSpace(DriverNameEntry.Text)) { ShowError("Driver name is required."); return; }
        if (OffencePicker.SelectedIndex < 0)               { ShowError("Please select an offence."); return; }
        if (string.IsNullOrWhiteSpace(LocationEntry.Text)) { ShowError("Location is required."); return; }

        SubmitButton.IsEnabled = false;
        SubmitButton.Text      = "Issuing fine...";

        var offence = _offenceCodes[OffencePicker.SelectedIndex];

        var req = new CreateFineRequest
        {
            PlateNumber        = PlateEntry.Text.Trim().ToUpper(),
            DriverName         = DriverNameEntry.Text.Trim(),
            OffenceCodeId      = offence.Id,
            OfficerId          = _session.CurrentOfficer?.Id ?? 1,
            Location           = LocationEntry.Text.Trim(),
            VehicleMake        = MakeEntry.Text?.Trim(),
            VehicleColour      = ColourEntry.Text?.Trim(),
            DriverNationalId   = NidEntry.Text?.Trim(),
            DriverPhone        = PhoneEntry.Text?.Trim(),
            Latitude           = _lat,
            Longitude          = _lng,
            Notes              = NotesEditor.Text?.Trim()
        };

        var fine = await _api.CreateFineAsync(req);

        if (fine is not null)
        {
            SuccessRefLabel.Text  = $"Reference: {fine.ReferenceNumber} · Amount: {fine.FormattedAmount}";
            SuccessFrame.IsVisible = true;
            ClearForm();

            // Update officer GPS location
            if (_lat.HasValue && _lng.HasValue)
                await _api.UpdateLocationAsync(req.OfficerId, _lat.Value, _lng.Value, req.Location);
        }
        else
        {
            ShowError("Failed to issue fine. Check your connection and try again.");
        }

        SubmitButton.IsEnabled = true;
        SubmitButton.Text      = "Issue Fine →";
    }

    private void ClearForm()
    {
        PlateEntry.Text        = string.Empty;
        DriverNameEntry.Text   = string.Empty;
        MakeEntry.Text         = string.Empty;
        ColourEntry.Text       = string.Empty;
        NidEntry.Text          = string.Empty;
        PhoneEntry.Text        = string.Empty;
        LocationEntry.Text     = string.Empty;
        NotesEditor.Text       = string.Empty;
        OffencePicker.SelectedIndex = -1;
        AmountFrame.IsVisible  = false;
        _lat = _lng = null;
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text      = message;
        ErrorFrame.IsVisible = true;
    }

    private void HideMessages()
    {
        ErrorFrame.IsVisible   = false;
        SuccessFrame.IsVisible = false;
    }
}
