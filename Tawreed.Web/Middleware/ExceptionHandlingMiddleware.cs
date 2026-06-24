using System.Net;
using System.Text.Json;
using Tawreed.Application.Common.Exceptions;

namespace Tawreed.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        object response = new { message = "An internal server error occurred." };

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                response = new { message = exception.Message };
                break;
            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                response = new { message = "Validation failed.", errors = validationEx.Errors };
                break;
            case FluentValidation.ValidationException:
                statusCode = HttpStatusCode.BadRequest;
                response = new { message = exception.Message };
                break;
        }

        _logger.LogError(exception, "Request failed: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
