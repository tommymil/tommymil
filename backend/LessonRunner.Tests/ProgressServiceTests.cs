using LessonRunner.Application.Progress;
using LessonRunner.Domain.Groups;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Postępy i projekty dzieci. Najważniejsze w tych testach nie jest to, że wpis się zapisuje,
/// tylko to, czyj wpis i kto go zobaczy.
/// </summary>
public sealed class ProgressServiceTests
{
    private static readonly Guid Instructor = Guid.NewGuid();
    private static readonly Guid OtherInstructor = Guid.NewGuid();
    private static readonly Guid Child = Guid.NewGuid();
    private static readonly Guid StrangerChild = Guid.NewGuid();

    [Fact]
    public async Task SaveSessionProgress_StoresEntry_AndSecondSaveUpdatesTheSameOne()
    {
        var (service, _, session) = Build();

        var first = await service.SaveSessionProgressAsync(
            session.Id,
            Instructor,
            new SaveSessionProgressDto([new SaveProgressEntryDto(Child, "independently", true, "Świetnie sobie poradził", "Powtórzyć pętle")]),
            CancellationToken.None);

        Assert.NotNull(first);
        var entry = Assert.Single(first!.Entries);
        Assert.Equal("independently", entry.Autonomy);
        Assert.Equal("Wykonuje samodzielnie", entry.AutonomyLabel);
        Assert.True(entry.LessonCompleted);

        var second = await service.SaveSessionProgressAsync(
            session.Id,
            Instructor,
            new SaveSessionProgressDto([new SaveProgressEntryDto(Child, "canextend", false, "Rozbudował grę o poziom 2")]),
            CancellationToken.None);

        // Jeden wpis na dziecko na termin - poprawka ma nadpisać, a nie dołożyć drugi wiersz.
        var updated = Assert.Single(second!.Entries);
        Assert.Equal(entry.Id, updated.Id);
        Assert.Equal("canextend", updated.Autonomy);
        Assert.False(updated.LessonCompleted);
    }

    [Fact]
    public async Task SaveSessionProgress_IgnoresChildrenFromOutsideTheGroup()
    {
        var (service, _, session) = Build();

        var result = await service.SaveSessionProgressAsync(
            session.Id,
            Instructor,
            new SaveSessionProgressDto(
            [
                new SaveProgressEntryDto(Child, "withhelp"),
                new SaveProgressEntryDto(StrangerChild, "canexplain", NoteForParent: "wpis pod złym identyfikatorem")
            ]),
            CancellationToken.None);

        // Literówka w identyfikatorze nie może dopisać postępu cudzemu dziecku - rodzic
        // zobaczyłby go w swoim portalu.
        var entry = Assert.Single(result!.Entries);
        Assert.Equal(Child, entry.ParticipantId);
    }

    [Fact]
    public async Task SaveSessionProgress_ForForeignSession_ReturnsNull()
    {
        var (service, _, session) = Build();

        var result = await service.SaveSessionProgressAsync(
            session.Id,
            OtherInstructor,
            new SaveSessionProgressDto([new SaveProgressEntryDto(Child, "withhelp")]),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Substitute_CanWriteProgress()
    {
        var (service, _, session) = Build();
        session.SubstituteInstructorId = OtherInstructor;

        var result = await service.SaveSessionProgressAsync(
            session.Id,
            OtherInstructor,
            new SaveSessionProgressDto([new SaveProgressEntryDto(Child, "withhelp")]),
            CancellationToken.None);

        // Zastępstwo prowadzi te zajęcia, więc ma je czym podsumować.
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Instructor_CannotReadProgressOfAChildFromAnotherGroup()
    {
        var (service, _, _) = Build();

        Assert.Null(await service.GetParticipantProgressAsync(StrangerChild, Instructor, isAdmin: false, CancellationToken.None));
        // Admin widzi każde dziecko - to on odpowiada na reklamacje.
        Assert.NotNull(await service.GetParticipantProgressAsync(StrangerChild, Instructor, isAdmin: true, CancellationToken.None));
    }

    [Fact]
    public async Task ProjectVersions_AreAppended_NeverOverwritten()
    {
        var (service, _, _) = Build();

        var project = await service.CreateProjectAsync(
            new CreateProjectDto(Child, "Gra w labirynt"), Instructor, isAdmin: false, CancellationToken.None);

        Assert.NotNull(project);

        await service.AddSubmissionAsync(
            project!.Id,
            new AddSubmissionDto(Url: "https://scratch.mit.edu/projects/1"),
            Instructor,
            isAdmin: false,
            CancellationToken.None);

        var afterSecond = await service.AddSubmissionAsync(
            project.Id,
            new AddSubmissionDto(FileUrl: "/uploads/abc.sb3", FileName: "labirynt.sb3", SizeBytes: 1024),
            Instructor,
            isAdmin: false,
            CancellationToken.None);

        // „Projekt dziecka zniknął” prawie zawsze znaczy „został zastąpiony gorszą wersją” -
        // dlatego poprzednia wersja zostaje.
        Assert.Equal(2, afterSecond!.Submissions.Count);
        Assert.Equal([2, 1], afterSecond.Submissions.Select(item => item.Version).ToArray());

        var newest = afterSecond.Submissions[0];
        Assert.NotNull(newest.DownloadUrl);
        Assert.StartsWith("/download/project-files/", newest.DownloadUrl);

        // Wersja z linkiem nie dostaje klucza pobrania - nie ma czego pobierać.
        Assert.Null(afterSecond.Submissions[1].DownloadUrl);
    }

    [Fact]
    public async Task Submission_WithBothLinkAndFile_IsRejected()
    {
        var (service, _, _) = Build();
        var project = await service.CreateProjectAsync(
            new CreateProjectDto(Child, "Gra"), Instructor, isAdmin: false, CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddSubmissionAsync(
            project!.Id,
            new AddSubmissionDto(Url: "https://example.com", FileUrl: "/uploads/x.sb3"),
            Instructor,
            isAdmin: false,
            CancellationToken.None));

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddSubmissionAsync(
            project.Id,
            new AddSubmissionDto(),
            Instructor,
            isAdmin: false,
            CancellationToken.None));
    }

    /// <summary>
    /// Link do projektu widzi i klika rodzic w swoim portalu, więc musi być http(s).
    /// Wcześniej pole nie było sprawdzane wcale.
    /// </summary>
    [Fact]
    public async Task Submission_WithANonHttpLink_IsRejected()
    {
        var (service, _, _) = Build();
        var project = await service.CreateProjectAsync(
            new CreateProjectDto(Child, "Kotek"), Instructor, isAdmin: false, CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddSubmissionAsync(
            project!.Id,
            new AddSubmissionDto(Url: "javascript:alert(document.cookie)"),
            Instructor,
            isAdmin: false,
            CancellationToken.None));
    }

    [Fact]
    public async Task Project_ForAChildFromAnotherGroup_IsNotCreated()
    {
        var (service, _, _) = Build();

        Assert.Null(await service.CreateProjectAsync(
            new CreateProjectDto(StrangerChild, "Cudzy projekt"), Instructor, isAdmin: false, CancellationToken.None));
    }

    private static (IProgressService Service, InMemoryProgressRepository Progress, ScheduledSession Session) Build()
    {
        var groups = new InMemoryGroupRepository();
        var progress = new InMemoryProgressRepository();

        var session = new ScheduledSession
        {
            ScheduledAt = DateTimeOffset.UtcNow.AddDays(-1),
            SequenceNumber = 1,
            LessonId = Guid.NewGuid()
        };

        var group = new Group
        {
            Name = "Scratch A",
            InstructorId = Instructor,
            Enrollments = [new GroupEnrollment { ParticipantId = Child }],
            Sessions = [session]
        };

        session.GroupId = group.Id;
        groups.AddAsync(group, CancellationToken.None).GetAwaiter().GetResult();

        return (new ProgressService(progress, groups), progress, session);
    }
}
