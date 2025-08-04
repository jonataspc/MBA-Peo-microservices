using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public class MatriculaCursoRequest
    {
        [Required]
        public Guid CursoId { get; set; }
    }
} 