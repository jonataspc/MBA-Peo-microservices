using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using MiniValidation;
using Peo.Core.DomainObjects;
using Peo.Core.Entities;
using Peo.Core.Web.Api;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Domain.Interfaces.Services;

namespace Peo.Identity.Application.Endpoints
{
    public class RegisterEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/register", HandleRegister)
               .WithSummary("Registra um novo usuário")
               .AllowAnonymous();
        }

        public static async Task<IResult> HandleRegister(
            RegisterRequest request,
            UserManager<IdentityUser> userManager,
            IUserService userService)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }

            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email,
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await userManager.ConfirmEmailAsync(user, await userManager.GenerateEmailConfirmationTokenAsync(user));

                var roleResult = await userManager.AddToRoleAsync(user, AccessRoles.Aluno);
                if (!roleResult.Succeeded)
                {
                    return TypedResults.BadRequest(new { Description = "Failed to assign role", Content = roleResult.Errors });
                }

                await userService.AddAsync(
                new Usuario(Guid.Parse(user.Id), request.Name, user.Email!)
                );

                return TypedResults.NoContent();
            }

            return TypedResults.BadRequest(new { Description = "Errors", Content = result.Errors });
        }
    }
}