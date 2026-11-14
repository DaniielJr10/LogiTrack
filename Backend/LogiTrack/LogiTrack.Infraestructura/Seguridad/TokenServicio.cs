using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LogiTrack.Aplicacion.Interfaces.Seguridad;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LogiTrack.Infraestructura.Seguridad;

public class TokenServicio : ITokenServicio
{
    private readonly IConfiguration _configuration;

    public TokenServicio(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerarToken(string userId, string email, string userName, IEnumerable<string> roles)
    {
        var jwtConfig = _configuration.GetSection("Jwt");
        var secretKey = jwtConfig["Key"] ?? throw new InvalidOperationException("Falta la configuración Jwt:Key.");
        var issuer = jwtConfig["Issuer"] ?? "LogiTrack";
        var audience = jwtConfig["Audience"] ?? "LogiTrack";
        var expiryMinutes = int.TryParse(jwtConfig["ExpiryMinutes"], out var parsed) ? parsed : 60;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.NameIdentifier, userId)
        };

        claims.AddRange(roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
