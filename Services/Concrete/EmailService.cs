using furkantural.Models;
using furkantural.Services.Abstract;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace furkantural.Services.Concrete
{
    public class EmailService (
        IOptions<SmtpViewModel> smtpOptions,
        IOptions<AllowerViewModel> allowerOptions,
        IEmailRateLimiter rateLimiter,
        ILogService logService
    ) : IEmailService
    {
        #region Fields
        private readonly SmtpViewModel _smtpOptions = smtpOptions.Value;
        private readonly AllowerViewModel _alloweroptions = allowerOptions.Value;
        private readonly IEmailRateLimiter _rateLimiter = rateLimiter;
        private readonly ILogService _logService = logService;
        #endregion

        #region Methods
        public async Task SendTransactionalEmailAsync(string toEmail, EmailType emailType, Dictionary<string, string> placeholders)
        {
            if (!_alloweroptions.Smtp)
            {
                await _logService.Log(LogLevel.Error, "Email service is disabled.");
                return;
            }
            else if (string.IsNullOrWhiteSpace(toEmail))
            {
                await _logService.Log(LogLevel.Error, $"User email is not set or not correct: {toEmail}");
                return;
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

                // Log the success.
                await _logService.Log(LogLevel.Success, $"An email has been sent to: {toEmail}");
            }
            catch (Exception exception)
            {
                await _logService.Log(LogLevel.Error, $"An error occured while executing emailing: {exception.Message}");
                throw;
            }
        }

        private async Task<string> GetHtmlBodyAsync(EmailType emailType, Dictionary<string, string> placeholders)
        {
            // The template that's going to be used for sending. Changes based on which email type it is.
            string template = File.ReadAllText($"wwwroot/templates/{emailType.ToString().ToLower()}.html");

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