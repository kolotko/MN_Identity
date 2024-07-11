using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie");


var app = builder.Build();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

var api = app.MapGroup("api");

api.MapGet("/", () => "Hello World!");

api.MapPost("/login", () => Results.SignIn(
    new ClaimsPrincipal(new ClaimsIdentity(
            new []
            {
                new Claim("id", Guid.NewGuid().ToString())
            },
            "cookie"
        )
    ),
    authenticationScheme: "cookie"
));
api.MapGet("/user", (ClaimsPrincipal user) => user.Claims.ToDictionary(x => x.Type, x => x.Value));

app.MapFallbackToFile("index.html");
app.Run();