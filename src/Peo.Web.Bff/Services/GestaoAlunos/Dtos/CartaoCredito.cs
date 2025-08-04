using System.ComponentModel.DataAnnotations;

namespace Peo.Web.Bff.Services.GestaoAlunos.Dtos
{
    public class CartaoCredito
    {
        [Required]
        public string Numero { get; set; } = null!;

        [Required]
        public string NomeTitular { get; set; } = null!;

        [Required]
        public string DataVencimento { get; set; } = null!;

        [Required]
        public string Cvv { get; set; } = null!;
    }
} 