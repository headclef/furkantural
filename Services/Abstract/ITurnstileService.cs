using furkantural.Wrappers;

namespace furkantural.Services.Abstract
{
    public interface ITurnstileService
    {
        #region Signatures
        Task<Result> ValidateTokenAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default);
        #endregion
    }
}