
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// Generate RSA keys
using var rsa = RSA.Create(2048);
var privateKey = rsa.ExportRSAPrivateKey();
var publicKey = rsa.ExportRSAPublicKey();

// Convert keys to Base64 strings
var privateKeyString = Convert.ToBase64String(privateKey);
var publicKeyString = Convert.ToBase64String(publicKey);

var secretKey = "Bardzodlugisekretktorypowinienbycprzechowywanywbezpiecznymmiejscu";

var builder = WebApplication.CreateBuilder(args);
// Configure services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "your-issuer",
            ValidateAudience = true,
            ValidAudience = "your-audience",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Endpoint to generate the JWT with encrypted payload
app.MapPost("/generate-token", (HttpContext context) =>
{
    // Define payload
    var payload = new
    {
        id = Guid.NewGuid().ToString()
    };

    // Encrypt payload
    var encryptedPayload = EncryptPayload(JsonSerializer.Serialize(payload), Convert.FromBase64String(publicKeyString));

    // Create JWT token
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(secretKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Issuer = "your-issuer",
        Audience = "your-audience",
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        Claims = new Dictionary<string, object>
        {
            { "data", encryptedPayload }
        }
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var jwtToken = tokenHandler.WriteToken(token);

    return Results.Ok(new { Token = jwtToken });
});

// Secure endpoint that requires a valid JWT
app.MapGet("/secure-endpoint", (ClaimsPrincipal user) =>
{
    var encryptedPayload = user.FindFirst("data")?.Value;
    if (encryptedPayload == null)
    {
        return Results.BadRequest("Invalid token");
    }

    var privateKey = Convert.FromBase64String(privateKeyString);
    using var rsa = RSA.Create();
    rsa.ImportRSAPrivateKey(privateKey, out _);

    var decryptedPayload = DecryptPayload(encryptedPayload, rsa);
    var values = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedPayload);
    return Results.Ok(values);
}).RequireAuthorization();


// Run the application
app.Run();

// Define helper methods for encryption and decryption
static string EncryptPayload(string payload, byte[] publicKey)
{
    using var rsa = RSA.Create();
    rsa.ImportRSAPublicKey(publicKey, out _);

    var dataToEncrypt = Encoding.UTF8.GetBytes(payload);
    var encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
    return Convert.ToBase64String(encryptedData);
}

static string DecryptPayload(string encryptedPayload, RSA rsa)
{
    var encryptedData = Convert.FromBase64String(encryptedPayload);
    var decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
    return Encoding.UTF8.GetString(decryptedData);
}