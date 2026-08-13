using LessonRunner.Application.Trials;
using LessonRunner.Domain.Trials;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Trials;

internal sealed class EfTrialRepository(AppDbContext dbContext) : ITrialRepository
{
    public async Task<IReadOnlyList<TrialLesson>> ListAsync(CancellationToken cancellationToken)
    {
        // SQLite nie sortuje po DateTimeOffset w SQL - materializujemy i porządkujemy
        // po stronie klienta, tak jak reszta repozytoriów w tym projekcie.
        var documents = await dbContext.TrialLessons.AsNoTracking().ToListAsync(cancellationToken);
        return Sorted(documents);
    }

    public async Task<IReadOnlyList<TrialLesson>> ListByInstructorAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var documents = await dbContext.TrialLessons
            .AsNoTracking()
            .Where(trial => trial.InstructorId == instructorId)
            .ToListAsync(cancellationToken);

        return Sorted(documents);
    }

    public async Task<TrialLesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.TrialLessons
            .AsNoTracking()
            .FirstOrDefaultAsync(trial => trial.Id == id, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task AddAsync(TrialLesson trial, CancellationToken cancellationToken)
    {
        dbContext.TrialLessons.Add(ToDocument(trial));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(TrialLesson trial, CancellationToken cancellationToken)
    {
        var document = await dbContext.TrialLessons.FirstOrDefaultAsync(item => item.Id == trial.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.ChildFirstName = trial.ChildFirstName;
        document.ChildLastName = trial.ChildLastName;
        document.ChildBirthDate = trial.ChildBirthDate;
        document.GuardianName = trial.GuardianName;
        document.GuardianEmail = trial.GuardianEmail;
        document.GuardianPhone = trial.GuardianPhone;
        document.Source = trial.Source;
        document.RequestNote = trial.RequestNote;
        document.InstructorId = trial.InstructorId;
        document.ScheduledAt = trial.ScheduledAt;
        document.MeetingUrl = trial.MeetingUrl;
        document.LessonId = trial.LessonId;
        document.Reading = trial.Reading.ToString();
        document.Computer = trial.Computer.ToString();
        document.Programming = trial.Programming.ToString();
        document.Recommendation = trial.Recommendation.ToString();
        document.RecommendedLevel = trial.RecommendedLevel;
        document.DiagnosisNote = trial.DiagnosisNote;
        document.DiagnosedAt = trial.DiagnosedAt;
        document.DiagnosedByUserId = trial.DiagnosedByUserId;
        document.Status = trial.Status.ToString();
        document.ParticipantId = trial.ParticipantId;
        document.DeclineReason = trial.DeclineReason;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        document.ClosedAt = trial.ClosedAt;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.TrialLessons.FirstOrDefaultAsync(trial => trial.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.TrialLessons.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Najpierw to, co czeka na ruch, potem najbliższe terminy.
    ///
    /// Lista tego modułu ma jedno zadanie: pokazać, komu nie oddzwoniliśmy. Sortowanie po
    /// dacie utworzenia spychałoby świeże zgłoszenie pod zamknięte sprawy sprzed miesiąca.
    /// </summary>
    private static IReadOnlyList<TrialLesson> Sorted(IEnumerable<TrialLessonDocument> documents) =>
        documents
            .Select(ToDomain)
            .OrderBy(trial => trial.Status.IsClosed())
            .ThenBy(trial => trial.ScheduledAt ?? DateTimeOffset.MaxValue)
            .ThenByDescending(trial => trial.CreatedAt)
            .ToList();

    private static TrialLesson ToDomain(TrialLessonDocument document) => new()
    {
        Id = document.Id,
        ChildFirstName = document.ChildFirstName,
        ChildLastName = document.ChildLastName,
        ChildBirthDate = document.ChildBirthDate,
        GuardianName = document.GuardianName,
        GuardianEmail = document.GuardianEmail,
        GuardianPhone = document.GuardianPhone,
        Source = document.Source,
        RequestNote = document.RequestNote,
        InstructorId = document.InstructorId,
        ScheduledAt = document.ScheduledAt,
        MeetingUrl = document.MeetingUrl,
        LessonId = document.LessonId,
        Reading = ParseEnum(document.Reading, ReadingSkill.Unknown),
        Computer = ParseEnum(document.Computer, ComputerSkill.Unknown),
        Programming = ParseEnum(document.Programming, ProgrammingBackground.Unknown),
        Recommendation = ParseEnum(document.Recommendation, TrialRecommendation.Undecided),
        RecommendedLevel = document.RecommendedLevel,
        DiagnosisNote = document.DiagnosisNote,
        DiagnosedAt = document.DiagnosedAt,
        DiagnosedByUserId = document.DiagnosedByUserId,
        Status = ParseEnum(document.Status, TrialStatus.Requested),
        ParticipantId = document.ParticipantId,
        DeclineReason = document.DeclineReason,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt,
        ClosedAt = document.ClosedAt
    };

    private static TrialLessonDocument ToDocument(TrialLesson trial) => new()
    {
        Id = trial.Id,
        ChildFirstName = trial.ChildFirstName,
        ChildLastName = trial.ChildLastName,
        ChildBirthDate = trial.ChildBirthDate,
        GuardianName = trial.GuardianName,
        GuardianEmail = trial.GuardianEmail,
        GuardianPhone = trial.GuardianPhone,
        Source = trial.Source,
        RequestNote = trial.RequestNote,
        InstructorId = trial.InstructorId,
        ScheduledAt = trial.ScheduledAt,
        MeetingUrl = trial.MeetingUrl,
        LessonId = trial.LessonId,
        Reading = trial.Reading.ToString(),
        Computer = trial.Computer.ToString(),
        Programming = trial.Programming.ToString(),
        Recommendation = trial.Recommendation.ToString(),
        RecommendedLevel = trial.RecommendedLevel,
        DiagnosisNote = trial.DiagnosisNote,
        DiagnosedAt = trial.DiagnosedAt,
        DiagnosedByUserId = trial.DiagnosedByUserId,
        Status = trial.Status.ToString(),
        ParticipantId = trial.ParticipantId,
        DeclineReason = trial.DeclineReason,
        CreatedAt = trial.CreatedAt,
        UpdatedAt = trial.UpdatedAt,
        ClosedAt = trial.ClosedAt
    };

    private static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct =>
        Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : fallback;
}
