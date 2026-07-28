using LessonRunner.Application.Courses;
using LessonRunner.Application.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Users;
using Xunit;

namespace LessonRunner.Tests;

public sealed class CourseServiceTests
{
    private static Lesson Lesson(string title, LessonStatus status = LessonStatus.Ready) => new()
    {
        Title = title,
        Subject = "Scratch",
        Level = "Poziom 1",
        Description = "Opis",
        Status = status
    };

    [Fact]
    public async Task CreateAsync_RejectsLessonThatIsNotReady()
    {
        var courses = new InMemoryCourseRepository();
        var lessons = new InMemoryLessonRepository();
        var draft = Lesson("Szkic", LessonStatus.Draft);
        await lessons.AddAsync(draft, CancellationToken.None);
        var service = new CourseService(courses, lessons);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(
                new UpsertCourseDto("Scratch A", "Scratch", "P1", "", [draft.Id]),
                CancellationToken.None));
    }

    [Fact]
    public async Task GroupCreatedFromCourse_UsesCourseLessonOrder()
    {
        var courseRepository = new InMemoryCourseRepository();
        var lessonRepository = new InMemoryLessonRepository();
        var users = new InMemoryUserRepository();
        var participants = new InMemoryParticipantRepository();
        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var l1 = Lesson("Pierwsza");
        var l2 = Lesson("Druga");
        var manual = Lesson("Manualna");
        await lessonRepository.AddAsync(l1, CancellationToken.None);
        await lessonRepository.AddAsync(l2, CancellationToken.None);
        await lessonRepository.AddAsync(manual, CancellationToken.None);

        var courseService = new CourseService(courseRepository, lessonRepository);
        var course = await courseService.CreateAsync(
            new UpsertCourseDto("Scratch A", "Scratch", "P1", "", [l2.Id, l1.Id]),
            CancellationToken.None);
        var groupService = new GroupService(
            new InMemoryGroupRepository(),
            lessonRepository,
            users,
            participants,
            courseRepository);

        var details = await groupService.CreateAsync(
            new CreateGroupDto(
                "Grupa",
                instructor.Id,
                [manual.Id],
                new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero),
                [],
                course.Id),
            CancellationToken.None);

        Assert.Equal(course.Id, details.CourseId);
        Assert.Equal([l2.Id, l1.Id], details.Sessions.Select(session => session.LessonId));
        Assert.DoesNotContain(details.Sessions, session => session.LessonId == manual.Id);
    }
}
