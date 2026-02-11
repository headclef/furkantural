namespace furkantural.Application.Services.Abstract
{
    public interface IEmailRateLimiter
    {
        Task<bool> CanSendExternalEmail();
    }
}