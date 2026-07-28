using LessonRunner.Application.Groups;
using LessonRunner.Application.Participants;
using LessonRunner.Domain.Groups;
using Xunit;

namespace LessonRunner.Tests;

public sealed class ParticipantServiceTests
{
    private static (ParticipantService Service, InMemoryParticipantRepository Participants, InMemoryGroupRepository Groups) Build()
    {
        var participants = new InMemoryParticipantRepository();
        var groups = new InMemoryGroupRepository();
        var service = new ParticipantService(participants, groups);
        return (service, participants, groups);
    }

    private static async Task<Group> AddGroupAsync(InMemoryGroupRepository groups, string name = "Grupa")
    {
        var group = new Group { Name = name, InstructorId = Guid.NewGuid() };
        await groups.AddAsync(group, CancellationToken.None);
        return group;
    }

    [Fact]
    public async Task CreateAsync_AddsParticipantWithoutAnyGroup()
    {
        var (service, _, _) = Build();

        var details = await service.CreateAsync(
            new CreateParticipantDto("Jan", "Kowalski", "123456789", "jan@x.pl"),
            CancellationToken.None);

        Assert.Equal("Jan", details.FirstName);
        Assert.Equal("Kowalski", details.LastName);
        Assert.Empty(details.Groups);
    }

    [Fact]
    public async Task CreateAsync_WithGroupIds_EnrollsImmediately()
    {
        var (service, _, groups) = Build();
        var group = await AddGroupAsync(groups);

        var details = await service.CreateAsync(
            new CreateParticipantDto("Jan", "Kowalski", null, null, GroupIds: [group.Id]),
            CancellationToken.None);

        Assert.Single(details.Groups);
        Assert.Equal(group.Id, details.Groups[0].GroupId);
    }

    [Fact]
    public async Task CreateAsync_RejectsMissingName()
    {
        var (service, _, _) = Build();

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(new CreateParticipantDto("", "Kowalski", null, null), CancellationToken.None));
    }

    [Fact]
    public async Task EnrollAsync_IsIdempotent()
    {
        var (service, _, groups) = Build();
        var group = await AddGroupAsync(groups);
        var created = await service.CreateAsync(new CreateParticipantDto("Jan", "Kowalski", null, null), CancellationToken.None);

        await service.EnrollAsync(created.Id, group.Id, CancellationToken.None);
        var details = await service.EnrollAsync(created.Id, group.Id, CancellationToken.None);

        Assert.NotNull(details);
        Assert.Single(details!.Groups);
    }

    [Fact]
    public async Task EnrollAsync_RejectsUnknownGroup()
    {
        var (service, _, _) = Build();
        var created = await service.CreateAsync(new CreateParticipantDto("Jan", "Kowalski", null, null), CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.EnrollAsync(created.Id, Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task UnenrollAsync_AlwaysRemovesEnrollment()
    {
        var (service, _, groups) = Build();
        var group = await AddGroupAsync(groups);
        var created = await service.CreateAsync(
            new CreateParticipantDto("Jan", "Kowalski", null, null, GroupIds: [group.Id]),
            CancellationToken.None);

        var details = await service.UnenrollAsync(created.Id, group.Id, CancellationToken.None);

        Assert.NotNull(details);
        Assert.Empty(details!.Groups);
    }

    [Fact]
    public async Task UnenrollAsync_PromotesFirstWaitlistedParticipant()
    {
        var (service, _, groups) = Build();
        var group = await AddGroupAsync(groups);
        group.Capacity = 1;
        await groups.UpdateGroupAsync(group, CancellationToken.None);
        var jan = await service.CreateAsync(new CreateParticipantDto("Jan", "Kowalski", null, null), CancellationToken.None);
        var ola = await service.CreateAsync(new CreateParticipantDto("Ola", "Nowak", null, null), CancellationToken.None);

        await service.EnrollAsync(jan.Id, group.Id, CancellationToken.None);
        await service.EnrollAsync(ola.Id, group.Id, CancellationToken.None);
        await service.UnenrollAsync(jan.Id, group.Id, CancellationToken.None);

        var reloaded = await groups.GetByIdAsync(group.Id, CancellationToken.None);
        Assert.Equal(EnrollmentStatus.Enrolled, reloaded!.Enrollments.Single(enrollment => enrollment.ParticipantId == ola.Id).Status);
    }

    [Fact]
    public async Task GetSummariesAsync_FiltersByQuery()
    {
        var (service, _, _) = Build();
        await service.CreateAsync(new CreateParticipantDto("Jan", "Kowalski", null, null), CancellationToken.None);
        await service.CreateAsync(new CreateParticipantDto("Ola", "Nowak", null, null), CancellationToken.None);

        var result = await service.GetSummariesAsync("kowal", includeArchived: false, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Kowalski", result[0].LastName);
    }

    [Fact]
    public async Task CreateAsync_StoresGuardianAndChildDetails()
    {
        var (service, _, _) = Build();

        var details = await service.CreateAsync(
            new CreateParticipantDto(
                "Zosia", "Kowalska", null, null,
                BirthDate: new DateOnly(2016, 5, 20),
                Notes: "Alergia na orzechy",
                GuardianName: "Anna Kowalska",
                GuardianPhone: "600100200",
                GuardianEmail: "anna@x.pl",
                GuardianRelation: "mama"),
            CancellationToken.None);

        Assert.Equal(new DateOnly(2016, 5, 20), details.BirthDate);
        Assert.Equal("Alergia na orzechy", details.Notes);
        Assert.Equal("Anna Kowalska", details.GuardianName);
        Assert.Equal("mama", details.GuardianRelation);
        Assert.False(details.IsArchived);
    }

    [Fact]
    public async Task CreateAsync_RejectsBirthDateInFuture()
    {
        var (service, _, _) = Build();
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1));

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(new CreateParticipantDto("Jan", "Nowak", null, null, BirthDate: tomorrow), CancellationToken.None));
    }

    [Fact]
    public async Task GetSummariesAsync_HidesArchivedUnlessRequested()
    {
        var (service, _, _) = Build();
        var created = await service.CreateAsync(new CreateParticipantDto("Jan", "Kowalski", null, null), CancellationToken.None);

        await service.SetArchivedAsync(created.Id, true, CancellationToken.None);

        Assert.Empty(await service.GetSummariesAsync(null, includeArchived: false, CancellationToken.None));
        var withArchived = await service.GetSummariesAsync(null, includeArchived: true, CancellationToken.None);
        Assert.Single(withArchived);
        Assert.True(withArchived[0].IsArchived);

        // Przywrócenie znów pokazuje uczestnika na domyślnej liście.
        await service.SetArchivedAsync(created.Id, false, CancellationToken.None);
        Assert.Single(await service.GetSummariesAsync(null, includeArchived: false, CancellationToken.None));
    }

    [Fact]
    public async Task Consent_IsStampedOnGrantKeptOnUpdateAndClearedOnRevoke()
    {
        var (service, _, _) = Build();

        var created = await service.CreateAsync(
            new CreateParticipantDto("Jan", "Kowalski", null, null, ConsentDataProcessing: true, ConsentImage: false),
            CancellationToken.None);

        Assert.NotNull(created.DataProcessingConsentAt);
        Assert.Null(created.ImageConsentAt);
        var originalStamp = created.DataProcessingConsentAt;

        // Podtrzymanie zgody zachowuje pierwotną datę, a druga zgoda dostaje własny stempel.
        var kept = await service.UpdateAsync(
            created.Id,
            new UpdateParticipantDto("Jan", "Kowalski", null, null, ConsentDataProcessing: true, ConsentImage: true),
            CancellationToken.None);

        Assert.Equal(originalStamp, kept!.DataProcessingConsentAt);
        Assert.NotNull(kept.ImageConsentAt);

        // Cofnięcie zgody czyści datę.
        var revoked = await service.UpdateAsync(
            created.Id,
            new UpdateParticipantDto("Jan", "Kowalski", null, null, ConsentDataProcessing: false, ConsentImage: true),
            CancellationToken.None);

        Assert.Null(revoked!.DataProcessingConsentAt);
        Assert.NotNull(revoked.ImageConsentAt);
    }

    [Fact]
    public async Task AnonymizeAsync_StripsPersonalDataButKeepsAttendanceRecord()
    {
        var (service, _, groups) = Build();
        var group = await AddGroupAsync(groups);
        var created = await service.CreateAsync(
            new CreateParticipantDto(
                "Zosia", "Kowalska", "600100200", "zosia@x.pl",
                GuardianName: "Anna", ConsentDataProcessing: true, GroupIds: [group.Id]),
            CancellationToken.None);

        var session = new ScheduledSession { GroupId = group.Id, LessonId = Guid.NewGuid(), ScheduledAt = DateTimeOffset.UtcNow, SequenceNumber = 1 };
        await groups.AddSessionAsync(session, CancellationToken.None);
        await groups.SaveAttendanceAsync(
            session.Id,
            [new AttendanceRecord { ScheduledSessionId = session.Id, ParticipantId = created.Id, Present = true }],
            CancellationToken.None);

        // Twarde usunięcie zablokowane przez historię obecności.
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(created.Id, CancellationToken.None));

        // Anonimizacja czyści dane, zachowuje rekord (frekwencja pozostaje spójna).
        Assert.True(await service.AnonymizeAsync(created.Id, CancellationToken.None));

        var details = await service.GetDetailsAsync(created.Id, CancellationToken.None);
        Assert.NotNull(details);
        Assert.Equal("usunięte", details!.LastName);
        Assert.Null(details.Phone);
        Assert.Null(details.GuardianName);
        Assert.Null(details.DataProcessingConsentAt);
        Assert.True(details.IsArchived);
    }

    [Fact]
    public async Task UpdateAsync_KeepsArchivedFlag()
    {
        var (service, _, _) = Build();
        var created = await service.CreateAsync(new CreateParticipantDto("Jan", "Kowalski", null, null), CancellationToken.None);
        await service.SetArchivedAsync(created.Id, true, CancellationToken.None);

        var updated = await service.UpdateAsync(
            created.Id,
            new UpdateParticipantDto("Jan", "Kowalski", "111", null),
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.True(updated!.IsArchived); // edycja danych nie odarchiwizowuje
        Assert.Equal("111", updated.Phone);
    }

    [Fact]
    public async Task UpdateAsync_ChangesContactData()
    {
        var (service, _, _) = Build();
        var created = await service.CreateAsync(new CreateParticipantDto("Jan", "Kowalski", null, null), CancellationToken.None);

        var updated = await service.UpdateAsync(
            created.Id,
            new UpdateParticipantDto("Jan", "Kowalski", "111222333", "jan.kowalski@x.pl"),
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("111222333", updated!.Phone);
        Assert.Equal("jan.kowalski@x.pl", updated.Email);
    }

    [Fact]
    public async Task DeleteAsync_RemovesParticipantWithoutAttendance()
    {
        var (service, _, _) = Build();
        var created = await service.CreateAsync(new CreateParticipantDto("Jan", "Kowalski", null, null), CancellationToken.None);

        var deleted = await service.DeleteAsync(created.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(await service.GetDetailsAsync(created.Id, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_ThrowsWhenParticipantHasAttendanceHistory()
    {
        var (service, _, groups) = Build();
        var group = await AddGroupAsync(groups);
        var created = await service.CreateAsync(
            new CreateParticipantDto("Jan", "Kowalski", null, null, GroupIds: [group.Id]),
            CancellationToken.None);

        var session = new ScheduledSession { GroupId = group.Id, LessonId = Guid.NewGuid(), ScheduledAt = DateTimeOffset.UtcNow, SequenceNumber = 1 };
        await groups.AddSessionAsync(session, CancellationToken.None);
        await groups.SaveAttendanceAsync(
            session.Id,
            [new AttendanceRecord { ScheduledSessionId = session.Id, ParticipantId = created.Id, Present = true }],
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(created.Id, CancellationToken.None));
    }
}
