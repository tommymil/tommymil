# Stan prac — A.K. HOUSE

Aktualizacja: **25 sierpnia 2026**

> **Zaczynasz nowy czat?** Wskaż ten plik jako pierwszy. Sekcje 1–8 to pełny obraz projektu:
> co to jest, gdzie co leży, co działa, czego nie wolno ruszać i co zostało do zrobienia.
> Reguły pisania kodu są w `CLAUDE.md`, opis architektury w `README.md` — ten plik ich nie powtarza,
> tylko do nich odsyła.

---

## 1. Czym jest projekt

Strona firmowa i sklep internetowy A.K. HOUSE — producenta domków mobilnych, saun ogrodowych
i mebli na wymiar (Mątwica 79C, KRS 0001121503).

Dwa niezależne kanały sprzedaży w jednej aplikacji:

- **lejek zapytań ofertowych** — formularz kontaktowy, kreator wyceny i zgłoszenia ręczne
  wpadają do wspólnej listy leadów obsługiwanej w panelu `/admin`;
- **sklep** — 156 gotowych produktów (wkłady, piece, filtracja, akcesoria do balii i saun)
  z koszykiem, kontem klienta i płatnością przelewem, obsługiwany w panelu `/sklep/panel`.

Stack: **.NET 10 / C#** (Clean Architecture, EF Core + SQLite, Minimal API, xUnit v3)
oraz **React 19 + TypeScript** (Vite 8, MVVM, Vitest). Szczegóły w `README.md`.

## 2. Mapa repozytorium

```
ak-house/            ← repo nadrzędne, gałąź 1.0 (dokumentacja, tools/, wskaźniki submodułów)
├─ backend/          ← SUBMODUŁ  ak-house-backend  (master)
│  ├─ src/AkHouse.Domain/          reguły biznesowe, bez zależności
│  ├─ src/AkHouse.Application/     przypadki użycia
│  ├─ src/AkHouse.Infrastructure/  EF Core, migracje, seed
│  └─ src/AkHouse.Api/             Minimal API, 7 plików endpointów
├─ frontend/         ← SUBMODUŁ  ak-house-frontend (main)
│  └─ src/features/  12 modułów, każdy: View / ViewModel / api
└─ tools/            skrypty pomocnicze (import cennika, import cen Balii)
```

**Trzy osobne repozytoria.** Zmiana kodu = zwykle dwa commity: najpierw w submodule, potem
w repo nadrzędnym (przesunięcie wskaźnika). Procedura w `CLAUDE.md`.

⚠️ Gałąź zdalna `origin/program-1.0` zawiera **inny projekt** („Lesson Runner") z własnym
`CLAUDE.md`. Nigdy nie czerp z niej reguł ani kodu.

## 3. Trzy panele, trzy klucze

| Panel | Adres | Klucz | Co daje |
|---|---|---|---|
| Zlecenia | `/admin` | `Admin:ApiKey` | Lejek leadów, zapytania klientów, e-maile, historia |
| Zdjęcia | `/admin/zdjecia` | `Admin:MediaApiKey` | Zdjęcia strony i galeria realizacji |
| Sklep | `/sklep/panel` | `Admin:ShopApiKey` | Asortyment, kategorie, cennik, dostawa, zamówienia |

Klucze są **celowo rozdzielone** — osoba prowadząca sklep nie widzi zapytań klientów.
Pilnują tego testy w `ShopAdminApiTests`. Klucz podaje się raz w oknie panelu; siedzi
w `sessionStorage` (`ak-shop-key`, `ak-media-key`), nie w kodzie.

Ustawianie lokalnie:

```powershell
dotnet user-secrets set "Admin:ShopApiKey" "<dlugi-losowy-ciag>" --project "D:\moje\ak-house\backend\src\AkHouse.Api"
```

Na produkcji przez zmienne środowiskowe z podwójnym podkreśleniem: `Admin__ShopApiKey`.
**Klucze użyte dotąd w rozmowach traktuj jako deweloperskie — na produkcję wygeneruj nowe.**

## 4. Gdzie czego szukać

| Plik | Co z niego bierzesz |
|---|---|
| `CLAUDE.md` | Jak pisać kod: język, warstwy, reguły domenowe, weryfikacja. Plus `backend/CLAUDE.md` i `frontend/CLAUDE.md`. |
| `README.md` | Architektura, pełna lista endpointów, konfiguracja, komendy. Jedyne miejsce na taki opis. |
| `STAN-PRAC.md` | Ten plik — stan prac i lista otwartych punktów. |
| `WYCENA.md` + `Wycena-AK-HOUSE-2026-08-23.pdf` | Dokument handlowy: podział na moduły, porównanie rynkowe, rabat. |
| `tools/import-balia/README.md` | Pobranie cen detalicznych Balii, dopasowanie, narzut. |
| `tools/import-cennik/README.md` | Starszy import cennika hurtowego (hurt × 1,10 × 1,23). |
| `ORDERS_SETUP.md` | **Archiwum.** Moduł `Orders` zastąpiony wspólnym lejkiem leadów. |
| `gemini-code-*.md` | Checklista UX/UI — wymagania produktowe, nie reguły kodu. |

## 5. Metryki (stan na 25.08.2026)

| | |
|---|---|
| Kod produkcyjny | 291 plików, ~27 200 linii (bez migracji i `node_modules`) |
| Endpointy HTTP | 70 |
| Migracje EF Core | 18, ostatnia `20260825132622_ShopCategoriesAndPricing` |
| Testy backendu | 148 metod w 14 plikach |
| Testy frontendu | 23 pliki, ostatni przebieg **131/131** |
| Moduły frontendu | 12 (`admin`, `catalog`, `configurator`, `consent`, `gallery`, `landing`, `media`, `notfound`, `order`, `realizations`, `shop`, `shopadmin`) |
| Produkty w sklepie | 156 w 13 używanych kategoriach (zdefiniowanych 15) |
| Ceny detaliczne wgrane | 68 z 156 |

## 6. Co powstało w sesji 25 sierpnia 2026

### 6.1 Osobne wejście do zarządzania sklepem — `/sklep/panel`

Nowy moduł `frontend/src/features/shopadmin/` (14 plików), całkowicie oddzielony od `/admin`.
Stary panel sklepu (`features/admin/components/shop/`) przeniesiony do `_to_delete/stary-panel-sklepu/`.

Pięć zakładek: **Produkty**, **Kategorie**, **Cennik**, **Dostawa**, **Zamówienia**.

Zakładka Produkty przy 156 pozycjach nie mogła zostać listą kafelków — jest tabela z wyszukiwarką,
filtrem kategorii i widoczności, sortowaniem, stronicowaniem po 25 i **operacjami zbiorczymi**
(zmiana kategorii, ukrycie/pokazanie, usunięcie zaznaczonych).

Trasa jest wyłączona z indeksowania (`robots.txt` → `Disallow: /sklep/panel`).

### 6.2 Kategorie przeniesione z enuma do bazy

Nowy agregat `ShopCategory` (`backend/src/AkHouse.Domain/Shop/ShopCategory.cs`) zastąpił enum
`ShopCategory`. Operator może teraz dodawać, zmieniać nazwę, slug i kolejność, ukrywać i usuwać półki
bez dotykania kodu.

**Migracja jest czysto addytywna i nie przepisała żadnego z 156 wierszy** — bo `ShopCategory.Key`
celowo używa dawnych nazw enuma (`Heaters`, `TubShells`, …), a kolumna `ShopProducts.Category`
zachowała nazwę, typ i szerokość 32:

```csharp
builder.Property(p => p.CategoryKey).HasColumnName("Category")
       .HasMaxLength(ShopCategory.MaxKeyLength).IsRequired();
```

Klucza obcego **nie ma świadomie** — spójności pilnuje `ShopAdminService`, a usunięcie kategorii
z produktami wymaga wskazania `moveProductsTo`. Piętnaście półek startowych siedzi
w `ShopCategoryDefaults`, przycisk *Przywróć domyślne* je odtwarza.

### 6.3 Cennik: cena detaliczna + narzut jednym kliknięciem

Nowa encja `ShopPricing` (jeden wiersz ustawień) i pole `ShopProduct.BasePriceGrosze`.

- **Narzut trzymany w punktach bazowych jako `int`** (1% = 100). Powód ten sam co przy pieniądzach:
  SQLite zapisuje `decimal` jako TEXT. Dodatkowo „5,5%" jest wtedy dokładnie 550, bez błędu zmiennoprzecinkowego.
- **Narzut liczy się zawsze od ceny bazowej, nigdy od bieżącej.** Kliknięcie tej samej wartości
  drugi raz nic nie zmienia; wpisanie `0` wraca dokładnie do cen Balii. Pilnuje tego test
  `ApplyMargin_ComputesFromTheBase_SoRepeatingItChangesNothing`.
- Produkty **bez** ceny bazowej (88 pozycji) zostają z ceną ustawianą ręcznie — narzut ich nie dotyka.

```csharp
public static int Apply(int basePriceGrosze, int marginBasisPoints)
{
    var scaled = decimal.Round(
        basePriceGrosze * (1m + marginBasisPoints / 10000m), 0, MidpointRounding.AwayFromZero);
    return Math.Max(1, (int)scaled);
}
```

W panelu wpisuje się procent z przecinkiem (`-5,5` działa), widać podgląd na trzech realnych
pozycjach i klika raz.

Nowe endpointy: `GET/PUT /api/admin/shop/pricing`, `PUT …/pricing/margin`,
`POST …/pricing/base-prices`, `POST …/products/bulk`, `GET/POST/PUT/DELETE …/categories[/{key}]`,
`POST …/categories/restore-defaults`.

### 6.4 Skąd wzięły się ceny detaliczne

Wasze ceny powstawały jako `hurt netto × 1,10 × 1,23` i rozjeżdżały się z detalem dostawcy
w obie strony (od −18% do +64% na sprawdzonych pozycjach) — jednym mnożnikiem się tego nie dało załatwić.

Zescrapowano **228 produktów** z baliatechnic.pl (WooCommerce Store API — ceny już w groszach,
`prices.price` przy `currency_minor_unit: 2`, więc nie ma jak pomylić się o 100×) do
`tools/import-balia/balia-katalog.json`.

Dopasowanie robi `tools/import-balia/import_balia.py`: ważony Jaccard na tokenach (0,6)
+ `SequenceMatcher` (0,4), progi `PEWNE = 0.86` / `WATPLIWE = 0.50`, plus **strażnik dwuznaczności**
(`MINIMALNA_PRZEWAGA = 0.08`). Ten ostatni jest ważny: „Wkład akrylowy okrągły" występuje u Balii
w szarej, białej i beżowej perle po 3799 / 3899 / 3999 zł — nazwy różnią się jednym słowem,
ceny dwiema stówami. Narzędzie w takiej sytuacji **odmawia wyboru zamiast strzelać**.

Wynik: **24 pewne + 44 zatwierdzone z wątpliwych = 68 cen wgranych.**
Zostaje **30 wątpliwych** i **58 bez dopasowania** (u Balii ich nie ma).

Uruchomienie całości jednym kliknięciem: `tools/import-balia/wgraj-ceny.cmd 8,5`
(migracje → build → klucz → start API → import → narzut). Do przeglądania na telefonie:
`tools/import-balia/wynik/przeglad-cen.html`.

### 6.5 Standardy kodu — dotąd ich nie było

Audyt wykazał, że projekt nie miał **żadnego** pliku mówiącego, jak pisać kod. Powstały:

- `CLAUDE.md` (root, backend, frontend) — tabela źródeł prawdy, procedura submodułów, zasady języka
  (kod po angielsku ~95%, UI i dokumenty po polsku z pełnymi diakrytykami), reguły domenowe;
- `.editorconfig` — C# 4 spacje / TS 2, CRLF dla `.ps1`, klamry Allman, reguły nazewnictwa,
  migracje EF wyłączone z egzekwowania stylu;
- `backend/Directory.Build.props` — wspólne `TargetFramework` / `Nullable` / `ImplicitUsings`,
  `TreatWarningsAsErrors` **tylko w Release**;
- `frontend/eslint.config.js` — zweryfikowane na realnym kodzie: **0 błędów, 36 ostrzeżeń** zastanych.
  Bramka je przepuszcza, ale **nie dokładaj nowych**.

## 7. Reguły, których nie wolno złamać

Pełna lista w `CLAUDE.md`. Najkosztowniejsze w skrócie:

- **Pieniądze to grosze w `int`**, narzut to punkty bazowe w `int`. Nigdy `decimal` w bazie.
- **Cenę liczy serwer.** Z przeglądarki przychodzą wyłącznie `productId` i `quantity`.
- **Pozycja zamówienia trzyma kopię nazwy i ceny** — zmiana cennika nie przepisuje historii.
- **Każdy endpoint administracyjny musi mieć filtr klucza.** Brak filtra = publiczny dostęp do danych klientów.
- **Trzy klucze zostają osobne.**
- **Sesja klienta to ciasteczko `HttpOnly`**, nie token w `localStorage`.
- **Publiczny endpoint przyjmujący dane** = zgoda RODO + honeypot + rate-limit per IP. Wszystkie trzy.
- **SVG w uploadzie jest odrzucany celowo** (wykonuje JavaScript po otwarciu wprost). JPG/PNG/WEBP/AVIF do 8 MB.
- **Konfigurator 3D jest zawieszony** — kod zostaje, nie prowadzi do niego trasa, nie rozwijaj go bez decyzji.
- Sekrety nigdy do repo: `Admin:ApiKey`, `Admin:MediaApiKey`, `Admin:ShopApiKey`,
  `Email:SmtpPassword`, `Shop:BankAccountNumber`.

### Pułapki środowiska (kosztowały czas)

- **Nie uruchamiaj `git` przez powłokę zdalną** w tym projekcie — zostawia pliki `index.lock`,
  których ten mount nie pozwala usunąć, i psuje natywnego gita.
- **`rm` nie działa na zamontowanym katalogu** („Operation not permitted") — używaj `mv` do `_to_delete/`.
- W PowerShellu kontynuacja wiersza to **backtick**, nie `^` (to CMD).
- `dotnet user-secrets` **wymaga `--project`**, inaczej szuka `.csproj` w bieżącym katalogu.

## 8. Co zweryfikowane, a co nie

| | Stan |
|---|---|
| Frontend: `test`, `typecheck`, `lint`, `build` | ✅ przechodzi lokalnie (131/131, 0 błędów lint) |
| Backend: `dotnet build` | ✅ przechodzi u klienta po poprawkach |
| Backend: `dotnet test` | ❌ **nigdy nie uruchomione** — 148 metod napisanych „na ślepo" |
| Migracja `ShopCategoriesAndPricing` | ✅ wygenerowana i zastosowana |
| Import 68 cen + narzut | ✅ przeszedł, panel działa |

Testy backendu zostały ponownie uruchomione 25 sierpnia 2026 po dodaniu oficjalnego SDK Stripe:
176/176 przechodzi. Frontend: 135/135; build produkcyjny i lint przechodzą (lint zachowuje
36 wcześniejszych ostrzeżeń, bez błędów).

## 9. Otwarte punkty

### Techniczne — do zrobienia najbliżej

0. **WYMIENIĆ KLUCZ z pliku `klucz upload/Nowy Dokument tekstowy.txt`.** Plik był zacommitowany
   i wypchnięty na GitHub — siedzi w historii, więc `git rm --cached` go stamtąd nie usunie.
   Jedyne skuteczne rozwiązanie to wygenerowanie nowego `Admin:MediaApiKey`.
1. **Zastane ostrzeżenia mogą zepsuć build `Release`** (`TreatWarningsAsErrors`): `xUnit1051`
   w kilku miejscach i jeden `CS8602` w `LeadTests`. Nie są moje, ale trzeba je zgasić przed wdrożeniem.
2. **Uruchomić `posprzataj.ps1`** — śmieci są już zebrane w `_do-usuniecia/` (115 MB), ale zdalna
   powłoka nie ma prawa kasować plików. Skrypt kasuje ten katalog, wynosi `elementy/` do
   `..\ak-house-materialy` i wypisuje z gita to, co przestało być częścią projektu.
   Podgląd bez zmian: `posprzataj.ps1 -NaSucho`.
3. Rozważyć zastąpienie kluczy API kontami operatorów (uzgodnione jako słuszny kierunek, nie zrobione).

### Ceny — 88 produktów wciąż bez ceny detalicznej

30 wątpliwych + 58 bez dopasowania. Pięć pytań czeka na rozstrzygnięcie:

- Pokrywa termiczna 200×200 — który wariant.
- Perła biała vs mleczna — to samo czy dwa różne wykończenia.
- Dwa wykończenia laminatu 196×203.
- Grzałka LX H30-RS1 „Komplet" — najpewniej 1229,99 zł, nie 438,99 zł.

### Przed uruchomieniem sprzedaży

- Płatności online są domyślnie niewidoczne. Aby je włączyć, ustaw sekrety środowiskowe
  `Payments__Stripe__SecretKey`, `Payments__Stripe__WebhookSecret` i dopiero wtedy
  `Payments__Stripe__Enabled=true`. Webhook Stripe kieruj na
  `/api/shop/payments/stripe/webhook`; checkout udostępnia kartę, BLIK i Przelewy24 w PLN.

- `Shop:BankAccountNumber` i nazwa banku — bez tego mail nie zawiera danych do przelewu.
- `Shop:SiteBaseUrl` na produkcji — używany w linku resetu hasła i adresach powrotu z płatności.
- Weryfikacja prawna `regulamin.html` i `zwroty.html`, uzupełnienie pól `[w nawiasach]`.
- Realny telefon, WhatsApp i profile społecznościowe zamiast tymczasowych.
- Produkcyjny adres e-mail i konfiguracja SMTP.
- Stawki dostawy — seed zakłada kurier 25 zł (gratis od 500 zł), paleta 199 zł, odbiór osobisty 0 zł.
- Zdjęcia, rzuty i zatwierdzone specyfikacje produktów katalogowych.
- Poprawić pozostawioną informację „12+".

### Handlowe

- Potwierdzić, czy meble od współwłaściciela to **barter** (skutki podatkowe) czy zwykły rabat
  z dobrej woli. Wycena zakłada na razie to drugie.

## 10. Komendy startowe

```powershell
# backend  → http://localhost:5033
dotnet run --project D:\moje\ak-house\backend\src\AkHouse.Api

# frontend → http://localhost:5173
npm --prefix D:\moje\ak-house\frontend run dev
```

Panele wymagają **obu** procesów. Migracje wykonują się automatycznie przy starcie backendu.

Pełna weryfikacja przed commitem:

```powershell
dotnet build D:\moje\ak-house\backend\AkHouse.slnx
dotnet test  D:\moje\ak-house\backend\AkHouse.slnx
npm --prefix D:\moje\ak-house\frontend run lint
npm --prefix D:\moje\ak-house\frontend run typecheck
npm --prefix D:\moje\ak-house\frontend test
npm --prefix D:\moje\ak-house\frontend run build
```

---

# Archiwum

Plan rozwoju sklepu po bieżącym P0/P1 znajduje się w `ROADMAP-SKLEP.md`. Obejmuje odłożone
P2/P3 oraz jednoznaczną zasadę, że konfigurator 3D pozostaje w kodzie, lecz bez publicznej trasy,
odnośników i ładowania WebGL.

## 12 sierpnia 2026 — sklep internetowy uruchomiony

Dodano osobną zakładkę **Sklep** na gotowe produkty i akcesoria, niezależną od lejka zapytań ofertowych:

- katalog sklepu z bazy (`ShopProduct`, `ShippingMethod`), strony `/sklep` i `/sklep/:slug`;
- koszyk w `localStorage` (`akhouse.cart.v1`) z przeliczaniem cen po stronie serwera;
- checkout z płatnością przelewem tradycyjnym, numerem `SKL-…` i mailem potwierdzającym;
- konta klientów na ASP.NET Core Identity (ciasteczko `HttpOnly`), zakup jako gość nadal możliwy;
- wzorce `regulamin.html` i `zwroty.html` + akceptacja regulaminu wymagana przy zamówieniu.

Migracje: `AddShopCatalog`, `AddShopOrders`, `AddCustomerAccounts`, `ReplaceStockWithAvailability`.
Testy po zmianach: backend 149/149, frontend 106/106.

## 29 lipca 2026 — wspólny lejek sprzedażowy i katalog

- **Dane firmy** scentralizowane we frontendzie (KRS 0001121503, NIP 7182170992, REGON 529386091).
- **Wspólny lejek leadów**: formularz kontaktowy, kreator wyceny i zgłoszenia ręczne w jednym miejscu.
  Widoki `Na dziś`, `Pulpit`, `Tablica`, `Lista`; etapy, priorytet, wartość, notatki, historia zmian.
  Migracja `UnifySalesPipeline` przeniosła historyczne zamówienia.
- **Ostatni kontakt i następne działanie** rozdzielone na trzy niezależne informacje (aktywność /
  rzeczywisty kontakt / zaplanowane działanie). Akcja „Kontakt wykonany", endpoint
  `POST /api/leads/{id}/contacts`, migracja `AddLeadLastContact`.
- **Katalog**: trasy `/produkty`, `/domki-drewniane`, `/sauny-ogrodowe`, `/kuchnie-na-wymiar`
  + lustrzane `/en/…`. Ceny tylko tam, gdzie były potwierdzone: Domek 28 — 129 000 zł,
  Sauna Panorama 12 — 49 000 zł, kuchnia — wycena indywidualna.
- **SEO**: canonicale, hreflang PL/EN + `x-default`, Open Graph, dane strukturalne
  `CollectionPage` / `Product` / `Offer`, sitemap, 14 statycznych stron wejściowych.
- **Nawigacja**: globalny reset przewijania przy zmianie trasy (`RouteScrollManager.tsx`),
  z zachowaniem deep-linków z hashem.

Testy po zmianach: backend 58/58, frontend 30/30.
Do zoptymalizowania: chunk `ConfiguratorScene` ~956 kB przed gzipem (konfigurator zawieszony).
