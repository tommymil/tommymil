namespace LessonRunner.Infrastructure.Notifications;

internal sealed class NotificationLogDocument
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public required string Type { get; set; }
    public required string Channel { get; set; }
    public required string Recipient { get; set; }
    public required string Subject { get; set; }
    public required string Status { get; set; }
    public required string DedupeKey { get; set; }
    public string? Error { get; set; }
}

internal sealed class NotificationSettingsDocument
{
    public int Id { get; set; }
    public bool RemindersEnabled { get; set; }
    public bool AbsenceEnabled { get; set; }
    public int ReminderLeadHours { get; set; }
    public required string FromName { get; set; }
    public required string FromEmail { get; set; }
    public required string ReminderSubject { get; set; }
    public required string ReminderBody { get; set; }
    public required string AbsenceSubject { get; set; }
    public required string AbsenceBody { get; set; }
}
