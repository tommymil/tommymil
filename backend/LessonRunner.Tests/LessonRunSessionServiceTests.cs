using LessonRunner.Application.LessonRuns;
using Xunit;

namespace LessonRunner.Tests;

public sealed class LessonRunSessionServiceTests
{
    private static async Task<(LessonRunSessionService Service, Guid LessonId, Guid UserId)> BuildServiceAsync()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = TestData.ValidLesson().ToTestLesson();
        await lessons.AddAsync(lesson, CancellationToken.None);

        var sessions = new InMemoryLessonRunSessionRepository();
        var service = new LessonRunSessionService(lessons, sessions);

        return (service, lesson.Id, Guid.NewGuid());
    }

    [Fact]
    public async Task GetOrCreateAsync_CreatesSessionForLessonAndUser()
    {
        var (service, lessonId, userId) = await BuildServiceAsync();

        var session = await service.GetOrCreateAsync(lessonId, userId, Guid.Empty, CancellationToken.None);

        Assert.NotNull(session);
        Assert.Equal(lessonId, session!.LessonId);
        Assert.Equal(userId, session.UserId);
        Assert.Equal(0, session.StepIndex);
        Assert.False(session.Running);
    }

    [Fact]
    public async Task GetOrCreateAsync_ReturnsExistingSession()
    {
        var (service, lessonId, userId) = await BuildServiceAsync();
        var first = await service.GetOrCreateAsync(lessonId, userId, Guid.Empty, CancellationToken.None);

        var second = await service.GetOrCreateAsync(lessonId, userId, Guid.Empty, CancellationToken.None);

        Assert.Equal(first!.Id, second!.Id);
    }

    [Fact]
    public async Task GetOrCreateAsync_SeparatesSessionsPerScheduledSession()
    {
        var (service, lessonId, userId) = await BuildServiceAsync();

        var adHoc = await service.GetOrCreateAsync(lessonId, userId, Guid.Empty, CancellationToken.None);
        var scheduled = await service.GetOrCreateAsync(lessonId, userId, Guid.NewGuid(), CancellationToken.None);

        Assert.NotNull(adHoc);
        Assert.NotNull(scheduled);
        Assert.NotEqual(adHoc!.Id, scheduled!.Id);
    }

    [Fact]
    public async Task UpdateAsync_ClampsStepIndexAndStoresTimers()
    {
        var (service, lessonId, userId) = await BuildServiceAsync();

        var updated = await service.UpdateAsync(
            lessonId,
            userId,
            Guid.Empty,
            new UpdateLessonRunSessionDto(99, 120, 30, true),
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(1, updated!.StepIndex);
        Assert.Equal(120, updated.ElapsedTotalSeconds);
        Assert.Equal(30, updated.ElapsedStepSeconds);
        Assert.True(updated.Running);
    }
}
