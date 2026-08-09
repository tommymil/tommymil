using LessonRunner.Domain.Progress;

namespace LessonRunner.Application.Progress;

public interface IProgressRepository
{
    Task<IReadOnlyList<ProgressEntry>> ListByParticipantsAsync(IReadOnlyList<Guid> participantIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProgressEntry>> ListBySessionAsync(Guid sessionId, CancellationToken cancellationToken);

    /// <summary>Dopisuje albo poprawia wpis dla pary termin+dziecko. Jeden wpis na dziecko
    /// na termin — kolejny zapis z kokpitu ma poprawiać ten sam, a nie mnożyć wersje.</summary>
    Task SaveEntryAsync(ProgressEntry entry, CancellationToken cancellationToken);

    Task<ProgressEntry?> GetEntryAsync(Guid? sessionId, Guid participantId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Project>> ListProjectsByParticipantsAsync(IReadOnlyList<Guid> participantIds, CancellationToken cancellationToken);
    Task<Project?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken);
    Task AddProjectAsync(Project project, CancellationToken cancellationToken);
    Task AddSubmissionAsync(ProjectSubmission submission, CancellationToken cancellationToken);
    Task<bool> SetSubmissionCommentAsync(Guid submissionId, string? comment, CancellationToken cancellationToken);

    /// <summary>Wersja projektu wskazana nieodgadywalnym kluczem z adresu pobrania.</summary>
    Task<ProjectSubmission?> GetSubmissionByTokenAsync(string downloadToken, CancellationToken cancellationToken);
}
