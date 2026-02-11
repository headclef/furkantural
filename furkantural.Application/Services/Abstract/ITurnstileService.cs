using furkantural.Application.Wrappers;

namespace furkantural.Application.Services.Abstract
{
    public interface ITurnstileService
    {
        Task<Result> ValidateTokenAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default);
    }
}