namespace LessonRunner.Application.Progress;

/// <summary>
/// Postępy i projekty dzieci.
///
/// Zapis jest zawsze związany z terminem i sprawdza właściciela: prowadzi go instruktor grupy
/// albo zastępstwo — dla cudzego terminu metoda zwraca `null` (mapowane na 404), tak samo jak
/// w <see cref="Groups.ISessionService"/>.
/// </summary>
public interface IProgressService
{
    Task<SessionProgressDto?> GetSessionProgressAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken);

    Task<SessionProgressDto?> SaveSessionProgressAsync(
        Guid sessionId,
        Guid userId,
        SaveSessionProgressDto dto,
        CancellationToken cancellationToken);

    /// <summary>Dorobek dziecka. <paramref name="isAdmin"/> otwiera dostęp do każdego dziecka;
    /// instruktor widzi wyłącznie dzieci ze swoich grup.</summary>
    Task<ParticipantProgressDto?> GetParticipantProgressAsync(
        Guid participantId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken);

    Task<ProjectDto?> CreateProjectAsync(CreateProjectDto dto, Guid userId, bool isAdmin, CancellationToken cancellationToken);

    Task<ProjectDto?> AddSubmissionAsync(Guid projectId, AddSubmissionDto dto, Guid userId, bool isAdmin, CancellationToken cancellationToken);

    Task<bool> SetSubmissionCommentAsync(Guid projectId, Guid submissionId, string? comment, Guid userId, bool isAdmin, CancellationToken cancellationToken);
}
