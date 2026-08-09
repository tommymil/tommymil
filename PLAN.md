# Lesson Runner - plan budowy aplikacji (ARCHIWUM)

> **Dokument archiwalny.** Opisuje przebieg budowy i decyzje architektoniczne z okresu
> czerwiec-lipiec 2026. Nie jest już aktualizowany i **nie opisuje aktualnego zakresu systemu** -
> powstałe później kursy, płatności, powiadomienia, kalendarz, audyt, portal rodzica i rola
> `Parent` nie są tu ujęte.
>
> Aktualny stan: [`STATUS.md`](STATUS.md). Uruchamianie: [`README.md`](README.md).
>
> Wartościowe i wciąż obowiązujące są sekcje "Architektura i wzorce" oraz
> "Format pliku importu (Markdown)" - do nich warto tu wracać.

## Cel

Zbudować kompletną aplikację Lesson Runner:

- backend w C# / ASP.NET Core Web API,
- frontend w React + Vite + TypeScript,
- trwały zapis lekcji, kroków, materiałów i notatek,
- tryb administratora do tworzenia konspektów,
- tryb instruktora do prowadzenia lekcji.

Obecne pliki `Lesson Runner.dc.html` i `support.js` traktujemy jako prototyp UI oraz specyfikacje zachowania, nie jako kod produkcyjny.

## Status prac

Zrealizowane:

- zapisano plan projektu w `PLAN.md`,
- utworzono backend jako solucję Clean Architecture,
- utworzono projekty `LessonRunner.Domain`, `LessonRunner.Application`, `LessonRunner.Infrastructure`, `LessonRunner.Api`,
- ustawiono zależności między projektami zgodnie z Clean Architecture,
- dodano pierwszy model domenowy lekcji, kroków, zasobów, notatek i materiałów ucznia,
- utworzono frontend React/Vite,
- przebudowano frontend pod strukturę MVVM-like: `app`, `pages`, `features`, `components/ui`, `components/layout`,
- dodano routing, layout, reusable `Button`, pierwsze strony i hooki view-model,
- zweryfikowano `dotnet build backend/LessonRunner.slnx`,
- zweryfikowano `npm run build` we frontendzie,
- uruchomiono dev server frontendu pod `http://127.0.0.1:5173`,
- dodano EF Core SQLite w `LessonRunner.Infrastructure`,
- dodano `AppDbContext` i repozytorium lekcji,
- dodano seed startowej lekcji Scratch,
- dodano DTO i query service dla listy lekcji w `LessonRunner.Application`,
- dodano endpoint `GET /api/lessons`,
- uruchomiono backend pod `http://127.0.0.1:5000`,
- zweryfikowano odpowiedź `GET http://127.0.0.1:5000/api/lessons`,
- podłączono frontendowy view-model biblioteki lekcji do `GET /api/lessons`,
- dodano stany loading/error/pusta lista w widokach biblioteki administratora i instruktora,
- zweryfikowano `npm run build` po podłączeniu frontendu do API,
- dodano endpoint `GET /api/lessons/{id}`,
- dodano DTO szczegółów lekcji, kroków, materiałów, zasobów i notatek,
- podłączono pierwszą wersję `PresenterPage` do szczegółów lekcji z API,
- dodano podstawową nawigację kroków w presenterze,
- zweryfikowano `GET http://127.0.0.1:5000/api/lessons/{id}`,
- zweryfikowano ponownie `dotnet build backend/LessonRunner.slnx` i `npm run build`,
- dodano backendowy przypadek użycia tworzenia lekcji,
- dodano endpoint `POST /api/lessons`,
- dodano endpoint `DELETE /api/lessons/{id}`,
- dodano frontendowy `useLessonEditorViewModel`,
- podłączono formularz edytora do API,
- edytor zapisuje metadane lekcji, tagi, kroki, skrypt, materiał ucznia i notatkę instruktora,
- zweryfikowano tworzenie lekcji przez `POST /api/lessons`,
- usunięto testowy rekord przez `DELETE /api/lessons/{id}`,
- zweryfikowano końcowo `dotnet build backend/LessonRunner.slnx` i `npm run build`,
- dodano aktualizowanie lekcji przez `PUT /api/lessons/{id}`,
- dodano zmianę statusu przez `POST /api/lessons/{id}/send-to-review`,
- dodano publikację przez `POST /api/lessons/{id}/publish`,
- dodano trasę frontendu `admin/lessons/:lessonId/edit`,
- edytor potrafi ładować istniejącą lekcję i zapisywać zmiany,
- edytor obsługuje zasoby kroku typu link i kod,
- karty lekcji prowadza do edycji konkretnego konspektu,
- zweryfikowano cykl API: create -> update -> send-to-review -> publish -> delete,
- zweryfikowano ponownie `dotnet build backend/LessonRunner.slnx` i `npm run build`,
- rozbudowano view-model prezentera o timer całej lekcji i timer kroku,
- dodano zapamiętywanie aktualnego kroku w `localStorage`,
- dodano skróty klawiaturowe: następny/poprzedni krok, start/pauza, notatki, materiały ucznia,
- dodano w presenterze widok listy kroków,
- dodano w presenterze materiały ucznia,
- dodano w presenterze zasoby link i kod z kopiowaniem,
- dodano w presenterze notatki instruktora,
- dodano przełączniki pokazywania materiałów i notatek,
- zweryfikowano `npm run build` po rozbudowie prezentera,
- dodano wyszukiwanie lekcji po tytułe, przedmiocie, opisię i poziomie,
- dodano filtrówanie biblioteki administratora po statusię,
- ograniczono widok instruktora do lekcji gotowych,
- dodano lepsze komunikaty dla pustej listy i braku wyników filtrowania,
- poprawiono karte lekcji tak, żeby nie renderowala linku prowadzenia dla niegotowych konspektów,
- dodano walidacje UX w edytorze dla zasobów link/kod oraz zmiany statusu bez kroków,
- poprawiono responsywność glownej nawigacji, biblioteki, edytora i prezentera,
- zweryfikowano `npm run build` po poprawkach ergonomii.

Etap stabilizacji technicznej:

- dodano pakiet `Microsoft.EntityFrameworkCore.Design` do projektu Api,
- wygenerowano migracje EF `InitialCreate`, zastąpiono `EnsureCreated` przez `Database.Migrate()`,
- migracje są stosowane automatycznie przy starcie aplikacji,
- ujednolicono port backendu na `http://localhost:5000` (spójnie z domyślnym klientem frontendu),
- ograniczono `UseHttpsRedirection` do środowiska nie-developerskiego (czysty profil http),
- dodano `.gitignore` (artefakty .NET, baza SQLite, node_modułes, dist),
- dodano `.env.development` i `.env.example` dla frontendu (`VITE_API_BASE_URL`),
- dodano `README.md` z instrukcją uruchamiania backendu, frontendu, migracji i testów,
- utworzono projekt testowy `LessonRunner.Tests` (xUnit) i dodano go do solucji,
- dodano `InternalsVisibleTo` z Infrastructure do testów,
- napisano testy warstwy Application (LessonCommands, LessonQueries) na fake repozytorium,
- napisano testy round-trip repozytorium EF na SQLite in-memory,
- backend: 33 testy przechodzą (`dotnet test`),
- skonfigurowano Vitest + Testing Library (jsdom) we frontendzie,
- dodano skrypty `test` / `test:watch` i setup testów,
- napisano testy `Button`, `LessonCard`, `useLessonsViewModel`, `usePresenterViewModel`,
- frontend: 18 testów przechodzi (`npm test`), `npm run build` nadal działa,
- dodano encje `User` i role `Admin`/`Instructor` w domenie,
- dodano warstwę Application auth: DTO, `IUserRepository`, `IPasswordHasher`, `IAuthService`, `AuthService`,
- dodano w Infrastructure: `Pbkdf2PasswordHasher`, `EfUserRepository`, mapowanie `Users` w `AppDbContext`, migracje `AddUsers`,
- dodano seed startowych kont (admin + instruktor) tylko dla developmentu,
- dodano endpointy `POST /api/auth/register`, `POST /api/auth/login` oraz szkielet `GET /api/auth/me` (501 do czasu tokenow),
- dodano testy `AuthService` i `Pbkdf2PasswordHasher`,
- zweryfikowano end-to-end: register 201, duplikat 409, login 200, błędne hasło 401, `me` 501.

Etap 6 - autoryzacja:

- dodano `JwtOptions` (Issuer/Audience/SigningKey/ExpiryMinutes) z `appsettings`,
- dodano `ITokenService` + `JwtTokenService` (HMAC-SHA256), token z claimami sub/email/role,
- rozszerzono kontrakt auth o `AuthResponseDto` (token + expiresAt + user),
- `AuthService.RegisterAsync`/`AuthenticateAsync` zwracaja token,
- skonfigurowano `AddAuthentication`/`AddJwtBearer` + `AddAuthorization` (polityka `AdminOnly`),
- dodano `UseAuthentication`/`UseAuthorization` i walidacje obecności klucza podpisu przy starcie,
- zabezpieczono `GET /api/lessons*` (zalogowani) oraz POST/PUT/DELETE/publish/send-to-review (tylko Admin),
- zaimplementowano `GET /api/auth/me` na podstawie claimów,
- dodano testy backendu: `JwtTokenService`, zaktualizowano `AuthService` (34 testy zielone),
- zweryfikowano end-to-end: bez tokenu 401, admin 200, instruktor POST 403, admin POST 201, `me` zwraca role, zły token 401,
- frontend: dodano typy auth, klienta z nagłówkiem Bearer i globalna obsługa 401 (`ApiError`),
- dodano `authApi` (login, me), `AuthProvider`/`useAuth` (sesja w `localStorage`, walidacja `/me` przy starcie),
- dodano guardy tras `RequireAuth` i `RequireAdmin`,
- przepisano `LoginPage` na realny formularz email/hasło z przekierowaniem wg roli,
- nawigacja (`AppShell`) zależna od roli + email użytkownika i wylogowanie,
- akcje administracyjne ukryte dla instruktora (i chronione guardami tras),
- dodano testy frontendu: klient API i `AuthContext` (29 testów zielonych), `npm run build` działa,
- zweryfikowano integracje: Vite serwuje, CORS preflight 204 z poprawnymi nagłówkami, login z originu frontendu 200.

Etap 7 - pliki (backend gotowy):

- dodano `IFileStorage` + `LocalFileStorage` (zapis na dysk, katalog `uploads`),
- dodano `FileStorageOptions` i `FileUploadRules` (limit rozmiaru, whitelist typow),
- dodano endpoint `POST /api/files` (walidacja pustego pliku, rozmiaru i typu),
- dodano serwowanie statyczne `/uploads` przez `PhysicalFileProvider`,
- dodano testy storage (backend: 37 testów zielonych),
- BRAKUJE: wpięcie uploadu w edytor frontendu (zasoby image/file, materiały ucznia z obrazem).

Edytor v2 - autoring konspektów (Slice 1: edycja + reorder):

- dodano edycję istniejącego kroku w miejscu (`editStep`/`commitStep`),
- edycja zmienia typ, tytuł, czas i skrypt; materiały ucznia, zasoby i notatki kroku
  pozostają nietknięte (brak utraty danych przy krokach z wieloma elementami),
- dodano przestawianie kroków w górę/dół (`moveStep`) z utrzymaniem wskaźnika edycji,
- `removeStep` koryguje/anuluje aktualną edycję,
- UI edytora: nagłówek trybu edycji, przyciski ↑/↓/Edytuj per krok, podświetlenie kroku w edycji, akcje Zapisz/Anuluj,
- dodano testy view-modelu edytora (frontend: 35 testów zielonych), `npm run build` działa.

Edytor v2 - autoring konspektów (Slice 2: pełna treść kroku):

- ujednolicono formularz kroku: trzyma tablice `studentItems`, `resources`, `notes`,
- edycja kroku ładuje pełną treść (znika logika "zachowaj nietknięte" ze Slice 1 - nic nie ginie, bo wszystko jest reprezentowane),
- dodawanie wielu materiałów ucznia (`addStudentItem`/`removeStudentItem`),
- dodawanie wielu notatek z rodzajem error/hint/pace (`addNote`/`removeNote`),
- dodawanie wielu zasobów link/kod z walidacją (`addResource`/`removeResource`),
- elementy nieedytowalne jeszcze w UI (obraz/plik z seedu) są pokazywane i zachowywane przy round-trip,
- UI: sekcje listowe materiałów/notatek/zasobów z usuwaniem pojedynczych pozycji, podsumowanie kroku z licznikami,
- dodano testy view-modelu (frontend: 38 testów zielonych), `npm run build` działa,
- zweryfikowano realny kontrakt API: login -> POST lekcji z 2 materiałami/2 zasobami/2 notatkami na krok -> GET zwraca komplet (rodzaje error/pace, link/code) -> DELETE 204,
- backend bez zmian: `CreateLessonDto.ToLesson` mapuje całe tablice, `ParseEnum` obsługuje rodzaje case-insensitive.

Edytor v2 - autoring konspektów (Slice 3: pliki i obrazy - Etap 7 domknięty):

- dodano w kliencie API `uploadFile` (multipart/form-data, nagłówek Bearer, obsługa 401, surfacowanie komunikatu błędu z backendu),
- dodano `resolveAssetUrl` (relatywne `/uploads/...` -> pełny adres backendu),
- edytor: upload obrazu/PDF jako zasób (`uploadResourceFile`, rodzaj image/file wg content-type) i obrazu jako materiał ucznia (`uploadStudentImage`),
- edytor: miniatury obrazów w listach, przyciski uploadu, stan "Przesyłanie...", blokada w trakcie,
- prezenter: render obrazów (materiał ucznia i zasób) oraz plików (PDF jako link do pobrania) przez `resolveAssetUrl`,
- dodano testy: klient (`uploadFile` FormData/błąd/`resolveAssetUrl`) i view-model (upload zasobu/obrazu, błąd) - frontend: 44 testy zielone, `npm run build` działa,
- zweryfikowano realny kontrakt: upload PNG 200 + `StoredFile`, plik serwowany pod `/uploads/...` 200 image/png, zły typ 400 z komunikatem, bez tokenu 401.

Edytor v2 - autoring konspektów (Slice 4: dopracowanie pozycji kroku):

- edycja pojedynczych pozycji w miejscu (inline) - nie trzeba już usuwać i wpisywać od nowa:
  `updateStudentItem`, `updateNoteItem` (rodzaj + treść), `updateResourceItem` (etykieta/URL/język/kod),
- przestawianie pozycji w obrębie kroku: `moveStudentItem`, `moveNoteItem`, `moveResourceItem` (wspólny helper `moveInArray`),
- UI: wiersze list są edytowalne (pola + ↑/↓/× przez wspólny `SubItemControls`), edycja podpisu obrazu, miniatury zachowane,
- usunięto martwe helpery widoku po przejściu na wiersze edytowalne,
- dodano testy view-modelu (frontend: 48 testów zielonych), `npm run build` działa,
- bez zmian w kontrakcie API (te same tablice przechodzą pełny zapis i odczyt jak w Slice 2/3).

Poprawki uruchomienia lokalnego (Visual Studio / VS Code):

- dodano testowy `Jwt:SigningKey` w bazowym `appsettings.json`, aby backend wstawał niezależnie od profilu/środowiska (klucz dev, do zmiany przed produkcją przez `Jwt__SigningKey`),
- poluzowano CORS: dozwolony dowolny port loopbacku (`localhost`/`127.0.0.1`), bo Vite bez `strictPort` potrafi wziąć 5174 gdy 5173 zajęty - to blokowało logowanie z frontendu.

Domknięcie pod pełne testowanie:

- dodano wspólny moduł etykiet `lessonLabels` (typ kroku, rodzaj notatki, rodzaj zasobu) z fallbackiem na surową wartość,
- prezenter pokazuje czytelny typ kroku i rodzaj notatki zamiast surowych "concept"/"hint",
- edytor: lista kroków używa etykiety typu, wiersz zasobu używa wspólnego modułu (usunięto lokalną mapę),
- dodano test `lessonLabels` (frontend: 50 testów zielonych), `npm run build` działa.

Bogaty seed do samodzielnego testowania:

- rozbudowano `LessonSeedData` z 1 do 4 lekcji w różnych statusach i przedmiotach:
  - "Pierwsza gra: ruch postaci" (Scratch, Ready, 7 kroków),
  - "Animacja: kotek mówi i zmienia kostiumy" (Scratch, Ready, 5 kroków),
  - "Python: zmienne i rozmowa z komputerem" (Python, Review, 4 kroki),
  - "Python: pętła for i lista zadań" (Python, Draft, 3 kroki),
- każda lekcja ma wiele materiałów, notatki różnych rodzajów (error/hint/pace) i zasoby link/kod,
- pozwala przetestować: wyszukiwanie, filtr statusu (admin), widok instruktora (tylko Ready), prezenter na wielu lekcjach,
- UWAGA: seed zasila tylko PUSTĄ bazę. Aby zobaczyć nowe lekcje, zatrzymaj backend i usuń
  `backend/LessonRunner.Api/lesson-runner.db` (oraz pliki `-wal`/`-shm`), potem uruchom ponownie.
- Infrastructure kompiluje się czysto; testy backendu używają własnego `TestData`, więc seed ich nie dotyczy.

Przeprojektowanie prezentera w kokpit nauczyciela:

- ZAŁOŻENIE: prowadzący udostępnia uczniom własny ekran ze Scratchem; ta aplikacja to jego
  prywatne zaplecze ("co mam teraz robić"), a nie pokaz dla uczniów.
- układ 50/50: lewy pasek kroków | środek "Co robić teraz" + wskazówki + materiały | prawy duży panel "Na ekranie / wrzutki",
- prawy panel zbiera WSZYSTKIE obrazy kroku (z materiałów i z zasobów) i pokazuje je dużo - na screeny bloczków,
- środek: skrypt jako lista ponumerowanych czynności, wskazówki (error/hint/pace), materiały pomocnicze (tekst, linki, kod z kopiowaniem),
- przeramówano nazewnictwo pod nauczyciela: skrypt -> "Co robić teraz", materiały ucznia -> "wrzutki/screeny na ekran",
  notatki -> "wskazówki", zasoby -> "materiały pomocnicze"; spójnie w edytorze i prezenterze (bez zmian w modelu danych),
- dodano test renderujacy `PresenterPage` (instrukcje jako lista + obraz w panelu ekranu z pełnym URL),
- frontend: 52 testy zielone, `npm run build` działa.

Ergonomia prowadzenia + tryb ciemny:

- "Co robić teraz" jako interaktywny wskaźnik bieżącej czynności: klik ustawia aktualny punkt,
  wcześniejsze są oznaćzone jako zrobione (przekreślone, przygaszone), kolejne czekają; reset przy zmianie kroku,
- tryb ciemny: tokeny CSS (`--bg`, `--surface`, `--border`, `--text*`) z jasnymi i ciemnymi wartośćiami,
  przełącznik w pasku (zawsze widoczny), wybór zapamiętany w `localStorage`, domyślnie wg systemu,
  motyw ustawiany przed renderem (bez migniecia), `color-scheme` dla natywnych kontrolek,
  ciemne warianty wskazówek (error/hint/pace) i pill typu kroku,
- dodano `features/theme` (`theme.ts` + `useTheme`), testy motywu i interaktywnych czynności,
- frontend: 56 testów zielonych, `npm run build` działa.

Import konspektów z Markdown (kreator):

- ZAŁOŻENIE: prowadzący pisze konspekt w prostym pliku .md wg szablonu, importer tworzy lekcje,
  a ręczna robota ogranicza się do dorobienia screenow że Scratcha w edytorze.
- parser `features/import/lessonMarkdown.ts` (czysta funkcja) -> `CreateLessonRequest` + ostrzeżenia,
- strona `admin/lessons/import` (guard admina, link "Import" w nawigacji): wklej/wczytaj .md, podgląd kroków i ostrzezen, "Utwórz konspekt",
- po imporcie otwiera się edytor lekcji (tam dodajesz obrazy),
- bez zmian w backendzie - używa istniejącego `POST /api/lessons`,
- dodano testy: parser (metadane, typy/czas kroków, polskie tagi notatek, blok kodu z wcięciami, tekstowe wrzutki, ostrzeżenia) i VM importu (parsowanie, walidacja, utworzenie+nawigacja, wczytanie pliku) - frontend: 66 testów zielonych, build działa,
- gotowy przykład: `docs/przyklad-animacja.md` (Twoja lekcja o animacji/sterowaniu w formacie importu).

Format pliku importu (Markdown):

```text
# Tytuł lekcji            -> tytuł (wymagany)
Subject: / Level: / Tags: / Opis:   -> metadane (przed pierwszym krokiem)

## [typ] Tytuł kroku (8 min)         -> krok; [typ] i (N min) opcjonalne
   typy: intro/review/concept/demo/guided/challenge/break/summary (lub PL: wprowadzenie, powtorka, ..., przerwa)

### Co robić teraz       -> punkty listy = czynności (skrypt)
- czynność 1

### Wskazówki            -> [błąd]/[podpowiedź]/[tempo] + treść (domyślnie podpowiedź)
- [błąd] częsty błąd

### Materiały            -> linki, kod i tekstowe wrzutki
- [link] Etykieta | https://...
- [kod] Język:
  ```
  ...wieloliniowy pseudo-kod (wcięcia zachowane)...
  ```
- Tekst bez tagu = tekstowa wrzutka na ekran
```

Grupy, grafik i obecność (system frekwencji):

- ZAŁOŻENIE: admin tworzy grupę, dobiera uporządkowane lekcje Ready, ustawia start i godzinę
  (terminy generują się co tydzień, 1 lekcja = 1 termin), dodaje uczestników i przypisuje
  instruktora; instruktor widzi swój grafik, wchodzi w konkretny termin, klika Start -> lista
  obecności -> Zapisz -> prowadzi lekcję (kokpit) -> Zakończ + notatka.
- Domena `LessonRunner.Domain/Groups`: `Group`, `GroupParticipant`, `ScheduledSession`,
  `AttendanceRecord` + enumy `GroupStatus`, `ScheduledSessionStatus` (Planned/InProgress/Completed/Cancelled).
  Model relacyjny (nie blob JSON) - potrzebne zapytania po instruktorze, datach, obecności.
- Persistence: `*Document` w `Infrastructure/Groups`, mapowanie w `AppDbContext` (FK + kaskady,
  indeksy `ScheduledSessions(GroupId,SequenceNumber)`/`ScheduledAt`, unikat `AttendanceRecords(SessionId,ParticipantId)`),
  migracja `AddGroupsAndAttendance`, demonstracyjny seed grupy (dev, gdy brak grup).
- Application `Groups`: `GroupService` (create + generowanie cotygodniowych terminów, walidacja
  instruktora i lekcji Ready, uczestnicy, odwołanie terminu, lista instruktorów),
  `SessionService` (grafik, start, get/save obecności, zakończenie - z kontrolą właściciela:
  obcy/nieistniejący termin -> null/404). `IUserRepository` rozszerzony o `GetByIdAsync`/`ListByRoleAsync`.
- API: admin (`AdminOnly`) `GET/POST/DELETE /api/groups`, `GET /api/groups/{id}`,
  `*/participants[/{pid}]`, `POST /api/groups/{id}/sessions/{sid}/cancel`, `GET /api/users/instructors`;
  instruktor (`RequireAuthorization`) `GET /api/schedule`, `GET /api/schedule/{id}`,
  `POST .../start`, `GET|PUT .../attendance`, `POST .../finish`.
- Frontend: typy `types/group.ts`, klient `api/groupsApi.ts`; admin (`AdminGroupsPage`,
  `GroupEditorPage` z podglądem terminów, `GroupDetailsPage`); instruktor (`InstructorSchedulePage`,
  `SessionCockpitPage`). Prezenter wydzielony do `features/presenter/PresenterCockpit` (reuse w
  prowadzeniu ad-hoc i z grafiku; lista obecności + zakończenie jako nakładki). Nawigacja: admin
  "Grupy", instruktor "Grafik".
- Decyzje: obecność startuje "wszyscy niezaznaczeni"; do grupy tylko lekcje Ready; jeden instruktor
  na grupę; uczestnicy per grupa (globalny roster - rozbudowa na później).
- Domknięcia: (1) **frekwencja** - `GET /api/groups/{id}/attendance` (obecności per uczestnik z
  zakończonych zajęć + %), tabela w szczegółach grupy; (2) **przekładanie terminu** -
  `POST .../sessions/{sid}/reschedule`, datetime-local w wierszu zaplanowanego terminu;
  (3) **stan prowadzenia per-termin** - `LessonRunSession` zakluczony też po `ScheduledSessionId`
  (Guid.Empty dla ad-hoc), migracja `ScheduleScopedRunSessions`, endpointy run-session z
  opcjonalnym `?scheduledSessionId`, kokpit z grafiku przekazuje id terminu (timery nie kolidują
  między grupami).
- Testy: backend 54 zielonych (generowanie terminów, walidacja, cykl start/obecność/zakończenie,
  kontrola właściciela, frekwencja, przekładanie, run-session per-termin, round-trip EF);
  frontend 77 zielonych (VM edytora grupy, kokpit sesji, grafik). `dotnet build`/`dotnet test` i
  `npm run build`/`npm test` przechodzą.

Zarządzanie kontami i komfort sesji:

- Konta (admin): encja `User.IsActive` (migracja `AddUserIsActive`, domyślnie aktywne),
  `UserAdminService` (lista/tworzenie/aktywacja-dezaktywacja), endpointy `GET/POST /api/users`,
  `POST /api/users/{id}/activate|deactivate` (AdminOnly), `GET /api/users/instructors` przeniesiony
  do grupy `/api/users` i filtruje aktywnych; nieaktywne konto nie zaloguje się (`AuthService`).
- Bezpieczeństwo: usunięto publiczny `POST /api/auth/register` (konta zakłada wyłącznie admin).
- Frontend: `pages/AdminUsersPage` + `useUsersViewModel` (lista, dodawanie, włącz/wyłącz),
  `api/usersApi`, nawigacja admina "Konta", trasa `admin/users`.
- Komfort: `Jwt:ExpiryMinutes` 120 -> 720 (token ważny ~12 h, nie wylogowuje w trakcie dnia).
- Edycja grupy po utworzeniu: `PUT /api/groups/{id}` (nazwa + instruktor, walidacja roli),
  `POST /api/groups/{id}/sessions` (dopisanie kolejnego terminu = lekcja Ready + data, następny
  `SequenceNumber`); w szczegółach grupy formularz edycji nagłówka i dodawania terminu.
- Używalność (po przeglądzie): (1) reset hasła przez admina - `POST /api/users/{id}/password`,
  inline "Ustaw hasło" w panelu Konta; (2) guardy integralności - nie da się usunąć lekcji
  przypisanej do terminu (409) ani uczestnika z zapisaną obecnością (409); klient API przekazuje
  teraz komunikat błędu z backendu; (3) uproszczona ścieżka instruktora - "Prowadzenie ad-hoc"
  widoczne tylko dla admina, instruktor ma w nawigacji "Grafik" i tam ląduje po zalogowaniu
  (obecność zawsze się liczy).
- Testy: backend 66 zielonych, frontend 80 zielonych; `dotnet build`/`dotnet test` i
  `npm run build`/`npm test` przechodzą.

## Aktualny stan (snapshot)

Pełny obieg gotowy do realnej pracy lokalnej:

- Role: admin (konspekty, grupy, konta) i instruktor (grafik, prowadzenie). Logowanie JWT,
  token ważny ~12 h, auto-wylogowanie przy 401.
- Konspekty: edytor, import z Markdown (typy kroków z `break`/przerwą), upload obrazów/PDF,
  publikacja Draft -> Review -> Ready, prezenter (kokpit nauczyciela, tryb ciemny, stan
  prowadzenia zapisywany per termin / ad-hoc).
- Grupy: tworzenie z cotygodniowym harmonogramem (1 lekcja = 1 termin), uczestnicy
  (imię/nazwisko/telefon/mail), przypisany instruktor; edycja grupy (nazwa/instruktor,
  dopisanie/odwołanie/przełożenie terminu); frekwencja (% per uczestnik).
- Grafik instruktora -> kokpit zajęć: Start -> lista obecności -> Zapisz -> prowadzenie
  -> Zakończ + notatka; obecność edytowalna ponownie; kontrola właściciela (instruktor widzi
  tylko swoje terminy).
- Konta (admin): lista, tworzenie, aktywacja/dezaktywacja, reset hasła. Publiczna rejestracja
  usunięta.
- Integralność: nie da się usunąć lekcji przypisanej do terminu ani uczestnika z obecnością (409).
- Persystencja: SQLite + EF, migracje stosowane przy starcie. OCZEKUJĄ (zastosują się po restarcie
  backendu): `AddGroupsAndAttendance`, `ScheduleScopedRunSessions`, `AddUserIsActive`.
- Jakość: backend 66 testów, frontend 80 testów - zielone; oba buildy przechodzą.

Backlog (świadomie odłożone, wg priorytetu z przeglądu używalności):

1. Frekwencja per uczestnik - które konkretnie terminy ktoś opuścił (teraz tylko zbiorczy %).
2. Eksport / kopia zapasowa danych (baza + `uploads`) - np. CSV grupy/frekwencji lub backup pliku.
3. Jedno-komendowe uruchamianie (teraz backend i frontend startują ręcznie w dwóch terminalach).
4. Drobne: wstawianie/zmiana kolejności terminów w środku, edycja notatki po zakończeniu,
   sprzątanie osieroconego stanu prowadzenia przy usuwaniu grupy, twardsze testy API (HTTP).
5. Przed ewentualną produkcją: realny `Jwt:SigningKey`, zmiana/wyłączenie kont i haseł seedowych.

## Etap 1: Struktura repozytorium

Docelowa struktura:

```text
kodziaki/
├─ backend/
│  ├─ LessonRunner.Api/
│  ├─ LessonRunner.Application/
│  ├─ LessonRunner.Domain/
│  └─ LessonRunner.Infrastructure/
├─ frontend/
│  └─ lesson-runner-web/
├─ Lesson Runner.dc.html
├─ support.js
└─ PLAN.md
```

## Architektura i wzorce

Projekt budujemy od początku jako aplikację utrzymywalną, a nie szybki prototyp.

### Backend: Clean Architecture

Backend dzielimy na cztery projekty:

```text
LessonRunner.Domain
- encje domenowe,
- enumy,
- podstawowe reguły domenowe,
- brak zależności od EF, HTTP, bazy danych i frameworkow webowych.

LessonRunner.Application
- przypadki użycia,
- DTO,
- interfejsy repozytoriow i serwisow,
- walidacja komend i zapytań,
- logika aplikacyjna niezależna od ASP.NET.

LessonRunner.Infrastructure
- Entity Framework Core,
- AppDbContext,
- implementacje repozytoriow,
- seed danych,
- integracje z plikami, mailem i storage.

LessonRunner.Api
- endpointy albo kontrolery,
- konfiguracja Dependency Injection,
- CORS,
- Swagger,
- autoryzacja,
- mapowanie request/response.
```

Kierunek zależności:

```text
Api -> Application -> Domain
Infrastructure -> Application -> Domain
Api -> Infrastructure tylko przez Dependency Injection
```

Nie umieszczamy logiki biznesowej w kontrolerach. Kontrolery mają przyjąć request, wywołać przypadek użycia i zwrócić response.

### Frontend: MVVM-like + reusable views

React nie będzie klasycznym MVVM z osobnymi klasami ViewModel, ale zastosujemy podobny podział odpowiedzialności:

```text
View
- komponenty prezentacyjne,
- nie znaja API,
- dostaja dane i callbacki przez props.

ViewModel
- custom hooki, np. useLessonEditorViewModel i usePresenterViewModel,
- trzymają stan ekranu,
- lacza dane z API z akcjami UI,
- obsługują timery, localStorage i skróty klawiaturowe.

Model/API
- typy domenowe,
- klient API,
- funkcje pobierania i zapisu danych.
```

Zasady frontendu:

1. Widoki nie wykonują bezpośrednio `fetch`.
2. Komponenty UI są wielokrotnego użycia i bez wiedzy o domenie.
3. Komponenty domenowe mogą znać typy lekcji, kroków i zasobów.
4. Hooki view-model obsługują stan ekranów i akcje.
5. Typy API są jawne, bez `any`.
6. UI przenosimy z prototypu etapami, rozbijajac go na komponenty.
7. Wszystkie teksty po polsku piszemy w pełni z polskimi znakami (ą, ć, ę, ł, ń, ó, ś, ź, ż):
   etykiety UI, placeholdery, komunikaty, dane seed, pliki przykladowe. ASCII zostaje tylko dla
   identyfikatorów, wartości enumów, kluczy, nazw plików/tras/klas CSS i URL. Szczegóły w `CLAUDE.md`.

Reusable views i komponenty od początku:

```text
Button
IconButton
Badge
StatusPill
PageHeader
AppShell
LessonCard
StepTypeBadge
ResourceBlock
NoteBlock
StepList
TimerDisplay
```

## Etap 2: Backend C#

### Fundament

1. Utworzyć projekt `ASP.NET Core Web API`.
2. Dodać Entity Framework Core.
3. Na start uzyc SQLite jako lokalnej bazy danych.
4. Dodać `AppDbContext`.
5. Dodać CORS dla frontendu.
6. Uruchomić Swagger/OpenAPI.

### Model domeny

Encje bazowe:

```text
User
- Id
- Email
- PasswordHash
- Role: Admin / Instructor

Lesson
- Id
- Title
- Subject
- Level
- Description
- Status: Draft / Review / Ready
- CreatedAt
- UpdatedAt

LessonStep
- Id
- LessonId
- Order
- Type: Intro / Review / Concept / Demo / Guided / Challenge / Summary
- Title
- DurationMinutes

LessonStepScript
- Id
- StepId
- Text
- Order

StudentItem
- Id
- StepId
- Kind: Text / Image
- Text
- Caption
- Url

Resource
- Id
- StepId
- Kind: Code / Link / Image / File
- Label
- Url
- Code
- Language

Note
- Id
- StepId
- Kind: Error / Hint / Pace
- Text
```

Później:

```text
LessonRunSession
- Id
- LessonId
- UserId
- StepIndex
- ElapsedTotalSeconds
- ElapsedStepSeconds
- Running
- StartedAt
- UpdatedAt
```

### API MVP

```http
GET    /api/lessons
GET    /api/lessons/{id}
POST   /api/lessons
PUT    /api/lessons/{id}
DELETE /api/lessons/{id}

POST   /api/lessons/{id}/publish
POST   /api/lessons/{id}/run-session
PUT    /api/lessons/{id}/run-session
POST   /api/lessons/{id}/send-to-review
```

Autoryzacja może zostać dodana po pierwszym działającym MVP:

```http
POST   /api/auth/login
POST   /api/auth/register
GET    /api/auth/me
```

### Kolejnosc prac backendowych

1. Stworzyc solucję `LessonRunner`.
2. Stworzyc projekty `Domain`, `Application`, `Infrastructure`, `Api`.
3. Ustawić zależności między projektami zgodnie z Clean Architecture.
4. Zdefiniowac encje i enumy w `Domain`.
5. Zdefiniowac DTO i interfejsy przypadkow użycia w `Application`.
6. Dodać EF Core, SQLite i `AppDbContext` w `Infrastructure`.
7. Dodać seed danych z lekcją Scratch z prototypu.
8. Dodać endpointy albo kontrolery w `Api`.
9. Uruchomić Swagger i sprawdzić endpointy.
10. Dodać walidację.
11. Dodać migracje EF.

## Etap 3: Frontend React/Vite

### Fundament

1. Utworzyć aplikację React + Vite + TypeScript.
2. Dodać routing.
3. Dodać klienta API.
4. Dodać typy domenowe zgodne z backendem.
5. Przeniesc UI z prototypu do komponentow React.

### Proponowana struktura

```text
frontend/lesson-runner-web/src/
├─ api/
│  ├─ client.ts
│  └─ lessonsApi.ts
├─ app/
│  ├─ App.tsx
│  └─ router.tsx
├─ components/
│  ├─ layout/
│  └─ ui/
├─ features/
│  ├─ editor/
│  ├─ lessons/
│  └─ presenter/
├─ pages/
│  ├─ LoginPage.tsx
│  ├─ AdminLibraryPage.tsx
│  ├─ LessonEditorPage.tsx
│  ├─ InstructorLessonsPage.tsx
│  └─ PresenterPage.tsx
├─ types/
│  └─ lesson.ts
└─ main.tsx
```

### Kolejnosc prac frontendowych

1. Stworzyc projekt Vite.
2. Dodać routing.
3. Dodać typy `Lesson`, `LessonStep`, `Resource`, `Note`.
4. Dodać klienta API.
5. Zbudować ekran wyboru roli.
6. Zbudować biblioteke lekcji.
7. Zbudować ekran instruktora.
8. Zbudować tryb prezentera.
9. Przeniesc timery, skróty klawiaturowe i preferencje `localStorage`.
10. Zbudować edytor lekcji.
11. Podłączyć zapis i edycję do backendu.

## Etap 4: Edytor lekcji

Edytor powinien obsługiwać:

1. edycję metadanych lekcji,
2. dodawanie kroków,
3. usuwanie kroków,
4. zmianę kolejności kroków,
5. edycję skryptu prowadzącego,
6. dodawanie materiałów ucznia,
7. dodawanie zasobów: kod, link, obraz, plik,
8. dodawanie notatek,
9. zapis szkicu,
10. zmianę statusu: szkic -> do sprawdzenia -> gotowa.

## Etap 5: Tryb prezentera

Presenter działa glownie po stronie frontendu:

1. pobiera gotową lekcję z API,
2. przechowuje aktualny krok lokalnie,
3. zapisuje ostatnią pozycję w `localStorage`,
4. ma timer kroku i całej lekcji,
5. obsługuje skróty:
   - `ArrowRight` / spacja: następny krok,
   - `ArrowLeft`: poprzedni krok,
   - `P`: start/pauza,
   - `N`: notatki,
   - `Escape`: wyjście,
6. pozwala zmienić układ widoku i rozmiar tekstu.

## Etap 6: Autoryzacja

Po MVP:

1. dodać logowanie,
2. dodać role:
   - Admin: tworzy i edytuje lekcje,
   - Instructor: prowadzi gotowe lekcje,
3. zabezpieczyc endpointy backendu,
4. ukryc akcje frontendowe zaleznie od roli.

## Etap 7: Pliki i materiały

Po podstawowym edytorze:

1. upload obrazów,
2. upload PDF,
3. przechowywanie plików lokalnie albo w chmurze,
4. powiązanie zasobów z krokami lekcji.

## Etap 8: Testy i jakość

Backend:

```text
- testy serwisow,
- testy API,
- walidacja DTO,
- migracje EF.
```

Frontend:

```text
- test renderowania glownych stron,
- test kontrolek prezentera,
- test edytora kroków.
```

## Najblizsza kolejnosc prac

1. [x] Utworzyć solucję backendową Clean Architecture.
2. [x] Utworzyć `frontend/lesson-runner-web` z podziałem MVVM-like.
3. [x] Backend: modele + SQLite + seed lekcji Scratch.
4. [x] Frontend: pobranie lekcji z API i pokazanie biblioteki.
5. [x] Presenter: pierwszą działająca wersja prowadzenia lekcji.
6. [x] Edytor: tworzenie i zapis lekcji.
7. [x] Stabilizacja: migracje EF, konfiguracja uruchamiania, testy backendu i frontendu.
8. [x] Fundament autoryzacji: encja User, hashowanie haseł, rejestracja/logowanie (bez JWT).
9. [x] Autoryzacja: tokeny JWT, middleware, `GET /api/auth/me`, zabezpieczenie endpointów rolami.
10. [x] Frontend: ekran logowania i ukrywanie akcji zaleznie od roli.
11. [x] Pliki i materiały (Etap 7): backend (`POST /api/files`, `LocalFileStorage`, `/uploads`) + wpięcie w edytor i prezenter (upload obrazów/PDF, render obrazów, zweryfikowane round-tripem).
12. [x] Edytor v2 Slice 1: edycja kroku w miejscu + przestawianie kroków (bez utraty pod-elementów).
13. [x] Edytor v2 Slice 2: wiele zasobów/notatek/materiałów na krok + rodzaje notatek error/hint/pace (zweryfikowane round-tripem API).
14. [x] Edytor v2 Slice 3: upload obrazów/PDF wpiety w edytor i prezenter (zweryfikowane round-tripem API).
15. [x] Edytor v2 Slice 4: edycja pozycji kroku w miejscu + reorder materiałów/notatek/zasobów.
16. [x] Etykiety enumów (typ kroku/notatki/zasobu) spójnie w prezenterze i edytorze.
17. [x] Bogaty seed: 4 lekcje w różnych statusach i przedmiotach (wymaga resetu bazy, by się pojawił).
18. [x] Prezenter przeprojektowany w kokpit nauczyciela: 50/50 instrukcje | duży panel na screeny/wrzutki, nazewnictwo pod prowadzącego.
19. [x] Ergonomia: interaktywny wskaźnik bieżącej czynności ("Co robić teraz").
20. [x] Tryb ciemny: tokeny CSS, przełącznik w pasku, zapamiętywanie wyboru, domyślnie wg systemu.
21. [x] Import konspektów z Markdown: parser + strona importu z podglądem, przykład `docs/przyklad-animacja.md`.
22. [x] Biblioteka: wybór technologii Scratch / Minecraft Education przed wyszukiwaniem i statusem.
23. [x] Trwałe sesje prowadzenia (`LessonRunSession`): tabela EF, endpointy `POST/PUT /api/lessons/{id}/run-session`, zapisywanie kroku/timerów/stanu prezentera.
24. [x] Grupy, grafik i obecność: grupy + uczestnicy + cotygodniowe terminy (admin), grafik instruktora i kokpit sesji (start -> lista obecności -> prowadzenie -> zakończenie + notatka), reuse `PresenterCockpit`.

## Przewodnik: lokalne wprowadzenie konspektu (smoke test)

Cel: od zera uruchomić aplikację lokalnie, wprowadzić prawdziwy konspekt (z obrazem/PDF),
opublikowac go i poprowadzic w trybie prezentera. Wszystko lokalnie, na realnym API.

### 0. Wymagania

- .NET SDK 10.0+, Node.js 20+ (patrz `README.md`).
- Dwa terminale: jeden na backend, jeden na frontend.

### 1. Start backendu (terminal 1)

```bash
cd backend
dotnet run --project LessonRunner.Api
```

- API: `http://localhost:5000`. Profil `http` z `launchSettings.json` ustawia środowisko
  `Development`, dzieki czemu wczytuje się `Jwt:SigningKey` i konta startowe (seed).
- Baza SQLite i startowa lekcja Scratch tworzą się automatycznie przy pierwszym starcie.
- Uwaga: nie uruchamiaj z `--no-launch-profile` bez ustawienia `ASPNETCORE_ENVIRONMENT=Development`
  oraz `Jwt:SigningKey` - bez klucza aplikacja celowo nie wystartuje.

### 2. Start frontendu (terminal 2)

```bash
cd frontend/lesson-runner-web
npm install   # tylko za pierwszym razem
npm run dev
```

- Aplikacja: `http://localhost:5173`. API bierze z `VITE_API_BASE_URL` (domyślnie `http://localhost:5000`).

### 3. Logowanie jako administrator

- Wejdź na `http://localhost:5173`, zaloguj się kontem admina:
  `admin@lessonrunner.local` / `admin12345`.
- Po zalogowaniu masz dostęp do biblioteki administratora i akcji tworzenia/edycji konspektów.

### 4. Utwórzenie konspektu

1. Otwórz tworzenie nowego konspektu (z biblioteki administratora).
2. Wypełnij metadane: tytuł i opis są wymagane; przedmiot, poziom i tagi - opcjonalnie
   (tagi po przecinku, np. `Pętle, Sterowanie`).
3. Dla każdego kroku w panelu "Nowy krok":
   - ustaw typ, tytuł, czas (min) i skrypt prowadzącego (każde zdanie/akapit w nowej linii),
   - "Materiały ucznia": wpisz tekst i "Dodaj materiał" albo "Dodaj obraz" (PNG/JPG/GIF/WEBP),
   - "Notatki instruktora": wybierz rodzaj (Podpowiedź/Częsty błąd/Tempo), wpisz treść, "Dodaj notatkę",
   - "Zasoby": link albo kod ("Dodaj zasób") lub "Dodaj obraz lub PDF" (upload),
   - możesz dodać wiele materiałów, notatek i zasobów do jednego kroku,
   - "Dodaj krok" dopisuje krok do listy na dółe.
4. Na liscie kroków: strzałki przestawiaja kolejnosc, "Edytuj" ładuje krok z powrotem do
   formularza (z pełna treścia - nic nie ginie), "Usuń" kasuje krok.
5. Kliknij "Zapisz" - konspekt zapisuje się przez API (`POST`/`PUT /api/lessons`).

### 5. Zmiana statusu i publikacja

- "Do sprawdzenia" -> status `Review`; "Publikuj" -> status `Ready`.
- Tylko konspekty `Ready` są widoczne i możliwe do prowadzenia w trybie instruktora.

### 6. Prowadzenie w trybie prezentera

- Zaloguj się instruktorem (`instructor@lessonrunner.local` / `teacher12345`) albo zostań adminem.
- Otwórz gotowy konspekt w prezenterze. Sprawdź:
  - nawigację kroków (strzałki / spacja), timer kroku i całej lekcji, start/pauza,
  - materiały ucznia (w tym obrazy), zasoby (link, kod z kopiowaniem, obraz, PDF jako link),
  - notatki instruktora, przełączniki widoczności materiałów i notatek.
- Stan prowadzenia zapisuje się w backendzie jako sesja per użytkownik i lekcja: aktualny krok, timer kroku, timer całej lekcji i start/pauza.

### 7. Szybka kontrola end-to-end (opcjonalnie, bez UI)

```bash
# token admina
curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@lessonrunner.local","password":"admin12345"}'
# lista lekcji (z nagłówkiem Authorization: Bearer <token>)
curl -s http://localhost:5000/api/lessons -H "Authorization: Bearer <token>"
# upload pliku
curl -s -X POST http://localhost:5000/api/files -H "Authorization: Bearer <token>" \
  -F "file=@sciezka/do/obraz.png;type=image/png"
```

### 8. Reset i rozwiazywanie problemów

- Reset danych: zatrzymaj backend i usuń plik bazy `backend/LessonRunner.Api/lesson-runner.db`
  (oraz pliki `-wal`/`-shm`, jeśli są). Przy następnym starcie seed odtworzy konta i lekcje Scratch.
  Wgrane pliki: katalog `backend/LessonRunner.Api/wwwroot/uploads` (czyść ręcznie, jeśli chcesz).
- "Aplikacja nie startuje - brak klucza Jwt:SigningKey": uruchamiasz bez profilu Development;
  użyj `dotnet run --project LessonRunner.Api` albo ustaw `ASPNETCORE_ENVIRONMENT=Development`.
- "401 w UI": token wygasl lub brak sesji - zaloguj się ponownie.
- "403 przy zapisie": jesteś instruktorem; akcje tworzenia/edycji są tylko dla admina.
- "Niedozwolony typ pliku" / "Plik przekracza limit": dozwolone PNG/JPG/GIF/WEBP/PDF, limit 10 MB.
- Obraz się nie pokazuje: sprawdź, czy backend działa (pliki serwowane są pod `/uploads/...`).
