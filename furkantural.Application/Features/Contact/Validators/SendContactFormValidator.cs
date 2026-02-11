using FluentValidation;
using furkantural.Application.Features.Contact.Commands;
namespace furkantural.Application.Features.Contact.Validators;

public class SendContactFormValidator : AbstractValidator<SendContactFormCommand>
{
    public SendContactFormValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresinizi girmek zorundasınız.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.NameSurname)
            .NotEmpty().WithMessage("Ad Soyad girmek zorundasınız.")
            .MaximumLength(100).WithMessage("Ad Soyad en fazla 100 karakter olabilir.");

        RuleFor(x => x.MessageNeed)
            .NotEmpty().WithMessage("Mesaj / İhtiyaç girmek zorundasınız.")
            .MaximumLength(2000).WithMessage("Mesaj en fazla 2000 karakter olabilir.");

        RuleFor(x => x.TurnstileResponse)
            .NotEmpty().WithMessage("Güvenlik doğrulaması gerekli.");
    }
}