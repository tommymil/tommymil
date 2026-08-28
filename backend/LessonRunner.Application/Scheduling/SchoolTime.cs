using System.Globalization;

namespace LessonRunner.Application.Scheduling;

/// <summary>
/// Czas szkoły: jedna strefa dla całego systemu i jeden sposób zapisania terminu słowami.
///
/// Powód wydzielenia jest konkretny. Przeglądarka wysyła termin przez `toISOString()`, czyli
/// z offsetem `Z`, a `DateTimeOffset.ToString("HH:mm")` drukuje godzinę **w offsecie samej
/// wartości**. Termin zapisany jako `16:00+00:00` trafiał więc do e-maila jako „16:00”,
/// podczas gdy w panelu i w kalendarzu rodzica widniało „18:00”. Co gorsza niespójnie:
/// terminy generowane cotygodniowo dostawały offset warszawski i drukowały się poprawnie,
/// więc pierwsze zajęcia z serii miały w mailu inną godzinę niż wszystkie następne.
///
/// Wniosek: instant trzymamy jak dotąd (offset w bazie jest bez znaczenia), ale **każdy
/// tekst dla człowieka** przechodzi przez ten typ. Wyjątkiem jest eksport ICS, który
/// z definicji podaje UTC i sam zostaje przeliczony przez aplikację kalendarza.
/// </summary>
public static class SchoolTime
{
    /// <summary>
    /// Strefa zajęć. Cotygodniowe terminy trzymają stałą godzinę ścienną (np. wtorki 18:00),
    /// a nie stały moment UTC — dzięki temu zmiana czasu nie przesuwa godziny zajęć.
    /// </summary>
    public static TimeZoneInfo Zone { get; } = ResolveZone();

    /// <summary>Ten sam moment widziany z zegara szkoły.</summary>
    public static DateTimeOffset ToSchoolTime(DateTimeOffset value) => TimeZoneInfo.ConvertTime(value, Zone);

    /// <summary>Data kalendarzowa według zegara szkoły — do porównań z dniami wolnymi.</summary>
    public static DateOnly LocalDate(DateTimeOffset value) => DateOnly.FromDateTime(ToSchoolTime(value).DateTime);

    /// <summary>
    /// Godzina ścienna szkoły opatrzona właściwym dla niej offsetem. Offset bierzemy dla
    /// **docelowej** godziny, a nie wyjściowej, więc przeskok czasu letniego nie przesuwa zajęć.
    /// </summary>
    public static DateTimeOffset FromWallClock(DateTime wallClock) =>
        new(wallClock, Zone.GetUtcOffset(wallClock));

    /// <summary>Termin dla człowieka: data i godzina według zegara szkoły.</summary>
    public static string FormatDateTime(DateTimeOffset value) =>
        ToSchoolTime(value).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

    /// <summary>
    /// Jak wyżej, ale ze wskazaniem strefy. Do dokumentów, które ktoś może czytać poza
    /// Polską albo dołączyć do reklamacji — tam sama godzina bywa za mało.
    /// </summary>
    public static string FormatDateTimeWithZone(DateTimeOffset value) =>
        ToSchoolTime(value).ToString("yyyy-MM-dd HH:mm zzz", CultureInfo.InvariantCulture);

    private static TimeZoneInfo ResolveZone()
    {
        foreach (var id in new[] { "Europe/Warsaw", "Central European Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.Local;
    }
}
