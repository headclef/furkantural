using furkantural.Infrastructure.Data;
using furkantural.Domain.Entities;
using furkantural.Application.Services.Abstract;
using Microsoft.AspNetCore.Http;

namespace furkantural.Infrastructure.Services.Concrete;

public class LogService(AppDbContext context, IHttpContextAccessor httpContextAccessor) : ILogService
{
    private const string ProjectName = "FurkanTural";

    public async Task LogAsync(string level, string message, string? detail = null)
    {
        try
        {
            var log = new Log
            {
                Project = ProjectName,
                Level = level,
                Message = message,
                Detail = detail,
                Date = DateTime.Now,
                IpAddress = GetIpAddress(),
                Path = GetPath()
            };

            context.Logs.Add(log);
            await context.SaveChangesAsync();
        }
        catch
        {
            // Logging should not break the application flow if it fails
        }
    }

    public Task Info(string message, string? detail = null) => LogAsync("Info", message, detail);
    public Task Error(string message, string? detail = null) => LogAsync("Error", message, detail);
    public Task Success(string message, string? detail = null) => LogAsync("Success", message, detail);
    public Task Warning(string message, string? detail = null) => LogAsync("Warning", message, detail);

    private string? GetIpAddress()
    {
        var httpContext = httpContextAccessor.HttpContext;
        return httpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    private string? GetPath()
    {
        var httpContext = httpContextAccessor.HttpContext;
        return httpContext?.Request?.Path;
    }
}