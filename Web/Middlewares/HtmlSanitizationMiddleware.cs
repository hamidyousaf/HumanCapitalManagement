using Ganss.Xss;
using System.Text;

namespace Web.Middlewares;

internal sealed class HtmlSanitizationMiddleware : IMiddleware
{
    private readonly HtmlSanitizer _sanitizer;
    public HtmlSanitizationMiddleware()
    {
        _sanitizer = new HtmlSanitizer();
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Check if the request has a body
        if (context.Request.ContentLength > 0 &&
            context.Request.ContentType != null &&
            context.Request.ContentType.Contains("application/json"))
        {
            context.Request.EnableBuffering();

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                // Sanitize the entire request body
                var sanitizedBody = _sanitizer.Sanitize(body);

                // Replace the original request body with the sanitized one
                var sanitizedBodyBytes = Encoding.UTF8.GetBytes(sanitizedBody);
                context.Request.Body = new MemoryStream(sanitizedBodyBytes);
            }
        }

        await next(context);
    }
}
