namespace Peo.GestaoConteudo.Domain.ValueObjects
{
    public class ConteudoProgramatico
    {
        public string? Conteudo { get; private set; }

        public ConteudoProgramatico(string? conteudo)
        {
            Conteudo = conteudo;
        }
    }
}