using furkantural.Application.Wrappers;
using MediatR;
namespace furkantural.Application.Features.Contact.Commands;

public class SendContactFormCommand : IRequest<Result>
{
    public string Email { get; set; } = string.Empty;
    public string NameSurname { get; set; } = string.Empty;
    public string MessageNeed { get; set; } = string.Empty;
    public string TurnstileResponse { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}