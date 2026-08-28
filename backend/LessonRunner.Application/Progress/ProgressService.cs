using System.Security.Cryptography;
using LessonRunner.Application.Common;
using LessonRunner.Application.Groups;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Progress;

namespace LessonRunner.Application.Progress;

public sealed class ProgressService(
    IProgressRepository progressRepository,
    IGroupRepository groupRepository) : IProgressService
{
    public async Task<SessionProgressDto?> GetSessionProgressAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken)
    {
        var owned = await LoadOwnedAsync(sessionId, userId, cancellationToken);

        if (owned is null)
        {
            return null;
        }

        var entries = await progressRepository.ListBySessionAsync(sessionId, cancellationToken);
        return new SessionProgressDto(sessionId, entries.Select(ToDto).ToList(), AutonomyOptions());
    }

    public async Task<SessionProgressDto?> SaveSessionProgressAsync(
        Guid sessionId,
        Guid userId,
        SaveSessionProgressDto dto,
        CancellationToken cancellationToken)
    {
        var owned = await LoadOwnedAsync(sessionId, userId, cancellationToken);

        if (owned is null)
        {
            return null;
        }

        var (group, session) = owned.Value;

        // Wpis można zrobić wyłącznie dziecku zapisanemu do tej grupy. Bez tego jeden literówkowy
        // identyfikator dopisałby postęp cudzemu dziecku, a rodzic zobaczyłby go w swoim portalu.
        var enrolled = group.Enrollments.Select(enrollment => enrollment.ParticipantId).ToHashSet();

        foreach (var entry in dto.Entries.Where(entry => enrolled.Contains(entry.ParticipantId)))
        {
            var existing = await progressRepository.GetEntryAsync(sessionId, entry.ParticipantId, cancellationToken);

            var saved = existing ?? new ProgressEntry
            {
                ParticipantId = entry.ParticipantId,
                SessionId = sessionId,
                GroupId = group.Id,
                LessonId = session.LessonId
            };

            saved.Autonomy = AutonomyLevels.Parse(entry.Autonomy);
            saved.LessonCompleted = entry.LessonCompleted;
            saved.NoteForParent = Trimmed(entry.NoteForParent);
            saved.NextStep = Trimmed(entry.NextStep);
            saved.UpdatedAt = DateTimeOffset.UtcNow;
            saved.AuthorUserId = userId;

            await progressRepository.SaveEntryAsync(saved, cancellationToken);
        }

        return await GetSessionProgressAsync(sessionId, userId, cancellationToken);
    }

    public async Task<ParticipantProgressDto?> GetParticipantProgressAsync(
        Guid participantId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        if (!await CanSeeParticipantAsync(participantId, userId, isAdmin, cancellationToken))
        {
            return null;
        }

        return await BuildParticipantProgressAsync(participantId, cancellationToken);
    }

    private async Task<ParticipantProgressDto> BuildParticipantProgressAsync(Guid participantId, CancellationToken cancellationToken)
    {
        var entries = await progressRepository.ListByParticipantsAsync([participantId], cancellationToken);
        var projects = await progressRepository.ListProjectsByParticipantsAsync([participantId], cancellationToken);

        return new ParticipantProgressDto(
            participantId,
            entries.OrderByDescending(entry => entry.UpdatedAt).Select(ToDto).ToList(),
            projects.OrderByDescending(project => project.CreatedAt).Select(ToDto).ToList());
    }

    public async Task<ProjectDto?> CreateProjectAsync(CreateProjectDto dto, Guid userId, bool isAdmin, CancellationToken cancellationToken)
    {
        var title = Trimmed(dto.Title);

        if (title is null)
        {
            throw new ArgumentException("Tytuł projektu jest wymagany.");
        }

        if (!await CanSeeParticipantAsync(dto.ParticipantId, userId, isAdmin, cancellationToken))
        {
            return null;
        }

        var project = new Project
        {
            ParticipantId = dto.ParticipantId,
            Title = title,
            Description = Trimmed(dto.Description),
            GroupId = dto.GroupId,
            LessonId = dto.LessonId,
            CreatedByUserId = userId
        };

        await progressRepository.AddProjectAsync(project, cancellationToken);
        return ToDto(project);
    }

    public async Task<ProjectDto?> AddSubmissionAsync(
        Guid projectId,
        AddSubmissionDto dto,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        // Link do projektu widzi rodzic w swoim portalu i klika go tam wprost, więc musi być
        // http(s). Bez tego `javascript:...` wykonałby się w sesji rodzica.
        var url = WebLink.Normalize(dto.Url, "Link do projektu musi być pełnym adresem http(s).");
        var fileUrl = Trimmed(dto.FileUrl);

        if (url is null && fileUrl is null)
        {
            throw new ArgumentException("Podaj link do projektu albo dołącz plik.");
        }

        // Jedna wersja = jedno źródło prawdy. Przy obu naraz nie wiadomo, co jest tą wersją.
        if (url is not null && fileUrl is not null)
        {
            throw new ArgumentException("Wersja projektu to link albo plik — nie oba naraz.");
        }

        var project = await progressRepository.GetProjectAsync(projectId, cancellationToken);

        if (project is null || !await CanSeeParticipantAsync(project.ParticipantId, userId, isAdmin, cancellationToken))
        {
            return null;
        }

        var submission = new ProjectSubmission
        {
            ProjectId = project.Id,
            // Wersje liczymy po najwyższej istniejącej, a nie po liczbie wpisów - numery mają
            // pozostać stabilne, nawet gdyby kiedyś doszło usuwanie.
            Version = project.Submissions.Count == 0 ? 1 : project.Submissions.Max(item => item.Version) + 1,
            Url = url,
            FileUrl = fileUrl,
            FileName = Trimmed(dto.FileName),
            ContentType = Trimmed(dto.ContentType),
            SizeBytes = dto.SizeBytes,
            DownloadToken = fileUrl is null ? null : CreateDownloadToken(),
            SubmittedByUserId = userId,
            InstructorComment = Trimmed(dto.InstructorComment)
        };

        await progressRepository.AddSubmissionAsync(submission, cancellationToken);

        // Dopisujemy do lokalnego grafu, żeby zwrócone DTO zawierało nową wersję bez ponownego
        // odczytu. Repozytorium EF pracuje na własnych dokumentach i tego grafu nie rusza, ale
        // implementacja trzymająca dane w pamięci operuje na **tym samym** obiekcie projektu —
        // wtedy wersja trafiłaby na listę dwa razy. Stąd sprawdzenie zamiast bezwarunkowego
        // `Add`: metoda ma dawać ten sam wynik niezależnie od tego, czy repozytorium
        // współdzieli graf, czy nie.
        if (project.Submissions.All(item => item.Id != submission.Id))
        {
            project.Submissions.Add(submission);
        }

        return ToDto(project);
    }

    public async Task<bool> SetSubmissionCommentAsync(
        Guid projectId,
        Guid submissionId,
        string? comment,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var project = await progressRepository.GetProjectAsync(projectId, cancellationToken);

        if (project is null
            || project.Submissions.All(submission => submission.Id != submissionId)
            || !await CanSeeParticipantAsync(project.ParticipantId, userId, isAdmin, cancellationToken))
        {
            return false;
        }

        return await progressRepository.SetSubmissionCommentAsync(submissionId, Trimmed(comment), cancellationToken);
    }

    private async Task<(Group Group, ScheduledSession Session)?> LoadOwnedAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return null;
        }

        return group.InstructorId != userId && session.SubstituteInstructorId != userId
            ? null
            : (group, session);
    }

    /// <summary>Admin widzi każde dziecko; instruktor wyłącznie dzieci ze swoich grup —
    /// wraz z tymi, w których prowadzi zastępstwo.</summary>
    private async Task<bool> CanSeeParticipantAsync(
        Guid participantId,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        if (isAdmin)
        {
            return true;
        }

        var groups = await groupRepository.ListForInstructorAsync(userId, cancellationToken);

        return groups.Any(group => group.Enrollments.Any(enrollment => enrollment.ParticipantId == participantId));
    }

    private static IReadOnlyList<AutonomyOptionDto> AutonomyOptions() =>
        AutonomyLevels.All
            .Select(level => new AutonomyOptionDto(level.ToString().ToLowerInvariant(), AutonomyLevels.Label(level), (int)level))
            .ToList();

    private static ProgressEntryDto ToDto(ProgressEntry entry) =>
        new(
            entry.Id,
            entry.ParticipantId,
            entry.SessionId,
            entry.GroupId,
            entry.LessonId,
            entry.Autonomy.ToString().ToLowerInvariant(),
            AutonomyLevels.Label(entry.Autonomy),
            (int)entry.Autonomy,
            entry.LessonCompleted,
            entry.NoteForParent,
            entry.NextStep,
            entry.UpdatedAt);

    private static ProjectDto ToDto(Project project) =>
        new(
            project.Id,
            project.ParticipantId,
            project.Title,
            project.Description,
            project.GroupId,
            project.CreatedAt,
            project.Submissions
                .OrderByDescending(submission => submission.Version)
                .Select(ToDto)
                .ToList());

    private static ProjectSubmissionDto ToDto(ProjectSubmission submission) =>
        new(
            submission.Id,
            submission.Version,
            submission.Url,
            submission.FileName,
            submission.SizeBytes,
            submission.DownloadToken is null ? null : $"/download/project-files/{submission.DownloadToken}",
            submission.SubmittedAt,
            submission.InstructorComment);

    private static string CreateDownloadToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(24))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

    private static string? Trimmed(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}
