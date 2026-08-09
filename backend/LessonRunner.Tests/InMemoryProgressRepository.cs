using LessonRunner.Application.Progress;
using LessonRunner.Domain.Progress;

namespace LessonRunner.Tests;

internal sealed class InMemoryProgressRepository : IProgressRepository
{
    private readonly List<ProgressEntry> _entries = [];
    private readonly List<Project> _projects = [];

    public Task<IReadOnlyList<ProgressEntry>> ListByParticipantsAsync(
        IReadOnlyList<Guid> participantIds,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ProgressEntry> result = _entries
            .Where(entry => participantIds.Contains(entry.ParticipantId))
            .ToList();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<ProgressEntry>> ListBySessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        IReadOnlyList<ProgressEntry> result = _entries.Where(entry => entry.SessionId == sessionId).ToList();
        return Task.FromResult(result);
    }

    public Task<ProgressEntry?> GetEntryAsync(Guid? sessionId, Guid participantId, CancellationToken cancellationToken) =>
        Task.FromResult(_entries.FirstOrDefault(entry =>
            entry.SessionId == sessionId && entry.ParticipantId == participantId));

    public Task SaveEntryAsync(ProgressEntry entry, CancellationToken cancellationToken)
    {
        if (_entries.All(item => item.Id != entry.Id))
        {
            _entries.Add(entry);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Project>> ListProjectsByParticipantsAsync(
        IReadOnlyList<Guid> participantIds,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Project> result = _projects
            .Where(project => participantIds.Contains(project.ParticipantId))
            .ToList();

        return Task.FromResult(result);
    }

    public Task<Project?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken) =>
        Task.FromResult(_projects.FirstOrDefault(project => project.Id == projectId));

    public Task AddProjectAsync(Project project, CancellationToken cancellationToken)
    {
        _projects.Add(project);
        return Task.CompletedTask;
    }

    public Task AddSubmissionAsync(ProjectSubmission submission, CancellationToken cancellationToken)
    {
        var project = _projects.FirstOrDefault(item => item.Id == submission.ProjectId);

        if (project is not null && project.Submissions.All(item => item.Id != submission.Id))
        {
            project.Submissions.Add(submission);
        }

        return Task.CompletedTask;
    }

    public Task<bool> SetSubmissionCommentAsync(Guid submissionId, string? comment, CancellationToken cancellationToken)
    {
        var submission = _projects
            .SelectMany(project => project.Submissions)
            .FirstOrDefault(item => item.Id == submissionId);

        if (submission is null)
        {
            return Task.FromResult(false);
        }

        submission.InstructorComment = comment;
        return Task.FromResult(true);
    }

    public Task<ProjectSubmission?> GetSubmissionByTokenAsync(string downloadToken, CancellationToken cancellationToken) =>
        Task.FromResult(_projects
            .SelectMany(project => project.Submissions)
            .FirstOrDefault(submission => submission.DownloadToken == downloadToken));
}
