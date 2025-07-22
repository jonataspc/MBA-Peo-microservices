using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using MiniValidation;
using Peo.Core.Web.Api;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Peo.Identity.Domain.Interfaces.Services;

namespace Peo.Identity.Application.Endpoints
{
    public class LoginEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", HandleLogin)
               .WithSummary("Autenticar um usuário")
               .AllowAnonymous();
        }

        public static async Task<IResult> HandleLogin(LoginRequest loginRequest, SignInManager<IdentityUser> signInManager, ITokenService tokenService)
        {
            if (!MiniValidator.TryValidate(loginRequest, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            var user = await signInManager.UserManager.FindByEmailAsync(loginRequest.Email);

            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            var signInResult = await signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                return TypedResults.Unauthorized();
            }

            var userRoles = await signInManager.UserManager.GetRolesAsync(user!);

            return TypedResults.Ok(new LoginResponse(tokenService.CreateToken(user, userRoles), Guid.Parse(user.Id)));
        }
    }
}