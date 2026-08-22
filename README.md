# A.K. HOUSE — strona producenta domków mobilnych, saun i mebli

Aplikacja webowa dla producenta domków mobilnych, saun i mebli na wymiar.
Repozytorium jest podzielone na dwie niezależnie wdrażalne części:

```
ak-house/
├─ backend/    → API w C# / .NET 10 (Clean Architecture)
├─ frontend/   → SPA w React + TypeScript (Vite), wzorzec MVVM
└─ (pliki źródłowe z Claude Design: A.K. HOUSE.dc.html, *.js, honeycomb.svg)
```

Obecny etap (MVP, Faza 0–1 z wyceny): **w pełni działająca strona prezentacyjna**
renderowana 1:1 z projektu, z treścią serwowaną przez backend i działającym
formularzem kontaktowym (lead trafia do bazy).

## ⏸ Konfigurator 3D — ZAWIESZONY

**Konfigurator 3D (WebGL / Three.js) jest wstrzymany i nie jest wystawiany publicznie.**
Nie prowadzi do niego żadna trasa ani odnośnik; kod zostaje w repozytorium na przyszłość.

Zawieszone pliki — nie rozwijaj ich bez decyzji o wznowieniu:

- `frontend/src/features/configurator/ConfiguratorPage.tsx` (bez trasy w `App.tsx`),
- `frontend/src/features/landing/components/ConfiguratorSection.tsx` (nierenderowana na landingu),
- `frontend/src/features/landing/components/ConfiguratorScene.tsx` + `.lazy.tsx`,
- `frontend/src/features/landing/viewmodels/useConfigurator.ts`,
- `frontend/public/models/test-project/3d-model.fbx` (2,4 MB — ładowany wyłącznie przez powyższe).

Wybór wariantu i rozmiaru odbywa się zamiast tego **bez 3D**, w kreatorze wariantów
(`frontend/src/features/catalog/variants.tsx`): rysunki elewacji w SVG plus rzut z góry.
Ten sam kreator działa na podstronach kategorii i w kroku „Szczegóły" na `/zamowienie`.

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
| `AkHouse.Domain` | Encje, agregaty (`Lead`, `OfferItem`, `Realization`, `GalleryImage`, `ShopProduct`, `ShopOrder`), value objects (`Email`), reguły biznesowe. Zero zależności zewnętrznych — także od ASP.NET Identity. |
| `AkHouse.Application` | Przypadki użycia (`LeadService`, `ContentService`), DTO, interfejsy portów (`IApplicationDbContext`, `IEmailSender`, `IDateTimeProvider`). |
| `AkHouse.Infrastructure` | EF Core (SQLite), konfiguracje encji, migracje, seeder, implementacje portów. **Tu izolowany jest dostawca bazy** — zamiana SQLite → PostgreSQL (zgodnie z wyceną) to jedna linijka w `DependencyInjection`. |
| `AkHouse.Api` | Minimal API (endpointy `Content`, `Leads`), CORS, OpenAPI, bootstrap. |

**Dlaczego tak:** testowalność (logika domenowa bez infrastruktury), wymienialność
warstw (baza, transport e-mail), i czytelna granica pod rozrost o konfigurator,
CPQ i panel CMS bez przepisywania rdzenia.

### Endpointy
- `GET  /api/content` — komplet treści strony (oferta, atuty, realizacje, galeria, kontakt).
- `POST /api/leads` — zapis zapytania z formularza. Wymaga zgody RODO; honeypot odsiewa boty; limit **5 zgłoszeń/min/IP** (`429` po przekroczeniu). Walidacja domenowa → `400` z `ProblemDetails`.
- `POST /api/leads/quote` — zapis pełnego briefu z kreatora wyceny w tym samym lejku; zwraca numer `ZAM-…` i zachowuje produkt, budżet, termin, lokalizację oraz preferowany kontakt.
- `POST /api/leads/manual` — **chronione** — ręczne dodanie zlecenia (np. z telefonu); bez rate-limitu i bez maila do klienta. E-mail opcjonalny, ale wymagany e-mail **lub** telefon. Zwraca utworzone zlecenie.
- `GET  /api/leads` — **chronione** (nagłówek `X-Api-Key`) — lista ostatnich leadów dla studia.
- `GET  /api/leads/stats` — **chronione** — agregaty do pulpitu (liczby per etap, nowe w tygodniu, zaległe terminy, wartość lejka).
- `GET  /api/leads/{id}` — **chronione** — pojedyncze zlecenie z osią czasu działań (`404` gdy brak).
- `PATCH /api/leads/{id}/stage` — **chronione** — zmiana etapu w lejku (`New`/`Contacted`/`Quote`/`Negotiation`/`Ordered`/`Production`/`Delivery`/`Completed`/`Lost`); `204`, `404` gdy brak, `400` dla nieznanego etapu. Zmiana etapu jest automatycznie zapisywana w historii.
- `PATCH /api/leads/{id}/fields` — **chronione** — priorytet / szacowana kwota / termin kontaktu (każda zmiana trafia do historii).
- `POST /api/leads/{id}/contacts` — **chronione** — rejestracja wykonanego kontaktu; rozlicza stary termin i opcjonalnie ustawia następne działanie.
- `PATCH /api/leads/{id}/contact` — **chronione** — korekta danych kontaktowych (imię, e-mail, telefon, typ, treść); e-mail opcjonalny, wymagany e-mail lub telefon.
- `POST /api/leads/{id}/message` — **chronione** — wysyłka e-maila do klienta (przez skonfigurowany SMTP, w dev logowany); zapisywana w historii. `400` gdy zlecenie nie ma e-maila.
- `POST /api/leads/{id}/notes` — **chronione** — dodanie notatki do osi czasu (`400` dla pustej treści).
- `PATCH /api/leads/{id}/read` — **chronione** — oznaczenie leada jako (nie)przeczytany.
- `GET  /api/site-images` — publiczna mapa `slot → { url, caption }`; SPA używa jej do podmiany zdjęć
  i nazw w konkretnych miejscach. Brak wpisu = zdjęcie i tekst wkompilowane w build.
- `GET/POST/DELETE /api/admin/site-images[/{slot}]` — **chronione `Admin:MediaApiKey`** — lista, wgranie
  (multipart, pole `file`) i przywrócenie oryginału. Dozwolone JPG/PNG/WEBP/AVIF do 8 MB; SVG jest
  odrzucany, bo wykonuje JavaScript po otwarciu bezpośrednio.
- `PUT /api/admin/site-images/{slot}/caption` — **chronione** — zmiana nazwy/podpisu miejsca
  (maks. 200 znaków). Pusta wartość przywraca tekst z builda; gdy slot nie ma też wgranego zdjęcia,
  nadpisanie znika w całości (`204`).
- `GET  /api/gallery-photos` — publiczna lista dodatkowych zdjęć galerii (bez slotów), od najnowszych.
- `GET/POST/PUT/DELETE /api/admin/gallery-photos[/{id}]` — **chronione `Admin:MediaApiKey`** — dodanie
  (multipart: `file`, `caption`, `category`), zmiana podpisu i kategorii, usunięcie. Kategoria musi być
  jedną z `domki`/`sauny`/`meble`/`wnetrza`.
- `GET  /health` — status.

### Sklep (`/api/shop`, `/api/account`, `/api/admin/shop`)

Osobna ścieżka sprzedaży: gotowe produkty i akcesoria kupowane z półki, niezależna od lejka
zapytań ofertowych. **Kwoty są liczone wyłącznie na serwerze** — przeglądarka wysyła tylko
`productId` i `quantity`, a ceny czytane są z bazy. Pieniądze trzymamy w groszach jako `int`
(SQLite zapisuje `decimal` jako TEXT, przez co `SUM` i `ORDER BY` po cenie dawałyby złe wyniki).

Publiczne:
- `GET  /api/shop/products` — opublikowane produkty; filtry `?category=&search=&sort=`.
- `GET  /api/shop/products/{slug}` — karta produktu (`404` dla nieopublikowanego).
- `GET  /api/shop/shipping-methods` — aktywne metody dostawy ze stawkami.
- `POST /api/shop/cart/validate` — przelicza koszyk: aktualne ceny, przycięcie ilości do stanu
  magazynowego, lista pozycji wycofanych ze sprzedaży.
- `POST /api/shop/orders` — złożenie zamówienia (honeypot + rate-limit jak przy leadach).
  Rezerwuje stan magazynowy i wysyła maila z numerem `SKL-…` i danymi do przelewu.
- `GET  /api/shop/orders/{reference}?email=` — status zamówienia dla gościa. Sam numer nie
  wystarczy — e-mail musi się zgadzać, inaczej `404`.

Konto klienta (ciasteczko sesji `akhouse.session`, `HttpOnly`):
- `POST /api/account/register` `/login` `/logout` `/forgot-password` `/reset-password`,
- `GET/PUT /api/account/profile` — dane do wysyłki,
- `GET  /api/account/orders` — historia zamówień zalogowanego (**chronione**).

Zakup jako gość jest możliwy; przy zalogowanej sesji zamówienie dostaje `UserId` i pojawia się
w `/konto`. Zakładanie konta nie „przejmuje” wcześniejszych zamówień gościa z tym samym adresem.

Panel (**chronione `Admin:ApiKey`**, nagłówek `X-Api-Key`):
- `GET/POST/PUT/DELETE /api/admin/shop/products[/{id}]` — asortyment,
- `POST /api/admin/shop/products/{id}/images` (multipart, pole `file`) i `DELETE .../images/{imageId}`,
- `GET/POST/PUT/DELETE /api/admin/shop/shipping-methods[/{id}]` — metody dostawy i stawki,
- `GET /api/admin/shop/orders[/{id}]`, `PATCH .../status`, `POST .../notes`, `PATCH .../read`.

Zmiana statusu na `Cancelled` **zwraca zarezerwowany stan magazynowy** na półkę (jednokrotnie).
Pozycje zamówienia trzymają własną kopię nazwy i ceny, więc późniejsza zmiana cennika ani
usunięcie produktu nie przepisują historii zamówień.

### Ukryty panel zdjęć (`/admin/zdjecia`)

Miejsce do podmiany zdjęć **i ich nazw** w konkretnych punktach strony: kafelki sekcji powitalnej,
galeria, realizacje, nagłówki i kafelki kategorii, zdjęcia produktów oraz galerie produktowe. Każde
miejsce ma edytowalną nazwę, informację gdzie występuje, podgląd aktualnego zdjęcia i przyciski
**Zapisz** / **Zmień** / **Przywróć**.

Nazwa działa dwojako i panel to rozróżnia (`nameOnSite` w rejestrze):
- **„Nazwa widoczna na stronie"** — kafelki galerii, realizacje i podpisy w galeriach produktów:
  tekst czyta odwiedzający, więc poprawka typu „Wnętrze sauny” → „Sauna od frontu” zmienia treść witryny.
- **„Opis zdjęcia (alt)"** — pozostałe miejsca, gdzie żaden napis nie jest wyświetlany; zmiana wpływa
  na atrybut `alt` (dostępność i SEO).

- Nie prowadzi tam żaden odnośnik; trasa jest wyłączona w `robots.txt` i nie ma jej w `sitemap.xml`.
- Wejście chroni `Admin:MediaApiKey` (nagłówek `X-Api-Key`), trzymany w `sessionStorage` — znika po
  zamknięciu karty. Klucz jest weryfikowany przy wejściu, nie dopiero przy pierwszym wgraniu.
- Wgrane pliki leżą poza `wwwroot` (`Media:UploadRoot`) i są serwowane spod `/uploads`. Dzięki temu
  publikacja z Visual Studio ich nie kasuje.
- „Przywróć" usuwa nadpisanie, więc miejsce wraca do zdjęcia z builda — nic nie jest tracone
  bezpowrotnie po stronie kodu.

Nowe miejsca dochodzą automatycznie: slot to `id` komponentu `<ImageSlot>`, a rejestr nazw dla panelu
żyje w `frontend/src/app/imageSlots.ts` (kategorie i produkty są z niego wyprowadzane z `catalog.ts`).

#### Sekcja „Galeria — dodatkowe zdjęcia"

Osobna, **nieograniczona co do liczby** lista zdjęć: wgrywasz ile chcesz (można zaznaczyć wiele plików
naraz), każde dostaje podpis i kategorię (Domki / Sauny / Meble / Wnętrza), a usunięcie kasuje też plik
z dysku. Te zdjęcia pojawiają się **wyłącznie na `/galeria`** — nie zmieniają żadnego ze slotów, więc
pozostałe podstrony zachowują swoje dobrane zdjęcia.

## Strona galerii (`/galeria`, `/en/galeria`)

Samodzielna strona z pełną galerią: kafelki z treści serwowanej przez API plus wszystko dodane
w panelu, z filtrami kategorii i powiększeniem. Pozycja **Galeria** w nawigacji prowadzi wprost tutaj
(wcześniej przewijała stronę główną do zapowiedzi), a przycisk pod zapowiedzią na stronie głównej
kieruje w to samo miejsce.

Limit zgłoszeń jest konfigurowalny: `RateLimiting:LeadsPerMinute` (domyślnie 5).

Globalny `IExceptionHandler` mapuje błędy domenowe na `400`, pozostałe na `500` (RFC 7807).

### Konfiguracja (`appsettings.json` / zmienne środowiskowe / `dotnet user-secrets`)
- `Admin:ApiKey` — klucz do endpointu admina. Domyślnie pusty (panel jest wtedy wyłączony);
  ustaw go poza repo, np. `dotnet user-secrets set "Admin:ApiKey" "<silny-losowy-klucz>" --project backend/src/AkHouse.Api`.
- `Admin:MediaApiKey` — **osobny** klucz do ukrytego panelu zdjęć (`/admin/zdjecia`). Niezależny od
  `Admin:ApiKey`, więc dostęp do zleceń i do zdjęć nadaje się i odbiera oddzielnie. Pusty = panel wyłączony (503).
- `Shop:BankAccountNumber` / `BankAccountHolder` / `BankName` / `PaymentDueDays` — dane do przelewu
  wysyłane klientowi w potwierdzeniu zamówienia. **Numer konta jest domyślnie pusty** — dopóki go nie
  uzupełnisz, mail informuje, że dane prześlemy osobno.
- `Shop:SiteBaseUrl` — publiczny adres używany do budowania linku resetu hasła w mailu. Brany
  z konfiguracji, a nie z nagłówka `Host`, żeby podrobiony nagłówek nie przekierował linku.
  W `appsettings.Development.json` wskazuje na `http://localhost:5173`.
- `Media:UploadRoot` — katalog na wgrane zdjęcia. Pusty = `uploads/` obok binariów, co na Azure
  **znika przy każdej publikacji** — na produkcji ustaw ścieżkę pod `/home`, np. `/home/data/uploads`.
- `Email:SmtpHost` + `SmtpPort/SmtpUser/SmtpPassword/SmtpUseSsl`, `Email:FromAddress/FromName`, `Email:StudioInbox` — gdy `SmtpHost` puste, używany jest deweloperski `LoggingEmailSender` (loguje maile zamiast wysyłać).
- Sekrety trzymaj poza repo: `dotnet user-secrets set "Email:SmtpPassword" "…"`.

### Testy
`backend/tests/AkHouse.Tests` (xUnit): testy domeny (`Lead`, `Email`, `ShopProduct`, `ShopOrder`,
`ShippingMethod`) + integracyjne na `WebApplicationFactory` (POST leada, walidacja, honeypot,
ochrona admina, katalog i checkout sklepu, konta klientów, panel sklepu).
Uruchom: `dotnet test backend/AkHouse.slnx`.

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
| **Routing** | `react-router-dom`: landing, katalog, kategorie, produkty, realizacje, zamówienie i panel `/admin` (konfigurator 3D bez trasy — zawieszony) | `App.tsx` |
| **Design tokens** | Kolory, typografia, kształt hex — jedno źródło prawdy | `app/theme.ts`, `app/global.css` |

Komponenty nie wołają `fetch` bezpośrednio — robią to view-modele przez warstwę `api/`,
co ułatwia testy i przyszłą rozbudowę (np. React Query, kolejne strony).

### Katalog i strony produktowe

- `/produkty` — indeks trzech kategorii.
- `/domki-drewniane`, `/sauny-ogrodowe`, `/kuchnie-na-wymiar` — dedykowane landingi kategorii.
- `/domki-drewniane/domek-28`, `/sauny-ogrodowe/sauna-panorama-12`,
  `/kuchnie-na-wymiar/kuchnia-indywidualna` — referencyjne karty produktów.
- Każda trasa ma lustrzaną wersję z prefiksem `/en`, canonical/hreflang, meta description
  oraz schema.org (`CollectionPage`/`Product`; `Offer` tylko tam, gdzie istnieje jawna cena bazowa).
- Katalog jest ładowany jako osobne chunki, więc jego rozbudowana treść nie zwiększa istotnie
  początkowego pakietu strony głównej.

### Sklep (`/sklep`) — osobna zakładka

Polskojęzyczna część sklepowa, bez lustra `/en` (przełącznik języka jest na tych trasach ukrywany,
bo nie ma dokąd prowadzić). Trasy: `/sklep`, `/sklep/:slug`, `/koszyk`, `/zamowienie-sklep`,
`/zamowienie-sklep/potwierdzenie`, `/konto` oraz `/konto/logowanie|rejestracja|haslo|nowe-haslo`.

- **Koszyk** żyje w `localStorage` pod wersjonowanym kluczem `akhouse.cart.v1` — działa dla gościa
  i nie wymaga sesji. Trzymana tam cena służy **wyłącznie do podglądu**: strona koszyka i checkout
  zawsze przeliczają zamówienie po stronie serwera.
- **Płatność**: przelew tradycyjny. Klient dostaje numer `SKL-…` i dane do przelewu w mailu.
  Bramka płatnicza jest poza zakresem MVP.
- **Konto** jest opcjonalne. Sesja to ciasteczko `HttpOnly` (nie token w `localStorage`), więc
  skrypt na stronie nie ma do niej dostępu.
- Ekran `/zamowienie-sklep/potwierdzenie` po odświeżeniu zamienia się w wyszukiwarkę statusu
  zamówienia (numer + e-mail), zamiast pokazywać pustą stronę.
- Dokumenty `/regulamin.html` i `/zwroty.html` to **wzorce do weryfikacji prawnej** — akceptacja
  regulaminu jest warunkiem złożenia zamówienia i jest sprawdzana także po stronie serwera.

### Panel zleceń (`/admin`)
Stanowisko pracy do kompleksowej obsługi zamówień. Po wejściu na `/admin` podajesz **klucz API**
(ten z `Admin:ApiKey`), który trafia do nagłówka `X-Api-Key` i jest trzymany w `sessionStorage`
(znika po zamknięciu karty). Formularz kontaktowy, kreator wyceny i wpisy ręczne trafiają do jednego
lejka; migracja kopiuje do niego również historyczne rekordy z tabel zamówień. Panel ma cztery widoki przełączane w nagłówku:
- **Na dziś** (domyślny) — lista robocza operatora: działania po terminie, zaplanowane na dziś oraz
  nowe/nieprzeczytane zgłoszenia. Licznik na zakładce pokazuje, ile pozycji wymaga uwagi.
- **Pulpit** — kafelki KPI (nowe w tygodniu, zlecenia w toku, działania po terminie, wartość
  lejka) oraz rozkład zleceń per etap (klik w etap przenosi do przefiltrowanej listy).
- **Tablica** (kanban) — kolumna na każdy etap lejka; karty przeciąga się między etapami
  (natywny HTML5 drag-and-drop), co optymistycznie zmienia etap (z wycofaniem przy błędzie).
- **Sklep** — asortyment (dodawanie, edycja, zdjęcia, publikacja), zamówienia sklepowe
  (status, notatki) oraz metody dostawy ze stawkami. Ceny wpisujesz w złotych, w bazie lądują
  w groszach.
- **Lista** — tabela z filtrami i sortowaniem po ostatniej aktywności, ostatnim kontakcie,
  dacie wpływu albo następnym działaniu. Domyślnie najdawniej obsługiwane zgłoszenia są na górze.

Przycisk **„+ Nowe zlecenie"** w nagłówku otwiera formularz ręcznego dodania zgłoszenia spoza
strony (np. telefon) — wymaga nazwy oraz e-maila lub telefonu; pozwala od razu ustawić etap,
priorytet, kwotę i termin. Nowe zlecenie pojawia się natychmiast na liście/tablicy.

Klik w kartę/wiersz otwiera **panel szczegółów** (drawer): edytowalne dane kontaktowe (imię,
telefon, e-mail, typ, treść), edycja etapu, priorytetu, kwoty wyceny i opcjonalnego następnego działania,
osobne daty ostatniej aktywności i ostatniego realnego kontaktu oraz akcja **„Kontakt wykonany”**,
**wysyłka e-maila do klienta** z gotowymi szablonami (oferta / przypomnienie / podziękowanie),
oraz **oś czasu działań** — notatki operatora plus automatyczne wpisy o zmianach etapu, pól,
danych kontaktowych i wysłanych wiadomościach. KPI i grupowanie kanbana liczone są po stronie klienta
z jednej pobranej listy, więc edycje są widoczne natychmiast. To lekka bramka dla narzędzia
wewnętrznego — pod publiczne wdrożenie warto dołożyć pełne logowanie (osobne konta operatorów).

---

## Mapowanie na fazy z wyceny

- **Faza 0–1 (zrobione tu):** strona prezentacyjna 1:1 + treść z backendu + działający formularz (RODO, anty-spam, e-mail SMTP), podgląd leadów dla studia, a11y, SEO, testy. CMS = treść już jest w bazie i serwowana przez API, gotowa pod panel edycji.
- **Faza 2 — ZAWIESZONA:** konfigurator WebGL (Three.js / R3F) wraz z encjami `Product`/`Configuration`, regułami CPQ i generowaniem PDF. Kod pozostaje w repozytorium, ale nie jest wystawiany — szczegóły w sekcji „Konfigurator 3D — ZAWIESZONY" na górze. Rolę doboru wariantu przejął kreator SVG (`features/catalog/variants.tsx`).
- **Faza 3:** integracje (SMTP/SendGrid zamiast `LoggingEmailSender`), analityka, wdrożenie; zamiana SQLite → PostgreSQL.
- **Sklep (poza pierwotną wyceną):** katalog z bazy, koszyk, checkout z płatnością przelewem,
  konta klientów i obsługa zamówień w panelu. Poza zakresem MVP zostają: bramka płatnicza,
  faktury VAT, osobny adres wysyłki, kody rabatowe, wersja EN sklepu i integracja z kurierem.

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
