using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public class ConcluirAulaRequest
    {
        [Required]
        public Guid MatriculaId { get; set; }

        [Required]
        public Guid AulaId { get; set; }
    }
} 