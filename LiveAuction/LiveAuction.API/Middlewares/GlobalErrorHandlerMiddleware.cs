using System.Net;
using System.Text.Json;
using LiveAuction.Application.Common;

namespace LiveAuction.api.Middlewares
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            string userFriendlyMessage = "An unexpected error occurred.";
            int statusCode = (int)HttpStatusCode.InternalServerError;


            switch (exception)
            {
                case KeyNotFoundException e:
                    statusCode = (int)HttpStatusCode.NotFound;
                    userFriendlyMessage = "The requested resource was not found.";
                    break;
                case UnauthorizedAccessException e:
                    statusCode = (int)HttpStatusCode.Forbidden;
                    userFriendlyMessage = "You do not have permission to perform this action.";
                    break;
                case ArgumentException e:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    userFriendlyMessage = e.Message;
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    userFriendlyMessage = "Internal Server Error. Please try again later.";
                    break;
            }

            context.Response.StatusCode = statusCode;
            var response = ApiResponse<string>.Failure(userFriendlyMessage);


            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, jsonOptions);

            await context.Response.WriteAsync(json);
        }
    }
}