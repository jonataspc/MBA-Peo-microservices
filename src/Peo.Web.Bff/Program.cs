using Peo.Core.Web.Configuration;
using Peo.ServiceDefaults;
using Peo.Web.Bff.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddExceptionHandler();
builder.Services.AddIdentity(builder.Configuration)
                .AddHttpContextAccessor()
                .AddFaturamento(builder.Configuration)
                .AddGestaoConteudo(builder.Configuration)
                .AddGestaoAlunos(builder.Configuration)
                .AddHistorico()
                .AddSwagger("PEO - BFF")
                .AddApiServices()
                .SetupWebApi(builder.Configuration)
                .AddPolicies()
                .AddJwtAuthentication(builder.Configuration);

builder.Services.AddOpenApiDocument(o =>
{
    o.DocumentName = "v1"; // nome que vamos pedir ao gerador
    o.Title = "Plataforma de Educação Online - WebAPI";
});

var app = builder.Build();

app.UseCustomSwagger(builder.Environment);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.AddIdentityEndpoints();
app.AddFaturamentoEndpoints();
app.AddGestaoConteudoEndpoints();
app.AddGestaoAlunosEndpoints();
app.AddHistoricoEndpoints();

await app.RunAsync();