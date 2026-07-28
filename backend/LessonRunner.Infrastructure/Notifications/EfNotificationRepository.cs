using LessonRunner.Application.Notifications;
using LessonRunner.Domain.Notifications;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Notifications;

internal sealed class EfNotificationRepository(AppDbContext dbContext) : INotificationRepository
{
    private const int SettingsId = 1;

    public async Task<NotificationSettings> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var document = await dbContext.NotificationSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(settings => settings.Id == SettingsId, cancellationToken);

        return document is null ? new NotificationSettings() : ToDomain(document);
    }

    public async Task SaveSettingsAsync(NotificationSettings settings, CancellationToken cancellationToken)
    {
        var document = await dbContext.NotificationSettings
            .FirstOrDefaultAsync(item => item.Id == SettingsId, cancellationToken);

        if (document is null)
        {
            dbContext.NotificationSettings.Add(ToDocument(settings));
        }
        else
        {
            document.RemindersEnabled = settings.RemindersEnabled;
            document.AbsenceEnabled = settings.AbsenceEnabled;
            document.ReminderLeadHours = settings.ReminderLeadHours;
            document.FromName = settings.FromName;
            document.FromEmail = settings.FromEmail;
            document.ReminderSubject = settings.ReminderSubject;
            document.ReminderBody = settings.ReminderBody;
            document.AbsenceSubject = settings.AbsenceSubject;
            document.AbsenceBody = settings.AbsenceBody;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> HasLogAsync(string dedupeKey, CancellationToken cancellationToken) =>
        dbContext.NotificationLogs.AnyAsync(log => log.DedupeKey == dedupeKey, cancellationToken);

    public async Task AddLogAsync(NotificationLog log, CancellationToken cancellationToken)
    {
        dbContext.NotificationLogs.Add(ToDocument(log));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationLog>> ListLogsAsync(int limit, CancellationToken cancellationToken)
    {
        // SQLite nie sortuje po DateTimeOffset w SQL - porządkujemy i tniemy limit po stronie klienta.
        var documents = await dbContext.NotificationLogs
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(log => log.CreatedAt)
            .Take(limit)
            .Select(ToDomain)
            .ToList();
    }

    private static NotificationSettings ToDomain(NotificationSettingsDocument document) => new()
    {
        RemindersEnabled = document.RemindersEnabled,
        AbsenceEnabled = document.AbsenceEnabled,
        ReminderLeadHours = document.ReminderLeadHours,
        FromName = document.FromName,
        FromEmail = document.FromEmail,
        ReminderSubject = document.ReminderSubject,
        ReminderBody = document.ReminderBody,
        AbsenceSubject = document.AbsenceSubject,
        AbsenceBody = document.AbsenceBody
    };

    private static NotificationSettingsDocument ToDocument(NotificationSettings settings) => new()
    {
        Id = SettingsId,
        RemindersEnabled = settings.RemindersEnabled,
        AbsenceEnabled = settings.AbsenceEnabled,
        ReminderLeadHours = settings.ReminderLeadHours,
        FromName = settings.FromName,
        FromEmail = settings.FromEmail,
        ReminderSubject = settings.ReminderSubject,
        ReminderBody = settings.ReminderBody,
        AbsenceSubject = settings.AbsenceSubject,
        AbsenceBody = settings.AbsenceBody
    };

    private static NotificationLog ToDomain(NotificationLogDocument document) => new()
    {
        Id = document.Id,
        CreatedAt = document.CreatedAt,
        SentAt = document.SentAt,
        Type = document.Type,
        Channel = document.Channel,
        Recipient = document.Recipient,
        Subject = document.Subject,
        Status = document.Status,
        DedupeKey = document.DedupeKey,
        Error = document.Error
    };

    private static NotificationLogDocument ToDocument(NotificationLog log) => new()
    {
        Id = log.Id,
        CreatedAt = log.CreatedAt,
        SentAt = log.SentAt,
        Type = log.Type,
        Channel = log.Channel,
        Recipient = log.Recipient,
        Subject = log.Subject,
        Status = log.Status,
        DedupeKey = log.DedupeKey,
        Error = log.Error
    };
}
