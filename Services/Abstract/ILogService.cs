namespace furkantural.Services.Abstract;

public interface ILogService
{
    Task LogAsync(string level, string message, string? detail = null);
    Task Info(string message, string? detail = null);
    Task Error(string message, string? detail = null);
    Task Success(string message, string? detail = null);
    Task Warning(string message, string? detail = null);
}