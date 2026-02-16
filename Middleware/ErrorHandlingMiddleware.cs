using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using VH_2ND_TASK.Middleware.Exceptions;

namespace VH_2ND_TASK.Middleware;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception happened");

            var statusCode = ex switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };  

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                message = statusCode == HttpStatusCode.InternalServerError
                    ? "Something went wrong"
                    : ex.Message,
                exceptionType = ex.GetType().Name,
                statusCode = context.Response.StatusCode,
                traceId = context.TraceIdentifier,
                detail = _env.IsDevelopment() ? ex.Message : null
               
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}