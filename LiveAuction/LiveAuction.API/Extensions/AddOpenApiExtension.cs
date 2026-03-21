using LiveAuction.Application.Helpers;
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
                    document.Info = new ()
                    {
                        Title = "Live Auction API",
                        Description = ScalarDocumentInfo.GetScalarDocumentInfo(),
                    };
                    document.Components ??= new OpenApiComponents();

    
                    var schemeName = "Bearer";
                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Token"
                    };


                    document.AddComponent(schemeName, securityScheme);

                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        {

                            new OpenApiSecuritySchemeReference(schemeName, document),
                            new List<string>()
                        }
                    });

                    return Task.CompletedTask;
                });

            });

            return services;
        }
    }
}
