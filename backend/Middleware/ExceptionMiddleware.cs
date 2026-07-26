using System.Net;
using System.Text.Json;

namespace FitnessTracker.Middleware
{
    public class ExceptionMiddleware
    {
        // Controller actions go through ASP.NET Core's own JSON formatter,
        // which defaults to camelCase - this middleware serializes manually
        // via JsonSerializer.Serialize, which defaults to PascalCase unless
        // told otherwise. Without this, error bodies came back as
        // {"Message": ...} while every frontend catch block reads
        // err.response.data.message (lowercase), so the real error text
        // never reached the UI - only the hardcoded fallback strings did.
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception ex) { 
                await HandleExceptionAsync(context, ex);
            }

        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";

            // Map exceptions → HTTP status codes
            switch (ex)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized access.";
                    break;

                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;

                default:
                    break;
            }

            // Log the error
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorResponse
            {
                Message = message,
                StatusCode = context.Response.StatusCode,
                TraceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(response, JsonOptions);

            await context.Response.WriteAsync(json);
        }
    }

}
