using System.Globalization;
using System.Text;

namespace LessonRunner.Application.Scheduling;

/// <summary>Pojedyncze zajęcia w formie nadającej się do kalendarza.</summary>
public sealed record CalendarEventDto(
    Guid SessionId,
    string Title,
    DateTimeOffset ScheduledAt,
    int DurationMinutes,
    string? Description,
    string? Location,
    string? MeetingUrl,
    bool Cancelled);

/// <summary>
/// Generator pliku iCalendar (.ics).
///
/// Świadomie **plik do pobrania, a nie adres subskrypcji**: subskrypcja wymaga URL-a, który
/// aplikacja kalendarza otwiera bez nagłówka `Authorization`, czyli osobnego, długożyjącego
/// tokenu w adresie. To osobna decyzja bezpieczeństwa i osobna migracja — na razie rodzic
/// i instruktor pobierają plik i importują go u siebie.
/// </summary>
public static class CalendarExport
{
    /// <summary>
    /// Domyślny czas trwania zajęć: 45 minut + 5 minut przerwy + 45 minut.
    ///
    /// Wpis w kalendarzu obejmuje przerwę, bo rodzic i instruktor blokują sobie czas od
    /// wejścia do wyjścia — 90 minut kończyło każde zajęcia pięć minut za wcześnie.
    /// </summary>
    public const int DefaultDurationMinutes = 95;

    public static byte[] ToIcs(IReadOnlyList<CalendarEventDto> events, string calendarName)
    {
        var builder = new StringBuilder();

        AppendLine(builder, "BEGIN:VCALENDAR");
        AppendLine(builder, "VERSION:2.0");
        AppendLine(builder, "PRODID:-//Szkola Programowania//Harmonogram//PL");
        AppendLine(builder, "CALSCALE:GREGORIAN");
        AppendLine(builder, "METHOD:PUBLISH");
        AppendLine(builder, $"X-WR-CALNAME:{Escape(calendarName)}");

        var stamp = Utc(DateTimeOffset.UtcNow);

        foreach (var item in events)
        {
            AppendLine(builder, "BEGIN:VEVENT");
            AppendLine(builder, $"UID:{item.SessionId}@szkola-programowania");
            AppendLine(builder, $"DTSTAMP:{stamp}");
            AppendLine(builder, $"DTSTART:{Utc(item.ScheduledAt)}");
            AppendLine(builder, $"DTEND:{Utc(item.ScheduledAt.AddMinutes(item.DurationMinutes))}");
            AppendLine(builder, $"SUMMARY:{Escape(item.Title)}");

            if (!string.IsNullOrWhiteSpace(item.Description))
            {
                AppendLine(builder, $"DESCRIPTION:{Escape(item.Description)}");
            }

            if (!string.IsNullOrWhiteSpace(item.Location))
            {
                AppendLine(builder, $"LOCATION:{Escape(item.Location)}");
            }

            if (!string.IsNullOrWhiteSpace(item.MeetingUrl))
            {
                AppendLine(builder, $"URL:{Escape(item.MeetingUrl)}");
            }

            // Odwołane terminy zostają w pliku ze statusem CANCELLED - dzięki temu przy ponownym
            // imporcie znikają z kalendarza zamiast tam zostać jako duchy.
            AppendLine(builder, item.Cancelled ? "STATUS:CANCELLED" : "STATUS:CONFIRMED");
            AppendLine(builder, "END:VEVENT");
        }

        AppendLine(builder, "END:VCALENDAR");

        return Encoding.UTF8.GetBytes(builder.ToString());
    }

    private static string Utc(DateTimeOffset value) =>
        value.UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);

    /// <summary>Escapowanie wg RFC 5545: odwrotny ukośnik, przecinek, średnik i złamania linii.</summary>
    private static string Escape(string value) => value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace(";", "\\;", StringComparison.Ordinal)
        .Replace(",", "\\,", StringComparison.Ordinal)
        .Replace("\r\n", "\\n", StringComparison.Ordinal)
        .Replace("\n", "\\n", StringComparison.Ordinal);

    /// <summary>
    /// Zapis linii ze zwijaniem po 75 oktetach (RFC 5545). Bez tego długie nazwy grup
    /// albo linki potrafią wywrócić import w części kalendarzy.
    /// </summary>
    private static void AppendLine(StringBuilder builder, string line)
    {
        const int maxOctets = 75;
        var bytes = Encoding.UTF8.GetBytes(line);

        if (bytes.Length <= maxOctets)
        {
            builder.Append(line).Append("\r\n");
            return;
        }

        var offset = 0;
        var first = true;

        while (offset < bytes.Length)
        {
            // Pierwsza linia ma 75 oktetów, kolejne 74 - jeden znak zajmuje wiodąca spacja.
            var budget = first ? maxOctets : maxOctets - 1;
            var take = Math.Min(budget, bytes.Length - offset);

            // Nie tniemy w środku znaku wielobajtowego.
            while (take > 0 && offset + take < bytes.Length && (bytes[offset + take] & 0xC0) == 0x80)
            {
                take--;
            }

            if (!first)
            {
                builder.Append(' ');
            }

            builder.Append(Encoding.UTF8.GetString(bytes, offset, take)).Append("\r\n");
            offset += take;
            first = false;
        }
    }
}
