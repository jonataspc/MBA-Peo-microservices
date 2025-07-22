using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Peo.Core.Dtos;
using Peo.Identity.Domain.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Peo.Identity.Application.Services
{
    public class TokenService(IOptions<JwtSettings> jwtSettings, IConfiguration configuration) : ITokenService
    {
        public string CreateToken(IdentityUser user, IEnumerable<string> roles)
        {
            var token = CreateJwtToken(CreateClaims(user, roles), CreateSigningCredentials());
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private JwtSecurityToken CreateJwtToken(IEnumerable<Claim> claims, SigningCredentials credentials) =>
            new(
                jwtSettings.Value.Issuer,
                jwtSettings.Value.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(configuration["Authentication:ExpirationInMinutes"]!)),
                signingCredentials: credentials
            );

        private static List<Claim> CreateClaims(IdentityUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
                    new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new(ClaimTypes.NameIdentifier, user.UserName!),
                    new(ClaimTypes.Name, user.UserName!)
                };

            foreach (var userRole in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return claims;
        }

        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(
            new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Value.Key)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}