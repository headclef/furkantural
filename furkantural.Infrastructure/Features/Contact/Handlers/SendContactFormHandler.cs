using furkantural.Application.Features.Contact.Commands;
using furkantural.Application.Models;
using furkantural.Application.Services.Abstract;
using furkantural.Application.Wrappers;
using furkantural.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;
namespace furkantural.Infrastructure.Features.Contact.Handlers;

public class SendContactFormHandler(
    IOptions<SmtpOptions> smtpOptions,
    IEmailService emailService,
    ITurnstileService turnstileService,
    ILogService logService,
    IDateTimeProvider dateTime
) : IRequestHandler<SendContactFormCommand, Result>
{
    private readonly SmtpOptions _smtpOptions = smtpOptions.Value;

    public async Task<Result> Handle(SendContactFormCommand request, CancellationToken cancellationToken)
    {
        // Turnstile validation
        var turnstileResult = await turnstileService.ValidateTokenAsync(
            request.TurnstileResponse, request.IpAddress, cancellationToken);

        if (!turnstileResult.Success)
        {
            var msg = turnstileResult.Errors.FirstOrDefault() ?? "Güvenlik doğrulaması başarısız.";
            return Result.Fail(msg);
        }

        // Build placeholders
        var placeholders = new Dictionary<string, string>
        {
            { "NameSurname", request.NameSurname },
            { "Email", request.Email },
            { "MessageNeed", request.MessageNeed },
            { "SendTime", dateTime.Now.ToString("yyyy.MM.dd HH:mm") },
            { "IpAddress", request.IpAddress ?? "Unknown" }
        };

        await logService.Info($"Visitor {request.Email} is attempting to send a contact form.");

        // Send to admin
        var adminResult = await emailService.SendTransactionalEmailAsync(
            _smtpOptions.ListenerEmail, EmailType.Listener, placeholders);

        // Send to user
        var userResult = await emailService.SendTransactionalEmailAsync(
            request.Email, EmailType.Contact, placeholders);

        if (!adminResult.Success && !userResult.Success)
        {
            var errorMsg = userResult.Errors.FirstOrDefault()
                ?? adminResult.Errors.FirstOrDefault()
                ?? "E-posta servisi şu anda çalışmıyor.";

            await logService.Error($"Contact form submission failed for {request.Email}.");
            return Result.Fail(errorMsg);
        }

        await logService.Success($"Contact form submitted successfully for {request.Email}.");
        return Result.Ok("Tamamdır, iletin bana ulaştı, en kısa sürede sana döneceğim!");
    }
}