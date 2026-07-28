namespace LessonRunner.Application.Notifications;

public sealed record NotificationSettingsDto(
    bool RemindersEnabled,
    bool AbsenceEnabled,
    int ReminderLeadHours,
    string FromName,
    string FromEmail,
    string ReminderSubject,
    string ReminderBody,
    string AbsenceSubject,
    string AbsenceBody);

public sealed record NotificationLogDto(
    Guid Id,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SentAt,
    string Type,
    string Channel,
    string Recipient,
    string Subject,
    string Status,
    string DedupeKey,
    string? Error);

public sealed record NotificationPreviewDto(string Type, string Subject, string Body);

public sealed record EmailMessage(string FromEmail, string FromName, string ToEmail, string Subject, string Body);
