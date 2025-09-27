using FluentAssertions;
using Mapster;
using Moq;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Application.Dtos;
using Peo.GestaoConteudo.Application.UseCases.Curso.ObterPorId;
using Peo.GestaoConteudo.Domain.ValueObjects;

namespace Peo.Tests.UnitTests.GestaoConteudo.Curso;

public class ObterCursoPorIdQueryHandlerTests
{
    private readonly Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>> _repositorioMock;
    private readonly Handler _handler;

    public ObterCursoPorIdQueryHandlerTests()
    {
        _repositorioMock = new Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>>();
        _handler = new Handler(_repositorioMock.Object);
    }

    [Fact]
    public async Task Handler_DeveRetornarCurso_QuandoExiste()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var curso = new Peo.GestaoConteudo.Domain.Entities.Curso(
            titulo: "Curso Teste",
            descricao: "Descrição Teste",
            instrutorNome: "Nome Instrutor Teste",
            conteudoProgramatico: new ConteudoProgramatico("Conteúdo Programático Teste"),
            preco: 99.99m,
            estaPublicado: true,
            dataPublicacao: DateTime.Now,
            tags: new List<string> { "teste", "curso" },
            aulas: new List<Peo.GestaoConteudo.Domain.Entities.Aula>()
        );

        _repositorioMock.Setup(x => x.GetAsync(cursoId, CancellationToken.None))
            .ReturnsAsync(curso);

        var consulta = new Query(cursoId);

        // Act
        var resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Curso.Should().NotBeNull();
        resultado.Value.Curso.Should().BeEquivalentTo(curso.Adapt<CursoResponse>());
    }

    [Fact]
    public async Task Handler_DeveRetornarFalha_QuandoCursoNaoEncontrado()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        _repositorioMock.Setup(x => x.GetAsync(cursoId, CancellationToken.None))
            .ReturnsAsync((Peo.GestaoConteudo.Domain.Entities.Curso?)null);

        var consulta = new Query(cursoId);

        // Act
        var result = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Curso.Should().BeNull();
    }
}