using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SyncSharpServer.Common.ExceptionHandling;
using SyncSharpServer.Common.Response;
using System.Net;

namespace SyncSharpServer.Filters
{
    public class ApiResponseWrapperFilter : IActionFilter
    {
        private readonly ILogger<ApiResponseWrapperFilter> _logger;
        public ApiResponseWrapperFilter(ILogger<ApiResponseWrapperFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .SelectMany(e => e.Value.Errors.Select(er =>
                    {
                        var key = e.Key.StartsWith("$.") ? e.Key[2..] : e.Key;
                        var msg = CleanErrorMessage(er.ErrorMessage);

                        return $"{key}: {msg}";
                    }))
                    .Distinct()
                    .ToList();

                // Build custom response
                var errorDetails = errors?.Count > 0 ? string.Join(", ", errors) : string.Empty;
                var errorResponse = new ApiErrorResponse
                {
                    Errors = ApiErrorHandler.GetDefaultErrorMessage(HttpStatusCode.BadRequest, errorDetails)
                };

                // Log errors
                _logger.LogError("----- System generated 400 BadRequest Model Validation Error -----\n" +
                                "Method: {Method}\n" +
                                "Path: {Path}\n" +
                                "Validation Errors: {@Errors}\n" +
                                "---------------------------------",
                                context.HttpContext.Request.Method, context.HttpContext.Request.Path, errorResponse
                                );

                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        private string CleanErrorMessage(string rawMessages)
        {
            if (string.IsNullOrWhiteSpace(rawMessages))
                return string.Empty;

            var keywordsToTrim = new string[] { "Path:", "LineNumber:", "BytePositionInLine:" };

            foreach (var keyword in keywordsToTrim)
            {
                var index = rawMessages.IndexOf(keyword);
                if (index >= 0)
                {
                    rawMessages = rawMessages[..index];
                    break;
                }
            }
            return rawMessages.Trim().TrimEnd('.') + ".";
        }
    }
}
