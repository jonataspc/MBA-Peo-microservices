using Peo.Web.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

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
app.UseExceptionHandler();

await app.UseDbMigrationHelperAsync();
await app.RunAsync();