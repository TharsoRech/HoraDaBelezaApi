using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HoraDaBeleza.Infrastructure.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) => _config = config;

    public string GenerateToken(User user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name,           user.Name),
            new Claim(ClaimTypes.Email,          user.Email),
            new Claim(ClaimTypes.Role,           user.Type.ToString()),
            new Claim("type",                    ((int)user.Type).ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             jwt["Issuer"],
            audience:           jwt["Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddDays(Convert.ToDouble(jwt["ExpiryDays"] ?? "7")),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
