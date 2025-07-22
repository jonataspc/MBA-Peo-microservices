using FluentAssertions;
using Moq;
using Peo.Core.DomainObjects;
using Peo.Core.Entities;
using Peo.Core.Interfaces.Services;
using Peo.Core.Interfaces.Services.Acls;
using Peo.GestaoAlunos.Application.Services;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Domain.Interfaces;
using Peo.GestaoAlunos.Domain.ValueObjects;
using System.Linq.Expressions;

namespace Peo.Tests.UnitTests.GestaoAlunos;

public class EstudanteServiceTests
{
    private readonly Mock<IEstudanteRepository> _estudanteRepositoryMock;
    private readonly Mock<ICursoAulaService> _aulaCursoServiceMock;
    private readonly Mock<IDetalhesUsuarioService> _detalhesUsuarioServiceMock;
    private readonly Mock<IAppIdentityUser> _appIdentityUserMock;
    private readonly EstudanteService _estudanteService;

    public EstudanteServiceTests()
    {
        _estudanteRepositoryMock = new Mock<IEstudanteRepository>();
        _aulaCursoServiceMock = new Mock<ICursoAulaService>();
        _detalhesUsuarioServiceMock = new Mock<IDetalhesUsuarioService>();
        _appIdentityUserMock = new Mock<IAppIdentityUser>();
        _estudanteService = new EstudanteService(
            _estudanteRepositoryMock.Object,
            _aulaCursoServiceMock.Object,
            _detalhesUsuarioServiceMock.Object,
            _appIdentityUserMock.Object);
    }

    [Fact]
    public async Task CriarEstudanteAsync_DeveCriarERetornarNovoEstudante()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var estudanteEsperado = new Estudante(usuarioId);
        _estudanteRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _estudanteService.CriarEstudanteAsync(usuarioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.UsuarioId.Should().Be(usuarioId);
        _estudanteRepositoryMock.Verify(x => x.AddAsync(It.Is<Estudante>(s => s.UsuarioId == usuarioId)), Times.Once);
        _estudanteRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MatricularEstudanteAsync_DeveCriarMatriculaQuandoValido()
    {
        // Arrange
        var estudanteId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var estudante = new Estudante(Guid.CreateVersion7());
        var matriculaEsperada = new Matricula(estudanteId, cursoId);

        _estudanteRepositoryMock.Setup(x => x.GetByIdAsync(estudanteId))
            .ReturnsAsync(estudante);
        _aulaCursoServiceMock.Setup(x => x.ValidarSeCursoExisteAsync(cursoId))
            .ReturnsAsync(true);
        _estudanteRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Estudante, bool>>>()))
            .ReturnsAsync(false);
        _estudanteRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _estudanteService.MatricularEstudanteAsync(estudanteId, cursoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.EstudanteId.Should().Be(estudanteId);
        resultado.CursoId.Should().Be(cursoId);
        _estudanteRepositoryMock.Verify(x => x.AddMatriculaAsync(It.Is<Matricula>(m => m.EstudanteId == estudanteId && m.CursoId == cursoId)), Times.Once);
        _estudanteRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MatricularEstudanteAsync_DeveLancarExcecaoQuandoEstudanteNaoEncontrado()
    {
        // Arrange
        var estudanteId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        _estudanteRepositoryMock.Setup(x => x.GetByIdAsync(estudanteId))
            .ReturnsAsync((Estudante?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _estudanteService.MatricularEstudanteAsync(estudanteId, cursoId));
    }

    [Fact]
    public async Task MatricularEstudanteAsync_DeveLancarExcecaoQuandoCursoNaoEncontrado()
    {
        // Arrange
        var estudanteId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var estudante = new Estudante(Guid.CreateVersion7());

        _estudanteRepositoryMock.Setup(x => x.GetByIdAsync(estudanteId))
            .ReturnsAsync(estudante);
        _aulaCursoServiceMock.Setup(x => x.ValidarSeCursoExisteAsync(cursoId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _estudanteService.MatricularEstudanteAsync(estudanteId, cursoId));
    }

    [Fact]
    public async Task IniciarAulaAsync_DeveCriarProgressoQuandoValido()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteId, Guid.CreateVersion7());
        var estudante = new Estudante(usuarioAtualId) { Id = estudanteId };

        matricula.ConfirmarPagamento();

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _estudanteRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(estudante);
        _estudanteRepositoryMock.Setup(x => x.GetProgressoMatriculaAsync(matriculaId, aulaId))
            .ReturnsAsync((ProgressoMatricula?)null);
        _estudanteRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _estudanteService.IniciarAulaAsync(matriculaId, aulaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.MatriculaId.Should().Be(matriculaId);
        resultado.AulaId.Should().Be(aulaId);
        resultado.EstaConcluido.Should().BeFalse();
        _estudanteRepositoryMock.Verify(x => x.AddProgressoMatriculaAsync(It.Is<ProgressoMatricula>(p => p.MatriculaId == matriculaId && p.AulaId == aulaId)), Times.Once);
        _estudanteRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConcluirAulaAsync_DeveAtualizarProgressoQuandoValido()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteId, Guid.CreateVersion7());
        var progresso = new ProgressoMatricula(matriculaId, aulaId);
        var estudante = new Estudante(usuarioAtualId) { Id = estudanteId };

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _estudanteRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(estudante);
        _estudanteRepositoryMock.Setup(x => x.GetProgressoMatriculaAsync(matriculaId, aulaId))
            .ReturnsAsync(progresso);
        _aulaCursoServiceMock.Setup(x => x.ObterTotalAulasDoCursoAsync(matricula.CursoId))
            .ReturnsAsync(10);
        _estudanteRepositoryMock.Setup(x => x.GetAulasConcluidasCountAsync(matriculaId))
            .ReturnsAsync(9);
        _estudanteRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _estudanteService.ConcluirAulaAsync(matriculaId, aulaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.MatriculaId.Should().Be(matriculaId);
        resultado.AulaId.Should().Be(aulaId);
        resultado.EstaConcluido.Should().BeTrue();
        _estudanteRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConcluirAulaAsync_DeveLancarExcecaoQuandoUsuarioAtualNaoEhEstudanteDaMatricula()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var estudanteMatriculaId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var estudanteDiferenteId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteMatriculaId, Guid.CreateVersion7());
        var estudante = new Estudante(usuarioAtualId) { Id = estudanteDiferenteId };

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _estudanteRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(estudante);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _estudanteService.ConcluirAulaAsync(matriculaId, aulaId));
    }

    [Fact]
    public async Task IniciarAulaAsync_DeveLancarExcecaoQuandoUsuarioAtualNaoEhEstudanteDaMatricula()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var aulaId = Guid.CreateVersion7();
        var estudanteMatriculaId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var estudanteDiferenteId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteMatriculaId, Guid.CreateVersion7());
        var estudante = new Estudante(usuarioAtualId) { Id = estudanteDiferenteId };

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _estudanteRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(estudante);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _estudanteService.IniciarAulaAsync(matriculaId, aulaId));
    }

    [Fact]
    public async Task MatricularEstudanteComUserIdAsync_DeveCriarEstudanteEMatricularQuandoValido()
    {
        // Arrange
        var usuarioId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var estudante = new Estudante(usuarioId) { Id = estudanteId };
        var matriculaEsperada = new Matricula(estudanteId, cursoId);

        _estudanteRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioId))
            .ReturnsAsync((Estudante?)null);

        _estudanteRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(estudante);

        //_estudanteRepositoryMock.Setup(x => x.Insert(It.Is<Estudante>(s => s.UsuarioId == usuarioId)))
        //    .ReturnsAsync(Task.CompletedTask);

        _aulaCursoServiceMock.Setup(x => x.ValidarSeCursoExisteAsync(cursoId))
            .ReturnsAsync(true);

        _estudanteRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Estudante, bool>>>()))
            .ReturnsAsync(false);

        _estudanteRepositoryMock.Setup(x => x.AddMatriculaAsync(It.IsAny<Matricula>()))
            .Returns(Task.CompletedTask);

        _estudanteRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var resultado = await _estudanteService.MatricularEstudanteComUserIdAsync(usuarioId, cursoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.CursoId.Should().Be(cursoId);
        _estudanteRepositoryMock.Verify(x => x.AddAsync(It.Is<Estudante>(s => s.UsuarioId == usuarioId)), Times.Once);
        _estudanteRepositoryMock.Verify(x => x.AddMatriculaAsync(It.Is<Matricula>(m => m.CursoId == cursoId)), Times.Once);
        _estudanteRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ObterProgressoGeralCursoAsync_DeveRetornarPorcentagemProgresso()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteId, Guid.CreateVersion7());
        var estudante = new Estudante(usuarioAtualId) { Id = estudanteId };
        matricula.AtualizarProgresso(75);

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _estudanteRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(estudante);

        // Act
        var resultado = await _estudanteService.ObterProgressoGeralCursoAsync(matriculaId);

        // Assert
        resultado.Should().Be(75);
    }

    [Fact]
    public async Task ConcluirMatriculaAsync_DeveConcluirMatriculaEGerarCertificado()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var estudante = new Estudante(usuarioAtualId) { Id = estudanteId };
        var matricula = new Matricula(estudanteId, cursoId);
        matricula.ConfirmarPagamento();
        matricula.Estudante = estudante;

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _estudanteRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(estudante);
        _aulaCursoServiceMock.Setup(x => x.ObterTotalAulasDoCursoAsync(cursoId))
            .ReturnsAsync(10);
        _estudanteRepositoryMock.Setup(x => x.GetAulasConcluidasCountAsync(matriculaId))
            .ReturnsAsync(10);
        _aulaCursoServiceMock.Setup(x => x.ObterTituloCursoAsync(cursoId))
            .ReturnsAsync("Curso Teste");
        _detalhesUsuarioServiceMock.Setup(x => x.ObterUsuarioPorIdAsync(usuarioAtualId))
            .ReturnsAsync(new Usuario(usuarioAtualId, "Usuario Teste", "teste@exemplo.com"));
        _estudanteRepositoryMock.Setup(x => x.AddCertificadoAsync(It.IsAny<Certificado>()))
            .Returns(Task.CompletedTask);
        _estudanteRepositoryMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _estudanteRepositoryMock.Setup(x => x.GetByIdAsync(estudanteId))
            .ReturnsAsync(estudante);

        // Act
        var resultado = await _estudanteService.ConcluirMatriculaAsync(matriculaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusMatricula.Concluido);
        resultado.DataConclusao.Should().NotBeNull();
        resultado.PercentualProgresso.Should().Be(100);
        _estudanteRepositoryMock.Verify(x => x.AddCertificadoAsync(It.IsAny<Certificado>()), Times.Once);
        _estudanteRepositoryMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConcluirMatriculaAsync_DeveLancarExcecaoQuandoNemTodasAulasConcluidas()
    {
        // Arrange
        var matriculaId = Guid.CreateVersion7();
        var estudanteId = Guid.CreateVersion7();
        var usuarioAtualId = Guid.CreateVersion7();
        var cursoId = Guid.CreateVersion7();
        var matricula = new Matricula(estudanteId, cursoId);
        var estudante = new Estudante(usuarioAtualId) { Id = estudanteId };
        matricula.ConfirmarPagamento();

        _estudanteRepositoryMock.Setup(x => x.GetMatriculaByIdAsync(matriculaId))
            .ReturnsAsync(matricula);
        _appIdentityUserMock.Setup(x => x.GetUserId())
            .Returns(usuarioAtualId);
        _estudanteRepositoryMock.Setup(x => x.GetByUserIdAsync(usuarioAtualId))
            .ReturnsAsync(estudante);
        _aulaCursoServiceMock.Setup(x => x.ObterTotalAulasDoCursoAsync(cursoId))
            .ReturnsAsync(10);
        _estudanteRepositoryMock.Setup(x => x.GetAulasConcluidasCountAsync(matriculaId))
            .ReturnsAsync(8);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _estudanteService.ConcluirMatriculaAsync(matriculaId));
    }

    [Fact]
    public async Task ObterCertificadosDoEstudanteAsync_DeveRetornarCertificados()
    {
        // Arrange
        var estudanteId = Guid.CreateVersion7();
        var matriculaId = Guid.CreateVersion7();
        var certificados = new List<Certificado>
        {
            new Certificado(matriculaId, "Certificado 1", DateTime.Now, "CERT-001"),
            new Certificado(matriculaId, "Certificado 2", DateTime.Now, "CERT-002")
        };

        _estudanteRepositoryMock.Setup(x => x.GetByIdAsync(estudanteId))
            .ReturnsAsync(new Estudante(Guid.CreateVersion7()));
        _estudanteRepositoryMock.Setup(x => x.GetCertificadosByEstudanteIdAsync(estudanteId))
            .ReturnsAsync(certificados);

        // Act
        var resultado = await _estudanteService.ObterCertificadosDoEstudanteAsync(estudanteId, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(certificados);
    }

    [Fact]
    public async Task ObterCertificadosDoEstudanteAsync_DeveLancarExcecaoQuandoEstudanteNaoEncontrado()
    {
        // Arrange
        var estudanteId = Guid.CreateVersion7();
        _estudanteRepositoryMock.Setup(x => x.GetByIdAsync(estudanteId))
            .ReturnsAsync((Estudante?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _estudanteService.ObterCertificadosDoEstudanteAsync(estudanteId, CancellationToken.None));
    }
}