using LessonRunner.Infrastructure.Groups;
using LessonRunner.Infrastructure.Courses;
using LessonRunner.Infrastructure.Audit;
using LessonRunner.Infrastructure.Billing;
using LessonRunner.Infrastructure.Notifications;
using LessonRunner.Infrastructure.Participants;
using LessonRunner.Infrastructure.Parents;
using LessonRunner.Infrastructure.Progress;
using LessonRunner.Infrastructure.Safety;
using LessonRunner.Infrastructure.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    internal DbSet<LessonDocument> Lessons => Set<LessonDocument>();
    internal DbSet<LessonRunSessionDocument> LessonRunSessions => Set<LessonRunSessionDocument>();
    internal DbSet<UserDocument> Users => Set<UserDocument>();
    internal DbSet<AccountTokenDocument> AccountTokens => Set<AccountTokenDocument>();
    internal DbSet<GroupDocument> Groups => Set<GroupDocument>();
    internal DbSet<GroupEnrollmentDocument> GroupEnrollments => Set<GroupEnrollmentDocument>();
    internal DbSet<ScheduledSessionDocument> ScheduledSessions => Set<ScheduledSessionDocument>();
    internal DbSet<AttendanceRecordDocument> AttendanceRecords => Set<AttendanceRecordDocument>();
    internal DbSet<SessionChangeLogDocument> SessionChangeLogs => Set<SessionChangeLogDocument>();
    internal DbSet<ParticipantDocument> Participants => Set<ParticipantDocument>();
    internal DbSet<CourseDocument> Courses => Set<CourseDocument>();
    internal DbSet<CourseLessonDocument> CourseLessons => Set<CourseLessonDocument>();
    internal DbSet<LocationDocument> Locations => Set<LocationDocument>();
    internal DbSet<HolidayDocument> Holidays => Set<HolidayDocument>();
    internal DbSet<AuditLogDocument> AuditLogs => Set<AuditLogDocument>();
    internal DbSet<NotificationLogDocument> NotificationLogs => Set<NotificationLogDocument>();
    internal DbSet<NotificationSettingsDocument> NotificationSettings => Set<NotificationSettingsDocument>();
    internal DbSet<PricePlanDocument> PricePlans => Set<PricePlanDocument>();
    internal DbSet<BillingEnrollmentDocument> BillingEnrollments => Set<BillingEnrollmentDocument>();
    internal DbSet<InvoiceDocument> Invoices => Set<InvoiceDocument>();
    internal DbSet<PaymentDocument> Payments => Set<PaymentDocument>();
    internal DbSet<LessonCreditDocument> LessonCredits => Set<LessonCreditDocument>();
    internal DbSet<ParentParticipantLinkDocument> ParentParticipantLinks => Set<ParentParticipantLinkDocument>();
    internal DbSet<ProgressEntryDocument> ProgressEntries => Set<ProgressEntryDocument>();
    internal DbSet<ProjectDocument> Projects => Set<ProjectDocument>();
    internal DbSet<ProjectSubmissionDocument> ProjectSubmissions => Set<ProjectSubmissionDocument>();
    internal DbSet<IncidentDocument> Incidents => Set<IncidentDocument>();
    internal DbSet<SupportTicketDocument> SupportTickets => Set<SupportTicketDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LessonDocument>(builder =>
        {
            builder.ToTable("Lessons");
            builder.HasKey(lesson => lesson.Id);
            builder.Property(lesson => lesson.Title).HasMaxLength(220).IsRequired();
            builder.Property(lesson => lesson.Subject).HasMaxLength(80).IsRequired();
            builder.Property(lesson => lesson.Status).HasMaxLength(40).IsRequired();
            builder.Property(lesson => lesson.DocumentJson).IsRequired();
        });

        modelBuilder.Entity<UserDocument>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Email).HasMaxLength(254).IsRequired();
            builder.HasIndex(user => user.Email).IsUnique();
            builder.Property(user => user.PasswordHash).IsRequired();
            builder.Property(user => user.Role).HasMaxLength(40).IsRequired();
            builder.Property(user => user.SecurityStamp).HasMaxLength(64).IsRequired().HasDefaultValue(string.Empty);
        });

        modelBuilder.Entity<AccountTokenDocument>(builder =>
        {
            builder.ToTable("AccountTokens");
            builder.HasKey(token => token.Id);
            // Skrót jest jednocześnie kluczem wyszukiwania - stąd indeks unikalny, a nie zwykły.
            builder.Property(token => token.TokenHash).HasMaxLength(64).IsRequired();
            builder.HasIndex(token => token.TokenHash).IsUnique();
            builder.Property(token => token.Purpose).HasMaxLength(40).IsRequired();
            builder.HasIndex(token => new { token.UserId, token.UsedAt });
        });

        modelBuilder.Entity<LessonRunSessionDocument>(builder =>
        {
            builder.ToTable("LessonRunSessions");
            builder.HasKey(session => session.Id);
            builder.HasIndex(session => new { session.LessonId, session.UserId, session.ScheduledSessionId }).IsUnique();
            builder.Property(session => session.StepIndex).IsRequired();
            builder.Property(session => session.ElapsedTotalSeconds).IsRequired();
            builder.Property(session => session.ElapsedStepSeconds).IsRequired();
            builder.Property(session => session.Running).IsRequired();
            builder.Property(session => session.StartedAt).IsRequired();
            builder.Property(session => session.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<GroupDocument>(builder =>
        {
            builder.ToTable("Groups");
            builder.HasKey(group => group.Id);
            builder.Property(group => group.Name).HasMaxLength(160).IsRequired();
            builder.Property(group => group.MeetingUrl).HasMaxLength(1000);
            builder.Property(group => group.Status).HasMaxLength(40).IsRequired();
            builder.HasIndex(group => group.InstructorId);
            builder.HasIndex(group => group.CourseId);
            builder.HasIndex(group => group.LocationId);
            builder.HasMany(group => group.Enrollments)
                .WithOne()
                .HasForeignKey(enrollment => enrollment.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(group => group.Sessions)
                .WithOne()
                .HasForeignKey(session => session.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GroupEnrollmentDocument>(builder =>
        {
            builder.ToTable("GroupEnrollments");
            builder.HasKey(enrollment => enrollment.Id);
            builder.Property(enrollment => enrollment.Status).HasMaxLength(40).IsRequired();
            builder.HasIndex(enrollment => new { enrollment.GroupId, enrollment.ParticipantId }).IsUnique();
        });

        modelBuilder.Entity<ParticipantDocument>(builder =>
        {
            builder.ToTable("Participants");
            builder.HasKey(participant => participant.Id);
            builder.Property(participant => participant.FirstName).HasMaxLength(120).IsRequired();
            builder.Property(participant => participant.LastName).HasMaxLength(120).IsRequired();
            builder.Property(participant => participant.Phone).HasMaxLength(40);
            builder.Property(participant => participant.Email).HasMaxLength(254);
            builder.Property(participant => participant.Notes).HasMaxLength(2000);
            builder.Property(participant => participant.GuardianName).HasMaxLength(200);
            builder.Property(participant => participant.GuardianPhone).HasMaxLength(40);
            builder.Property(participant => participant.GuardianEmail).HasMaxLength(254);
            builder.Property(participant => participant.GuardianRelation).HasMaxLength(60);
        });

        modelBuilder.Entity<ScheduledSessionDocument>(builder =>
        {
            builder.ToTable("ScheduledSessions");
            builder.HasKey(session => session.Id);
            builder.Property(session => session.Status).HasMaxLength(40).IsRequired();
            builder.Property(session => session.InstructorNote).HasMaxLength(4000);
            builder.Property(session => session.UnfinishedNote).HasMaxLength(2000);
            builder.Property(session => session.ParentSummary).HasMaxLength(2000);
            builder.Property(session => session.MeetingUrl).HasMaxLength(1000);
            builder.Property(session => session.RecordingUrl).HasMaxLength(1000);
            builder.HasIndex(session => session.ScheduledAt);
            builder.HasIndex(session => session.LocationId);
            builder.HasIndex(session => session.SubstituteInstructorId);
            builder.HasIndex(session => new { session.GroupId, session.SequenceNumber });
            builder.HasMany(session => session.Attendance)
                .WithOne()
                .HasForeignKey(record => record.ScheduledSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AttendanceRecordDocument>(builder =>
        {
            builder.ToTable("AttendanceRecords");
            builder.HasKey(record => record.Id);
            builder.Property(record => record.Status).HasMaxLength(40).IsRequired().HasDefaultValue(string.Empty);
            builder.Property(record => record.LiveStatus).HasMaxLength(40).IsRequired().HasDefaultValue(string.Empty);
            builder.Property(record => record.Note).HasMaxLength(500);
            builder.HasIndex(record => new { record.ScheduledSessionId, record.ParticipantId }).IsUnique();
            builder.HasIndex(record => record.MakeupSessionId);
        });

        modelBuilder.Entity<SessionChangeLogDocument>(builder =>
        {
            builder.ToTable("SessionChangeLogs");
            builder.HasKey(change => change.Id);
            builder.Property(change => change.ChangeType).HasMaxLength(40).IsRequired();
            builder.Property(change => change.Reason).HasMaxLength(1000);
            builder.Property(change => change.Details).HasMaxLength(1000);
            builder.HasIndex(change => change.ScheduledSessionId);
            builder.HasIndex(change => new { change.GroupId, change.ChangedAt });
            // Świadomie bez klucza obcego do ScheduledSessions: historia ma przetrwać
            // usunięcie grupy albo terminu - to jest jej jedyny sens przy reklamacji.
        });

        modelBuilder.Entity<CourseDocument>(builder =>
        {
            builder.ToTable("Courses");
            builder.HasKey(course => course.Id);
            builder.Property(course => course.Name).HasMaxLength(160).IsRequired();
            builder.Property(course => course.Subject).HasMaxLength(80).IsRequired();
            builder.Property(course => course.Level).HasMaxLength(80).IsRequired();
            builder.Property(course => course.Description).HasMaxLength(2000).IsRequired();
            builder.HasIndex(course => course.Name).IsUnique();
            builder.HasMany(course => course.Lessons)
                .WithOne()
                .HasForeignKey(lesson => lesson.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CourseLessonDocument>(builder =>
        {
            builder.ToTable("CourseLessons");
            builder.HasKey(lesson => lesson.Id);
            builder.HasIndex(lesson => new { lesson.CourseId, lesson.Order }).IsUnique();
            builder.HasIndex(lesson => new { lesson.CourseId, lesson.LessonId }).IsUnique();
        });

        modelBuilder.Entity<LocationDocument>(builder =>
        {
            builder.ToTable("Locations");
            builder.HasKey(location => location.Id);
            builder.Property(location => location.Name).HasMaxLength(160).IsRequired();
            builder.Property(location => location.Description).HasMaxLength(1000);
            builder.HasIndex(location => location.Name).IsUnique();
        });

        modelBuilder.Entity<HolidayDocument>(builder =>
        {
            builder.ToTable("Holidays");
            builder.HasKey(holiday => holiday.Id);
            builder.Property(holiday => holiday.Name).HasMaxLength(160).IsRequired();
            builder.HasIndex(holiday => holiday.Date).IsUnique();
        });

        modelBuilder.Entity<AuditLogDocument>(builder =>
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(log => log.Id);
            builder.Property(log => log.Action).HasMaxLength(120).IsRequired();
            builder.Property(log => log.EntityType).HasMaxLength(120).IsRequired();
            builder.Property(log => log.EntityId).HasMaxLength(120);
            builder.Property(log => log.Details).HasMaxLength(2000);
            builder.HasIndex(log => log.OccurredAt);
            builder.HasIndex(log => log.ActorUserId);
            builder.HasIndex(log => new { log.EntityType, log.EntityId });
        });

        modelBuilder.Entity<NotificationLogDocument>(builder =>
        {
            builder.ToTable("NotificationLogs");
            builder.HasKey(log => log.Id);
            builder.Property(log => log.Type).HasMaxLength(80).IsRequired();
            builder.Property(log => log.Channel).HasMaxLength(40).IsRequired();
            builder.Property(log => log.Recipient).HasMaxLength(254).IsRequired();
            builder.Property(log => log.Subject).HasMaxLength(300).IsRequired();
            builder.Property(log => log.Status).HasMaxLength(40).IsRequired();
            builder.Property(log => log.DedupeKey).HasMaxLength(220).IsRequired();
            builder.Property(log => log.Error).HasMaxLength(2000);
            builder.HasIndex(log => log.CreatedAt);
            builder.HasIndex(log => log.DedupeKey).IsUnique();
        });

        modelBuilder.Entity<NotificationSettingsDocument>(builder =>
        {
            builder.ToTable("NotificationSettings");
            builder.HasKey(settings => settings.Id);
            builder.Property(settings => settings.FromName).HasMaxLength(160).IsRequired();
            builder.Property(settings => settings.FromEmail).HasMaxLength(254).IsRequired();
            builder.Property(settings => settings.ReminderSubject).HasMaxLength(300).IsRequired();
            builder.Property(settings => settings.ReminderBody).HasMaxLength(4000).IsRequired();
            builder.Property(settings => settings.AbsenceSubject).HasMaxLength(300).IsRequired();
            builder.Property(settings => settings.AbsenceBody).HasMaxLength(4000).IsRequired();
        });

        // Rozdział 8 dokumentu: incydenty są osobnym rejestrem, nie polem przy obecności.
        // Brak kluczy obcych jest tu **celowy** - sprawa musi przetrwać usunięcie grupy
        // i anonimizację dziecka, bo to ona jest dowodem w razie sporu.
        modelBuilder.Entity<IncidentDocument>(builder =>
        {
            builder.ToTable("Incidents");
            builder.HasKey(incident => incident.Id);
            builder.Property(incident => incident.Kind).HasMaxLength(40).IsRequired();
            builder.Property(incident => incident.Severity).HasMaxLength(20).IsRequired();
            builder.Property(incident => incident.Status).HasMaxLength(20).IsRequired();
            builder.Property(incident => incident.ParticipantIds).HasMaxLength(1000).IsRequired();
            builder.Property(incident => incident.Description).HasMaxLength(4000).IsRequired();
            builder.Property(incident => incident.ActionsTaken).HasMaxLength(4000);
            builder.Property(incident => incident.Resolution).HasMaxLength(4000);
            builder.HasIndex(incident => incident.Status);
            builder.HasIndex(incident => incident.ReportedByUserId);
        });

        modelBuilder.Entity<SupportTicketDocument>(builder =>
        {
            builder.ToTable("SupportTickets");
            builder.HasKey(ticket => ticket.Id);
            builder.Property(ticket => ticket.Category).HasMaxLength(40).IsRequired();
            builder.Property(ticket => ticket.Status).HasMaxLength(20).IsRequired();
            builder.Property(ticket => ticket.Description).HasMaxLength(4000).IsRequired();
            builder.Property(ticket => ticket.Resolution).HasMaxLength(4000);
            // Najczęstsze zapytanie to „pokaż problemy tego dziecka" - historia jest tu
            // wartościowsza niż pojedyncze zgłoszenie.
            builder.HasIndex(ticket => ticket.ParticipantId);
            builder.HasIndex(ticket => ticket.Status);
        });

        modelBuilder.Entity<PricePlanDocument>(builder =>
        {
            builder.ToTable("PricePlans");
            builder.HasKey(plan => plan.Id);
            builder.Property(plan => plan.Name).HasMaxLength(160).IsRequired();
            builder.Property(plan => plan.Currency).HasMaxLength(3).IsRequired();
            builder.HasIndex(plan => plan.CourseId);
            builder.HasIndex(plan => plan.GroupId);
        });

        modelBuilder.Entity<BillingEnrollmentDocument>(builder =>
        {
            builder.ToTable("BillingEnrollments");
            builder.HasKey(enrollment => enrollment.Id);
            builder.Property(enrollment => enrollment.Status).HasMaxLength(40).IsRequired();
            builder.HasIndex(enrollment => new { enrollment.ParticipantId, enrollment.GroupId }).IsUnique();
            builder.HasIndex(enrollment => enrollment.PricePlanId);
        });

        modelBuilder.Entity<InvoiceDocument>(builder =>
        {
            builder.ToTable("Invoices");
            builder.HasKey(invoice => invoice.Id);
            builder.Property(invoice => invoice.Number).HasMaxLength(40).IsRequired();
            builder.Property(invoice => invoice.Currency).HasMaxLength(3).IsRequired();
            builder.Property(invoice => invoice.Status).HasMaxLength(40).IsRequired();
            builder.HasIndex(invoice => invoice.Number).IsUnique();
            builder.HasIndex(invoice => invoice.BillingEnrollmentId);
            builder.HasIndex(invoice => invoice.DueDate);
            builder.HasIndex(invoice => invoice.Status);
        });

        modelBuilder.Entity<PaymentDocument>(builder =>
        {
            builder.ToTable("Payments");
            builder.HasKey(payment => payment.Id);
            builder.Property(payment => payment.Provider).HasMaxLength(80).IsRequired();
            builder.Property(payment => payment.ExternalId).HasMaxLength(160);
            builder.Property(payment => payment.Currency).HasMaxLength(3).IsRequired();
            builder.Property(payment => payment.Status).HasMaxLength(40).IsRequired();
            builder.HasIndex(payment => payment.InvoiceId);
            builder.HasIndex(payment => new { payment.Provider, payment.ExternalId });
        });

        modelBuilder.Entity<LessonCreditDocument>(builder =>
        {
            builder.ToTable("LessonCredits");
            builder.HasKey(credit => credit.Id);
            builder.Property(credit => credit.Reason).HasMaxLength(1000).IsRequired();
            builder.Property(credit => credit.Currency).HasMaxLength(3);
            builder.Property(credit => credit.Status).HasMaxLength(40).IsRequired();
            builder.Property(credit => credit.Usage).HasMaxLength(40).IsRequired();
            builder.Property(credit => credit.UsageNote).HasMaxLength(1000);
            builder.HasIndex(credit => credit.ParticipantId);
            builder.HasIndex(credit => credit.SourceSessionId);
            builder.HasIndex(credit => new { credit.ParticipantId, credit.Status });
        });

        modelBuilder.Entity<ParentParticipantLinkDocument>(builder =>
        {
            builder.ToTable("ParentParticipantLinks");
            builder.HasKey(link => link.Id);
            builder.Property(link => link.Relation).HasMaxLength(60);
            builder.Property(link => link.ReceivesNotifications).HasDefaultValue(true);
            builder.HasIndex(link => new { link.ParentUserId, link.ParticipantId }).IsUnique();
            builder.HasIndex(link => link.ParticipantId);
        });

        modelBuilder.Entity<ProgressEntryDocument>(builder =>
        {
            builder.ToTable("ProgressEntries");
            builder.HasKey(entry => entry.Id);
            builder.Property(entry => entry.Autonomy).HasMaxLength(40).IsRequired();
            builder.Property(entry => entry.NoteForParent).HasMaxLength(2000);
            builder.Property(entry => entry.NextStep).HasMaxLength(2000);
            builder.HasIndex(entry => entry.ParticipantId);
            // Jeden wpis na dziecko na termin: kolejne zapisy z kokpitu mają poprawiać ten sam
            // wpis, a nie mnożyć wersje tej samej lekcji.
            builder.HasIndex(entry => new { entry.SessionId, entry.ParticipantId }).IsUnique();
        });

        modelBuilder.Entity<ProjectDocument>(builder =>
        {
            builder.ToTable("Projects");
            builder.HasKey(project => project.Id);
            builder.Property(project => project.Title).HasMaxLength(200).IsRequired();
            builder.Property(project => project.Description).HasMaxLength(2000);
            builder.HasIndex(project => project.ParticipantId);
            builder
                .HasMany(project => project.Submissions)
                .WithOne(submission => submission.Project!)
                .HasForeignKey(submission => submission.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectSubmissionDocument>(builder =>
        {
            builder.ToTable("ProjectSubmissions");
            builder.HasKey(submission => submission.Id);
            builder.Property(submission => submission.Url).HasMaxLength(1000);
            builder.Property(submission => submission.FileUrl).HasMaxLength(1000);
            builder.Property(submission => submission.FileName).HasMaxLength(260);
            builder.Property(submission => submission.ContentType).HasMaxLength(160);
            builder.Property(submission => submission.DownloadToken).HasMaxLength(64);
            builder.Property(submission => submission.InstructorComment).HasMaxLength(2000);
            builder.HasIndex(submission => submission.DownloadToken);
            builder.HasIndex(submission => new { submission.ProjectId, submission.Version }).IsUnique();
        });
    }
}
