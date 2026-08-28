namespace LessonRunner.Application.Common;

/// <summary>
/// Adres, który trafi do atrybutu `href` w przeglądarce.
///
/// Powód wydzielenia: aplikacja renderuje linki wpisane przez personel — link do spotkania,
/// link do nagrania, link do projektu dziecka, materiał w bibliotece. Bez sprawdzenia schematu
/// da się tam wpisać `javascript:...`, a wtedy skrypt wykonuje się w sesji **osoby klikającej**.
/// W portalu rodzica oznacza to skrypt w sesji rodzica.
///
/// Reguła jest jedna dla całego systemu: wyłącznie `http` i `https`, adres bezwzględny.
/// Dotąd sprawdzały to trzy niezależne kopie tej samej logiki, a dwa miejsca nie sprawdzały
/// nic — dlatego jest to jedno miejsce, a nie czwarta kopia.
/// </summary>
public static class WebLink
{
    /// <summary>Czy adres jest bezwzględnym adresem http(s).</summary>
    public static bool IsHttpUrl(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    /// <summary>
    /// Przycina adres i sprawdza schemat. Pusty adres to `null` (brak linku jest dozwolony);
    /// adres w złym schemacie to <see cref="ArgumentException"/> z podanym komunikatem.
    /// </summary>
    public static string? Normalize(string? value, string error)
    {
        var trimmed = value?.Trim();

        if (string.IsNullOrEmpty(trimmed))
        {
            return null;
        }

        return IsHttpUrl(trimmed) ? trimmed : throw new ArgumentException(error);
    }
}
