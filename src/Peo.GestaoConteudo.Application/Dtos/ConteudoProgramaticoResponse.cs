using Peo.GestaoConteudo.Domain.ValueObjects;

namespace Peo.GestaoConteudo.Application.Dtos;

public class ConteudoProgramaticoResponse
{
    public Guid Id { get; set; }
    
    public ConteudoProgramatico? ConteudoProgramatico { get; set; }
}