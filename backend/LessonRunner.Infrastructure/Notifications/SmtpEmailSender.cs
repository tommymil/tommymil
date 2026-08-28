using System.Net;
using System.Net.Mail;
using LessonRunner.Application.Notifications;
using LessonRunner.Domain.Notifications;
using Microsoft.Extensions.Options;

namespace LessonRunner.Infrastructure.Notifications;

internal sealed class SmtpEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var smtp = options.Value;

        // Adres z domeny nieistniejącej w DNS odbija się o dostawcę poczty, a rodzic nie
        // dostaje nic. Odmawiamy wprost, żeby w dzienniku wysyłek stanął powód do naprawienia,
        // a nie cudzy komunikat w rodzaju „550 sender domain not found”.
        if (NotificationSettings.IsUnroutableSenderAddress(message.FromEmail))
        {
            throw new InvalidOperationException(
                $"Adres nadawcy '{message.FromEmail}' prowadzi do domeny, której nie ma w DNS — "
                + "poczta z niego nie dojdzie. Ustaw adres szkoły w Powiadomieniach albo "
                + "w zmiennej NOTIFICATIONS_FROM_EMAIL.");
        }

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
