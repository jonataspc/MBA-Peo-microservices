using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public class ConcluirMatriculaRequest
    {
        [Required]
        public Guid MatriculaId { get; set; }
    }
} 