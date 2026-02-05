using furkantural.Models;
using furkantural.Services.Abstract;
using furkantural.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MimeKit;
using System.Net.Mail;
using Microsoft.AspNetCore.Localization;

namespace furkantural.Controllers
{
    [AllowAnonymous]
    public class BaseController (
        IOptions<SmtpViewModel> smtpOptions,
        ILogService logService,
        IEmailService emailService,
        ITurnstileService turnstileService
    ): Controller
    {
        #region Properties
        private readonly SmtpViewModel _smtpOptions = smtpOptions.Value;
        private readonly ILogService _logService = logService;
        private readonly IEmailService _emailService = emailService;
        private readonly ITurnstileService _turnstileService = turnstileService;
        #endregion

        #region Methods
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMail(MailViewModel viewModel)
        {
            // Pre-validate
            if (!ModelState.IsValid) { return await Task.FromResult(BadRequest("Formda oynamalar yapmayalım, bu senin iyiliğin için.")); }

            // Turnstile validation
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var turnstileResult = await _turnstileService.ValidateTokenAsync(viewModel.TurnstileResponse, ipAddress);
            if (!turnstileResult.Success)
            {
                // Hata mesajını servisten al
                var msg = turnstileResult.Errors.FirstOrDefault() ?? "Güvenlik doğrulaması başarısız.";
                ModelState.AddModelError(string.Empty, msg);
                return await Task.FromResult(BadRequest(msg));
            }

            // Ready the placeholders before sending out.
            var placeholders = new Dictionary<string, string>()
            {
                { "NameSurname", viewModel.NameSurname },
                { "Email", viewModel.Email },
                { "MessageNeed", viewModel.MessageNeed },
                { "SendTime", DateTime.Now.ToString("yyyy.MM.dd HH:mm") },
                { "IpAddress", await GetClientIp(HttpContext) }
            };

            // Log the attempt
            await _logService.Info($"Visitor {viewModel.Email} is attempting to send a contact form.");

            // 1) Send to listener (Admin)
            var adminMailResult = await _emailService.SendTransactionalEmailAsync(_smtpOptions.ListenerEmail, EmailType.Listener, placeholders);

            // 2) Send to user (Contact)
            var userMailResult = await _emailService.SendTransactionalEmailAsync(viewModel.Email, EmailType.Contact, placeholders);

            if (!adminMailResult.Success && !userMailResult.Success)
            {
                 // Eğer ikisi de başarısızsa kullanıcıya hata dön
                 // Kullanıcıya giden mailin hatasını öncelikli göster
                 var errorMsg = userMailResult.Errors.FirstOrDefault() ?? adminMailResult.Errors.FirstOrDefault() ?? "E-posta servisi şu anda çalışmıyor.";
                 
                 await _logService.Error($"Contact form submission failed for {viewModel.Email}.");
                 return await Task.FromResult(BadRequest(errorMsg));
            }

            // Return ok if success (at least one sent)
            await _logService.Success($"Contact form submitted successfully for {viewModel.Email}.");
            return await Task.FromResult(Ok("Tamamdır, iletin bana ulaştı, en kısa sürede sana döneceğim!"));
        }

        [HttpGet]
        public async Task<IActionResult> Error(ErrorViewModel viewModel)
        {
            return await Task.FromResult(View(viewModel));
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
        #endregion

        #region Helpers
        private async Task<string> GetClientIp(HttpContext context)
        {
            // 1) Cloudflare özel bağlı var ise; (CF-Connection-IP)
            if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp) && !string.IsNullOrWhiteSpace(cfIp))
                return cfIp.ToString();

            // 2) Standart proxy/header: X-Forwarded-For (yani birden fazla geçişli de olabilir -> ilk gerçek istemci hangisiyse artık)
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var xff) && !StringValues.IsNullOrEmpty(xff))
            {
                var first = xff.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).FirstOrDefault();
                if (!string.IsNullOrEmpty(first))
                    return first;
            }

            // 3) Fallback: doğrudan bağlantı IP'si
            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp is not null)
            {
                // IPv4 biçiminde almak istersen MapToIPv4 kullan
                try
                {
                    return remoteIp.MapToIPv4().ToString();
                }
                catch
                {
                    return remoteIp.ToString();
                }
            }

            return "Unknown";
        }
        #endregion
    }
}
