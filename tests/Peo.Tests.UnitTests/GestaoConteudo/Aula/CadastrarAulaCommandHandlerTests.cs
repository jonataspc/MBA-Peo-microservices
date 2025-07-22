using FluentAssertions;
using Moq;
using Peo.Core.Interfaces.Data;
using Peo.GestaoConteudo.Application.UseCases.Aula.Cadastrar;
using Peo.GestaoConteudo.Domain.ValueObjects;

namespace Peo.Tests.UnitTests.GestaoConteudo.Aula;

public class CadastrarAulaCommandHandlerTests
{
    private readonly Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>> _repositorioMock;
    private readonly Handler _handler;

    public CadastrarAulaCommandHandlerTests()
    {
        _repositorioMock = new Mock<IRepository<Peo.GestaoConteudo.Domain.Entities.Curso>>();
        _handler = new Handler(_repositorioMock.Object);
    }

    [Fact]
    public async Task Handler_DeveCadastrarAula_QuandoValido()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var curso = new Peo.GestaoConteudo.Domain.Entities.Curso(
            titulo: "Curso Teste",
            descricao: "Descrição Teste",
            instrutorId: Guid.CreateVersion7(),
            conteudoProgramatico: new ConteudoProgramatico("Conteúdo Programático Teste"),
            preco: 99.99m,
            estaPublicado: true,
            dataPublicacao: DateTime.Now,
            tags: ["teste", "curso"],
            aulas: []
        );

        var comando = new Command
        {
            CursoId = cursoId,
            Titulo = "Aula Teste",
            Descricao = "Descrição da Aula Teste",
            UrlVideo = "https://example.com/video",
            Duracao = TimeSpan.FromMinutes(30),
            Arquivos = []
        };

        _repositorioMock.Setup(x => x.WithTracking().GetAsync(cursoId))
            .ReturnsAsync(curso);
        _repositorioMock.Setup(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.AulaId.Should().NotBeEmpty();

        _repositorioMock.Verify(x => x.WithTracking().GetAsync(cursoId), Times.Once);
        _repositorioMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handler_DeveRetornarFalha_QuandoCursoNaoEncontrado()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();
        var comando = new Command
        {
            CursoId = cursoId,
            Titulo = "Aula Teste",
            Descricao = "Descrição da Aula Teste",
            UrlVideo = "https://example.com/video",
            Duracao = TimeSpan.FromMinutes(30),
            Arquivos = []
        };

        _repositorioMock.Setup(x => x.WithTracking().GetAsync(cursoId))
            .ReturnsAsync((Peo.GestaoConteudo.Domain.Entities.Curso?)null);

        // Act
        var resultado = await _handler.Handle(comando, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNull();
        resultado.Error.Message.Should().Be("Curso não encontrado");

        _repositorioMock.Verify(x => x.WithTracking().GetAsync(cursoId), Times.Once);
        _repositorioMock.Verify(x => x.UnitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}