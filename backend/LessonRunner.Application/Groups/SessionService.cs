using LessonRunner.Application.Auth;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Notifications;
using LessonRunner.Application.Participants;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Participants;

namespace LessonRunner.Application.Groups;

public sealed class SessionService(
    IGroupRepository groupRepository,
    ILessonRepository lessonRepository,
    IParticipantRepository participantRepository,
    IUserRepository? userRepository = null,
    INotificationService? notificationService = null) : ISessionService
{
    public async Task<IReadOnlyList<ScheduledSessionDto>> GetScheduleAsync(Guid userId, CancellationToken cancellationToken)
    {
        var groups = await groupRepository.ListAsync(cancellationToken);
        var lessonTitles = await LessonTitleMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);

        return groups
            .SelectMany(group => group.Sessions
                .Where(session => group.InstructorId == userId || session.SubstituteInstructorId == userId)
                .Select(session => GroupMapping.ToSessionDto(session, group, lessonTitles, null, instructorNames)))
            .OrderBy(session => session.ScheduledAt)
            .ThenBy(session => session.SequenceNumber)
            .ToList();
    }

    public async Task<ScheduledSessionDto?> GetSessionAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken)
    {
        var (group, session) = await LoadOwnedAsync(sessionId, userId, cancellationToken);

        if (group is null || session is null)
        {
            return null;
        }

        var lessonTitles = await LessonTitleMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        return GroupMapping.ToSessionDto(session, group, lessonTitles, null, instructorNames);
    }

    public async Task<SessionAttendanceDto?> StartAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken)
    {
        var (group, session) = await LoadOwnedAsync(sessionId, userId, cancellationToken);

        if (group is null || session is null)
        {
            return null;
        }

        if (session.Status.IsCancelled())
        {
            throw new ArgumentException("Termin jest odwołany.");
        }

        if (session.Status.IsUpcoming())
        {
            session.Status = ScheduledSessionStatus.InProgress;
            session.StartedAt = DateTimeOffset.UtcNow;
            await groupRepository.UpdateSessionAsync(session, cancellationToken);
        }

        // Utwórz brakujące wiersze obecności (domyślnie niezaznaczeni) dla aktualnych uczestników.
        var enrolled = await SessionParticipantsAsync(group, session, cancellationToken);
        var existing = session.Attendance.ToDictionary(record => record.ParticipantId);
        var records = enrolled
            .Select(participant => existing.TryGetValue(participant.Id, out var record)
                ? record
                : new AttendanceRecord { ScheduledSessionId = sessionId, ParticipantId = participant.Id, Present = false })
            .ToList();

        // UWAGA: tu NIE wysyłamy powiadomień o nieobecności. Na starcie zajęć wszyscy są
        // domyślnie niezaznaczeni, więc wysyłka w tym miejscu oznaczałaby e-mail "dziecko było
        // nieobecne" do każdego opiekuna, zanim instruktor w ogóle odhaczył listę.
        // Powiadomienia idą dopiero z FinishAsync, gdy obecność jest już ostateczna.
        await groupRepository.SaveAttendanceAsync(sessionId, records, cancellationToken);

        var participants = await AttendanceParticipantsAsync(group, session, cancellationToken);
        var makeupOptions = await MakeupSessionOptionsAsync(sessionId, cancellationToken);
        return GroupMapping.ToAttendanceDto(sessionId, session.Status, participants, records, makeupOptions);
    }

    public async Task<SessionAttendanceDto?> GetAttendanceAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken)
    {
        var (group, session) = await LoadOwnedAsync(sessionId, userId, cancellationToken);

        if (group is null || session is null)
        {
            return null;
        }

        var participants = await AttendanceParticipantsAsync(group, session, cancellationToken);
        var makeupOptions = await MakeupSessionOptionsAsync(sessionId, cancellationToken);
        return GroupMapping.ToAttendanceDto(sessionId, session.Status, participants, session.Attendance, makeupOptions);
    }

    public async Task<SessionAttendanceDto?> SaveAttendanceAsync(
        Guid sessionId,
        Guid userId,
        SaveAttendanceDto dto,
        CancellationToken cancellationToken)
    {
        var (group, session) = await LoadOwnedAsync(sessionId, userId, cancellationToken);

        if (group is null || session is null)
        {
            return null;
        }

        if (session.Status.IsCancelled())
        {
            throw new ArgumentException("Termin jest odwołany.");
        }

        var entryById = (dto.Entries ?? []).ToDictionary(entry => entry.ParticipantId);
        var existing = session.Attendance.ToDictionary(record => record.ParticipantId);
        var enrolled = await SessionParticipantsAsync(group, session, cancellationToken);

        var records = enrolled
            .Select(participant =>
            {
                var hasSubmittedEntry = entryById.TryGetValue(participant.Id, out var submittedEntry);
                var status = hasSubmittedEntry && submittedEntry is not null
                    ? GroupMapping.ParseAttendanceStatus(submittedEntry.Status, submittedEntry.Present)
                    : AttendanceStatus.UnexcusedAbsence;
                var present = status.CountsAsPresent();

                // Odrabianie ma sens tylko dla dziecka, którego nie było.
                var makeupRequired = hasSubmittedEntry && !present && submittedEntry is not null && submittedEntry.MakeupRequired;
                var makeupSessionId = makeupRequired ? submittedEntry?.MakeupSessionId : null;

                if (existing.TryGetValue(participant.Id, out var record))
                {
                    // Wiersze, których instruktor nie przysłał, zostawiamy nietknięte -
                    // zapis częściowej listy nie może kasować już odhaczonych dzieci.
                    if (!hasSubmittedEntry)
                    {
                        return record;
                    }

                    record.Status = status;
                    record.Note = NormalizeNote(submittedEntry?.Note);
                    record.JoinedAt = submittedEntry?.JoinedAt;
                    record.LeftAt = submittedEntry?.LeftAt;
                    record.MakeupRequired = makeupRequired;
                    record.MakeupSessionId = makeupSessionId;
                    record.MarkedAt = DateTimeOffset.UtcNow;
                    return record;
                }

                return new AttendanceRecord
                {
                    ScheduledSessionId = sessionId,
                    ParticipantId = participant.Id,
                    Status = status,
                    Note = NormalizeNote(submittedEntry?.Note),
                    JoinedAt = submittedEntry?.JoinedAt,
                    LeftAt = submittedEntry?.LeftAt,
                    MakeupRequired = makeupRequired,
                    MakeupSessionId = makeupSessionId
                };
            })
            .ToList();

        await groupRepository.SaveAttendanceAsync(sessionId, records, cancellationToken);

        var participants = await AttendanceParticipantsAsync(group, session, cancellationToken);
        var makeupOptions = await MakeupSessionOptionsAsync(sessionId, cancellationToken);
        return GroupMapping.ToAttendanceDto(sessionId, session.Status, participants, records, makeupOptions);
    }

    public async Task<ScheduledSessionDto?> FinishAsync(
        Guid sessionId,
        Guid userId,
        FinishSessionDto dto,
        CancellationToken cancellationToken)
    {
        var (group, session) = await LoadOwnedAsync(sessionId, userId, cancellationToken);

        if (group is null || session is null)
        {
            return null;
        }

        if (session.Status.IsCancelled())
        {
            throw new ArgumentException("Termin jest odwołany.");
        }

        session.Status = ScheduledSessionStatus.Completed;
        session.StartedAt ??= DateTimeOffset.UtcNow;
        session.CompletedAt = DateTimeOffset.UtcNow;
        session.InstructorNote = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();

        await groupRepository.UpdateSessionAsync(session, cancellationToken);

        // Obecność jest już ostateczna - dopiero teraz informujemy opiekunów o nieobecnościach.
        // Deduplikacja po kluczu "absence:{sessionId}:{participantId}" w NotificationService
        // sprawia, że ponowne zakończenie terminu nie wyśle drugiego e-maila.
        if (notificationService is not null)
        {
            // Powiadamiamy tylko o nieobecności niezgłoszonej. Jeżeli opiekun sam zgłosił,
            // że dziecka nie będzie, informowanie go o tym z powrotem jest bez sensu.
            var absentParticipantIds = session.Attendance
                .Where(record => record.Status.ShouldNotifyGuardian())
                .Select(record => record.ParticipantId)
                .Distinct()
                .ToList();

            if (absentParticipantIds.Count > 0)
            {
                await notificationService.NotifyAbsencesAsync(sessionId, absentParticipantIds, cancellationToken);
            }
        }

        var lessonTitles = await LessonTitleMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        return GroupMapping.ToSessionDto(session, group, lessonTitles, null, instructorNames);
    }

    private static string? NormalizeNote(string? note)
    {
        var trimmed = note?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed[..Math.Min(trimmed.Length, 500)];
    }

    private async Task<(Group? Group, ScheduledSession? Session)> LoadOwnedAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetBySessionIdAsync(sessionId, cancellationToken);

        if (group is null)
        {
            return (null, null);
        }

        var session = group.Sessions.FirstOrDefault(item => item.Id == sessionId);
        if (session is null || (group.InstructorId != userId && session.SubstituteInstructorId != userId))
        {
            return (null, null);
        }

        return (group, session);
    }

    private async Task<IReadOnlyDictionary<Guid, string>> LessonTitleMapAsync(CancellationToken cancellationToken)
    {
        var lessons = await lessonRepository.ListAsync(cancellationToken);
        return lessons.ToDictionary(lesson => lesson.Id, lesson => lesson.Title);
    }

    private async Task<IReadOnlyDictionary<Guid, string>> InstructorNameMapAsync(CancellationToken cancellationToken)
    {
        if (userRepository is null)
        {
            return new Dictionary<Guid, string>();
        }

        var users = await userRepository.ListAsync(cancellationToken);
        return users.ToDictionary(user => user.Id, user => user.DisplayName);
    }

    private async Task<IReadOnlyList<Participant>> SessionParticipantsAsync(
        Group group,
        ScheduledSession session,
        CancellationToken cancellationToken)
    {
        var participantIds = group.Enrollments
            .Where(enrollment => enrollment.Status == EnrollmentStatus.Enrolled)
            .Select(enrollment => enrollment.ParticipantId)
            .Concat(session.Attendance.Select(record => record.ParticipantId))
            .Concat(await MakeupParticipantIdsForSessionAsync(session.Id, cancellationToken))
            .Distinct()
            .ToList();

        return await participantRepository.GetByIdsAsync(participantIds, cancellationToken);
    }

    /// <summary>Aktualni członkowie grupy + każdy, kto ma zapis obecności w tym konkretnym terminie -
    /// wypisanie z grupy nie chowa historii obecności z terminów, w których ktoś już był odnotowany.</summary>
    private async Task<IReadOnlyList<Participant>> AttendanceParticipantsAsync(
        Group group,
        ScheduledSession session,
        CancellationToken cancellationToken)
    {
        var makeupParticipantIds = await MakeupParticipantIdsForSessionAsync(session.Id, cancellationToken);
        var participantIds = group.Enrollments
            .Select(enrollment => enrollment.ParticipantId)
            .Concat(session.Attendance.Select(record => record.ParticipantId))
            .Concat(makeupParticipantIds)
            .Distinct()
            .ToList();

        return await participantRepository.GetByIdsAsync(participantIds, cancellationToken);
    }

    private async Task<IReadOnlyList<Guid>> MakeupParticipantIdsForSessionAsync(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var groups = await groupRepository.ListAsync(cancellationToken);
        return groups
            .SelectMany(group => group.Sessions)
            .SelectMany(session => session.Attendance)
            .Where(record => record.MakeupRequired && record.MakeupSessionId == sessionId)
            .Select(record => record.ParticipantId)
            .Distinct()
            .ToList();
    }

    private async Task<IReadOnlyList<MakeupSessionOptionDto>> MakeupSessionOptionsAsync(
        Guid currentSessionId,
        CancellationToken cancellationToken)
    {
        var groups = await groupRepository.ListAsync(cancellationToken);
        var lessonTitles = await LessonTitleMapAsync(cancellationToken);
        var earliest = DateTimeOffset.UtcNow.AddDays(-1);

        return groups
            .SelectMany(group => group.Sessions.Select(session => (group, session)))
            .Where(item => item.session.Id != currentSessionId)
            .Where(item => item.session.Status.IsActive())
            .Where(item => item.session.ScheduledAt >= earliest)
            .OrderBy(item => item.session.ScheduledAt)
            .ThenBy(item => item.group.Name)
            .Take(50)
            .Select(item => new MakeupSessionOptionDto(
                item.session.Id,
                item.group.Id,
                item.group.Name,
                item.session.ScheduledAt,
                item.session.LessonId is Guid lessonId && lessonTitles.TryGetValue(lessonId, out var title) ? title : null))
            .ToList();
    }
}
