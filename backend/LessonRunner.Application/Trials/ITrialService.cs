namespace LessonRunner.Application.Trials;

/// <summary>
/// Lekcje próbne 1:1 — droga od zgłoszenia do zapisanego uczestnika.
///
/// Metody z parametrem `instructorId` są wersją dla prowadzącego: widzi wyłącznie własne
/// lekcje i może zapisać wyłącznie diagnozę. Decyzja o przyjęciu dziecka należy do
/// administracji i ma osobną politykę autoryzacji.
/// </summary>
public interface ITrialService
{
    Task<TrialBoardDto> GetBoardAsync(CancellationToken cancellationToken);

    /// <summary>Lekcje próbne jednego instruktora — jego osobna lista, poza grafikiem grup.</summary>
    Task<TrialBoardDto> GetInstructorBoardAsync(Guid instructorId, CancellationToken cancellationToken);

    Task<TrialLessonDto?> GetAsync(Guid id, Guid? instructorId, CancellationToken cancellationToken);

    Task<TrialLessonDto> CreateAsync(CreateTrialDto dto, CancellationToken cancellationToken);
    Task<TrialLessonDto?> ScheduleAsync(Guid id, ScheduleTrialDto dto, CancellationToken cancellationToken);

    /// <summary>Zapis diagnozy. `instructorId` niepuste = wymagamy, żeby to była jego lekcja.</summary>
    Task<TrialLessonDto?> SaveDiagnosisAsync(Guid id, SaveTrialDiagnosisDto dto, Guid userId, Guid? instructorId, CancellationToken cancellationToken);

    Task<TrialEnrollmentResultDto?> EnrollAsync(Guid id, EnrollTrialDto dto, Guid? actingUserId, CancellationToken cancellationToken);
    Task<TrialLessonDto?> DeclineAsync(Guid id, DeclineTrialDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
