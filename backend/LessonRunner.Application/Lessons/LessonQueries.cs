using LessonRunner.Domain.Lessons;

namespace LessonRunner.Application.Lessons;

public sealed class LessonQueries(ILessonRepository lessonRepository) : ILessonQueries
{
    public async Task<LessonDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(id, cancellationToken);

        return lesson is null ? null : ToDetails(lesson);
    }

    public async Task<IReadOnlyList<LessonSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken)
    {
        var lessons = await lessonRepository.ListAsync(cancellationToken);

        return lessons
            .OrderBy(lesson => lesson.Order == 0 ? int.MaxValue : lesson.Order)
            .ThenBy(lesson => lesson.Subject)
            .ThenBy(lesson => lesson.Title)
            .Select(ToSummary)
            .ToList();
    }

    private static LessonSummaryDto ToSummary(Lesson lesson)
    {
        var duration = lesson.Steps.Count > 0
            ? lesson.Steps.Sum(step => step.DurationMinutes)
            : 0;

        return new LessonSummaryDto(
            lesson.Id,
            lesson.Title,
            lesson.Subject,
            lesson.Level,
            lesson.Description,
            lesson.Order,
            lesson.Status.ToString().ToLowerInvariant(),
            ToStatusLabel(lesson.Status),
            lesson.Steps.Count,
            duration);
    }

    private static LessonDetailsDto ToDetails(Lesson lesson)
    {
        return new LessonDetailsDto(
            lesson.Id,
            lesson.Title,
            lesson.Subject,
            lesson.Level,
            lesson.Description,
            lesson.Order,
            lesson.Status.ToString().ToLowerInvariant(),
            ToStatusLabel(lesson.Status),
            lesson.Steps.Count,
            lesson.Steps.Sum(step => step.DurationMinutes),
            lesson.Tags,
            ToProjectFiles(lesson.ProjectFiles),
            lesson.Steps
                .OrderBy(step => step.Order)
                .Select(step => new LessonStepDto(
                    step.Id,
                    step.Order,
                    step.Type.ToString().ToLowerInvariant(),
                    step.Title,
                    step.DurationMinutes,
                    step.Script,
                    step.StudentItems.Select(item => new StudentItemDto(
                        item.Kind.ToString().ToLowerInvariant(),
                        item.Text,
                        item.Caption,
                        item.Url)).ToList(),
                    step.Resources.Select(resource => new LessonResourceDto(
                        resource.Kind.ToString().ToLowerInvariant(),
                        resource.Label,
                        resource.Url,
                        resource.Code,
                        resource.Language)).ToList(),
                    step.Notes.Select(note => new LessonNoteDto(
                        note.Kind.ToString().ToLowerInvariant(),
                        note.Text)).ToList()))
                .ToList(),
            lesson.Objective,
            lesson.SuccessCriteria,
            lesson.Preparation,
            lesson.Homework);
    }

    private static LessonProjectFilesDto ToProjectFiles(LessonProjectFiles projectFiles)
    {
        return new LessonProjectFilesDto(
            ToProjectFile(projectFiles.Starter),
            ToProjectFile(projectFiles.Final));
    }

    private static LessonProjectFileDto? ToProjectFile(LessonProjectFile? file)
    {
        return file is null
            ? null
            : new LessonProjectFileDto(
                file.Label,
                file.Url,
                file.FileName,
                file.ContentType,
                file.SizeBytes,
                file.DownloadToken,
                $"/download/lesson-files/{file.DownloadToken}");
    }

    private static string ToStatusLabel(LessonStatus status)
    {
        return status switch
        {
            LessonStatus.Ready => "Gotowa",
            LessonStatus.Review => "Do sprawdzenia",
            _ => "Szkic"
        };
    }
}
