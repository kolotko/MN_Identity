using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _authService;

    public TokenAuthenticationStateProvider(AuthService authService)
    {
        _authService = authService;
    }
    
    public async Task Login()
    {
        var success = await _authService.LoginAsync();

        if (success)
        {
            base.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // po wejściu na stronę wywoływana jest ta metoda więc jeśli token nie aktywny to należy obsłużyć ścieżkę z wylogowaniem
        var user = await _authService.GetUsersStateAsync();
        return new AuthenticationState(user);
    }

    public void LogOut()
    {
        var emptyState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
        NotifyAuthenticationStateChanged(emptyState);
    }
}
