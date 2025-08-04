using Peo.Core.Web.Configuration;
using Peo.Identity.Infra.Data.Helpers;
using Peo.Identity.WebApi.Configuration;
using Peo.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddDependencies(builder.Configuration, builder.Environment)
                .AddSwagger()
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

await app.UseIdentityDbMigrationHelperAsync();
await app.RunAsync();