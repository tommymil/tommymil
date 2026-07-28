namespace LessonRunner.Application.Lessons;

public interface ILessonQueries
{
    Task<IReadOnlyList<LessonSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken);
    Task<LessonDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken);
}
