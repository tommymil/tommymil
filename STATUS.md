# Lesson Runner — aktualny stan projektu

Ostatnia aktualizacja: 13.08.2026

Ten plik opisuje **stan bieżący** i plan prac. Powiązane dokumenty:

- [`system_zajec_online_programowanie_dla_dzieci.md`](system_zajec_online_programowanie_dla_dzieci.md)
  — docelowy zakres systemu i katalog sytuacji, które musi obsłużyć. **Źródło wymagań.**
- [`AUDYT_2026-07-27.md`](AUDYT_2026-07-27.md) — ostatni audyt kodu.
- [`UX_AUDYT_2026-08-02.html`](UX_AUDYT_2026-08-02.html) — audyt UX interfejsu w podziale
  na role, wraz z kierunkiem wizualnym. Wdrożony 03.08.2026.
- [`README.md`](README.md) — uruchamianie i konfiguracja.
- [`PLAN.md`](PLAN.md) — archiwum: historia budowy i wzorce architektoniczne.

## Co system robi

Aplikacja do prowadzenia zajęć programowania dla dzieci — online i stacjonarnie.

**Role i punkty wejścia**

| Rola | Widzi | Startowy ekran |
|---|---|---|
| `Admin` | wszystko: konspekty, kursy, grupy, uczestnicy, płatności, konta, powiadomienia, audyt, backup | `/admin/dashboard` |
| `Instructor` | wyłącznie swoje grupy i terminy (w tym zastępstwa), konspekty, kokpit prowadzenia | `/instructor/schedule` |
| `Parent` | wyłącznie dane powiązanych dzieci: harmonogram, link do zajęć, frekwencja, rozliczenia | `/parent/portal` |

**Obszary funkcjonalne**

- Konspekty: edytor, import z Markdown, upload obrazów/PDF/paczek projektów, cykl Draft → Review → Ready.
- Prowadzenie: kokpit nauczyciela (instrukcje, wskazówki, materiały, screeny), timery, tryb ciemny,
  okno przerwy, stan prowadzenia zapisywany per termin.
- Grupy i grafik: kursy jako szablony, cotygodniowe terminy z pomijaniem dni wolnych, lokalizacje,
  wykrywanie kolizji instruktora i sali, zastępstwa, przekładanie i odwoływanie terminów.
- Obecność: lista per termin, limit miejsc, lista oczekujących z automatycznym awansem,
  odrabianie zajęć w innym terminie, frekwencja per uczestnik, eksport CSV.
- Rozliczenia: cenniki, zapisy rozliczeniowe, faktury, wpłaty (ręczny provider za `IPaymentProvider`).
- Komunikacja: przypomnienia o zajęciach i powiadomienia o nieobecności (SMTP lub tryb log),
  szablony, log wysyłek z deduplikacją, warunek zgody RODO i adresu opiekuna.
- Operacje: audyt zdarzeń, `/health`, backup ZIP (baza + uploady), Serilog, Docker.

## Stan techniczny

- Backend: .NET 10, Clean Architecture, EF Core + SQLite, migracje stosowane przy starcie.
- Frontend: React 19 + Vite + TypeScript, podział MVVM-like, testy Vitest.
- Uwierzytelnianie: JWT (12 h) ze znacznikiem sesji — dezaktywacja konta oraz zmiana i reset
  hasła unieważniają token natychmiast. Hasło ustawia się samodzielnie: z resetu albo
  z zaproszenia wysłanego przez administratora.
- Wdrożenie: `docker compose up` (nginx serwuje frontend i proxuje `/api` do backendu).

## Zmiany z 27.07.2026 (po audycie)

Naprawione punkty 1–6 z [`AUDYT_2026-07-27.md`](AUDYT_2026-07-27.md):

1. **Fałszywe e-maile o nieobecności.** `SessionService` wysyłał powiadomienia przy starcie zajęć,
   gdy lista obecności była jeszcze pusta — każdy opiekun dostawał informację o nieobecności
   dziecka. Wysyłka przeniesiona do `FinishAsync`, czyli po zamknięciu listy.
2. **Rodzic czytał konspekty.** Nowa polityka `StaffOnly` na `/api/lessons`, `/api/schedule`
   i `/api/calendar` — rola `Parent` dostaje tam `403`.
3. **Blokada systemu przez dezaktywację admina.** `HasAnyAdminAsync` liczy tylko aktywne konta,
   a `UserAdminService` nie pozwala wyłączyć samego siebie ani ostatniego aktywnego administratora
   (`409` z czytelnym komunikatem).
4. **Utwardzenie logowania.** Limit 10 prób / 5 min na adres IP (`429` z `Retry-After`),
   dane kont developerskich znikają z ekranu logowania poza trybem DEV, `/health` nie ujawnia
   już komunikatu błędu bazy.
5. **Konfiguracja wdrożeniowa.** Sekrety wyprowadzone z `docker-compose.yml` do `.env`
   (wzorzec: `.env.example`, brak zmiennej = compose nie wstanie). Frontend nie ma już zaszytego
   `http://localhost:5000` — nginx proxuje `/api`, `/uploads` i `/download`, więc CORS jest zbędny.
   Wymuszanie HTTPS stało się opcją (`Security:ForceHttps`), bo za reverse proxy powodowało
   pętlę przekierowań; doszło `UseForwardedHeaders`, żeby rate limiter widział realne IP.
6. **Pozostałe z sekcji 2–3 audytu.** Unieważnianie tokenów (znacznik sesji), `Cache-Control:
   private, no-store` na `/uploads`, numeracja faktur per rok z ponowieniem przy kolizji,
   zastępstwa widoczne w kalendarzu, ochrona przed CSV injection w eksportach,
   checkpoint WAL przed kopią zapasową SQLite.
7. **Testy integracyjne HTTP** (`ApiAuthorizationTests`, `ApiSessionSecurityTests`) na realnym
   pipelinie API — pokrywają dokładnie te ścieżki uprawnień, których testy serwisów nie widzą.
8. **Polskie znaki.** Poprawione teksty w UI (Powiadomienia, Operacje, Profil, klient API)
   oraz w danych seed. Uwaga: obok brakujących ogonków były też **nadgorliwe podmiany** —
   „Zdarzenia że Sterowaniem” zamiast „ze”, „skroc pokaż do 10 s” zamiast „pokaz”,
   „postaći”, „odpowiedźi”, `zadańia` w kodzie Pythona dla dzieci. Dodany test
   `polishDiacritics.test.ts` skanuje źródła frontendu i pilnuje obu rodzajów błędów.

### Znalezione przy okazji: PostgreSQL nie zadziała bez własnych migracji

Komplet migracji w repozytorium jest wygenerowany dla SQLite (`type: "TEXT"`, `type: "INTEGER"`).
Na PostgreSQL `Guid` mapuje się na `uuid`, a `bool` na `boolean`, więc `Database.Migrate()`
przy starcie wywróci się na niezgodności typów. Dlatego `docker-compose.yml` domyślnie używa
teraz SQLite w wolumenie `api-data`, a usługa `postgres` jest schowana za profilem.
Przejście na PostgreSQL wymaga wygenerowania osobnego zestawu migracji — patrz README.

## Zmiany z 27.07.2026, część druga (punkt 7 audytu)

9. **Link do spotkania i nagranie na poziomie terminu.** `ScheduledSession.MeetingUrl`
   nadpisuje link grupy (zastępstwo prowadzi we własnym pokoju, odrabianie bywa gdzie indziej),
   a puste pole przywraca link grupy. `ScheduledSession.RecordingUrl` trafia do portalu rodzica
   po zakończeniu zajęć. Endpoint `PUT /api/groups/{id}/sessions/{sid}/links`, formularz „Linki”
   przy terminie w szczegółach grupy. Migracja `AddSessionMeetingAndRecordingUrl`.
10. **Materiały dla rodzica.** Portal pokazuje sekcję „Materiały po zajęciach”: pliki projektu
    lekcji (starter / wersja końcowa) i nagranie — **wyłącznie z terminów o statusie
    `Completed`**. Świadomie nie udostępniamy scenariusza prowadzenia ani notatek instruktora;
    pobieranie idzie przez `/download/lesson-files/{token}` (adres-klucz z nieodgadywalnym tokenem).

## Analiza luk wobec `system_zajec_online_programowanie_dla_dzieci.md`

Zestawienie wymagań z dokumentu koncepcyjnego (27.07.2026) ze stanem faktycznym kodu.
Legenda: ✅ jest · ⚠️ jest częściowo · ❌ brak.

**Dobra wiadomość na start:** najważniejsza zasada projektowa z rozdziału 16 — „zajęcia to nie
jest tylko wydarzenie w kalendarzu” — jest już spełniona. `ScheduledSession` to osobny rekord
z własnym terminem, statusem, obecnościami, notatką, zastępstwem, linkiem i nagraniem. Można
zmienić jeden termin bez ruszania całego harmonogramu. To fundament, którego nie trzeba przerabiać.

### 1. Terminy, odwołania, przesuwanie zajęć

| Wymaganie | Stan |
|---|---|
| Zajęcia jako osobny rekord | ✅ `ScheduledSession` |
| Przełożenie, odwołanie, zastępstwo | ✅ |
| Pomijanie dni wolnych, wykrywanie kolizji | ✅ |
| Rozszerzone statusy (potwierdzone, przełożone, odwołane przez instruktora/rodzica, przerwane technicznie, niezrealizowane, oczekujące na termin) | ❌ mamy 4: zaplanowane / w toku / zakończone / odwołane |
| Historia zmiany terminu: poprzedni termin, nowy, kto, powód, kiedy | ❌ nadpisujemy `ScheduledAt` bez śladu |
| Sposób powiadomienia i potwierdzenie rodzica | ❌ |
| Rozdzielenie zmiany terminu od rozliczenia (zwrot / kredyt / odrobienie / brak) | ❌ dziś odrabianie jest jedyną ścieżką |
| Przesunięcie numeracji kolejnych lekcji | ❌ |

### 2. Obecność

| Wymaganie | Stan |
|---|---|
| Lista obecności per termin | ✅ |
| Odrabianie w innym terminie | ✅ `MakeupRequired` + `MakeupSessionId` |
| Rozszerzone statusy (spóźniony, nieobecność zgłoszona / niezgłoszona, problemy techniczne, wyszedł wcześniej, częściowo, obecny ale nieaktywny) | ❌ mamy `bool Present` |
| Czas dołączenia i opuszczenia | ❌ |
| Notatka per dziecko per zajęcia („problemy z mikrofonem”, „wyszedł po 40 min”) | ❌ notatka jest tylko zbiorcza na terminie |
| Nieobecność zgłaszana przez rodzica | ❌ |

### 3. Problemy techniczne

Cały rozdział to **❌ biała plama**. Brak: testu technicznego przed pierwszymi zajęciami,
zapasowego linku do spotkania, instrukcji dołączenia, listy wymaganych programów, zadania
awaryjnego, rejestru zgłoszeń technicznych (`SupportTicket`) i historii problemów przy dziecku.
Przy zajęciach z programowania to obszar generujący najwięcej realnych strat czasu lekcyjnego.

### 4. Różny poziom dzieci

| Wymaganie | Stan |
|---|---|
| Scenariusz z krokami, wskazówkami, materiałami | ✅ |
| Projekt startowy i końcowy | ✅ `LessonProjectFiles` |
| Zadanie prostsze / dodatkowe / awaryjne, cel podstawowy, oczekiwany efekt | ❌ brak pól w modelu lekcji |
| Profil dziecka: poziom, tempo, znajomość środowisk, ostatni ukończony etap, rekomendacja | ❌ `Participant` ma tylko wolne `Notes` |
| Panel na żywo: kto potrzebuje pomocy / skończył / ma problem | ❌ |

### 5. Struktura zajęć: 45 + 5 przerwy + 45 (95 minut)

Timery lekcji i kroku, lista etapów, odhaczanie czynności, osobne okno przerwy — ✅ to działa
i jest mocną stroną kokpitu. Import pilnuje kształtu: ✅ zgłasza blok pracy dłuższy niż
45 minut i rozjazd sumy kroków z metadaną `Czas:`. Brakuje ❌ przypomnienia o przerwie
wyliczanego z planu **w trakcie zajęć** oraz ❌ zapisu „czego nie zdążyliśmy” w formie
strukturalnej (dziś tylko wolna notatka).

Uwaga: dokument koncepcyjny mówi o 90 minutach, a realny kształt zajęć to 95 (45 + 5 + 45).
Kod trzyma się 95 — `CalendarExport.DefaultDurationMinutes` i lint importu.

### 6. Komunikacja z rodzicami

| Wymaganie | Stan |
|---|---|
| Przypomnienie o zajęciach, informacja o nieobecności | ✅ |
| Szablony, log wysyłek, deduplikacja, warunek zgody RODO | ✅ |
| Pozostałe typy: zmiana terminu, prośba o potwierdzenie, podsumowanie zajęć, zadanie domowe, ostrzeżenie o zachowaniu, przypomnienie o płatności, koniec pakietu | ❌ |
| Dwoje opiekunów przy dziecku | ❌ jeden `GuardianEmail` na `Participant` |
| Historia ustaleń w systemie (`Message`) | ❌ |
| Link do spotkania w treści przypomnienia | ❌ szablon nie ma zmiennej `{{link}}` |

### 7. Płatności i kredyty

| Wymaganie | Stan |
|---|---|
| Cenniki, zapisy rozliczeniowe, faktury, wpłaty | ✅ |
| Okres próbny | ⚠️ status `Trial`, bez logiki pakietu |
| **Kredyty zajęciowe** (`LessonCredit`) | ❌ — dokument stawia je w centrum rozliczeń |
| Pakiet N zajęć, dołączenie/rezygnacja w połowie miesiąca, zawieszenie, rabat rodzeństwa | ❌ |
| Nadpłata, zwrot, reklamacja płatności | ❌ |
| Powiązanie operacji finansowej z konkretnymi zajęciami | ❌ faktura wisi na grupie, nie na terminie |
| Ślad audytowy każdej zmiany kwoty | ❌ audyt nie obejmuje modułu płatności |

### 8. Bezpieczeństwo dzieci i prywatność

| Wymaganie | Stan |
|---|---|
| Rozdzielenie ról: rodzic klientem, dziecko uczestnikiem | ✅ |
| Rodzic widzi wyłącznie dane swojego dziecka | ✅ |
| Brak prywatnego kanału dziecko–instruktor | ✅ (dzieci nie mają kont) |
| Zgoda na przetwarzanie danych i na wizerunek | ✅ |
| Osobna zgoda na nagrywanie i na publikację prac | ❌ |
| **Moduł incydentów** (`Incident`) | ❌ |
| Procedura zgłaszania niewłaściwego zachowania | ❌ |
| Historia działań administratora i instruktora | ⚠️ `AuditLog` obejmuje logowanie, konta i backup — **nie** obejmuje grup, terminów, obecności ani płatności |

### 9. Zarządzanie grupami

| Wymaganie | Stan |
|---|---|
| Limit uczestników, lista rezerwowa | ✅ |
| Przenoszenie między grupami | ⚠️ przez wypisanie i zapisanie, bez historii |
| Historia członkostwa (kiedy dołączył, kiedy odszedł, dlaczego) | ❌ `GroupEnrollment` nie ma daty wypisania |
| Poziomy zaawansowania, materiał wymagany przed dołączeniem | ❌ |
| Indywidualny postęp niezależny od postępu grupy | ❌ |
| Grupy próbne i tymczasowe do odrabiania | ❌ |

### 10. Nieobecność instruktora

Zastępstwo na terminie z dostępem do prowadzenia — ✅. Scenariusze i pliki startowe — ✅.
Brakuje ❌ operacji „jednym ruchem”: odwołaj + powód + powiadom rodziców + przyznaj kredyt +
przesuń numerację kolejnych lekcji. Dziś to cztery osobne kliknięcia i część z nich nie istnieje.

### 11. Postępy i projekty dzieci

| Wymaganie | Stan |
|---|---|
| `ProgressEntry` ze skalą samodzielności | ✅ pięć poziomów z rozdziału 11 |
| Ukończone lekcje | ✅ znacznik przy wpisie |
| Notatka dla rodzica i zadanie do poprawy | ✅ |
| `Project` / `ProjectSubmission`: link albo plik, wersje | ✅ |
| Komentarz instruktora do wersji | ✅ |
| Zdobyte umiejętności jako osobny słownik | ❌ na razie mieszczą się w notatce |
| Krótkie podsumowania okresowe | ⚠️ `ProgressEntry` bez terminu jest na to gotowy, brak ekranu |

### 12. Reklamacje i spory

Dane potrzebne do obrony: log powiadomień ✅, obecności ✅, historia płatności ⚠️,
historia terminów ❌, historia wiadomości ❌, historia zmian danych ⚠️.
Brak ❌ encji zgłoszenia/reklamacji ze statusem i osobą odpowiedzialną.

### 13. Encje — mapowanie

Istnieją: `Child`→`Participant`, `Guardian`→`User(Parent)`, `GuardianChildRelation`→`ParentParticipantLink`,
`Instructor`→`User(Instructor)`, `Course`, `Group`, `GroupMembership`→`GroupEnrollment`,
`LessonPlan`→`Lesson`, `LessonOccurrence`→`ScheduledSession`, `Attendance`, `Payment`,
`Invoice`, `Notification`→`NotificationLog`, `AuditLog`.

Brakuje: `RescheduleRequest`, `MakeupLesson` (dziś dwa pola na `AttendanceRecord`),
`LessonCredit`, `Project`, `ProjectSubmission`, `ProgressEntry`, `Message`, `Consent`
(dziś dwie kolumny na `Participant`), `TechnicalCheck`, `SupportTicket`, `Incident`.

**Dług modelowy do naprawienia przy okazji:** dane opiekuna są zduplikowane —
`Participant.GuardianName/Email/Phone` żyje obok konta `User(Parent)` powiązanego przez
`ParentParticipantLink`. Powiadomienia idą na `GuardianEmail`, a portal na konto. Przy dwóch
opiekunach to się rozjedzie. Docelowo jedno źródło prawdy: opiekun = konto, relacja = tabela.

### 14. MVP z dokumentu — stan realizacji

*Stan na 04.08.2026, po etapie B, C, przebudowie interfejsu i etapie D p.15–16.*

1. Konta opiekunów i dzieci ✅ · 2. Grupy i przypisywanie ✅ · 3. Kalendarz cykliczny ze zmianą
pojedynczych zajęć ✅ · 4. Link do spotkania ✅ · 5. Automatyczne przypomnienia ✅ ·
6. Obecność i spóźnienia ✅ *(etap A1)* · 7. Odwołanie / przełożenie / odrabianie ✅
(powody, statusy, kredyty i kreator z podsumowaniem skutków) · 8. Notatka po zajęciach ✅
*(od 03.08 w trzech polach: wewnętrzna, „czego nie zdążyliśmy”, podsumowanie dla rodzica)* ·
9. Scenariusze i materiały ✅ · 10. Projekty dzieci ✅ · 11. Płatności, **kredyty**
i rozliczenia ✅ *(pakiety zajęć zostają w etapie B p.7)* · 12. Historia komunikacji ⚠️
(log wysyłek obejmuje już zmiany terminu, podsumowania i płatności — nadal bez wątku
i bez encji `Message`) · 13. Zgody i uprawnienia ✅ *(rodzic zarządza zgodą na wizerunek
samodzielnie)* · 14. Rejestr problemów technicznych i incydentów ✅ *(etap D p.15–16,
04.08.2026 — dwa osobne rejestry, zgłoszenie z kokpitu, historia problemów przy dziecku)*.

**Podsumowanie: 13 z 14 punktów MVP zamknięte, 1 częściowo.**
(Przed 04.08 było 12 / 1 / 1; przed 03.08 — 9 / 3 / 2.)

Do domknięcia MVP zostaje **jeden** obszar: **historia komunikacji jako wątek** (encja
`Message`). Log wysyłek pokrywa dziś kierunek szkoła → rodzic; brakuje kierunku odwrotnego
i powiązania wiadomości w wątek.
(Przed etapem A było 8 / 4 / 2.)

---

## Zmiany z 03.08.2026 — przebudowa interfejsu po audycie UX

Audyt: [`UX_AUDYT_2026-08-02.html`](UX_AUDYT_2026-08-02.html) — 40 ustaleń w podziale na role.
Wniosek główny: model danych jest dojrzały, ale interfejs był zbudowany pod administratora
i podany wszystkim trzem rolom. Najbardziej cierpiał na tym rodzic, czyli klient szkoły.

### Warstwa wspólna

- **Tokeny projektowe** (`styles/tokens.css`): dwie warstwy — paleta i semantyka. W `styles.css`
  nie ma już **żadnego** koloru wpisanego wprost (było 27 wystąpień `#4f46e5`) ani żadnej łatki
  `:root[data-theme="dark"] .komponent` (było siedem). Doszły tokeny statusów terminu, obecności
  i pracy na żywo — ten sam status wygląda teraz identycznie w kalendarzu, grupie, kokpicie
  i portalu rodzica.
- **Dwie skóry, jeden system.** `[data-skin="parent"]` przedefiniowuje wyłącznie warstwę
  semantyczną: cieplejszy akcent, większe promienie i większy tekst bazowy. Panel roboczy
  zostaje chłodny i gęsty.
- **Biblioteka komponentów** w `components/ui`: `Dialog`, `EmptyState`, `Skeleton`,
  `StatusBadge`, `SegmentedControl`, `Tabs`, `ActionMenu`.
- **Koniec z natywnymi oknami przeglądarki.** Siedem wywołań `window.confirm` / `window.prompt`
  / `window.alert` zastąpił `DialogProvider`. Usunięcie grupy nie miało **żadnego**
  potwierdzenia — teraz, tak jak anonimizacja danych dziecka, wymaga przepisania nazwy.
- **Dostępność:** jedna globalna reguła `:focus-visible` (były dwie reguły na całą aplikację),
  odnośnik „przejdź do treści”, pułapka fokusu i obsługa Escape we wszystkich oknach
  modalnych, `role="alert"` na błędach. Token `--text-muted` przesunięty na `#556070` —
  poprzedni dawał na `--surface-2` około 4,3:1, poniżej progu dla tekstu w 12–13 px.
- **Router:** `errorElement`, trasa `*` i ekran 403. Strażnicy ról pokazują teraz ekran braku
  uprawnień zamiast cichego przekierowania na sztywno wpisany adres.
- **Wyszukiwanie globalne** (`Ctrl+K`, `GET /api/search`, `StaffOnly`). Instruktor przeszukuje
  wyłącznie swoje grupy i dzieci do nich zapisane; rola `Parent` nie ma tu wstępu w ogóle.
  Notatek o dzieciach świadomie nie przeszukujemy — trafiają tam uwagi o sytuacji rodzinnej.

### Portal rodzica

- **Własny układ** (`ParentShell`): górna belka zamiast 292-pikselowego paska bocznego z dwiema
  pozycjami, dolna nawigacja na telefonie, ciepła skóra. Rodzic dostawał wcześniej ten sam
  szkielet co administrator, razem z belką „Dostępne opcje są po lewej stronie”.
- **Hierarchia zamiast siedmiu sekcji naraz:** kafel najbliższych zajęć z przyciskiem „Dołącz”,
  cztery liczby i zakładki (Plan · Postępy · Materiały · Rozliczenia · Zgody).
- **Nowe wskaźniki:** kwota do zapłaty z terminem, postęp w kursie, kredyty i frekwencja.
  Wcześniej były to „Dzieci: 1”, „Rozliczenia: 3” — czyli liczba faktur zamiast kwoty.
- **Kredyty zajęciowe w portalu.** Istniały od 27.07, ale wyłącznie w panelu administratora —
  osoba, której się należały, nie miała jak się o nich dowiedzieć.
- **Zgłoszenie nieobecności** przez dialog z opisem skutków zamiast `window.prompt`, z toastem
  potwierdzającym. Był to najbardziej amatorski moment w aplikacji i trafiał w klienta.
- **Zgody opiekuna** (`PUT /api/parent/consents`). Zgoda na wizerunek jest przełącznikiem —
  przy RODO to opiekun jest stroną, a dotąd było to pole ustawiane przez administratora.
  Zgoda na przetwarzanie danych zostaje do wglądu: bez niej nie da się prowadzić dziennika,
  więc jej wycofanie to rozmowa z administracją, a nie przełącznik.
- **Przełącznik dziecka** przy rodzeństwie, filtrujący wszystkie sekcje naraz.
- **Daty względne** (`dziś o 17:00`, `we wtorek o 17:00`) i poprawna pluralizacja przez
  `Intl.PluralRules` — w interfejsie klienta widniało wcześniej `3 wersje/wersji`.
- **Karta projektu** z datą, komentarzem instruktora i wyróżnioną najnowszą wersją; starsze
  w historii. Wcześniej był to rząd przycisków „Wersja 1, 2, 3” bez dat.
- **Konto rodzica w seedzie** (`parent@lessonrunner.local`) wraz z powiązaniem do dwojga
  dzieci. Portal był kompletny w kodzie, ale nie istniało konto, którym dałoby się na niego
  wejść — a bez `ParentParticipantLink` sama rola pokazuje pusty ekran. Przy okazji
  `SeedAsync` dokłada brakujące konta per e-mail, zamiast wychodzić przy pierwszym
  istniejącym użytkowniku.
- **Prowadzący podpisany rolą, nie adresem.** `User.DisplayName` przy koncie bez imienia
  zwraca e-mail, więc służbowy adres pracownika trafiał do klienta szkoły jako „prowadzi…”.
  Portal używa teraz własnego fallbacku „Instruktor”.
### Lekcje próbne 1:1 (pozyskanie klienta)

Osobny moduł od zgłoszenia do zapisanego uczestnika. Dziecko zgłasza się samo, my umawiamy
je jeden na jeden z instruktorem i sprawdzamy, czy odnajdzie się w programowaniu.

- **`TrialLesson` jest osobną encją, a nie grupą jednoosobową.** Grupa niosłaby ze sobą
  uczestnika, a uczestnika nie ma i nie tworzymy go na zapas: dopóki rodzina nie zdecyduje,
  dziecko nie ma po co pojawiać się w bazie uczestników, na listach frekwencji ani
  w rozliczeniach. Dane kandydata żyją przy zgłoszeniu i przenoszą się przy zapisie.
- **Diagnoza zamiast obecności.** Instruktor odpowiada na trzy pytania (czytanie, obsługa
  komputera, doświadczenie) i stawia rekomendację. Bez kompletu odpowiedzi zgłoszenie nie
  przechodzi do stanu „po lekcji”, a administracja nie może zapisać dziecka — decyzja bez
  diagnozy byłaby zgadywaniem.
- **Dwie trasy, dwie odpowiedzialności.** `/api/trials` (`AdminOnly`) prowadzi sprawę,
  `/api/my-trials` (`StaffOnly`) daje instruktorowi wyłącznie własne kandydatury i zapis
  diagnozy. Wspólna trasa oznaczałaby, że każdy prowadzący czyta dane kontaktowe wszystkich
  rodzin, które kiedykolwiek się zgłosiły.
- **Zapis do systemu** tworzy uczestnika z danych zgłoszenia, przenosi obserwacje z lekcji
  do jego notatek i zakłada opiekunowi konto z zaproszeniem (ten sam przepływ co z karty
  dziecka). Zgód RODO **nie przenosimy** — nikt ich jeszcze nie udzielił. Do grupy zapisuje
  się osobno, bo poziom wynika z rekomendacji, a nie z faktu przyjęcia.
- Konto opiekuna jest dodatkiem, nie warunkiem: gdy adres okaże się zajęty przez pracownika
  albo poczta padnie, dziecko i tak zostaje uczestnikiem, a ekran mówi, czego zabrakło.
- Ekrany są rozdzielone: „Lekcje próbne” u administratora (zgłoszenia / umówione / zamknięte)
  i osobna pozycja u instruktora, poza grafikiem grup.

- **Import konspektu przestał gubić treść po cichu.** Parser zwraca `issues` z numerem linii,
  rozdzielone na błędy (fragment nie trafi do konspektu) i uwagi (parser coś przyjął za
  autora). Import z błędami wymaga zaznaczenia zgody, a zgoda kasuje się po każdej edycji
  pliku. Metadana `Czas:` pozwala sprawdzić, czy kroki wypełniają zajęcia; lint zgłasza brak
  przerwy powyżej 60 min, brak `intro`/`summary`, krok dłuższy niż 20 min i brak celu lekcji.
- **Format konspektu o to, czego nie było gdzie zapisać:** `Cel:` (`Lesson.Objective`) oraz
  sekcje `### Po zajęciach dziecko potrafi` / `### Przygotuj przed zajęciami` /
  `### Zadanie domowe` przed pierwszym krokiem. W kokpicie cel wisi przez całe zajęcia,
  przygotowanie pokazuje się na pierwszym kroku, a kryteria i zadanie domowe na ostatnim.
  Doszły wskazówki `[dla szybszych]` i `[gdy nie zdążysz]` (rozbicie `pace`, który mieszał
  dwie przeciwne sytuacje), znacznik `[mów]` dla kwestii do wypowiedzenia wprost, rozdzielona
  etykieta i język bloku kodu (`[kod] Etykieta | scratch:`) oraz obrazy `![podpis](url)`.
  Lekcje leżą w bazie jako JSON, więc żadne z tych pól nie wymagało migracji.
- **Konto opiekuna z karty dziecka** (`POST /api/participants/{id}/guardian-account`,
  `AdminOnly`): zakłada konto z zapisanych danych opiekuna, wiąże je z dzieckiem i wysyła
  zaproszenie — jednym przyciskiem zamiast przepisywania adresu do panelu użytkowników
  i osobnego powiązania. Istniejące konto jest wyłącznie dowiązywane (drugie dziecko tej
  samej rodziny nie dostaje kolejnego maila o ustawianiu hasła), a adres należący do
  pracownika jest odrzucany.

### Kokpit instruktora

- **Stała szyna zamiast trzech nakładek.** Scenariusz nie znika już przy odhaczaniu obecności,
  wpisywaniu postępów ani kończeniu zajęć.
- **Autozapis obecności.** Zmiany żyły wcześniej wyłącznie w stanie komponentu do momentu
  kliknięcia przycisku zapisu, a zamknięcie okna nie ostrzegało — przy dziesięciorgu dzieci
  utrata listy była kwestią czasu. Backend przyjmuje listę częściową, więc autozapis jest
  bezpieczny; stan pokazuje pasek „Zapisano 17:12”.
- **Segmentowa obecność:** trzy najczęstsze stany widoczne od razu, pozostałe pod „Więcej”.
  Wcześniej każde dziecko miało pięć kontrolek naraz, w oknie modalnym, w trakcie zajęć.
- **Znaczniki pracy na żywo** (`LiveWorkStatus`, `PUT /api/schedule/{id}/live-status`):
  pracuje / potrzebuje pomocy / skończył / problem techniczny. Lista sortuje się po pilności
  po stronie backendu. To punkt 25 planu i rozdział 4 dokumentu koncepcyjnego.

  Decyzje warte zapamiętania:
  - Znacznik **nie trafia do portalu rodzica**. „Potrzebuje pomocy” jest informacją
    organizacyjną dla prowadzącego; pokazany rodzicowi zamieniłby się w etykietę przypiętą
    do dziecka.
  - Idzie **osobnym, wąskim żądaniem** z pominięciem autozapisu: instruktor klika to co
    kilkadziesiąt sekund, a przepychanie przy tym całej listy obecności groziłoby nadpisaniem
    świeżej zmiany danymi sprzed chwili.
  - Nierozpoznany znacznik oznacza „bez zmiany”, a nie „Working” — literówka we froncie nie
    może wyzerować stanu całej listy.
- **Strukturalne zakończenie zajęć:** notatka wewnętrzna, „czego nie zdążyliśmy”
  (`ScheduledSession.UnfinishedNote`) i podsumowanie dla rodzica (`ParentSummary`).
  Wszystko szło wcześniej do jednego pola, z którego nic nie dało się odczytać automatycznie.
  „Czego nie zdążyliśmy” pokazuje się teraz wprost na osi kursu w grafiku.
- **Przypomnienie o przerwie** po 42 minutach od startu — jako belka, nie okno modalne.
  „Instruktor zapomni o przerwie” to pierwsza pozycja z listy problemów rozdziału 5.
- **Widok dnia w grafiku** i połączenie „Kalendarza”, „Grafiku” i „Szybkiego startu”
  w jedną pozycję menu. Dwie z nich miały w dodatku tę samą ikonę.

### Panel administracji

- **Kreator odwołania zajęć** z podsumowaniem skutków przed potwierdzeniem: ile dzieci, ile
  kredytów, co z materiałem, co z powiadomieniem. Wcześniej ta decyzja — najważniejsza
  operacja w systemie — była rozsypana po czterech kontrolkach w wierszu listy.
- **Odwołanie zbiorcze** (ferie, święta): zaznaczenie wielu terminów i jedno okno. Realizowane
  istniejącym endpointem w pętli — przy tygodniu ferii to kilka żądań, a osobna operacja
  zbiorcza w backendzie i tak robiłaby dokładnie to samo.
- **Menu akcji zamiast `<select>` jako spustu operacji.** Wybór pozycji w liście rozwijanej
  wykonywał zmianę statusu natychmiast, bez potwierdzenia i bez możliwości cofnięcia.
- **Zakładki w szczegółach grupy** (Terminy · Uczestnicy · Frekwencja · Historia · Ustawienia).
  Formularze wyszły z wierszy listy do okien dialogowych — `SessionRow` miał dziesięć propsów
  i osiem stanów lokalnych.
- **Pulpit „Wymaga uwagi”** nad wskaźnikami: terminy bez lekcji, grupy bez prowadzącego,
  dzieci z frekwencją poniżej 50%, faktury po terminie, kredyty tracące ważność, wolne miejsca
  przy niepustej liście rezerwowej. Każda pozycja z odnośnikiem do miejsca załatwienia sprawy.
- **Okruszki w belce** zamiast napisu „Dostępne opcje są po lewej stronie” i szuflada mobilna
  zamiast rozwijania całego menu nad treścią.

### Migracja

`AddSessionDebriefAndLiveStatus` (03.08.2026): `ScheduledSessions.UnfinishedNote`,
`ScheduledSessions.ParentSummary`, `AttendanceRecords.LiveStatus`. Istniejące wiersze dostają
`LiveStatus = 'Working'`; repozytorium ma ten sam fallback przy odczycie, tak jak przy
`AddAttendanceStatus`. Migracja napisana ręcznie — bez uruchomionego `dotnet ef`.

### Znalezione przy okazji

- **Naruszenie zasady o cudzysłowach w kodzie sprzed audytu.** `CLAUDE.md` wymaga par `„…”`,
  a w `types/group.ts`, `styles.css`, `LessonEditorPage.tsx`, `ResetPasswordPage.tsx`
  i kilku innych plikach otwierający `„` był zamykany zwykłym `”`. Test
  `polishDiacritics.test.ts` powinien był to wyłapać. Poprawione w całym `src` (38 plików).
- **Kotwica dostępności i wartości enumów po polsku bez ogonków** (`id="tresc"`,
  `"obecnosc"`) wpadały w strażnika diakrytyków. Zmienione na angielskie identyfikatory —
  zgodnie z zasadą, że elementy techniczne zostają w ASCII.
- **Audyt przeszacował liczbę zduplikowanych selektorów w CSS.** Z dziesięciu „duplikatów”
  dziewięć okazało się poprawnym wzorcem: reguła wspólna dla kilku selektorów plus
  nadpisanie dla jednego z nich. Faktycznie zbędny był tylko `.sub-item` — scalony.

## Zmiany z 03.08.2026, część druga — testy i pozostałe powiadomienia

### Testy

- **Przepisane pod nowe widoki:** `SessionCockpitPage.test.tsx` (szyna zamiast modali,
  autozapis, znacznik pracy osobnym żądaniem, zakończenie w trzech polach),
  `InstructorSchedulePage.test.tsx` (pasek dnia, zwijanie przyszłych lekcji),
  `AdminParticipantsPage.test.tsx` (potwierdzenia w oknach dialogowych).
- **Nowe:** `Dialog.test.tsx` (semantyka, Escape, pułapka i powrót fokusu),
  `DialogContext.test.tsx` (kontrakt `confirm`/`prompt`, przepisanie nazwy),
  `SegmentedControl.test.tsx`, `CancelSessionDialog.test.tsx` (skutki przed decyzją,
  wymagany powód), `datetime.test.ts` (daty względne, odmiana dni, pluralizacja).
- **`ApiAuthorizationTests`:** `GET /api/search` (StaffOnly, rodzic dostaje 403),
  `PUT /api/parent/consents` (ParentOnly, cudze dziecko → 404, nie 403),
  `PUT /api/schedule/{id}/live-status` (StaffOnly + właściciel terminu).

### Sprzątanie po przebudowie kokpitu

Z `useSessionCockpitViewModel` zniknęły `showAttendance`, `openAttendance`,
`closeAttendance` i `togglePresent` — nic ich już nie używało po przejściu na szynę.
`setAllPresent` **zostaje** i wróciło do interfejsu: na początku zajęć zwykle są wszyscy,
więc jedno kliknięcie zamiast dziesięciu ma realną wartość.

### Błąd gramatyczny znaleziony przy pisaniu testów

`formatFriendlyDateTime` sklejało przyimek z mianownikiem z `Intl`, dając **„w sobota
o 17:00”**. `Intl` nie zna polskiej deklinacji, więc dni tygodnia w bierniku są teraz
krótką, zamkniętą tabelą w `datetime.ts`. To dokładnie ten rodzaj błędu, który przechodzi
przez przegląd kodu i wychodzi dopiero w mailu do rodzica.

### Etap C p.13 — pozostałe typy powiadomień ✅

Doszły cztery typy: **zmiana terminu**, **odwołanie zajęć**, **podsumowanie zajęć**
i **przypomnienie o płatności**.

Decyzje warte zapamiętania:

- **Znacznik „powiadomiono opiekunów” wypełnia się sam.** Przy przełożeniu i odwołaniu
  `GroupService` wywołuje wysyłkę i zapisuje do historii **wynik** tej wysyłki, a nie
  deklarację z formularza. Wcześniej system prosił człowieka o potwierdzenie faktu,
  którego sam nie sprawdzał, i zapisywał tę deklarację jako dowód przy reklamacji
  „nie dostaliśmy informacji o zmianie”. Checkbox w interfejsie zostaje jako wariant
  awaryjny (telefon, SMS) — sumuje się z wysyłką, nie zastępuje jej.
- **Błąd wysyłki nie wywraca operacji na terminie.** Lepiej odwołać zajęcia bez maila niż
  nie odwołać wcale; nieudana próba i tak zostaje w dzienniku, a historia zapisze wtedy
  „bez powiadomienia”, czyli prawdę.
- **Podsumowanie idzie tylko wtedy, gdy instruktor je napisał** (`ParentSummary`).
  Notatka wewnętrzna terminu nie przechodzi tędy w ogóle — to była najprostsza droga,
  żeby uwaga dla zespołu wylądowała w skrzynce opiekuna. Pusty mail „zajęcia się odbyły”
  jest gorszy niż brak maila.
- **Przypomnienia o płatnościach nie mają automatu ani przełącznika w ustawieniach.**
  Uruchamia je administrator przyciskiem, po potwierdzeniu w oknie z opisem skutków,
  i widzi w odpowiedzi, ile wiadomości wyszło. Upominanie się o pieniądze to decyzja
  biznesowa i wizerunkowa, a nie ustawienie techniczne — nie powinna zapadać raz
  i działać po cichu.
- **Zaległość liczymy z daty, nie ze statusu faktury.** Status `Overdue` zmienia się
  dopiero przy jakiejś operacji na dokumencie, więc faktura po terminie potrafiłaby
  tygodniami udawać „Do zapłaty”. Ta sama zasada obowiązuje w portalu rodzica i na pulpicie.
- **Klucz deduplikacji przy zmianie terminu niesie nowy termin**
  (`reschedule:{id}:{scheduledAt}:{email}`). Ten sam termin bywa przekładany kilka razy,
  a rodzic ma dostać informację o każdej zmianie — deduplikacja po samym identyfikatorze
  terminu zjadłaby drugą i trzecią.
- **Szablony tych czterech typów są wbudowane w domenę (`NotificationTemplates`),
  nie edytowalne w panelu.** Ta sama decyzja co przy e-mailach o dostępie do konta.
  Każdy z nich niesie zmienne, których nie ma w pozostałych (`{{previousAt}}`,
  `{{reason}}`, `{{summary}}`, `{{amount}}`, `{{dueDate}}`); wpuszczenie ich do wspólnego
  edytora oznaczałoby, że da się wstawić `{{amount}}` do przypomnienia o zajęciach
  i dostać w mailu surowy `{{amount}}`. Wyniesienie do ustawień to osobna zmiana:
  dziewięć kolumn, migracja i walidacja zmiennych per typ.

Nierozstrzygnięte: **„koniec pakietu”** z rozdziału 6 czeka na pakiety zajęć (etap B p.7) —
bez nich nie ma czego liczyć.

### Etap E p.27 — CI ✅

`.github/workflows/ci.yml`: `dotnet build` + `dotnet test` i `npm run build` + `npm test`
na każdy push i pull request, w dwóch niezależnych zadaniach.

Powstał z konkretnego powodu: przebudowa z 03.08 trafiła do repozytorium bez ani jednego
przebiegu kompilacji i testów. Ta sytuacja nie powinna być możliwa drugi raz.

### Etap E p.26 — wydajność, dwa najgorsze miejsca ✅

- **`GroupService.GetSummariesAsync`** wołał `userRepository.GetByIdAsync` w pętli po grupach
  — klasyczne N+1 po tabeli, która ma kilkanaście wierszy. Instruktorzy pobierani są teraz
  raz, przed pętlą.
- **`SessionService.SetLiveStatusAsync`** przestał liczyć listę terminów odrabiania.
  `MakeupSessionOptionsAsync` czyta **wszystkie grupy i wszystkie lekcje**, a znacznik pracy
  jest wołany co kilkadziesiąt sekund w trakcie zajęć — to najczęściej wywoływana operacja
  w całym systemie. Lista terminów odrabiania i tak się w trakcie lekcji nie zmienia, więc
  front scala odpowiedź zamiast podmieniać cały obiekt.

**Zostaje:** `ParentPortalService.GetPortalAsync` nadal czyta wszystkie grupy i filtruje
w pamięci. Poprawka wymaga nowej metody repozytorium (`ListByParticipantsAsync`) — świadomie
odłożona do czasu, aż kod przejdzie kompilację, żeby nie dokładać niezweryfikowanej warstwy
dostępu do danych.

### Przegląd własnego kodu przed kompilacją — trzy realne błędy

Skoro backend powstał bez ani jednego `dotnet build`, przegląd był wart więcej niż kolejna
funkcja. Znalezione i naprawione:

1. **Klucz deduplikacji mógł przekroczyć kolumnę.** `NotificationLogs.DedupeKey` ma 220 znaków,
   a klucz zawiera adres e-mail, który sam może mieć 254. Przy długim adresie klucz był po cichu
   za długi: SQLite tego nie pilnuje, więc problem nie dawał znaku o sobie, ale na PostgreSQL
   zapis by się wywrócił, a przy obcięciu **dwa różne adresy dałyby ten sam klucz i wygasiły
   drugą wiadomość**. Dotyczyło to również kluczy sprzed 03.08 (`absence`, `reminder`).
   Wszystkie klucze idą teraz przez `DedupeKey(...)`: czytelne w typowym przypadku, a po
   przekroczeniu limitu ogon zastępuje deterministyczny skrót SHA-256.

2. **Wyłączenie przypomnień wyciszało informację o odwołaniu zajęć.** Podpiąłem wysyłkę pod
   `RemindersEnabled` bez rozróżnienia. „Nie przypominaj mi co tydzień” to nie to samo, co
   „nie mów mi, że zajęcia się nie odbędą” — rodzic, który przywiezie dziecko na odwołane
   zajęcia, ma pełne prawo do pretensji. **Odwołanie idzie teraz zawsze**, niezależnie
   od ustawień; przełożenie i podsumowanie zostają pod przełącznikiem.

3. **Podsumowanie zajęć nie miało żadnej bramki.** Szkoła, która wyłączyła pocztę o zajęciach,
   i tak dostawałaby podsumowania. Dołożone pod `RemindersEnabled`, z komentarzem, że osobny
   przełącznik czeka na kolumny w ustawieniach.

Plus drobiazg: jawne rzutowanie `(DateOnly?)` w `BuildSummary` — `null` i `DateOnly` nie mają
wspólnego typu, więc bez niego kompilator opiera się wyłącznie na typie docelowym parametru.

### Czwarty błąd: kreator odwołania mówił nieprawdę

Wyszedł przy porównaniu okna z tym, co faktycznie robi backend po wdrożeniu powiadomień.

`CancelSessionDialog` twierdził: **„Zaznaczysz w historii, że opiekunowie zostali
poinformowani. Wiadomości wyślij osobno w panelu Powiadomienia”** — a e-maile o odwołaniu
wychodzą od tej pory automatycznie i bezwarunkowo. Okno, które źle opisuje skutki, jest
gorsze niż okno bez opisu, bo administrator mu ufa i planuje pracę wokół niego.

Poprawione we wszystkich trzech miejscach:

- **Odwołanie:** „Opiekunowie N dzieci dostaną e-mail o odwołaniu — od razu po potwierdzeniu.
  Każdy adres dostanie go raz.”
- **Odwołanie zbiorcze:** wprost liczba wiadomości (jedna na termin) — zanim administrator
  zaznaczy dwanaście terminów ferii.
- **Przełożenie:** z zastrzeżeniem „jeśli powiadomienia o zajęciach są włączone”, bo
  przełożenie — w odróżnieniu od odwołania — podlega przełącznikowi w ustawieniach.

Checkbox „Opiekunowie zostali poinformowani” zmienił znaczenie i etykietę na **„Dodatkowo
poinformowano opiekunów telefonicznie lub SMS-em”**, domyślnie odznaczony. System powiadamia
sam i zapisuje do historii wynik własnej wysyłki; checkbox jest już tylko śladem kanału
zapasowego, o którym system nie ma skąd wiedzieć.

### Pierwsze uruchomienie testów: 205/208, trzy błędy — żaden nowy

Kompilacja backendu przeszła bez błędu, `tsc` też. Testy wyłapały trzy rzeczy i **wszystkie
trzy istniały przed przebudową interfejsu**:

**1. SQLite nie tłumaczy `DateTimeOffset` w zapytaniach — cztery miejsca.**

Repozytoria w większości już to obchodzą (materializują listę i sortują po stronie klienta,
z komentarzem przy każdym takim miejscu). Cztery metody wyłamywały się z tej konwencji
i **wywracały się przy każdym wywołaniu**:

| Metoda | Skutek dla użytkownika |
|---|---|
| `EfBillingRepository.ListCreditsAsync` | lista kredytów nigdy się nie ładowała |
| `EfGroupRepository.ListSessionChangesAsync` | historia zmian terminów nie działała |
| `EfProgressRepository.ListByParticipantsAsync` | postępy dziecka nie ładowały się |
| `EfAccountTokenRepository.CountIssuedSinceAsync` | formularz „nie pamiętam hasła” rzucał wyjątkiem |

Czerwony test pokazał tylko pierwszą z nich, bo pulpit zaczął od 03.08 czytać kredyty.
Pozostałe trzy wyszły przy przeszukaniu kodu pod tym samym kątem — **testy nie pokrywały
tych ścieżek w ogóle**. Wszystkie cztery poprawione zgodnie z konwencją, która była już
w projekcie.

Sprawdzone przy okazji: `EfSchedulingRepository.ListHolidaysAsync` sortuje po `DateOnly`,
a to SQLite tłumaczy poprawnie — zostaje bez zmian.

**2. Podwójny zapis wersji projektu.** `ProgressService.AddSubmissionAsync` wołało
`repository.AddSubmissionAsync(...)`, a zaraz potem bezwarunkowo `project.Submissions.Add(...)`.
Repozytorium EF pracuje na własnych dokumentach i grafu domeny nie rusza, ale implementacja
in-memory operuje na **tym samym** obiekcie projektu — wersja trafiała na listę dwa razy.
Dopisanie jest teraz idempotentne, więc metoda daje ten sam wynik niezależnie od tego,
czy repozytorium współdzieli graf.

**Wniosek:** przy 208 testach i pełnej zieloności kompilatora trzy realne błędy produkcyjne
siedziały w kodzie od lipca. Trzy z czterech miejsc z punktu 1 **nadal nie mają testu** —
wykryło je przeszukanie kodu, nie zestaw testowy.

### Testy frontendu: 151/151 po poprawkach (pierwszy przebieg: 145/151)

**Pułapka fokusu w `Dialog` nie działała w ogóle.** Filtr widocznych elementów opierał się na
`offsetParent !== null`, a jsdom zwraca tu zawsze `null` — lista elementów do sfokusowania
wychodziła pusta, więc fokus nigdy nie wchodził do okna, a obsługa Tab nie miała czego pilnować.
W przeglądarce byłoby podobnie dla elementów w kontenerze `position: fixed`, a scrim właśnie
taki jest. Warunek jest teraz strukturalny (`hidden`, `aria-hidden`), więc daje ten sam wynik
w przeglądarce i w testach.

To był jedyny błąd produkcyjny z tej szóstki — pozostałe pięć to wady moich testów:

- **Zbyt wczesna asercja.** `PresenterCockpit` ładuje lekcję asynchronicznie i do tego czasu
  renderuje sam stan ładowania, bez sterowania. Pomocnik `startClass` czeka teraz na przyciski.
- **Dwa testy zależne od czasu.** Sprawdzały treść żądania po autozapisie, więc wynik zależał
  od tego, ile milisekund minęło między klikanymi kontrolkami. Wymuszają teraz zapis
  przyciskiem „Zapisz teraz”; sam debounce pokrywa osobny, przechodzący test.
- **Dane testowe kolidujące z interfejsem.** `parentSummary` miało treść identyczną z nagłówkiem
  sekcji, więc zapytanie trafiało w oba naraz.
- **Zapytanie zbyt szerokie.** Po zwinięciu przyszłych lekcji przycisk „Otwórz” jest w karcie
  dwa razy — w osi kursu i w rozwinięciu. Zapytanie zawężone do osi.

**Wniosek z całej weryfikacji:** kompilacja i testy wyłapały łącznie dziewięć rzeczy.
Cztery to błędy produkcyjne — trzy z nich starsze niż przebudowa, jeden mój. Pięć to wady
testów, które napisałem, nie widząc działającego interfejsu. Ani jeden błąd produkcyjny
nie pochodził z samej przebudowy interfejsu.

## Zmiany z 04.08.2026 — etap D, punkty 15–16 (backend)

Incydenty (rozdział 8) i zgłoszenia techniczne (rozdział 3). Backend kompletny, interfejs
w kolejnym kroku.

### Dlaczego dwie osobne encje, a nie jedna „zdarzenia"

Zepsuty mikrofon i nękanie dziecka nie mają ze sobą nic wspólnego poza tym, że oba są
zdarzeniem. Wspólna tabela wymuszałaby wspólne uprawnienia, a te **muszą** się różnić —
i to jest właściwy powód rozdzielenia, nie estetyka modelu.

### Incydenty — decyzje

- **Nie trafiają do portalu rodzica.** Nigdy, w żadnej formie. O incydencie informuje
  człowiek w rozmowie, a nie powiadomienie wygenerowane z rekordu.
- **Instruktor widzi wyłącznie własne zgłoszenia.** Wśród rodzajów jest „zachowanie osoby
  prowadzącej"; rejestr, w którym każdy instruktor czyta zgłoszenia o wszystkich, jest
  rejestrem, do którego nikt nie zgłosi niczego niewygodnego.
- **Prowadzenie sprawy tylko dla administracji** (`AdminOnly` na trasie aktualizacji).
  Zgłaszać może każdy pracownik.
- **Zamknięcie wymaga opisu rozwiązania.** Sprawa ze statusem „rozwiązana" i pustym polem
  nie jest dowodem na nic.
- **Sprawy dotyczące dobra dziecka i zachowania personelu są z definicji pilne**, niezależnie
  od wagi wybranej przez zgłaszającego — żeby zgłoszenie „dziecko powiedziało coś
  niepokojącego o domu" nie wylądowało na dole listy przez domyślną wagę zostawioną w pośpiechu.
- **Bez kluczy obcych** do grup, terminów i dzieci: sprawa ma przetrwać usunięcie grupy
  i anonimizację dziecka, bo to ona jest dowodem w razie sporu.
- Identyfikatory dzieci jako lista rozdzielona przecinkami, nie tabela łącząca. Incydent
  dotyczy najczęściej jednego–dwojga dzieci, a jedyne potrzebne zapytanie to „incydenty
  tego dziecka".

### Zgłoszenia techniczne — decyzje

- **Kluczowe jest powiązanie z dzieckiem, nie z terminem.** Wartość tego rejestru bierze się
  z historii („trzeci raz to samo"), a nie z pojedynczego zgłoszenia. Bez tego każde zajęcia
  zaczynają się od pytania „co ci znowu nie działa", bo nikt nie pamięta, że u tego dziecka
  Roblox nie instaluje się przez kontrolę rodzicielską.
- **Opis rozwiązania wymagany przy zamknięciu** — to jest cała wartość rejestru.
- **`CostLessonTime` jako osobne pole**, bo to ono decyduje o rozliczeniu: rozdział 7 traktuje
  awarię po stronie uczestnika i po stronie organizatora jako dwa różne przypadki, a jeden
  z nich uzasadnia kredyt.
- **Prowadzi je też instruktor**, nie tylko administracja: to on wie, co pomogło, i to jego
  następne zajęcia zależą od tego, czy problem został opisany.

### Do zrobienia zaraz

- [x] **Migracja wygenerowana** (`AddIncidentsAndSupportTickets`) — tym razem normalnie, przez
      narzędzie, bo powłoka wróciła.
- [x] `dotnet build` ✅
- [x] Interfejs — opisany niżej
- [ ] `dotnet test` i `npm test` po całości (użytkownik uruchamia na końcu etapu)

## Etap D p.15–16 — interfejs

### Gdzie co stoi

| Rola | Wejście | Co może |
| --- | --- | --- |
| Administrator | `/admin/safety`, menu „Bezpieczeństwo" | Oba rejestry, prowadzenie i zamykanie spraw |
| Instruktor | kokpit zajęć: „Zgłoś incydent" oraz „Zapisz problem" przy dziecku | Wyłącznie zgłaszanie |
| Rodzic | — | Nic. Rejestr incydentów nie istnieje z perspektywy portalu rodzica |

Instruktor **nie ma pozycji w menu**. To wynika wprost z decyzji backendu: prowadzenie sprawy
należy do administracji, a rejestr, w którym instruktor przegląda cudze zgłoszenia, byłby
rejestrem pustym. Zgłaszać musi móc jednak w trakcie zajęć, nie po powrocie do domu — stąd
oba wejścia w kokpicie.

### Trzy decyzje warte zapamiętania

- **Zapis problemu wisi przy znaczniku „problem techniczny", nie w osobnym miejscu.**
  Znacznik żyje tylko do końca zajęć i nie zostawia śladu; zgłoszenie zostaje. Przycisk stoi
  tuż obok znacznika, żeby ta różnica nie wymagała szukania — w trakcie zajęć nikt nie szuka.
- **Historia problemów dziecka pokazuje się w oknie zgłoszenia, nad formularzem** — a nie
  w rozwiniętym wierszu obecności, jak zakładał pierwszy plan. Powód: to jedyny moment,
  w którym instruktor jej realnie potrzebuje („to samo było w marcu, pomogło X"), a przy
  okazji to samo żądanie przynosi listę kategorii, więc nie ma osobnego zapytania o słownik.
  Ubocznie: szyna kokpitu zostaje komponentem prezentacyjnym, bez wywołań API.
- **Listy awaryjne kategorii i rodzajów** (`SUPPORT_CATEGORY_FALLBACK`, `INCIDENT_KIND_FALLBACK`
  w `types/safety.ts`). Okna otwierają się w trakcie zajęć; nieudane pobranie słownika nie może
  zostawić instruktora z pustą listą rozwijaną, a tym bardziej zablokować zgłoszenia incydentu.
  **Przy dopisaniu wartości do enuma w domenie trzeba uzupełnić też te listy.**

Historia ładuje się dopiero przy otwarciu okna — kokpit i tak startuje z kilkoma żądaniami
naraz, a zgłoszenie jest zdarzeniem rzadkim. Błąd jej pobrania jest świadomie cichy: to pomoc,
nie warunek zgłoszenia.

### Etap D — dlaczego nie ruszony (nieaktualne od 04.08.2026)

Wszystkie punkty etapu D (incydenty, zgłoszenia techniczne, test techniczny, zapasowe linki,
osobne zgody, eksport RODO) wymagają **nowych tabel albo kolumn**, czyli migracji z plikiem
`.Designer.cs`. Środowisko, w którym powstawały zmiany z 03.08, straciło dostęp do powłoki,
a plik Designer to pełny snapshot modelu — nie da się go sensownie napisać ręcznie.

Etap D zaczynamy po pierwszym zielonym przebiegu CI.

### Do zrobienia po tej zmianie

- [x] ~~`dotnet build`~~ — przechodzi bez błędów.
- [x] ~~`npm run build`~~ — `tsc` i `vite build` przechodzą.
- [x] ~~`npm test`~~ — **151/151**.
- [x] ~~Usunąć pusty plik migracji `AddNotificationTemplates`~~.
- [ ] `dotnet test` **po poprawkach** — pierwszy przebieg dał 205/208, trzy błędy naprawione,
      ale wynik po poprawce nie został jeszcze potwierdzony.
- [ ] Sprawdzić migrację `AddSessionDebriefAndLiveStatus` na kopii bazy: plik `.Designer.cs`
      i snapshot powstały ręcznie, bez `dotnet ef`. To ostatnia rzecz, której nie zweryfikował
      żaden automat.
- [ ] Przeklikać aplikację. Testy nie powiedzą, czy szyna nie zasłania scenariusza na małym
      ekranie, czy portal rodzica wygląda sensownie na telefonie i czy autozapis daje
      instruktorowi poczucie, że obecność jest zapisana.

## Zmiany z 13.08.2026 — przegląd pod kątem wdrożenia testowego

Przegląd przed wystawieniem aplikacji kilku osobom. Stan wyjściowy był zdrowszy, niż
sugerowała liczba niezacommitowanych plików: backend **223/223**, frontend **173/173**,
oba buildy zielone. Wyszły natomiast dwie rzeczy, które w takim teście dały o sobie znać
pierwszego dnia — obie na styku „kod działa u mnie” a „kod działa u kogoś innego”.

### Frontend nie istniał w świeżym klonie

W repozytorium był zapisany jako **gitlink** (tryb `160000`, commit `0e7091e`)
**bez pliku `.gitmodules`**. Taki wpis nie jest submodułem, tylko jego połową: `git clone`
tworzył pusty katalog `frontend/`, a `docker compose build` wywracał się na pierwszym
`COPY frontend/lesson-runner-web/package*.json`. Działało wyłącznie z jednej kopii roboczej
na jednym dysku.

Frontend jest teraz zwykłą częścią repozytorium (163 pliki). Historia zostaje na
`github.com/tommymil/szk-prg-front`, a kopia zapasowa `.git` wraz z łatką niezacommitowanej
pracy leży poza repozytorium, w `D:\moje\szk-prg-front-git-backup`.

Decyzje warte zapamiętania:

- **Monorepo zamiast dopisania `.gitmodules`.** Submoduł działa, ale przenosi problem gdzie
  indziej: wymaga `--recurse-submodules` przy klonowaniu i osobnego `push` do drugiego repo,
  więc ta sama pomyłka („zapomniałem wypchnąć frontend”) wraca przy każdym wdrożeniu.
- **W katalogu `lesson-runner-web` siedziało drugie, zapomniane repozytorium** — bez commitów
  i bez indeksu, ale z 885 KB luźnych obiektów po `git add`. Nazwanie go „pustym” na podstawie
  `git show-ref` było błędem; skasowanie w tej wierze byłoby nieodwracalne. Przeniesione do
  kopii, nie usunięte.

### Limit logowania działał na całą szkołę naraz

`UseForwardedHeaders` był włączony bez listy zaufanych sieci. Domyślnie ASP.NET Core ufa
**wyłącznie pętli zwrotnej**, a nginx w compose ma adres z sieci bridge — jego
`X-Forwarded-For` był więc po cichu odrzucany i `Connection.RemoteIpAddress` zostawał adresem
proxy dla **każdego** żądania. Polityka `auth` partycjonuje po tym adresie, więc limit
„10 nieudanych prób na 5 minut na IP” był w praktyce jednym limitem na całą instalację:
dziesięć pomyłek jednej osoby blokowało logowanie wszystkim pozostałym. Przy teście, w którym
kilka osób pierwszy raz ustawia hasła z zaproszeń, to kwestia pierwszej godziny.

Zaufane sieci są teraz podane wprost (`Security:TrustedProxyNetworks`, domyślnie pętla zwrotna
i zakresy prywatne), a budowanie opcji wyjechało z `Program.cs` do `ForwardedHeadersSetup`.

Decyzje warte zapamiętania:

- **Pusta lista nie jest rozwiązaniem.** Zaufanie wszystkim pozwala obejść limit logowania
  samym nagłówkiem — wystarczy podstawiać losowy adres przy każdej próbie. Błąd trzeba było
  naprawić w obie strony naraz.
- **Domyślnie całe zakresy prywatne, nie samo `172.16.0.0/12`.** Węższa lista wracałaby do tego
  samego cichego błędu przy niestandardowej sieci Dockera, a kontener API i tak nie jest
  wystawiany na zewnątrz. Zawężenie jest opisane w README, dla wdrożeń z proxy pod adresem
  publicznym.
- **Testy idą wprost przez `ForwardedHeadersMiddleware`, nie przez `ApiFactory`.** `TestServer`
  nie ustawia `Connection.RemoteIpAddress`, a middleware celowo przepuszcza pierwszy wpis, gdy
  adres połączenia jest `null` — test przez `WebApplicationFactory` przechodziłby **zawsze**,
  niezależnie od konfiguracji, czyli nie pilnowałby niczego.
- **Zestaw sprawdzony mutacją.** Po cofnięciu poprawki czerwienieją cztery z siedmiu testów.
  Trzy pozostałe przechodzą też w wersji błędnej i tak ma być — „niezaufane proxy jest
  ignorowane” oraz walidacja CIDR działają w obu wariantach.
- Przy okazji `KnownNetworks` → `KnownIPNetworks` (`ASPDEPR005`, .NET 10).

### Weryfikacja

- Backend **230/230** (7 nowych), frontend **173/173**, oba buildy bez błędów.
- Test świeżego klonu: `git clone` do osobnego katalogu, komplet ścieżek `COPY` z obu
  Dockerfile'ów obecny, `dotnet build` + `dotnet test` + `npm ci` + `npm run build` + `npm test`
  przechodzą **z klonu**. Build frontendu dał identyczne hasze artefaktów co z kopii roboczej.
- **Nie zweryfikowane:** `docker compose build`. Na maszynie, na której powstawał ten przegląd,
  Docker nie jest zainstalowany. Warstwa obrazów (nginx, aspnet, `npm ci` pod `node:24-alpine`)
  pozostaje niesprawdzona — to pierwsza rzecz do zrobienia na maszynie docelowej.

Jedno ostrzeżenie kompilatora zostaje: `CS8602` w `ProgressServiceTests.cs:161`, starsze niż
ta zmiana.

## Plan prac

Kolejność wynika z tego, ile realnego bólu operacyjnego usuwa dana rzecz, a nie z wielkości
kodu. Etapy 1–3 zamykają MVP z dokumentu koncepcyjnego.

### Etap A — domknięcie dziennika zajęć ✅ ZAMKNIĘTY 27.07.2026

Bez tego przy pierwszej reklamacji nie było się czym bronić. Cztery punkty zrobione;
`AuditLog` + `SessionChangeLog` + statusy obecności i zajęć dają komplet danych z rozdziału 12
dokumentu koncepcyjnego (historia terminów, obecności, log powiadomień, historia zmian danych).

1. ~~**Rozszerzone statusy obecności**~~ — ✅ **zrobione 27.07.2026.**
   `AttendanceStatus`: obecny, spóźniony, wyszedł wcześniej, uczestniczył częściowo, obecny ale
   nieaktywny, problemy techniczne, nieobecność zgłoszona, nieobecność niezgłoszona, odrabiał
   z inną grupą. Doszła notatka per dziecko per zajęcia oraz czas dołączenia i wyjścia.

   Decyzje warte zapamiętania:
   - `Present` **zostaje** jako właściwość wyliczana ze statusu (`CountsAsPresent()`), więc
     frekwencja, KPI i eksporty działają bez zmian, a kolumna `Present` w bazie jest
     zdenormalizowanym skrótem.
   - Do frekwencji liczymy każde stawienie się na zajęciach, także nieudane — problemy
     techniczne i bierna obecność to powód do rozmowy z rodzicem, nie do odbierania dziecku
     frekwencji. Odrabianie z inną grupą też liczymy, bo materiał został zrealizowany.
   - Powiadomienie o nieobecności idzie **tylko** przy statusie „niezgłoszona” — odsyłanie
     rodzicowi informacji o nieobecności, którą sam zgłosił, to szum.
   - Zapis częściowej listy nie kasuje już odhaczonych dzieci: wiersze nieprzysłane w żądaniu
     zostają nietknięte.
   - Słownik statusów przychodzi z backendu (`statusOptions`), więc frontend nie utrzymuje
     własnej kopii etykiet.
   - Migracja `AddAttendanceStatus` wypełnia status istniejących wierszy na podstawie `Present`;
     repozytorium ma ten sam fallback przy odczycie.
2. ~~**Historia zmian terminu**~~ — ✅ **zrobione 27.07.2026.**
   `SessionChangeLog` zapisuje: poprzedni i nowy termin, rodzaj zmiany (dodano / przełożono /
   odwołano / zastępstwo / linki), powód, autora, moment zmiany i to, czy poinformowano opiekunów.
   Przy przekładaniu i odwoływaniu doszło pole „powód” oraz znacznik powiadomienia; w szczegółach
   grupy jest sekcja „Historia zmian terminów” (`GET /api/groups/{id}/sessions/history`).

   Decyzje warte zapamiętania:
   - Tabela **nie ma klucza obcego** do `ScheduledSessions` ani `Groups` — historia ma przetrwać
     usunięcie terminu albo grupy, bo właśnie wtedy bywa najbardziej potrzebna.
   - Wpisy są tylko do dopisywania; nie edytujemy ich ani nie kasujemy.
   - Historia ładuje się na żądanie, a nie razem z grupą — to widok kontrolny, otwierany rzadko.
   - Znacznik „powiadomiono opiekunów” jest na razie zaznaczany ręcznie przez osobę robiącą
     zmianę. Po wpięciu powiadomień o zmianie terminu (etap C) ma się wypełniać automatycznie.
3. ~~**Rozszerzone statusy zajęć**~~ — ✅ **zrobione 27.07.2026.**
   Doszły: potwierdzone, odwołane przez instruktora, odwołane przez rodzica, przerwane
   technicznie, niezrealizowane, czeka na nowy termin. Nowy endpoint
   `PUT /api/groups/{id}/sessions/{sid}/status` z powodem, a przy odwołaniu wskazujemy stronę
   (`cancelledBy: instructor|parent`). Każda zmiana statusu trafia do historii jako
   „Zaplanowane → Przerwane technicznie”.

   Decyzje warte zapamiętania:
   - **Nie ma statusu „przełożone”.** W tym modelu przełożenie przesuwa ten sam rekord, więc po
     zmianie termin nadal jest zaplanowany, tylko na inną datę. Fakt przełożenia z poprzednią
     datą, powodem i autorem trzyma `SessionChangeLog` — status byłby kłamstwem o przyszłych zajęciach.
   - Rozsiane po kodzie porównania `== Cancelled` zastąpione predykatami domenowymi
     (`IsUpcoming`, `IsActive`, `IsCancelled`, `CountsAsHeld`, `IsClosed`). Dodanie kolejnego
     statusu nie wymaga już polowania na porównania w dziesięciu plikach.
   - **Do frekwencji liczy się tylko `Completed`.** Awaria techniczna po naszej stronie ani
     zajęcia niezrealizowane nie obniżają frekwencji dziecka.
   - `InProgress` można ustawić wyłącznie przez rozpoczęcie zajęć w kokpicie — ręczne wpisanie
     rozjechałoby timery i listę obecności, więc endpoint to odrzuca.
   - Etykiety statusów mieszkają w domenie; zniknęły dwie kopie słownika (`GroupMapping`
     i `ParentPortalService`), a frontend dostaje listę statusów z backendu.
4. ~~**Audyt na operacjach domenowych**~~ — ✅ **zrobione 27.07.2026.**
   Dziennik obejmuje teraz grupy, terminy, obecność, uczestników, konspekty, kursy, płatności,
   powiadomienia, lokalizacje, dni wolne i operacje — nie tylko logowanie i konta.
   W panelu Operacje doszły filtry: obszar, fragment nazwy akcji, „tylko nieudane”.

   Decyzje warte zapamiętania:
   - Zamiast dopisywać wywołanie `IAuditService` do każdego endpointu, audytuje **filtr
     `AuditEndpointFilter` przypięty do grup tras**. Ręczne wołanie przy trzydziestu
     endpointach nie skaluje się — ktoś zawsze zapomni, a brak wpisu wychodzi dopiero przy
     reklamacji. Filtr działa odwrotnie: domyślnie audytuje wszystko, co zmienia stan,
     a wyjątki trzeba zaznaczyć świadomie przez `.SelfAudited()`.
   - **Nie zapisujemy treści żądania.** Poszłyby tam hasła (`POST /api/users/{id}/password`,
     `POST /api/auth/change-password`). W dzienniku ląduje metoda, ścieżka i kod odpowiedzi.
     Pokrywa to test regresyjny.
   - Odczyty (GET) nie są audytowane — zaśmiecałyby dziennik, nie zmieniając stanu.
   - Jako nazwa akcji służy nazwa endpointu (`CancelSession`), a nie wzorzec trasy.
   - Nieudane żądania też trafiają do dziennika — przy sporze liczy się również to,
     czego ktoś próbował, a co się nie udało.

### Etap B — rozliczenia, które nie kłamią

5. ~~**Kredyty zajęciowe**~~ — ✅ **zrobione 27.07.2026.**
   `LessonCredit`: przyznanie z powodem i autorem, powiązanie z uczestnikiem, grupą i konkretnym
   odwołanym terminem, termin ważności, wykorzystanie (odrabianie / pomniejszenie płatności /
   zajęcia dodatkowe / przedłużenie kursu) oraz wycofanie. Endpointy pod `/api/billing/credits`,
   sekcja „Kredyty zajęciowe” w panelu Płatności, nowe KPI „kredyty do wykorzystania”.
6. ~~**Rozdzielenie terminu od rozliczenia**~~ — ✅ **zrobione 27.07.2026.**
   `CancelSessionDto.Compensation` (`none` / `credit` / `makeup` / `refund`) jest **osobną
   decyzją** od samego odwołania. Przy `credit` system przyznaje kredyt każdemu aktywnie
   zapisanemu dziecku, z powodem odwołania i wskazaniem terminu źródłowego.

   Decyzje warte zapamiętania:
   - Jeden kredyt = jedne zajęcia. `AmountCents` wypełniamy tylko wtedy, gdy kredyt ma
     pomniejszyć fakturę i znamy jego wartość pieniężną.
   - **Powód przyznania jest wymagany** — kredyt bez powodu jest nie do obronienia przy
     rozmowie z rodzicem ani przy kontroli.
   - Automatyzujemy wyłącznie `credit`. Zwrot i odrobienie wymagają rozmowy z rodzicem
     i wpisuje się je ręcznie — udawanie automatyzacji byłoby tu gorsze niż jej brak.
   - Kredyty po terminie ważności przestawiamy **przy odczycie** (nie ma workera);
     przeterminowany kredyt pokazywany jako „do wykorzystania” wprowadzałby w błąd.
   - Wykorzystanego kredytu nie da się wycofać ani wykorzystać drugi raz.
   - Tabela bez kluczy obcych do uczestników, grup i terminów — historia rozliczeń ma przetrwać
     usunięcie grupy.

8. ~~**„Odwołaj zajęcia” jednym ruchem**~~ — ✅ **zrobione 27.07.2026.**
   Jedno żądanie robi komplet z rozdziału 10: odwołanie + powód + kto odwołał + znacznik
   powiadomienia + rekompensata + przesunięcie materiału.

   `ShiftFollowingLessons` przesuwa **przypisanie lekcji, nie kalendarz**: materiał z odwołanych
   zajęć wchodzi na najbliższy termin, każdy kolejny przesuwa się o jeden, a lekcja wypchnięta
   z końca dostaje nowy termin tydzień po ostatnim. Daty istniejących terminów zostają nietknięte,
   więc rodzice nie muszą nic przestawiać w swoim tygodniu, a materiał zostaje zrealizowany
   w całości. Kurs wydłuża się o jedne zajęcia. Domyślnie wyłączone — czasem zajęcia po prostu
   przepadają i tak ma być.

**Zostało w etapie B:**

7. **Pakiety zajęć** — N zajęć zamiast wyłącznie abonamentu miesięcznego, zawieszenie,
   dołączenie i rezygnacja w połowie okresu, rabat rodzeństwa. Odłożone świadomie: ma sens
   dopiero, gdy zaczniesz realnie rozliczać rodziców w innym modelu niż miesięczny.

### Etap C — to, o co pyta rodzic *(zamknięty 03.08.2026, poza „końcem pakietu”)*

9. ~~**Reset hasła i zaproszenia dla rodziców**~~ — ✅ **zrobione 28.07.2026.** *(punkt 8 audytu)*
   Tabela `AccountTokens` (migracja `AddAccountTokens`), trasy publiczne
   `POST /api/auth/password-reset`, `GET /api/auth/password-reset/{token}`,
   `POST /api/auth/password-reset/confirm` oraz `POST /api/users/{id}/invite` dla admina.
   We froncie: „Nie pamiętam hasła” na logowaniu, ekrany `/reset-password` i `/set-password`,
   przycisk „Wyślij zaproszenie” przy koncie.

   Decyzje warte zapamiętania:
   - **Żądanie resetu nigdy nie zdradza, czy konto istnieje.** Zawsze `202`, ten sam ekran
     i ta sama treść odpowiedzi. Inaczej formularz „nie pamiętam hasła” byłby wygodnym
     sprawdzaczem, którzy rodzice korzystają ze szkoły — a to dane o dzieciach.
   - **W bazie leży wyłącznie skrót SHA-256 tokenu.** Postać jawna istnieje tylko w wysłanym
     e-mailu. Wyciek kopii bazy nie może oznaczać przejęcia konta, a przy koncie admina
     oznaczałby oddanie całego systemu.
   - **Konto można założyć bez hasła.** Puste pole w formularzu = konto z zaproszeniem: hasło
     ustawia sam użytkownik i nikt poza nim go nie zna. Nie ma osobnego stanu „brak hasła” —
     konto dostaje skrót z losowego ciągu — bo taki stan prędzej czy później ktoś potraktowałby
     jako „hasło się zgadza”.
   - **Wydanie nowego tokenu unieważnia poprzednie**, a udana zmiana hasła unieważnia resztę.
     W skrzynce może leżeć kilka wiadomości; działa najnowsza.
   - **Wyłączone konto nie dostaje linku.** Reset hasła nie może być obejściem dezaktywacji.
   - **Te e-maile omijają przełączniki powiadomień i warunek zgody RODO.** To poczta o dostępie
     do konta, nie informacja o zajęciach — wyłączenie przypomnień nie może odbierać rodzicowi
     możliwości zalogowania się. Trafiają natomiast do dziennika wysyłek (`password-reset`,
     `invitation`), z kluczem deduplikacji zawierającym identyfikator tokenu, żeby żadne
     żądanie resetu nie wypadło przez deduplikację.
   - **Treść wiadomości jest wbudowana, nie edytowalna w panelu.** Szablon zepsuty literówką
     oznaczałby, że nikt nie odzyska hasła; z ustawień powiadomień bierzemy tylko nadawcę.
   - Dwa limity zamiast jednego: `429` per adres IP (istniejąca polityka `auth`) **i** pięć
     tokenów na godzinę per konto — inaczej z wielu adresów dałoby się zasypać jedną skrzynkę.
   - Ważność: 2 h dla resetu, 7 dni dla zaproszenia. Reset to akcja w toku, zaproszenie czeka,
     aż rodzic zajrzy do skrzynki.
   - **Adres linku pochodzi z konfiguracji (`App:PublicOrigin`), nie z nagłówka `Host`.**
     Host podlega podmianie przez klienta, więc link w mailu prowadziłby pod adres atakującego.
10. ~~**Zgłaszanie nieobecności przez rodzica**~~ — ✅ **zrobione 27.07.2026.**
    Przy każdym nadchodzącym terminie w portalu jest przycisk „Zgłoś nieobecność” (z opcjonalnym
    powodem), a instruktor widzi to od razu na liście obecności jako status „nieobecność
    zgłoszona”. Domyka pętlę z etapu A1: status istniał, ale nic nie mogło go ustawić od strony
    rodzica. `POST /api/parent/sessions/{id}/absence`.

    Decyzje warte zapamiętania:
    - Powiązanie opiekun–dziecko jest **jedynym dowodem uprawnienia** — rodzic nie zgłosi
      nieobecności cudzego dziecka.
    - Nie da się zgłosić nieobecności na termin, który już się odbył: wtedy liczy się to,
      co odhaczył instruktor, a nie deklaracja rodzica.
    - Odmowa zwraca **404, nie 403** — rodzic nie ma prawa wiedzieć, czy termin cudzego dziecka
      w ogóle istnieje.
    - Skutek uboczny z A1: taka nieobecność **nie generuje** e-maila „dziecko było nieobecne”,
      bo powiadamiamy tylko o nieobecności niezgłoszonej.
11. ~~**Postępy dziecka**~~ — ✅ **zrobione 28.07.2026.**
    `ProgressEntry` ze skalą samodzielności (wymaga pełnej pomocy → potrafi rozbudować
    rozwiązanie), znacznikiem ukończenia materiału, notatką dla rodzica i kolejnym krokiem.
    Wpisuje się je w kokpicie prowadzenia (przycisk „Postępy”), a rodzic widzi je w portalu.
12. ~~**Projekty dzieci**~~ — ✅ **zrobione 28.07.2026.**
    `Project` / `ProjectSubmission`: link albo plik, kolejne wersje, komentarz instruktora.
    Migracja `AddProgressAndProjects`, trasy pod `/api/progress` (`StaffOnly`) i pobieranie
    plików przez `/download/project-files/{token}`.

    Decyzje warte zapamiętania (11 i 12 razem):
    - **Skala nie mierzy tego, czy projekt działa.** Dziecko, które skopiowało gotowe
      rozwiązanie, ma działający projekt i zerowe zrozumienie; dziecko, które samo znalazło błąd,
      nauczyło się więcej. Stąd pięć poziomów samodzielności z rozdziału 11, a nie ocena.
    - **Notatka jest jedna i z definicji widoczna dla rodzica.** Świadomie nie ma drugiego,
      „wewnętrznego” pola przy wpisie: notatka, o której trzeba pamiętać, że jej nie widać,
      prędzej czy później zostanie pokazana. Uwagi dla zespołu zostają w notatce terminu, która
      do portalu nie trafia w ogóle. Formularz w kokpicie mówi to wprost.
    - **Ukończenie materiału jest osobne od samodzielności** — można skończyć z pomocą i nie
      skończyć samodzielnie. Jedna liczba zlepiłaby dwie różne informacje.
    - **Jeden wpis na dziecko na termin** (indeks unikalny). Kolejny zapis z kokpitu poprawia
      ten sam wpis; inaczej po trzech poprawkach nie wiadomo, która jest aktualna.
    - **Wpis można zrobić wyłącznie dziecku zapisanemu do tej grupy.** Bez tego jeden literówkowy
      identyfikator dopisałby postęp cudzemu dziecku — i rodzic zobaczyłby go u siebie.
    - **Wersje projektu są dopisywane, nigdy nadpisywane.** Reklamacja „projekt dziecka zniknął”
      prawie zawsze znaczy „został zastąpiony gorszą wersją”. Numer wersji liczymy po najwyższej
      istniejącej, nie po liczbie wpisów.
    - **Wersja to link albo plik, nigdy oba naraz** — przy obu nie wiadomo, co jest tą wersją.
    - Zapis jest związany z terminem i sprawdza właściciela (instruktor grupy albo zastępstwo),
      tak samo jak `SessionService`. Odczyt dorobku dziecka: **admin widzi każde dziecko**
      (to on odpowiada na reklamacje), instruktor wyłącznie dzieci ze swoich grup. Odmowa to
      **404, nie 403** — instruktor spoza grupy nie ma prawa wiedzieć, czy takie dziecko istnieje.
    - Pliki wersji pobiera się przez nieodgadywalny klucz w adresie, tak jak pliki lekcji: link
      do pobrania nie niesie nagłówka `Authorization`, więc uprawnienie musi siedzieć w adresie.
    - Tabele bez kluczy obcych do `Participants`, `Groups` i `ScheduledSessions` — dorobek
      dziecka ma przetrwać usunięcie grupy albo terminu. Jedyny klucz obcy to wersja → projekt.
    - **Świadoma luka:** UI instruktora obsługuje na razie wpisy postępów; projekty i ich wersje
      zakłada się przez API. Ekran do zarządzania projektami jest do dorobienia — API i portal
      rodzica są gotowe.
13. ~~**Pozostałe typy powiadomień**~~ — ✅ **zrobione 03.08.2026.** Zmiana terminu,
    odwołanie zajęć, podsumowanie zajęć i przypomnienie o płatności. Szczegóły i decyzje
    w sekcji „Zmiany z 03.08.2026, część druga”. Zmienna `{{link}}` działała już od 27.07.

    **Zostaje:** „koniec pakietu” — czeka na pakiety zajęć (etap B p.7), bo bez nich nie ma
    czego liczyć.
14. ~~**Eksport kalendarza do ICS**~~ — ✅ **zrobione 27.07.2026.**
    Przycisk „Dodaj do kalendarza” w grafiku instruktora i w portalu rodzica
    (`GET /api/schedule/export.ics`, `GET /api/parent/export.ics`).

    Decyzje warte zapamiętania:
    - **Plik do pobrania, nie adres subskrypcji.** Subskrypcja wymaga URL-a, który aplikacja
      kalendarza otwiera bez nagłówka `Authorization` — czyli osobnego, długożyjącego tokenu
      w adresie. To osobna decyzja bezpieczeństwa i osobna migracja; na razie użytkownik
      pobiera plik i importuje go u siebie.
    - Odwołane terminy zostają w pliku ze `STATUS:CANCELLED`, żeby przy ponownym imporcie
      znikały z kalendarza zamiast zostać tam jako duchy.
    - Czas trwania to stała 95 minut (45 + 5 przerwy + 45) — sesje nie niosą własnego czasu.
      Wpis w kalendarzu obejmuje przerwę, bo blokuje się czas od wejścia do wyjścia.
    - Zwijanie linii po 75 oktetach wg RFC 5545, bez rozcinania znaków wielobajtowych
      (inaczej polskie znaki w nazwach grup rozsypywały import w części kalendarzy).

### Etap D — bezpieczeństwo i obsługa wyjątków

15. ~~**Moduł incydentów** (`Incident`)~~ — ✅ **zrobione 04.08.2026**. Rejestr osobny od uwag
    edukacyjnych, poza zasięgiem portalu rodzica; prowadzenie spraw tylko dla administracji,
    zgłaszanie dla całego personelu (także z kokpitu w trakcie zajęć).
16. ~~**Zgłoszenia techniczne** (`SupportTicket`) i historia problemów przy dziecku~~ —
    ✅ **zrobione 04.08.2026**. Zgłoszenie jednym kliknięciem przy znaczniku „problem
    techniczny", historia dziecka nad formularzem zgłoszenia.
17. **Test techniczny przed pierwszymi zajęciami** — checklista: mikrofon, kamera, przeglądarka,
    wymagany program, pobranie pliku testowego.
18. **Zapasowy link, instrukcja dołączenia, lista wymaganych programów, zadanie awaryjne**
    na poziomie grupy i terminu.
19. **Osobne zgody**: na nagrywanie i na publikację prac dziecka.
20. **RODO** — eksport danych dziecka na żądanie (art. 15/20) i polityka retencji.

### Etap E — dojrzałość i skala

21. ~~**Ujednolicenie modelu opiekuna**~~ — ✅ **zrobione 27.07.2026** (wyjęte przed etap C,
    bo C dokłada kolejne rzeczy oparte na opiekunie).

    `ParentParticipantLink` przestał być samym powiązaniem i niesie teraz dane: `Relation`
    („mama”, „tata”, „opiekun prawny”), `IsPrimaryContact` i `ReceivesNotifications`.
    Powstał `GuardianDirectory` — **jedno miejsce odpowiadające na pytanie „do kogo napisać
    w sprawie tego dziecka”**.

    Decyzje warte zapamiętania:
    - Zasada rozstrzygania jest bezwarunkowa: **są powiązane konta opiekunów → piszemy do nich
      i tylko do nich; nie ma żadnego → wracamy do danych wpisanych przy dziecku.** Wcześniej
      istniały dwa równoległe światy — powiadomienia szły na `Participant.GuardianEmail`,
      a portal działał na kontach; przy dwojgu opiekunów jedno dostawałoby e-maile, a drugie
      widziało portal.
    - **Kolumn `Participant.Guardian*` nie skasowaliśmy.** Zostają jako zapas dla rodzin, które
      nie mają jeszcze konta — usunięcie ich oznaczałoby utratę kontaktu do wszystkich takich
      rodzin i wymuszałoby zakładanie kont, zanim istnieje przepływ zaproszeń (etap C).
    - Deduplikacja powiadomień idzie teraz po **adresie**, nie po dziecku: przy dwojgu opiekunów
      każde ma dostać swoją wiadomość, ale dokładnie jedną.
    - Powiązanie z wyłączonym kontem opiekuna **nie** powoduje powrotu do danych przy dziecku —
      konto jest źródłem prawdy, a wyłączone konto znaczy, że tego adresu świadomie nie używamy.
    - Kontakt pierwszego wyboru jest jeden na dziecko; ustawienie go zdejmuje flagę pozostałym.
    - Zgoda RODO pozostaje warunkiem wstępnym niezależnie od źródła danych.
22. **Historia członkostwa w grupie** — data dołączenia i odejścia, powód, przenoszenie
    między grupami bez gubienia przeszłości.
23. **Poziomy zaawansowania** grup i dzieci, materiał wymagany przed dołączeniem.
24. **Rozbudowa lekcji** — zadanie prostsze, dodatkowe, awaryjne, cel podstawowy, oczekiwany efekt.
25. **Panel prowadzenia na żywo** — kto potrzebuje pomocy, kto skończył, kto ma problem techniczny.
26. **Wydajność** — kilka miejsc ładuje całe tabele i filtruje w pamięci
    (`ParentPortalService`, `SessionService.MakeupSessionOptionsAsync`, `GroupService.GetSummariesAsync`).
    Niewidoczne przy kilkunastu grupach, odczuwalne przy pięćdziesięciu.
27. **CI** — `dotnet test` i `npm test` na każdy push.
28. **PostgreSQL** — własny zestaw migracji, jeśli SQLite przestanie wystarczać.

### Świadomie odłożone

Automatyczne tworzenie spotkań przez API platformy wideo, certyfikaty, grywalizacja,
raporty rentowności, ankiety satysfakcji, aplikacja mobilna, automatyczne wykrywanie dzieci
wymagających zmiany poziomu (rozdział 15 dokumentu). Wszystko to ma sens dopiero, gdy
etapy A–C działają na realnych zajęciach.

## Przed uruchomieniem produkcyjnym

- [ ] `docker compose build` na maszynie docelowej — jedyna warstwa, której nie pokrywa
      ani CI, ani test świeżego klonu z 13.08.
- [ ] `Security:TrustedProxyNetworks` zawężone, jeśli reverse proxy stoi pod adresem
      publicznym. Przy proxy spoza listy wszyscy użytkownicy dzielą jeden limit logowania.
- [ ] `JWT_SIGNING_KEY` z generatora (`openssl rand -base64 48`), nie z przykładu.
- [ ] Zmiana hasła konta bootstrap zaraz po pierwszym zalogowaniu.
- [ ] `SMTP_MODE=Smtp` dopiero po sprawdzeniu szablonów w trybie `Log`.
- [ ] Backup uruchamiany cyklicznie i **odtworzony testowo** — kopia bez próby odtworzenia
      to nie jest kopia zapasowa.
- [ ] Polityka prywatności uwzględniająca, że pliki z `/uploads` są dostępne pod adresem URL
      bez dodatkowej autoryzacji (nazwy są losowymi GUID-ami).
- [ ] HTTPS na reverse proxy przed aplikacją.
