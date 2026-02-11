using furkantural.Domain.Enums;
using furkantural.Application.Wrappers;

namespace furkantural.Application.Services.Abstract
{
    public interface IEmailService
    {
        Task<Result> SendTransactionalEmailAsync(string toEmail, EmailType emailType, Dictionary<string, string> placeholders);
    }
}