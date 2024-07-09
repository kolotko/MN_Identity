using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(b =>
{
    b.AddPolicy("uzytkownik", p =>
    {
        p.AddAuthenticationSchemes("cookie-otrzymane-po-oauth")
            .RequireAuthenticatedUser();
    });
});
builder.Services.AddAuthentication()
    .AddCookie("cookie-otrzymane-po-oauth")
    .AddOAuth("mock-oauth", builder =>
    {
        builder.SignInScheme = "cookie-otrzymane-po-oauth";
        builder.ClientId = "test";
        builder.ClientSecret = "test";

        builder.AuthorizationEndpoint = "https://oauth.wiremockapi.cloud/oauth/authorize";
        builder.TokenEndpoint = "https://oauth.wiremockapi.cloud/oauth/token";
        builder.UserInformationEndpoint = "https://oauth.wiremockapi.cloud/userinfo";
        builder.CallbackPath = "/notInUseWithMockButRequired";
        builder.Scope.Add("testScope");
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", async ctx =>
{
    var t = ctx.User;
});

app.MapGet("/oauth-login", async ctx =>
{
    await ctx.ChallengeAsync("mock-oauth", new AuthenticationProperties()
    {
        RedirectUri = "/"
    });
});


app.Run();

 