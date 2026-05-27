using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Users;

namespace NexaCRM.Infrastructure.Auth;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
        => _configuration = configuration;

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["JwtSettings:AccessTokenExpiryMinutes"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,    user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email,  user.Email),
            new Claim(ClaimTypes.Role,                user.Role),
            new Claim("tenantId",                     user.TenantId.ToString()),
            new Claim("fullName",                     user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti,    Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    public UserDto? ValidateAccessToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));

            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwt.Subject);
            var email = jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
            var role = jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            var tenantId = Guid.Parse(jwt.Claims.First(c => c.Type == "tenantId").Value);
            var fullName = jwt.Claims.First(c => c.Type == "fullName").Value;

            return new UserDto(userId, email, fullName, role, tenantId);
        }
        catch
        {
            return null;
        }
    }
}