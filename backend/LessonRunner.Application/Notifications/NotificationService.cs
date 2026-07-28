using LessonRunner.Application.Groups;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Participants;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Notifications;
using LessonRunner.Domain.Participants;

namespace LessonRunner.Application.Notifications;

public sealed class NotificationService(
    INotificationRepository notificationRepository,
    IEmailSender emailSender,
    IGroupRepository groupRepository,
    IParticipantRepository participantRepository,
    ILessonRepository lessonRepository) : INotificationService
{
    public async Task<NotificationSettingsDto> GetSettingsAsync(CancellationToken cancellationToken) =>
        ToDto(await notificationRepository.GetSettingsAsync(cancellationToken));

    public async Task<NotificationSettingsDto> UpdateSettingsAsync(NotificationSettingsDto dto, CancellationToken cancellationToken)
    {
        var settings = new NotificationSettings
        {
            RemindersEnabled = dto.RemindersEnabled,
            AbsenceEnabled = dto.AbsenceEnabled,
            ReminderLeadHours = Math.Clamp(dto.ReminderLeadHours, 1, 168),
            FromName = Required(dto.FromName, "Nazwa nadawcy jest wymagana."),
            FromEmail = RequiredEmail(dto.FromEmail),
            ReminderSubject = Required(dto.ReminderSubject, "Temat przypomnienia jest wymagany."),
            ReminderBody = Required(dto.ReminderBody, "Treść przypomnienia jest wymagana."),
            AbsenceSubject = Required(dto.AbsenceSubject, "Temat nieobecności jest wymagany."),
            AbsenceBody = Required(dto.AbsenceBody, "Treść nieobecności jest wymagana.")
        };

        await notificationRepository.SaveSettingsAsync(settings, cancellationToken);
        return ToDto(settings);
    }

    public async Task<IReadOnlyList<NotificationLogDto>> ListLogsAsync(int limit, CancellationToken cancellationToken)
    {
        var logs = await notificationRepository.ListLogsAsync(Math.Clamp(limit <= 0 ? 100 : limit, 1, 500), cancellationToken);
        return logs.Select(ToDto).ToList();
    }

    public async Task<NotificationPreviewDto> PreviewAsync(string type, CancellationToken cancellationToken)
    {
        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);
        var context = new TemplateContext(
            "Jan Kowalski",
            "Scratch A",
            "Pierwsze kroki w Scratch",
            DateTimeOffset.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm"),
            "Opiekun");

        var normalized = (type ?? string.Empty).Trim().ToLowerInvariant();
        return normalized == "absence"
            ? new NotificationPreviewDto("absence", Render(settings.AbsenceSubject, context), Render(settings.AbsenceBody, context))
            : new NotificationPreviewDto("reminder", Render(settings.ReminderSubject, context), Render(settings.ReminderBody, context));
    }

    public async Task NotifyAbsencesAsync(Guid sessionId, IReadOnlyList<Guid> absentParticipantIds, CancellationToken cancellationToken)
    {
        if (absentParticipantIds.Count == 0)
        {
            return;
        }

        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);

        if (!settings.AbsenceEnabled)
        {
            return;
        }

        var group = await groupRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return;
        }

        var participants = await participantRepository.GetByIdsAsync(absentParticipantIds.Distinct().ToList(), cancellationToken);
        var lessonTitle = await LessonTitleAsync(session.LessonId, cancellationToken);

        foreach (var participant in participants)
        {
            if (!CanContactGuardian(participant))
            {
                continue;
            }

            var dedupeKey = $"absence:{sessionId}:{participant.Id}";
            if (await notificationRepository.HasLogAsync(dedupeKey, cancellationToken))
            {
                continue;
            }

            var context = CreateContext(participant, group, session, lessonTitle);
            await SendLoggedAsync(
                "absence",
                dedupeKey,
                participant.GuardianEmail!,
                Render(settings.AbsenceSubject, context),
                Render(settings.AbsenceBody, context),
                settings,
                cancellationToken);
        }
    }

    public async Task SendUpcomingSessionRemindersAsync(CancellationToken cancellationToken)
    {
        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);

        if (!settings.RemindersEnabled)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var until = now.AddHours(settings.ReminderLeadHours);
        var groups = await groupRepository.ListAsync(cancellationToken);
        var lessonTitles = (await lessonRepository.ListAsync(cancellationToken)).ToDictionary(lesson => lesson.Id, lesson => lesson.Title);

        foreach (var group in groups)
        {
            var participants = await participantRepository.GetByIdsAsync(
                group.Enrollments
                    .Where(enrollment => enrollment.Status == EnrollmentStatus.Enrolled)
                    .Select(enrollment => enrollment.ParticipantId)
                    .ToList(),
                cancellationToken);

            foreach (var session in group.Sessions.Where(session =>
                session.Status.IsUpcoming()
                && session.ScheduledAt >= now
                && session.ScheduledAt <= until))
            {
                var lessonTitle = session.LessonId is Guid lessonId && lessonTitles.TryGetValue(lessonId, out var title)
                    ? title
                    : "Zajęcia";

                foreach (var participant in participants.Where(CanContactGuardian))
                {
                    var dedupeKey = $"reminder:{session.Id}:{participant.Id}";
                    if (await notificationRepository.HasLogAsync(dedupeKey, cancellationToken))
                    {
                        continue;
                    }

                    var context = CreateContext(participant, group, session, lessonTitle);
                    await SendLoggedAsync(
                        "reminder",
                        dedupeKey,
                        participant.GuardianEmail!,
                        Render(settings.ReminderSubject, context),
                        Render(settings.ReminderBody, context),
                        settings,
                        cancellationToken);
                }
            }
        }
    }

    private async Task SendLoggedAsync(
        string type,
        string dedupeKey,
        string recipient,
        string subject,
        string body,
        NotificationSettings settings,
        CancellationToken cancellationToken)
    {
        var log = new NotificationLog
        {
            Type = type,
            Channel = "email",
            Recipient = recipient,
            Subject = subject,
            Status = "pending",
            DedupeKey = dedupeKey
        };

        try
        {
            await emailSender.SendAsync(new EmailMessage(settings.FromEmail, settings.FromName, recipient, subject, body), cancellationToken);
            log.Status = "sent";
            log.SentAt = DateTimeOffset.UtcNow;
        }
        catch (Exception ex)
        {
            log.Status = "failed";
            log.Error = ex.Message;
        }

        await notificationRepository.AddLogAsync(log, cancellationToken);
    }

    private async Task<string> LessonTitleAsync(Guid? lessonId, CancellationToken cancellationToken)
    {
        if (lessonId is null)
        {
            return "Zajęcia";
        }

        var lesson = await lessonRepository.GetByIdAsync(lessonId.Value, cancellationToken);
        return lesson?.Title ?? "Zajęcia";
    }

    private static TemplateContext CreateContext(Participant participant, Group group, ScheduledSession session, string lessonTitle) =>
        new(
            $"{participant.FirstName} {participant.LastName}",
            group.Name,
            lessonTitle,
            session.ScheduledAt.ToString("yyyy-MM-dd HH:mm"),
            participant.GuardianName ?? "Opiekun");

    private static bool CanContactGuardian(Participant participant) =>
        participant.DataProcessingConsentAt is not null
        && !string.IsNullOrWhiteSpace(participant.GuardianEmail)
        && participant.GuardianEmail.Contains('@');

    private static string Render(string template, TemplateContext context) =>
        template
            .Replace("{{participant}}", context.Participant, StringComparison.OrdinalIgnoreCase)
            .Replace("{{group}}", context.Group, StringComparison.OrdinalIgnoreCase)
            .Replace("{{lesson}}", context.Lesson, StringComparison.OrdinalIgnoreCase)
            .Replace("{{sessionAt}}", context.SessionAt, StringComparison.OrdinalIgnoreCase)
            .Replace("{{guardian}}", context.Guardian, StringComparison.OrdinalIgnoreCase);

    private static NotificationSettingsDto ToDto(NotificationSettings settings) =>
        new(
            settings.RemindersEnabled,
            settings.AbsenceEnabled,
            settings.ReminderLeadHours,
            settings.FromName,
            settings.FromEmail,
            settings.ReminderSubject,
            settings.ReminderBody,
            settings.AbsenceSubject,
            settings.AbsenceBody);

    private static NotificationLogDto ToDto(NotificationLog log) =>
        new(log.Id, log.CreatedAt, log.SentAt, log.Type, log.Channel, log.Recipient, log.Subject, log.Status, log.DedupeKey, log.Error);

    private static string Required(string? value, string error)
    {
        var normalized = (value ?? string.Empty).Trim();
        return normalized.Length == 0 ? throw new ArgumentException(error) : normalized;
    }

    private static string RequiredEmail(string? value)
    {
        var normalized = Required(value, "Adres e-mail nadawcy jest wymagany.");
        return normalized.Contains('@') ? normalized : throw new ArgumentException("Adres e-mail nadawcy jest nieprawidłowy.");
    }

    private sealed record TemplateContext(string Participant, string Group, string Lesson, string SessionAt, string Guardian);
}
