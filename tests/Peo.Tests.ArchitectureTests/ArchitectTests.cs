using FluentAssertions;
using NetArchTest.Rules;
using Peo.Core.Dtos;
using Peo.Core.Interfaces.Data;
using Peo.Faturamento.Application.Services;
using Peo.Faturamento.Domain.Dtos;
using Peo.Faturamento.Infra.Data.Helpers;
using Peo.Faturamento.Integrations.Paypal.Services;
using Peo.GestaoAlunos.Application.Endpoints;
using Peo.GestaoAlunos.Domain.Entities;
using Peo.GestaoAlunos.Infra.Data.Helpers;
using Peo.GestaoConteudo.Application.Services;
using Peo.GestaoConteudo.Domain.Entities;
using Peo.GestaoConteudo.Infra.Data.Helpers;
using Peo.Tests.ArchitectureTests.Extensions;
using System.Reflection;

namespace Peo.Tests.ArchitectureTests;

public class ArchitectTests
{
    private static IEnumerable<Assembly> GetDomainAssemblies()
    {
        return [
            typeof(Peo.Identity.Domain.Interfaces.Data.IUserRepository ).Assembly,
            typeof(CartaoCredito).Assembly,
            typeof(Curso).Assembly,
            typeof(Certificado ).Assembly,
            typeof(Peo.Core.DomainObjects.AccessRoles).Assembly,
            ];
    }

    private static IEnumerable<Assembly> GetAllAssemblies()
    {
        var list = new List<Assembly>
        {
            typeof(Peo.Web.Api.Services.AppIdentityUser).Assembly,
            typeof(Peo.Core.Web.Api.IEndpoint).Assembly,
            typeof(Peo.Core.Infra.Data.Contexts.Base.DbContextBase ).Assembly,
            typeof(GestaoAlunosDbMigrationHelpers).Assembly,
            typeof(EndpointsEstudante ).Assembly,
            typeof(Peo.Identity.Infra.Data.Helpers.IdentityDbMigrationHelpers).Assembly,
            typeof(Peo.Identity.Application.Services.TokenService).Assembly,
            typeof(GestaoConteudoDbMigrationHelpers).Assembly,
            typeof(CursoAulaService).Assembly,
            typeof(PaypalBrokerService ).Assembly,
            typeof(FaturamentoDbMigrationHelpers).Assembly,
            typeof(PagamentoService ).Assembly,
        };

        list.AddRange(GetDomainAssemblies());

        return list;
    }

    [Fact]
    public void Domain_Must_Not_Reference_External_Libraries()
    {
        var result = Types
            .InAssemblies(GetDomainAssemblies())
            .Should()
            .OnlyHaveDependencyOn(
            "System",
            "Microsoft",
            "MediatR",
            "FluentValidation",
            "Peo.Identity.Domain",
            "Peo.Core",
            "Peo.Faturamento.Domain",
            "Peo.GestaoConteudo.Domain",
            "Peo.GestaoAlunos.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(result.GetDetails());
    }

    [Fact]
    public void DataAccess_Must_Reside_In_Infra()
    {
        var result = Types.InAssemblies(GetAllAssemblies())
          .That()
          .HaveDependencyOnAll("Microsoft.EntityFrameworkCore")
          .And()
          .ResideInNamespace("Peo")
          .Should()
          .ResideInNamespaceMatching(@"^Peo.*Infra\.Data.*")
          .GetResult();

        result.IsSuccessful.Should().BeTrue(result.GetDetails());
    }

    [Fact]
    public void Repositories_Must_Have_Name_Ending_In_Repository()
    {
        var result = Types.InAssemblies(GetAllAssemblies())
          .That()
          .AreClasses()
          .And()
          .ImplementInterface(typeof(IRepository<>))
          .Should()
          .HaveNameEndingWith("Repository")
          .And()
          .ResideInNamespaceMatching(@"^Peo.*Infra\.Data.*")
          .GetResult();

        result.IsSuccessful.Should().BeTrue(result.GetDetails());
    }

    [Fact]
    public void Interfaces_Must_Start_Whith_I()
    {
        var result = Types.InAssemblies(GetAllAssemblies())
        .That()
        .AreInterfaces()
        .Should()
        .HaveNameStartingWith("I")
        .GetResult();

        result.IsSuccessful.Should().BeTrue(result.GetDetails());
    }
}