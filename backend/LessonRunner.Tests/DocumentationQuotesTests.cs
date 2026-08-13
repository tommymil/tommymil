using System.Text.RegularExpressions;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Strażnik zasady z CLAUDE.md: polskie cudzysłowy zawsze parą „…”.
///
/// Odpowiednik `polishDiacritics.test.ts` dla dokumentacji. Nie dało się go dopisać po stronie
/// frontendu - Vite blokuje odczyt plików spoza katalogu projektu („Denied ID"), a poluzowanie
/// `server.fs.allow` otworzyłoby serwerowi deweloperskiemu całe repozytorium tylko po to,
/// żeby test miał co czytać.
///
/// Kontrola jest na poziomie dokumentu, a nie linii: w prozie cytat legalnie przechodzi przez
/// łamanie wiersza, więc bilans per linia dawałby same fałszywe trafienia. Sprawdzamy to,
/// co jest realnym błędem - otwierający „ zamknięty prostym ".
/// </summary>
public sealed class DocumentationQuotesTests
{
    private static readonly string[] PomijaneKatalogi =
        ["node_modules", "bin", "obj", "dist", ".git", ".vs"];

    [Fact]
    public void Dokumentacja_ZamykaCudzyslowyWlasciwymZnakiem()
    {
        var znalezione = new List<string>();

        foreach (var (sciezka, tresc) in WczytajDokumentacje())
        {
            var bezKodu = UsunKod(tresc);

            for (var i = 0; i < bezKodu.Length; i++)
            {
                if (bezKodu[i] != '„')
                {
                    continue;
                }

                var nastepny = bezKodu.IndexOfAny(['„', '”', '"'], i + 1);

                if (nastepny >= 0 && bezKodu[nastepny] == '"')
                {
                    znalezione.Add($"{sciezka}:{NumerLinii(bezKodu, i)} → {Fragment(bezKodu, i)}");
                }
            }
        }

        Assert.Empty(znalezione);
    }

    [Fact]
    public void Dokumentacja_MaSparowaneCudzyslowy()
    {
        var znalezione = new List<string>();

        foreach (var (sciezka, tresc) in WczytajDokumentacje())
        {
            var bezKodu = UsunKod(tresc);
            var otwierajace = bezKodu.Count(c => c == '„');
            var zamykajace = bezKodu.Count(c => c == '”');

            if (otwierajace != zamykajace)
            {
                znalezione.Add($"{sciezka}: otwierających {otwierajace}, zamykających {zamykajace}");
            }
        }

        Assert.Empty(znalezione);
    }

    [Fact]
    public void Dokumentacja_JestWidocznaDlaTestu()
    {
        // Strażnik, który nie znajduje plików, przechodzi zawsze i nie pilnuje niczego.
        // Ten test pilnuje samego strażnika - np. gdy zmieni się układ katalogów.
        var pliki = WczytajDokumentacje();

        Assert.Contains(pliki, p => p.Sciezka == "CLAUDE.md");
        Assert.Contains(pliki, p => p.Sciezka == "STATUS.md");
        Assert.True(pliki.Count >= 5, $"Znaleziono tylko {pliki.Count} plików .md - podejrzanie mało.");
    }

    /// <summary>
    /// Bloki ``` i wstawki `…` wypadają: zwykły cudzysłów jest tam poprawny (JSON, C#, ścieżki),
    /// a CLAUDE.md trzyma w takiej wstawce wręcz przykład **złego** zapisu. Bloki zamieniamy
    /// na same znaki nowej linii, żeby numeracja linii w komunikacie została prawdziwa.
    /// </summary>
    private static string UsunKod(string tresc)
    {
        var bezBlokow = Regex.Replace(
            tresc,
            "```.*?```",
            m => new string('\n', m.Value.Count(c => c == '\n')),
            RegexOptions.Singleline);

        return Regex.Replace(bezBlokow, "`[^`\n]*`", string.Empty);
    }

    private static List<(string Sciezka, string Tresc)> WczytajDokumentacje()
    {
        var katalogGlowny = ZnajdzKatalogGlowny();

        return Directory
            .EnumerateFiles(katalogGlowny, "*.md", SearchOption.AllDirectories)
            .Where(sciezka => !SciezkaJestPomijana(katalogGlowny, sciezka))
            .OrderBy(sciezka => sciezka, StringComparer.Ordinal)
            .Select(sciezka => (
                Sciezka: Path.GetRelativePath(katalogGlowny, sciezka).Replace('\\', '/'),
                Tresc: File.ReadAllText(sciezka)))
            .ToList();
    }

    private static bool SciezkaJestPomijana(string katalogGlowny, string sciezka)
    {
        var wzgledna = Path.GetRelativePath(katalogGlowny, sciezka);

        return wzgledna
            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Any(segment => PomijaneKatalogi.Contains(segment));
    }

    /// <summary>
    /// Katalog główny poznajemy po CLAUDE.md - testy uruchamiają się z `bin/Debug/net10.0`,
    /// więc ścieżka względna do repozytorium zależy od konfiguracji builda.
    /// </summary>
    private static string ZnajdzKatalogGlowny()
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

    private static int NumerLinii(string tresc, int pozycja) =>
        tresc.Take(pozycja).Count(c => c == '\n') + 1;

    private static string Fragment(string tresc, int pozycja)
    {
        var poczatek = Math.Max(0, pozycja - 30);
        var dlugosc = Math.Min(80, tresc.Length - poczatek);

        return tresc.Substring(poczatek, dlugosc).Replace('\n', ' ').Trim();
    }
}
