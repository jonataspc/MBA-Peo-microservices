using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public class IniciarAulaRequest
    {
        [Required]
        public Guid MatriculaId { get; set; }

        [Required]
        public Guid AulaId { get; set; }
    }
} 