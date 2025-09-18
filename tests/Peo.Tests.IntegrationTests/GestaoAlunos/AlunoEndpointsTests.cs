extern alias GestaoAlunosWebApi;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Peo.Core.DomainObjects.Result;
using Peo.GestaoAlunos.Application.Dtos.Requests;
using Peo.GestaoAlunos.Application.Dtos.Responses;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoConteudo.Domain.Entities;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Peo.Tests.IntegrationTests.Setup;
using System.Net;
using System.Net.Http.Json;
using Peo.Tests.IntegrationTests.Factories;

namespace Peo.Tests.IntegrationTests.GestaoAlunos;

public class AlunoEndpointsTests : IClassFixture<IntegrationTestFactory<GestaoAlunosWebApi.Program>>, IAsyncLifetime
{
    private readonly IntegrationTestFactory<GestaoAlunosWebApi.Program> _factory;
    private readonly HttpClient _client;
    private readonly TestDatabaseSetup _testDb;
    private Guid _testUserId = Guid.CreateVersion7();
    private Curso _testCurso = null!;
    private Curso _testCursoNaoMatriculado = null!;
    private Aluno? _testAluno;
    private Matricula? _testMatricula;
    private Matricula? _testMatriculaNaoPaga;

    public AlunoEndpointsTests(IntegrationTestFactory<GestaoAlunosWebApi.Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _testDb = new TestDatabaseSetup(_factory.Services);
    }

    public async Task InitializeAsync()
    {
        await _testDb.InitializeAsync();

        _testAluno = await _testDb.CriarAlunoTesteAsync(_testUserId);

        var curso = await _testDb.CriarCursoTesteAsync(instrutorId: _testAluno.UsuarioId);
        _testCurso = curso;

        var cursoDois = await _testDb.CriarCursoTesteAsync(instrutorId: _testAluno.UsuarioId);

        var cursoNaoMatriculado = await _testDb.CriarCursoTesteAsync(instrutorId: _testAluno.UsuarioId);
        _testCursoNaoMatriculado = cursoNaoMatriculado;

        // Create test
        _testMatricula = await _testDb.CriarMatriculaTesteAsync(_testAluno.Id, _testCurso.Id, true);
        _testMatriculaNaoPaga = await _testDb.CriarMatriculaTesteAsync(_testAluno.Id, cursoDois.Id, false);

        await LoginAsync();
    }

    private async Task LoginAsync()
    {
        // Arrange
        var request = new LoginRequest(_testDb.EmailUsuarioTeste, _testDb.SenhaUsuarioTeste);

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();

        _testUserId = result!.UserId;

        // Set the token in the client
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
    }

    public async Task DisposeAsync()
    {
        await _testDb.LimparAsync();
    }

    [Fact]
    public async Task ObterCertificadosAluno_DeveRetornarCertificados()
    {
        // Arrange
        await _testDb.CriarCertificadoTesteAsync(
            _testMatricula!.Id,
            "Test Certificate Content");

        // Act
        var response = await _client.GetAsync("/v1/aluno/certificados");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var certificados = await response.Content.ReadFromJsonAsync<IEnumerable<CertificadoAlunoResponse>>();
        certificados.Should().NotBeNull();
        certificados.Should().Contain(c => c.MatriculaId == _testMatricula.Id);
    }

    [Fact]
    public async Task IniciarAula_ComRequisicaoValida_DeveIniciarAula()
    {
        // Arrange
        var request = new IniciarAulaRequest
        {
            MatriculaId = _testMatricula!.Id,
            AulaId = _testCurso.Aulas.First().Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/aluno/matricula/aula/iniciar", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProgressoAulaResponse>();
        result.Should().NotBeNull();
        result!.MatriculaId.Should().Be(request.MatriculaId);
        result.AulaId.Should().Be(request.AulaId);
        result.EstaConcluida.Should().BeFalse();
    }

    [Fact]
    public async Task ConcluirAula_ComRequisicaoValida_DeveConcluirAula()
    {
        // Arrange
        var aulaId = Guid.CreateVersion7();
        await _testDb.CriarProgressoAulaTesteAsync(_testMatricula!.Id, aulaId);

        var request = new ConcluirAulaRequest
        {
            MatriculaId = _testMatricula.Id,
            AulaId = aulaId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/aluno/matricula/aula/concluir", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProgressoAulaResponse>();
        result.Should().NotBeNull();
        result!.MatriculaId.Should().Be(request.MatriculaId);
        result.AulaId.Should().Be(request.AulaId);
        result.EstaConcluida.Should().BeTrue();
    }

    [Fact]
    public async Task MatricularCurso_ComRequisicaoValida_QuandoJaMatriculadoNoCurso()
    {
        // Arrange
        var request = new MatriculaCursoRequest
        {
            CursoId = _testCurso.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/aluno/matricula", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadAsStringAsync();
        result.Should().Contain("Aluno já está matriculado neste curso");
    }

    [Fact]
    public async Task MatricularCurso_ComRequisicaoValida_DeveCriarMatricula()
    {
        // Arrange
        var request = new MatriculaCursoRequest
        {
            CursoId = _testCursoNaoMatriculado.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/aluno/matricula", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<MatriculaCursoResponse>();
        result.Should().NotBeNull();
        result!.MatriculaId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task ConcluirMatricula_ComRequisicaoValida_DeveConcluirMatricula()
    {
        // Arrange

        foreach (var aula in _testCurso.Aulas)
        {
            var requestLessonStart = new IniciarAulaRequest
            {
                MatriculaId = _testMatricula!.Id,
                AulaId = aula.Id
            };

            await _client.PostAsJsonAsync("/v1/aluno/matricula/aula/iniciar", requestLessonStart);

            var requestLessonEnd = new ConcluirAulaRequest
            {
                MatriculaId = _testMatricula.Id,
                AulaId = aula.Id
            };
            await _client.PostAsJsonAsync("/v1/aluno/matricula/aula/concluir", requestLessonEnd);
        }

        var request = new ConcluirMatriculaRequest
        {
            MatriculaId = _testMatricula!.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/aluno/matricula/concluir", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ConcluirMatriculaResponse>();
        result.Should().NotBeNull();
        result!.MatriculaId.Should().Be(request.MatriculaId);
        result.Status.Should().Be("Concluido");
    }

    [Fact]
    public async Task PagamentoMatricula_ComRequisicaoValida_DeveProcessarPagamento()
    {
        // Arrange
        var request = new PagamentoMatriculaRequest
        {
            MatriculaId = _testMatriculaNaoPaga!.Id,
            DadosCartao = new(
                "4111111111111111",
                "12/25",
                "123",
                "John Doe"
            )
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/aluno/matricula/pagamento", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagamentoMatriculaResponse>();
        result.Should().NotBeNull();
        result!.MatriculaId.Should().Be(_testMatriculaNaoPaga.Id);
        result.StatusPagamento.Should().Be(Faturamento.Domain.ValueObjects.StatusPagamento.Pago.ToString());
    }

    [Fact]
    public async Task PagamentoMatricula_ComMatriculaInvalida_DeveRetornarBadRequest()
    {
        // Arrange
        var request = new PagamentoMatriculaRequest
        {
            MatriculaId = Guid.CreateVersion7(), // Matricula nao existente
            DadosCartao = new(
                "4111111111111111",
                "12/25",
                "123",
                "John Doe"
            )
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/aluno/matricula/pagamento", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        result.Should().NotBeNull();
        result!.Message.Should().Contain("Matrícula não encontrada");
    }

    [Fact]
    public async Task PagamentoMatricula_ComCartaoInvalido_DeveRetornarErroValidacao()
    {
        // Arrange
        var request = new PagamentoMatriculaRequest
        {
            MatriculaId = _testMatriculaNaoPaga!.Id,
            DadosCartao = new(
                "1234", // Invalid card number
                "12/25",
                "123",
                "John Doe")
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/aluno/matricula/pagamento", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        result.Should().NotBeNull();
        result!.Message.Should().Contain("Credit card is invalid");
    }

    [Fact]
    public async Task PagamentoMatricula_ComCamposObrigatoriosFaltando_DeveRetornarErroValidacao()
    {
        // Arrange
        var request = new PagamentoMatriculaRequest
        {
            MatriculaId = _testMatriculaNaoPaga!.Id,
            DadosCartao = new(
                null, // Missing card number
                "12/25",
                "123",
                "John Doe")
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/aluno/matricula/pagamento", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        result.Should().NotBeNull();
        result!.Message.Should().Contain("Credit card is null");
    }
}