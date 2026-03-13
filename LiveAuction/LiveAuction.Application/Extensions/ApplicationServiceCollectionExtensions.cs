using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FluentValidation;
using LiveAuction.Application.Helpers;
using LiveAuction.Application.Interfaces;
using LiveAuction.Application.Interfaces.UserProfileInterfaces;
using LiveAuction.Application.Profiles;
using LiveAuction.Application.Services;
using LiveAuction.Application.Services.UserProfileServices;
using Microsoft.Extensions.DependencyInjection;

namespace LiveAuction.Application.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAppStatusService, AppStatusService>();
            services.AddScoped<IUserProfileService,UserProfileService>();
            services.AddScoped<JWT>();
            services.AddScoped<TokenHelper>();
            services.AddAutoMapper(cfg => cfg.AddProfile<AuthProfile>());
            services.AddMemoryCache();

            services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);

        }
    }
}
