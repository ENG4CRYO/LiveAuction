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
                    document.Info = new()
                    {
                        Title = "Live Auction API",
                        Description = ScalarDocumentInfo.GetScalarDocumentInfo(),
                    };
                    document.Components ??= new OpenApiComponents();

      
                    var jwtSchemeName = "Bearer";
                    var jwtSecurityScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Token for User Authentication"
                    };
                    document.AddComponent(jwtSchemeName, jwtSecurityScheme);

   
                    var appCheckSchemeName = "AppCheck";
                    var appCheckSecurityScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Name = "X-Firebase-AppCheck",    
                        In = ParameterLocation.Header,  
                        Description = "Firebase App Check Token for App Authentication"
                    };
                    document.AddComponent(appCheckSchemeName, appCheckSecurityScheme);

                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecuritySchemeReference(jwtSchemeName, document),
                            new List<string>()
                        },
                        {
                            new OpenApiSecuritySchemeReference(appCheckSchemeName, document),
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