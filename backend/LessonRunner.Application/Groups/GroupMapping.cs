using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Participants;

namespace LessonRunner.Application.Groups;

/// <summary>Wspólne mapowanie encji grup na DTO (używane przez GroupService i SessionService).</summary>
internal static class GroupMapping
{
    // Nazwy i etykiety statusów żyją w domenie - tu tylko je przepuszczamy, żeby nie mnożyć
    // kopii słownika, które potrafią się rozjechać.
    public static string StatusName(ScheduledSessionStatus status) => status.Name();

    public static string StatusLabel(ScheduledSessionStatus status) => status.Label();

    /// <summary>Statusy do wyboru w panelu - bez `InProgress`, który ustawia się sam przy starcie zajęć.</summary>
    public static IReadOnlyList<SessionStatusOptionDto> SessionStatusOptions { get; } =
    new[]
    {
        ScheduledSessionStatus.Planned,
        ScheduledSessionStatus.Confirmed,
        ScheduledSessionStatus.Completed,
        ScheduledSessionStatus.CancelledByInstructor,
        ScheduledSessionStatus.CancelledByParent,
        ScheduledSessionStatus.AwaitingReschedule,
        ScheduledSessionStatus.TechnicalFailure,
        ScheduledSessionStatus.NotDelivered
    }
    .Select(status => new SessionStatusOptionDto(status.Name(), status.Label(), status.CountsAsHeld()))
    .ToList();

    public static ScheduledSessionStatus? ParseSessionStatus(string? status) =>
        Enum.TryParse<ScheduledSessionStatus>(status, ignoreCase: true, out var parsed) ? parsed : null;

    public static ParticipantDto ToParticipantDto(Participant participant, EnrollmentStatus enrollmentStatus) =>
        new(
            participant.Id,
            participant.FirstName,
            participant.LastName,
            participant.Phone,
            participant.Email,
            enrollmentStatus.ToString().ToLowerInvariant());

    public static ScheduledSessionDto ToSessionDto(
        ScheduledSession session,
        Group group,
        IReadOnlyDictionary<Guid, string> lessonTitles,
        IReadOnlyDictionary<Guid, string>? locationNames = null,
        IReadOnlyDictionary<Guid, string>? instructorNames = null)
    {
        string? lessonTitle = session.LessonId is Guid lessonId && lessonTitles.TryGetValue(lessonId, out var title)
            ? title
            : null;
        var locationId = session.LocationId ?? group.LocationId;
        string? locationName = locationId is Guid id && locationNames?.TryGetValue(id, out var name) == true
            ? name
            : null;

        return new ScheduledSessionDto(
            session.Id,
            group.Id,
            group.Name,
            session.LessonId,
            lessonTitle,
            session.ScheduledAt,
            session.SequenceNumber,
            StatusName(session.Status),
            StatusLabel(session.Status),
            session.StartedAt,
            session.CompletedAt,
            session.InstructorNote,
            locationId,
            locationName,
            session.SubstituteInstructorId,
            session.SubstituteInstructorId is Guid substituteId && instructorNames?.TryGetValue(substituteId, out var substituteName) == true
                ? substituteName
                : null,
            // Link terminu wygrywa z linkiem grupy: zastępstwo prowadzi u siebie, a zajęcia
            // odrabiane potrafią być na innej platformie.
            string.IsNullOrWhiteSpace(session.MeetingUrl) ? group.MeetingUrl : session.MeetingUrl,
            session.MeetingUrl,
            session.RecordingUrl);
    }

    public static SessionAttendanceDto ToAttendanceDto(
        Guid sessionId,
        ScheduledSessionStatus status,
        IReadOnlyList<Participant> participants,
        IReadOnlyList<AttendanceRecord> records,
        IReadOnlyList<MakeupSessionOptionDto>? makeupOptions = null)
    {
        var recordsById = records.ToDictionary(record => record.ParticipantId);

        var entries = participants
            .OrderBy(participant => participant.LastName)
            .ThenBy(participant => participant.FirstName)
            .Select(participant =>
            {
                var record = recordsById.GetValueOrDefault(participant.Id);
                var attendanceStatus = record?.Status ?? AttendanceStatus.UnexcusedAbsence;

                return new AttendanceEntryDto(
                    participant.Id,
                    participant.FirstName,
                    participant.LastName,
                    attendanceStatus.CountsAsPresent(),
                    record?.MakeupRequired ?? false,
                    record?.MakeupSessionId,
                    AttendanceStatusName(attendanceStatus),
                    attendanceStatus.Label(),
                    record?.Note,
                    record?.JoinedAt,
                    record?.LeftAt);
            })
            .ToList();

        return new SessionAttendanceDto(
            sessionId,
            StatusName(status),
            StatusLabel(status),
            entries,
            makeupOptions ?? [],
            AttendanceStatusOptions);
    }

    public static string AttendanceStatusName(AttendanceStatus status) => status.ToString().ToLowerInvariant();

    /// <summary>Kolejność jak w kokpicie: najpierw to, co instruktor klika najczęściej.</summary>
    public static IReadOnlyList<AttendanceStatusOptionDto> AttendanceStatusOptions { get; } =
    new[]
    {
        AttendanceStatus.Present,
        AttendanceStatus.Late,
        AttendanceStatus.LeftEarly,
        AttendanceStatus.Partial,
        AttendanceStatus.Inactive,
        AttendanceStatus.TechnicalIssues,
        AttendanceStatus.ExcusedAbsence,
        AttendanceStatus.UnexcusedAbsence,
        AttendanceStatus.MakeupElsewhere
    }
    .Select(status => new AttendanceStatusOptionDto(
        AttendanceStatusName(status),
        status.Label(),
        status.CountsAsPresent()))
    .ToList();

    /// <summary>Parsuje status z żądania. Gdy nie podano, wracamy do starego kontraktu
    /// opartego na samym `present`, żeby nie zepsuć istniejących klientów.</summary>
    public static AttendanceStatus ParseAttendanceStatus(string? status, bool present)
    {
        if (!string.IsNullOrWhiteSpace(status)
            && Enum.TryParse<AttendanceStatus>(status, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        return present ? AttendanceStatus.Present : AttendanceStatus.UnexcusedAbsence;
    }
}
