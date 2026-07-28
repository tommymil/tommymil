using System.Net;
using System.Net.Mail;
using LessonRunner.Application.Notifications;
using Microsoft.Extensions.Options;

namespace LessonRunner.Infrastructure.Notifications;

internal sealed class SmtpEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var smtp = options.Value;

        using var client = new SmtpClient(smtp.Host, smtp.Port)
        {
            EnableSsl = smtp.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(smtp.Username))
        {
            client.Credentials = new NetworkCredential(smtp.Username, smtp.Password);
        }

        using var mail = new MailMessage
        {
            From = new MailAddress(message.FromEmail, message.FromName),
            Subject = message.Subject,
            Body = message.Body
        };
        mail.To.Add(message.ToEmail);

        using var registration = cancellationToken.Register(client.SendAsyncCancel);
        await client.SendMailAsync(mail, cancellationToken);
    }
}
