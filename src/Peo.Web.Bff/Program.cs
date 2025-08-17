using Peo.Core.Web.Configuration;
using Peo.ServiceDefaults;
using Peo.Web.Bff.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddExceptionHandler();
builder.Services.AddIdentity(builder.Configuration)
                .AddFaturamento(builder.Configuration)
                .AddGestaoConteudo(builder.Configuration)
                .AddGestaoAlunos(builder.Configuration)
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
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseHttpsRedirection();

app.AddIdentityEndpoints();
app.AddFaturamentoEndpoints();
app.AddGestaoConteudoEndpoints();
app.AddGestaoAlunosEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

await app.RunAsync();