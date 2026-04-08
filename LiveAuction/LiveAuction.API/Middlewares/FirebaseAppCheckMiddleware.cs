using LiveAuction.Application.Common;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Hosting;

namespace LiveAuction.api.Middlewares
{
    public class FirebaseAppCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<FirebaseAppCheckMiddleware> _logger;
        private readonly IWebHostEnvironment _env; 

        private static IList<SecurityKey>? _signingKeys;
        private static DateTime _keysLastFetched = DateTime.MinValue;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private const string ProjectNumber = "1032003656948";
        private const string JwksUrl = "https://firebaseappcheck.googleapis.com/v1/jwks";

        public FirebaseAppCheckMiddleware(RequestDelegate next, ILogger<FirebaseAppCheckMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_env.IsDevelopment())
            {
                await _next(context);
                return;
            }

            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/scalar") ||
                context.Request.Path.StartsWithSegments("/openapi") ||
                context.Request.Path.StartsWithSegments("/liveauction/appstatus"))
            {
                await _next(context);
                return;
            }


            if (!context.Request.Headers.TryGetValue("X-Firebase-AppCheck", out var appCheckToken))
            {
                _logger.LogWarning("Blocked request: Missing X-Firebase-AppCheck token.");
                await ReturnUnauthorizedResponse(context, "Unauthorized: App Check token is missing.");
                return;
            }

            try
            {
                var keys = await GetSigningKeysAsync();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = keys,
                    ValidateIssuer = true,
                    ValidIssuer = $"https://firebaseappcheck.googleapis.com/{ProjectNumber}",
                    ValidateAudience = true,
                    ValidAudience = $"projects/{ProjectNumber}",
                    ValidateLifetime = true
                };

                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(appCheckToken.ToString(), validationParameters, out _);

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blocked request: Invalid App Check token.");
                await ReturnUnauthorizedResponse(context, "Unauthorized: Invalid App Check token.");
            }
        }

        private async Task<IList<SecurityKey>> GetSigningKeysAsync()
        {
            if (_signingKeys != null && (DateTime.UtcNow - _keysLastFetched).TotalHours < 24)
            {
                return _signingKeys;
            }

            await _semaphore.WaitAsync();
            try
            {
                if (_signingKeys != null && (DateTime.UtcNow - _keysLastFetched).TotalHours < 24)
                {
                    return _signingKeys;
                }

                using var httpClient = new HttpClient();
                var jwksResponse = await httpClient.GetStringAsync(JwksUrl);
                var jwks = new JsonWebKeySet(jwksResponse);

                _signingKeys = jwks.GetSigningKeys();
                _keysLastFetched = DateTime.UtcNow;
            }
            finally
            {
                _semaphore.Release();
            }

            return _signingKeys;
        }

        private static async Task ReturnUnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Failure(message);
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(json);
        }
    }
}