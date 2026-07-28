using LessonRunner.Application.Auth;
using LessonRunner.Application.Billing;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Participants;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Parents;
using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Parents;

public sealed class ParentPortalService(
    IParentPortalRepository parentRepository,
    IUserRepository userRepository,
    IParticipantRepository participantRepository,
    IGroupRepository groupRepository,
    ILessonRepository lessonRepository,
    IBillingRepository billingRepository) : IParentPortalService
{
    public async Task<ParentPortalDto> GetPortalAsync(Guid parentUserId, CancellationToken cancellationToken)
    {
        var participantIds = (await parentRepository.ListByParentAsync(parentUserId, cancellationToken))
            .Select(link => link.ParticipantId)
            .Distinct()
            .ToHashSet();

        if (participantIds.Count == 0)
        {
            return new ParentPortalDto([], [], [], [], []);
        }

        var participants = await participantRepository.GetByIdsAsync(participantIds.ToList(), cancellationToken);
        var groups = await groupRepository.ListAsync(cancellationToken);
        var lessons = await lessonRepository.ListAsync(cancellationToken);
        var lessonTitles = lessons.ToDictionary(lesson => lesson.Id, lesson => lesson.Title);
        var lessonsById = lessons.ToDictionary(lesson => lesson.Id);
        var invoices = await billingRepository.ListInvoicesAsync(cancellationToken);

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
                string.IsNullOrWhiteSpace(item.session.MeetingUrl) ? item.group.MeetingUrl : item.session.MeetingUrl))
            .ToList();

        var attendance = groups
            .Where(group => group.Enrollments.Any(enrollment => participantIds.Contains(enrollment.ParticipantId)))
            .SelectMany(group => participantIds
                .Where(participantId => group.Enrollments.Any(enrollment => enrollment.ParticipantId == participantId))
                .Select(participantId => ToAttendance(group, participantId)))
            .Where(item => item.HeldCount > 0)
            .OrderBy(item => item.GroupName)
            .ToList();

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
                invoice.PaidAt))
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
                    item.session.RecordingUrl);
            })
            .Where(material => material.Files.Count > 0 || material.RecordingUrl is not null)
            .ToList();

        return new ParentPortalDto(children, schedule, attendance, invoiceDtos, materials);
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
            .Select(link => new ParentParticipantLinkDto(link.ParentUserId, link.ParticipantId))
            .ToList();
    }

    public async Task<ParentParticipantLinkDto> LinkAsync(Guid parentUserId, Guid participantId, CancellationToken cancellationToken)
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

        var existing = await parentRepository.ListByParentAsync(parentUserId, cancellationToken);
        if (!existing.Any(link => link.ParticipantId == participantId))
        {
            await parentRepository.AddAsync(new ParentParticipantLink { ParentUserId = parentUserId, ParticipantId = participantId }, cancellationToken);
        }

        return new ParentParticipantLinkDto(parentUserId, participantId);
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

        return new ParentAttendanceItemDto(group.Id, group.Name, present, sessions.Count, rate);
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
