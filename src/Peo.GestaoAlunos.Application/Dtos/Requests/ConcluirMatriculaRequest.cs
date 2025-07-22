using System.ComponentModel.DataAnnotations;

namespace Peo.GestaoAlunos.Application.Dtos.Requests;

public class ConcluirMatriculaRequest
{
    [Required]
    public Guid MatriculaId { get; set; }
}