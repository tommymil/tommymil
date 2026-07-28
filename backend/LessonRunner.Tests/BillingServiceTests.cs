using LessonRunner.Application.Billing;
using LessonRunner.Application.Groups;
using LessonRunner.Domain.Courses;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Users;
using Xunit;

namespace LessonRunner.Tests;

public sealed class BillingServiceTests
{
    /// <summary>
    /// Zasada z dokumentu koncepcyjnego: zmiany terminu NIE łączymy z rozliczeniem.
    /// Samo odwołanie nie tworzy kredytu — dopiero świadoma decyzja `compensation: credit`.
    /// </summary>
    [Fact]
    public async Task CancelSession_WithoutCompensation_DoesNotCreateCredits()
    {
        var (groupService, billingService, group, _) = await BuildCancellationScenarioAsync();

        await groupService.CancelSessionAsync(
            group.Id,
            group.Sessions[0].Id,
            new CancelSessionDto("Instruktor chory"),
            null,
            CancellationToken.None);

        Assert.Empty(await billingService.ListCreditsAsync(null, CancellationToken.None));
    }

    [Fact]
    public async Task CancelSession_WithCreditCompensation_IssuesCreditPerEnrolledChild()
    {
        var (groupService, billingService, group, participantId) = await BuildCancellationScenarioAsync();

        await groupService.CancelSessionAsync(
            group.Id,
            group.Sessions[0].Id,
            new CancelSessionDto("Awaria prądu", Compensation: "credit"),
            null,
            CancellationToken.None);

        var credits = await billingService.ListCreditsAsync(null, CancellationToken.None);
        var credit = Assert.Single(credits);

        Assert.Equal(participantId, credit.ParticipantId);
        Assert.Equal(group.Id, credit.GroupId);
        Assert.Equal(group.Sessions[0].Id, credit.SourceSessionId);
        Assert.Equal("available", credit.Status);
        Assert.Contains("Awaria prądu", credit.Reason);
    }

    [Fact]
    public async Task UseCredit_MarksItUsed_AndBlocksSecondUse()
    {
        var (_, billingService, group, participantId) = await BuildCancellationScenarioAsync();

        var credit = await billingService.IssueCreditAsync(
            new IssueLessonCreditDto(participantId, "Odwołane zajęcia", group.Id),
            null,
            CancellationToken.None);

        var used = await billingService.UseCreditAsync(
            credit.Id,
            new UseLessonCreditDto("makeupsession", UsageNote: "Odrobione z grupą B"),
            null,
            CancellationToken.None);

        Assert.Equal("used", used!.Status);
        Assert.Equal("makeupsession", used.Usage);
        Assert.Equal("Odrabianie zajęć", used.UsageLabel);

        // Ten sam kredyt nie może zostać wykorzystany dwa razy.
        await Assert.ThrowsAsync<ArgumentException>(() => billingService.UseCreditAsync(
            credit.Id,
            new UseLessonCreditDto("invoicediscount"),
            null,
            CancellationToken.None));
    }

    [Fact]
    public async Task IssueCredit_RequiresReason()
    {
        var (_, billingService, group, participantId) = await BuildCancellationScenarioAsync();

        await Assert.ThrowsAsync<ArgumentException>(() => billingService.IssueCreditAsync(
            new IssueLessonCreditDto(participantId, "   ", group.Id),
            null,
            CancellationToken.None));
    }

    [Fact]
    public async Task ExpiredCredit_IsMarkedOnRead_AndCannotBeUsed()
    {
        var (_, billingService, group, participantId) = await BuildCancellationScenarioAsync();

        var credit = await billingService.IssueCreditAsync(
            new IssueLessonCreditDto(
                participantId,
                "Kredyt z krótkim terminem",
                group.Id,
                ExpiresAt: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))),
            null,
            CancellationToken.None);

        // Nie ma workera - status przeliczamy przy odczycie, żeby lista nie kłamała.
        var credits = await billingService.ListCreditsAsync(participantId, CancellationToken.None);
        Assert.Equal("expired", Assert.Single(credits).Status);

        await Assert.ThrowsAsync<ArgumentException>(() => billingService.UseCreditAsync(
            credit.Id,
            new UseLessonCreditDto("makeupsession"),
            null,
            CancellationToken.None));
    }

    private static async Task<(GroupService Groups, BillingService Billing, GroupDetailsDto Group, Guid ParticipantId)>
        BuildCancellationScenarioAsync()
    {
        var billing = new InMemoryBillingRepository();
        var participants = new InMemoryParticipantRepository();
        var groups = new InMemoryGroupRepository();
        var courses = new InMemoryCourseRepository();
        var lessons = new InMemoryLessonRepository();
        var users = new InMemoryUserRepository();

        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);
        var lesson = new Lesson { Title = "Scratch 1", Subject = "Scratch", Level = "P1", Description = "Opis", Status = LessonStatus.Ready };
        await lessons.AddAsync(lesson, CancellationToken.None);
        var participant = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        await participants.AddAsync(participant, CancellationToken.None);

        var billingService = new BillingService(billing, participants, groups, courses, new ManualPaymentProvider());
        var groupService = new GroupService(groups, lessons, users, participants, courses, null, billingService);

        var group = await groupService.CreateAsync(
            new CreateGroupDto("Grupa A", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow.AddDays(7), [participant.Id]),
            CancellationToken.None);

        return (groupService, billingService, group, participant.Id);
    }

    [Fact]
    public async Task BillingCycle_CreatesEnrollmentInvoiceAndManualPayment()
    {
        var billing = new InMemoryBillingRepository();
        var participants = new InMemoryParticipantRepository();
        var groups = new InMemoryGroupRepository();
        var courses = new InMemoryCourseRepository();
        var lessons = new InMemoryLessonRepository();
        var users = new InMemoryUserRepository();

        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);
        var lesson = new Lesson { Title = "Scratch 1", Subject = "Scratch", Level = "P1", Description = "Opis", Status = LessonStatus.Ready };
        await lessons.AddAsync(lesson, CancellationToken.None);
        var participant = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        await participants.AddAsync(participant, CancellationToken.None);
        var course = new Course
        {
            Name = "Scratch Start",
            Subject = "Scratch",
            Level = "P1",
            Description = "Opis"
        };
        course.Lessons.Add(new CourseLesson { CourseId = course.Id, LessonId = lesson.Id, Order = 1 });
        await courses.AddAsync(course, CancellationToken.None);

        var groupService = new GroupService(groups, lessons, users, participants, courses);
        var group = await groupService.CreateAsync(
            new CreateGroupDto(
                "Grupa A",
                instructor.Id,
                [lesson.Id],
                DateTimeOffset.UtcNow.AddDays(7),
                [participant.Id],
                course.Id),
            CancellationToken.None);

        var service = new BillingService(billing, participants, groups, courses, new ManualPaymentProvider());
        var pricePlan = await service.CreatePricePlanAsync(
            new UpsertPricePlanDto("Semestr Scratch", course.Id, null, 120000, "PLN"),
            CancellationToken.None);
        var enrollment = await service.CreateEnrollmentAsync(
            new CreateBillingEnrollmentDto(participant.Id, group.Id, pricePlan.Id, Trial: true, TrialEndsAt: new DateOnly(2026, 9, 30)),
            CancellationToken.None);
        var invoice = await service.CreateInvoiceAsync(
            new CreateInvoiceDto(enrollment.Id, new DateOnly(2026, 10, 7)),
            CancellationToken.None);

        var paid = await service.MarkInvoicePaidAsync(invoice.Id, new RecordManualPaymentDto(), CancellationToken.None);
        var overview = await service.GetOverviewAsync(CancellationToken.None);

        Assert.Equal("trial", enrollment.Status);
        Assert.Equal(120000, invoice.AmountCents);
        Assert.Equal("paid", paid!.Status);
        Assert.Equal(120000, overview.Kpis.PaidAmountCents);
        Assert.Single(overview.Payments);
    }

    [Fact]
    public async Task CreateEnrollment_RejectsParticipantOutsideGroup()
    {
        var billing = new InMemoryBillingRepository();
        var participants = new InMemoryParticipantRepository();
        var groups = new InMemoryGroupRepository();
        var courses = new InMemoryCourseRepository();
        var participant = new Participant { FirstName = "Ola", LastName = "Nowak" };
        await participants.AddAsync(participant, CancellationToken.None);
        var group = new Group { Name = "Grupa", InstructorId = Guid.NewGuid() };
        await groups.AddAsync(group, CancellationToken.None);

        var service = new BillingService(billing, participants, groups, courses, new ManualPaymentProvider());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateEnrollmentAsync(new CreateBillingEnrollmentDto(participant.Id, group.Id, null), CancellationToken.None));
    }
}
