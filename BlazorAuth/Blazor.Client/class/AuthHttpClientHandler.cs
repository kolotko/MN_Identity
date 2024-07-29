using System.Net;
using System.Net.Http.Headers;
using Blazored.LocalStorage;

public class AuthHttpClientHandler : DelegatingHandler
{
    private bool _refreshing;
    private ILocalStorageService _localStorage;
    private AuthService _authService;

    public AuthHttpClientHandler(ILocalStorageService localStorage, AuthService authService)
    {
        _localStorage = localStorage;
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("token");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (_refreshing || response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        try
        {
            _refreshing = true;
            if (await _authService.RefreshAsync())
            {
                token = await _localStorage.GetItemAsync<string>("token");
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                response = await base.SendAsync(request, cancellationToken);
            }
        }
        finally
        {
            _refreshing = false;
        }

        return response;
    }
}