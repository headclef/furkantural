using furkantural.Application.Services.Abstract;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
namespace furkantural.Infrastructure.Middleware;

public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment environment
)
{
    public async Task InvokeAsync(HttpContext context, ILogService logService)
    {
        try
        {
            await next(context);
        }
        catch (AntiforgeryValidationException)
        {
            await HandleExceptionAsync(context, logService,
                statusCode: (int)HttpStatusCode.BadRequest,
                type: "Bad Request",
                message: "Güvenlik doğrulaması başarısız oldu.",
                detail: "Form doğrulama hatası oluştu, lütfen tekrar deneyin.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
            await logService.Error($"Unhandled exception: {ex.Message}", ex.StackTrace);

            var detail = environment.IsDevelopment()
                ? ex.ToString()
                : "Beklenmeyen bir hata oluştu, lütfen daha sonra tekrar deneyin.";

            await HandleExceptionAsync(context, logService,
                statusCode: (int)HttpStatusCode.InternalServerError,
                type: "Internal Server Error",
                message: "Sunucuda bir hata oluştu.",
                detail: detail);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        ILogService logService,
        int statusCode,
        string type,
        string message,
        string detail)
    {
        if (context.Response.HasStarted) return;

        context.Response.Clear();

        var errorPath = $"/Base/Error?Code={statusCode}" +
                        $"&Type={Uri.EscapeDataString(type)}" +
                        $"&Message={Uri.EscapeDataString(message)}" +
                        $"&Detail={Uri.EscapeDataString(detail)}";

        context.Response.Redirect(errorPath);
        await Task.CompletedTask;
    }
}