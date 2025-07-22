using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Peo.Core.Dtos;
using Peo.Identity.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Peo.Tests.UnitTests.Identity;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public TokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();

        // Setup JwtSettings
        _jwtSettings = new JwtSettings
        {
            Key = "7c02bcde06b6b2c8711775145b694135a314503b9111edc4434d4312c2a6b49beee4f7e6e518f6a275cc18affc189427245dbac648378020fb97e9ec4a8559e9a156f7bbfa0d0e4bc8287960f1e77b671713472f7c67a0e922a6cf275ee64e451d3c788233e9066ef54dce462446ec39ed90a4c0d2c4aa25ba79f1375322ada48bdff86f693f83c250f423be4f96c04b505d8822f559408b6da1544c4c63c3e15ff325c641f5a9f7591200b39241d55f47757673a25e0a0721fdc74defdea9091b42555ecef984c58059d296d88afc5c356b5d7178bad9c166745a16218cddf028553e7e3e042e499cba33dbee934aa5bfee5fc86d23bb0cde3c23ed10c52c7f",
            Issuer = "test-issuer",
            Audience = "test-audience"
        };
        _jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);

        // Mock Authentication section for expiration
        var authSectionMock = new Mock<IConfigurationSection>();
        authSectionMock.Setup(x => x["ExpirationInMinutes"]).Returns("60");
        _configurationMock.Setup(x => x.GetSection("Authentication")).Returns(authSectionMock.Object);

        // Mock direct configuration access for expiration
        _configurationMock.Setup(x => x["Authentication:ExpirationInMinutes"]).Returns("60");

        _tokenService = new TokenService(_jwtSettingsMock.Object, _configurationMock.Object);
    }

    [Fact]
    public void CriarToken_DeveRetornarJwtValido()
    {
        // Arrange
        var usuario = new Microsoft.AspNetCore.Identity.IdentityUser
        {
            Id = "123",
            UserName = "teste@exemplo.com"
        };
        var papeis = new[] { "Aluno" };

        // Act
        var token = _tokenService.CreateToken(usuario, papeis);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = "test-issuer",
            ValidateAudience = true,
            ValidAudience = "test-audience",
            ValidateLifetime = true
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
        principal.Should().NotBeNull();
        validatedToken.Should().NotBeNull();

        var jwtToken = (JwtSecurityToken)validatedToken;
        jwtToken.Issuer.Should().Be("test-issuer");
        jwtToken.Audiences.Should().Contain("test-audience");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "teste@exemplo.com");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Aluno");
    }
}