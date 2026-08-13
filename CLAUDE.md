# Zasady projektu Lesson Runner

## Struktura repozytorium

Katalog główny projektu: `D:\moje\kodziaki`.

- **Frontend**: `frontend\lesson-runner-web` — aplikacja React + TypeScript + Vite. Kod źródłowy w `src`, parser importu w `src/features/import/lessonMarkdown.ts`. Uruchamianie: `npm install` i `npm run dev` w tym katalogu.
- **Backend**: `backend` — rozwiązanie .NET (`LessonRunner.slnx`) w architekturze warstwowej: `LessonRunner.Api`, `LessonRunner.Application`, `LessonRunner.Domain`, `LessonRunner.Infrastructure`, `LessonRunner.Tests`.
- **Dokumentacja** (w katalogu głównym):
  - `system_zajec_online_programowanie_dla_dzieci.md` — docelowy zakres systemu i katalog sytuacji, które musi obsłużyć. **Źródło wymagań** — przy nowej funkcji sprawdź, czy jest tam już opisana.
  - `STATUS.md` — aktualny stan projektu, analiza luk wobec powyższego i plan prac. **To jest źródło prawdy o tym, co zrobione.**
  - `README.md` — uruchamianie, konfiguracja, role i uprawnienia, migracje.
  - `PLAN.md` — archiwum: historia budowy oraz wciąż obowiązujące sekcje "Architektura i wzorce" i "Format pliku importu".
  - `AUDYT_2026-07-27.md` — ostatni audyt kodu.
  - `.env.example` — wzorzec konfiguracji dla `docker compose` (plik `.env` jest ignorowany przez git).
- **Pliki pomocnicze**: `docs/` (przykładowe konspekty `*.md`), `tools/`, prototypy `Lesson Runner.dc.html` i `support.js`.

Uwaga: domyślny katalog roboczy to często `backend`. Frontendu szukaj zawsze w `frontend\lesson-runner-web` w katalogu głównym (poziom wyżej).

## Język i polskie znaki

- Wszystkie teksty po polsku piszemy w pełni z polskimi znakami diakrytycznymi: ą, ć, ę, ł, ń, ó, ś, ź, ż (oraz wielkie odpowiedniki).
- Dotyczy to w szczególności: etykiet i tekstów UI, placeholderów, komunikatów błędów i powiadomień, danych seed (lekcje, materiały, notatki), plików przykładowych (np. `docs/*.md`).
- Pozostają w ASCII (bez ogonków) wyłącznie elementy techniczne: identyfikatory kodu, wartości enumów (`error`, `hint`, `pace`, `intro`, ...), klucze obiektów, nazwy plików, tras i klas CSS, adresy URL.
- Parser importu (`features/import/lessonMarkdown.ts`) normalizuje polskie znaki przy dopasowaniu, więc treść konspektów piszemy poprawnie po polsku, a tagi typu `[błąd]`/`[podpowiedź]` działają tak samo jak ich formy ASCII.
- **Cudzysłowy polskie zawsze parą `„…”`.** Wpisanie otwierającego `„` i zamknięcie go zwykłym `"` psuje literał w C# (`"Status „w toku" ..."` kończy string w środku zdania i wywala build). W literałach kodu bezpieczniej użyć apostrofów `'…'` albo w ogóle zrezygnować z cudzysłowu.
- Zasad pilnują trzy testy: `polishDiacritics.test.ts` skanuje źródła frontendu (`ts`, `tsx`, `css`), `PolishQuotesTests` — pliki `.md` i komentarze w `.cs` w całym repozytorium, a `AsciiIdentifiersTests` — nazwy w pozycjach deklaracji w `.cs`, `.ts` i `.tsx`. W dokumentacji pomijane są bloki ``` i wstawki `…`, a przy cudzysłowach w kodzie wszystko poza komentarzem: w literale ten błąd wyłapuje już kompilator, a wartości w rodzaju `"Overdue"` mają zostać w ASCII.
- Ogonek w nazwie zmiennej (`const hasMateriałs`) to zawsze ślad po nadgorliwym find/replace, z tej samej podmiany co „postaći” i „zadańia”. Lista błędnych form w `polishDiacritics.test.ts` z definicji tego nie złapie — od klasy błędu jest `AsciiIdentifiersTests`.

## Architektura

Wzorce warstw (Clean Architecture w backendzie, MVVM-like we frontendzie): sekcja
"Architektura i wzorce" w `PLAN.md`. Aktualny zakres systemu i plan prac: `STATUS.md`.

## Role i uprawnienia

System ma trzy role: `Admin` (wszystko), `Instructor` (tylko własne grupy i terminy, w tym
zastępstwa) oraz `Parent` (wyłącznie dane powiązanych dzieci).

Przy dodawaniu nowego endpointu **zawsze** wskaż politykę autoryzacji jawnie:

- `RequireAuthorization("AdminOnly")` — administracja,
- `RequireAuthorization("StaffOnly")` — konspekty, grafik, kalendarz (Admin + Instructor),
- `RequireAuthorization("ParentOnly")` — portal rodzica.

Samo `RequireAuthorization()` oznacza „dowolny zalogowany”, czyli **także rodzic** — to był realny
wyciek treści konspektów. Nowe ścieżki uprawnień pokrywaj testem w `ApiAuthorizationTests`.

## Audyt

Grupy tras modyfikujące dane mają `.AddEndpointFilter<AuditEndpointFilter>()` — każde żądanie
POST/PUT/DELETE trafia do `AuditLogs` automatycznie. Dodając nową grupę tras, dopnij filtr.

Filtr **nie zapisuje treści żądania** (byłyby tam hasła). Endpoint, który loguje własny,
bogatszy wpis, oznacz `.SelfAudited()`, żeby nie dublować wpisów.

`EntityType` to **obszar w konwencji segmentu ścieżki**: `users`, `groups`, `billing`, `auth`.
Nie nazwa encji (`User`) — inaczej filtrowanie po obszarze gubi część dziennika.
`AuditService` wymusza małe litery.

## Baza danych

Migracje w repozytorium są wygenerowane dla SQLite. Nie zakładaj, że zadziałają na PostgreSQL —
szczegóły w `README.md`, sekcja „Baza danych”.

## Weryfikacja zmian

```bash
cd backend && dotnet build && dotnet test
cd frontend/lesson-runner-web && npm run build && npm test
```
