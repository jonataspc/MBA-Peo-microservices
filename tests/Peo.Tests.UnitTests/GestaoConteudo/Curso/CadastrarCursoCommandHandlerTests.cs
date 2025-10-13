using FluentAssertions;
using Moq;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Application.UseCases.Curso.Cadastrar;

namespace Peo.Tests.UnitTests.GestaoConteudo.Curso;

public class CadastrarCursoCommandHandlerTests
{
    private readonly Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>> _repositorioMock;
    private readonly Handler _handler;

    public CadastrarCursoCommandHandlerTests()
    {
        _repositorioMock = new Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>>();
        _handler = new Handler(_repositorioMock.Object);
    }

    [Fact]
    public async Task Handler_DeveCadastrarCurso_QuandoValido()
    {
        // Arrange
        var comando = new Command(
            Titulo: "Curso Teste",
            Descricao: "Descrição Teste",
            InstrutorNome: "Nome Instrutor Teste",
            ConteudoProgramatico: "Conteúdo Programático Teste",
            Preco: 99.99m,
            Tags: ["teste", "curso"]
        );

        _repositorioMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.CursoId.Should().NotBeEmpty();

        _repositorioMock.Verify(x => x.Insert(It.Is<Peo.GestaoConteudo.Domain.Entities.Curso>(c =>
            c.Titulo == comando.Titulo &&
            c.Descricao == comando.Descricao &&
            c.InstrutorNome == comando.InstrutorNome &&
            c.ConteudoProgramatico!.Conteudo == comando.ConteudoProgramatico &&
            c.Preco == comando.Preco &&
            c.Tags.SequenceEqual(comando.Tags!)
        ), CancellationToken.None), Times.Once);

        _repositorioMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}