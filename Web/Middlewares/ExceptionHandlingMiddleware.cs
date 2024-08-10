using Domain.DTOs.Responces;
using System.Net;
using System.Text.Json;

namespace Web.Middlewares;

internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly IHostEnvironment _env;
    public ILogger<ExceptionHandlingMiddleware> _logger { get; }
    private ExceptionHandlingMiddleware() { }
    public ExceptionHandlingMiddleware(IHostEnvironment env, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, exception.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = _env.IsDevelopment()
            ? new ApiException(context.Response.StatusCode, exception.Message, exception.StackTrace?.ToString())
            : new ApiException(context.Response.StatusCode, exception.Message, "Internal server error");

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}