using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie");
builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("EU passport", pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes("cookie")
            .RequireClaim("passport_type", "EU");
    });
});
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();


app.MapGet("/sweden", (HttpContext ctx) =>
{
    return "allowed";
}).RequireAuthorization("EU passport");

app.MapGet("/login", async (HttpContext ctx) =>
{
    // sprawdzenie danych autoryzacyjnych
    
    
    // utworzenie kontekstu użytkownika
    var claims = new List<Claim>();
    claims.Add(new Claim("user", "Miłosz"));
    claims.Add(new Claim("passport_type", "EU"));
    var identity = new ClaimsIdentity(claims, "cookie");
    await ctx.SignInAsync("cookie", new ClaimsPrincipal(identity));
    return "ok";
});

app.Run();
