using System.Text;

namespace LessonRunner.Tests;

/// <summary>Pliki repozytorium widziane z testów — wspólne dla strażników tekstu.</summary>
internal static class RepositoryFiles
{
    private static readonly string[] PomijaneKatalogi =
        ["node_modules", "bin", "obj", "dist", ".git", ".vs"];

    public static List<(string Sciezka, string Tresc)> Find(params string[] wzorce)
    {
        var katalogGlowny = Root();

        return wzorce
            .SelectMany(wzorzec => Directory.EnumerateFiles(katalogGlowny, wzorzec, SearchOption.AllDirectories))
            .Where(sciezka => !JestPomijana(katalogGlowny, sciezka))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(sciezka => sciezka, StringComparer.Ordinal)
            .Select(sciezka => (
                Sciezka: Path.GetRelativePath(katalogGlowny, sciezka).Replace('\\', '/'),
                Tresc: File.ReadAllText(sciezka)))
            .ToList();
    }

    /// <summary>
    /// Katalog główny poznajemy po CLAUDE.md — testy uruchamiają się z `bin/Debug/net10.0`,
    /// więc ścieżka względna do repozytorium zależy od konfiguracji builda.
    /// </summary>
    public static string Root()
    {
        var katalog = new DirectoryInfo(AppContext.BaseDirectory);

        while (katalog is not null)
        {
            if (File.Exists(Path.Combine(katalog.FullName, "CLAUDE.md")))
            {
                return katalog.FullName;
            }

            katalog = katalog.Parent;
        }

        throw new InvalidOperationException(
            $"Nie znaleziono katalogu głównego repozytorium (CLAUDE.md) idąc w górę od {AppContext.BaseDirectory}.");
    }

    private static bool JestPomijana(string katalogGlowny, string sciezka) =>
        Path.GetRelativePath(katalogGlowny, sciezka)
            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Any(segment => PomijaneKatalogi.Contains(segment));
}

/// <summary>
/// Dzieli kod na komentarze, literały i resztę, po czym wygasza to, czego dany strażnik
/// nie ma oglądać. Bez tego cudzysłów wewnątrz literału wygląda jak domknięcie cytatu
/// z sąsiedniego komentarza, a polskie słowo w komentarzu — jak nazwa zmiennej.
///
/// Jeden skaner obsługuje C# i TypeScript: składnia literałów i komentarzy pokrywa się
/// na tyle, że różnicą są tylko odwrotne apostrofy TypeScriptu oraz `@""` i `"""` C#.
/// </summary>
internal static class SourceText
{
    private enum Kind { Code, Comment, Literal }

    /// <summary>Zostawia treść komentarzy, resztę zamienia na spacje.</summary>
    public static string CommentsOnly(string source) => Mask(source, Kind.Comment);

    /// <summary>Zostawia właściwy kod, wygasza komentarze i literały.</summary>
    public static string CodeOnly(string source) => Mask(source, Kind.Code);

    private static string Mask(string source, Kind keep)
    {
        var kinds = Classify(source);
        var wynik = new StringBuilder(source.Length);

        for (var i = 0; i < source.Length; i++)
        {
            // Znaki nowej linii zostają zawsze, żeby numeracja linii się zgadzała.
            wynik.Append(source[i] == '\n' || kinds[i] == keep ? source[i] : ' ');
        }

        return wynik.ToString();
    }

    private static Kind[] Classify(string source)
    {
        var kinds = new Kind[source.Length];
        var i = 0;

        while (i < source.Length)
        {
            if (source[i] == '/' && i + 1 < source.Length && source[i + 1] == '/')
            {
                var koniec = source.IndexOf('\n', i);
                koniec = koniec < 0 ? source.Length : koniec;
                Oznacz(kinds, i, koniec, Kind.Comment);
                i = koniec;
                continue;
            }

            if (source[i] == '/' && i + 1 < source.Length && source[i + 1] == '*')
            {
                var koniec = source.IndexOf("*/", i + 2, StringComparison.Ordinal);
                koniec = koniec < 0 ? source.Length : koniec + 2;
                Oznacz(kinds, i, koniec, Kind.Comment);
                i = koniec;
                continue;
            }

            var poLiterale = SkipLiteral(source, i);

            if (poLiterale > i)
            {
                Oznacz(kinds, i, poLiterale, Kind.Literal);
                i = poLiterale;
                continue;
            }

            i++;
        }

        return kinds;
    }

    private static void Oznacz(Kind[] kinds, int od, int doWylacznie, Kind kind)
    {
        for (var i = od; i < Math.Min(doWylacznie, kinds.Length); i++)
        {
            kinds[i] = kind;
        }
    }

    /// <summary>Zwraca pozycję tuż za literałem albo <paramref name="start"/>, gdy go tu nie ma.</summary>
    private static int SkipLiteral(string source, int start)
    {
        // Surowy literał C#: potrójny (lub dłuższy) cudzysłów, zamykany ciągiem tej samej długości.
        if (start + 2 < source.Length && source[start] == '"' && source[start + 1] == '"' && source[start + 2] == '"')
        {
            var dlugosc = 0;

            while (start + dlugosc < source.Length && source[start + dlugosc] == '"')
            {
                dlugosc++;
            }

            var ogranicznik = new string('"', dlugosc);
            var koniec = source.IndexOf(ogranicznik, start + dlugosc, StringComparison.Ordinal);

            return koniec < 0 ? source.Length : koniec + dlugosc;
        }

        // Verbatim C#: @"..." albo $@"..." / @$"..." — w środku "" oznacza jeden cudzysłów.
        var przedrostek = 0;

        while (start + przedrostek < source.Length && source[start + przedrostek] is '@' or '$')
        {
            przedrostek++;
        }

        var otwiera = start + przedrostek < source.Length && source[start + przedrostek] == '"';

        if (przedrostek > 0 && otwiera && source.AsSpan(start, przedrostek).Contains('@'))
        {
            var i = start + przedrostek + 1;

            while (i < source.Length)
            {
                if (source[i] == '"')
                {
                    if (i + 1 < source.Length && source[i + 1] == '"')
                    {
                        i += 2;
                        continue;
                    }

                    return i + 1;
                }

                i++;
            }

            return source.Length;
        }

        var znak = start + przedrostek < source.Length ? source[start + przedrostek] : '\0';

        // Zwykły, interpolowany, apostrofowy oraz szablonowy (TypeScript) literał.
        if (znak is '"' or '\'' or '`')
        {
            var i = start + przedrostek + 1;
            // Szablon TypeScriptu może przechodzić przez wiele linii, pozostałe nie.
            var wieloliniowy = znak == '`';

            while (i < source.Length)
            {
                if (source[i] == '\\')
                {
                    i += 2;
                    continue;
                }

                if (source[i] == znak)
                {
                    return i + 1;
                }

                if (source[i] == '\n' && !wieloliniowy)
                {
                    return i;
                }

                i++;
            }

            return source.Length;
        }

        return start;
    }
}
