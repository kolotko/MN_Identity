using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using JwtAuthorization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthorization.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private string _tokenSecret;
    private int _tokenLifetime;
    private string _issuer;
    private string _audience;
    public IdentityController(IOptions<JwtSettings> jwtSettings)
    {
        _tokenSecret = jwtSettings.Value.Key;
        _tokenLifetime = jwtSettings.Value.ExpireAfterHours;
        _issuer = jwtSettings.Value.Issuer;
        _audience = jwtSettings.Value.Audience;
    }

    [HttpGet]
    public IActionResult GenerateToken(bool isAdmin)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "507507507"),
            new Claim(JwtRegisteredClaimNames.Name, "John Doe"),
            new Claim(JwtRegisteredClaimNames.Email, "john.doe@example.com"),
            new Claim("admin", isAdmin.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenLifetime),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = tokenString });
    }
}