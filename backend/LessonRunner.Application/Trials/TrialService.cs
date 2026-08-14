using LessonRunner.Application.Auth;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Parents;
using LessonRunner.Application.Participants;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Trials;
using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Trials;

public sealed class TrialService(
    ITrialRepository trialRepository,
    IUserRepository userRepository,
    ILessonRepository lessonRepository,
    IParticipantRepository participantRepository,
    // Zakładanie konta opiekunowi ma już jedno miejsce w systemie (karta dziecka).
    // Powielenie go tutaj oznaczałoby dwie ścieżki, które rozjadą się przy pierwszej zmianie.
    IParentPortalService parentPortalService) : ITrialService
{
    public async Task<TrialBoardDto> GetBoardAsync(CancellationToken cancellationToken) =>
        await BuildBoardAsync(await trialRepository.ListAsync(cancellationToken), cancellationToken);

    public async Task<TrialBoardDto> GetInstructorBoardAsync(Guid instructorId, CancellationToken cancellationToken) =>
        await BuildBoardAsync(await trialRepository.ListByInstructorAsync(instructorId, cancellationToken), cancellationToken);

    public async Task<TrialLessonDto?> GetAsync(Guid id, Guid? instructorId, CancellationToken cancellationToken)
    {
        var trial = await trialRepository.GetByIdAsync(id, cancellationToken);

        // Instruktor pyta o cudzą lekcję próbną - odpowiadamy tak samo jak przy nieistniejącej.
        // Informacja „taka lekcja istnieje, ale nie twoja” też jest informacją o kandydacie.
        if (trial is null || (instructorId is not null && trial.InstructorId != instructorId))
        {
            return null;
        }

        return await ToDtoAsync(trial, cancellationToken);
    }

    public async Task<TrialLessonDto> CreateAsync(CreateTrialDto dto, CancellationToken cancellationToken)
    {
        var firstName = Require(dto.ChildFirstName, "imię dziecka");
        var lastName = Require(dto.ChildLastName, "nazwisko dziecka");

        var trial = new TrialLesson
        {
            ChildFirstName = firstName,
            ChildLastName = lastName,
            ChildBirthDate = dto.ChildBirthDate,
            GuardianName = Clean(dto.GuardianName),
            GuardianEmail = Clean(dto.GuardianEmail),
            GuardianPhone = Clean(dto.GuardianPhone),
            Source = Clean(dto.Source),
            RequestNote = Clean(dto.RequestNote)
        };

        await trialRepository.AddAsync(trial, cancellationToken);
        return await ToDtoAsync(trial, cancellationToken);
    }

    public async Task<TrialLessonDto?> ScheduleAsync(Guid id, ScheduleTrialDto dto, CancellationToken cancellationToken)
    {
        var trial = await trialRepository.GetByIdAsync(id, cancellationToken);

        if (trial is null)
        {
            return null;
        }

        if (trial.Status.IsClosed())
        {
            throw new InvalidOperationException("Zgłoszenie jest zamknięte — nie da się już zmienić terminu.");
        }

        if (dto.InstructorId is Guid instructorId)
        {
            var instructor = await userRepository.GetByIdAsync(instructorId, cancellationToken)
                ?? throw new ArgumentException("Wybrany instruktor nie istnieje.");

            if (instructor.Role is not (UserRole.Instructor or UserRole.Admin))
            {
                throw new ArgumentException("Lekcję próbną może poprowadzić wyłącznie instruktor albo administrator.");
            }
        }

        if (dto.LessonId is Guid lessonId)
        {
            var lesson = await lessonRepository.GetByIdAsync(lessonId, cancellationToken)
                ?? throw new ArgumentException("Wybrany konspekt nie istnieje.");

            if (lesson.Kind != LessonKind.Showcase)
            {
                throw new ArgumentException("Do lekcji pokazowej można przypisać wyłącznie konspekt rodzaju „Pokazowa”.");
            }

            if (lesson.Status != LessonStatus.Ready)
            {
                throw new ArgumentException("Konspekt lekcji pokazowej musi być opublikowany przed przypisaniem do terminu.");
            }
        }

        trial.InstructorId = dto.InstructorId;
        trial.ScheduledAt = dto.ScheduledAt;
        trial.MeetingUrl = Clean(dto.MeetingUrl);
        trial.LessonId = dto.LessonId;

        // Status wynika z danych, a nie z osobnego przycisku: jest termin i prowadzący,
        // to lekcja jest umówiona. Skasowanie terminu cofa zgłoszenie na początek.
        trial.Status = dto is { ScheduledAt: not null, InstructorId: not null }
            ? TrialStatus.Scheduled
            : TrialStatus.Requested;

        trial.UpdatedAt = DateTimeOffset.UtcNow;
        await trialRepository.UpdateAsync(trial, cancellationToken);

        return await ToDtoAsync(trial, cancellationToken);
    }

    public async Task<TrialLessonDto?> SaveDiagnosisAsync(
        Guid id,
        SaveTrialDiagnosisDto dto,
        Guid userId,
        Guid? instructorId,
        CancellationToken cancellationToken)
    {
        var trial = await trialRepository.GetByIdAsync(id, cancellationToken);

        if (trial is null || (instructorId is not null && trial.InstructorId != instructorId))
        {
            return null;
        }

        trial.Reading = ParseEnum(dto.Reading, ReadingSkill.Unknown);
        trial.Computer = ParseEnum(dto.Computer, ComputerSkill.Unknown);
        trial.Programming = ParseEnum(dto.Programming, ProgrammingBackground.Unknown);
        trial.Recommendation = ParseEnum(dto.Recommendation, TrialRecommendation.Undecided);
        trial.RecommendedLevel = Clean(dto.RecommendedLevel);
        trial.DiagnosisNote = Clean(dto.DiagnosisNote);
        trial.DiagnosedAt = DateTimeOffset.UtcNow;
        trial.DiagnosedByUserId = userId;

        // Do stanu „po lekcji” przechodzimy dopiero przy komplecie odpowiedzi. Instruktor
        // może zapisać diagnozę w kawałkach, ale administracja ma zobaczyć na liście
        // wyłącznie te sprawy, w których faktycznie jest już co decydować.
        if (trial.HasDiagnosis && !trial.Status.IsClosed())
        {
            trial.Status = TrialStatus.Diagnosed;
        }

        trial.UpdatedAt = DateTimeOffset.UtcNow;
        await trialRepository.UpdateAsync(trial, cancellationToken);

        return await ToDtoAsync(trial, cancellationToken);
    }

    /// <summary>
    /// Zapis dziecka do systemu — moment, w którym kandydat staje się uczestnikiem.
    ///
    /// Dane przenosimy, a nie przepisujemy ręcznie: imię, nazwisko, data urodzenia i komplet
    /// kontaktu do opiekuna są już w zgłoszeniu. Obserwacje z lekcji trafiają do notatek
    /// uczestnika, bo to jedyna część diagnozy przydatna prowadzącemu na pierwszych zajęciach.
    /// Zgód RODO **nie przenosimy** — nikt ich jeszcze nie udzielił, a domyślna zgoda
    /// wpisana przez system byłaby zgodą, której nie ma.
    /// </summary>
    public async Task<TrialEnrollmentResultDto?> EnrollAsync(
        Guid id,
        EnrollTrialDto dto,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var trial = await trialRepository.GetByIdAsync(id, cancellationToken);

        if (trial is null)
        {
            return null;
        }

        if (trial.ParticipantId is not null)
        {
            throw new InvalidOperationException("To dziecko jest już zapisane w systemie.");
        }

        var participant = new Participant
        {
            FirstName = trial.ChildFirstName,
            LastName = trial.ChildLastName,
            BirthDate = trial.ChildBirthDate,
            GuardianName = trial.GuardianName,
            GuardianEmail = trial.GuardianEmail,
            GuardianPhone = trial.GuardianPhone,
            Notes = BuildNotes(trial)
        };

        await participantRepository.AddAsync(participant, cancellationToken);

        trial.ParticipantId = participant.Id;
        trial.Status = TrialStatus.Enrolled;
        trial.ClosedAt = DateTimeOffset.UtcNow;
        trial.UpdatedAt = DateTimeOffset.UtcNow;
        await trialRepository.UpdateAsync(trial, cancellationToken);

        if (!dto.CreateGuardianAccount || string.IsNullOrWhiteSpace(trial.GuardianEmail))
        {
            return new TrialEnrollmentResultDto(
                participant.Id,
                participant.FirstName + " " + participant.LastName,
                GuardianAccountCreated: false,
                InvitationSent: false,
                trial.GuardianEmail,
                Error: null);
        }

        // Konto opiekuna jest miłym dodatkiem, a nie warunkiem zapisu. Gdyby padło (adres
        // zajęty przez pracownika, awaria poczty), dziecko i tak zostaje uczestnikiem —
        // inaczej jeden błąd wysyłki cofałby całą decyzję administracji.
        try
        {
            var account = await parentPortalService.CreateGuardianAccountAsync(participant.Id, actingUserId, cancellationToken);

            return new TrialEnrollmentResultDto(
                participant.Id,
                participant.FirstName + " " + participant.LastName,
                account?.Created ?? false,
                account?.InvitationSent ?? false,
                account?.Email ?? trial.GuardianEmail,
                account?.Error);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return new TrialEnrollmentResultDto(
                participant.Id,
                participant.FirstName + " " + participant.LastName,
                GuardianAccountCreated: false,
                InvitationSent: false,
                trial.GuardianEmail,
                exception.Message);
        }
    }

    public async Task<TrialLessonDto?> DeclineAsync(Guid id, DeclineTrialDto dto, CancellationToken cancellationToken)
    {
        var trial = await trialRepository.GetByIdAsync(id, cancellationToken);

        if (trial is null)
        {
            return null;
        }

        if (trial.Status == TrialStatus.Enrolled)
        {
            throw new InvalidOperationException("Dziecko jest już zapisane — zamknij sprawę w kartotece uczestnika.");
        }

        trial.Status = dto.NoShow ? TrialStatus.NoShow : TrialStatus.Declined;
        trial.DeclineReason = Clean(dto.Reason);
        trial.ClosedAt = DateTimeOffset.UtcNow;
        trial.UpdatedAt = DateTimeOffset.UtcNow;
        await trialRepository.UpdateAsync(trial, cancellationToken);

        return await ToDtoAsync(trial, cancellationToken);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        trialRepository.DeleteAsync(id, cancellationToken);

    /// <summary>
    /// Notatka startowa uczestnika złożona z diagnozy.
    ///
    /// Prowadzący pierwszych zajęć nie ma dostępu do zgłoszenia i nie powinien go mieć —
    /// ale musi wiedzieć, że dziecko czyta wolno albo nie radzi sobie z myszką.
    /// </summary>
    private static string? BuildNotes(TrialLesson trial)
    {
        var parts = new List<string>();

        if (trial.HasDiagnosis)
        {
            parts.Add($"Z lekcji próbnej: {trial.Reading.Label().ToLowerInvariant()}, {trial.Computer.Label().ToLowerInvariant()}, {trial.Programming.Label().ToLowerInvariant()}.");
        }

        if (!string.IsNullOrWhiteSpace(trial.RecommendedLevel))
        {
            parts.Add($"Rekomendowany poziom: {trial.RecommendedLevel!.Trim()}.");
        }

        if (!string.IsNullOrWhiteSpace(trial.DiagnosisNote))
        {
            parts.Add(trial.DiagnosisNote!.Trim());
        }

        return parts.Count == 0 ? null : string.Join(" ", parts);
    }

    private async Task<TrialBoardDto> BuildBoardAsync(IReadOnlyList<TrialLesson> trials, CancellationToken cancellationToken)
    {
        var dtos = new List<TrialLessonDto>(trials.Count);
        var instructorNames = await InstructorNamesAsync(cancellationToken);
        var lessons = await lessonRepository.ListAsync(cancellationToken);
        var lessonTitles = lessons.ToDictionary(lesson => lesson.Id, lesson => lesson.Title);

        foreach (var trial in trials)
        {
            dtos.Add(ToDto(trial, instructorNames, lessonTitles));
        }

        return new TrialBoardDto(
            dtos,
            Options<ReadingSkill>(value => value.Name(), value => value.Label()),
            Options<ComputerSkill>(value => value.Name(), value => value.Label()),
            Options<ProgrammingBackground>(value => value.Name(), value => value.Label()),
            Options<TrialRecommendation>(value => value.Name(), value => value.Label()),
            Options<TrialStatus>(value => value.Name(), value => value.Label()),
            lessons
                .Where(lesson => lesson.Kind == LessonKind.Showcase && lesson.Status == LessonStatus.Ready)
                .OrderBy(lesson => lesson.Title)
                .Select(lesson => new TrialLessonOptionDto(
                    lesson.Id,
                    lesson.Title,
                    lesson.Kind.ScheduledDurationMinutes(),
                    lesson.Kind.EarlyLeaveAfterMinutes() ?? LessonKindExtensions.ShowcaseEarlyLeaveAfterMinutes))
                .ToList());
    }

    private async Task<TrialLessonDto> ToDtoAsync(TrialLesson trial, CancellationToken cancellationToken) =>
        ToDto(trial, await InstructorNamesAsync(cancellationToken), await LessonTitlesAsync(cancellationToken));

    private async Task<Dictionary<Guid, string>> InstructorNamesAsync(CancellationToken cancellationToken) =>
        (await userRepository.ListAsync(cancellationToken)).ToDictionary(user => user.Id, user => user.DisplayName);

    private async Task<Dictionary<Guid, string>> LessonTitlesAsync(CancellationToken cancellationToken) =>
        (await lessonRepository.ListAsync(cancellationToken)).ToDictionary(lesson => lesson.Id, lesson => lesson.Title);

    private static TrialLessonDto ToDto(
        TrialLesson trial,
        IReadOnlyDictionary<Guid, string> instructorNames,
        IReadOnlyDictionary<Guid, string> lessonTitles) =>
        new(
            trial.Id,
            trial.ChildFirstName,
            trial.ChildLastName,
            trial.ChildBirthDate,
            Age(trial.ChildBirthDate),
            trial.GuardianName,
            trial.GuardianEmail,
            trial.GuardianPhone,
            trial.Source,
            trial.RequestNote,
            trial.InstructorId,
            trial.InstructorId is Guid instructorId ? instructorNames.GetValueOrDefault(instructorId) : null,
            trial.ScheduledAt,
            trial.MeetingUrl,
            trial.LessonId,
            trial.LessonId is Guid lessonId ? lessonTitles.GetValueOrDefault(lessonId) : null,
            trial.Reading.Name(),
            trial.Reading.Label(),
            trial.Computer.Name(),
            trial.Computer.Label(),
            trial.Programming.Name(),
            trial.Programming.Label(),
            trial.Recommendation.Name(),
            trial.Recommendation.Label(),
            trial.RecommendedLevel,
            trial.DiagnosisNote,
            trial.DiagnosedAt,
            trial.Status.Name(),
            trial.Status.Label(),
            trial.ParticipantId,
            trial.DeclineReason,
            trial.CreatedAt,
            trial.ClosedAt,
            trial.HasDiagnosis,
            trial.NeedsAttention());

    /// <summary>Wiek w pełnych latach — pierwsze pytanie przy kwalifikacji na kurs.</summary>
    private static int? Age(DateOnly? birthDate)
    {
        if (birthDate is null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);
        var age = today.Year - birthDate.Value.Year;

        if (birthDate.Value > today.AddYears(-age))
        {
            age--;
        }

        return age is >= 0 and <= 120 ? age : null;
    }

    private static IReadOnlyList<TrialOptionDto> Options<TEnum>(Func<TEnum, string> name, Func<TEnum, string> label)
        where TEnum : struct, Enum =>
        Enum.GetValues<TEnum>().Select(value => new TrialOptionDto(name(value), label(value))).ToList();

    private static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct =>
        Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : fallback;

    private static string? Clean(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static string Require(string value, string what) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"Podaj {what}.")
            : value.Trim();
}
