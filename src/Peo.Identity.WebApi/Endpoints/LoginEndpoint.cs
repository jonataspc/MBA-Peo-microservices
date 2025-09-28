using Microsoft.AspNetCore.Identity;
using MiniValidation;
using Peo.Core.Web.Api;
using Peo.Identity.Domain.Interfaces.Services;
using Peo.Identity.WebApi.Endpoints.Requests;
using Peo.Identity.WebApi.Endpoints.Responses;

namespace Peo.Identity.WebApi.Endpoints
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