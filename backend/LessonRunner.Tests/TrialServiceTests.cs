using LessonRunner.Application.Auth;
using LessonRunner.Application.Parents;
using LessonRunner.Application.Trials;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Users;
using LessonRunner.Infrastructure.Auth;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Droga od zgłoszenia do zapisanego uczestnika. Najważniejsze w tym module nie jest
/// samo CRUD, tylko dwie granice: instruktor nie dotyka cudzych kandydatur, a zapis
/// dziecka do systemu przenosi dane bez przepisywania ich ręcznie.
/// </summary>
public sealed class TrialServiceTests
{
    [Fact]
    public async Task CreateAsync_StartsAsARequestWaitingForAction()
    {
        var fixture = new Fixture();

        var trial = await fixture.Service.CreateAsync(
            new CreateTrialDto("Zofia", "Nowak", GuardianEmail: "mama@example.com", Source: "polecenie"),
            CancellationToken.None);

        Assert.Equal("requested", trial.Status);
        Assert.True(trial.NeedsAttention);
        Assert.False(trial.HasDiagnosis);
    }

    /// <summary>Status wynika z danych, a nie z osobnego przycisku: jest termin i prowadzący,
    /// to lekcja jest umówiona.</summary>
    [Fact]
    public async Task ScheduleAsync_MarksTheTrialAsScheduled_AndBackAgainWhenTheDateIsCleared()
    {
        var fixture = new Fixture();
        var trial = await fixture.CreateAsync();

        var scheduled = await fixture.Service.ScheduleAsync(
            trial.Id,
            new ScheduleTrialDto(fixture.Instructor.Id, DateTimeOffset.UtcNow.AddDays(2), "https://meet.example/abc"),
            CancellationToken.None);

        Assert.Equal("scheduled", scheduled!.Status);
        Assert.Equal(fixture.Instructor.Id, scheduled.InstructorId);

        var cleared = await fixture.Service.ScheduleAsync(
            trial.Id,
            new ScheduleTrialDto(fixture.Instructor.Id, null),
            CancellationToken.None);

        Assert.Equal("requested", cleared!.Status);
    }

    [Fact]
    public async Task ScheduleAsync_AcceptsOnlyShowcaseLessonPlans()
    {
        var fixture = new Fixture();
        var trial = await fixture.CreateAsync();
        var standard = new Lesson
        {
            Title = "Zwykła lekcja",
            Subject = "Scratch",
            Level = "Poziom 1",
            Description = "Opis",
            Kind = LessonKind.Standard
        };
        var showcase = new Lesson
        {
            Title = "Pokaz Scratcha",
            Subject = "Scratch",
            Level = "Poziom 1",
            Description = "Opis",
            Kind = LessonKind.Showcase,
            Status = LessonStatus.Ready
        };
        await fixture.Lessons.AddAsync(standard, CancellationToken.None);
        await fixture.Lessons.AddAsync(showcase, CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.ScheduleAsync(
            trial.Id,
            new ScheduleTrialDto(fixture.Instructor.Id, DateTimeOffset.UtcNow.AddDays(1), LessonId: standard.Id),
            CancellationToken.None));

        var scheduled = await fixture.Service.ScheduleAsync(
            trial.Id,
            new ScheduleTrialDto(fixture.Instructor.Id, DateTimeOffset.UtcNow.AddDays(1), LessonId: showcase.Id),
            CancellationToken.None);
        var board = await fixture.Service.GetBoardAsync(CancellationToken.None);

        Assert.Equal(showcase.Id, scheduled!.LessonId);
        Assert.Equal(60, scheduled.DurationMinutes);
        Assert.Equal(55, scheduled.EarlyLeaveAfterMinutes);
        Assert.Single(board.LessonOptions!);
        Assert.Equal(showcase.Id, board.LessonOptions![0].Id);
    }

    [Fact]
    public async Task SaveDiagnosisAsync_MovesToDiagnosedOnlyWhenTheAnswersAreComplete()
    {
        var fixture = new Fixture();
        var trial = await fixture.ScheduledAsync();

        var partial = await fixture.Service.SaveDiagnosisAsync(
            trial.Id,
            new SaveTrialDiagnosisDto("fluent", "unknown", "none", "undecided"),
            fixture.Instructor.Id,
            fixture.Instructor.Id,
            CancellationToken.None);

        Assert.Equal("scheduled", partial!.Status);
        Assert.False(partial.HasDiagnosis);

        var complete = await fixture.Service.SaveDiagnosisAsync(
            trial.Id,
            new SaveTrialDiagnosisDto("slow", "needshelp", "none", "readywithsupport", "Poziom 1", "Potrzebuje pomocy przy myszce."),
            fixture.Instructor.Id,
            fixture.Instructor.Id,
            CancellationToken.None);

        Assert.Equal("diagnosed", complete!.Status);
        Assert.True(complete.HasDiagnosis);
        Assert.True(complete.NeedsAttention);
    }

    [Fact]
    public async Task SaveDiagnosisAsync_RefusesSomeoneElsesTrial()
    {
        var fixture = new Fixture();
        var trial = await fixture.ScheduledAsync();

        var result = await fixture.Service.SaveDiagnosisAsync(
            trial.Id,
            new SaveTrialDiagnosisDto("fluent", "confident", "blocks", "ready"),
            Guid.NewGuid(),
            instructorId: Guid.NewGuid(),
            CancellationToken.None);

        Assert.Null(result);
    }

    /// <summary>
    /// Zapis do systemu: dane przenoszą się ze zgłoszenia, obserwacje z lekcji lądują
    /// w notatkach uczestnika, a opiekun dostaje konto z zaproszeniem.
    /// </summary>
    [Fact]
    public async Task EnrollAsync_CreatesTheParticipantWithTheGuardianAccount()
    {
        var fixture = new Fixture();
        var trial = await fixture.DiagnosedAsync();

        var result = await fixture.Service.EnrollAsync(trial.Id, new EnrollTrialDto(), null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result!.GuardianAccountCreated);
        Assert.True(result.InvitationSent);

        var participant = await fixture.Participants.GetByIdAsync(result.ParticipantId, CancellationToken.None);
        Assert.Equal("Zofia", participant!.FirstName);
        Assert.Equal("mama@example.com", participant.GuardianEmail);
        Assert.Contains("Potrzebuje pomocy przy myszce.", participant.Notes);
        // Zgody RODO zostają puste - nikt ich jeszcze nie udzielił.
        Assert.Null(participant.DataProcessingConsentAt);

        var links = await fixture.Links.ListByParticipantsAsync([participant.Id], CancellationToken.None);
        Assert.Single(links);

        var reloaded = await fixture.Service.GetAsync(trial.Id, null, CancellationToken.None);
        Assert.Equal("enrolled", reloaded!.Status);
        Assert.Equal(participant.Id, reloaded.ParticipantId);
    }

    [Fact]
    public async Task EnrollAsync_RefusesToEnrolTheSameChildTwice()
    {
        var fixture = new Fixture();
        var trial = await fixture.DiagnosedAsync();
        await fixture.Service.EnrollAsync(trial.Id, new EnrollTrialDto(), null, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => fixture.Service.EnrollAsync(trial.Id, new EnrollTrialDto(), null, CancellationToken.None));
    }

    /// <summary>Konto opiekuna jest dodatkiem, nie warunkiem: adres zajęty przez pracownika
    /// nie może cofnąć decyzji administracji o przyjęciu dziecka.</summary>
    [Fact]
    public async Task EnrollAsync_KeepsTheParticipantWhenTheGuardianAccountFails()
    {
        var fixture = new Fixture();
        await fixture.Users.AddAsync(
            new User { Email = "mama@example.com", PasswordHash = "h", Role = UserRole.Instructor },
            CancellationToken.None);
        var trial = await fixture.DiagnosedAsync();

        var result = await fixture.Service.EnrollAsync(trial.Id, new EnrollTrialDto(), null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result!.GuardianAccountCreated);
        Assert.NotNull(result.Error);
        Assert.NotNull(await fixture.Participants.GetByIdAsync(result.ParticipantId, CancellationToken.None));
    }

    [Fact]
    public async Task DeclineAsync_ClosesTheRequest_AndRefusesAfterEnrolment()
    {
        var fixture = new Fixture();
        var trial = await fixture.DiagnosedAsync();

        var declined = await fixture.Service.DeclineAsync(
            trial.Id,
            new DeclineTrialDto("Za daleko od naszych godzin"),
            CancellationToken.None);

        Assert.Equal("declined", declined!.Status);
        Assert.False(declined.NeedsAttention);

        var second = await fixture.DiagnosedAsync();
        await fixture.Service.EnrollAsync(second.Id, new EnrollTrialDto(CreateGuardianAccount: false), null, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => fixture.Service.DeclineAsync(second.Id, new DeclineTrialDto(), CancellationToken.None));
    }

    private sealed class Fixture
    {
        public InMemoryUserRepository Users { get; } = new();
        public InMemoryParticipantRepository Participants { get; } = new();
        public InMemoryParentPortalRepository Links { get; } = new();
        public InMemoryTrialRepository Trials { get; } = new();
        public InMemoryLessonRepository Lessons { get; } = new();
        public FakeEmailSender Emails { get; } = new();
        public User Instructor { get; }
        public ITrialService Service { get; }

        public Fixture()
        {
            var hasher = new Pbkdf2PasswordHasher();
            Instructor = new User { Email = "trener@example.com", PasswordHash = "h", Role = UserRole.Instructor };
            Users.AddAsync(Instructor, CancellationToken.None).GetAwaiter().GetResult();

            var parentPortal = new ParentPortalService(
                Links,
                Users,
                Participants,
                new InMemoryGroupRepository(),
                new InMemoryLessonRepository(),
                new InMemoryBillingRepository(),
                progressRepository: null,
                new UserAdminService(Users, hasher),
                new AccountTokenService(
                    Users,
                    new InMemoryAccountTokenRepository(),
                    hasher,
                    Emails,
                    new InMemoryNotificationRepository(),
                    new AppOptions { PublicOrigin = "https://zajecia.test" }));

            Service = new TrialService(Trials, Users, Lessons, Participants, parentPortal);
        }

        public Task<TrialLessonDto> CreateAsync() =>
            Service.CreateAsync(
                new CreateTrialDto("Zofia", "Nowak", GuardianName: "Anna Nowak", GuardianEmail: "mama@example.com"),
                CancellationToken.None);

        public async Task<TrialLessonDto> ScheduledAsync()
        {
            var trial = await CreateAsync();
            return (await Service.ScheduleAsync(
                trial.Id,
                new ScheduleTrialDto(Instructor.Id, DateTimeOffset.UtcNow.AddDays(1)),
                CancellationToken.None))!;
        }

        public async Task<TrialLessonDto> DiagnosedAsync()
        {
            var trial = await ScheduledAsync();
            return (await Service.SaveDiagnosisAsync(
                trial.Id,
                new SaveTrialDiagnosisDto("slow", "needshelp", "none", "readywithsupport", "Poziom 1", "Potrzebuje pomocy przy myszce."),
                Instructor.Id,
                Instructor.Id,
                CancellationToken.None))!;
        }
    }
}
