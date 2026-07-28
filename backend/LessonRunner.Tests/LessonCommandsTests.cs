using LessonRunner.Application.Lessons;
using LessonRunner.Domain.Groups;
using Xunit;

namespace LessonRunner.Tests;

public sealed class LessonCommandsTests
{
    private static LessonCommands BuildCommands(out InMemoryLessonRepository repository)
    {
        repository = new InMemoryLessonRepository();
        var queries = new LessonQueries(repository);
        return new LessonCommands(repository, queries, new InMemoryGroupRepository());
    }

    [Fact]
    public async Task CreateAsync_PersistsLessonAsDraft_WithMappedSteps()
    {
        var commands = BuildCommands(out _);

        var details = await commands.CreateAsync(TestData.ValidLesson(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, details.Id);
        Assert.Equal("Pierwsza gra", details.Title);
        Assert.Equal("draft", details.Status);
        Assert.Equal("Szkic", details.StatusLabel);
        Assert.Equal(2, details.Steps.Count);
        Assert.Equal(15, details.DurationMinutes);
        // Kroki są numerowane od 1 i posortowane po Order.
        Assert.Equal([1, 2], details.Steps.Select(step => step.Order).ToArray());
        Assert.Equal("intro", details.Steps[0].Type);
    }

    [Fact]
    public async Task CreateAsync_TrimsTitle_AndDropsEmptyTags()
    {
        var commands = BuildCommands(out _);
        var dto = TestData.ValidLesson() with
        {
            Title = "  Lekcja z spacjami  ",
            Tags = ["scratch", "  ", ""]
        };

        var details = await commands.CreateAsync(dto, CancellationToken.None);

        Assert.Equal("Lekcja z spacjami", details.Title);
        Assert.Equal(["scratch"], details.Tags);
    }

    [Fact]
    public async Task CreateAsync_ReturnsProjectFiles_WithGeneratedDownloadUrls()
    {
        var commands = BuildCommands(out _);
        var dto = TestData.ValidLesson() with
        {
            ProjectFiles = new CreateLessonProjectFilesDto(
                Starter: new CreateLessonProjectFileDto(
                    "Start",
                    "/uploads/start.sb3",
                    "start.sb3",
                    "application/octet-stream",
                    123,
                    null),
                Final: null)
        };

        var details = await commands.CreateAsync(dto, CancellationToken.None);

        Assert.NotNull(details.ProjectFiles.Starter);
        Assert.Equal("Start", details.ProjectFiles.Starter!.Label);
        Assert.EndsWith(details.ProjectFiles.Starter.DownloadToken, details.ProjectFiles.Starter.DownloadUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_Throws_WhenTitleMissing(string title)
    {
        var commands = BuildCommands(out _);
        var dto = TestData.ValidLesson() with { Title = title };

        await Assert.ThrowsAsync<ArgumentException>(
            () => commands.CreateAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenSubjectMissing()
    {
        var commands = BuildCommands(out _);
        var dto = TestData.ValidLesson() with { Subject = "  " };

        await Assert.ThrowsAsync<ArgumentException>(
            () => commands.CreateAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenStepHasNoTitle()
    {
        var commands = BuildCommands(out _);
        var dto = TestData.ValidLesson() with
        {
            Steps =
            [
                new CreateLessonStepDto("intro", "  ", 5, [], [], [], [])
            ]
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => commands.CreateAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task PublishAsync_SetsStatusToReady()
    {
        var commands = BuildCommands(out _);
        var created = await commands.CreateAsync(TestData.ValidLesson(), CancellationToken.None);

        var published = await commands.PublishAsync(created.Id, CancellationToken.None);

        Assert.NotNull(published);
        Assert.Equal("ready", published!.Status);
        Assert.Equal("Gotowa", published.StatusLabel);
    }

    [Fact]
    public async Task SendToReviewAsync_SetsStatusToReview()
    {
        var commands = BuildCommands(out _);
        var created = await commands.CreateAsync(TestData.ValidLesson(), CancellationToken.None);

        var review = await commands.SendToReviewAsync(created.Id, CancellationToken.None);

        Assert.NotNull(review);
        Assert.Equal("review", review!.Status);
        Assert.Equal("Do sprawdzenia", review.StatusLabel);
    }

    [Fact]
    public async Task UpdateAsync_PreservesId_AndStatus_WhileChangingContent()
    {
        var commands = BuildCommands(out _);
        var created = await commands.CreateAsync(TestData.ValidLesson(), CancellationToken.None);
        await commands.PublishAsync(created.Id, CancellationToken.None);

        var dto = TestData.ValidLesson() with { Title = "Zmieniony tytuł" };
        var updated = await commands.UpdateAsync(created.Id, dto, CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated!.Id);
        Assert.Equal("Zmieniony tytuł", updated.Title);
        // Status pozostaje "ready" mimo edycji treści.
        Assert.Equal("ready", updated.Status);
    }

    [Fact]
    public async Task UpdateAsync_PreservesOrder_WhenNotSpecified()
    {
        var commands = BuildCommands(out _);
        var created = await commands.CreateAsync(TestData.ValidLesson(), CancellationToken.None);

        var updated = await commands.UpdateAsync(created.Id, TestData.ValidLesson() with { Title = "Po edycji" }, CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(created.Order, updated!.Order);
    }

    [Fact]
    public async Task UpdateAsync_ChangesOrder_WhenSpecified()
    {
        var commands = BuildCommands(out _);
        var created = await commands.CreateAsync(TestData.ValidLesson(), CancellationToken.None);

        var updated = await commands.UpdateAsync(created.Id, TestData.ValidLesson() with { Order = 42 }, CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(42, updated!.Order);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenLessonMissing()
    {
        var commands = BuildCommands(out _);

        var result = await commands.UpdateAsync(Guid.NewGuid(), TestData.ValidLesson(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesLesson()
    {
        var commands = BuildCommands(out var repository);
        var created = await commands.CreateAsync(TestData.ValidLesson(), CancellationToken.None);

        var deleted = await commands.DeleteAsync(created.Id, CancellationToken.None);
        var afterDelete = await repository.GetByIdAsync(created.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task DeleteAsync_Throws_WhenLessonUsedByGroupSession()
    {
        var lessonRepository = new InMemoryLessonRepository();
        var groupRepository = new InMemoryGroupRepository();
        var commands = new LessonCommands(lessonRepository, new LessonQueries(lessonRepository), groupRepository);

        var created = await commands.CreateAsync(TestData.ValidLesson(), CancellationToken.None);

        var group = new Group { Name = "Grupa", InstructorId = Guid.NewGuid() };
        group.Sessions = [new ScheduledSession { GroupId = group.Id, LessonId = created.Id, ScheduledAt = DateTimeOffset.UtcNow, SequenceNumber = 1 }];
        await groupRepository.AddAsync(group, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => commands.DeleteAsync(created.Id, CancellationToken.None));
    }

    [Fact]
    public async Task PublishAsync_ReturnsNull_WhenLessonMissing()
    {
        var commands = BuildCommands(out _);

        var result = await commands.PublishAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }
}
