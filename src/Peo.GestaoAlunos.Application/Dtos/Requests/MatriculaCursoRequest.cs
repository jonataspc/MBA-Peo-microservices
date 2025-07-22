using System.ComponentModel.DataAnnotations;

namespace Peo.GestaoAlunos.Application.Dtos.Requests;

public class MatriculaCursoRequest
{
    [Required]
    public Guid CursoId { get; set; }
}