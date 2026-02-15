using System;
using System.Collections.Generic;
using System.Text;
using LiveAuction.Application.Helpers;
using LiveAuction.Application.Interfaces;
using LiveAuction.Core.Entites;
using LiveAuction.Infrastructure.Data;
using LiveAuction.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LiveAuction.Infrastructure.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'LocalDb' not found.");
            }

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                 .AddEntityFrameworkStores<AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString, b =>
                b.MigrationsAssembly(typeof(InfrastructureServiceCollectionExtensions).Assembly.FullName)
                ));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.Configure<JWT>(configuration.GetSection("JWT"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtKey = configuration["JWT:Key"];
                var jwtIssuer = configuration["JWT:Issuer"];
                var jwtAudience = configuration["JWT:Audience"];

                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new InvalidOperationException("JWT Key is missing from configuration.");
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            }); ;

            return services;
        }
    }
}
