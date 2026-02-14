using System.Threading.RateLimiting;
using LiveAuction.Application.Common;

namespace LiveAuction.api.Extensions
{
    public static class RateLimiterExtension
    {
        public static IServiceCollection AddGlobalRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    var response = ApiResponse<object>.Failure("Error : Rate Limiter", new List<string> { "Reached Requests Limit, Please Wait" });
                    await context.HttpContext.Response.WriteAsJsonAsync(response, token);
                };

                options.AddPolicy("IpLimiter", httpContext =>
                {
                    var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
                    var partitionKey = !string.IsNullOrEmpty(remoteIpAddress) ? remoteIpAddress : "LocalHostUser";

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey: partitionKey, factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromSeconds(60),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });

                options.AddPolicy("AuthLimiter", httpContext =>
                {
                    var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
                    var partitionKey = !string.IsNullOrEmpty(remoteIpAddress) ? remoteIpAddress : "LocalHostUser";

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey: partitionKey, factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromSeconds(30),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
            });

            return services;
        }
    }
}
