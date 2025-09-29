using Peo.Core.Entities.Base;

namespace Peo.GestaoAlunos.Domain.Entities;

public class Certificado : EntityBase
{
    public Guid MatriculaId { get; private set; }
    public string Conteudo { get; private set; } = null!;
    public DateTime? DataEmissao { get; private set; }
    public string? NumeroCertificado { get; private set; }
    public virtual Matricula Matricula { get; private set; } = null!;

    protected Certificado()
    {
    }

    public Certificado(Guid matriculaId, string conteudo, DateTime? dataEmissao, string? numeroCertificado)
    {
        MatriculaId = matriculaId;
        Conteudo = conteudo;
        DataEmissao = dataEmissao;
        NumeroCertificado = numeroCertificado;
    }
}