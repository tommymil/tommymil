using LessonRunner.Application.Progress;
using LessonRunner.Domain.Progress;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Progress;

internal sealed class EfProgressRepository(AppDbContext dbContext) : IProgressRepository
{
    public async Task<IReadOnlyList<ProgressEntry>> ListByParticipantsAsync(
        IReadOnlyList<Guid> participantIds,
        CancellationToken cancellationToken)
    {
        if (participantIds.Count == 0)
        {
            return [];
        }

        // SQLite nie sortuje po DateTimeOffset w SQL - filtrujemy po dzieciach w bazie,
        // porządkujemy po stronie klienta.
        var documents = await dbContext.ProgressEntries
            .AsNoTracking()
            .Where(entry => participantIds.Contains(entry.ParticipantId))
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(entry => entry.UpdatedAt)
            .Select(ToDomain)
            .ToList();
    }

    public async Task<IReadOnlyList<ProgressEntry>> ListBySessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var documents = await dbContext.ProgressEntries
            .AsNoTracking()
            .Where(entry => entry.SessionId == sessionId)
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<ProgressEntry?> GetEntryAsync(Guid? sessionId, Guid participantId, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProgressEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(
                entry => entry.SessionId == sessionId && entry.ParticipantId == participantId,
                cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task SaveEntryAsync(ProgressEntry entry, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProgressEntries.FirstOrDefaultAsync(item => item.Id == entry.Id, cancellationToken);

        if (document is null)
        {
            dbContext.ProgressEntries.Add(new ProgressEntryDocument
            {
                Id = entry.Id,
                ParticipantId = entry.ParticipantId,
                SessionId = entry.SessionId,
                GroupId = entry.GroupId,
                LessonId = entry.LessonId,
                Autonomy = entry.Autonomy.ToString(),
                LessonCompleted = entry.LessonCompleted,
                NoteForParent = entry.NoteForParent,
                NextStep = entry.NextStep,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt,
                AuthorUserId = entry.AuthorUserId
            });
        }
        else
        {
            document.Autonomy = entry.Autonomy.ToString();
            document.LessonCompleted = entry.LessonCompleted;
            document.NoteForParent = entry.NoteForParent;
            document.NextStep = entry.NextStep;
            document.UpdatedAt = entry.UpdatedAt;
            document.AuthorUserId = entry.AuthorUserId;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> ListProjectsByParticipantsAsync(
        IReadOnlyList<Guid> participantIds,
        CancellationToken cancellationToken)
    {
        if (participantIds.Count == 0)
        {
            return [];
        }

        var documents = await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Submissions)
            .Where(project => participantIds.Contains(project.ParticipantId))
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<Project?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var document = await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Submissions)
            .FirstOrDefaultAsync(project => project.Id == projectId, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task AddProjectAsync(Project project, CancellationToken cancellationToken)
    {
        dbContext.Projects.Add(new ProjectDocument
        {
            Id = project.Id,
            ParticipantId = project.ParticipantId,
            Title = project.Title,
            Description = project.Description,
            GroupId = project.GroupId,
            LessonId = project.LessonId,
            CreatedAt = project.CreatedAt,
            CreatedByUserId = project.CreatedByUserId
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddSubmissionAsync(ProjectSubmission submission, CancellationToken cancellationToken)
    {
        dbContext.ProjectSubmissions.Add(new ProjectSubmissionDocument
        {
            Id = submission.Id,
            ProjectId = submission.ProjectId,
            Version = submission.Version,
            Url = submission.Url,
            FileUrl = submission.FileUrl,
            FileName = submission.FileName,
            ContentType = submission.ContentType,
            SizeBytes = submission.SizeBytes,
            DownloadToken = submission.DownloadToken,
            SubmittedAt = submission.SubmittedAt,
            SubmittedByUserId = submission.SubmittedByUserId,
            InstructorComment = submission.InstructorComment
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SetSubmissionCommentAsync(Guid submissionId, string? comment, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProjectSubmissions
            .FirstOrDefaultAsync(submission => submission.Id == submissionId, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.InstructorComment = comment;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ProjectSubmission?> GetSubmissionByTokenAsync(string downloadToken, CancellationToken cancellationToken)
    {
        var document = await dbContext.ProjectSubmissions
            .AsNoTracking()
            .FirstOrDefaultAsync(submission => submission.DownloadToken == downloadToken, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    private static ProgressEntry ToDomain(ProgressEntryDocument document) => new()
    {
        Id = document.Id,
        ParticipantId = document.ParticipantId,
        SessionId = document.SessionId,
        GroupId = document.GroupId,
        LessonId = document.LessonId,
        Autonomy = AutonomyLevels.Parse(document.Autonomy),
        LessonCompleted = document.LessonCompleted,
        NoteForParent = document.NoteForParent,
        NextStep = document.NextStep,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt,
        AuthorUserId = document.AuthorUserId
    };

    private static Project ToDomain(ProjectDocument document) => new()
    {
        Id = document.Id,
        ParticipantId = document.ParticipantId,
        Title = document.Title,
        Description = document.Description,
        GroupId = document.GroupId,
        LessonId = document.LessonId,
        CreatedAt = document.CreatedAt,
        CreatedByUserId = document.CreatedByUserId,
        Submissions = document.Submissions.Select(ToDomain).ToList()
    };

    private static ProjectSubmission ToDomain(ProjectSubmissionDocument document) => new()
    {
        Id = document.Id,
        ProjectId = document.ProjectId,
        Version = document.Version,
        Url = document.Url,
        FileUrl = document.FileUrl,
        FileName = document.FileName,
        ContentType = document.ContentType,
        SizeBytes = document.SizeBytes,
        DownloadToken = document.DownloadToken,
        SubmittedAt = document.SubmittedAt,
        SubmittedByUserId = document.SubmittedByUserId,
        InstructorComment = document.InstructorComment
    };
}
