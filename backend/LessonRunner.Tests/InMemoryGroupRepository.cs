using LessonRunner.Application.Groups;
using LessonRunner.Domain.Groups;

namespace LessonRunner.Tests;

/// <summary>
/// Fake repozytorium grup trzymające agregaty w pamięci (referencje żywe), do testów warstwy Application.
/// </summary>
internal sealed class InMemoryGroupRepository : IGroupRepository
{
    private readonly List<Group> _groups = [];

    public Task<IReadOnlyList<Group>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Group> result = _groups.ToList();
        return Task.FromResult(result);
    }

    public Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_groups.FirstOrDefault(group => group.Id == id));
    }

    public Task<IReadOnlyList<Group>> ListByInstructorAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Group> result = _groups.Where(group => group.InstructorId == instructorId).ToList();
        return Task.FromResult(result);
    }

    public Task<Group?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_groups.FirstOrDefault(group => group.Sessions.Any(session => session.Id == sessionId)));
    }

    public Task AddAsync(Group group, CancellationToken cancellationToken)
    {
        _groups.Add(group);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateGroupAsync(Group group, CancellationToken cancellationToken)
    {
        var stored = _groups.FirstOrDefault(item => item.Id == group.Id);

        if (stored is null || ReferenceEquals(stored, group))
        {
            return Task.FromResult(stored is not null);
        }

        stored.Name = group.Name;
        stored.InstructorId = group.InstructorId;
        stored.LocationId = group.LocationId;
        stored.Capacity = group.Capacity;
        stored.UpdatedAt = group.UpdatedAt;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_groups.RemoveAll(group => group.Id == id) > 0);
    }

    public Task AddSessionAsync(ScheduledSession session, CancellationToken cancellationToken)
    {
        _groups.First(group => group.Id == session.GroupId).Sessions.Add(session);
        return Task.CompletedTask;
    }

    public Task<bool> AnyScheduledSessionForLessonAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_groups.SelectMany(group => group.Sessions).Any(session => session.LessonId == lessonId));
    }

    public Task AddEnrollmentAsync(GroupEnrollment enrollment, CancellationToken cancellationToken)
    {
        _groups.First(group => group.Id == enrollment.GroupId).Enrollments.Add(enrollment);
        return Task.CompletedTask;
    }

    public Task<bool> SetEnrollmentStatusAsync(
        Guid groupId,
        Guid participantId,
        EnrollmentStatus status,
        CancellationToken cancellationToken)
    {
        var enrollment = _groups
            .FirstOrDefault(group => group.Id == groupId)?
            .Enrollments
            .FirstOrDefault(item => item.ParticipantId == participantId);

        if (enrollment is null)
        {
            return Task.FromResult(false);
        }

        enrollment.Status = status;
        return Task.FromResult(true);
    }

    public Task<bool> RemoveEnrollmentAsync(Guid groupId, Guid participantId, CancellationToken cancellationToken)
    {
        var group = _groups.FirstOrDefault(item => item.Id == groupId);

        if (group is null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(group.Enrollments.RemoveAll(enrollment => enrollment.ParticipantId == participantId) > 0);
    }

    public Task UpdateSessionAsync(ScheduledSession session, CancellationToken cancellationToken)
    {
        var stored = FindSession(session.Id);

        if (stored is not null && !ReferenceEquals(stored, session))
        {
            stored.LessonId = session.LessonId;
            stored.ScheduledAt = session.ScheduledAt;
            stored.LocationId = session.LocationId;
            stored.SubstituteInstructorId = session.SubstituteInstructorId;
            stored.SequenceNumber = session.SequenceNumber;
            stored.Status = session.Status;
            stored.StartedAt = session.StartedAt;
            stored.CompletedAt = session.CompletedAt;
            stored.InstructorNote = session.InstructorNote;
        }

        return Task.CompletedTask;
    }

    public Task SaveAttendanceAsync(Guid sessionId, IReadOnlyList<AttendanceRecord> records, CancellationToken cancellationToken)
    {
        var session = FindSession(sessionId);

        if (session is not null)
        {
            session.Attendance = records.ToList();
        }

        return Task.CompletedTask;
    }

    private ScheduledSession? FindSession(Guid sessionId) =>
        _groups.SelectMany(group => group.Sessions).FirstOrDefault(session => session.Id == sessionId);

    private readonly List<SessionChangeLog> _changes = [];

    public Task AddSessionChangeAsync(SessionChangeLog change, CancellationToken cancellationToken)
    {
        _changes.Add(change);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<SessionChangeLog>> ListSessionChangesAsync(Guid groupId, CancellationToken cancellationToken)
    {
        IReadOnlyList<SessionChangeLog> result = _changes
            .Where(change => change.GroupId == groupId)
            .OrderByDescending(change => change.ChangedAt)
            .ToList();
        return Task.FromResult(result);
    }
}
