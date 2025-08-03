using Peo.Web.Bff.Middlewares;

namespace Peo.Web.Bff.Configuration
{
    public static class MiddlewaresDependencies

    {
        public static WebApplicationBuilder AddExceptionHandler(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
                builder.Services.AddProblemDetails();
            }

            return builder;
        }
    }
}