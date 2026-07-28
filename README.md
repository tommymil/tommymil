# A.K. HOUSE — strona z konfiguratorem 3D

Aplikacja webowa dla producenta domków mobilnych, saun i mebli na wymiar:
strona prezentacyjna + (docelowo) interaktywny konfigurator 3D z silnikiem wyceny (CPQ).
Repozytorium jest podzielone na dwie niezależnie wdrażalne części:

```
ak-house/
├─ backend/    → API w C# / .NET 10 (Clean Architecture)
├─ frontend/   → SPA w React + TypeScript (Vite), wzorzec MVVM
└─ (pliki źródłowe z Claude Design: A.K. HOUSE.dc.html, *.js, honeycomb.svg)
```

Obecny etap (MVP, Faza 0–1 z wyceny): **w pełni działająca strona prezentacyjna**
renderowana 1:1 z projektu, z treścią serwowaną przez backend i działającym
formularzem kontaktowym (lead trafia do bazy). Konfigurator 3D jest na razie
sekcją-zapowiedzią — pod jego pełną implementację (Three.js / React Three Fiber +
CPQ, Faza 2) architektura jest już przygotowana.

---

## Szybki start

Wymagania: **.NET SDK 10**, **Node.js 22+**.

### 1. Backend (port 5033)
```bash
cd backend
dotnet run --project src/AkHouse.Api
```
Przy starcie aplikuje migracje EF Core i zasiewa treść (SQLite: `akhouse.db`).
- Swagger / OpenAPI: `http://localhost:5033/openapi/v1.json`
- Health: `http://localhost:5033/health`

### 2. Frontend (port 5173)
```bash
cd frontend
npm install
npm run dev
```
Vite proxuje `/api/*` na backend (`:5033`), więc przeglądarka widzi jeden origin.
Jeśli 5173 jest zajęty, Vite wybierze kolejny wolny port — sprawdź log startowy.

---

## Architektura backendu (Clean Architecture)

Zależności wskazują do środka: `Api → Infrastructure → Application → Domain`.
Domena nie wie nic o bazie, web ani frameworkach.

| Projekt | Odpowiedzialność |
|---|---|
| `AkHouse.Domain` | Encje, agregaty (`Lead`, `OfferItem`, `Realization`, `GalleryImage`), value objects (`Email`), reguły biznesowe. Zero zależności zewnętrznych. |
| `AkHouse.Application` | Przypadki użycia (`LeadService`, `ContentService`), DTO, interfejsy portów (`IApplicationDbContext`, `IEmailSender`, `IDateTimeProvider`). |
| `AkHouse.Infrastructure` | EF Core (SQLite), konfiguracje encji, migracje, seeder, implementacje portów. **Tu izolowany jest dostawca bazy** — zamiana SQLite → PostgreSQL (zgodnie z wyceną) to jedna linijka w `DependencyInjection`. |
| `AkHouse.Api` | Minimal API (endpointy `Content`, `Leads`), CORS, OpenAPI, bootstrap. |

**Dlaczego tak:** testowalność (logika domenowa bez infrastruktury), wymienialność
warstw (baza, transport e-mail), i czytelna granica pod rozrost o konfigurator,
CPQ i panel CMS bez przepisywania rdzenia.

### Endpointy
- `GET  /api/content` — komplet treści strony (oferta, atuty, realizacje, galeria, kontakt).
- `POST /api/leads` — zapis zapytania z formularza. Wymaga zgody RODO; honeypot odsiewa boty; limit **5 zgłoszeń/min/IP** (`429` po przekroczeniu). Walidacja domenowa → `400` z `ProblemDetails`.
- `POST /api/leads/manual` — **chronione** — ręczne dodanie zlecenia (np. z telefonu); bez rate-limitu i bez maila do klienta. E-mail opcjonalny, ale wymagany e-mail **lub** telefon. Zwraca utworzone zlecenie.
- `GET  /api/leads` — **chronione** (nagłówek `X-Api-Key`) — lista ostatnich leadów dla studia.
- `GET  /api/leads/stats` — **chronione** — agregaty do pulpitu (liczby per etap, nowe w tygodniu, zaległe terminy, wartość lejka).
- `GET  /api/leads/{id}` — **chronione** — pojedyncze zlecenie z osią czasu działań (`404` gdy brak).
- `PATCH /api/leads/{id}/stage` — **chronione** — zmiana etapu w lejku (`New`/`Contacted`/`Quote`/`Negotiation`/`Ordered`/`Production`/`Delivery`/`Completed`/`Lost`); `204`, `404` gdy brak, `400` dla nieznanego etapu. Zmiana etapu jest automatycznie zapisywana w historii.
- `PATCH /api/leads/{id}/fields` — **chronione** — priorytet / szacowana kwota / termin kontaktu (każda zmiana trafia do historii).
- `PATCH /api/leads/{id}/contact` — **chronione** — korekta danych kontaktowych (imię, e-mail, telefon, typ, treść); e-mail opcjonalny, wymagany e-mail lub telefon.
- `POST /api/leads/{id}/message` — **chronione** — wysyłka e-maila do klienta (przez skonfigurowany SMTP, w dev logowany); zapisywana w historii. `400` gdy zlecenie nie ma e-maila.
- `POST /api/leads/{id}/notes` — **chronione** — dodanie notatki do osi czasu (`400` dla pustej treści).
- `PATCH /api/leads/{id}/read` — **chronione** — oznaczenie leada jako (nie)przeczytany.
- `GET  /health` — status.

Limit zgłoszeń jest konfigurowalny: `RateLimiting:LeadsPerMinute` (domyślnie 5).

Globalny `IExceptionHandler` mapuje błędy domenowe na `400`, pozostałe na `500` (RFC 7807).

### Konfiguracja (`appsettings.json` / zmienne środowiskowe / `dotnet user-secrets`)
- `Admin:ApiKey` — klucz do endpointu admina (w repo dev-owy; **zmień na produkcji**).
- `Email:SmtpHost` + `SmtpPort/SmtpUser/SmtpPassword/SmtpUseSsl`, `Email:FromAddress/FromName`, `Email:StudioInbox` — gdy `SmtpHost` puste, używany jest deweloperski `LoggingEmailSender` (loguje maile zamiast wysyłać).
- Sekrety trzymaj poza repo: `dotnet user-secrets set "Email:SmtpPassword" "…"`.

### Testy
`backend/tests/AkHouse.Tests` (xUnit): testy domeny (`Lead`, `Email`) + integracyjne na `WebApplicationFactory`
(POST leada, walidacja, honeypot, ochrona admina). Uruchom: `dotnet test backend/AkHouse.slnx`.

### Zgodność / RODO i SEO
Formularz ma checkbox zgody i link do **`/polityka-prywatnosci.html`** (wzorzec do uzupełnienia danymi firmy).
SEO: `robots.txt`, `sitemap.xml`, Open Graph/Twitter meta, favicon marki. Dostępność: powiązane `label`/`input`,
`aria-invalid`, modale z obsługą `Esc`, focus-trap i `role="dialog"`.

## Architektura frontendu (MVVM, feature-based)

| Warstwa | Rola | Lokalizacja |
|---|---|---|
| **View** | Komponenty prezentacyjne (czysty JSX + style z tokenów) | `features/landing/components/`, `features/admin/components/` |
| **ViewModel** | Hooki ze stanem i logiką (`useContactForm`, `useGalleryFilter`, `useConfigurator`, `useSiteContent`, `useLeads`, `useLeadDetail`, `useAdminAuth`…) | `features/*/viewmodels/` |
| **Model / usługi** | Klient HTTP, API treści i leadów, typy DTO | `api/`, `types/` |
| **Routing** | `react-router-dom`: `/` (landing), `/admin` (panel zleceń) | `App.tsx` |
| **Design tokens** | Kolory, typografia, kształt hex — jedno źródło prawdy | `app/theme.ts`, `app/global.css` |

Komponenty nie wołają `fetch` bezpośrednio — robią to view-modele przez warstwę `api/`,
co ułatwia testy i przyszłą rozbudowę (np. React Query, kolejne strony).

### Panel zleceń (`/admin`)
Stanowisko pracy do kompleksowej obsługi zamówień. Po wejściu na `/admin` podajesz **klucz API**
(ten z `Admin:ApiKey`), który trafia do nagłówka `X-Api-Key` i jest trzymany w `sessionStorage`
(znika po zamknięciu karty). Panel ma cztery widoki przełączane w nagłówku:
- **Na dziś** (domyślny) — lista robocza operatora: kontakty po terminie, zaplanowane na dziś oraz
  nowe/nieprzeczytane zgłoszenia. Licznik na zakładce pokazuje, ile pozycji wymaga uwagi.
- **Pulpit** — kafelki KPI (nowe w tygodniu, zlecenia w toku, zaległe terminy kontaktu, wartość
  lejka) oraz rozkład zleceń per etap (klik w etap przenosi do przefiltrowanej listy).
- **Tablica** (kanban) — kolumna na każdy etap lejka; karty przeciąga się między etapami
  (natywny HTML5 drag-and-drop), co optymistycznie zmienia etap (z wycofaniem przy błędzie).
- **Lista** — tabela z filtrami (szukajka, etap, priorytet, typ).

Przycisk **„+ Nowe zlecenie"** w nagłówku otwiera formularz ręcznego dodania zgłoszenia spoza
strony (np. telefon) — wymaga nazwy oraz e-maila lub telefonu; pozwala od razu ustawić etap,
priorytet, kwotę i termin. Nowe zlecenie pojawia się natychmiast na liście/tablicy.

Klik w kartę/wiersz otwiera **panel szczegółów** (drawer): edytowalne dane kontaktowe (imię,
telefon, e-mail, typ, treść), edycja etapu, priorytetu, kwoty wyceny i terminu kontaktu,
**wysyłka e-maila do klienta** z gotowymi szablonami (oferta / przypomnienie / podziękowanie),
oraz **oś czasu działań** — notatki operatora plus automatyczne wpisy o zmianach etapu, pól,
danych kontaktowych i wysłanych wiadomościach. KPI i grupowanie kanbana liczone są po stronie klienta
z jednej pobranej listy, więc edycje są widoczne natychmiast. To lekka bramka dla narzędzia
wewnętrznego — pod publiczne wdrożenie warto dołożyć pełne logowanie (osobne konta operatorów).

---

## Mapowanie na fazy z wyceny

- **Faza 0–1 (zrobione tu):** strona prezentacyjna 1:1 + treść z backendu + działający formularz (RODO, anty-spam, e-mail SMTP), podgląd leadów dla studia, a11y, SEO, testy. CMS = treść już jest w bazie i serwowana przez API, gotowa pod panel edycji.
- **Faza 2:** sekcja `ConfiguratorSection` → realny konfigurator WebGL (Three.js / R3F) w `features/configurator/`; encje `Product`/`Configuration`/reguły CPQ w domenie; endpointy zapisu konfiguracji i generowania PDF.
- **Faza 3:** integracje (SMTP/SendGrid zamiast `LoggingEmailSender`), analityka, wdrożenie; zamiana SQLite → PostgreSQL.

## Przydatne komendy

```bash
# Backend
dotnet build backend/AkHouse.slnx
dotnet ef migrations add <Nazwa> --project backend/src/AkHouse.Infrastructure --startup-project backend/src/AkHouse.Api --output-dir Persistence/Migrations

# Frontend
npm --prefix frontend run build       # typecheck + produkcyjny build
npm --prefix frontend run typecheck   # sama analiza typów
npm --prefix frontend test            # testy jednostkowe (Vitest + Testing Library)

# Assety (uruchamiane z katalogu frontend/)
node scripts/generate-honeycomb.mjs   # regeneruje public/honeycomb.svg (tło 3D)
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/optimize-images.ps1
                                      # tworzy webowe .jpg z oryginałów .png w public/images
```
