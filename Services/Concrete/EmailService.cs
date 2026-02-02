using furkantural.Models;
using furkantural.Services.Abstract;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using furkantural.Wrappers;

namespace furkantural.Services.Concrete
{
    public class EmailService (
        IOptions<SmtpViewModel> smtpOptions,
        IOptions<AllowerViewModel> allowerOptions,
        IEmailRateLimiter rateLimiter,
        ILogService logService,
        IWebHostEnvironment env
    ) : IEmailService
    {
        #region Fields
        private readonly SmtpViewModel _smtpOptions = smtpOptions.Value;
        private readonly AllowerViewModel _alloweroptions = allowerOptions.Value;
        private readonly IEmailRateLimiter _rateLimiter = rateLimiter;
        private readonly ILogService _logService = logService;
        private readonly IWebHostEnvironment _env = env;
        #endregion

        #region Methods
        public async Task<Result> SendTransactionalEmailAsync(string toEmail, EmailType emailType, Dictionary<string, string> placeholders)
        {
            if (!_alloweroptions.Smtp)
            {
                await _logService.Log(LogLevel.Error, "Email service is disabled.");
                return Result.Fail("Email service is disabled.");
            }
            else if (string.IsNullOrWhiteSpace(toEmail))
            {
                await _logService.Log(LogLevel.Error, $"User email is not set or not correct: {toEmail}");
                return Result.Fail("Invalid email address.");
            }

            try
            {
                // Html body that's going to be used in mail.
                string body = await GetHtmlBodyAsync(emailType, placeholders);
                MimeMessage message = new();
                BodyBuilder builder = new() { HtmlBody = body };

                // Setting the mail properties.
                message.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = emailType == EmailType.Contact ? "Mesajın bana ulaştı 🚀" : "Yeni iletişim formu gönderildi 📩";
                message.Body = builder.ToMessageBody();

                // Can email be sent?
                if (await _rateLimiter.CanSendExternalEmail())
                {
                    // Smtp client for sending out the email.
                    using SmtpClient smtpClient = new();

                    // Connect to smtp server.
                    await smtpClient.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, _smtpOptions.EnableSsl);

                    // Authenticate the server.
                    await smtpClient.AuthenticateAsync(_smtpOptions.SenderEmail, _smtpOptions.Password);

                    // Send the message.
                    await smtpClient.SendAsync(message);

                    // Disconnect from the server.
                    await smtpClient.DisconnectAsync(true);
                }
                else
                {
                    await _logService.Log(LogLevel.Warning, $"Rate limit exceeded for email: {toEmail}");
                    return Result.Fail("Rate limit exceeded. Please try again later.");
                }

                // Log the success.
                await _logService.Log(LogLevel.Success, $"An email has been sent to: {toEmail}");
                return Result.Ok($"Email sent to {toEmail}");
            }
            catch (Exception exception)
            {
                await _logService.Log(LogLevel.Error, $"An error occured while executing emailing: {exception.Message}");
                return Result.Fail($"Internal error: {exception.Message}");
            }
        }

        private async Task<string> GetHtmlBodyAsync(EmailType emailType, Dictionary<string, string> placeholders)
        {
            // The template that's going to be used for sending. Changes based on which email type it is.
            var path = Path.Combine(_env.WebRootPath, "templates", $"{emailType.ToString().ToLower()}.html");
            string template = await File.ReadAllTextAsync(path);

            // Fill and replace each detail with their respective values that's been set before this progress.
            foreach (var placeholder in placeholders)
                template = template.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);

            return template;
        }
        #endregion
    }

    #region Helpers
    public enum EmailType
    {
        Contact = 0,
        Listener = 1
    }
    #endregion
}