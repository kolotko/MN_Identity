using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _client;
    
    public CookieAuthenticationStateProvider(HttpClient client)
    {
        _client = client;
    }

    public async Task Login()
    {
        var result = await _client.PostAsJsonAsync<Dictionary<string,string>>("/api/login", new ());
        base.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUsersStateAsync();
        return new AuthenticationState(user);
    }

    private async Task<ClaimsPrincipal> GetUsersStateAsync()
    {
        var user = await _client.GetFromJsonAsync<Dictionary<string,string>>("/api/user");

        return new ClaimsPrincipal(
            new ClaimsIdentity(
                user?.Select(kv => new Claim(kv.Key, kv.Value)),
                "cookie"
            )
        );
    }
}