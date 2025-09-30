using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Peo.Core.Interfaces.Services;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Application.Queries.ObterCertificadosAluno;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Services;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class ObterCertificadosAlunoQueryHandlerTests
{
    private readonly Mock<IAlunoService> _alunoServiceMock;
    private readonly Mock<ILogger<ObterCertificadosAlunoQueryHandler>> _loggerMock;
    private readonly Mock<IAppIdentityUser> _appIdentityUserMock;
    private readonly ObterCertificadosAlunoQueryHandler _handler;

    public ObterCertificadosAlunoQueryHandlerTests()
    {
        _alunoServiceMock = new Mock<IAlunoService>();
        _loggerMock = new Mock<ILogger<ObterCertificadosAlunoQueryHandler>>();
        _appIdentityUserMock = new Mock<IAppIdentityUser>();
        _handler = new ObterCertificadosAlunoQueryHandler(
            _alunoServiceMock.Object,
            _loggerMock.Object,
            _appIdentityUserMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarCertificados_QuandoValido()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var alunoId = Guid.CreateVersion7();
        var matriculaId = Guid.CreateVersion7();
        var aluno = new Aluno(usuarioId) { Id = alunoId };
        var certificados = new List<Certificado>
        {
            new Certificado(matriculaId, "Certificado 1", DateTime.Now, "CERT-001"),
            new Certificado(matriculaId, "Certificado 2", DateTime.Now, "CERT-002")
        };

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioId);
        _alunoServiceMock.Setup(x => x.ObterAlunoPorUserIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);
        _alunoServiceMock.Setup(x => x.ObterCertificadosDoAlunoAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certificados);

        // Act
        var resultado = await _handler.Handle(new ObterCertificadosAlunoQuery(), CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Should().HaveCount(2);
        resultado.Value.Should().BeEquivalentTo(certificados.Select(c => new CertificadoAlunoResponse(
            c.Id,
            c.MatriculaId,
            c.Conteudo,
            c.DataEmissao,
            c.NumeroCertificado
        )));
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoAlunoNaoEncontrado()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var mensagemErro = "Aluno não encontrado";

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioId);
        _alunoServiceMock.Setup(x => x.ObterAlunoPorUserIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException(mensagemErro));

        // Act
        var resultado = await _handler.Handle(new ObterCertificadosAlunoQuery(), CancellationToken.None);

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
        var alunoId = Guid.CreateVersion7();
        var aluno = new Aluno(usuarioId) { Id = alunoId };
        var mensagemErro = "Ocorreu um erro inesperado";

        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioId);
        _alunoServiceMock.Setup(x => x.ObterAlunoPorUserIdAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);
        _alunoServiceMock.Setup(x => x.ObterCertificadosDoAlunoAsync(alunoId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(mensagemErro));

        // Act
        var resultado = await _handler.Handle(new ObterCertificadosAlunoQuery(), CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNull();
        resultado.Error.Message.Should().Be(mensagemErro);
    }
}