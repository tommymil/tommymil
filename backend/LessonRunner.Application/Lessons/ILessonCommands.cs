namespace LessonRunner.Application.Lessons;

public interface ILessonCommands
{
    Task<LessonDetailsDto> CreateAsync(CreateLessonDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<LessonDetailsDto?> SendToReviewAsync(Guid id, CancellationToken cancellationToken);
    Task<LessonDetailsDto?> PublishAsync(Guid id, CancellationToken cancellationToken);
    Task<LessonDetailsDto?> UpdateAsync(Guid id, CreateLessonDto dto, CancellationToken cancellationToken);
}
