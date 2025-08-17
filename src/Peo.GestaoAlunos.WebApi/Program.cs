using Peo.Core.Web.Configuration;
using Peo.GestaoAlunos.Infra.Data.Helpers;
using Peo.GestaoAlunos.WebApi.Configuration;
using Peo.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddDependencies(builder.Configuration, builder.Environment)
                .AddSwagger("PEO - Gestao Aluno")
                .SetupWebApi(builder.Configuration)
                .AddPolicies()
                .AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseCustomSwagger(builder.Environment);
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapEndpoints();
 
await app.UseGestaoAlunosDbMigrationHelperAsync();
await app.RunAsync();