using Application.Abstractions.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Infrastructure.Services;

public class EmailService(IFluentEmail fluentEmail): IEmailService
{
    public async Task SendAsync(string to, string subject, string body, bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        IFluentEmail? email = fluentEmail
            .To(to)
            .Subject(subject)
            .Body(body, isHtml);
        
        SendResponse? response = await email.SendAsync(cancellationToken);

        if (!response.Successful)
        {
            throw new InvalidOperationException($"Email sending failed: {string.Join(", ", response.ErrorMessages)}");
        }
    }
}
