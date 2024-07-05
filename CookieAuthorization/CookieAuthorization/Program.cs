using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

//jest to definicja sposobu autoryzacji, coś na zasadzie wspomnianego wczesniej dowodu i prawa ajzdy
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie");
var app = builder.Build();

//odpowiada sekcji pisanej w middleware
app.UseAuthentication();

app.MapGet("/username", (HttpContext ctx) =>
{
    return ctx.User.FindFirst("user").Value;
});

app.MapGet("/login", async (HttpContext ctx) =>
{
    // sprawdzenie danych autoryzacyjnych
    
    
    // utworzenie kontekstu użytkownika
    var claims = new List<Claim>();
    claims.Add(new Claim("user", "Miłosz"));
    var identity = new ClaimsIdentity(claims, "cookie");
    await ctx.SignInAsync("cookie", new ClaimsPrincipal(identity));
    return "ok";
});

app.Run();

