using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();
app.Use(async (ctx, next) =>
{
    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
    var protector = idp.CreateProtector("auth-cookie");
    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault();
    if (authCookie is not null)
    {
        var authInfo = authCookie.Split(";").FirstOrDefault(x => x.Contains("auth="));
        var protectedPayload = authInfo.Split("=").Last();
        var payload = protector.Unprotect(protectedPayload);
        var value =  payload.Split(":").Last();

        // zestaw twoich właściwości klucz wartośc np imię = xxx
        var claims = new List<Claim>();
        claims.Add(new Claim("user", value));
        // obiekt definiujący jak cię identyfikować coś na zasadzie dowodu osobistego 
        var identity = new ClaimsIdentity(claims);
        // obiekt zawierający informacje kim jesteś
        ctx.User = new ClaimsPrincipal(identity);

        await next.Invoke();
    }
});

app.MapGet("/username", (HttpContext ctx) =>
{
    return ctx.User.FindFirst("user").Value;
});

app.MapGet("/login", (AuthService auth) =>
{
    auth.SignIn();
    return "ok";
});

app.Run();

public class AuthService
{
    private readonly IDataProtectionProvider _idp;
    private readonly IHttpContextAccessor _accessor;
    public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
    {
        _idp = idp;
        _accessor = accessor;
    }

    public void SignIn()
    {
        var protector = _idp.CreateProtector("auth-cookie");
        _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("user:milosz")}";
    }
}