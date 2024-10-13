using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using MurabaDemo.Models.Configuration;
namespace MurabaDemo.Middlewares;
public class SwaggerAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SwaggerConfig settings;

    public SwaggerAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        this.settings = configuration.GetSection("Swagger").Get<SwaggerConfig>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (settings == null || settings.Authentication == false)
        {
            await _next(context);
            return;
        }

        string? authHeader = context.Request.Headers["Authorization"];

        if (authHeader != null && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var header = AuthenticationHeaderValue.Parse(authHeader);
                var inBytes = Convert.FromBase64String(header.Parameter!);
                var credentials = Encoding.UTF8.GetString(inBytes).Split(':');

                if (string.Equals(credentials.FirstOrDefault(), settings.Username, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(credentials.LastOrDefault(), settings.Password, StringComparison.OrdinalIgnoreCase))
                {
                    await _next(context);
                    return;
                }
            }
            catch (FormatException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Invalid Authorization header format");
                return;
            }
        }

        context.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        await context.Response.WriteAsync("Unauthorized access.");
    }
}