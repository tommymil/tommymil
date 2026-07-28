using LessonRunner.Application.Notifications;
using Microsoft.Extensions.Logging;

namespace LessonRunner.Infrastructure.Notifications;

internal sealed class LogEmailSender(ILogger<LogEmailSender> logger) : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Notification email noop: To={Recipient} Subject={Subject} Body={Body}",
            message.ToEmail,
            message.Subject,
            message.Body);
        return Task.CompletedTask;
    }
}
