namespace furkantural.Services.Abstract
{
    public interface ITurnstileService
    {
        #region Signatures
        Task<bool> ValidateTokenAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default);
        #endregion
    }
}