using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Notifications;

public sealed class NotificationLog : Entity
{
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SentAt { get; set; }
    public required string Type { get; set; }
    public required string Channel { get; set; }
    public required string Recipient { get; set; }
    public required string Subject { get; set; }
    public required string Status { get; set; }
    public required string DedupeKey { get; set; }
    public string? Error { get; set; }
}
