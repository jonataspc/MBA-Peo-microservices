using Microsoft.AspNetCore.Components;
using Peo.Web.Spa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Peo.Web.Spa.Pages.Cursos
{
    public partial class Cursos
    {
        private IEnumerable<CursoResponse> _cursosLista = new List<CursoResponse>();

        protected override async Task OnInitializedAsync()
        {
        }

        private void AdicionarCurso()
        {
            var moduloCurso = ToRoman(_cursosLista.Count()+1);
            var novoCurso = new CursoResponse()
            {
                Id = Guid.NewGuid(),
                Titulo = "Curso de C# modulo "  + moduloCurso   ,
                Descricao = "Aprenda C# do básico ao avançado" + (_cursosLista.Count()+1).ToString(),
                Preco = 300.00m
            };
            _cursosLista = _cursosLista.Append(novoCurso);
        }

        public static string ToRoman(int number)
        {
            if (number < 1 || number > 3999) return "Número fora do intervalo";

            var map = new Dictionary<int, string>
            {
                {1000, "M"}, {900, "CM"}, {500, "D"}, {400, "CD"},
                {100, "C"}, {90, "XC"}, {50, "L"}, {40, "XL"},
                {10, "X"}, {9, "IX"}, {5, "V"}, {4, "IV"}, {1, "I"}
            };

            var roman = new StringBuilder();

            foreach (var kvp in map)
            {
                while (number >= kvp.Key)
                {
                    roman.Append(kvp.Value);
                    number -= kvp.Key;
                }
            }

            return roman.ToString();
        }

    }

    public class CursoResponse
    {
        public Guid Id { get; set; } 
        public string? Titulo { get; set; } 
        public string? Descricao { get; set; } 
        public decimal? Preco { get; set; } 

    }
}
