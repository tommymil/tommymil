using LessonRunner.Domain.Lessons;

namespace LessonRunner.Application.Lessons;

public interface ILessonRepository
{
    Task<IReadOnlyList<Lesson>> ListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Same tytuły konspektów, po identyfikatorze.
    ///
    /// Treść konspektu leży w kolumnie JSON, więc `ListAsync` deserializuje pełny scenariusz
    /// każdej lekcji — kroki, notatki, materiały, zrzuty ekranu. Wywołujący, którzy potrzebują
    /// wyłącznie podpisu w kalendarzu albo w mailu, płacili za to bez powodu. Tytuł jest
    /// osobną kolumną, więc tutaj JSON-a w ogóle nie ruszamy.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, string>> ListTitlesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Plik projektu o podanym kluczu pobierania albo `null`.
    ///
    /// Trasa `/download/lesson-files/{token}` jest anonimowa i dotąd wczytywała **wszystkie**
    /// konspekty, żeby znaleźć w nich jeden plik — czyli dawała każdemu w internecie sposób
    /// na wymuszenie pełnego odczytu i deserializacji biblioteki jednym żądaniem.
    /// </summary>
    Task<LessonProjectFile?> FindProjectFileByDownloadTokenAsync(string token, CancellationToken cancellationToken);

    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Lesson lesson, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Lesson lesson, CancellationToken cancellationToken);
}
