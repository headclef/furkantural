using furkantural.Application.Models;
using furkantural.Application.Services.Abstract;
using furkantural.Domain.Enums;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using furkantural.Application.Wrappers;
using Microsoft.AspNetCore.Hosting;

namespace furkantural.Infrastructure.Services.Concrete
{
    public class EmailService (
        IOptions<SmtpOptions> smtpOptions,
        IOptions<AllowerOptions> allowerOptions,
        IEmailRateLimiter rateLimiter,
        ILogService logService,
        IWebHostEnvironment env
    ) : IEmailService
    {
        private readonly SmtpOptions _smtpOptions = smtpOptions.Value;
        private readonly AllowerOptions _alloweroptions = allowerOptions.Value;
        private readonly IEmailRateLimiter _rateLimiter = rateLimiter;
        private readonly ILogService _logService = logService;
        private readonly IWebHostEnvironment _env = env;

        public async Task<Result> SendTransactionalEmailAsync(string toEmail, EmailType emailType, Dictionary<string, string> placeholders)
        {
            if (!_alloweroptions.Smtp)
            {
                await _logService.Error("Email service is disabled.");
                return Result.Fail("Email service is disabled.");
            }
            else if (string.IsNullOrWhiteSpace(toEmail))
            {
                await _logService.Error($"User email is not set or not correct: {toEmail}");
                return Result.Fail("Invalid email address.");
            }

            try
            {
                string body = await GetHtmlBodyAsync(emailType, placeholders);
                MimeMessage message = new();
                BodyBuilder builder = new() { HtmlBody = body };

                message.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = emailType == EmailType.Contact ? "Mesajın bana ulaştı 🚀" : "Yeni iletişim formu gönderildi 📩";
                message.Body = builder.ToMessageBody();

                if (await _rateLimiter.CanSendExternalEmail())
                {
                    using SmtpClient smtpClient = new();
                    await smtpClient.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, _smtpOptions.EnableSsl);
                    await smtpClient.AuthenticateAsync(_smtpOptions.SenderEmail, _smtpOptions.Password);
                    await smtpClient.SendAsync(message);
                    await smtpClient.DisconnectAsync(true);
                }
                else
                {
                    await _logService.Warning($"Rate limit exceeded for email: {toEmail}");
                    return Result.Fail("Rate limit exceeded. Please try again later.");
                }

                await _logService.Success($"An email has been sent to: {toEmail}");
                return Result.Ok($"Email sent to {toEmail}");
            }
            catch (Exception exception)
            {
                await _logService.Error($"An error occured while executing emailing: {exception.Message}");
                return Result.Fail($"Internal error: {exception.Message}");
            }
        }

        private async Task<string> GetHtmlBodyAsync(EmailType emailType, Dictionary<string, string> placeholders)
        {
            var path = Path.Combine(_env.WebRootPath, "templates", $"{emailType.ToString().ToLower()}.html");
            string template = await File.ReadAllTextAsync(path);

            foreach (var placeholder in placeholders)
                template = template.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);

            return template;
        }
    }
}