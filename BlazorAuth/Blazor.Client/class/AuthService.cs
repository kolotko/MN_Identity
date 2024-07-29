using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;

public class AuthService
{
    private IHttpClientFactory _httpClientFactory;
    private ILocalStorageService _localStorage;
    
    public AuthService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
    {
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorage;
    }
    
    public async Task<bool> LoginAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("Api");
        var loginResponse = await httpClient.PostAsJsonAsync("generate-token", new { });

        if (loginResponse.IsSuccessStatusCode)
        {
            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
            await _localStorage.SetItemAsync("token", tokenResponse.Token);
            return true;
        }

        return false;
    }

    public async Task<bool> RefreshAsync()
    {
        try
        {
            //Tutaj powinna być ścieżka z refresh tokenem jeśli by istniała, tak to po prostu logujemy się jeszcze raz
            return await LoginAsync();
        }
        catch (Exception)
        {
            return false;
        }
        
    }
    
    public async Task<ClaimsPrincipal> GetUsersStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("token");
        if (string.IsNullOrEmpty(token))
        {
            return new ClaimsPrincipal();
        }
        var httpClient = _httpClientFactory.CreateClient("Api");
        var user = await httpClient.GetFromJsonAsync<Dictionary<string,string>>("secure-endpoint");

        return new ClaimsPrincipal(
            new ClaimsIdentity(
                user?.Select(kv => new Claim(kv.Key, kv.Value)),
                "token"
            )
        );
    }
}