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
| `AkHouse.Domain` | Encje, agregaty (`Lead`, `OfferItem`, `Realization`, `GalleryImage`, `PromotionCampaign`, `ShopProduct`, `ShopOrder`), value objects (`Email`), reguły biznesowe. Zero zależności zewnętrznych — także od ASP.NET Identity. |
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
- `GET /api/promotion-campaign` — publiczna kampania obowiązująca w bieżącej chwili albo `204`.
  Gdy harmonogramy się nakładają, wygrywa włączona kampania z najpóźniejszym startem.
- `GET/POST/PUT/DELETE /api/admin/promotion-campaigns[/{id}]` — **chronione uprawnieniem `Media`** —
  lista, tworzenie, edycja i usuwanie kampanii popup. Kampania zawiera wyróżnik, tytuł, opis,
  przycisk z bezpiecznym adresem, datę startu, opcjonalny koniec i przełącznik publikacji.
- `GET  /health` — status.

### Sklep (`/api/shop`, `/api/account`, `/api/admin/shop`)

Osobna ścieżka sprzedaży: gotowe produkty i akcesoria kupowane z półki, niezależna od lejka
zapytań ofertowych. **Kwoty są liczone wyłącznie na serwerze** — przeglądarka wysyła tylko
`productId` i `quantity`, a ceny czytane są z bazy. Pieniądze trzymamy w groszach jako `int`
(SQLite zapisuje `decimal` jako TEXT, przez co `SUM` i `ORDER BY` po cenie dawałyby złe wyniki).

Publiczne:
- `GET  /api/shop/products` — opublikowane produkty; filtry `?category=&search=&sort=`.
- `GET  /api/shop/products/{slug}` — karta produktu (`404` dla nieopublikowanego).
- `GET  /api/shop/categories` — aktywne półki, które mają choć jeden opublikowany produkt.
  Zasila chipy filtrów na `/sklep`; pusta półka nie trafia na listę, bo klik w nią dawałby pustą stronę.
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

Panel sklepu (**chronione uprawnieniem operatora `Shop`** — ciasteczko `akhouse.operator`; skrypty
mogą też użyć klucza maszynowego `Operators:MachineKeys:Shop` w nagłówku `X-Api-Key`. Szczegóły
w `KONTA-OPERATOROW.md`):
- `GET/POST/PUT/DELETE /api/admin/shop/products[/{id}]` — asortyment,
- `GET/POST/PUT/DELETE /api/admin/shop/suppliers[/{id}]` — dostawcy i ich dane wewnętrzne,
- `PUT /api/admin/shop/suppliers/{id}/margin` — ustawia narzut jednego dostawcy i przelicza tylko
  przypisane do niego pozycje z ceną bazową,
- `PUT /api/admin/shop/suppliers/margins` — jawnie ustawia ten sam narzut wszystkim aktywnym dostawcom,
- `POST /api/admin/shop/suppliers/{id}/base-prices` — wgrywa cennik wybranego dostawcy po jego kodzie
  produktu (ze zgodnością wsteczną po slugu); zwraca `updated` i `unknownIdentifiers`,
- `POST /api/admin/shop/products/bulk` — jedna zmiana na wielu pozycjach naraz: publikacja,
  dostępność, przeniesienie do kategorii, procentowa zmiana ceny, stan magazynowy (`stockQuantity`)
  albo jego zdjęcie (`clearStock`). Pola opcjonalne (`null` = nie ruszaj), zmiana ceny ograniczona
  do zakresu −99%…+500%, żeby zgubione zero nie rozdało asortymentu,
- `POST /api/admin/shop/products/{id}/images` (multipart, pole `file`) i `DELETE .../images/{imageId}`,
- `GET/POST/PUT/DELETE /api/admin/shop/categories[/{key}]` — półki sklepu. Kasowanie półki z produktami
  wymaga `?moveProductsTo=<klucz>` (inaczej `400`), więc nic nie zostaje bez kategorii,
- `POST /api/admin/shop/categories/restore-defaults` — odtwarza skasowane kategorie startowe (idempotentne),
- `GET/POST/PUT/DELETE /api/admin/shop/shipping-methods[/{id}]` — metody dostawy i stawki,
- `GET /api/admin/shop/orders[/{id}]`, `PATCH .../status`, `POST .../notes`, `PATCH .../read`.

Zmiana statusu na `Cancelled` **zwraca zarezerwowany stan magazynowy** na półkę (jednokrotnie);
cofnięcie anulowania rezerwuje go z powrotem i kończy się `400`, jeśli w międzyczasie towar
zszedł. Pozycje zamówienia trzymają własną kopię nazwy i ceny, więc późniejsza zmiana cennika ani
usunięcie produktu nie przepisują historii zamówień.

#### Stan magazynowy i limit sztuk

`ShopProduct.StockQuantity` jest **opcjonalny** i to rozróżnienie jest tu całą treścią:

| Wartość | Znaczenie | Ile klient kupi jednym zamówieniem |
|---|---|---|
| `null` | pozycja bez magazynu — sprowadzana od dostawcy albo sprzedawana na metry | 99 szt. (`ShopProduct.DefaultMaxOrderQuantity`) |
| `n > 0` | tyle sztuk stoi na półce | `n` |
| `0` | wyprzedane — znika ze sprzedaży, ale zostaje widoczne w sklepie | 0 |

Licznik zmniejsza się przy składaniu zamówienia i wraca przy jego anulowaniu. `IsAvailable` to
osobny, ręczny przełącznik: pozycję można zdjąć ze sprzedaży, mając ją na stanie, i sprzedawać
bez licznika. `IsOnSale` = opublikowana ∧ dostępna ∧ (bez licznika ∨ stan > 0).

Rezerwacja nie ma blokady optymistycznej: dwa zamówienia złożone w tej samej sekundzie mogą
odczytać ten sam stan i zejść poniżej zera. Przy skali tego sklepu to ryzyko teoretyczne, ale
zanim ruszy kampania z ograniczoną serią, warto dołożyć znacznik współbieżności na `ShopProducts`.

Limit trzyma **serwer**, w `ShopOrder.Place`. Pole „Ilość" w przeglądarce i przycinanie koszyka
w `POST /api/shop/cart/validate` są uprzejmością wobec klienta — nie zabezpieczeniem. To samo
żądanie da się wysłać z curla i wtedy jedyne, co stoi między sklepem a zamówieniem na 999 balii,
to walidacja w domenie.

### Panel sklepu (`/sklep/panel`)

Osobne wejście do zarządzania sklepem: asortyment, kategorie, zamówienia i dostawa.
Nie prowadzi tam żaden odnośnik, trasa jest wyłączona w `robots.txt` i nie ma jej w `sitemap.xml`.

**Dlaczego osobno, a nie zakładka w `/crm`:** wcześniej sklepem zarządzało się piątą zakładką
panelu zleceń, chronioną tym samym `Admin:ApiKey`. Kto miał prowadzić asortyment, dostawał wgląd
we wszystkie zapytania klientów. Teraz panel ma własny `Admin:ShopApiKey` — obie role nadaje się
i odbiera niezależnie. Klucz jest sprawdzany przy wejściu (nie dopiero przy pierwszym zapisie)
i leży w `sessionStorage`, więc znika po zamknięciu karty.

Pięć zakładek:

- **Asortyment** — tabela, nie ściana kafelków. Wyszukiwarka (nazwa, opis, adres), filtr kategorii,
  filtr widoczności (widoczne / ukryte / niedostępne), sortowanie i stronicowanie po 25 pozycji.
  Przy 156 pozycjach z importu cennika to jest różnica między „da się pracować" a „przewiń 155 kart,
  żeby zmienić cenę uszczelki".
  **Zaznaczenie wielu wierszy** odsłania pasek operacji masowych: pokaż/ukryj, dostępny/niedostępny,
  przenieś do kategorii, procentowa zmiana ceny. Zaznaczenie odfiltrowane z widoku samo z niego
  wypada — zmiana masowa nie może objąć pozycji, których operator już nie widzi.
- **Kategorie** — dodawanie, zmiana nazwy i adresu, kolejność, ukrywanie, usuwanie.
  Usunięcie półki z produktami wymaga wskazania, dokąd je przenieść.
- **Dostawcy i cennik** — kartoteka dostawców, osobne narzuty i import ich cen bazowych.
- **Zamówienia** — filtrowanie po statusie, płatności i stanie obsługi, wyszukiwarka po numerze,
  kliencie i kupionych pozycjach, kolorowe plakietki statusów, statusy i notatki.
- **Dostawa** — metody i stawki (przeniesione z dawnego `/admin` bez zmian).

#### Dostawcy, producenci i ceny

`ShopSupplier` opisuje źródło zakupu i przechowuje własny narzut. Produkt wskazuje dostawcę przez
`SupplierId`, a stabilny `SupplierProductCode` pozwala aktualizować jego cennik niezależnie od
publicznego slugu. Balia Technic jest pierwszym dostawcą utworzonym podczas migracji.

Producent nie jest wyprowadzany z dostawcy. Operator wpisuje go ręcznie w
`ShopProduct.ManufacturerName`; ta informacja jest widoczna klientowi na kafelku i stronie produktu.
Dostawca pozostaje informacją wewnętrzną panelu.

Cena sklepowa powstaje z `ShopProduct.BasePriceGrosze` i narzutu przypisanego dostawcy.
Każdy dostawca ma własny narzut, a osobna, wyraźnie opisana operacja może ustawić jednakową wartość
wszystkim aktywnym dostawcom.

Pierwotna formuła `hurt netto × 1,10 × 1,23` nie odtwarza detalu dostawcy — na sprawdzonych
pozycjach mieściła się w przedziale od −18% do +64% względem ich cen. Dlatego bazą jest ich cena,
a nie nasze przeliczenie.

Trzy rzeczy, które trzymają ten mechanizm w ryzach:

- **Narzut jest aplikowany, nie kumulowany.** Cena liczy się zawsze od bazy, nigdy od bieżącej.
  Ustawienie +10% dwa razy to nadal +10%, a wpisanie `0` wraca dokładnie do ceny dostawcy.
  Ta sama arytmetyka jest po obu stronach (`ShopPricing.Apply` i `features/shopadmin/pricing.ts`),
  co do grosza — inaczej podgląd kłamałby o wyniku.
- **Punkty bazowe, nie ułamki.** Narzut siedzi w bazie jako `int` (1% = 100), z tego samego
  powodu co pieniądze w groszach: SQLite zapisuje `decimal` jako TEXT, a `-5,5%` ma być dokładne.
- **Pusta cena bazowa = cena ręczna.** Pozycje bez cennika dostawcy i wszystko dodane samodzielnie
  mają `BasePriceGrosze = null` i narzut ich nie rusza. Panel pokazuje, ilu pozycji to dotyczy.

Zakres narzutu to −99%…+500%; poza nim API zwraca `400`, żeby zgubione zero nie przeceniło sklepu.

Pobieranie i dopasowanie cen Balii opisuje `tools/import-balia/README.md`. Dopasowanie **nie jest
automatyczne**: slugi po obu stronach się nie zgadzają, więc narzędzie produkuje listę do akceptacji
i osobno oznacza przypadki, w których obok stoi podobny wariant o innej cenie.

#### Kategorie sklepu żyją w bazie

Były enumem w kodzie C#, więc nowa półka wymagała zmiany kodu i wdrożenia. Teraz to tabela
`ShopCategories`, a panel jest jej właścicielem. Dwa identyfikatory, oba potrzebne:

- **`Key`** — niezmienny klucz główny. To po nim produkt trzyma się półki, i to on siedzi
  w kolumnie `ShopProducts.Category` — dokładnie tam, gdzie wcześniej stała nazwa enuma
  (`Heaters`, `TubShells`…). Dlatego przejście na tabelę **nie przepisało ani jednego ze 156
  wierszy produktów**, a migracja jest czysto addytywna (`CreateTable` + `CreateIndex`).
  Klucz nowej kategorii powstaje ze slugu i mieści się w 32 znakach.
- **`Slug`** — edytowalny adres w URL (`/sklep?kategoria=piece`). Zmiana nazwy półki dla
  odwiedzających nie może osierocić produktów, więc nazwa i adres się zmieniają, a klucz nie.

Celowo **nie ma klucza obcego** z `ShopProducts`. Ograniczenie na poziomie bazy wymusiłoby
przebudowę tabeli SQLite na żywym katalogu i wracałoby do operatora jako nieczytelny błąd
sterownika. Spójności pilnuje `ShopAdminService`: produkt nie zapisze się na nieistniejącej półce,
a półki z produktami nie da się skasować bez wskazania celu — po polsku, z liczbą pozycji.

### Ukryty panel zarządzania stroną (`/zdjecia`)

Wspólny panel kampanii reklamowych oraz podmiany zdjęć **i ich nazw** w konkretnych punktach strony:
kafelki sekcji powitalnej,
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

#### Sekcja „Kampanie reklamowe"

Operator ustawia treść popupu (opcjonalny wyróżnik, np. „-10%”, tytuł, opis i przycisk), jego cel,
datę rozpoczęcia, opcjonalną datę zakończenia oraz publikację. Można przygotować wiele kampanii z
wyprzedzeniem; panel pokazuje stan „aktywna”, „zaplanowana”, „zakończona” albo „wyłączona”. Aktywna
kampania pojawia się jako modal po wejściu klienta na publiczną część strony. Klient przechodzi do
wskazanego miejsca albo zamyka popup przyciskiem **X**; zamknięcie jest pamiętane do końca bieżącej
sesji przeglądarki. Zmiana treści kampanii powoduje pokazanie jej ponownie.

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
- `Operators:Bootstrap:Email` / `:DisplayName` / `:Password` — pierwsze konto właściciela, zakładane
  przy starcie API i **tylko wtedy, gdy nie ma jeszcze żadnego konta**. Dostaje wszystkie uprawnienia
  i wymóg zmiany hasła przy pierwszym logowaniu. Pełna instrukcja w `KONTA-OPERATOROW.md`.
- `Operators:MachineKeys:Enabled` + `:Leads` / `:Media` / `:Shop` — poświadczenia **maszynowe** dla
  skryptów (nagłówek `X-Api-Key`), domyślnie wyłączone. Osobne dla każdego obszaru; żaden nie
  zarządza kontami operatorów.

  Dostęp ludzi idzie przez konta, nie przez klucze: kto prowadzi asortyment, nie dostaje wglądu
  w zapytania klientów — ich telefony, budżety i historię kontaktów.
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
| **Routing** | `react-router-dom`: landing, katalog, kategorie, produkty, realizacje, zamówienie oraz narzędzia wewnętrzne `/crm`, `/zdjecia`, `/operatorzy`, `/sklep/panel` (konfigurator 3D bez trasy — zawieszony) | `App.tsx` |
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

### CRM — panel zleceń (`/crm`)
Stanowisko pracy do obsługi zapytań i zamówień na domki, sauny i meble na wymiar — **nie** na
asortyment sklepu, który ma własny panel. Po wejściu na `/crm` logujesz się kontem operatora
z uprawnieniem `Leads`; sesję trzyma ciasteczko `akhouse.operator` (`HttpOnly`).
Formularz kontaktowy, kreator wyceny i wpisy ręczne trafiają do jednego
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
