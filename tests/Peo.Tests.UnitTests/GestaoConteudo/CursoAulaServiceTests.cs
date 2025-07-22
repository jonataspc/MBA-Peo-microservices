using FluentAssertions;
using Moq;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Application.Services;

namespace Peo.Tests.UnitTests.GestaoConteudo;

public class CursoAulaServiceTests
{
    private readonly Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>> _cursoRepositoryMock;
    private readonly CursoAulaService _cursoAulaService;

    public CursoAulaServiceTests()
    {
        _cursoRepositoryMock = new Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>>();
        _cursoAulaService = new CursoAulaService(_cursoRepositoryMock.Object);
    }

    [Fact]
    public async Task ValidarSeCursoExisteAsync_DeveRetornarVerdadeiroQuandoCursoExiste()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var curso = new Peo.GestaoConteudo.Domain.Entities.Curso("Curso Teste", "Descrição", Guid.CreateVersion7(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), new List<Peo.GestaoConteudo.Domain.Entities.Aula>());

        _cursoRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Peo.GestaoConteudo.Domain.Entities.Curso, bool>>>()))
            .ReturnsAsync(true);

        // Act
        var resultado = await _cursoAulaService.ValidarSeCursoExisteAsync(cursoId);

        // Assert
        resultado.Should().BeTrue();
        _cursoRepositoryMock.Verify(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Peo.GestaoConteudo.Domain.Entities.Curso, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task ValidarSeCursoExisteAsync_DeveRetornarFalsoQuandoCursoNaoExiste()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();

        _cursoRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Peo.GestaoConteudo.Domain.Entities.Curso, bool>>>()))
            .ReturnsAsync(false);

        // Act
        var resultado = await _cursoAulaService.ValidarSeCursoExisteAsync(cursoId);

        // Assert
        resultado.Should().BeFalse();
        _cursoRepositoryMock.Verify(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Peo.GestaoConteudo.Domain.Entities.Curso, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task ObterPrecoCursoAsync_DeveRetornarPrecoDoCurso()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var precoEsperado = 99.99m;
        var curso = new Peo.GestaoConteudo.Domain.Entities.Curso("Curso Teste", "Descrição", Guid.CreateVersion7(), null, precoEsperado, true, DateTime.UtcNow, new List<string>(), new List<Peo.GestaoConteudo.Domain.Entities.Aula>());

        _cursoRepositoryMock.Setup(x => x.GetAsync(cursoId))
            .ReturnsAsync(curso);

        // Act
        var resultado = await _cursoAulaService.ObterPrecoCursoAsync(cursoId);

        // Assert
        resultado.Should().Be(precoEsperado);
        _cursoRepositoryMock.Verify(x => x.GetAsync(cursoId), Times.Once);
    }

    [Fact]
    public async Task ObterTituloCursoAsync_DeveRetornarTituloDoCurso()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var tituloEsperado = "Curso Teste";
        var curso = new Peo.GestaoConteudo.Domain.Entities.Curso(tituloEsperado, "Descrição", Guid.CreateVersion7(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), new List<Peo.GestaoConteudo.Domain.Entities.Aula>());

        _cursoRepositoryMock.Setup(x => x.GetAsync(cursoId))
            .ReturnsAsync(curso);

        // Act
        var resultado = await _cursoAulaService.ObterTituloCursoAsync(cursoId);

        // Assert
        resultado.Should().Be(tituloEsperado);
        _cursoRepositoryMock.Verify(x => x.GetAsync(cursoId), Times.Once);
    }

    [Fact]
    public async Task ObterTotalAulasDoCursoAsync_DeveRetornarQuantidadeDeAulas()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var quantidadeEsperada = 10;
        var aulas = Enumerable.Range(0, quantidadeEsperada)
            .Select(_ => new Peo.GestaoConteudo.Domain.Entities.Aula("Aula Teste", "Descrição", "video-url", TimeSpan.FromMinutes(30), new List<Peo.GestaoConteudo.Domain.Entities.ArquivoAula>(), cursoId))
            .ToList();
        var curso = new Peo.GestaoConteudo.Domain.Entities.Curso("Curso Teste", "Descrição", Guid.CreateVersion7(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), aulas);

        _cursoRepositoryMock.Setup(x => x.GetAsync(cursoId))
            .ReturnsAsync(curso);

        // Act
        var resultado = await _cursoAulaService.ObterTotalAulasDoCursoAsync(cursoId);

        // Assert
        resultado.Should().Be(quantidadeEsperada);
        _cursoRepositoryMock.Verify(x => x.GetAsync(cursoId), Times.Once);
    }

    [Fact]
    public async Task ObterTotalAulasDoCursoAsync_DeveRetornarZeroQuandoNaoHaAulas()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var curso = new Peo.GestaoConteudo.Domain.Entities.Curso("Curso Teste", "Descrição", Guid.CreateVersion7(), null, 99.99m, true, DateTime.UtcNow, new List<string>(), new List<Peo.GestaoConteudo.Domain.Entities.Aula>());

        _cursoRepositoryMock.Setup(x => x.GetAsync(cursoId))
            .ReturnsAsync(curso);

        // Act
        var resultado = await _cursoAulaService.ObterTotalAulasDoCursoAsync(cursoId);

        // Assert
        resultado.Should().Be(0);
        _cursoRepositoryMock.Verify(x => x.GetAsync(cursoId), Times.Once);
    }
}