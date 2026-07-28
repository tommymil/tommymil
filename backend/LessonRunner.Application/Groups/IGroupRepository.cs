using LessonRunner.Domain.Groups;

namespace LessonRunner.Application.Groups;

public interface IGroupRepository
{
    Task<IReadOnlyList<Group>> ListAsync(CancellationToken cancellationToken);
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Group>> ListByInstructorAsync(Guid instructorId, CancellationToken cancellationToken);

    /// <summary>Zwraca pełny agregat grupy zawierającej dany termin (uczestnicy + terminy + obecność).</summary>
    Task<Group?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);

    Task AddAsync(Group group, CancellationToken cancellationToken);
    Task<bool> UpdateGroupAsync(Group group, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task AddSessionAsync(ScheduledSession session, CancellationToken cancellationToken);

    /// <summary>Czy jakikolwiek termin (w dowolnej grupie) korzysta z danej lekcji.</summary>
    Task<bool> AnyScheduledSessionForLessonAsync(Guid lessonId, CancellationToken cancellationToken);

    Task AddEnrollmentAsync(GroupEnrollment enrollment, CancellationToken cancellationToken);
    Task<bool> SetEnrollmentStatusAsync(Guid groupId, Guid participantId, EnrollmentStatus status, CancellationToken cancellationToken);
    Task<bool> RemoveEnrollmentAsync(Guid groupId, Guid participantId, CancellationToken cancellationToken);

    /// <summary>Aktualizuje pola terminu (status, timery, notatka) - bez kolekcji obecności.</summary>
    Task UpdateSessionAsync(ScheduledSession session, CancellationToken cancellationToken);

    /// <summary>Upsert wierszy obecności dla terminu (po parze ScheduledSessionId+ParticipantId).</summary>
    Task SaveAttendanceAsync(Guid sessionId, IReadOnlyList<AttendanceRecord> records, CancellationToken cancellationToken);

    /// <summary>Dopisuje wpis do historii zmian terminu. Wpisów nie edytujemy ani nie kasujemy.</summary>
    Task AddSessionChangeAsync(SessionChangeLog change, CancellationToken cancellationToken);

    /// <summary>Historia zmian terminów danej grupy, od najnowszej.</summary>
    Task<IReadOnlyList<SessionChangeLog>> ListSessionChangesAsync(Guid groupId, CancellationToken cancellationToken);
}
