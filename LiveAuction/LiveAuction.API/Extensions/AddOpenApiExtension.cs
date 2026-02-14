using Microsoft.OpenApi;

namespace LiveAuction.API.Extensions
{
    public static class AddOpenApiExtension
    {
        public static IServiceCollection AddOpenApiConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Servers = new List<OpenApiServer>
                    {
                            new OpenApiServer { Url = configuration.GetSection("DomainUrl").Value }
                    };
                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}
