using LessonRunner.Application.Lessons;
using LessonRunner.Domain.Courses;
using LessonRunner.Domain.Lessons;

namespace LessonRunner.Application.Courses;

public sealed class CourseService(
    ICourseRepository courseRepository,
    ILessonRepository lessonRepository) : ICourseService
{
    public async Task<IReadOnlyList<CourseSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken)
    {
        var courses = await courseRepository.ListAsync(cancellationToken);

        return courses
            .OrderBy(course => course.Subject)
            .ThenBy(course => course.Level)
            .ThenBy(course => course.Name)
            .Select(course => new CourseSummaryDto(
                course.Id,
                course.Name,
                course.Subject,
                course.Level,
                course.Description,
                course.Lessons.Count))
            .ToList();
    }

    public async Task<CourseDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(id, cancellationToken);

        if (course is null)
        {
            return null;
        }

        var lessonsById = (await lessonRepository.ListAsync(cancellationToken)).ToDictionary(lesson => lesson.Id);
        return ToDetails(course, lessonsById);
    }

    public async Task<CourseDetailsDto> CreateAsync(UpsertCourseDto dto, CancellationToken cancellationToken)
    {
        var lessonsById = await ValidateAsync(dto, cancellationToken);
        var lessonIds = NormalizeLessonIds(dto.LessonIds);
        var course = new Course
        {
            Name = dto.Name.Trim(),
            Subject = dto.Subject.Trim(),
            Level = dto.Level.Trim(),
            Description = (dto.Description ?? string.Empty).Trim(),
            Lessons = lessonIds
                .Select((lessonId, index) => new CourseLesson { LessonId = lessonId, Order = index + 1 })
                .ToList()
        };

        foreach (var lesson in course.Lessons)
        {
            lesson.CourseId = course.Id;
        }

        await courseRepository.AddAsync(course, cancellationToken);
        return ToDetails(course, lessonsById);
    }

    public async Task<CourseDetailsDto?> UpdateAsync(Guid id, UpsertCourseDto dto, CancellationToken cancellationToken)
    {
        var existing = await courseRepository.GetByIdAsync(id, cancellationToken);

        if (existing is null)
        {
            return null;
        }

        var lessonsById = await ValidateAsync(dto, cancellationToken);
        var lessonIds = NormalizeLessonIds(dto.LessonIds);

        existing.Name = dto.Name.Trim();
        existing.Subject = dto.Subject.Trim();
        existing.Level = dto.Level.Trim();
        existing.Description = (dto.Description ?? string.Empty).Trim();
        existing.UpdatedAt = DateTimeOffset.UtcNow;
        existing.Lessons = lessonIds
            .Select((lessonId, index) => new CourseLesson
            {
                CourseId = existing.Id,
                LessonId = lessonId,
                Order = index + 1
            })
            .ToList();

        await courseRepository.UpdateAsync(existing, cancellationToken);
        return ToDetails(existing, lessonsById);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        courseRepository.DeleteAsync(id, cancellationToken);

    private async Task<IReadOnlyDictionary<Guid, Lesson>> ValidateAsync(UpsertCourseDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Nazwa kursu jest wymagana.");
        }

        if (string.IsNullOrWhiteSpace(dto.Subject))
        {
            throw new ArgumentException("Przedmiot kursu jest wymagany.");
        }

        if (string.IsNullOrWhiteSpace(dto.Level))
        {
            throw new ArgumentException("Poziom kursu jest wymagany.");
        }

        var lessonIds = NormalizeLessonIds(dto.LessonIds);

        if (lessonIds.Count == 0)
        {
            throw new ArgumentException("Wybierz przynajmniej jedną lekcję kursu.");
        }

        var lessonsById = (await lessonRepository.ListAsync(cancellationToken)).ToDictionary(lesson => lesson.Id);

        foreach (var lessonId in lessonIds)
        {
            if (!lessonsById.TryGetValue(lessonId, out var lesson))
            {
                throw new ArgumentException("Wybrana lekcja nie istnieje.");
            }

            if (lesson.Status != LessonStatus.Ready)
            {
                throw new ArgumentException($"Lekcja \"{lesson.Title}\" nie jest opublikowana (Ready).");
            }
        }

        return lessonsById;
    }

    private static List<Guid> NormalizeLessonIds(IReadOnlyList<Guid>? lessonIds)
    {
        return (lessonIds ?? [])
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();
    }

    private static CourseDetailsDto ToDetails(Course course, IReadOnlyDictionary<Guid, Lesson> lessonsById)
    {
        return new CourseDetailsDto(
            course.Id,
            course.Name,
            course.Subject,
            course.Level,
            course.Description,
            course.Lessons
                .OrderBy(lesson => lesson.Order)
                .Select(lesson =>
                {
                    lessonsById.TryGetValue(lesson.LessonId, out var source);

                    return new CourseLessonDto(
                        lesson.LessonId,
                        source?.Title ?? "(brak lekcji)",
                        source?.Subject ?? course.Subject,
                        source?.Level ?? course.Level,
                        lesson.Order);
                })
                .ToList());
    }
}
