using LessonRunner.Application.Auth;
using LessonRunner.Infrastructure.Auth;
using LessonRunner.Application.Billing;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Parents;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Users;
using Xunit;

namespace LessonRunner.Tests;

public sealed class ParentPortalServiceTests
{
    [Fact]
    public async Task GetPortalAsync_ReturnsOnlyLinkedParticipantData()
    {
        var parentLinks = new InMemoryParentPortalRepository();
        var users = new InMemoryUserRepository();
        var participants = new InMemoryParticipantRepository();
        var groups = new InMemoryGroupRepository();
        var lessons = new InMemoryLessonRepository();
        var billing = new InMemoryBillingRepository();
        var courses = new InMemoryCourseRepository();

        var parent = new User { Email = "parent@example.com", PasswordHash = "h", Role = UserRole.Parent };
        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(parent, CancellationToken.None);
        await users.AddAsync(instructor, CancellationToken.None);

        var lesson = new Lesson { Title = "Scratch", Subject = "Scratch", Level = "P1", Description = "Opis", Status = LessonStatus.Ready };
        await lessons.AddAsync(lesson, CancellationToken.None);
        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        var ola = new Participant { FirstName = "Ola", LastName = "Nowak" };
        await participants.AddAsync(jan, CancellationToken.None);
        await participants.AddAsync(ola, CancellationToken.None);

        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = instructor.Id,
            MeetingUrl = "https://meet.google.com/abc-defg-hij",
            Enrollments =
            [
                new GroupEnrollment { ParticipantId = jan.Id, Status = EnrollmentStatus.Enrolled },
                new GroupEnrollment { ParticipantId = ola.Id, Status = EnrollmentStatus.Enrolled }
            ],
            Sessions =
            [
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(2),
                    SequenceNumber = 1,
                    Status = ScheduledSessionStatus.Planned
                },
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(-2),
                    SequenceNumber = 0,
                    Status = ScheduledSessionStatus.Completed,
                    Attendance = [new AttendanceRecord { ParticipantId = jan.Id, Present = true }]
                }
            ]
        };
        foreach (var enrollment in group.Enrollments)
        {
            enrollment.GroupId = group.Id;
        }
        foreach (var session in group.Sessions)
        {
            session.GroupId = group.Id;
            foreach (var record in session.Attendance)
            {
                record.ScheduledSessionId = session.Id;
            }
        }
        await groups.AddAsync(group, CancellationToken.None);

        var service = new ParentPortalService(parentLinks, users, participants, groups, lessons, billing);
        await service.LinkAsync(parent.Id, jan.Id, null, true, true, CancellationToken.None);

        var portal = await service.GetPortalAsync(parent.Id, CancellationToken.None);

        Assert.Single(portal.Children);
        Assert.Equal(jan.Id, portal.Children[0].ParticipantId);
        Assert.DoesNotContain(portal.Children, child => child.ParticipantId == ola.Id);
        Assert.Single(portal.Schedule);
        Assert.Equal("https://meet.google.com/abc-defg-hij", portal.Schedule[0].MeetingUrl);
        // Konto instruktora nie ma wpisanego imienia. Rodzic ma zobaczyć rolę, a nie
        // służbowy adres e-mail pracownika - `DisplayName` zwróciłby tu „i@example.com".
        Assert.Equal("Instruktor", portal.Schedule[0].InstructorName);
        Assert.Single(portal.Attendance);
        Assert.Equal(100, portal.Attendance[0].RatePercent);
    }

    [Fact]
    public async Task GetPortalAsync_PrefersSessionMeetingUrl_AndSharesMaterialsOnlyAfterCompletedSession()
    {
        var parentLinks = new InMemoryParentPortalRepository();
        var users = new InMemoryUserRepository();
        var participants = new InMemoryParticipantRepository();
        var groups = new InMemoryGroupRepository();
        var lessons = new InMemoryLessonRepository();
        var billing = new InMemoryBillingRepository();

        var parent = new User { Email = "parent@example.com", PasswordHash = "h", Role = UserRole.Parent };
        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(parent, CancellationToken.None);
        await users.AddAsync(instructor, CancellationToken.None);

        var lesson = new Lesson
        {
            Title = "Scratch",
            Subject = "Scratch",
            Level = "P1",
            Description = "Opis",
            Status = LessonStatus.Ready,
            ProjectFiles = new LessonProjectFiles
            {
                Final = new LessonProjectFile
                {
                    Label = "Projekt końcowy",
                    Url = "/uploads/abc.sb3",
                    FileName = "kotek.sb3",
                    ContentType = "application/zip",
                    SizeBytes = 2048,
                    DownloadToken = "token-do-pobrania-projektu-123"
                }
            }
        };
        await lessons.AddAsync(lesson, CancellationToken.None);

        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        await participants.AddAsync(jan, CancellationToken.None);

        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = instructor.Id,
            MeetingUrl = "https://meet.google.com/grupowy-link",
            Enrollments = [new GroupEnrollment { ParticipantId = jan.Id, Status = EnrollmentStatus.Enrolled }],
            Sessions =
            [
                // Zastępstwo prowadzi u siebie - link terminu ma wygrać z linkiem grupy.
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(2),
                    SequenceNumber = 2,
                    Status = ScheduledSessionStatus.Planned,
                    MeetingUrl = "https://zoom.us/j/zastepstwo"
                },
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(-2),
                    SequenceNumber = 1,
                    Status = ScheduledSessionStatus.Completed,
                    RecordingUrl = "https://nagrania.example.com/1",
                    Attendance = [new AttendanceRecord { ParticipantId = jan.Id, Present = true }]
                }
            ]
        };
        group.Enrollments[0].GroupId = group.Id;
        foreach (var session in group.Sessions)
        {
            session.GroupId = group.Id;
            foreach (var record in session.Attendance)
            {
                record.ScheduledSessionId = session.Id;
            }
        }
        await groups.AddAsync(group, CancellationToken.None);

        var service = new ParentPortalService(parentLinks, users, participants, groups, lessons, billing);
        await service.LinkAsync(parent.Id, jan.Id, null, true, true, CancellationToken.None);

        var portal = await service.GetPortalAsync(parent.Id, CancellationToken.None);

        Assert.Single(portal.Schedule);
        Assert.Equal("https://zoom.us/j/zastepstwo", portal.Schedule[0].MeetingUrl);

        // Materiały wyłącznie z terminu zakończonego - przed zajęciami nic nie udostępniamy.
        Assert.Single(portal.Materials);
        Assert.Equal("https://nagrania.example.com/1", portal.Materials[0].RecordingUrl);
        Assert.Single(portal.Materials[0].Files);
        Assert.Equal("kotek.sb3", portal.Materials[0].Files[0].FileName);
        Assert.Equal("/download/lesson-files/token-do-pobrania-projektu-123", portal.Materials[0].Files[0].DownloadUrl);
    }

    /// <summary>
    /// Domyka pętlę z etapu A1: status „nieobecność zgłoszona" istniał, ale nic nie mogło go
    /// ustawić od strony rodzica. Teraz rodzic zgłasza sam, a instruktor widzi to na liście.
    /// </summary>
    [Fact]
    public async Task ReportAbsenceAsync_SetsExcusedAbsence_AndShowsUpInPortal()
    {
        var (service, parent, jan, group) = await BuildPortalScenarioAsync();
        var upcomingSessionId = group.Sessions.First(session => session.Status == ScheduledSessionStatus.Planned).Id;

        Assert.True(await service.ReportAbsenceAsync(
            parent.Id,
            upcomingSessionId,
            new ReportAbsenceDto(jan.Id, "Wyjazd rodzinny"),
            CancellationToken.None));

        var portal = await service.GetPortalAsync(parent.Id, CancellationToken.None);
        var termin = portal.Schedule.Single(item => item.SessionId == upcomingSessionId);
        var dziecko = Assert.Single(termin.Children!);

        Assert.True(dziecko.AbsenceReported);
        Assert.Equal("Wyjazd rodzinny", dziecko.AbsenceNote);
    }

    [Fact]
    public async Task ReportAbsenceAsync_RejectsChildOfAnotherParent()
    {
        var (service, _, jan, group) = await BuildPortalScenarioAsync();
        var upcomingSessionId = group.Sessions.First(session => session.Status == ScheduledSessionStatus.Planned).Id;
        var obcyRodzic = Guid.NewGuid();

        Assert.False(await service.ReportAbsenceAsync(
            obcyRodzic,
            upcomingSessionId,
            new ReportAbsenceDto(jan.Id),
            CancellationToken.None));
    }

    [Fact]
    public async Task ReportAbsenceAsync_RejectsSessionThatAlreadyHappened()
    {
        var (service, parent, jan, group) = await BuildPortalScenarioAsync();
        var pastSessionId = group.Sessions.First(session => session.Status == ScheduledSessionStatus.Completed).Id;

        // Po zajęciach liczy się to, co odhaczył instruktor - nie deklaracja rodzica.
        Assert.False(await service.ReportAbsenceAsync(
            parent.Id,
            pastSessionId,
            new ReportAbsenceDto(jan.Id),
            CancellationToken.None));
    }

    private static async Task<(ParentPortalService Service, User Parent, Participant Jan, Group Group)>
        BuildPortalScenarioAsync()
    {
        var parentLinks = new InMemoryParentPortalRepository();
        var users = new InMemoryUserRepository();
        var participants = new InMemoryParticipantRepository();
        var groups = new InMemoryGroupRepository();
        var lessons = new InMemoryLessonRepository();
        var billing = new InMemoryBillingRepository();

        var parent = new User { Email = "parent@example.com", PasswordHash = "h", Role = UserRole.Parent };
        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(parent, CancellationToken.None);
        await users.AddAsync(instructor, CancellationToken.None);

        var lesson = new Lesson { Title = "Scratch", Subject = "Scratch", Level = "P1", Description = "Opis", Status = LessonStatus.Ready };
        await lessons.AddAsync(lesson, CancellationToken.None);

        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        await participants.AddAsync(jan, CancellationToken.None);

        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = instructor.Id,
            Enrollments = [new GroupEnrollment { ParticipantId = jan.Id, Status = EnrollmentStatus.Enrolled }],
            Sessions =
            [
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(3),
                    SequenceNumber = 2,
                    Status = ScheduledSessionStatus.Planned
                },
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(-3),
                    SequenceNumber = 1,
                    Status = ScheduledSessionStatus.Completed
                }
            ]
        };
        group.Enrollments[0].GroupId = group.Id;
        foreach (var session in group.Sessions)
        {
            session.GroupId = group.Id;
        }
        await groups.AddAsync(group, CancellationToken.None);

        var service = new ParentPortalService(parentLinks, users, participants, groups, lessons, billing);
        await service.LinkAsync(parent.Id, jan.Id, "mama", true, true, CancellationToken.None);

        return (service, parent, jan, group);
    }

    [Fact]
    public async Task LinkAsync_RejectsNonParentAccount()
    {
        var users = new InMemoryUserRepository();
        var participants = new InMemoryParticipantRepository();
        var admin = new User { Email = "admin@example.com", PasswordHash = "h", Role = UserRole.Admin };
        var child = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        await users.AddAsync(admin, CancellationToken.None);
        await participants.AddAsync(child, CancellationToken.None);

        var service = new ParentPortalService(
            new InMemoryParentPortalRepository(),
            users,
            participants,
            new InMemoryGroupRepository(),
            new InMemoryLessonRepository(),
            new InMemoryBillingRepository());

        await Assert.ThrowsAsync<ArgumentException>(() => service.LinkAsync(admin.Id, child.Id, null, true, true, CancellationToken.None));
    }

    /// <summary>
    /// Konto opiekuna z karty dziecka: powstaje użytkownik z roli `Parent`, powiązanie
    /// i zaproszenie - wszystko z danych, które są już przy uczestniku.
    /// </summary>
    [Fact]
    public async Task CreateGuardianAccountAsync_CreatesLinksAndInvites()
    {
        var fixture = new GuardianAccountFixture();
        var child = new Participant
        {
            FirstName = "Zofia",
            LastName = "Kowalska",
            GuardianName = "Katarzyna Kowalska",
            GuardianEmail = "Katarzyna.Kowalska@Example.com",
            GuardianPhone = "600500600",
            GuardianRelation = "mama"
        };
        await fixture.Participants.AddAsync(child, CancellationToken.None);

        var result = await fixture.Service.CreateGuardianAccountAsync(child.Id, actingUserId: null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result!.Created);
        Assert.True(result.InvitationSent);
        // Adres normalizujemy - inaczej ten sam opiekun wpisany raz z wielkiej litery
        // dostałby drugie konto przy drugim dziecku.
        Assert.Equal("katarzyna.kowalska@example.com", result.Email);

        var created = await fixture.Users.GetByIdAsync(result.ParentUserId, CancellationToken.None);
        Assert.Equal(UserRole.Parent, created!.Role);
        Assert.Equal("Katarzyna", created.FirstName);
        Assert.Equal("Kowalska", created.LastName);

        var links = await fixture.Links.ListByParentAsync(result.ParentUserId, CancellationToken.None);
        Assert.Single(links);
        Assert.Equal(child.Id, links[0].ParticipantId);
        Assert.Equal("mama", links[0].Relation);
        Assert.Single(fixture.Emails.Messages);
    }

    /// <summary>
    /// Drugie dziecko tej samej rodziny: konto już jest, więc dopinamy wyłącznie powiązanie.
    /// Ponowny mail z linkiem do ustawiania hasła wyglądałby jak próba przejęcia konta.
    /// </summary>
    [Fact]
    public async Task CreateGuardianAccountAsync_ReusesExistingAccount_WithoutSecondInvitation()
    {
        var fixture = new GuardianAccountFixture();
        var first = new Participant
        {
            FirstName = "Zofia",
            LastName = "Kowalska",
            GuardianName = "Katarzyna Kowalska",
            GuardianEmail = "katarzyna@example.com"
        };
        var second = new Participant
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            GuardianName = "Katarzyna Kowalska",
            GuardianEmail = "katarzyna@example.com"
        };
        await fixture.Participants.AddAsync(first, CancellationToken.None);
        await fixture.Participants.AddAsync(second, CancellationToken.None);

        var created = await fixture.Service.CreateGuardianAccountAsync(first.Id, null, CancellationToken.None);
        var reused = await fixture.Service.CreateGuardianAccountAsync(second.Id, null, CancellationToken.None);

        Assert.False(reused!.Created);
        Assert.False(reused.InvitationSent);
        Assert.Equal(created!.ParentUserId, reused.ParentUserId);
        Assert.Equal(2, (await fixture.Links.ListByParentAsync(reused.ParentUserId, CancellationToken.None)).Count);
        Assert.Single(fixture.Emails.Messages);
    }

    [Fact]
    public async Task CreateGuardianAccountAsync_RejectsChildWithoutGuardianEmail()
    {
        var fixture = new GuardianAccountFixture();
        var child = new Participant { FirstName = "Maja", LastName = "Wiśniewska", GuardianName = "Ewa Wiśniewska" };
        await fixture.Participants.AddAsync(child, CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(
            () => fixture.Service.CreateGuardianAccountAsync(child.Id, null, CancellationToken.None));
    }

    /// <summary>
    /// Adres należący do instruktora albo administratora. Podniesienie takiego konta do
    /// roli rodzica albo dopięcie mu dziecka po cichu zmieniłoby zakres uprawnień pracownika.
    /// </summary>
    [Fact]
    public async Task CreateGuardianAccountAsync_RejectsEmailOfStaffAccount()
    {
        var fixture = new GuardianAccountFixture();
        await fixture.Users.AddAsync(
            new User { Email = "trener@example.com", PasswordHash = "h", Role = UserRole.Instructor },
            CancellationToken.None);
        var child = new Participant
        {
            FirstName = "Antoni",
            LastName = "Nowak",
            GuardianName = "Tomasz Trener",
            GuardianEmail = "trener@example.com"
        };
        await fixture.Participants.AddAsync(child, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => fixture.Service.CreateGuardianAccountAsync(child.Id, null, CancellationToken.None));
    }

    private sealed class GuardianAccountFixture
    {
        public InMemoryUserRepository Users { get; } = new();
        public InMemoryParticipantRepository Participants { get; } = new();
        public InMemoryParentPortalRepository Links { get; } = new();
        public FakeEmailSender Emails { get; } = new();

        public ParentPortalService Service { get; }

        public GuardianAccountFixture()
        {
            var hasher = new Pbkdf2PasswordHasher();

            Service = new ParentPortalService(
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
        }
    }
}
