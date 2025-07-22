using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Peo.Core.Dtos;
using Peo.Identity.Application.Endpoints;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Peo.Identity.Domain.Interfaces.Services;

namespace Peo.Tests.UnitTests.Identity.Endpoints;

public class LoginEndpointTests
{
    private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly LoginRequest _validRequest;
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly JwtSettings _jwtSettings;

    public LoginEndpointTests()
    {
        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();

        // Configuração do JwtSettings
        _jwtSettings = new JwtSettings
        {
            Key = "7c02bcde06b6b2c8711775145b694135a314503b9111edc4434d4312c2a6b49beee4f7e6e518f6a275cc18affc189427245dbac648378020fb97e9ec4a8559e9a156f7bbfa0d0e4bc8287960f1e77b671713472f7c67a0e922a6cf275ee64e451d3c788233e9066ef54dce462446ec39ed90a4c0d2c4aa25ba79f1375322ada48bdff86f693f83c250f423be4f96c04b505d8822f559408b6da1544c4c63c3e15ff325c641f5a9f7591200b39241d55f47757673a25e0a0721fdc74defdea9091b42555ecef984c58059d296d88afc5c356b5d7178bad9c166745a16218cddf028553e7e3e042e499cba33dbee934aa5bfee5fc86d23bb0cde3c23ed10c52c7f",
            Issuer = "test-issuer",
            Audience = "test-audience"
        };
        _jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);

        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        // Configuração do comportamento do UserManager
        userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => new IdentityUser { Email = email });

        userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(new[] { "Aluno" });

        _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
            null!, null!, null!, null!);

        _tokenServiceMock = new Mock<ITokenService>();
        _validRequest = new LoginRequest(
            Email: "teste@exemplo.com",
            Password: "Teste@123"
        );
    }

    [Fact]
    public async Task HandleLogin_DeveRetornarProblemaValidacao_QuandoRequisicaoInvalida()
    {
        // Arrange
        var requisicaoInvalida = new LoginRequest("", "");

        // Act
        var resultado = await LoginEndpoint.HandleLogin(
            requisicaoInvalida,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Assert
        resultado.Should().BeOfType<ProblemHttpResult>();
    }

    [Fact]
    public async Task HandleLogin_DeveRetornarNaoAutorizado_QuandoUsuarioNaoEncontrado()
    {
        // Arrange
        var userManager = _signInManagerMock.Object.UserManager;
        Mock.Get(userManager).Setup(x => x.FindByEmailAsync(_validRequest.Email))
            .ReturnsAsync((IdentityUser?)null);

        // Act
        var resultado = await LoginEndpoint.HandleLogin(
            _validRequest,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Assert
        resultado.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task HandleLogin_DeveRetornarNaoAutorizado_QuandoSenhaInvalida()
    {
        // Arrange
        var usuario = new IdentityUser { Id = Guid.CreateVersion7().ToString() };
        var userManager = _signInManagerMock.Object.UserManager;

        Mock.Get(userManager).Setup(x => x.FindByEmailAsync(_validRequest.Email))
            .ReturnsAsync(usuario);

        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(usuario, _validRequest.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var resultado = await LoginEndpoint.HandleLogin(
            _validRequest,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Assert
        resultado.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task HandleLogin_DeveRetornarOk_QuandoLoginBemSucedido()
    {
        // Arrange
        var usuario = new IdentityUser { Id = Guid.CreateVersion7().ToString() };
        var papeis = new[] { "Aluno" };
        var token = "token-teste";

        // Configuração do comportamento do UserManager
        var userManager = _signInManagerMock.Object.UserManager;
        Mock.Get(userManager).Setup(x => x.FindByEmailAsync(_validRequest.Email))
            .ReturnsAsync(usuario);

        Mock.Get(userManager).Setup(x => x.GetRolesAsync(usuario))
            .ReturnsAsync(papeis);

        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(usuario, _validRequest.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _tokenServiceMock.Setup(x => x.CreateToken(usuario, papeis))
            .Returns(token);

        // Act
        var resultado = await LoginEndpoint.HandleLogin(
            _validRequest,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Assert
        resultado.Should().BeOfType<Ok<LoginResponse>>();

        var resposta = ((Ok<LoginResponse>)resultado).Value;
        resposta.Should().NotBeNull();
        resposta!.Token.Should().Be(token);
        resposta!.UserId.Should().Be(Guid.Parse(usuario.Id));
    }

    [Fact]
    public async Task HandleRefreshToken_DeveRetornarNaoAutorizado_QuandoTokenAindaValido()
    {
        // Arrange
        var usuario = new IdentityUser { Id = Guid.CreateVersion7().ToString() };
        var papeis = new[] { "Aluno" };
        var token = "token-teste";

        // Configuração do comportamento do UserManager
        var userManager = _signInManagerMock.Object.UserManager;
        Mock.Get(userManager).Setup(x => x.FindByEmailAsync(_validRequest.Email))
            .ReturnsAsync(usuario);

        Mock.Get(userManager).Setup(x => x.GetRolesAsync(usuario))
            .ReturnsAsync(papeis);

        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(usuario, _validRequest.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _tokenServiceMock.Setup(x => x.CreateToken(usuario, papeis))
            .Returns(token);

        var resultadoToken = await LoginEndpoint.HandleLogin(
            _validRequest,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);

        // Act
        var resposta = ((Ok<LoginResponse>)resultadoToken).Value;

        var resultado = await RefreshTokenEndpoint.HandleRefreshToken(new RefreshTokenRequest(resposta!.Token), userManager, _jwtSettingsMock.Object, _tokenServiceMock.Object, new Mock<ILogger<RefreshTokenEndpoint>>().Object);

        // Assert
        resultado.Should().BeOfType<UnauthorizedHttpResult>();
    }
}