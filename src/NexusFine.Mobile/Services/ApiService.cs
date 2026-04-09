using System.Net.Http.Json;
using NexusFine.Mobile.Models;

namespace NexusFine.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("NexusFineAPI");
    }

    // ── FINES ─────────────────────────────────────────────────
    public async Task<Fine?> LookupFineAsync(string type, string value)
    {
        try
        {
            return await _http.GetFromJsonAsync<Fine>(
                $"api/fines/lookup?type={type}&value={Uri.EscapeDataString(value)}");
        }
        catch { return null; }
    }

    public async Task<PagedResult<Fine>?> GetOfficerFinesAsync(int officerId, int page = 1)
    {
        try
        {
            return await _http.GetFromJsonAsync<PagedResult<Fine>>(
                $"api/fines?officerId={officerId}&page={page}&pageSize=20");
        }
        catch { return null; }
    }

    public async Task<Fine?> CreateFineAsync(CreateFineRequest req)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/fines", req);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Fine>();
            return null;
        }
        catch { return null; }
    }

    // ── OFFENCE CODES ─────────────────────────────────────────
    public async Task<List<OffenceCodeDto>> GetOffenceCodesAsync()
    {
        try
        {
            // Offence codes are seeded — cache them locally
            var codes = await _http.GetFromJsonAsync<List<OffenceCodeDto>>("api/offencecodes");
            return codes ?? new();
        }
        catch { return new(); }
    }

    // ── DASHBOARD ─────────────────────────────────────────────
    public async Task<KpiDto?> GetKpisAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<KpiDto>("api/dashboard/kpis");
        }
        catch { return null; }
    }

    public async Task<List<Fine>> GetRecentFinesAsync(int limit = 10)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<List<Fine>>(
                $"api/dashboard/recent?limit={limit}");
            return result ?? new();
        }
        catch { return new(); }
    }

    // ── LOCATION ──────────────────────────────────────────────
    public async Task<bool> UpdateLocationAsync(int officerId, double lat, double lng, string locationName)
    {
        try
        {
            var response = await _http.PatchAsJsonAsync(
                $"api/officers/{officerId}/location",
                new { Latitude = lat, Longitude = lng, LocationName = locationName });
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}
