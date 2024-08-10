using Domain.DTOs.Responces;
using MediatR;

namespace Application.CQRS.Email;

public sealed class SendEmailCommand : IRequest<Result<bool>>
{
    public string ToEmail { get; }
    public string Subject { get; }
    public string Content { get; }

    public SendEmailCommand(string toEmail, string subject, string content)
    {
        ToEmail = toEmail;
        Subject = subject;
        Content = content;
    }
}
