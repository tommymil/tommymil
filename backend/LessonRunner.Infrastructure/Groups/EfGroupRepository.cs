using LessonRunner.Application.Groups;
using LessonRunner.Domain.Groups;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Groups;

internal sealed class EfGroupRepository(AppDbContext dbContext) : IGroupRepository
{
    public async Task<IReadOnlyList<Group>> ListAsync(CancellationToken cancellationToken)
    {
        var documents = await dbContext.Groups
            .AsNoTracking()
            .Include(group => group.Enrollments)
            .Include(group => group.Sessions)
            .AsSplitQuery()
            .OrderBy(group => group.Name)
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await QueryAggregate()
            .FirstOrDefaultAsync(group => group.Id == id, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task<IReadOnlyList<Group>> ListForInstructorAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var documents = await QueryAggregate()
            .Where(group => group.InstructorId == instructorId
                || group.Sessions.Any(session => session.SubstituteInstructorId == instructorId))
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<Group?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var document = await QueryAggregate()
            .FirstOrDefaultAsync(group => group.Sessions.Any(session => session.Id == sessionId), cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task AddAsync(Group group, CancellationToken cancellationToken)
    {
        dbContext.Groups.Add(ToDocument(group));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateGroupAsync(Group group, CancellationToken cancellationToken)
    {
        var document = await dbContext.Groups.FirstOrDefaultAsync(item => item.Id == group.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Name = group.Name;
        document.InstructorId = group.InstructorId;
        document.LocationId = group.LocationId;
        document.Capacity = group.Capacity;
        document.MeetingUrl = group.MeetingUrl;
        document.UpdatedAt = group.UpdatedAt;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Groups.FirstOrDefaultAsync(group => group.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.Groups.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddSessionAsync(ScheduledSession session, CancellationToken cancellationToken)
    {
        dbContext.ScheduledSessions.Add(ToDocument(session));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> AnyScheduledSessionForLessonAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        return dbContext.ScheduledSessions.AnyAsync(session => session.LessonId == lessonId, cancellationToken);
    }

    public async Task AddEnrollmentAsync(GroupEnrollment enrollment, CancellationToken cancellationToken)
    {
        dbContext.GroupEnrollments.Add(ToDocument(enrollment));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SetEnrollmentStatusAsync(
        Guid groupId,
        Guid participantId,
        EnrollmentStatus status,
        CancellationToken cancellationToken)
    {
        var document = await dbContext.GroupEnrollments
            .FirstOrDefaultAsync(item => item.GroupId == groupId && item.ParticipantId == participantId, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Status = status.ToString();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveEnrollmentAsync(Guid groupId, Guid participantId, CancellationToken cancellationToken)
    {
        var document = await dbContext.GroupEnrollments
            .FirstOrDefaultAsync(item => item.GroupId == groupId && item.ParticipantId == participantId, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.GroupEnrollments.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task UpdateSessionAsync(ScheduledSession session, CancellationToken cancellationToken)
    {
        var document = await dbContext.ScheduledSessions
            .FirstOrDefaultAsync(item => item.Id == session.Id, cancellationToken);

        if (document is null)
        {
            return;
        }

        document.LessonId = session.LessonId;
        document.ScheduledAt = session.ScheduledAt;
        document.LocationId = session.LocationId;
        document.SubstituteInstructorId = session.SubstituteInstructorId;
        document.SequenceNumber = session.SequenceNumber;
        document.Status = session.Status.ToString();
        document.StartedAt = session.StartedAt;
        document.CompletedAt = session.CompletedAt;
        document.InstructorNote = session.InstructorNote;
        document.UnfinishedNote = session.UnfinishedNote;
        document.ParentSummary = session.ParentSummary;
        document.MeetingUrl = session.MeetingUrl;
        document.RecordingUrl = session.RecordingUrl;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAttendanceAsync(Guid sessionId, IReadOnlyList<AttendanceRecord> records, CancellationToken cancellationToken)
    {
        var existing = await dbContext.AttendanceRecords
            .Where(record => record.ScheduledSessionId == sessionId)
            .ToListAsync(cancellationToken);

        var existingByParticipant = existing.ToDictionary(record => record.ParticipantId);

        foreach (var record in records)
        {
            if (existingByParticipant.TryGetValue(record.ParticipantId, out var document))
            {
                document.Status = record.Status.ToString();
                document.LiveStatus = record.LiveStatus.ToString();
                document.Present = record.Present;
                document.Note = record.Note;
                document.JoinedAt = record.JoinedAt;
                document.LeftAt = record.LeftAt;
                document.MakeupRequired = record.MakeupRequired;
                document.MakeupSessionId = record.MakeupSessionId;
                document.MarkedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                dbContext.AttendanceRecords.Add(new AttendanceRecordDocument
                {
                    Id = record.Id,
                    ScheduledSessionId = sessionId,
                    ParticipantId = record.ParticipantId,
                    Status = record.Status.ToString(),
                    LiveStatus = record.LiveStatus.ToString(),
                    Present = record.Present,
                    Note = record.Note,
                    JoinedAt = record.JoinedAt,
                    LeftAt = record.LeftAt,
                    MakeupRequired = record.MakeupRequired,
                    MakeupSessionId = record.MakeupSessionId,
                    MarkedAt = DateTimeOffset.UtcNow
                });
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddSessionChangeAsync(SessionChangeLog change, CancellationToken cancellationToken)
    {
        dbContext.SessionChangeLogs.Add(new SessionChangeLogDocument
        {
            Id = change.Id,
            ScheduledSessionId = change.ScheduledSessionId,
            GroupId = change.GroupId,
            ChangeType = change.ChangeType.ToString(),
            PreviousScheduledAt = change.PreviousScheduledAt,
            NewScheduledAt = change.NewScheduledAt,
            Reason = change.Reason,
            Details = change.Details,
            ChangedByUserId = change.ChangedByUserId,
            GuardiansNotified = change.GuardiansNotified,
            ChangedAt = change.ChangedAt
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SessionChangeLog>> ListSessionChangesAsync(Guid groupId, CancellationToken cancellationToken)
    {
        // SQLite nie sortuje po DateTimeOffset w SQL - filtrujemy po grupie w bazie
        // (to się tłumaczy), a porządkujemy po stronie klienta.
        var documents = await dbContext.SessionChangeLogs
            .AsNoTracking()
            .Where(change => change.GroupId == groupId)
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(change => change.ChangedAt)
            .Select(ToDomain)
            .ToList();
    }

    private static SessionChangeLog ToDomain(SessionChangeLogDocument document) => new()
    {
        Id = document.Id,
        ScheduledSessionId = document.ScheduledSessionId,
        GroupId = document.GroupId,
        ChangeType = ParseEnum(document.ChangeType, SessionChangeType.Rescheduled),
        PreviousScheduledAt = document.PreviousScheduledAt,
        NewScheduledAt = document.NewScheduledAt,
        Reason = document.Reason,
        Details = document.Details,
        ChangedByUserId = document.ChangedByUserId,
        GuardiansNotified = document.GuardiansNotified,
        ChangedAt = document.ChangedAt
    };

    private IQueryable<GroupDocument> QueryAggregate() =>
        dbContext.Groups
            .AsNoTracking()
            .AsSplitQuery()
            .Include(group => group.Enrollments)
            .Include(group => group.Sessions)
            .ThenInclude(session => session.Attendance);

    private static Group ToDomain(GroupDocument document) => new()
    {
        Id = document.Id,
        Name = document.Name,
        InstructorId = document.InstructorId,
        CourseId = document.CourseId,
        LocationId = document.LocationId,
        Capacity = document.Capacity,
        MeetingUrl = document.MeetingUrl,
        Status = ParseEnum(document.Status, GroupStatus.Active),
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt,
        Enrollments = document.Enrollments.Select(ToDomain).ToList(),
        Sessions = document.Sessions.Select(ToDomain).ToList()
    };

    private static GroupEnrollment ToDomain(GroupEnrollmentDocument document) => new()
    {
        Id = document.Id,
        GroupId = document.GroupId,
        ParticipantId = document.ParticipantId,
        Status = ParseEnum(document.Status, EnrollmentStatus.Enrolled),
        EnrolledAt = document.EnrolledAt
    };

    private static ScheduledSession ToDomain(ScheduledSessionDocument document) => new()
    {
        Id = document.Id,
        GroupId = document.GroupId,
        LessonId = document.LessonId,
        ScheduledAt = document.ScheduledAt,
        LocationId = document.LocationId,
        SubstituteInstructorId = document.SubstituteInstructorId,
        SequenceNumber = document.SequenceNumber,
        Status = ParseEnum(document.Status, ScheduledSessionStatus.Planned),
        StartedAt = document.StartedAt,
        CompletedAt = document.CompletedAt,
        InstructorNote = document.InstructorNote,
        UnfinishedNote = document.UnfinishedNote,
        ParentSummary = document.ParentSummary,
        MeetingUrl = document.MeetingUrl,
        RecordingUrl = document.RecordingUrl,
        Attendance = document.Attendance.Select(ToDomain).ToList()
    };

    private static AttendanceRecord ToDomain(AttendanceRecordDocument document) => new()
    {
        Id = document.Id,
        ScheduledSessionId = document.ScheduledSessionId,
        ParticipantId = document.ParticipantId,
        // Rekordy sprzed migracji `AddAttendanceStatus` mają pusty status - wyprowadzamy go
        // wtedy z kolumny Present, żeby stara historia obecności nie zamieniła się w zera.
        Status = ParseEnum(
            document.Status,
            document.Present ? AttendanceStatus.Present : AttendanceStatus.UnexcusedAbsence),
        // Rekordy sprzed migracji `AddSessionDebriefAndLiveStatus` mają pusty LiveStatus.
        LiveStatus = ParseEnum(document.LiveStatus, LiveWorkStatus.Working),
        Note = document.Note,
        JoinedAt = document.JoinedAt,
        LeftAt = document.LeftAt,
        MakeupRequired = document.MakeupRequired,
        MakeupSessionId = document.MakeupSessionId,
        MarkedAt = document.MarkedAt
    };

    private static GroupDocument ToDocument(Group group) => new()
    {
        Id = group.Id,
        Name = group.Name,
        InstructorId = group.InstructorId,
        CourseId = group.CourseId,
        LocationId = group.LocationId,
        Capacity = group.Capacity,
        MeetingUrl = group.MeetingUrl,
        Status = group.Status.ToString(),
        CreatedAt = group.CreatedAt,
        UpdatedAt = group.UpdatedAt,
        Enrollments = group.Enrollments.Select(ToDocument).ToList(),
        Sessions = group.Sessions.Select(ToDocument).ToList()
    };

    private static GroupEnrollmentDocument ToDocument(GroupEnrollment enrollment) => new()
    {
        Id = enrollment.Id,
        GroupId = enrollment.GroupId,
        ParticipantId = enrollment.ParticipantId,
        Status = enrollment.Status.ToString(),
        EnrolledAt = enrollment.EnrolledAt
    };

    private static ScheduledSessionDocument ToDocument(ScheduledSession session) => new()
    {
        Id = session.Id,
        GroupId = session.GroupId,
        LessonId = session.LessonId,
        ScheduledAt = session.ScheduledAt,
        LocationId = session.LocationId,
        SubstituteInstructorId = session.SubstituteInstructorId,
        SequenceNumber = session.SequenceNumber,
        Status = session.Status.ToString(),
        StartedAt = session.StartedAt,
        CompletedAt = session.CompletedAt,
        InstructorNote = session.InstructorNote,
        UnfinishedNote = session.UnfinishedNote,
        ParentSummary = session.ParentSummary,
        MeetingUrl = session.MeetingUrl,
        RecordingUrl = session.RecordingUrl,
        Attendance = session.Attendance.Select(ToDocument).ToList()
    };

    private static AttendanceRecordDocument ToDocument(AttendanceRecord record) => new()
    {
        Id = record.Id,
        ScheduledSessionId = record.ScheduledSessionId,
        ParticipantId = record.ParticipantId,
        Status = record.Status.ToString(),
        LiveStatus = record.LiveStatus.ToString(),
        Present = record.Present,
        Note = record.Note,
        JoinedAt = record.JoinedAt,
        LeftAt = record.LeftAt,
        MakeupRequired = record.MakeupRequired,
        MakeupSessionId = record.MakeupSessionId,
        MarkedAt = record.MarkedAt
    };

    private static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct =>
        Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : fallback;
}
