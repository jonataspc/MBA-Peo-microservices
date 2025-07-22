using MediatR;
using Peo.Core.DomainObjects.Result;
using Peo.GestaoConteudo.Application.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peo.GestaoConteudo.Application.UseCases.Aula.Cadastrar;

public sealed class Command : IRequest<Result<Response>>
{
    [NotMapped]
    public Guid CursoId { get; set; }

    [Required]
    public required string Titulo { get; set; }

    public string? Descricao { get; set; }

    [Required]
    public required string UrlVideo { get; set; }

    [Required]
    public TimeSpan Duracao { get; set; }

    public IEnumerable<ArquivoAulaRequest> Arquivos { get; set; } = [];
}