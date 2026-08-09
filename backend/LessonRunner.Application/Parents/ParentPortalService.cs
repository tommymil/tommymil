using LessonRunner.Application.Auth;
using LessonRunner.Application.Billing;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Participants;
using LessonRunner.Application.Progress;
using LessonRunner.Application.Scheduling;
using LessonRunner.Domain.Billing;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Parents;
using LessonRunner.Domain.Progress;
using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Parents;

public sealed class ParentPortalService(
    IParentPortalRepository parentRepository,
    IUserRepository userRepository,
    IParticipantRepository participantRepository,
    IGroupRepository groupRepository,
    ILessonRepository lessonRepository,
    IBillingRepository billingRepository,
    // Opcjonalne, żeby starsze testy budujące serwis ręcznie nadal się kompilowały. Brak
    // repozytorium oznacza pusty dorobek, a nie wywrócony portal.
    IProgressRepository? progressRepository = null) : IParentPortalService
{
    public async Task<ParentPortalDto> GetPortalAsync(Guid parentUserId, CancellationToken cancellationToken)
    {
        var participantIds = (await parentRepository.ListByParentAsync(parentUserId, cancellationToken))
            .Select(link => link.ParticipantId)
            .Distinct()
            .ToHashSet();

        if (participantIds.Count == 0)
        {
            return new ParentPortalDto([], [], [], [], [], [], EmptySummary(), [], [], []);
        }

        var participants = await participantRepository.GetByIdsAsync(participantIds.ToList(), cancellationToken);
        var groups = await groupRepository.ListAsync(cancellationToken);
        var lessons = await lessonRepository.ListAsync(cancellationToken);
        var lessonTitles = lessons.ToDictionary(lesson => lesson.Id, lesson => lesson.Title);
        var lessonsById = lessons.ToDictionary(lesson => lesson.Id);
        var invoices = await billingRepository.ListInvoicesAsync(cancellationToken);
        var credits = await billingRepository.ListCreditsAsync(cancellationToken);
        var instructorNames = (await userRepository.ListAsync(cancellationToken))
            .ToDictionary(user => user.Id, user => user.DisplayName);

        var groupsByParticipant = BuildGroupMap(groups, participantIds);
        var children = participants
            .OrderBy(participant => participant.LastName)
            .ThenBy(participant => participant.FirstName)
            .Select(participant => new ParentChildDto(
                participant.Id,
                participant.FirstName,
                participant.LastName,
                groupsByParticipant.GetValueOrDefault(participant.Id, [])))
            .ToList();

        var schedule = groups
            .SelectMany(group => group.Sessions.Select(session => (group, session)))
            .Where(item => item.group.Enrollments.Any(enrollment =>
                participantIds.Contains(enrollment.ParticipantId) && enrollment.Status == EnrollmentStatus.Enrolled))
            .Where(item => item.session.Status.IsActive())
            .OrderBy(item => item.session.ScheduledAt)
            .Take(30)
            .Select(item => new ParentScheduleItemDto(
                item.session.Id,
                item.group.Id,
                item.group.Name,
                item.session.LessonId is Guid lessonId && lessonTitles.TryGetValue(lessonId, out var title) ? title : null,
                item.session.ScheduledAt,
                StatusName(item.session.Status),
                StatusLabel(item.session.Status),
                // Link terminu wygrywa z linkiem grupy - zastępstwo bywa prowadzone
                // w innym pokoju niż zwykłe zajęcia.
                string.IsNullOrWhiteSpace(item.session.MeetingUrl) ? item.group.MeetingUrl : item.session.MeetingUrl,
                SessionChildren(item.group, item.session, participantIds, participants),
                item.session.SequenceNumber,
                item.group.Sessions.Count,
                // Zastępstwo wygrywa z instruktorem grupy - rodzic ma wiedzieć, kto
                // faktycznie poprowadzi te zajęcia.
                instructorNames.GetValueOrDefault(
                    item.session.SubstituteInstructorId ?? item.group.InstructorId)))
            .ToList();

        var attendance = groups
            .Where(group => group.Enrollments.Any(enrollment => participantIds.Contains(enrollment.ParticipantId)))
            .SelectMany(group => participantIds
                .Where(participantId => group.Enrollments.Any(enrollment => enrollment.ParticipantId == participantId))
                .Select(participantId => ToAttendance(group, participantId)))
            .Where(item => item.HeldCount > 0)
            .OrderBy(item => item.GroupName)
            .ToList();

        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        var invoiceDtos = invoices
            .Where(invoice => participantIds.Contains(invoice.ParticipantId))
            .OrderByDescending(invoice => invoice.IssuedAt)
            .Select(invoice => new ParentInvoiceDto(
                invoice.Id,
                invoice.Number,
                groups.FirstOrDefault(group => group.Id == invoice.GroupId)?.Name ?? "(nieznana)",
                invoice.AmountCents,
                invoice.Currency,
                invoice.Status.ToString().ToLowerInvariant(),
                InvoiceStatusLabel(invoice.Status.ToString()),
                invoice.DueDate,
                invoice.PaidAt,
                invoice.ParticipantId,
                // Zaległość liczymy przy odczycie. Status „Overdue" w bazie zmienia się
                // dopiero przy jakiejś operacji na fakturze, więc dokument po terminie
                // potrafiłby tygodniami pokazywać się rodzicowi jako „Do zapłaty".
                IsUnpaid(invoice.Status) && invoice.DueDate < today))
            .ToList();

        // Materiały udostępniamy dopiero po zakończonych zajęciach - przed lekcją pliki projektu
        // psułyby zabawę, a rodzic i tak nie ma co z nimi zrobić.
        var materials = groups
            .Where(group => group.Enrollments.Any(enrollment => participantIds.Contains(enrollment.ParticipantId)))
            .SelectMany(group => group.Sessions.Select(session => (group, session)))
            .Where(item => item.session.Status.CountsAsHeld())
            .OrderByDescending(item => item.session.ScheduledAt)
            .Take(30)
            .Select(item =>
            {
                var lesson = item.session.LessonId is Guid lessonId && lessonsById.TryGetValue(lessonId, out var found)
                    ? found
                    : null;

                var files = ProjectFiles(lesson)
                    .Select(file => new ParentMaterialFileDto(
                        file.Label,
                        file.FileName,
                        file.SizeBytes,
                        $"/download/lesson-files/{file.DownloadToken}"))
                    .ToList();

                return new ParentMaterialDto(
                    item.session.Id,
                    item.group.Id,
                    item.group.Name,
                    lesson?.Title,
                    item.session.ScheduledAt,
                    files,
                    item.session.RecordingUrl,
                    item.group.Enrollments
                        .Where(enrollment => participantIds.Contains(enrollment.ParticipantId))
                        .Select(enrollment => enrollment.ParticipantId)
                        .Distinct()
                        .ToList(),
                    item.session.ParentSummary);
            })
            .Where(material => material.Files.Count > 0
                || material.RecordingUrl is not null
                || material.Summary is not null)
            .ToList();

        var progress = await BuildProgressAsync(participants, cancellationToken);

        var childNames = participants.ToDictionary(
            participant => participant.Id,
            participant => $"{participant.FirstName} {participant.LastName}");

        // Kredyty pokazujemy wyłącznie te do wykorzystania. Kredyt wycofany albo
        // przeterminowany na ekranie rodzica byłby obietnicą bez pokrycia.
        var creditDtos = credits
            .Where(credit => participantIds.Contains(credit.ParticipantId) && credit.IsUsable(today))
            .OrderBy(credit => credit.ExpiresAt ?? DateOnly.MaxValue)
            .Select(credit => new ParentCreditDto(
                credit.Id,
                credit.ParticipantId,
                childNames.GetValueOrDefault(credit.ParticipantId, ""),
                credit.GroupId is Guid groupId
                    ? groups.FirstOrDefault(group => group.Id == groupId)?.Name
                    : null,
                credit.Reason,
                credit.IssuedAt,
                credit.ExpiresAt))
            .ToList();

        var courseProgress = BuildCourseProgress(groups, participantIds, lessonTitles);
        var consents = participants
            .OrderBy(participant => participant.FirstName)
            .Select(participant => new ParentConsentDto(
                participant.Id,
                $"{participant.FirstName} {participant.LastName}",
                participant.DataProcessingConsentAt is not null,
                participant.DataProcessingConsentAt,
                participant.ImageConsentAt is not null,
                participant.ImageConsentAt))
            .ToList();

        var summary = BuildSummary(invoiceDtos, creditDtos, attendance, schedule, today);

        return new ParentPortalDto(
            children,
            schedule,
            attendance,
            invoiceDtos,
            materials,
            progress,
            summary,
            creditDtos,
            courseProgress,
            consents);
    }

    /// <summary>
    /// Cztery liczby, po które rodzic wchodzi do portalu.
    ///
    /// „Do zapłaty" liczymy z faktur nieopłaconych i nieanulowanych; „zaległe" to ich
    /// podzbiór po terminie. Walutę bierzemy z pierwszej niezapłaconej faktury —
    /// szkoła rozlicza się w jednej walucie, a sumowanie różnych dałoby liczbę,
    /// która nic nie znaczy.
    /// </summary>
    private static ParentSummaryDto BuildSummary(
        IReadOnlyList<ParentInvoiceDto> invoices,
        IReadOnlyList<ParentCreditDto> credits,
        IReadOnlyList<ParentAttendanceItemDto> attendance,
        IReadOnlyList<ParentScheduleItemDto> schedule,
        DateOnly today)
    {
        var unpaid = invoices.Where(invoice => invoice.Status is "open" or "overdue" or "draft").ToList();
        var held = attendance.Sum(item => item.HeldCount);
        var present = attendance.Sum(item => item.PresentCount);

        return new ParentSummaryDto(
            unpaid.Sum(invoice => invoice.AmountCents),
            unpaid.Where(invoice => invoice.DueDate < today).Sum(invoice => invoice.AmountCents),
            unpaid.FirstOrDefault()?.Currency ?? "PLN",
            // Rzutowanie jest jawne, bo `null` i `DateOnly` same z siebie nie mają wspólnego
            // typu - bez niego kompilator opiera się wyłącznie na typie docelowym parametru.
            unpaid.Count == 0 ? null : (DateOnly?)unpaid.Min(invoice => invoice.DueDate),
            credits.Count,
            held == 0 ? 0 : (int)Math.Round(100.0 * present / held),
            schedule.FirstOrDefault());
    }

    private static ParentSummaryDto EmptySummary() =>
        new(0, 0, "PLN", null, 0, 0, null);

    /// <summary>
    /// Postęp w kursie: ile lekcji za dzieckiem, ile przed nim.
    ///
    /// Liczymy po terminach, które faktycznie się odbyły (`CountsAsHeld`), a nie po
    /// samej dacie — zajęcia odwołane albo przerwane technicznie nie są zrealizowanym
    /// materiałem, choć data już minęła.
    /// </summary>
    private static IReadOnlyList<ParentCourseProgressDto> BuildCourseProgress(
        IReadOnlyList<Group> groups,
        IReadOnlySet<Guid> participantIds,
        IReadOnlyDictionary<Guid, string> lessonTitles)
    {
        var result = new List<ParentCourseProgressDto>();

        foreach (var group in groups)
        {
            var enrolled = group.Enrollments
                .Where(enrollment => participantIds.Contains(enrollment.ParticipantId)
                    && enrollment.Status == EnrollmentStatus.Enrolled)
                .Select(enrollment => enrollment.ParticipantId)
                .Distinct()
                .ToList();

            if (enrolled.Count == 0)
            {
                continue;
            }

            var completed = group.Sessions.Count(session => session.Status.CountsAsHeld());
            var next = group.Sessions
                .Where(session => session.Status.IsUpcoming())
                .OrderBy(session => session.ScheduledAt)
                .FirstOrDefault();

            foreach (var participantId in enrolled)
            {
                result.Add(new ParentCourseProgressDto(
                    participantId,
                    group.Id,
                    group.Name,
                    completed,
                    group.Sessions.Count,
                    next?.LessonId is Guid lessonId ? lessonTitles.GetValueOrDefault(lessonId) : null,
                    next?.ScheduledAt));
            }
        }

        return result;
    }

    private static bool IsUnpaid(InvoiceStatus status) =>
        status is InvoiceStatus.Open or InvoiceStatus.Overdue or InvoiceStatus.Draft;

    /// <summary>
    /// Zmiana zgody na wizerunek przez samego opiekuna.
    ///
    /// Zgoda na przetwarzanie danych celowo nie jest tu obsługiwana: jest warunkiem
    /// świadczenia usługi, więc jej wycofanie to rozmowa z administracją, a nie
    /// przełącznik. Powiązanie opiekun–dziecko pozostaje jedynym dowodem uprawnienia.
    /// </summary>
    public async Task<bool> UpdateConsentAsync(
        Guid parentUserId,
        UpdateParentConsentDto dto,
        CancellationToken cancellationToken)
    {
        var linked = (await parentRepository.ListByParentAsync(parentUserId, cancellationToken))
            .Any(link => link.ParticipantId == dto.ParticipantId);

        if (!linked)
        {
            return false;
        }

        var participant = await participantRepository.GetByIdAsync(dto.ParticipantId, cancellationToken);

        if (participant is null)
        {
            return false;
        }

        // Datę udzielenia zachowujemy przy ponownym zaznaczeniu tej samej zgody -
        // inaczej każde wejście na ekran przesuwałoby moment jej wyrażenia.
        if (dto.ImageConsent && participant.ImageConsentAt is null)
        {
            participant.ImageConsentAt = DateTimeOffset.UtcNow;
        }
        else if (!dto.ImageConsent)
        {
            participant.ImageConsentAt = null;
        }

        participant.UpdatedAt = DateTimeOffset.UtcNow;
        return await participantRepository.UpdateAsync(participant, cancellationToken);
    }

    /// <summary>
    /// Dorobek dzieci: postępy i projekty.
    ///
    /// Do portalu trafia **wyłącznie to, co napisano z myślą o rodzicu** — poziom samodzielności,
    /// notatka dla rodzica i kolejny krok. Notatka terminu (uwagi instruktora dla zespołu) nie
    /// przechodzi tędy w ogóle: to była najprostsza droga, żeby wewnętrzna uwaga o dziecku
    /// wylądowała na ekranie jego rodzica.
    /// </summary>
    private async Task<IReadOnlyList<ParentChildProgressDto>> BuildProgressAsync(
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken)
    {
        if (progressRepository is null || participants.Count == 0)
        {
            return [];
        }

        var participantIds = participants.Select(participant => participant.Id).ToList();
        var entries = await progressRepository.ListByParticipantsAsync(participantIds, cancellationToken);
        var projects = await progressRepository.ListProjectsByParticipantsAsync(participantIds, cancellationToken);

        return participants
            .Select(participant =>
            {
                var childEntries = entries
                    .Where(entry => entry.ParticipantId == participant.Id)
                    .OrderByDescending(entry => entry.UpdatedAt)
                    .Select(entry => new ParentProgressEntryDto(
                        entry.UpdatedAt,
                        AutonomyLevels.Label(entry.Autonomy),
                        (int)entry.Autonomy,
                        entry.LessonCompleted,
                        entry.NoteForParent,
                        entry.NextStep))
                    .ToList();

                var childProjects = projects
                    .Where(project => project.ParticipantId == participant.Id)
                    .OrderByDescending(project => project.CreatedAt)
                    .Select(project => new ParentProjectDto(
                        project.Id,
                        project.Title,
                        project.Description,
                        project.Submissions
                            .OrderByDescending(submission => submission.Version)
                            .Select(submission => new ParentProjectVersionDto(
                                submission.Version,
                                submission.Url,
                                submission.FileName,
                                submission.DownloadToken is null
                                    ? null
                                    : $"/download/project-files/{submission.DownloadToken}",
                                submission.SubmittedAt,
                                submission.InstructorComment))
                            .ToList()))
                    .ToList();

                return new ParentChildProgressDto(
                    participant.Id,
                    participant.FirstName,
                    participant.LastName,
                    childEntries,
                    childProjects);
            })
            .Where(child => child.Entries.Count > 0 || child.Projects.Count > 0)
            .ToList();
    }

    public async Task<byte[]> ExportScheduleIcsAsync(Guid parentUserId, CancellationToken cancellationToken)
    {
        var portal = await GetPortalAsync(parentUserId, cancellationToken);

        var events = portal.Schedule
            .Select(item => new CalendarEventDto(
                item.SessionId,
                $"{item.GroupName}: {item.LessonTitle ?? "zajęcia"}",
                item.ScheduledAt,
                CalendarExport.DefaultDurationMinutes,
                item.Children is { Count: > 0 }
                    ? string.Join(", ", item.Children.Select(child => $"{child.FirstName} {child.LastName}"))
                    : null,
                Location: null,
                item.MeetingUrl,
                Cancelled: false))
            .ToList();

        return CalendarExport.ToIcs(events, "Zajęcia mojego dziecka");
    }

    /// <summary>Dzieci tego rodzica zapisane na dany termin, wraz z informacją,
    /// czy nieobecność została już zgłoszona.</summary>
    private static IReadOnlyList<ParentSessionChildDto> SessionChildren(
        Group group,
        ScheduledSession session,
        IReadOnlySet<Guid> participantIds,
        IReadOnlyList<Participant> participants)
    {
        var attendanceByParticipant = session.Attendance.ToDictionary(record => record.ParticipantId);

        return group.Enrollments
            .Where(enrollment => participantIds.Contains(enrollment.ParticipantId)
                && enrollment.Status == EnrollmentStatus.Enrolled)
            .Select(enrollment =>
            {
                var participant = participants.FirstOrDefault(item => item.Id == enrollment.ParticipantId);
                var record = attendanceByParticipant.GetValueOrDefault(enrollment.ParticipantId);

                return new ParentSessionChildDto(
                    enrollment.ParticipantId,
                    participant?.FirstName ?? "",
                    participant?.LastName ?? "",
                    record?.Status == AttendanceStatus.ExcusedAbsence,
                    record?.Note);
            })
            .ToList();
    }

    public async Task<bool> ReportAbsenceAsync(
        Guid parentUserId,
        Guid sessionId,
        ReportAbsenceDto dto,
        CancellationToken cancellationToken)
    {
        // Rodzic może zgłosić nieobecność wyłącznie swojego dziecka - powiązanie jest
        // jedynym dowodem na to, że wolno mu ruszać ten rekord.
        var linked = (await parentRepository.ListByParentAsync(parentUserId, cancellationToken))
            .Any(link => link.ParticipantId == dto.ParticipantId);

        if (!linked)
        {
            return false;
        }

        var group = await groupRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return false;
        }

        // Po zajęciach zgłaszanie nieobecności nie ma sensu - wtedy liczy się to,
        // co odhaczył instruktor.
        if (!session.Status.IsUpcoming())
        {
            return false;
        }

        if (!group.Enrollments.Any(enrollment =>
            enrollment.ParticipantId == dto.ParticipantId && enrollment.Status == EnrollmentStatus.Enrolled))
        {
            return false;
        }

        var note = dto.Reason?.Trim();
        note = string.IsNullOrEmpty(note) ? "Nieobecność zgłoszona przez opiekuna." : note[..Math.Min(note.Length, 500)];

        var records = session.Attendance.ToList();
        var existing = records.FirstOrDefault(record => record.ParticipantId == dto.ParticipantId);

        if (existing is null)
        {
            records.Add(new AttendanceRecord
            {
                ScheduledSessionId = sessionId,
                ParticipantId = dto.ParticipantId,
                Status = AttendanceStatus.ExcusedAbsence,
                Note = note
            });
        }
        else
        {
            existing.Status = AttendanceStatus.ExcusedAbsence;
            existing.Note = note;
            existing.MarkedAt = DateTimeOffset.UtcNow;
        }

        await groupRepository.SaveAttendanceAsync(sessionId, records, cancellationToken);
        return true;
    }

    private static IEnumerable<LessonProjectFile> ProjectFiles(Lesson? lesson)
    {
        if (lesson?.ProjectFiles.Starter is not null)
        {
            yield return lesson.ProjectFiles.Starter;
        }

        if (lesson?.ProjectFiles.Final is not null)
        {
            yield return lesson.ProjectFiles.Final;
        }
    }

    public async Task<IReadOnlyList<ParentParticipantLinkDto>> ListLinksAsync(CancellationToken cancellationToken)
    {
        var links = await parentRepository.ListAsync(cancellationToken);
        return links
            .OrderBy(link => link.ParentUserId)
            .ThenBy(link => link.ParticipantId)
            .Select(link => new ParentParticipantLinkDto(
                link.ParentUserId,
                link.ParticipantId,
                link.Relation,
                link.IsPrimaryContact,
                link.ReceivesNotifications))
            .ToList();
    }

    public async Task<ParentParticipantLinkDto> LinkAsync(
        Guid parentUserId,
        Guid participantId,
        string? relation,
        bool isPrimaryContact,
        bool receivesNotifications,
        CancellationToken cancellationToken)
    {
        var parent = await userRepository.GetByIdAsync(parentUserId, cancellationToken)
            ?? throw new ArgumentException("Wybrane konto rodzica nie istnieje.");

        if (parent.Role != UserRole.Parent)
        {
            throw new ArgumentException("Wybrane konto nie ma roli Parent.");
        }

        if (await participantRepository.GetByIdAsync(participantId, cancellationToken) is null)
        {
            throw new ArgumentException("Wybrany uczestnik nie istnieje.");
        }

        var link = new ParentParticipantLink
        {
            ParentUserId = parentUserId,
            ParticipantId = participantId,
            Relation = string.IsNullOrWhiteSpace(relation) ? null : relation.Trim()[..Math.Min(relation.Trim().Length, 60)],
            IsPrimaryContact = isPrimaryContact,
            ReceivesNotifications = receivesNotifications
        };

        // Ponowne powiązanie tej samej pary aktualizuje rolę i preferencje zamiast tworzyć duplikat.
        var existing = await parentRepository.ListByParentAsync(parentUserId, cancellationToken);
        if (existing.Any(item => item.ParticipantId == participantId))
        {
            await parentRepository.UpdateAsync(link, cancellationToken);
        }
        else
        {
            await parentRepository.AddAsync(link, cancellationToken);
        }

        // Kontakt pierwszego wyboru jest jeden - pozostałym opiekunom tego dziecka zdejmujemy flagę.
        if (isPrimaryContact)
        {
            foreach (var other in (await parentRepository.ListByParticipantsAsync([participantId], cancellationToken))
                .Where(item => item.ParentUserId != parentUserId && item.IsPrimaryContact))
            {
                other.IsPrimaryContact = false;
                await parentRepository.UpdateAsync(other, cancellationToken);
            }
        }

        return new ParentParticipantLinkDto(parentUserId, participantId, link.Relation, isPrimaryContact, receivesNotifications);
    }

    public Task<bool> UnlinkAsync(Guid parentUserId, Guid participantId, CancellationToken cancellationToken) =>
        parentRepository.DeleteAsync(parentUserId, participantId, cancellationToken);

    private static Dictionary<Guid, List<ParentChildGroupDto>> BuildGroupMap(IReadOnlyList<Group> groups, IReadOnlySet<Guid> participantIds)
    {
        var map = new Dictionary<Guid, List<ParentChildGroupDto>>();

        foreach (var group in groups)
        {
            foreach (var enrollment in group.Enrollments.Where(enrollment => participantIds.Contains(enrollment.ParticipantId)))
            {
                if (!map.TryGetValue(enrollment.ParticipantId, out var list))
                {
                    list = [];
                    map[enrollment.ParticipantId] = list;
                }

                list.Add(new ParentChildGroupDto(group.Id, group.Name));
            }
        }

        return map;
    }

    private static ParentAttendanceItemDto ToAttendance(Group group, Guid participantId)
    {
        var sessions = group.Sessions
            .Where(session => session.Status.CountsAsHeld())
            .Where(session => session.Attendance.Any(record => record.ParticipantId == participantId))
            .ToList();
        var present = sessions.Count(session =>
            session.Attendance.Any(record => record.ParticipantId == participantId && record.Present));
        var rate = sessions.Count == 0 ? 0 : (int)Math.Round(100.0 * present / sessions.Count);

        return new ParentAttendanceItemDto(group.Id, group.Name, present, sessions.Count, rate, participantId);
    }

    private static string StatusName(ScheduledSessionStatus status) => status.Name();

    private static string StatusLabel(ScheduledSessionStatus status) => status.Label();

    private static string InvoiceStatusLabel(string status) => status.ToLowerInvariant() switch
    {
        "open" => "Do zapłaty",
        "paid" => "Opłacona",
        "overdue" => "Zaległa",
        "cancelled" => "Anulowana",
        _ => status
    };
}
