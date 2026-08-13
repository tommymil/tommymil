using System.Text.RegularExpressions;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Strażnik zasady z CLAUDE.md: elementy techniczne zostają w ASCII. Nazwa zmiennej
/// z ogonkiem to zawsze ślad po nadgorliwym find/replace — tak powstał
/// `const hasMateriałs` w kokpicie prowadzenia, z tej samej podmiany co „postaći”
/// i „zadańia” w tekstach.
///
/// `polishDiacritics.test.ts` opiera się na zamkniętej liście błędnych form wyrazowych,
/// więc nie miał szans tego złapać — żadnej z tych nazw nie było i nie mogło być na liście.
/// Ten test pilnuje **klasy** błędu zamiast wyliczanki.
///
/// Patrzymy wyłącznie na pozycje deklaracji. Polski w komentarzach, literałach i treści
/// JSX jest poprawny i pożądany, a nazwę widać dokładnie tam, gdzie powstaje.
/// </summary>
public sealed class AsciiIdentifiersTests
{
    private const string PolskieZnaki = "ąćęłńóśźżĄĆĘŁŃÓŚŹŻ";

    private static readonly Regex Deklaracja = new(
        $@"\b(const|let|var|function|class|interface|type|enum|namespace|record|struct|public|private|protected|internal|static|readonly|async)\s+([A-Za-z_][A-Za-z0-9_]*[{PolskieZnaki}][A-Za-z0-9_{PolskieZnaki}]*)",
        RegexOptions.Compiled);

    [Fact]
    public void Kod_NieMaIdentyfikatorowZPolskimiZnakami()
    {
        var znalezione = new List<string>();

        foreach (var (sciezka, tresc) in RepositoryFiles.Find("*.cs", "*.ts", "*.tsx"))
        {
            if (sciezka.EndsWith("AsciiIdentifiersTests.cs", StringComparison.Ordinal))
            {
                // Ten plik z definicji zawiera wzorzec z polskimi znakami.
                continue;
            }

            var kod = SourceText.CodeOnly(tresc);

            foreach (Match trafienie in Deklaracja.Matches(kod))
            {
                var linia = kod.Take(trafienie.Index).Count(c => c == '\n') + 1;
                znalezione.Add($"{sciezka}:{linia} → {trafienie.Value.Trim()}");
            }
        }

        Assert.Empty(znalezione);
    }

    [Fact]
    public void Skaner_WygaszaKomentarzeILiteraly()
    {
        // Bez tego strażnik zapalałby się na polskiej prozie: „// type zgłoszenia” wygląda
        // dokładnie jak deklaracja typu o nazwie z ogonkiem.
        const string zrodlo = """
            // type zgłoszenia opisuje rodzaj sprawy
            const opis = "const wartość z literału";
            const etykieta = 'let pozycję';
            const szablon = `var wartość`;
            /* class Zgłoszenie w komentarzu blokowym */
            const poprawny = 1;
            """;

        var kod = SourceText.CodeOnly(zrodlo);

        Assert.Empty(Deklaracja.Matches(kod));
        Assert.Contains("const poprawny", kod);
    }

    [Fact]
    public void Skaner_WidziIdentyfikatorPozaLiteralem()
    {
        // Kontrola odwrotna: gdyby maskowanie wygaszało za dużo, test wyżej przechodziłby
        // zawsze i nie pilnowałby niczego. To jest dokładnie przypadek `hasMateriałs`.
        const string zrodlo = "const hasMateriałs = textInserts.length > 0;";

        var trafienia = Deklaracja.Matches(SourceText.CodeOnly(zrodlo));

        Assert.Single(trafienia);
        Assert.Contains("hasMateriałs", trafienia[0].Value);
    }

    [Fact]
    public void Pliki_SaWidoczneDlaTestu()
    {
        // Strażnik, który nie znajduje plików, przechodzi zawsze.
        var pliki = RepositoryFiles.Find("*.cs", "*.ts", "*.tsx");

        Assert.Contains(pliki, p => p.Sciezka.EndsWith(".tsx", StringComparison.Ordinal));
        Assert.Contains(pliki, p => p.Sciezka.EndsWith(".cs", StringComparison.Ordinal));
        Assert.True(pliki.Count >= 100, $"Tylko {pliki.Count} plików źródłowych — podejrzanie mało.");
    }
}
