using MediatR;
using Peo.Core.DomainObjects.Result;
using System.ComponentModel.DataAnnotations;

namespace Peo.GestaoConteudo.Application.UseCases.Curso.Cadastrar;

public sealed record Command(

    [Required]
    string Titulo,

    string? Descricao,

    [Required]
    Guid InstrutorId,

    string? ConteudoProgramatico,

    [Required]
    decimal Preco,

    List<string>? Tags)

    : IRequest<Result<Response>>;