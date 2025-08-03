using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using System.Net;
using System.Net.Http.Json;

namespace Peo.Tests.IntegrationTests.Identity;

public class IdentityEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IServiceScope _scope;

    public IdentityEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Registrar_ComRequisicaoValida_DeveCriarUsuario()
    {
        // Arrange
        var request = new RegisterRequest(
            Email: $"{Guid.CreateVersion7()}@example.com",
            Password: "Test123!91726312389831625192JHTBADPDJANDHJPXASDO",
            Name: "Test User"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var user = await _userManager.FindByEmailAsync(request.Email);
        user.Should().NotBeNull();
        user!.Email.Should().Be(request.Email);
        user.UserName.Should().Be(request.Email);

        var isInRole = await _userManager.IsInRoleAsync(user, "Aluno");
        isInRole.Should().BeTrue();
    }

    [Fact]
    public async Task Registrar_ComEmailInvalido_DeveRetornarErroValidacao()
    {
        // Arrange
        var request = new RegisterRequest(
            Email: "invalid-email",
            Password: "Test123!91726312389831625192JHTBADPDJANDHJPXASDO",
            Name: "Test User"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
    {
        // Arrange
        var email = $"{Guid.CreateVersion7()}@example.com";
        var password = "Test123!91726312389831625192JHTBADPDJANDHJPXASDO";

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        await _userManager.CreateAsync(user, password);
        await _userManager.AddToRoleAsync(user, "Aluno");

        var request = new LoginRequest(email, password);

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.UserId.Should().Be(Guid.Parse(user.Id));
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_DeveRetornarNaoAutorizado()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "WrongPassword");

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_ComTokenInvalido_DeveRetornarNaoAutorizado()
    {
        // Arrange
        var request = new RefreshTokenRequest("invalid-token");

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/refresh-token", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}