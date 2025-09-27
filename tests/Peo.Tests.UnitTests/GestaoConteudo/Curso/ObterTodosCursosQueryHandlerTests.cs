using FluentAssertions;
using Mapster;
using Moq;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Application.Dtos;
using Peo.GestaoConteudo.Application.UseCases.Curso.ObterTodos;
using Peo.GestaoConteudo.Domain.ValueObjects;

namespace Peo.Tests.UnitTests.GestaoConteudo.Curso;

public class ObterTodosCursosQueryHandlerTests
{
    private readonly Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>> _repositorioMock;
    private readonly Handler _handler;

    public ObterTodosCursosQueryHandlerTests()
    {
        _repositorioMock = new Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>>();
        _handler = new Handler(_repositorioMock.Object);
    }

    [Fact]
    public async Task Handler_DeveRetornarTodosCursos()
    {
        // Arrange
        var cursos = new List<Peo.GestaoConteudo.Domain.Entities.Curso>
        {
            new(
                titulo: "Curso Teste 1",
                descricao: "Descrição Teste 1",
                instrutorNome: "Nome Instrutor Teste",
                conteudoProgramatico: new ConteudoProgramatico("Conteúdo Programático Teste 1"),
                preco: 99.99m,
                estaPublicado: true,
                dataPublicacao: DateTime.Now,
                tags: new List<string> { "teste", "curso" },
                aulas: new List<Peo.GestaoConteudo.Domain.Entities.Aula>()
            ),
            new(
                titulo: "Curso Teste 2",
                descricao: "Descrição Teste 2",
                instrutorNome:"Nome Instrutor Teste",
                conteudoProgramatico: new ConteudoProgramatico("Conteúdo Programático Teste 2"),
                preco: 199.99m,
                estaPublicado: true,
                dataPublicacao: DateTime.Now,
                tags: new List<string> { "teste", "curso" },
                aulas: new List<Peo.GestaoConteudo.Domain.Entities.Aula>()
            )
        };

        _repositorioMock.Setup(x => x.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(cursos);

        var consulta = new Query();

        // Act
        var resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Cursos.Should().NotBeNull();
        resultado.Value.Cursos.Should().HaveCount(2);
        resultado.Value.Cursos.Should().BeEquivalentTo(cursos.Adapt<IEnumerable<CursoResponse>>());
    }

    [Fact]
    public async Task Handler_DeveRetornarListaVazia_QuandoNaoExistemCursos()
    {
        // Arrange
        _repositorioMock.Setup(x => x.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(Enumerable.Empty<Peo.GestaoConteudo.Domain.Entities.Curso>());

        var consulta = new Query();

        // Act
        var resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Cursos.Should().NotBeNull();
        resultado.Value.Cursos.Should().BeEmpty();
    }
}