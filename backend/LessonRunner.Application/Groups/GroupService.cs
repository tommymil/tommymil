using LessonRunner.Application.Auth;
using LessonRunner.Application.Billing;
using LessonRunner.Application.Common;
using LessonRunner.Application.Courses;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Notifications;
using LessonRunner.Application.Participants;
using LessonRunner.Application.Scheduling;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Users;
using System.Text;

namespace LessonRunner.Application.Groups;

public sealed class GroupService(
    IGroupRepository groupRepository,
    ILessonRepository lessonRepository,
    IUserRepository userRepository,
    IParticipantRepository participantRepository,
    ICourseRepository? courseRepository = null,
    ISchedulingRepository? schedulingRepository = null,
    IBillingService? billingService = null,
    // Opcjonalne, żeby starsze testy budujące serwis ręcznie nadal się kompilowały.
    // Brak serwisu oznacza brak wysyłki, a nie wywróconą operację na terminie.
    INotificationService? notificationService = null) : IGroupService
{
    /// <summary>
    /// Lista grup dla panelu administratora.
    ///
    /// Instruktorów pobieramy **jednym zapytaniem przed pętlą**, a nie po jednym na grupę.
    /// Wcześniej `GetByIdAsync` w pętli dawał klasyczne N+1: przy pięćdziesięciu grupach
    /// pięćdziesiąt osobnych odczytów po tabeli, która ma kilkanaście wierszy. Przy kilkunastu
    /// grupach to niewidoczne, przy pięćdziesięciu — odczuwalne (punkt 26 planu prac).
    /// </summary>
    public async Task<IReadOnlyList<GroupSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken)
    {
        var groups = await groupRepository.ListAsync(cancellationToken);
        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructors = (await userRepository.ListAsync(cancellationToken))
            .ToDictionary(user => user.Id);
        var summaries = new List<GroupSummaryDto>(groups.Count);

        foreach (var group in groups.OrderBy(group => group.Name))
        {
            var instructor = instructors.GetValueOrDefault(group.InstructorId);
            var nextSession = group.Sessions
                .Where(session => session.Status.IsActive())
                .OrderBy(session => session.ScheduledAt)
                .Select(session => (DateTimeOffset?)session.ScheduledAt)
                .FirstOrDefault();

            summaries.Add(new GroupSummaryDto(
                group.Id,
                group.Name,
                group.InstructorId,
                instructor?.Email ?? "(nieznany)",
                instructor?.DisplayName ?? "(nieznany)",
                group.CourseId,
                group.LocationId,
                group.LocationId is Guid locationId ? locationNames.GetValueOrDefault(locationId) : null,
                group.Capacity,
                group.MeetingUrl,
                group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Enrolled),
                group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Waitlisted),
                group.Sessions.Count,
                nextSession));
        }

        return summaries;
    }

    public async Task<GroupDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(id, cancellationToken);

        if (group is null)
        {
            return null;
        }

        var instructor = await userRepository.GetByIdAsync(group.InstructorId, cancellationToken);
        var lessonsById = await LessonMapAsync(cancellationToken);
        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        var participants = await participantRepository.GetByIdsAsync(
            group.Enrollments.Select(enrollment => enrollment.ParticipantId).ToList(),
            cancellationToken);

        return ToDetails(
            group,
            instructor?.Email ?? "(nieznany)",
            instructor?.DisplayName ?? "(nieznany)",
            lessonsById,
            locationNames,
            instructorNames,
            participants);
    }

    public async Task<GroupDetailsDto> CreateAsync(CreateGroupDto dto, CancellationToken cancellationToken)
    {
        var name = (dto.Name ?? string.Empty).Trim();

        if (name.Length == 0)
        {
            throw new ArgumentException("Nazwa grupy jest wymagana.");
        }

        var instructor = await userRepository.GetByIdAsync(dto.InstructorId, cancellationToken)
            ?? throw new ArgumentException("Wybrany instruktor nie istnieje.");

        if (instructor.Role != UserRole.Instructor && instructor.Role != UserRole.Admin)
        {
            throw new ArgumentException("Wybrany użytkownik nie jest instruktorem.");
        }

        var lessons = await lessonRepository.ListAsync(cancellationToken);
        var lessonById = lessons.ToDictionary(lesson => lesson.Id);
        var lessonIds = await ResolveLessonIdsAsync(dto, cancellationToken);

        if (lessonIds.Count == 0)
        {
            throw new ArgumentException("Wybierz przynajmniej jedną lekcję lub kurs.");
        }

        foreach (var lessonId in lessonIds)
        {
            if (!lessonById.TryGetValue(lessonId, out var lesson))
            {
                throw new ArgumentException("Wybrana lekcja nie istnieje.");
            }

            if (lesson.Status != LessonStatus.Ready)
            {
                throw new ArgumentException($"Lekcja \"{lesson.Title}\" nie jest opublikowana (Ready).");
            }
        }

        var participantIds = (dto.ParticipantIds ?? []).Distinct().ToList();
        var participants = await participantRepository.GetByIdsAsync(participantIds, cancellationToken);

        if (participants.Count != participantIds.Count)
        {
            throw new ArgumentException("Wybrany uczestnik nie istnieje.");
        }

        var locationId = await ResolveLocationIdAsync(dto.LocationId, cancellationToken);
        var holidays = await HolidayDatesAsync(cancellationToken);
        var capacity = NormalizeCapacity(dto.Capacity);
        var group = new Group
        {
            Name = name,
            InstructorId = dto.InstructorId,
            CourseId = dto.CourseId,
            LocationId = locationId,
            Capacity = capacity,
            MeetingUrl = NormalizeMeetingUrl(dto.MeetingUrl)
        };

        group.Enrollments = participantIds
            .Select((participantId, index) => new GroupEnrollment
            {
                GroupId = group.Id,
                ParticipantId = participantId,
                Status = IsWithinCapacity(index, capacity) ? EnrollmentStatus.Enrolled : EnrollmentStatus.Waitlisted
            })
            .ToList();

        // Terminy: co tydzień, jedna lekcja = jeden termin, w kolejności wybranych lekcji.
        // Tydzień liczymy po godzinie ściennej w strefie zajęć, nie po dokładnych 168 h -
        // inaczej terminy przechodzące przez zmianę czasu (marzec/październik) dryfowałyby o godzinę.
        group.Sessions = lessonIds
            .Select((lessonId, index) => new ScheduledSession
            {
                GroupId = group.Id,
                LessonId = lessonId,
                ScheduledAt = MovePastHoliday(AddWeeksPreservingLocalTime(dto.FirstSessionAt, index), holidays),
                LocationId = locationId,
                SubstituteInstructorId = null,
                SequenceNumber = index + 1,
                Status = ScheduledSessionStatus.Planned
            })
            .ToList();

        await EnsureNoSchedulingConflictsAsync(group, group.Sessions, new HashSet<Guid>(), cancellationToken);
        await groupRepository.AddAsync(group, cancellationToken);

        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        return ToDetails(group, instructor.Email, instructor.DisplayName, lessonById, locationNames, instructorNames, participants);
    }

    public async Task<GroupDetailsDto?> UpdateAsync(Guid id, UpdateGroupDto dto, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(id, cancellationToken);

        if (group is null)
        {
            return null;
        }

        var name = (dto.Name ?? string.Empty).Trim();

        if (name.Length == 0)
        {
            throw new ArgumentException("Nazwa grupy jest wymagana.");
        }

        var instructor = await userRepository.GetByIdAsync(dto.InstructorId, cancellationToken)
            ?? throw new ArgumentException("Wybrany instruktor nie istnieje.");

        if (instructor.Role != UserRole.Instructor && instructor.Role != UserRole.Admin)
        {
            throw new ArgumentException("Wybrany użytkownik nie jest instruktorem.");
        }

        group.Name = name;
        group.InstructorId = dto.InstructorId;
        group.LocationId = await ResolveLocationIdAsync(dto.LocationId, cancellationToken);
        group.Capacity = NormalizeCapacity(dto.Capacity);
        group.MeetingUrl = NormalizeMeetingUrl(dto.MeetingUrl);
        NormalizeEnrollmentStatuses(group);
        group.UpdatedAt = DateTimeOffset.UtcNow;
        await EnsureNoSchedulingConflictsAsync(
            group,
            group.Sessions.Where(session => !session.Status.IsCancelled()).ToList(),
            group.Sessions.Select(session => session.Id).ToHashSet(),
            cancellationToken);
        await groupRepository.UpdateGroupAsync(group, cancellationToken);
        foreach (var enrollment in group.Enrollments)
        {
            await groupRepository.SetEnrollmentStatusAsync(group.Id, enrollment.ParticipantId, enrollment.Status, cancellationToken);
        }

        var lessonsById = await LessonMapAsync(cancellationToken);
        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        var participants = await participantRepository.GetByIdsAsync(
            group.Enrollments.Select(enrollment => enrollment.ParticipantId).ToList(),
            cancellationToken);

        return ToDetails(group, instructor.Email, instructor.DisplayName, lessonsById, locationNames, instructorNames, participants);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        groupRepository.DeleteAsync(id, cancellationToken);

    public async Task<ScheduledSessionDto?> AddSessionAsync(Guid groupId, AddSessionDto dto, Guid? actingUserId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);

        if (group is null)
        {
            return null;
        }

        var lesson = await lessonRepository.GetByIdAsync(dto.LessonId, cancellationToken)
            ?? throw new ArgumentException("Wybrana lekcja nie istnieje.");

        if (lesson.Status != LessonStatus.Ready)
        {
            throw new ArgumentException($"Lekcja \"{lesson.Title}\" nie jest opublikowana (Ready).");
        }

        var nextSequence = group.Sessions.Count == 0 ? 1 : group.Sessions.Max(session => session.SequenceNumber) + 1;
        var locationId = await ResolveLocationIdAsync(dto.LocationId ?? group.LocationId, cancellationToken);
        var substituteInstructorId = await ResolveInstructorIdAsync(dto.SubstituteInstructorId, cancellationToken);
        await EnsureNotHolidayAsync(dto.ScheduledAt, cancellationToken);
        var session = new ScheduledSession
        {
            GroupId = group.Id,
            LessonId = dto.LessonId,
            ScheduledAt = dto.ScheduledAt,
            LocationId = locationId,
            SubstituteInstructorId = substituteInstructorId,
            SequenceNumber = nextSequence,
            Status = ScheduledSessionStatus.Planned
        };

        await EnsureNoSchedulingConflictsAsync(group, [session], new HashSet<Guid> { session.Id }, cancellationToken);
        await groupRepository.AddSessionAsync(session, cancellationToken);
        await RecordSessionChangeAsync(
            session,
            SessionChangeType.Created,
            previousScheduledAt: null,
            newScheduledAt: session.ScheduledAt,
            reason: null,
            details: null,
            actingUserId,
            guardiansNotified: false,
            cancellationToken);

        var lessonsById = await LessonMapAsync(cancellationToken);
        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        return GroupMapping.ToSessionDto(session, group, lessonsById, locationNames, instructorNames);
    }

    public async Task<ScheduledSessionDto?> CancelSessionAsync(
        Guid groupId,
        Guid sessionId,
        CancelSessionDto? dto,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return null;
        }

        // Rozróżniamy, kto odwołał - przy rozliczeniu i reklamacji to nie jest ten sam przypadek.
        session.Status = string.Equals(dto?.CancelledBy, "parent", StringComparison.OrdinalIgnoreCase)
            ? ScheduledSessionStatus.CancelledByParent
            : ScheduledSessionStatus.CancelledByInstructor;
        await groupRepository.UpdateSessionAsync(session, cancellationToken);

        // Znacznik „powiadomiono opiekunów” bierzemy z faktycznej wysyłki, a nie z deklaracji
        // w formularzu. Dotąd zaznaczał go człowiek, a system zapisywał tę deklarację jako
        // dowód przy reklamacji „nie dostaliśmy informacji o zmianie”.
        var notified = await NotifyScheduleChangeAsync(
            sessionId, previousScheduledAt: null, dto?.Reason, cancelled: true, cancellationToken);

        await RecordSessionChangeAsync(
            session,
            SessionChangeType.Cancelled,
            previousScheduledAt: session.ScheduledAt,
            newScheduledAt: null,
            reason: dto?.Reason,
            details: session.Status.Label(),
            actingUserId,
            notified || (dto?.GuardiansNotified ?? false),
            cancellationToken);

        await ApplyCompensationAsync(group, session, dto, actingUserId, cancellationToken);

        if (dto?.ShiftFollowingLessons == true)
        {
            await ShiftFollowingLessonsAsync(group, session, actingUserId, cancellationToken);
        }

        var lessonsById = await LessonMapAsync(cancellationToken);
        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        return GroupMapping.ToSessionDto(session, group, lessonsById, locationNames, instructorNames);
    }

    /// <summary>
    /// Przesuwa materiał po odwołanych zajęciach: lekcja z odwołanego terminu wchodzi na
    /// najbliższy zaplanowany, każda kolejna przesuwa się o jeden, a wypchnięta z końca
    /// dostaje nowy termin tydzień po ostatnim. Kurs wydłuża się o jedne zajęcia.
    ///
    /// Daty terminów zostają nietknięte — przesuwamy przypisanie lekcji, nie kalendarz.
    /// Dzięki temu rodzice nie muszą przestawiać niczego w swoim tygodniu, a materiał
    /// zostaje zrealizowany w całości.
    ///
    /// Terminów zakończonych i odwołanych nie ruszamy.
    /// </summary>
    private async Task ShiftFollowingLessonsAsync(
        Group group,
        ScheduledSession cancelled,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var following = group.Sessions
            .Where(session => session.SequenceNumber > cancelled.SequenceNumber && session.Status.IsUpcoming())
            .OrderBy(session => session.SequenceNumber)
            .ToList();

        if (following.Count == 0 || cancelled.LessonId is null)
        {
            return;
        }

        Guid? lessonToPlace = cancelled.LessonId;

        foreach (var session in following)
        {
            var displaced = session.LessonId;
            session.LessonId = lessonToPlace;
            await groupRepository.UpdateSessionAsync(session, cancellationToken);
            lessonToPlace = displaced;
        }

        await RecordSessionChangeAsync(
            cancelled,
            SessionChangeType.StatusChanged,
            previousScheduledAt: null,
            newScheduledAt: null,
            reason: null,
            details: $"Materiał przesunięty na kolejne terminy ({following.Count} zajęć)",
            actingUserId,
            guardiansNotified: false,
            cancellationToken);

        // Lekcja wypchnięta z ostatniego terminu potrzebuje nowego miejsca na końcu kursu.
        if (lessonToPlace is not Guid trailingLessonId)
        {
            return;
        }

        var last = following[^1];
        var holidays = await HolidayDatesAsync(cancellationToken);
        var extraSession = new ScheduledSession
        {
            GroupId = group.Id,
            LessonId = trailingLessonId,
            ScheduledAt = MovePastHoliday(AddWeeksPreservingLocalTime(last.ScheduledAt, 1), holidays),
            LocationId = last.LocationId,
            SequenceNumber = group.Sessions.Max(session => session.SequenceNumber) + 1,
            Status = ScheduledSessionStatus.Planned
        };

        await groupRepository.AddSessionAsync(extraSession, cancellationToken);
        await RecordSessionChangeAsync(
            extraSession,
            SessionChangeType.Created,
            previousScheduledAt: null,
            newScheduledAt: extraSession.ScheduledAt,
            reason: null,
            details: "Termin dodany po przesunięciu materiału z odwołanych zajęć",
            actingUserId,
            guardiansNotified: false,
            cancellationToken);
    }

    /// <summary>
    /// Rekompensata za odwołane zajęcia. Świadomie osobna od samego odwołania: termin można
    /// odwołać bez rozstrzygania rozliczeń, a decyzję podjąć później.
    ///
    /// Na razie automatyzujemy tylko `credit` - zwrot i odrabianie wymagają rozmowy z rodzicem
    /// i wpisuje się je ręcznie.
    /// </summary>
    private async Task ApplyCompensationAsync(
        Group group,
        ScheduledSession session,
        CancelSessionDto? dto,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        if (billingService is null || !string.Equals(dto?.Compensation, "credit", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var reason = string.IsNullOrWhiteSpace(dto?.Reason)
            ? $"Odwołane zajęcia #{session.SequenceNumber} w grupie {group.Name}"
            : $"Odwołane zajęcia #{session.SequenceNumber}: {dto!.Reason!.Trim()}";

        foreach (var enrollment in group.Enrollments.Where(item => item.Status == EnrollmentStatus.Enrolled))
        {
            await billingService.IssueCreditAsync(
                new IssueLessonCreditDto(
                    enrollment.ParticipantId,
                    reason,
                    group.Id,
                    session.Id,
                    ExpiresAt: dto?.CreditExpiresAt),
                actingUserId,
                cancellationToken);
        }
    }

    public async Task<ScheduledSessionDto?> RescheduleSessionAsync(
        Guid groupId,
        Guid sessionId,
        RescheduleSessionDto dto,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return null;
        }

        if (session.Status.IsClosed())
        {
            throw new ArgumentException("Nie można przełożyć zakończonego lub odwołanego terminu.");
        }

        var previousScheduledAt = session.ScheduledAt;

        session.ScheduledAt = dto.ScheduledAt;
        session.LocationId = await ResolveLocationIdAsync(dto.LocationId ?? session.LocationId ?? group.LocationId, cancellationToken);
        session.SubstituteInstructorId = await ResolveInstructorIdAsync(dto.SubstituteInstructorId ?? session.SubstituteInstructorId, cancellationToken);
        await EnsureNotHolidayAsync(session.ScheduledAt, cancellationToken);
        await EnsureNoSchedulingConflictsAsync(group, [session], new HashSet<Guid> { session.Id }, cancellationToken);
        await groupRepository.UpdateSessionAsync(session, cancellationToken);

        var notified = await NotifyScheduleChangeAsync(
            sessionId, previousScheduledAt, dto.Reason, cancelled: false, cancellationToken);

        await RecordSessionChangeAsync(
            session,
            SessionChangeType.Rescheduled,
            previousScheduledAt,
            session.ScheduledAt,
            dto.Reason,
            details: null,
            actingUserId,
            notified || dto.GuardiansNotified,
            cancellationToken);

        var lessonsById = await LessonMapAsync(cancellationToken);
        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        return GroupMapping.ToSessionDto(session, group, lessonsById, locationNames, instructorNames);
    }

    public async Task<ScheduledSessionDto?> SetSubstituteInstructorAsync(
        Guid groupId,
        Guid sessionId,
        SetSubstituteInstructorDto dto,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return null;
        }

        session.SubstituteInstructorId = await ResolveInstructorIdAsync(dto.SubstituteInstructorId, cancellationToken);
        await groupRepository.UpdateSessionAsync(session, cancellationToken);

        var substituteName = session.SubstituteInstructorId is Guid substitute
            ? (await userRepository.GetByIdAsync(substitute, cancellationToken))?.DisplayName
            : null;
        await RecordSessionChangeAsync(
            session,
            SessionChangeType.SubstituteChanged,
            previousScheduledAt: null,
            newScheduledAt: null,
            reason: null,
            details: substituteName is null ? "Zdjęto zastępstwo" : $"Zastępstwo: {substituteName}",
            actingUserId,
            guardiansNotified: false,
            cancellationToken);

        var lessonsById = await LessonMapAsync(cancellationToken);
        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        return GroupMapping.ToSessionDto(session, group, lessonsById, locationNames, instructorNames);
    }

    public async Task<ScheduledSessionDto?> SetSessionLinksAsync(
        Guid groupId,
        Guid sessionId,
        UpdateSessionLinksDto dto,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return null;
        }

        session.MeetingUrl = NormalizeMeetingUrl(dto.MeetingUrl);
        session.RecordingUrl = NormalizeRecordingUrl(dto.RecordingUrl);
        await groupRepository.UpdateSessionAsync(session, cancellationToken);
        await RecordSessionChangeAsync(
            session,
            SessionChangeType.LinksChanged,
            previousScheduledAt: null,
            newScheduledAt: null,
            reason: null,
            details: session.MeetingUrl is null ? "Link terminu wyczyszczony" : "Ustawiono własny link terminu",
            actingUserId,
            guardiansNotified: false,
            cancellationToken);

        var lessonsById = await LessonMapAsync(cancellationToken);
        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        return GroupMapping.ToSessionDto(session, group, lessonsById, locationNames, instructorNames);
    }

    public async Task<GroupAttendanceSummaryDto?> GetAttendanceSummaryAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);

        if (group is null)
        {
            return null;
        }

        // Frekwencję liczymy z zajęć już zakończonych.
        var allGroups = await groupRepository.ListAsync(cancellationToken);
        var heldSessions = group.Sessions
            .Where(session => session.Status.CountsAsHeld())
            .ToList();

        var heldCount = heldSessions.Count;

        // Frekwencja obejmuje aktualnych członków grupy ORAZ każdego, kto ma w tej grupie
        // jakikolwiek zapis obecności - wypisanie z grupy nie usuwa historii frekwencji.
        var participantIds = group.Enrollments
            .Select(enrollment => enrollment.ParticipantId)
            .Concat(group.Sessions.SelectMany(session => session.Attendance.Select(record => record.ParticipantId)))
            .Distinct()
            .ToList();

        var participantsById = (await participantRepository.GetByIdsAsync(participantIds, cancellationToken))
            .ToDictionary(participant => participant.Id);

        var participants = participantIds
            .Where(participantsById.ContainsKey)
            .Select(participantId => participantsById[participantId])
            .OrderBy(participant => participant.LastName)
            .ThenBy(participant => participant.FirstName)
            .Select(participant =>
            {
                // Uczeń liczy się do frekwencji tylko za te zakończone terminy, w których był
                // na liście obecności (istnieje rekord). Dołączenie w trakcie kursu nie zaniża
                // wyniku o zajęcia sprzed zapisu, a wypisany zachowuje historię swoich terminów.
                var participantSessions = heldSessions
                    .Where(session => session.Attendance.Any(record => record.ParticipantId == participant.Id))
                    .ToList();
                var participantHeld = participantSessions.Count;
                var presentCount = participantSessions.Count(session =>
                    session.Attendance.Any(record => record.ParticipantId == participant.Id && IsPresentOrMadeUp(record, allGroups)));
                var rate = participantHeld == 0 ? 0 : (int)Math.Round(100.0 * presentCount / participantHeld);

                return new ParticipantAttendanceDto(
                    participant.Id,
                    participant.FirstName,
                    participant.LastName,
                    presentCount,
                    participantHeld,
                    rate);
            })
            .ToList();

        return new GroupAttendanceSummaryDto(heldCount, participants);
    }

    private static bool IsPresentOrMadeUp(AttendanceRecord record, IReadOnlyList<Group> allGroups)
    {
        if (record.Present)
        {
            return true;
        }

        if (!record.MakeupRequired || record.MakeupSessionId is not Guid makeupSessionId)
        {
            return false;
        }

        var makeupSession = allGroups
            .SelectMany(group => group.Sessions)
            .FirstOrDefault(session => session.Id == makeupSessionId);

        return makeupSession?.Status.CountsAsHeld() == true
            && makeupSession.Attendance.Any(item => item.ParticipantId == record.ParticipantId && item.Present);
    }

    public async Task<AttendanceExportDto?> ExportAttendanceSummaryAsync(Guid groupId, string? format, CancellationToken cancellationToken)
    {
        EnsureCsvFormat(format);

        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);

        if (group is null)
        {
            return null;
        }

        var summary = await GetAttendanceSummaryAsync(groupId, cancellationToken)
            ?? throw new InvalidOperationException("Nie udało się zbudować podsumowania frekwencji.");
        var lines = new List<IReadOnlyList<string>>
        {
            new[] { "Grupa", group.Name },
            new[] { "Odbyte zajęcia", summary.HeldSessions.ToString() },
            Array.Empty<string>(),
            new[] { "Uczestnik", "Obecności", "Liczone zajęcia", "Frekwencja (%)" }
        };

        lines.AddRange(summary.Participants.Select(participant => new[]
        {
            $"{participant.FirstName} {participant.LastName}",
            participant.PresentCount.ToString(),
            participant.HeldCount.ToString(),
            participant.RatePercent.ToString()
        }));

        return new AttendanceExportDto(
            $"{FileSafe(group.Name)}-frekwencja.csv",
            "text/csv; charset=utf-8",
            ToCsvBytes(lines));
    }

    public async Task<AttendanceExportDto?> ExportSessionAttendanceAsync(
        Guid groupId,
        Guid sessionId,
        string? format,
        CancellationToken cancellationToken)
    {
        EnsureCsvFormat(format);

        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return null;
        }

        var participantIds = group.Enrollments
            .Select(enrollment => enrollment.ParticipantId)
            .Concat(session.Attendance.Select(record => record.ParticipantId))
            .Distinct()
            .ToList();
        var participants = await participantRepository.GetByIdsAsync(participantIds, cancellationToken);
        var participantsById = participants.ToDictionary(participant => participant.Id);
        var attendanceById = session.Attendance.ToDictionary(record => record.ParticipantId);
        var lessonsById = await LessonMapAsync(cancellationToken);
        var lessonTitle = session.LessonId is Guid lessonId && lessonsById.TryGetValue(lessonId, out var lesson)
            ? lesson.Title
            : string.Empty;

        var lines = new List<IReadOnlyList<string>>
        {
            new[] { "Grupa", group.Name },
            new[] { "Termin", SchoolTime.FormatDateTimeWithZone(session.ScheduledAt) },
            new[] { "Lekcja", lessonTitle },
            new[] { "Status", GroupMapping.StatusLabel(session.Status) },
            Array.Empty<string>(),
            new[] { "Lp.", "Uczestnik", "Obecny", "Status", "Notatka", "Podpis" }
        };

        var rowNumber = 1;
        lines.AddRange(participantIds
            .Where(participantsById.ContainsKey)
            .Select(id => participantsById[id])
            .OrderBy(participant => participant.LastName)
            .ThenBy(participant => participant.FirstName)
            .Select(participant => new[]
            {
                (rowNumber++).ToString(),
                $"{participant.FirstName} {participant.LastName}",
                attendanceById.TryGetValue(participant.Id, out var record) ? (record.Present ? "tak" : "nie") : string.Empty,
                attendanceById.TryGetValue(participant.Id, out record) ? record.Status.Label() : string.Empty,
                attendanceById.TryGetValue(participant.Id, out record) ? record.Note ?? string.Empty : string.Empty,
                string.Empty
            }));

        return new AttendanceExportDto(
            $"{FileSafe(group.Name)}-termin-{session.SequenceNumber}-lista-obecnosci.csv",
            "text/csv; charset=utf-8",
            ToCsvBytes(lines));
    }

    public async Task<IReadOnlyList<InstructorDto>> GetInstructorsAsync(CancellationToken cancellationToken)
    {
        var instructors = await userRepository.ListByRoleAsync(UserRole.Instructor, cancellationToken);
        return instructors.Select(user => new InstructorDto(user.Id, user.Email, user.DisplayName)).ToList();
    }

    /// <summary>Konspekty pod ręką w całości — termin potrzebuje i tytułu, i rodzaju lekcji.</summary>
    private async Task<IReadOnlyDictionary<Guid, Lesson>> LessonMapAsync(CancellationToken cancellationToken)
    {
        var lessons = await lessonRepository.ListAsync(cancellationToken);
        return lessons.ToDictionary(lesson => lesson.Id);
    }

    private async Task<IReadOnlyDictionary<Guid, string>> LocationNameMapAsync(CancellationToken cancellationToken)
    {
        if (schedulingRepository is null)
        {
            return new Dictionary<Guid, string>();
        }

        var locations = await schedulingRepository.ListLocationsAsync(cancellationToken);
        return locations.ToDictionary(location => location.Id, location => location.Name);
    }

    private async Task<IReadOnlyDictionary<Guid, string>> InstructorNameMapAsync(CancellationToken cancellationToken)
    {
        var instructors = await userRepository.ListAsync(cancellationToken);
        return instructors.ToDictionary(user => user.Id, user => user.DisplayName);
    }

    private async Task<Guid?> ResolveLocationIdAsync(Guid? locationId, CancellationToken cancellationToken)
    {
        if (locationId is null)
        {
            return null;
        }

        if (schedulingRepository is null)
        {
            return locationId;
        }

        var location = await schedulingRepository.GetLocationByIdAsync(locationId.Value, cancellationToken)
            ?? throw new ArgumentException("Wybrana lokalizacja nie istnieje.");

        if (!location.IsActive)
        {
            throw new ArgumentException("Wybrana lokalizacja jest nieaktywna.");
        }

        return location.Id;
    }

    private async Task<Guid?> ResolveInstructorIdAsync(Guid? instructorId, CancellationToken cancellationToken)
    {
        if (instructorId is null)
        {
            return null;
        }

        var instructor = await userRepository.GetByIdAsync(instructorId.Value, cancellationToken)
            ?? throw new ArgumentException("Wybrany instruktor zastepczy nie istnieje.");

        if (instructor.Role != UserRole.Instructor && instructor.Role != UserRole.Admin)
        {
            throw new ArgumentException("Wybrany użytkownik nie jest instruktorem.");
        }

        return instructor.Id;
    }

    private static int? NormalizeCapacity(int? capacity)
    {
        if (capacity is null)
        {
            return null;
        }

        if (capacity <= 0)
        {
            throw new ArgumentException("Limit miejsc musi być większy od zera.");
        }

        return capacity;
    }

    private static bool IsWithinCapacity(int index, int? capacity) => capacity is null || index < capacity;

    private static string? NormalizeMeetingUrl(string? meetingUrl) =>
        WebLink.Normalize(meetingUrl, "Link do spotkania musi być pełnym adresem http(s).");

    public async Task<ScheduledSessionDto?> SetSessionStatusAsync(
        Guid groupId,
        Guid sessionId,
        SetSessionStatusDto dto,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return null;
        }

        var status = GroupMapping.ParseSessionStatus(dto.Status)
            ?? throw new ArgumentException("Nieznany status terminu.");

        // `InProgress` ustawia się wyłącznie przez rozpoczęcie zajęć w kokpicie - ręczne
        // wpisanie tego stanu rozjechałoby timery i listę obecności.
        if (status == ScheduledSessionStatus.InProgress)
        {
            throw new ArgumentException("Status „w toku” ustawia się przez rozpoczęcie zajęć.");
        }

        var previous = session.Status;
        session.Status = status;

        if (status.CountsAsHeld())
        {
            session.CompletedAt ??= DateTimeOffset.UtcNow;
        }

        await groupRepository.UpdateSessionAsync(session, cancellationToken);
        await RecordSessionChangeAsync(
            session,
            status.IsCancelled() ? SessionChangeType.Cancelled : SessionChangeType.StatusChanged,
            previousScheduledAt: null,
            newScheduledAt: null,
            dto.Reason,
            $"{previous.Label()} → {status.Label()}",
            actingUserId,
            dto.GuardiansNotified,
            cancellationToken);

        var lessonsById = await LessonMapAsync(cancellationToken);
        var locationNames = await LocationNameMapAsync(cancellationToken);
        var instructorNames = await InstructorNameMapAsync(cancellationToken);
        return GroupMapping.ToSessionDto(session, group, lessonsById, locationNames, instructorNames);
    }

    public IReadOnlyList<SessionStatusOptionDto> GetSessionStatusOptions() => GroupMapping.SessionStatusOptions;

    public async Task<IReadOnlyList<SessionChangeDto>?> GetSessionHistoryAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);

        if (group is null)
        {
            return null;
        }

        var changes = await groupRepository.ListSessionChangesAsync(groupId, cancellationToken);
        var sequenceBySession = group.Sessions.ToDictionary(session => session.Id, session => session.SequenceNumber);
        var userNames = (await userRepository.ListAsync(cancellationToken))
            .ToDictionary(user => user.Id, user => user.DisplayName);

        return changes
            .Select(change => new SessionChangeDto(
                change.Id,
                change.ScheduledSessionId,
                sequenceBySession.GetValueOrDefault(change.ScheduledSessionId),
                change.ChangeType.ToString().ToLowerInvariant(),
                ChangeTypeLabel(change.ChangeType),
                change.PreviousScheduledAt,
                change.NewScheduledAt,
                change.Reason,
                change.Details,
                change.ChangedByUserId,
                change.ChangedByUserId is Guid userId ? userNames.GetValueOrDefault(userId) : null,
                change.GuardiansNotified,
                change.ChangedAt))
            .ToList();
    }

    /// <summary>
    /// Wysyła opiekunom informację o przełożeniu albo odwołaniu i mówi, czy cokolwiek wyszło.
    ///
    /// Błąd wysyłki nie może wywrócić samej operacji: lepiej odwołać zajęcia bez maila niż
    /// nie odwołać wcale. Nieudana próba i tak zostaje w dzienniku wysyłek, a historia zmian
    /// zapisze wtedy „bez powiadomienia” — czyli prawdę.
    /// </summary>
    private async Task<bool> NotifyScheduleChangeAsync(
        Guid sessionId,
        DateTimeOffset? previousScheduledAt,
        string? reason,
        bool cancelled,
        CancellationToken cancellationToken)
    {
        if (notificationService is null)
        {
            return false;
        }

        try
        {
            var sent = await notificationService.NotifySessionRescheduledAsync(
                sessionId, previousScheduledAt, reason, cancelled, cancellationToken);

            return sent > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Dopisuje wpis do historii zmian terminu. Błąd zapisu historii nie może wywrócić
    /// samej operacji - lepiej przełożyć zajęcia bez wpisu niż nie przełożyć wcale.</summary>
    private async Task RecordSessionChangeAsync(
        ScheduledSession session,
        SessionChangeType changeType,
        DateTimeOffset? previousScheduledAt,
        DateTimeOffset? newScheduledAt,
        string? reason,
        string? details,
        Guid? actingUserId,
        bool guardiansNotified,
        CancellationToken cancellationToken)
    {
        await groupRepository.AddSessionChangeAsync(
            new SessionChangeLog
            {
                ScheduledSessionId = session.Id,
                GroupId = session.GroupId,
                ChangeType = changeType,
                PreviousScheduledAt = previousScheduledAt,
                NewScheduledAt = newScheduledAt,
                Reason = Trim(reason, 1000),
                Details = Trim(details, 1000),
                ChangedByUserId = actingUserId,
                GuardiansNotified = guardiansNotified
            },
            cancellationToken);
    }

    private static string? Trim(string? value, int maxLength)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed[..Math.Min(trimmed.Length, maxLength)];
    }

    private static string ChangeTypeLabel(SessionChangeType changeType) => changeType switch
    {
        SessionChangeType.Created => "Dodano termin",
        SessionChangeType.Rescheduled => "Przełożono",
        SessionChangeType.Cancelled => "Odwołano",
        SessionChangeType.SubstituteChanged => "Zmiana zastępstwa",
        SessionChangeType.LinksChanged => "Zmiana linków",
        SessionChangeType.StatusChanged => "Zmiana statusu",
        _ => changeType.ToString()
    };

    private static string? NormalizeRecordingUrl(string? recordingUrl) =>
        WebLink.Normalize(recordingUrl, "Link do nagrania musi być pełnym adresem http(s).");

    private static void NormalizeEnrollmentStatuses(Group group)
    {
        var enrolledCount = 0;

        foreach (var enrollment in group.Enrollments.OrderBy(enrollment => enrollment.EnrolledAt))
        {
            if (group.Capacity is not null && enrolledCount >= group.Capacity)
            {
                enrollment.Status = EnrollmentStatus.Waitlisted;
                continue;
            }

            enrollment.Status = EnrollmentStatus.Enrolled;
            enrolledCount++;
        }
    }

    private async Task<IReadOnlySet<DateOnly>> HolidayDatesAsync(CancellationToken cancellationToken)
    {
        if (schedulingRepository is null)
        {
            return new HashSet<DateOnly>();
        }

        var holidays = await schedulingRepository.ListHolidaysAsync(cancellationToken);
        return holidays.Select(holiday => holiday.Date).ToHashSet();
    }

    private async Task EnsureNotHolidayAsync(DateTimeOffset scheduledAt, CancellationToken cancellationToken)
    {
        var holidays = await HolidayDatesAsync(cancellationToken);
        var date = SchoolTime.LocalDate(scheduledAt);

        if (holidays.Contains(date))
        {
            throw new ArgumentException("Termin wypada w dniu wolnym.");
        }
    }

    private async Task EnsureNoSchedulingConflictsAsync(
        Group targetGroup,
        IReadOnlyList<ScheduledSession> targetSessions,
        IReadOnlySet<Guid> ignoredSessionIds,
        CancellationToken cancellationToken)
    {
        var allGroups = await groupRepository.ListAsync(cancellationToken);

        foreach (var targetSession in targetSessions)
        {
            if (targetSession.Status.IsCancelled())
            {
                continue;
            }

            var targetLocationId = targetSession.LocationId ?? targetGroup.LocationId;

            foreach (var existingGroup in allGroups)
            {
                foreach (var existingSession in existingGroup.Sessions)
                {
                    if (ignoredSessionIds.Contains(existingSession.Id)
                        || existingSession.Status.IsCancelled()
                        || existingSession.ScheduledAt.UtcDateTime != targetSession.ScheduledAt.UtcDateTime)
                    {
                        continue;
                    }

                    var targetInstructorIds = new[] { targetGroup.InstructorId, targetSession.SubstituteInstructorId }
                        .Where(id => id is not null)
                        .Select(id => id!.Value)
                        .ToHashSet();
                    var existingInstructorIds = new[] { existingGroup.InstructorId, existingSession.SubstituteInstructorId }
                        .Where(id => id is not null)
                        .Select(id => id!.Value)
                        .ToHashSet();

                    if (targetInstructorIds.Overlaps(existingInstructorIds))
                    {
                        throw new SchedulingConflictException(
                            $"Instruktor ma już termin w tym czasie: {existingGroup.Name}.");
                    }

                    var existingLocationId = existingSession.LocationId ?? existingGroup.LocationId;

                    if (targetLocationId is not null && existingLocationId == targetLocationId)
                    {
                        throw new SchedulingConflictException(
                            $"Sala jest już zajęta w tym czasie: {existingGroup.Name}.");
                    }
                }
            }
        }
    }

    private async Task<IReadOnlyList<Guid>> ResolveLessonIdsAsync(CreateGroupDto dto, CancellationToken cancellationToken)
    {
        if (dto.CourseId is null)
        {
            return (dto.LessonIds ?? [])
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();
        }

        var course = courseRepository is null
            ? null
            : await courseRepository.GetByIdAsync(dto.CourseId.Value, cancellationToken);

        if (course is null)
        {
            throw new ArgumentException("Wybrany kurs nie istnieje.");
        }

        return course.Lessons
            .OrderBy(lesson => lesson.Order)
            .Select(lesson => lesson.LessonId)
            .ToList();
    }

    /// <summary>
    /// Termin przesunięty o pełne tygodnie z zachowaniem godziny ściennej.
    ///
    /// Zero tygodni **nie jest skrótem do zwrócenia wejścia**: przeglądarka przysyła termin
    /// z offsetem `Z`, więc bez przeliczenia pierwsze zajęcia serii zostawały w UTC, a każde
    /// następne dostawały offset warszawski. Ta sama seria miała wtedy dwie różne godziny
    /// w tekście e-maila i w eksportach.
    /// </summary>
    private static DateTimeOffset AddWeeksPreservingLocalTime(DateTimeOffset start, int weeks) =>
        AddDaysPreservingLocalTime(start, 7 * weeks);

    private static DateTimeOffset MovePastHoliday(DateTimeOffset scheduledAt, IReadOnlySet<DateOnly> holidays)
    {
        var current = scheduledAt;

        while (holidays.Contains(SchoolTime.LocalDate(current)))
        {
            current = AddDaysPreservingLocalTime(current, 1);
        }

        return current;
    }

    private static DateTimeOffset AddDaysPreservingLocalTime(DateTimeOffset start, int days) =>
        SchoolTime.FromWallClock(SchoolTime.ToSchoolTime(start).DateTime.AddDays(days));

    private static void EnsureCsvFormat(string? format)
    {
        if (string.IsNullOrWhiteSpace(format) || string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new ArgumentException("Obsługiwany format eksportu: csv.");
    }

    private static byte[] ToCsvBytes(IEnumerable<IReadOnlyList<string>> rows)
    {
        var builder = new StringBuilder();

        foreach (var row in rows)
        {
            builder.AppendLine(string.Join(';', row.Select(EscapeCsv)));
        }

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(builder.ToString())).ToArray();
    }

    private static string EscapeCsv(string value)
    {
        // Ochrona przed CSV injection: Excel/LibreOffice traktują komórkę zaczynającą się od
        // = + - @ (albo tabulatora / CR) jako formułę. Poprzedzamy ją apostrofem, żeby dane
        // wpisane w aplikacji nie wykonały się po otwarciu eksportu.
        var safe = value.Length > 0 && value[0] is '=' or '+' or '-' or '@' or '\t' or '\r'
            ? $"'{value}"
            : value;

        if (safe.Contains('"') || safe.Contains(';') || safe.Contains('\n') || safe.Contains('\r'))
        {
            return $"\"{safe.Replace("\"", "\"\"")}\"";
        }

        return safe;
    }

    private static string FileSafe(string value)
    {
        var invalid = Path.GetInvalidFileNameChars().ToHashSet();
        var normalized = new string(value
            .Select(character => invalid.Contains(character) || char.IsWhiteSpace(character) ? '-' : char.ToLowerInvariant(character))
            .ToArray()).Trim('-');

        return string.IsNullOrWhiteSpace(normalized) ? "grupa" : normalized;
    }

    private static GroupDetailsDto ToDetails(
        Group group,
        string instructorEmail,
        string instructorName,
        IReadOnlyDictionary<Guid, Lesson> lessonsById,
        IReadOnlyDictionary<Guid, string> locationNames,
        IReadOnlyDictionary<Guid, string> instructorNames,
        IReadOnlyList<Participant> participants)
    {
        var locationName = group.LocationId is Guid locationId ? locationNames.GetValueOrDefault(locationId) : null;

        return new GroupDetailsDto(
            group.Id,
            group.Name,
            group.InstructorId,
            instructorEmail,
            instructorName,
            group.Status.ToString().ToLowerInvariant(),
            group.CourseId,
            group.LocationId,
            locationName,
            group.Capacity,
            group.MeetingUrl,
            participants
                .OrderBy(participant => participant.LastName)
                .ThenBy(participant => participant.FirstName)
                .Select(participant =>
                {
                    var enrollment = group.Enrollments.FirstOrDefault(item => item.ParticipantId == participant.Id);
                    return GroupMapping.ToParticipantDto(participant, enrollment?.Status ?? EnrollmentStatus.Enrolled);
                })
                .ToList(),
            group.Sessions
                .OrderBy(session => session.SequenceNumber)
                .Select(session => GroupMapping.ToSessionDto(session, group, lessonsById, locationNames, instructorNames))
                .ToList());
    }
}
