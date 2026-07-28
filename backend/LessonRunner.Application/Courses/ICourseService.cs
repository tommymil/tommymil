namespace LessonRunner.Application.Courses;

public interface ICourseService
{
    Task<IReadOnlyList<CourseSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken);
    Task<CourseDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<CourseDetailsDto> CreateAsync(UpsertCourseDto dto, CancellationToken cancellationToken);
    Task<CourseDetailsDto?> UpdateAsync(Guid id, UpsertCourseDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
