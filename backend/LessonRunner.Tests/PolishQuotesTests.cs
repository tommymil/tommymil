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
    [Fact]
    public void Dokumentacja_ZamykaCudzyslowyWlasciwymZnakiem()
    {
        var znalezione = Wykroczenia(RepositoryFiles.Find("*.md"), UsunKodZMarkdown);

        Assert.Empty(znalezione);
    }

    [Fact]
    public void Dokumentacja_MaSparowaneCudzyslowy()
    {
        var znalezione = Niesparowane(RepositoryFiles.Find("*.md"), UsunKodZMarkdown);

        Assert.Empty(znalezione);
    }

    [Fact]
    public void KomentarzeWKodzie_ZamykajaCudzyslowyWlasciwymZnakiem()
    {
        var znalezione = Wykroczenia(RepositoryFiles.Find("*.cs"), SourceText.CommentsOnly);

        Assert.Empty(znalezione);
    }

    [Fact]
    public void KomentarzeWKodzie_MajaSparowaneCudzyslowy()
    {
        var znalezione = Niesparowane(RepositoryFiles.Find("*.cs"), SourceText.CommentsOnly);

        Assert.Empty(znalezione);
    }

    [Fact]
    public void Pliki_SaWidoczneDlaTestu()
    {
        // Strażnik, który nie znajduje plików, przechodzi zawsze i nie pilnuje niczego.
        // Ten test pilnuje samego strażnika - np. gdy zmieni się układ katalogów.
        var dokumentacja = RepositoryFiles.Find("*.md");
        var kod = RepositoryFiles.Find("*.cs");

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

    private static int NumerLinii(string tresc, int pozycja) =>
        tresc.Take(pozycja).Count(c => c == '\n') + 1;

    private static string Fragment(string tresc, int pozycja)
    {
        var poczatek = Math.Max(0, pozycja - 30);
        var dlugosc = Math.Min(80, tresc.Length - poczatek);

        return tresc.Substring(poczatek, dlugosc).Replace('\n', ' ').Trim();
    }
}
