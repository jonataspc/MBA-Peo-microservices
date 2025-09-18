using FluentAssertions;
using MassTransit;
using Moq;
using Peo.Core.DomainObjects;
using Peo.Core.Interfaces.Services;
using Peo.Core.Messages.IntegrationRequests;
using Peo.Core.Messages.IntegrationResponses;
using Peo.GestaoAlunos.Application.Services;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Domain.ValueObjects;
using System.Linq.Expressions;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class AlunoServiceTests
{
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<IRequestClient<ObterDetalhesCursoRequest>> _obterDetalhesCursoRequestMock;
    private readonly Mock<IRequestClient<ObterDetalhesUsuarioRequest>> _obterDetalhesUsuarioRequestMock;
    private readonly Mock<IAppIdentityUser> _appIdentityUserMock;
    private readonly AlunoService _alunoService;

    public AlunoServiceTests()
    {
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _obterDetalhesUsuarioRequestMock = new Mock<IRequestClient<ObterDetalhesUsuarioRequest>>();
        _appIdentityUserMock = new Mock<IAppIdentityUser>();
        _obterDetalhesCursoRequestMock = new Mock<IRequestClient<ObterDetalhesCursoRequest>>();

        _alunoService = new AlunoService(
            _obterDetalhesCursoRequestMock.Object,
            _alunoRepositoryMock.Object,
            _obterDetalhesUsuarioRequestMock.Object,
            _appIdentityUserMock.Object);
    }

    [Fact]
    public async Task CriarAlunoAsync_DeveCriarERetornarNovoAluno()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var alunoEsperado = new Aluno(usuarioId);
        _alunoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _alunoService.CriarAlunoAsync(usuarioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.UsuarioId.Should().Be(usuarioId);
        _alunoRepositoryMock.Verify(x => x.AddAsync(It.Is<Aluno>(s => s.UsuarioId == usuarioId)), Times.Once);
        _alunoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MatricularAlunoAsync_DeveCriarMatriculaQuandoValido()
    {
        // Arrange
        var alunoId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var aluno = new Aluno(Guid.CreateVersion7());
        var matriculaEsperada = new Matricula(alunoId, cursoId);

        // Setup curso response mock
        var mockResponse = new Mock<Response<ObterDetalhesCursoResponse>>();
        mockResponse.Setup(x => x.Message).Returns(new ObterDetalhesCursoResponse(cursoId, 10, "Curso Teste", 100.00m));

        _alunoRepositoryMock.Setup(x => x.GetByIdAsync(alunoId))
            .ReturnsAsync(aluno);

        _obterDetalhesCursoRequestMock.Setup(x => x.GetResponse<ObterDetalhesCursoResponse>(
            It.IsAny<ObterDetalhesCursoRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(mockResponse.Object);

        _alunoRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Aluno, bool>>>()))
            .ReturnsAsync(false);

        _alunoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _alunoService.MatricularAlunoAsync(alunoId, cursoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.AlunoId.Should().Be(alunoId);
        resultado.CursoId.Should().Be(cursoId);
        _alunoRepositoryMock.Verify(x => x.AddMatriculaAsync(It.Is<Matricula>(m => m.AlunoId == alunoId && m.CursoId == cursoId)), Times.Once);
        _alunoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MatricularAlunoAsync_DeveLancarExcecaoQuandoAlunoNaoEncontrado()
    {
        // Arrange
        var alunoId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        _alunoRepositoryMock.Setup(x => x.GetByIdAsync(alunoId))
            .ReturnsAsync((Aluno?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _alunoService.MatricularAlunoAsync(alunoId, cursoId));
    }

    [Fact]
    public async Task MatricularAlunoAsync_DeveLancarExcecaoQuandoCursoNaoEncontrado()
    {
        // Arrange
        var alunoId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var aluno = new Aluno(Guid.CreateVersion7());

        // Setup empty curso response mock
        var mockResponse = new Mock<Response<ObterDetalhesCursoResponse>>();
        mockResponse.Setup(x => x.Message).Returns(new ObterDetalhesCursoResponse(null, null, null, null));

        _alunoRepositoryMock.Setup(x => x.GetByIdAsync(alunoId))
            .ReturnsAsync(aluno);

        _obterDetalhesCursoRequestMock.Setup(x => x.GetResponse<ObterDetalhesCursoResponse>(
            It.IsAny<ObterDetalhesCursoRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(mockResponse.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _alunoService.MatricularAlunoAsync(alunoId, cursoId));
    }

    [Fact]
    public async Task IniciarAulaAsync_DeveCriarProgressoQuandoValido()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var alunoId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var matricula = new Matricula(alunoId, Guid.CreateVersion7());
        var aluno = new Aluno(usuarioAtualId) { Id = alunoId };

        matricula.ConfirmarPagamento();

        _alunoRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _alunoRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(aluno);
        _alunoRepositoryMock.Setup(x => x.GetProgressoMatriculaAsync(matriculaId, aulaId))
            .ReturnsAsync((ProgressoMatricula?)null);
        _alunoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _alunoService.IniciarAulaAsync(matriculaId, aulaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.MatriculaId.Should().Be(matriculaId);
        resultado.AulaId.Should().Be(aulaId);
        resultado.EstaConcluido.Should().BeFalse();
        _alunoRepositoryMock.Verify(x => x.AddProgressoMatriculaAsync(It.Is<ProgressoMatricula>(p => p.MatriculaId == matriculaId && p.AulaId == aulaId)), Times.Once);
        _alunoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConcluirAulaAsync_DeveAtualizarProgressoQuandoValido()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var alunoId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var matricula = new Matricula(alunoId, cursoId);
        var progresso = new ProgressoMatricula(matriculaId, aulaId);
        var aluno = new Aluno(usuarioAtualId) { Id = alunoId };

        // Setup curso response mock
        var mockResponse = new Mock<Response<ObterDetalhesCursoResponse>>();
        mockResponse.Setup(x => x.Message).Returns(new ObterDetalhesCursoResponse(cursoId, 10, "Curso Teste", 100.00m));

        _alunoRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _alunoRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(aluno);
        _alunoRepositoryMock.Setup(x => x.GetProgressoMatriculaAsync(matriculaId, aulaId))
            .ReturnsAsync(progresso);
        _obterDetalhesCursoRequestMock.Setup(x => x.GetResponse<ObterDetalhesCursoResponse>(
            It.IsAny<ObterDetalhesCursoRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(mockResponse.Object);
        _alunoRepositoryMock.Setup(x => x.GetAulasConcluidasCountAsync(matriculaId))
            .ReturnsAsync(9);
        _alunoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _alunoService.ConcluirAulaAsync(matriculaId, aulaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.MatriculaId.Should().Be(matriculaId);
        resultado.AulaId.Should().Be(aulaId);
        resultado.EstaConcluido.Should().BeTrue();
        _alunoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConcluirAulaAsync_DeveLancarExcecaoQuandoUsuarioAtualNaoEhAlunoDaMatricula()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var alunoMatriculaId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var alunoDiferenteId = Guid.CreateVersion7();
        var matricula = new Matricula(alunoMatriculaId, Guid.CreateVersion7());
        var aluno = new Aluno(usuarioAtualId) { Id = alunoDiferenteId };

        _alunoRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _alunoRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(aluno);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _alunoService.ConcluirAulaAsync(matriculaId, aulaId));
    }

    [Fact]
    public async Task IniciarAulaAsync_DeveLancarExcecaoQuandoUsuarioAtualNaoEhAlunoDaMatricula()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var alunoMatriculaId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var alunoDiferenteId = Guid.CreateVersion7();
        var matricula = new Matricula(alunoMatriculaId, Guid.CreateVersion7());
        var aluno = new Aluno(usuarioAtualId) { Id = alunoDiferenteId };

        _alunoRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _alunoRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(aluno);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _alunoService.IniciarAulaAsync(matriculaId, aulaId));
    }

    [Fact]
    public async Task MatricularAlunoComUserIdAsync_DeveCriarAlunoEMatricularQuandoValido()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var alunoId = Guid.CreateVersion7();
        var aluno = new Aluno(usuarioId) { Id = alunoId };

        // Setup curso response mock
        var mockResponse = new Mock<Response<ObterDetalhesCursoResponse>>();
        mockResponse.Setup(x => x.Message).Returns(new ObterDetalhesCursoResponse(cursoId, 10, "Curso Teste", 100.00m));

        _alunoRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioId))
            .ReturnsAsync((Aluno?)null);

        _alunoRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(aluno);

        _obterDetalhesCursoRequestMock.Setup(x => x.GetResponse<ObterDetalhesCursoResponse>(
            It.IsAny<ObterDetalhesCursoRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(mockResponse.Object);

        _alunoRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Aluno, bool>>>()))
            .ReturnsAsync(false);

        _alunoRepositoryMock.Setup(x => x.AddMatriculaAsync(It.IsAny<Matricula>()))
            .Returns(Task.CompletedTask);

        _alunoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _alunoService.MatricularAlunoComUserIdAsync(usuarioId, cursoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.CursoId.Should().Be(cursoId);
        _alunoRepositoryMock.Verify(x => x.AddAsync(It.Is<Aluno>(s => s.UsuarioId == usuarioId)), Times.Once);
        _alunoRepositoryMock.Verify(x => x.AddMatriculaAsync(It.Is<Matricula>(m => m.CursoId == cursoId)), Times.Once);
        _alunoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ObterProgressoGeralCursoAsync_DeveRetornarPorcentagemProgresso()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var alunoId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var matricula = new Matricula(alunoId, Guid.CreateVersion7());
        var aluno = new Aluno(usuarioAtualId) { Id = alunoId };
        matricula.AtualizarProgresso(75);

        _alunoRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _alunoRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(aluno);

        // Act
        var resultado = await _alunoService.ObterProgressoGeralCursoAsync(matriculaId);

        // Assert
        resultado.Should().Be(75);
    }

    [Fact]
    public async Task ConcluirMatriculaAsync_DeveConcluirMatriculaEGerarCertificado()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var alunoId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var aluno = new Aluno(usuarioAtualId) { Id = alunoId };
        var matricula = new Matricula(alunoId, cursoId);
        matricula.ConfirmarPagamento();
        matricula.Aluno = aluno;

        // Setup curso response mock
        var mockCursoResponse = new Mock<Response<ObterDetalhesCursoResponse>>();
        mockCursoResponse.Setup(x => x.Message).Returns(new ObterDetalhesCursoResponse(cursoId, 10, "Curso Teste", 100.00m));

        // Setup usuario response mock
        var mockUsuarioResponse = new Mock<Response<ObterDetalhesUsuarioResponse>>();
        mockUsuarioResponse.Setup(x => x.Message).Returns(new ObterDetalhesUsuarioResponse("Usuario Teste"));

        _alunoRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _alunoRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(aluno);
        _obterDetalhesCursoRequestMock.Setup(x => x.GetResponse<ObterDetalhesCursoResponse>(
            It.IsAny<ObterDetalhesCursoRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(mockCursoResponse.Object);
        _obterDetalhesUsuarioRequestMock.Setup(x => x.GetResponse<ObterDetalhesUsuarioResponse>(
            It.IsAny<ObterDetalhesUsuarioRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(mockUsuarioResponse.Object);
        _alunoRepositoryMock.Setup(x => x.GetAulasConcluidasCountAsync(matriculaId))
            .ReturnsAsync(10);
        _alunoRepositoryMock.Setup(x => x.AddCertificadoAsync(It.IsAny<Certificado>()))
            .Returns(Task.CompletedTask);
        _alunoRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _alunoRepositoryMock.Setup(x => x.GetByIdAsync(alunoId))
            .ReturnsAsync(aluno);

        // Act
        var resultado = await _alunoService.ConcluirMatriculaAsync(matriculaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusMatricula.Concluido);
        resultado.DataConclusao.Should().NotBeNull();
        resultado.PercentualProgresso.Should().Be(100);
        _alunoRepositoryMock.Verify(x => x.AddCertificadoAsync(It.IsAny<Certificado>()), Times.Once);
        _alunoRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConcluirMatriculaAsync_DeveLancarExcecaoQuandoNemTodasAulasConcluidas()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var alunoId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var matricula = new Matricula(alunoId, cursoId);
        var aluno = new Aluno(usuarioAtualId) { Id = alunoId };
        matricula.ConfirmarPagamento();

        // Setup curso response mock
        var mockResponse = new Mock<Response<ObterDetalhesCursoResponse>>();
        mockResponse.Setup(x => x.Message).Returns(new ObterDetalhesCursoResponse(cursoId, 10, "Curso Teste", 100.00m));

        _alunoRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _alunoRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(aluno);
        _obterDetalhesCursoRequestMock.Setup(x => x.GetResponse<ObterDetalhesCursoResponse>(
            It.IsAny<ObterDetalhesCursoRequest>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<RequestTimeout>()))
            .ReturnsAsync(mockResponse.Object);
        _alunoRepositoryMock.Setup(x => x.GetAulasConcluidasCountAsync(matriculaId))
            .ReturnsAsync(8);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _alunoService.ConcluirMatriculaAsync(matriculaId));
    }

    [Fact]
    public async Task ObterCertificadosDoAlunoAsync_DeveRetornarCertificados()
    {
        // Arrange
        var alunoId = Guid.CreateVersion7();
        var matriculaId = Guid.CreateVersion7();
        var certificados = new List<Certificado>
        {
            new Certificado(matriculaId, "Certificado 1", DateTime.Now, "CERT-001"),
            new Certificado(matriculaId, "Certificado 2", DateTime.Now, "CERT-002")
        };

        _alunoRepositoryMock.Setup(x => x.GetByIdAsync(alunoId))
            .ReturnsAsync(new Aluno(Guid.CreateVersion7()));
        _alunoRepositoryMock.Setup(x => x.GetCertificadosByAlunoIdAsync(alunoId))
            .ReturnsAsync(certificados);

        // Act
        var resultado = await _alunoService.ObterCertificadosDoAlunoAsync(alunoId, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(certificados);
    }

    [Fact]
    public async Task ObterCertificadosDoAlunoAsync_DeveLancarExcecaoQuandoAlunoNaoEncontrado()
    {
        // Arrange
        var alunoId = Guid.CreateVersion7();
        _alunoRepositoryMock.Setup(x => x.GetByIdAsync(alunoId))
            .ReturnsAsync((Aluno?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _alunoService.ObterCertificadosDoAlunoAsync(alunoId, CancellationToken.None));
    }
}