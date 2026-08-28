using LessonRunner.Application.Auth;
using LessonRunner.Application.Billing;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Participants;
using LessonRunner.Application.Scheduling;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Dashboard;

public sealed class DashboardService(
    IGroupRepository groupRepository,
    IParticipantRepository participantRepository,
    ILessonRepository lessonRepository,
    IUserRepository userRepository,
    ISchedulingRepository? schedulingRepository = null,
    // Opcjonalne, żeby starsze testy budujące serwis ręcznie nadal się kompilowały.
    // Brak repozytorium oznacza listę bez spraw rozliczeniowych, a nie wywrócony pulpit.
    IBillingRepository? billingRepository = null) : IDashboardService
{
    public async Task<DashboardDto> GetAsync(CancellationToken cancellationToken)
    {
        var groups = await groupRepository.ListAsync(cancellationToken);
        var participants = await participantRepository.ListAsync(cancellationToken);
        var lessons = await lessonRepository.ListTitlesAsync(cancellationToken);
        var users = await userRepository.ListAsync(cancellationToken);
        var instructorNames = users.ToDictionary(user => user.Id, user => user.DisplayName);
        var locationNames = schedulingRepository is null
            ? new Dictionary<Guid, string>()
            : (await schedulingRepository.ListLocationsAsync(cancellationToken)).ToDictionary(location => location.Id, location => location.Name);

        var activeGroups = groups.Where(group => group.Status == GroupStatus.Active).ToList();
        var completedSessions = activeGroups.SelectMany(group => group.Sessions).Where(session => session.Status.CountsAsHeld()).ToList();
        var completedAttendance = completedSessions.SelectMany(session => session.Attendance).ToList();
        var attendancePercent = completedAttendance.Count == 0
            ? 0
            : (int)Math.Round(100.0 * completedAttendance.Count(record => record.Present) / completedAttendance.Count);
        var now = DateTimeOffset.UtcNow;
        var upcoming = activeGroups
            .SelectMany(group => group.Sessions.Select(session => (group, session)))
            .Where(item => item.session.Status.IsActive())
            .Where(item => item.session.ScheduledAt >= now.AddHours(-2))
            .OrderBy(item => item.session.ScheduledAt)
            .ToList();
        var enrolledCount = activeGroups.Sum(group => group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Enrolled));
        var waitlistedCount = activeGroups.Sum(group => group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Waitlisted));
        var groupsAtCapacity = activeGroups.Count(group =>
            group.Capacity is int capacity
            && group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Enrolled) >= capacity);
        var pendingMakeups = activeGroups
            .SelectMany(group => group.Sessions)
            .SelectMany(session => session.Attendance)
            .Count(record => record.MakeupRequired && !IsMakeupCompleted(record, activeGroups));

        var kpis = new DashboardKpiDto(
            participants.Count(participant => !participant.IsArchived),
            activeGroups.Count,
            enrolledCount,
            waitlistedCount,
            groupsAtCapacity,
            upcoming.Count,
            completedSessions.Count,
            attendancePercent,
            pendingMakeups);

        var upcomingDtos = upcoming
            .Take(8)
            .Select(item =>
            {
                var locationId = item.session.LocationId ?? item.group.LocationId;
                return new DashboardUpcomingSessionDto(
                    item.session.Id,
                    item.group.Id,
                    item.group.Name,
                    item.session.LessonId is Guid lessonId && lessons.TryGetValue(lessonId, out var lessonTitle) ? lessonTitle : null,
                    item.session.ScheduledAt,
                    instructorNames.GetValueOrDefault(item.session.SubstituteInstructorId ?? item.group.InstructorId, "(nieznany)"),
                    locationId is Guid id ? locationNames.GetValueOrDefault(id) : null);
            })
            .ToList();

        var instructorLoads = users
            .Where(user => user.Role is UserRole.Instructor or UserRole.Admin)
            .Select(user => new DashboardInstructorLoadDto(
                user.Id,
                user.DisplayName,
                activeGroups.Count(group => group.InstructorId == user.Id),
                upcoming.Count(item => item.group.InstructorId == user.Id),
                upcoming.Count(item => item.session.SubstituteInstructorId == user.Id)))
            .Where(load => load.ActiveGroups > 0 || load.UpcomingSessions > 0 || load.SubstituteSessions > 0)
            .OrderByDescending(load => load.UpcomingSessions + load.SubstituteSessions)
            .ThenBy(load => load.InstructorName)
            .Take(8)
            .ToList();

        var groupFill = activeGroups
            .Select(group =>
            {
                var enrolled = group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Enrolled);
                var waitlisted = group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Waitlisted);
                var fillPercent = group.Capacity is int capacity && capacity > 0
                    ? Math.Min(100, (int)Math.Round(100.0 * enrolled / capacity))
                    : 0;

                return new DashboardGroupFillDto(group.Id, group.Name, enrolled, waitlisted, group.Capacity, fillPercent);
            })
            .OrderByDescending(group => group.Capacity is null ? -1 : group.FillPercent)
            .ThenByDescending(group => group.Waitlisted)
            .ThenBy(group => group.GroupName)
            .Take(10)
            .ToList();

        var attention = await BuildAttentionAsync(activeGroups, instructorNames, participants.Count, cancellationToken);

        return new DashboardDto(kpis, upcomingDtos, instructorLoads, groupFill, attention);
    }

    /// <summary>
    /// Lista spraw do załatwienia, posortowana od najpilniejszych.
    ///
    /// Świadomie krótka (maksymalnie 12 pozycji): lista, której nie da się przejrzeć
    /// jednym spojrzeniem, przestaje być listą zadań i zamienia się w kolejny raport.
    /// </summary>
    private async Task<IReadOnlyList<DashboardAttentionDto>> BuildAttentionAsync(
        IReadOnlyList<Group> activeGroups,
        IReadOnlyDictionary<Guid, string> instructorNames,
        int participantCount,
        CancellationToken cancellationToken)
    {
        var items = new List<DashboardAttentionDto>();
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);
        var now = DateTimeOffset.UtcNow;

        // 1. Terminy bez przypisanej lekcji - instruktor stanie przed grupą bez scenariusza.
        foreach (var item in activeGroups
            .SelectMany(group => group.Sessions.Select(session => (group, session)))
            .Where(item => item.session.Status.IsUpcoming() && item.session.LessonId is null)
            .Where(item => item.session.ScheduledAt <= now.AddDays(14))
            .OrderBy(item => item.session.ScheduledAt)
            .Take(5))
        {
            items.Add(new DashboardAttentionDto(
                "nolesson",
                $"{item.group.Name}: termin bez lekcji",
                $"{item.session.ScheduledAt:dd.MM, HH:mm} — nie przypisano konspektu.",
                item.session.ScheduledAt <= now.AddDays(2) ? "danger" : "warning",
                $"/admin/groups/{item.group.Id}"));
        }

        // 2. Grupy z instruktorem, którego nie ma już na liście kont.
        foreach (var group in activeGroups.Where(group => !instructorNames.ContainsKey(group.InstructorId)).Take(3))
        {
            items.Add(new DashboardAttentionDto(
                "noinstructor",
                $"{group.Name}: brak prowadzącego",
                "Konto przypisanego instruktora nie istnieje albo zostało usunięte.",
                "danger",
                $"/admin/groups/{group.Id}"));
        }

        // 3. Dzieci z frekwencją poniżej połowy - sygnał do rozmowy z rodzicem,
        //    zanim skończy się kurs i zostanie sama reklamacja.
        foreach (var group in activeGroups)
        {
            var held = group.Sessions.Where(session => session.Status.CountsAsHeld()).ToList();

            if (held.Count < 3)
            {
                continue;
            }

            foreach (var enrollment in group.Enrollments.Where(item => item.Status == EnrollmentStatus.Enrolled))
            {
                var attended = held.Count(session => session.Attendance
                    .Any(record => record.ParticipantId == enrollment.ParticipantId && record.Present));
                var percent = (int)Math.Round(100.0 * attended / held.Count);

                if (percent < 50)
                {
                    items.Add(new DashboardAttentionDto(
                        "lowattendance",
                        $"{group.Name}: niska frekwencja dziecka",
                        $"Obecność na {attended} z {held.Count} zajęć ({percent}%).",
                        "warning",
                        $"/admin/groups/{group.Id}"));
                }
            }
        }

        // 4. Listy rezerwowe przy grupach, w których zwolniło się miejsce.
        foreach (var group in activeGroups.Where(group =>
            group.Capacity is int capacity
            && group.Enrollments.Count(item => item.Status == EnrollmentStatus.Waitlisted) > 0
            && group.Enrollments.Count(item => item.Status == EnrollmentStatus.Enrolled) < capacity))
        {
            items.Add(new DashboardAttentionDto(
                "waitlist",
                $"{group.Name}: wolne miejsce, ktoś czeka",
                "W grupie jest miejsce, a na liście rezerwowej ktoś oczekuje na awans.",
                "warning",
                $"/admin/groups/{group.Id}"));
        }

        if (billingRepository is not null)
        {
            // 5. Faktury po terminie. Liczymy z daty, a nie ze statusu w bazie - status
            //    „Overdue” zmienia się dopiero przy jakiejś operacji na fakturze.
            var overdue = (await billingRepository.ListInvoicesAsync(cancellationToken))
                .Where(invoice => invoice.Status is Domain.Billing.InvoiceStatus.Open
                    or Domain.Billing.InvoiceStatus.Overdue)
                .Where(invoice => invoice.DueDate < today)
                .OrderBy(invoice => invoice.DueDate)
                .Take(5)
                .ToList();

            foreach (var invoice in overdue)
            {
                items.Add(new DashboardAttentionDto(
                    "overdue",
                    $"Faktura {invoice.Number} po terminie",
                    $"{invoice.AmountCents / 100m:0.00} {invoice.Currency}, termin minął {invoice.DueDate:dd.MM.yyyy}.",
                    "danger",
                    "/admin/billing"));
            }

            // 6. Kredyty tracące ważność w ciągu miesiąca - przeterminowany kredyt to
            //    najczęstsza przyczyna rozmowy „przecież nam się należało”.
            var expiring = (await billingRepository.ListCreditsAsync(cancellationToken))
                .Where(credit => credit.IsUsable(today))
                .Where(credit => credit.ExpiresAt is DateOnly expires && expires <= today.AddDays(30))
                .ToList();

            if (expiring.Count > 0)
            {
                items.Add(new DashboardAttentionDto(
                    "expiringcredit",
                    $"Kredyty tracą ważność: {expiring.Count}",
                    "Ustal z rodzicami termin odrobienia albo przedłuż ważność.",
                    "warning",
                    "/admin/billing"));
            }
        }

        // Uczestnicy bez grupy pojawiają się tylko wtedy, gdy w ogóle ktoś jest w bazie -
        // przy pustej szkole to nie jest sprawa do załatwienia, tylko stan początkowy.
        if (participantCount > 0 && activeGroups.Count == 0)
        {
            items.Add(new DashboardAttentionDto(
                "noinstructor",
                "Brak aktywnych grup",
                "W bazie są uczestnicy, ale żadna grupa nie jest aktywna.",
                "warning",
                "/admin/groups/new"));
        }

        return items
            .OrderBy(item => item.Severity == "danger" ? 0 : 1)
            .Take(12)
            .ToList();
    }

    private static bool IsMakeupCompleted(AttendanceRecord record, IReadOnlyList<Group> groups)
    {
        if (!record.MakeupRequired || record.MakeupSessionId is not Guid makeupSessionId)
        {
            return false;
        }

        var makeupSession = groups
            .SelectMany(group => group.Sessions)
            .FirstOrDefault(session => session.Id == makeupSessionId);

        return makeupSession?.Status.CountsAsHeld() == true
            && makeupSession.Attendance.Any(item => item.ParticipantId == record.ParticipantId && item.Present);
    }
}
