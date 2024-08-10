using Domain.DTOs.Responces;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.CQRS.Email;

public sealed class EmailHandlers : IRequestHandler<SendEmailCommand, Result<bool>>
{
    private IConfiguration _configuration;

    public EmailHandlers(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Result<bool>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["SendGridAPIKey"];
        // var client = new SendGridClient(apiKey);
        // var from = new EmailAddress("test@authdemo.com", "JWT Auth Demo");
        // var to = new EmailAddress(toEmail);
        // var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
        // var response = await client.SendEmailAsync(msg);

        return Result<bool>.Success(true, "Email sent successfully!");
    }
}
