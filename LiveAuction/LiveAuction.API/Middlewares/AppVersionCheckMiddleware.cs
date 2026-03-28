using LiveAuction.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace LiveAuction.API.Middlewares
{
    public class AppVersionCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private const string AppVersionHeader = "X-App-Version";

        private static readonly PathString[] _excludedPaths = new[]
        {
            new PathString("/scalar"),
            new PathString("/openapi"),
            new PathString("/swagger"),
            new PathString("/liveauction/appstatus")
        };

        public AppVersionCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAppStatusService appStatusService)
        {
            if (IsExcludedPath(context.Request.Path))
            {
                await _next(context);
                return;
            }

            string appVersionStr = "1.0.0"; 

            if (context.Request.Headers.TryGetValue(AppVersionHeader, out var appVersionValues))
            {
                var headerValue = appVersionValues.ToString();
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    appVersionStr = headerValue;
                }
            }
            var statusResponse = await appStatusService.CheckStatusAsync(appVersionStr,null!);

            if (statusResponse.Data != null)
            {
                if (statusResponse.Data.IsMaintenance)
                {
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(statusResponse);
                    return;
                }

                if (statusResponse.Data.IsBanned)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(statusResponse);
                    return;
                }

      
                if (statusResponse.Data.UpdateRequired)
                {
                    context.Response.StatusCode = StatusCodes.Status426UpgradeRequired;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(statusResponse);
                    return;
                }
            }
            else if (!statusResponse.Succeeded)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(statusResponse);
                return;
            }

         
            await _next(context);
        }

        private static bool IsExcludedPath(PathString path)
        {
            foreach (var excludedPath in _excludedPaths)
            {
               
                if (path.StartsWithSegments(excludedPath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}