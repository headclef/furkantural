using furkantural.Models;
using furkantural.Services.Abstract;
using furkantural.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MimeKit;
using System.Net.Mail;

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
            if (!ModelState.IsValid) { return await Task.FromResult(BadRequest("Formda oynamalar yapmayalým, bu senin iyiliðin için.")); }

            // Turnstile validation
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var isValidTurnstile = await _turnstileService.ValidateTokenAsync(viewModel.TurnstileResponse, ipAddress);
            if (!isValidTurnstile)
            {
                ModelState.AddModelError(string.Empty, "Güvenlik doðrulamasý baþarýsýz oldu. Lütfen tekrar deneyiniz.");
                return await Task.FromResult(BadRequest("Güvenlik doðrulamasýný tamamlamak zorundasýn..."));
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

            // 1) Send to listener
            await _emailService.SendTransactionalEmailAsync(_smtpOptions.ListenerEmail, EmailType.Listener, placeholders);

            // 2) Send to user
            await _emailService.SendTransactionalEmailAsync(viewModel.Email, EmailType.Contact, placeholders);

            // Return ok if success
            return await Task.FromResult(Ok("Tamamdýr, iletin bana ulaþtý, en kýsa sürede sana döneceðim!"));
        }

        [HttpGet]
        public async Task<IActionResult> Error(ErrorViewModel viewModel)
        {
            return await Task.FromResult(View(viewModel));
        }
        #endregion

        #region Helpers
        private async Task<string> GetClientIp(HttpContext context)
        {
            // 1) Cloudflare özel baþlýðý var ise; (CF-Connection-IP)
            if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp) && !string.IsNullOrWhiteSpace(cfIp))
                return cfIp.ToString();

            // 2) Standart proxy/header: X-Forwarded-For (yani birden fazla geçiþli de olabilir -> ilk gerçek istemci hangisiyse artýk)
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var xff) && !StringValues.IsNullOrEmpty(xff))
            {
                var first = xff.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).FirstOrDefault();
                if (!string.IsNullOrEmpty(first))
                    return first;
            }

            // 3) Fallback: doðrudan baðlantý IP'si
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