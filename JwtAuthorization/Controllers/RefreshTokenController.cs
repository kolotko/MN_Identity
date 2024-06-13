using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtAuthorization.Abstractions;
using JwtAuthorization.Dto;
using JwtAuthorization.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthorization.Controllers;

[ApiController]
[Route("[controller]")]
public class RefreshTokenController : ControllerBase
{
    private string _tokenSecret;
    private int _tokenLifetime;
    private string _issuer;
    private string _audience;
    private readonly IUserManagerRepository _userManager;
    
    public RefreshTokenController(IOptions<JwtSettings> jwtSettings, IUserManagerRepository userManager)
    {
        _tokenSecret = jwtSettings.Value.Key;
        _tokenLifetime = jwtSettings.Value.ExpireAfterHours;
        _issuer = jwtSettings.Value.Issuer;
        _audience = jwtSettings.Value.Audience;
        _userManager = userManager;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var user = _userManager.FindByName(loginRequest.Username);
        if (user == null)
        {
            return Unauthorized();
        }
        
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        _userManager.Update(user);

        return Ok(new
        {
            Token = accessToken,
            RefreshToken = refreshToken
        });
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRequest tokenRequest)
    {
        var principal = GetPrincipalFromExpiredToken(tokenRequest.Token);
        if (principal == null)
        {
            return BadRequest("Invalid access token or refresh token");
        }

        var username = principal.Identity.Name;
        var user = _userManager.FindByName(username);

        if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid refresh token");
        }

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        _userManager.Update(user);

        return Ok(new
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenLifetime),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // ignorujemy, bo token jest już nieważny
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSecret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}