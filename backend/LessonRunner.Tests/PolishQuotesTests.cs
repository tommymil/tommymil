using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Strażnik zasady z CLAUDE.md: polskie cudzysłowy zawsze parą „…”.
///
/// Obejmuje dokumentację (`.md`) i komentarze w kodzie (`.cs`). Odpowiednik
/// `polishDiacritics.test.ts`, który pilnuje źródeł frontendu. Nie dało się dopisać tego po
/// stronie frontu - Vite blokuje odczyt plików spoza katalogu projektu („Denied ID”),
/// a poluzowanie `server.fs.allow` otworzyłoby serwerowi deweloperskiemu całe repozytorium
/// tylko po to, żeby test miał co czytać.
///
/// Kontrola jest na poziomie dokumentu, a nie linii: w prozie cytat legalnie przechodzi przez
/// łamanie wiersza, więc bilans per linia dawałby same fałszywe trafienia. Szukamy tego,
/// co jest realnym błędem - otwierającego cudzysłowu domkniętego prostym znakiem ASCII.
///
/// W `.cs` sprawdzamy **wyłącznie komentarze**. Wewnątrz literału tekstowego ten błąd wyłapuje
/// już kompilator (zwykły cudzysłów kończy string w środku zdania), a wartości w rodzaju
/// `"Overdue"` czy `"172.16.0.0/12"` mają być w ASCII i nie podlegają tej zasadzie.
/// </summary>
public sealed class PolishQuotesTests
{
    private static readonly string[] PomijaneKatalogi =
        ["node_modules", "bin", "obj", "dist", ".git", ".vs"];

    [Fact]
    public void Dokumentacja_ZamykaCudzyslowyWlasciwymZnakiem()
    {
        var znalezione = Wykroczenia(WczytajPliki("*.md"), UsunKodZMarkdown);

        Assert.Empty(znalezione);
    }

    [Fact]
    public void Dokumentacja_MaSparowaneCudzyslowy()
    {
        var znalezione = Niesparowane(WczytajPliki("*.md"), UsunKodZMarkdown);

        Assert.Empty(znalezione);
    }

    [Fact]
    public void KomentarzeWKodzie_ZamykajaCudzyslowyWlasciwymZnakiem()
    {
        var znalezione = Wykroczenia(WczytajPliki("*.cs"), ZostawSameKomentarze);

        Assert.Empty(znalezione);
    }

    [Fact]
    public void KomentarzeWKodzie_MajaSparowaneCudzyslowy()
    {
        var znalezione = Niesparowane(WczytajPliki("*.cs"), ZostawSameKomentarze);

        Assert.Empty(znalezione);
    }

    [Fact]
    public void Pliki_SaWidoczneDlaTestu()
    {
        // Strażnik, który nie znajduje plików, przechodzi zawsze i nie pilnuje niczego.
        // Ten test pilnuje samego strażnika - np. gdy zmieni się układ katalogów.
        var dokumentacja = WczytajPliki("*.md");
        var kod = WczytajPliki("*.cs");

        Assert.Contains(dokumentacja, p => p.Sciezka == "CLAUDE.md");
        Assert.Contains(dokumentacja, p => p.Sciezka == "STATUS.md");
        Assert.True(dokumentacja.Count >= 5, $"Tylko {dokumentacja.Count} plików .md - podejrzanie mało.");
        Assert.True(kod.Count >= 50, $"Tylko {kod.Count} plików .cs - podejrzanie mało.");
    }

    /// <summary>Otwierający cudzysłów domknięty prostym znakiem ASCII zamiast pary.</summary>
    private static List<string> Wykroczenia(
        List<(string Sciezka, string Tresc)> pliki,
        Func<string, string> przygotuj)
    {
        var znalezione = new List<string>();

        foreach (var (sciezka, tresc) in pliki)
        {
            var tekst = przygotuj(tresc);

            for (var i = 0; i < tekst.Length; i++)
            {
                if (tekst[i] != '„')
                {
                    continue;
                }

                var nastepny = tekst.IndexOfAny(['„', '”', '"'], i + 1);

                if (nastepny >= 0 && tekst[nastepny] == '"')
                {
                    znalezione.Add($"{sciezka}:{NumerLinii(tekst, i)} → {Fragment(tekst, i)}");
                }
            }
        }

        return znalezione;
    }

    private static List<string> Niesparowane(
        List<(string Sciezka, string Tresc)> pliki,
        Func<string, string> przygotuj)
    {
        var znalezione = new List<string>();

        foreach (var (sciezka, tresc) in pliki)
        {
            var tekst = przygotuj(tresc);
            var otwierajace = tekst.Count(c => c == '„');
            var zamykajace = tekst.Count(c => c == '”');

            if (otwierajace != zamykajace)
            {
                znalezione.Add($"{sciezka}: otwierających {otwierajace}, zamykających {zamykajace}");
            }
        }

        return znalezione;
    }

    /// <summary>
    /// Bloki ``` i wstawki `…` wypadają: zwykły cudzysłów jest tam poprawny (JSON, C#, ścieżki),
    /// a CLAUDE.md trzyma w takiej wstawce wręcz przykład błędnego zapisu. Bloki zamieniamy
    /// na same znaki nowej linii, żeby numeracja linii w komunikacie została prawdziwa.
    /// </summary>
    private static string UsunKodZMarkdown(string tresc)
    {
        var bezBlokow = Regex.Replace(
            tresc,
            "```.*?```",
            m => new string('\n', m.Value.Count(c => c == '\n')),
            RegexOptions.Singleline);

        return Regex.Replace(bezBlokow, "`[^`\n]*`", string.Empty);
    }

    /// <summary>
    /// Zostawia treść komentarzy, a wszystko inne zamienia na spacje (znaki nowej linii
    /// zachowuje, żeby numeracja linii się zgadzała). Obsługuje literały zwykłe, verbatim
    /// (@""), interpolowane ($"") i surowe (potrójny cudzysłów) - inaczej cudzysłów wewnątrz
    /// literału wyglądałby jak domknięcie cytatu z komentarza obok.
    /// </summary>
    private static string ZostawSameKomentarze(string kod)
    {
        var wynik = new StringBuilder(new string(' ', kod.Length));

        for (var i = 0; i < kod.Length; i++)
        {
            if (kod[i] == '\n')
            {
                wynik[i] = '\n';
            }
        }

        var pozycja = 0;

        while (pozycja < kod.Length)
        {
            var znak = kod[pozycja];

            if (znak == '/' && pozycja + 1 < kod.Length && kod[pozycja + 1] == '/')
            {
                var koniec = kod.IndexOf('\n', pozycja);
                koniec = koniec < 0 ? kod.Length : koniec;
                Przepisz(kod, wynik, pozycja, koniec);
                pozycja = koniec;
                continue;
            }

            if (znak == '/' && pozycja + 1 < kod.Length && kod[pozycja + 1] == '*')
            {
                var koniec = kod.IndexOf("*/", pozycja + 2, StringComparison.Ordinal);
                koniec = koniec < 0 ? kod.Length : koniec + 2;
                Przepisz(kod, wynik, pozycja, koniec);
                pozycja = koniec;
                continue;
            }

            pozycja = PomienLiteral(kod, pozycja);
        }

        return wynik.ToString();
    }

    private static void Przepisz(string kod, StringBuilder wynik, int od, int doWylacznie)
    {
        for (var i = od; i < doWylacznie; i++)
        {
            wynik[i] = kod[i];
        }
    }

    /// <summary>Zwraca pozycję tuż za literałem zaczynającym się w <paramref name="pozycja"/>.</summary>
    private static int PomienLiteral(string kod, int pozycja)
    {
        // Surowy literał: potrójny (lub dłuższy) cudzysłów, zamykany ciągiem tej samej długości.
        if (pozycja + 2 < kod.Length && kod[pozycja] == '"' && kod[pozycja + 1] == '"' && kod[pozycja + 2] == '"')
        {
            var dlugosc = 0;

            while (pozycja + dlugosc < kod.Length && kod[pozycja + dlugosc] == '"')
            {
                dlugosc++;
            }

            var ogranicznik = new string('"', dlugosc);
            var koniec = kod.IndexOf(ogranicznik, pozycja + dlugosc, StringComparison.Ordinal);

            return koniec < 0 ? kod.Length : koniec + dlugosc;
        }

        // Verbatim: @"..." albo $@"..." / @$"..." - w środku "" oznacza jeden cudzysłów.
        var przedrostek = 0;

        while (pozycja + przedrostek < kod.Length && (kod[pozycja + przedrostek] is '@' or '$'))
        {
            przedrostek++;
        }

        var maAt = kod.AsSpan(pozycja, przedrostek).Contains('@');
        var otwiera = pozycja + przedrostek < kod.Length && kod[pozycja + przedrostek] == '"';

        if (przedrostek > 0 && otwiera && maAt)
        {
            var i = pozycja + przedrostek + 1;

            while (i < kod.Length)
            {
                if (kod[i] == '"')
                {
                    if (i + 1 < kod.Length && kod[i + 1] == '"')
                    {
                        i += 2;
                        continue;
                    }

                    return i + 1;
                }

                i++;
            }

            return kod.Length;
        }

        if (otwiera || kod[pozycja] == '"' || kod[pozycja] == '\'')
        {
            var zamykajacy = otwiera ? '"' : kod[pozycja];
            var i = (otwiera ? pozycja + przedrostek : pozycja) + 1;

            while (i < kod.Length)
            {
                if (kod[i] == '\\')
                {
                    i += 2;
                    continue;
                }

                if (kod[i] == zamykajacy || kod[i] == '\n')
                {
                    return i + 1;
                }

                i++;
            }

            return kod.Length;
        }

        return pozycja + 1;
    }

    private static List<(string Sciezka, string Tresc)> WczytajPliki(string wzorzec)
    {
        var katalogGlowny = ZnajdzKatalogGlowny();

        return Directory
            .EnumerateFiles(katalogGlowny, wzorzec, SearchOption.AllDirectories)
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
