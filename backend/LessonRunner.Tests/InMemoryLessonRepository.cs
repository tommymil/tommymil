using LessonRunner.Application.Lessons;
using LessonRunner.Domain.Lessons;

namespace LessonRunner.Tests;

/// <summary>
/// Prosty fake repozytorium trzymający lekcje w pamięci, używany do testów warstwy Application.
/// </summary>
internal sealed class InMemoryLessonRepository : ILessonRepository
{
    private readonly Dictionary<Guid, Lesson> _lessons = [];

    public Task<IReadOnlyList<Lesson>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Lesson> result = _lessons.Values.ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyDictionary<Guid, string>> ListTitlesAsync(CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<Guid, string> result = _lessons.Values.ToDictionary(
            lesson => lesson.Id,
            lesson => lesson.Title);

        return Task.FromResult(result);
    }

    public Task<LessonProjectFile?> FindProjectFileByDownloadTokenAsync(string token, CancellationToken cancellationToken)
    {
        var file = _lessons.Values
            .SelectMany(lesson => new[] { lesson.ProjectFiles.Starter, lesson.ProjectFiles.Final })
            .OfType<LessonProjectFile>()
            .FirstOrDefault(item => string.Equals(item.DownloadToken, token, StringComparison.Ordinal));

        return Task.FromResult(file);
    }

    public Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_lessons.TryGetValue(id, out var lesson) ? lesson : null);
    }

    public Task AddAsync(Lesson lesson, CancellationToken cancellationToken)
    {
        _lessons[lesson.Id] = lesson;
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_lessons.Remove(id));
    }

    public Task<bool> UpdateAsync(Lesson lesson, CancellationToken cancellationToken)
    {
        if (!_lessons.ContainsKey(lesson.Id))
        {
            return Task.FromResult(false);
        }

        _lessons[lesson.Id] = lesson;
        return Task.FromResult(true);
    }

}
