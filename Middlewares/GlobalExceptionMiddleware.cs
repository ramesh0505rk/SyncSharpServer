using SyncSharpServer.Common.ExceptionHandling;
using SyncSharpServer.Common.Response;
using System.Net;

namespace SyncSharpServer.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (EnableBuffering(context))
            {
                context.Request.EnableBuffering();
            }

            try
            {
                await _next(context);
                await HandlePipelineErrorsAsync(context);
            }
            catch (Exception ex)
            {
                await LogAndWriteErrorAsync(context, ex);
            }
        }

        private bool EnableBuffering(HttpContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(context.Request.ContentType))
                    return false;

                if ((context.Request.Method == HttpMethods.Post ||
                    context.Request.Method == HttpMethods.Put ||
                    context.Request.Method == HttpMethods.Patch)
                    && (context.Request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
                    || context.Request.ContentType.Contains("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task HandlePipelineErrorsAsync(HttpContext context)
        {
            var statusMap = new Dictionary<int, HttpStatusCode>
            {
                { StatusCodes.Status401Unauthorized, HttpStatusCode.Unauthorized },
                { StatusCodes.Status403Forbidden, HttpStatusCode.Forbidden },
                { StatusCodes.Status500InternalServerError, HttpStatusCode.InternalServerError }
            };

            if (statusMap.TryGetValue(context.Response.StatusCode, out var statusCode))
            {
                _logger.LogError("----- {StatusCode} {StatusDescription} Response Returned by Pipeline -----\nPath: {Path}", (int)statusCode, statusCode, context.Request.Path);


            }
        }

        private async Task LogAndWriteErrorAsync(HttpContext context, Exception ex)
        {
            var (statusCode, customErrors) = ex switch
            {
                BadRequestException badRequestEx => (HttpStatusCode.BadRequest, badRequestEx.Errors),
                UnauthorizedException unauthorizeEx => (HttpStatusCode.Unauthorized, unauthorizeEx.Errors),
                BadHttpRequestException => (HttpStatusCode.BadRequest, null),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, null),
                _ => (HttpStatusCode.BadRequest, null)
            };
        }

        private async Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, List<string>? customMessages = null)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Cannot write error response because the response has already started.");
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorDetails = customMessages?.Count > 0 ? string.Join("; ", customMessages) : string.Empty;
            var errorResponse = new ApiErrorResponse
            {
                Errors = ApiErrorHandler.GetDefaultErrorMessage(statusCode, errorDetails)
            };


        }
    }
}
