using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniValidation;
using Peo.Core.Dtos;
using Peo.Core.Web.Api;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Peo.Identity.Domain.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Peo.Identity.Application.Endpoints
{
    public class RefreshTokenEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/refresh-token", HandleRefreshToken)
               .WithSummary("Atualiza um token")
               .AllowAnonymous();
        }

        public static async Task<IResult> HandleRefreshToken(
            RefreshTokenRequest request,
            UserManager<IdentityUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            ITokenService tokenService,
            ILogger<RefreshTokenEndpoint> logger)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSettings.Value.Key);

            try
            {
                var lifetimeValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Value.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Value.Audience,
                    ValidateLifetime = true
                };

                try
                {
                    // Check if the token is still valid
                    tokenHandler.ValidateToken(request.Token, lifetimeValidationParameters, out _);
                    return TypedResults.Unauthorized();
                }
                catch (SecurityTokenExpiredException)
                {
                    // Token is expired, which is what we want for refresh
                }

                // Validate the token without lifetime check to get the claims
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Value.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Value.Audience,
                    ValidateLifetime = false
                };

                var principal = tokenHandler.ValidateToken(request.Token, tokenValidationParameters, out var _);
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return TypedResults.Unauthorized();
                }

                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return TypedResults.Unauthorized();
                }

                var userRoles = await userManager.GetRolesAsync(user);
                var newToken = tokenService.CreateToken(user, userRoles);

                return TypedResults.Ok(new RefreshTokenResponse(newToken, Guid.Parse(user.Id)));
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return TypedResults.Unauthorized();
            }
        }
    }
}