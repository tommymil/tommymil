using LessonRunner.Application.Groups;
using LessonRunner.Application.Trials;
using LessonRunner.Domain.Lessons;

namespace LessonRunner.Application.Lessons;

public sealed class LessonCommands(
    ILessonRepository lessonRepository,
    ILessonQueries lessonQueries,
    IGroupRepository groupRepository,
    ITrialRepository trialRepository) : ILessonCommands
{
    public async Task<LessonDetailsDto> CreateAsync(CreateLessonDto dto, CancellationToken cancellationToken)
    {
        Validate(dto);

        var existingLessons = await lessonRepository.ListAsync(cancellationToken);
        var order = dto.Order ?? NextOrder(existingLessons, dto.Subject);
        var lesson = dto.ToLesson(order);
        await lessonRepository.AddAsync(lesson, cancellationToken);

        return await lessonQueries.GetDetailsAsync(lesson.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created lesson could not be loaded.");
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await groupRepository.AnyScheduledSessionForLessonAsync(id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Lekcja jest przypisana do terminu w grupie. Usuń lub przełóż te zajęcia przed skasowaniem lekcji.");
        }

        return await lessonRepository.DeleteAsync(id, cancellationToken);
    }

    public Task<LessonDetailsDto?> SendToReviewAsync(Guid id, CancellationToken cancellationToken)
    {
        return SetStatusAsync(id, LessonStatus.Review, cancellationToken);
    }

    public Task<LessonDetailsDto?> PublishAsync(Guid id, CancellationToken cancellationToken)
    {
        return SetStatusAsync(id, LessonStatus.Ready, cancellationToken);
    }

    public async Task<LessonDetailsDto?> UpdateAsync(Guid id, CreateLessonDto dto, CancellationToken cancellationToken)
    {
        Validate(dto);

        var existing = await lessonRepository.GetByIdAsync(id, cancellationToken);

        if (existing is null)
        {
            return null;
        }

        var order = dto.Order ?? existing.Order;
        var updated = dto.ToLesson(order, existing.Id, existing.Status);

        if (updated.Kind != existing.Kind)
        {
            await EnsureKindChangeAllowedAsync(id, cancellationToken);
        }

        // Reguła czasu obowiązywała dotąd wyłącznie przy publikacji, a zapis zachowuje status.
        // Opublikowany konspekt pokazowy dawało się więc skrócić do dwudziestu minut i zostawał
        // gotowy - a to właśnie z gotowych buduje się listę wyboru przy lekcji próbnej.
        if (existing.Status == LessonStatus.Ready)
        {
            EnsurePlanFitsKind(updated);
        }

        updated.UpdatedAt = DateTimeOffset.UtcNow;

        await lessonRepository.UpdateAsync(updated, cancellationToken);

        return await lessonQueries.GetDetailsAsync(id, cancellationToken);
    }

    private async Task<LessonDetailsDto?> SetStatusAsync(Guid id, LessonStatus status, CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(id, cancellationToken);

        if (lesson is null)
        {
            return null;
        }

        if (status == LessonStatus.Ready)
        {
            EnsurePlanFitsKind(lesson);
        }

        lesson.Status = status;
        lesson.UpdatedAt = DateTimeOffset.UtcNow;

        await lessonRepository.UpdateAsync(lesson, cancellationToken);

        return await lessonQueries.GetDetailsAsync(id, cancellationToken);
    }

    /// <summary>
    /// Plan konspektu musi wypełniać okno, które lekcja rezerwuje w kalendarzu — dokładnie,
    /// nie w przybliżeniu. Obowiązuje oba rodzaje: termin blokuje rodzicowi i instruktorowi
    /// konkretną liczbę minut niezależnie od tego, ile materiału ktoś zaplanował.
    /// </summary>
    private static void EnsurePlanFitsKind(Lesson lesson)
    {
        var totalMinutes = lesson.Steps.Sum(step => step.DurationMinutes);

        if (!lesson.Kind.HasValidPlanDuration(totalMinutes))
        {
            throw new InvalidOperationException($"{lesson.Kind.PlanDurationRule()} Ten ma {totalMinutes}.");
        }
    }

    /// <summary>
    /// Rodzaj lekcji wyznacza długość wpisu w kalendarzu. Przełączenie konspektu wpiętego
    /// w terminy przeliczyłoby po cichu wszystkie eksporty ICS - także te, które rodzice mają
    /// już zaimportowane u siebie.
    /// </summary>
    private async Task EnsureKindChangeAllowedAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await groupRepository.AnyScheduledSessionForLessonAsync(id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Nie można zmienić rodzaju lekcji przypisanej do terminu w grupie — zmieniłaby się długość "
                + "zajęć w kalendarzach rodziców. Odepnij lekcję od terminów albo utwórz nowy konspekt.");
        }

        if (await trialRepository.AnyForLessonAsync(id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Nie można zmienić rodzaju konspektu przypisanego do lekcji próbnej. Odepnij go od tych "
                + "lekcji albo utwórz nowy konspekt.");
        }
    }

    private static int NextOrder(IReadOnlyList<Lesson> lessons, string subject)
    {
        var sameSubject = lessons.Where(lesson => string.Equals(lesson.Subject, subject, StringComparison.OrdinalIgnoreCase)).ToList();
        return sameSubject.Count == 0 ? 1 : sameSubject.Max(lesson => lesson.Order) + 1;
    }

    private static void Validate(CreateLessonDto dto)
    {
        if (!Enum.TryParse<LessonKind>(dto.Kind, ignoreCase: true, out _))
        {
            throw new ArgumentException("Nieznany rodzaj lekcji. Dozwolone wartości: standard, showcase.", nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            throw new ArgumentException("Lesson title is required.", nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.Subject))
        {
            throw new ArgumentException("Lesson subject is required.", nameof(dto));
        }

        if (dto.Steps.Any(step => string.IsNullOrWhiteSpace(step.Title)))
        {
            throw new ArgumentException("Every lesson step must have a title.", nameof(dto));
        }
    }
}
