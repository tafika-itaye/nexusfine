using System.Net.Http.Headers;

namespace NexusFine.Admin.Services;

/// <summary>
/// Attaches "Authorization: Bearer {token}" to every outgoing HttpClient
/// request when the user is authenticated. Reads the token from the
/// circuit-scoped <see cref="JwtAuthStateProvider"/>.
/// </summary>
public class JwtBearerHandler : DelegatingHandler
{
    private readonly JwtAuthStateProvider _authProvider;

    public JwtBearerHandler(JwtAuthStateProvider authProvider)
    {
        _authProvider = authProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _authProvider.AccessToken;
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}
