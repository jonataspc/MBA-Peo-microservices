using Peo.Core.Web.Configuration;
using Peo.ServiceDefaults;
using Peo.Web.Bff.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddExceptionHandler();
builder.Services.AddIdentity(builder.Configuration)
                .AddSwagger()
                .SetupWebApi(builder.Configuration);

var app = builder.Build();

app.UseCustomSwagger(builder.Environment);
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseHttpsRedirection();

app.AddIdentityEndpoints();
// demais endpoints podem ser adicionados aqui

app.UseExceptionHandler();
await app.RunAsync();