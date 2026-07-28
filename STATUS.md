# Lesson Runner — aktualny stan projektu

Ostatnia aktualizacja: 27.07.2026

Ten plik opisuje **stan bieżący** i plan prac. Powiązane dokumenty:

- [`system_zajec_online_programowanie_dla_dzieci.md`](system_zajec_online_programowanie_dla_dzieci.md)
  — docelowy zakres systemu i katalog sytuacji, które musi obsłużyć. **Źródło wymagań.**
- [`AUDYT_2026-07-27.md`](AUDYT_2026-07-27.md) — ostatni audyt kodu.
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
- Uwierzytelnianie: JWT (12 h) ze znacznikiem sesji — dezaktywacja konta i zmiana hasła
  unieważniają token natychmiast.
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
   „Zdarzenia że Sterowaniem" zamiast „ze", „skroc pokaż do 10 s" zamiast „pokaz",
   „postaći", „odpowiedźi", `zadańia` w kodzie Pythona dla dzieci. Dodany test
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
   po zakończeniu zajęć. Endpoint `PUT /api/groups/{id}/sessions/{sid}/links`, formularz „Linki"
   przy terminie w szczegółach grupy. Migracja `AddSessionMeetingAndRecordingUrl`.
10. **Materiały dla rodzica.** Portal pokazuje sekcję „Materiały po zajęciach": pliki projektu
    lekcji (starter / wersja końcowa) i nagranie — **wyłącznie z terminów o statusie
    `Completed`**. Świadomie nie udostępniamy scenariusza prowadzenia ani notatek instruktora;
    pobieranie idzie przez `/download/lesson-files/{token}` (adres-klucz z nieodgadywalnym tokenem).

## Analiza luk wobec `system_zajec_online_programowanie_dla_dzieci.md`

Zestawienie wymagań z dokumentu koncepcyjnego (27.07.2026) ze stanem faktycznym kodu.
Legenda: ✅ jest · ⚠️ jest częściowo · ❌ brak.

**Dobra wiadomość na start:** najważniejsza zasada projektowa z rozdziału 16 — „zajęcia to nie
jest tylko wydarzenie w kalendarzu" — jest już spełniona. `ScheduledSession` to osobny rekord
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
| Notatka per dziecko per zajęcia („problemy z mikrofonem", „wyszedł po 40 min") | ❌ notatka jest tylko zbiorcza na terminie |
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

### 5. Struktura 90 minut

Timery lekcji i kroku, lista etapów, odhaczanie czynności, osobne okno przerwy — ✅ to działa
i jest mocną stroną kokpitu. Brakuje ❌ przypomnienia o przerwie wyliczanego z planu oraz
❌ zapisu „czego nie zdążyliśmy" w formie strukturalnej (dziś tylko wolna notatka).

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
Brakuje ❌ operacji „jednym ruchem": odwołaj + powód + powiadom rodziców + przyznaj kredyt +
przesuń numerację kolejnych lekcji. Dziś to cztery osobne kliknięcia i część z nich nie istnieje.

### 11. Postępy i projekty dzieci

Cały rozdział to **❌ biała plama**. Brak `Project`, `ProjectSubmission`, `ProgressEntry`,
skali samodzielności, ukończonych umiejętności i podsumowań okresowych. To jest jednocześnie
odpowiedź na najczęstsze pytanie rodzica („czego dziecko się nauczyło?") i na reklamację
„moje dziecko niczego się nie nauczyło".

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

1. Konta opiekunów i dzieci ✅ · 2. Grupy i przypisywanie ✅ · 3. Kalendarz cykliczny ze zmianą
pojedynczych zajęć ✅ · 4. Link do spotkania ✅ · 5. Automatyczne przypomnienia ✅ ·
6. Obecność i spóźnienia ✅ *(etap A1)* · 7. Odwołanie / przełożenie / odrabianie ⚠️
(powody i statusy są, brakuje kredytów) · 8. Notatka po zajęciach ✅ · 9. Scenariusze i materiały ✅ ·
10. Projekty dzieci ❌ · 11. Płatności, **kredyty** i rozliczenia ⚠️ ·
12. Historia komunikacji ⚠️ (log wysyłek, bez wątku) · 13. Zgody i uprawnienia ✅ ·
14. Rejestr problemów technicznych i incydentów ❌.

**Podsumowanie: 9 z 14 punktów MVP zamknięte, 3 częściowo, 2 nietknięte.**
(Przed etapem A było 8 / 4 / 2.)

---

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
   - Powiadomienie o nieobecności idzie **tylko** przy statusie „niezgłoszona" — odsyłanie
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
   Przy przekładaniu i odwoływaniu doszło pole „powód" oraz znacznik powiadomienia; w szczegółach
   grupy jest sekcja „Historia zmian terminów" (`GET /api/groups/{id}/sessions/history`).

   Decyzje warte zapamiętania:
   - Tabela **nie ma klucza obcego** do `ScheduledSessions` ani `Groups` — historia ma przetrwać
     usunięcie terminu albo grupy, bo właśnie wtedy bywa najbardziej potrzebna.
   - Wpisy są tylko do dopisywania; nie edytujemy ich ani nie kasujemy.
   - Historia ładuje się na żądanie, a nie razem z grupą — to widok kontrolny, otwierany rzadko.
   - Znacznik „powiadomiono opiekunów" jest na razie zaznaczany ręcznie przez osobę robiącą
     zmianę. Po wpięciu powiadomień o zmianie terminu (etap C) ma się wypełniać automatycznie.
3. ~~**Rozszerzone statusy zajęć**~~ — ✅ **zrobione 27.07.2026.**
   Doszły: potwierdzone, odwołane przez instruktora, odwołane przez rodzica, przerwane
   technicznie, niezrealizowane, czeka na nowy termin. Nowy endpoint
   `PUT /api/groups/{id}/sessions/{sid}/status` z powodem, a przy odwołaniu wskazujemy stronę
   (`cancelledBy: instructor|parent`). Każda zmiana statusu trafia do historii jako
   „Zaplanowane → Przerwane technicznie".

   Decyzje warte zapamiętania:
   - **Nie ma statusu „przełożone".** W tym modelu przełożenie przesuwa ten sam rekord, więc po
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
   W panelu Operacje doszły filtry: obszar, fragment nazwy akcji, „tylko nieudane".

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
   sekcja „Kredyty zajęciowe" w panelu Płatności, nowe KPI „kredyty do wykorzystania".
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
     przeterminowany kredyt pokazywany jako „do wykorzystania" wprowadzałby w błąd.
   - Wykorzystanego kredytu nie da się wycofać ani wykorzystać drugi raz.
   - Tabela bez kluczy obcych do uczestników, grup i terminów — historia rozliczeń ma przetrwać
     usunięcie grupy.

**Zostało w etapie B:**

7. **Pakiety zajęć** — N zajęć zamiast wyłącznie abonamentu miesięcznego, zawieszenie,
   dołączenie i rezygnacja w połowie okresu, rabat rodzeństwa.
8. **Przesunięcie numeracji kolejnych terminów** przy odwołaniu (rozdział 10 dokumentu).
   Reszta operacji „jednym ruchem" — powód, powiadomienie, kredyt — działa już w jednym żądaniu.

### Etap C — to, o co pyta rodzic

9. **Reset hasła i zaproszenia dla rodziców** — token jednorazowy na e-mail; dziś każde hasło
   ustawia ręcznie admin, co przy trzydziestu rodzicach jest etatem. *(punkt 8 audytu)*
10. **Zgłaszanie nieobecności przez rodzica** z portalu, widoczne na liście obecności instruktora.
11. **Postępy dziecka** — `ProgressEntry` ze skalą samodzielności (wymaga pełnej pomocy →
    potrafi rozbudować rozwiązanie), ukończone lekcje, notatka widoczna dla rodzica.
12. **Projekty dzieci** — `Project` / `ProjectSubmission`: link albo plik, wersje, komentarz
    instruktora. Odpowiedź na „projekt dziecka zniknął".
13. **Pozostałe typy powiadomień** — zmiana terminu, podsumowanie zajęć, przypomnienie
    o płatności, koniec pakietu. Plus `{{link}}` w szablonie przypomnienia.
14. **Eksport kalendarza do ICS** dla rodzica i instruktora. *(punkt 9 audytu)*

### Etap D — bezpieczeństwo i obsługa wyjątków

15. **Moduł incydentów** (`Incident`) — data, osoby, opis, działania, status, osoba odpowiedzialna.
    Trzymany osobno od uwag edukacyjnych, z węższym dostępem.
16. **Zgłoszenia techniczne** (`SupportTicket`) i historia problemów przy dziecku.
17. **Test techniczny przed pierwszymi zajęciami** — checklista: mikrofon, kamera, przeglądarka,
    wymagany program, pobranie pliku testowego.
18. **Zapasowy link, instrukcja dołączenia, lista wymaganych programów, zadanie awaryjne**
    na poziomie grupy i terminu.
19. **Osobne zgody**: na nagrywanie i na publikację prac dziecka.
20. **RODO** — eksport danych dziecka na żądanie (art. 15/20) i polityka retencji.

### Etap E — dojrzałość i skala

21. **Ujednolicenie modelu opiekuna** — koniec z `Participant.Guardian*` obok konta rodzica;
    jedno źródło prawdy i obsługa dwojga opiekunów.
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

- [ ] `JWT_SIGNING_KEY` z generatora (`openssl rand -base64 48`), nie z przykładu.
- [ ] Zmiana hasła konta bootstrap zaraz po pierwszym zalogowaniu.
- [ ] `SMTP_MODE=Smtp` dopiero po sprawdzeniu szablonów w trybie `Log`.
- [ ] Backup uruchamiany cyklicznie i **odtworzony testowo** — kopia bez próby odtworzenia
      to nie jest kopia zapasowa.
- [ ] Polityka prywatności uwzględniająca, że pliki z `/uploads` są dostępne pod adresem URL
      bez dodatkowej autoryzacji (nazwy są losowymi GUID-ami).
- [ ] HTTPS na reverse proxy przed aplikacją.
