using furkantural.Models;
using furkantural.Application.Features.Contact.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;

namespace furkantural.Controllers
{
    [AllowAnonymous]
    public class BaseController(
        IMediator mediator
    ) : Controller
    {
        #region Properties
        private readonly IMediator _mediator = mediator;
        #endregion

        #region Methods
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMail(ContactFormRequest viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Formda oynamalar yapmayalım, bu senin iyiliğin için.");

            var command = new SendContactFormCommand
            {
                Email = viewModel.Email,
                NameSurname = viewModel.NameSurname,
                MessageNeed = viewModel.MessageNeed,
                TurnstileResponse = viewModel.TurnstileResponse,
                IpAddress = GetClientIp(HttpContext)
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Errors.FirstOrDefault() ?? "Bir hata oluştu.");

            return Ok(result.Message);
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
        private string GetClientIp(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp) && !string.IsNullOrWhiteSpace(cfIp))
                return cfIp.ToString();

            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var xff) && !StringValues.IsNullOrEmpty(xff))
            {
                var first = xff.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).FirstOrDefault();
                if (!string.IsNullOrEmpty(first))
                    return first;
            }

            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp is not null)
            {
                try { return remoteIp.MapToIPv4().ToString(); }
                catch { return remoteIp.ToString(); }
            }

            return "Unknown";
        }
        #endregion
    }
}