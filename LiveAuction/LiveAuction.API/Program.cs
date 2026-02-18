using LiveAuction.api.Extensions;
using LiveAuction.api.Factories;
using LiveAuction.api.Middlewares;
using LiveAuction.API.Extensions;
using LiveAuction.Application.Common;
using LiveAuction.Application.Extensions;
using LiveAuction.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

SerilogExtension.SetupBootstrapLogger();

try
{
    Log.Information("Starting Web Application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.RegisterSerilog();
    builder.Services.AddGlobalRateLimiter();
    builder.Services.AddGlobalCors(builder.Configuration);
    builder.Services.AddGlobalHealthChecks();
    builder.Services.AddApiResponseCompression();

    builder.Services.AddOpenApiConfig(builder.Configuration);
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddInfrastructureService(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddFluentValidationAutoValidation(config =>
    config.OverrideDefaultResultFactoryWith<CustomResultFactory>());

    builder.Services.AddApiVersion();

    builder.Services.AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });



    var app = builder.Build();


    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                   Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    });

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<LiveAuction.Infrastructure.Data.AppDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error While Create Database");
        }
    }


    app.UseSerilogRequestLogging();

    app.UseMiddleware<GlobalErrorHandlerMiddleware>();
    app.UseStaticFiles();
    app.UseResponseCompression();
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("LiveAuction API Documentation"); 
        options.WithTheme(ScalarTheme.BluePlanet);
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseCors("AllowAll");
    }
    else
    {
        app.UseCors("Production");
    }

    app.UseHttpsRedirection();

    app.UseSecurityHeaders(PolicyCollectionExtension.policyCollection(app));
    app.UseGlobalHealthChecks();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}