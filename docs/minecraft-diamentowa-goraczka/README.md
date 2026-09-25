# Diamentowa Gorączka – Minecraft Education (2 spotkania online)

Kurs na 2 spotkania po 95 minut (45 + 5 przerwy + 45), co tydzień, **online**: prowadzący na rozmowie wideo, każde dziecko w domu na własnym komputerze. Grupa 10–12 lat, początkujący, MakeCode w trybie bloczków.

## Zawartość

| Plik | Do czego |
|---|---|
| `spotkanie-1.md` | import: „Diamentowa Gorączka 1: Budujemy świat gry” (12 kroków) |
| `spotkanie-2.md` | import: „Diamentowa Gorączka 2: Zamieniamy świat w grę” (11 kroków) |
| `pdf/diamentowa-goraczka-spotkanie-N.pdf` | konspekty dla prowadzącego (z infografikami) |
| `diamentowa-goraczka-START.txt` | kod **projektu startowego** (sam silnik gry), z niego robisz `diamentowa-goraczka-start.mkcd` |
| `diamentowa-goraczka-PUNKT-KONTROLNY.txt` | kod stanu po spotkaniu 1, z niego robisz `diamentowa-goraczka-punkt-kontrolny.mkcd` |
| `diamentowa-goraczka-PELNA-GRA.txt` | kod **pełnej gry**, z niego robisz `diamentowa-goraczka-pelna-gra.mkcd` |
| `infografiki/*.png` | 17 plansz do pokazywania na ekranie dla uczniów (wgrywane automatycznie przy imporcie) |
| `_kod/` | źródła: silnik (`silnik.js`, `silnik.py`), kod uczniów (`po-spotkaniu-*.js`, `kod-ucznia.py`), szablony plików importu |

Generowanie po zmianach:

- `python tools/buduj_konspekt_minecraft.py` – pliki `spotkanie-N.md` i trzy pliki z kodem,
- `python tools/infografiki_minecraft.py` – infografiki (wymaga Chrome lub Edge),
- `python tools/pdf_konspekt_minecraft.py` – PDF-y konspektów.

Pliki z kodem są **tylko dla nauczyciela**. Każdy ma dwie sekcje: JavaScript i Python (wystarczy jedna). Start i pełną grę w obu językach sprawdzono w edytorze minecraft.makecode.com: kompilują się bez błędów i w całości zamieniają na bloczki. Punkt kontrolny składa się z tych samych, sprawdzonych fragmentów.

## Jak to działa na zajęciach

1. Ty wklejasz kod w MakeCode, przełączasz na Bloki i zapisujesz projekt jako `.mkcd`. Dzieci nigdy nie widzą kodu tekstowego.
2. Dziecko dostaje **projekt startowy** (`.mkcd`) i otwiera go na pierwszym spotkaniu przez **MakeCode → Importuj → Importuj plik**. Ma wtedy silnik, czyli supermoce w kategorii Funkcje, ale w grze jeszcze nic się nie dzieje.
3. Na obu spotkaniach dzieci dokładają resztę bloczków: komendy, kopalnię, skarby, punkty, czas, rekord.
4. Na końcu ich projekt działa tak jak **pełna gra**. Pełną grę masz do pokazu na starcie kursu, jako ratunek dla dziecka, które się zgubiło, i do wysłania rodzicom po kursie.
5. Dziecko, które na drugim spotkaniu nie ma swojego projektu, dostaje **punkt kontrolny** (`.mkcd`).

## Przed pierwszymi zajęciami (online)

1. **Świat-szablon.** Utwórz płaski świat z kodami i wpisz w nim komendy z kroku 1 spotkania 1 (materiał „Przygotowanie świata”). Sprawdź, czy na trawie Y pod stopami wynosi −60. Wyeksportuj świat do `.mcworld`.
2. **Projekty `.mkcd`.** Z każdego z trzech plików `.txt` zrób projekt według instrukcji na górze pliku: wklej jedną sekcję, przełącz na Bloki, zapisz jako `.mkcd`.
3. **Pliki w aplikacji.** W obu lekcjach w edytorze wgraj:
   - **Lekcja startowa** → `diamentowa-goraczka-start.mkcd`,
   - **Lekcja końcowa** → `diamentowa-goraczka-pelna-gra.mkcd`.
   Prezenter pokaże przy nich przycisk „Kopiuj link”. Link pobiera plik bez logowania, więc możesz go wysłać dzieciom i rodzicom. Punkt kontrolny trzymaj u siebie i wysyłaj na czacie temu, kto go potrzebuje.
4. **Świat `.mcworld`** wyślij rodzicom jako załącznik (wiadomość albo pliki w Teams), bo sloty lekcji zajmują projekty `.mkcd`.
5. **Wiadomość do rodziców** 2–3 dni wcześniej (gotowy tekst w materiałach kroku 1): instalacja i logowanie w Minecraft Education, świat, link do projektu startowego, test dźwięku, link do spotkania.
6. **Próba generalna.** Otwórz u siebie pełną grę i zagraj. Kod nie był uruchamiany w samym Minecraft Education. Jeśli któryś efekt daje błąd w czacie gry (najbardziej prawdopodobne: `camerashake`, nazwy cząsteczek, `fireworks_rocket`), usuń tę linię z `_kod/silnik.js` i `_kod/silnik.py` i przebuduj pliki.

## Import do aplikacji

1. Admin → **Import konspektu** → „Wczytaj plik .md, obrazy i PDF” → wybierz `spotkanie-1.md`. Kliknij przycisk jeszcze raz, wejdź do `infografiki/` i zaznacz wszystko (Ctrl+A).
2. W podglądzie: 12 kroków, 0 ostrzeżeń, wszystkie obrazy „gotowy”. **Utwórz konspekt**, potem w edytorze **Publikuj**.
3. To samo dla `spotkanie-2.md` (11 kroków).
4. Dodaj obie lekcje do grupy w tej kolejności (1 lekcja = 1 termin tygodniowy).

## Prowadzenie online

- W kokpicie kliknij **Ekran dla uczniów** i udostępnij to okno w rozmowie (Teams, Zoom, Meet). Dzieci widzą tylko tytuł kroku, infografiki i odliczanie przerwy, bez Twoich notatek. **Zaciemnij** (klawisz B) wyłącza obraz, gdy chcesz, żeby patrzyły na Ciebie.
- Do pokazów w grze (pierwsza runda, pomoc) przełącz udostępnianie na okno Minecrafta.
- Gdy dziecko utknie, poproś, żeby udostępniło ekran. Odpowiedzi, wyniki i nazwy studiów zbieraj na czacie.
- Dzieci z jednym ekranem przełączają się między rozmową a grą klawiszami Alt+Tab. Pokaż to na początku pierwszych zajęć.
