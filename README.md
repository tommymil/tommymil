# Lesson Runner

Aplikacja do prowadzenia zajęć programowania dla dzieci: administrator tworzy konspekty i układa
grupy, instruktor prowadzi zajęcia z kokpitu, rodzic ma własny portal z harmonogramem, linkiem do
zajęć online, frekwencją i rozliczeniami.

- **Backend**: C# / ASP.NET Core (Clean Architecture, EF Core + SQLite).
- **Frontend**: React + Vite + TypeScript (podział MVVM-like).

Aktualny stan i plany: [`STATUS.md`](STATUS.md). Historia budowy: [`PLAN.md`](PLAN.md).
Ostatni audyt: [`AUDYT_2026-07-27.md`](AUDYT_2026-07-27.md).

## Wymagania

- .NET SDK 10.0+
- Node.js 20+ (testowane na 22) i npm
- Narzędzia EF Core: `dotnet tool install --global dotnet-ef` (do generowania migracji)

## Uruchomienie lokalne (development)

Dwa terminale.

```bash
# terminal 1 - backend
cd backend
dotnet run --project LessonRunner.Api
```

```bash
# terminal 2 - frontend
cd frontend/lesson-runner-web
npm install   # tylko za pierwszym razem
npm run dev
```

- API: `http://localhost:5000`, aplikacja: `http://localhost:5173`.
- Baza SQLite (`lesson-runner.db`) tworzy się automatycznie przez migracje przy starcie
  i zostaje zasilona danymi demonstracyjnymi.
- Swagger/OpenAPI (tylko Development): `http://localhost:5000/openapi/v1.json`.
- Adres API konfiguruje `VITE_API_BASE_URL` (patrz `.env.development` / `.env.example`).

Konta startowe — **tylko w środowisku Development**, na produkcji nie powstają:

- `admin@lessonrunner.local` / `admin12345` (Admin)
- `instructor@lessonrunner.local` / `teacher12345` (Instructor)
- `parent@lessonrunner.local` / `parent12345` (Parent) — powiązany z Zofią Kowalską i Janem
  Kowalskim, więc portal rodzica ma co pokazać (w tym przełącznik dziecka)

## Uruchomienie przez Docker

```bash
cp .env.example .env     # uzupełnij sekrety - bez nich compose celowo nie wstanie
docker compose up --build
```

- Aplikacja: `http://localhost:8080` (nginx serwuje frontend i proxuje `/api`, `/uploads`
  oraz `/download` do backendu — dzięki temu nie trzeba konfigurować CORS).
- Pierwszy start zakłada konto administratora z `BOOTSTRAP_ADMIN_EMAIL` / `BOOTSTRAP_ADMIN_PASSWORD`.
  **Zmień to hasło zaraz po pierwszym zalogowaniu.**
- Dane trwałe siedzą w wolumenach: `api-data` (baza), `api-uploads` (pliki), `api-backups` (kopie).

HTTPS zapewnia reverse proxy przed aplikacją. Jeżeli API ma samo obsługiwać TLS, ustaw
`Security__ForceHttps=true` — domyślnie wyłączone, bo za proxy powodowałoby pętlę przekierowań.

### Zaufane proxy (`Security:TrustedProxyNetworks`)

Poza trybem Development aplikacja czyta adres klienta z `X-Forwarded-For`, ale **tylko od proxy
z zaufanej sieci**. Domyślnie są to pętla zwrotna i zakresy prywatne
(`10.0.0.0/8`, `172.16.0.0/12`, `192.168.0.0/16`) — nginx w compose mieści się w drugim z nich.

Ta lista nie jest ozdobnikiem. Gdy proxy do niej nie należy, nagłówek jest odrzucany i **wszyscy
użytkownicy dzielą jeden adres IP**, więc limit „10 nieudanych logowań na 5 minut” staje się
limitem na całą szkołę: dziesięć pomyłek jednej osoby blokuje pozostałym logowanie. W drugą
stronę pusta lista (zaufanie wszystkim) pozwoliłaby obejść ten limit samym nagłówkiem.

Gdy proxy stoi pod adresem publicznym, podaj jego zakres wprost:

```json
{ "Security": { "TrustedProxyNetworks": [ "203.0.113.7/32" ] } }
```

## Baza danych

Domyślnie SQLite. **Komplet migracji w repozytorium jest wygenerowany dla SQLite**
(kolumny `TEXT`/`INTEGER`), więc uruchomienie ich na PostgreSQL zakończy się błędem typów —
`Guid` mapuje się tam na `uuid`, a `bool` na `boolean`.

Przejście na PostgreSQL wymaga własnego zestawu migracji: wydzielenia ich do osobnego assembly
per provider (`--migrations-assembly`) i wygenerowania pod Npgsql. Usługa `postgres` w compose
czeka za profilem:

```bash
docker compose --profile postgres up
```

### Migracje EF

Schemat jest zarządzany migracjami; przy starcie aplikacja wykonuje `Database.Migrate()`.

```bash
cd backend
# nowa migracja po zmianie modelu:
dotnet ef migrations add <Nazwa> --project LessonRunner.Infrastructure --startup-project LessonRunner.Api --output-dir Persistence/Migrations
# ręczne zastosowanie:
dotnet ef database update --project LessonRunner.Infrastructure --startup-project LessonRunner.Api
```

## Uwierzytelnianie i role

- Logowanie: `POST /api/auth/login` → `{ token, expiresAt, user }`. Token ważny 12 h.
- Bieżący użytkownik: `GET /api/auth/me` (nagłówek `Authorization: Bearer <token>`).
- **Publicznej rejestracji nie ma** — konta zakłada wyłącznie administrator.
- Logowanie jest limitowane: 10 prób na 5 minut z jednego adresu IP (`429`).
- Token zawiera znacznik sesji. Dezaktywacja konta oraz zmiana lub reset hasła unieważniają
  wszystkie aktywne sesje **natychmiast**, bez czekania na wygaśnięcie tokenu.

### Reset hasła i zaproszenia

Konta zakłada administrator, ale hasła nie musi już wymyślać za nikogo.

- `POST /api/auth/password-reset` — żądanie linku (bez logowania). Odpowiada `202` **zawsze**,
  także dla adresu, którego nie ma w bazie: inaczej formularz „nie pamiętam hasła” byłby
  sprawdzaczem, kto ma konto w systemie.
- `GET /api/auth/password-reset/{token}` — czy link jeszcze żyje i czyje to konto.
- `POST /api/auth/password-reset/confirm` — ustawienie hasła z linku.
- `POST /api/users/{id}/invite` — zaproszenie wysyłane przez administratora (`AdminOnly`).

Konto można założyć **bez hasła** (puste pole w formularzu): wtedy system od razu wysyła
zaproszenie, a hasło ustawia sam użytkownik. Do konta bez hasła nikt się nie zaloguje, dopóki
tego nie zrobi.

Ważność linku: 2 godziny dla resetu, 7 dni dla zaproszenia. Link działa raz, a wydanie nowego
unieważnia poprzednie. W bazie leży wyłącznie skrót tokenu — postać jawna istnieje tylko
w wysłanym e-mailu.

Linki w wiadomościach budowane są od `App:PublicOrigin` (zmienna `PUBLIC_ORIGIN` w `.env`).
**Bez ustawienia tej zmiennej e-maile prowadzą pod `http://localhost:8080`** — backend nie zna
adresu frontendu, a zgadywanie go z nagłówka `Host` dałoby się podmienić z zewnątrz.
Przy `SMTP_MODE=Log` wiadomości nie wychodzą na świat, tylko trafiają do logu aplikacji —
wygodne przy pierwszym uruchomieniu, ale rodzic nic nie dostanie.

Uprawnienia:

| Zakres | Admin | Instructor | Parent |
|---|:--:|:--:|:--:|
| Konspekty (`/api/lessons`) — odczyt | tak | tak | **nie** |
| Konspekty — zapis, publikacja | tak | nie | nie |
| Grafik, kalendarz | tak | tylko swoje | **nie** |
| Grupy, uczestnicy, konta, płatności, operacje | tak | nie | nie |
| Portal rodzica (`/api/parent/portal`) | nie | nie | tak |
| Postępy i projekty (`/api/progress`) — zapis | tak | tylko swoje grupy | **nie** |
| Postępy i projekty — odczyt dorobku dziecka | wszystkie | tylko swoje grupy | w portalu, tylko swoje dzieci |

Klucz podpisu (`Jwt:SigningKey`, min. 32 bajty) jest wymagany — bez niego aplikacja celowo nie
wystartuje. Lokalnie bierze się z `appsettings.Development.json`, na produkcji ze zmiennej
środowiskowej `Jwt__SigningKey`.

## Import konspektów

Konspekt ma jawny rodzaj:

- `Rodzaj: standardowa` — zwykłe zajęcia grupowe, **dokładnie 95 minut**: 45 pracy,
  5 przerwy, 45 pracy. Żaden blok bez przerwy nie może przekroczyć 45 minut.
- `Rodzaj: pokazowa` — spotkanie **dokładnie 60-minutowe**, bez wymaganej przerwy. Od 55. minuty
  uczestnik może zakończyć zajęcia, więc końcówkę planuje się jako domknięcie, a nie nowy
  materiał. To granica wyjścia, a nie dopuszczalna długość planu: konspekt na 55 minut
  zostawiałby pięć minut zarezerwowanego okna pustych.

Brak pola `Rodzaj` oznacza lekcję standardową, dzięki czemu starsze pliki pozostają zgodne.
Importer dobiera walidację czasu i przerwy do rodzaju, a wartość jest zapisywana w konspekcie.

Regułę czasu sprawdza też backend — przy publikacji i przy zapisie lekcji już opublikowanej.
Rodzaju nie da się zmienić, gdy konspekt jest wpięty w termin grupy albo w lekcję próbną.
Lekcji próbnej można przypisać wyłącznie opublikowany konspekt pokazowy.

Pojedynczy konspekt wgrywa się w aplikacji: **Konspekty → Importuj**, wklejając treść
albo wskazując plik `.md`. Po imporcie system otwiera edytor, żeby dorzucić zrzuty ekranu.

Całe partie plików — na przykład gotowy semestr z `docs/program/` — wgrywa skrypt wsadowy.
Używa tego samego parsera co ekran importu, więc plik wgrany wsadowo daje ten sam konspekt
co wklejony ręcznie.

```bash
cd frontend/lesson-runner-web && npm run import:lekcje -- --dir ../../docs/program/scratch-7-9 --dry-run
```

`--dry-run` tylko sprawdza pliki i nie łączy się z API — od tego zaczynaj. Bez tej flagi
skrypt loguje się i wysyła konspekty:

```bash
LESSON_RUNNER_PASSWORD='...' npm run import:lekcje -- --dir ../../docs/program/scratch-7-9 --email admin@example.com
```

Hasło idzie zmienną środowiskową, nie parametrem — parametry zostają w historii powłoki.
Zamiast loginu można podać gotowy `LESSON_RUNNER_TOKEN`.

- Błąd parsera w którymkolwiek pliku **przerywa całą partię**. Błąd znaczy, że fragment
  pliku nie trafiłby do konspektu, a wsadowo nikt tego nie zobaczy na ekranie. `--force`
  wymusza import mimo to.
- Konspekt o tytule, który już jest w systemie, zostaje **pominięty**. `--update` nadpisuje
  go zamiast pomijać, więc poprawki w plikach wgrywa się tą samą komendą.
- Konspekty powstają jako wersje robocze. `--publish` publikuje je od razu (wymaga roli Admin).
- Domyślny adres API to `http://localhost:5000`, inny podasz przez `--api`.

Pliki w `docs/program/` są pilnowane testem `programLessons.test.ts`: każdy musi się
parsować bez błędów i mieć cel, wprowadzenie, przerwę, podsumowanie oraz dokładnie
95 minut w krokach.

## Testy

```bash
cd backend && dotnet test
cd frontend/lesson-runner-web && npm test
```

Backend ma testy serwisów, testy repozytoriów EF (SQLite in-memory) oraz testy HTTP end-to-end
na `WebApplicationFactory` — te ostatnie pilnują uprawnień ról i cyklu życia sesji.

## Struktura repozytorium

```text
kodziaki/
├─ backend/
│  ├─ LessonRunner.Api/            # endpointy, DI, CORS, autoryzacja
│  ├─ LessonRunner.Application/    # przypadki użycia, DTO, interfejsy
│  ├─ LessonRunner.Domain/         # encje i reguły domenowe
│  ├─ LessonRunner.Infrastructure/ # EF Core, repozytoria, migracje, seed
│  └─ LessonRunner.Tests/          # testy backendu (xUnit)
├─ frontend/
│  └─ lesson-runner-web/           # aplikacja React/Vite + nginx.conf
├─ docs/                           # przykładowe konspekty w formacie importu
├─ .env.example                    # wzorzec konfiguracji dla docker compose
├─ STATUS.md                       # aktualny stan projektu
├─ PLAN.md                         # archiwum: historia budowy i wzorce
└─ AUDYT_2026-07-27.md             # ostatni audyt kodu
```

Pliki `Lesson Runner.dc.html` i `support.js` to referencyjny prototyp UI z początku projektu.
