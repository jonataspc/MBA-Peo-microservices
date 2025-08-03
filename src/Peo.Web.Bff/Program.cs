using Peo.Core.Web.Configuration;
using Peo.ServiceDefaults;
using Peo.Web.Bff.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddExceptionHandler();
builder.Services.AddIdentity(builder.Configuration)
                .AddGestaoConteudo(builder.Configuration)
                .AddGestaoAlunos(builder.Configuration)
                .AddSwagger()
                .AddApiServices()
                .SetupWebApi(builder.Configuration);

var app = builder.Build();

app.UseCustomSwagger(builder.Environment);
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseHttpsRedirection();

app.AddIdentityEndpoints();
app.AddGestaoConteudoEndpoints();
app.AddGestaoAlunosEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

await app.RunAsync();