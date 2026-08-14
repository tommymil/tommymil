using LessonRunner.Application.Trials;
using LessonRunner.Domain.Trials;

namespace LessonRunner.Tests;

internal sealed class InMemoryTrialRepository : ITrialRepository
{
    private readonly List<TrialLesson> items = [];

    public Task<IReadOnlyList<TrialLesson>> ListAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<TrialLesson>>(items.ToList());

    public Task<IReadOnlyList<TrialLesson>> ListByInstructorAsync(Guid instructorId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<TrialLesson>>(items.Where(trial => trial.InstructorId == instructorId).ToList());

    public Task<bool> AnyForLessonAsync(Guid lessonId, CancellationToken cancellationToken) =>
        Task.FromResult(items.Any(trial => trial.LessonId == lessonId));

    public Task<TrialLesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(items.FirstOrDefault(trial => trial.Id == id));

    public Task AddAsync(TrialLesson trial, CancellationToken cancellationToken)
    {
        items.Add(trial);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateAsync(TrialLesson trial, CancellationToken cancellationToken)
    {
        var index = items.FindIndex(item => item.Id == trial.Id);

        if (index < 0)
        {
            return Task.FromResult(false);
        }

        items[index] = trial;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(items.RemoveAll(trial => trial.Id == id) > 0);
}
