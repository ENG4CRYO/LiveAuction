using LiveAuction.api.Extensions;
using LiveAuction.api.Factories;
using LiveAuction.api.Middlewares;
using LiveAuction.API.Extensions;
using LiveAuction.API.Helper.CustomCssScalar;
using LiveAuction.API.Middlewares;
using LiveAuction.Application.Common;
using LiveAuction.Application.Extensions;
using LiveAuction.Infrastructure.Data;
using LiveAuction.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

SerilogExtension.SetupBootstrapLogger();

try
{
    Log.Information("Starting Web Application...");
    string firebaseKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "secrets", "firebase-adminsdk.json");

    if (File.Exists(firebaseKeyPath))
    {
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(firebaseKeyPath)
        });
        Console.WriteLine("Firebase App Check Initialized Successfully!");
    }
    else
    {
        Console.WriteLine($"WARNING: Firebase JSON key not found at {firebaseKeyPath}");
    }

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
            var context = services.GetRequiredService<AppDbContext>();
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
    app.UseMiddleware<FirebaseAppCheckMiddleware>();
    app.UseStaticFiles();
    app.UseResponseCompression();
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("LiveAuction API Documentation");
        options.WithTheme(ScalarTheme.BluePlanet);
        options.Layout = ScalarLayout.Modern;
        options.CustomCss = CssScalar.CustomCss;

        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = new[] { "Bearer", "AppCheck" }
        };
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
    app.UseRateLimiter();
    app.UseGlobalHealthChecks();
    app.UseMiddleware<AppVersionCheckMiddleware>();
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