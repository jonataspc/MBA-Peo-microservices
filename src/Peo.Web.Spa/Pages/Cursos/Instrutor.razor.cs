using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Peo.Web.Spa.Pages.Cursos
{
    public partial class Instrutor 
    {
        private IEnumerable<InstrutorResponse> instrutores = new List<InstrutorResponse>
        {
            new InstrutorResponse { Id = Guid.NewGuid(), Nome = "John", Sobrenome = "Doe" },
            new InstrutorResponse { Id = Guid.NewGuid(), Nome = "Jane", Sobrenome = "Smith" },
            new InstrutorResponse { Id = Guid.NewGuid(), Nome = "Michael", Sobrenome = "Johnson" }
        };

        protected override void OnInitialized()
        {

        }

        private void AdicionarInstrutor()
        {
            var novoInstrutor = new InstrutorResponse
            {
                Id = Guid.NewGuid(),
                Nome = "Novo",
                Sobrenome = "Instrutor"
            };
            instrutores = instrutores.Append(novoInstrutor);
        }
    }

    public class InstrutorResponse 
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;

    }
}
