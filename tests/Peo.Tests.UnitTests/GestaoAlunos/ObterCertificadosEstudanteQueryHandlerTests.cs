using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.Queries.ObterCertificadosEstudante;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class ObterCertificadosEstudanteQueryHandlerTests
{
    private readonly Mock<IEstudanteService> _estudanteServiceMock;
    private readonly Mock<ILogger<ObterCertificadosEstudanteQueryHandler>> _loggerMock;
    private readonly Mock<IAppIdentityUser> _appIdentityUserMock;
    private readonly ObterCertificadosEstudanteQueryHandler _handler;

    public ObterCertificadosEstudanteQueryHandlerTests()
    {
        _estudanteServiceMock = new Mock<IEstudanteService>();
        _loggerMock = new Mock<ILogger<ObterCertificadosEstudanteQueryHandler>>();
        _appIdentityUserMock = new Mock<IAppIdentityUser>();
        _handler = new ObterCertificadosEstudanteQueryHandler(
            _estudanteServiceMock.Object,
            _loggerMock.Object,
            _appIdentityUserMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarCertificados_QuandoValido()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var matriculaId = Guid.CreateVersion7();
        var estudante = new Estudante(usuarioId) { Id = estudanteId };
        var certificados = new List<Certificado>
        {
            new Certificado(matriculaId, "Certificado 1", DateTime.Now, "CERT-001"),
            new Certificado(matriculaId, "Certificado 2", DateTime.Now, "CERT-002")
        };

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioId);
        _estudanteServiceMock.Setup(x => x.ObterEstudantePorUserIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(estudante);
        _estudanteServiceMock.Setup(x => x.ObterCertificadosDoEstudanteAsync(estudanteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certificados);

        // Act
        var resultado = await _handler.Handle(new ObterCertificadosEstudanteQuery(), CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Should().HaveCount(2);
        resultado.Value.Should().BeEquivalentTo(certificados.Select(c => new CertificadoEstudanteResponse(
            c.Id,
            c.MatriculaId,
            c.Conteudo,
            c.DataEmissao,
            c.NumeroCertificado
        )));
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoEstudanteNaoEncontrado()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var mensagemErro = "Estudante não encontrado";

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioId);
        _estudanteServiceMock.Setup(x => x.ObterEstudantePorUserIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException(mensagemErro));

        // Act
        var resultado = await _handler.Handle(new ObterCertificadosEstudanteQuery(), CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNull();
        resultado.Error.Message.Should().Be(mensagemErro);
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoErroOcorre()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var estudante = new Estudante(usuarioId) { Id = estudanteId };
        var mensagemErro = "Ocorreu um erro inesperado";

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioId);
        _estudanteServiceMock.Setup(x => x.ObterEstudantePorUserIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(estudante);
        _estudanteServiceMock.Setup(x => x.ObterCertificadosDoEstudanteAsync(estudanteId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(mensagemErro));

        // Act
        var resultado = await _handler.Handle(new ObterCertificadosEstudanteQuery(), CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNull();
        resultado.Error.Message.Should().Be(mensagemErro);
    }
}