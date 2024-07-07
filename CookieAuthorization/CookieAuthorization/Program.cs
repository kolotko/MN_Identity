using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

//jest to definicja sposobu autoryzacji, coś na zasadzie wspomnianego wczesniej dowodu i prawa ajzdy
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie");
var app = builder.Build();

//odpowiada sekcji pisanej w middleware
app.UseAuthentication();

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/login"))
    {
        await next.Invoke();
        return;
    }
    if (ctx.User.Identities.All(x => x.AuthenticationType != "cookie"))
    {
        ctx.Response.StatusCode = 401;
        return;
    }

    if (!ctx.User.HasClaim("passport_type", "EU"))
    {
        ctx.Response.StatusCode = 403;
        return;
    }
    await next.Invoke();
});

app.MapGet("/username", (HttpContext ctx) =>
{
    return ctx.User.FindFirst("user")?.Value ?? "brak";
});

app.MapGet("/sweden", (HttpContext ctx) =>
{
    return "allowed";
});

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

