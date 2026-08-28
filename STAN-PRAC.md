# Stan prac — A.K. HOUSE

Aktualizacja: **27 sierpnia 2026, po południu**

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
  wpadają do wspólnej listy leadów obsługiwanej w CRM pod `/crm`;
- **sklep** — 155 gotowych produktów (wkłady, piece, filtracja, akcesoria do balii i saun)
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

## 3. Cztery panele, jedno logowanie

Klucze API zostały **zastąpione kontami operatorów** (27.08). Jeden formularz logowania obsługuje
wszystkie panele, a dostęp do każdego z nich jest osobną flagą przypisywaną kontu.

| Panel | Adres | Uprawnienie | Co daje |
|---|---|---|---|
| CRM (zlecenia) | `/crm` | `Leads` | Lejek leadów na domki i sauny, e-maile, historia |
| Strona | `/zdjecia` | `Media` | Kampanie reklamowe, zdjęcia strony i galeria realizacji |
| Sklep | `/sklep/panel` | `Shop` | Asortyment, kategorie, cennik, magazyn, dostawa, zamówienia |
| Konta | `/operatorzy` | `Operators` | Zakładanie kont, nadawanie i odbieranie dostępów |

Uprawnienia są **celowo rozdzielone** — osoba prowadząca sklep nie widzi zapytań klientów —
i **czytane z bazy przy każdym żądaniu**, więc odebranie dostępu działa natychmiast, bez czekania
na wygaśnięcie ciasteczka (`akhouse.operator`, `HttpOnly`). Pilnują tego testy w `OperatorApiTests`
i `ShopAdminApiTests`.

Pierwsze konto powstaje przy starcie API z `Operators:Bootstrap:*` i **tylko wtedy, gdy nie ma
jeszcze żadnego konta**; dostaje wszystkie uprawnienia i wymóg zmiany hasła przy pierwszym
logowaniu. Pełna instrukcja: `KONTA-OPERATOROW.md`.

Nagłówek `X-Api-Key` żyje dalej, ale **tylko jako poświadczenie maszynowe** dla skryptów
(`Operators:MachineKeys:{Leads,Media,Shop}`), domyślnie wyłączone. Żaden klucz maszynowy nie
zarządza kontami — pilnuje tego test `NoMachineKeyEverManagesOperators`.

**Hasła i klucze użyte dotąd w rozmowach traktuj jako deweloperskie — na produkcję wygeneruj nowe.**

## 4. Gdzie czego szukać

| Plik | Co z niego bierzesz |
|---|---|
| `CLAUDE.md` | Jak pisać kod: język, warstwy, reguły domenowe, weryfikacja. Plus `backend/CLAUDE.md` i `frontend/CLAUDE.md`. |
| `README.md` | Architektura, pełna lista endpointów, konfiguracja, komendy. Jedyne miejsce na taki opis. |
| `STAN-PRAC.md` | Ten plik — stan prac i lista otwartych punktów. |
| `ROADMAP-SKLEP.md` | Co świadomie odłożone: P2/P3 sklepu i los konfiguratora 3D. |
| `WYCENA.md` + `Wycena-AK-HOUSE-2026-08-23.pdf` | Dokument handlowy: podział na moduły, porównanie rynkowe, rabat. |
| `tools/import-balia/README.md` | Pobranie cen detalicznych Balii, dopasowanie, narzut. |
| `tools/import-cennik/README.md` | Starszy import cennika hurtowego (hurt × 1,10 × 1,23). |
| `ORDERS_SETUP.md` | **Archiwum.** Moduł `Orders` zastąpiony wspólnym lejkiem leadów. |
| `gemini-code-*.md` | Checklista UX/UI — wymagania produktowe, nie reguły kodu. |

Skrypty w katalogu głównym — każdy z przełącznikiem podglądu:

| Skrypt | Co robi |
|---|---|
| `zweryfikuj.ps1` | Pełna bramka przed commitem: build Release, testy, obie migracje na świeżej bazie, frontend. |
| `zacommituj.ps1` | Commituje trzy repozytoria we właściwej kolejności (`-NaSucho`, `-Wypchnij`). |
| `posprzataj.ps1` | Kasuje `_do-usuniecia/`, wynosi `elementy/` poza repo, porządkuje gita. |
| `tools/sprawdz-migracje.py` | Wykonuje SQL z migracji na tymczasowej bazie SQLite i sprawdza dane. |

## 5. Metryki (stan na 27.08.2026, po południu)

| | |
|---|---|
| Kod produkcyjny | ~290 plików, ~31 000 linii (bez migracji, `obj/`, `bin/` i `node_modules`) |
| Endpointy HTTP | 86 |
| Migracje EF Core | 24, ostatnia `20260827140000_ProductStock` |
| Testy backendu | **232 przypadki, wszystkie przechodzą** (Debug i Release) |
| Testy frontendu | 27 plików → **164 testy, wszystkie przechodzą** |
| Moduły frontendu | 13 (`admin`, `campaign`, `catalog`, `configurator`, `consent`, `gallery`, `landing`, `media`, `notfound`, `order`, `realizations`, `shop`, `shopadmin`) |
| Produkty w sklepie | 155 w 13 używanych kategoriach (zdefiniowanych 15) |
| Ceny detaliczne wgrane | 68 z 155 |
| Grupy wariantów | 6 grup obejmujących 17 produktów |

## 6. Co powstało 25 sierpnia 2026

### 6.1 Osobne wejście do zarządzania sklepem — `/sklep/panel`

Nowy moduł `frontend/src/features/shopadmin/` (14 plików), całkowicie oddzielony od CRM.
Stary panel sklepu (`features/admin/components/shop/`) przeniesiony do `_to_delete/stary-panel-sklepu/`.

Pięć zakładek: **Produkty**, **Kategorie**, **Dostawcy i cennik**, **Dostawa**, **Zamówienia**.

Zakładka Produkty przy 155 pozycjach nie mogła zostać listą kafelków — jest tabela z wyszukiwarką,
filtrem kategorii i widoczności, sortowaniem, stronicowaniem po 25 i **operacjami zbiorczymi**
(zmiana kategorii, ukrycie/pokazanie, usunięcie zaznaczonych).

Trasa jest wyłączona z indeksowania (`robots.txt` → `Disallow: /sklep/panel`).

### 6.2 Kategorie przeniesione z enuma do bazy

Nowy agregat `ShopCategory` (`backend/src/AkHouse.Domain/Shop/ShopCategory.cs`) zastąpił enum
`ShopCategory`. Operator może teraz dodawać, zmieniać nazwę, slug i kolejność, ukrywać i usuwać półki
bez dotykania kodu.

**Migracja jest czysto addytywna i nie przepisała żadnego ze 156 ówczesnych wierszy** — bo `ShopCategory.Key`
celowo używa dawnych nazw enuma (`Heaters`, `TubShells`, …), a kolumna `ShopProducts.Category`
zachowała nazwę, typ i szerokość 32:

```csharp
builder.Property(p => p.CategoryKey).HasColumnName("Category")
       .HasMaxLength(ShopCategory.MaxKeyLength).IsRequired();
```

Klucza obcego **nie ma świadomie** — spójności pilnuje `ShopAdminService`, a usunięcie kategorii
z produktami wymaga wskazania `moveProductsTo`. Piętnaście półek startowych siedzi
w `ShopCategoryDefaults`, przycisk *Przywróć domyślne* je odtwarza.

### 6.3 Cennik wielu dostawców

Encja `ShopSupplier` przechowuje kartotekę dostawcy i jego własny narzut. Produkt wskazuje dostawcę
przez `SupplierId`, ma niezależny `SupplierProductCode` do importów oraz opcjonalną cenę bazową.
Dotychczasowe produkty zostały przypisane do dostawcy Balia Technic. Producent/marka to osobne,
ręcznie uzupełniane pole `ManufacturerName` widoczne klientowi; dostawca pozostaje informacją wewnętrzną.

- **Narzut trzymany w punktach bazowych jako `int`** (1% = 100). Powód ten sam co przy pieniądzach:
  SQLite zapisuje `decimal` jako TEXT. Dodatkowo „5,5%" jest wtedy dokładnie 550, bez błędu zmiennoprzecinkowego.
- **Narzut liczy się zawsze od ceny bazowej, nigdy od bieżącej.** Ustawienie tej samej wartości
  drugi raz nic nie zmienia; wpisanie `0` wraca dokładnie do cen danego dostawcy. Pilnuje tego test
  `ApplyMargin_ComputesFromTheBase_SoRepeatingItChangesNothing`.
- Zmiana narzutu jednego dostawcy przelicza wyłącznie jego produkty. Osobna operacja zbiorcza
  ustawia tę samą wartość wszystkim aktywnym dostawcom.
- Produkty **bez** ceny bazowej zostają z ceną ustawianą ręcznie — narzut ich nie dotyka.

```csharp
public static int Apply(int basePriceGrosze, int marginBasisPoints)
{
    var scaled = decimal.Round(
        basePriceGrosze * (1m + marginBasisPoints / 10000m), 0, MidpointRounding.AwayFromZero);
    return Math.Max(1, (int)scaled);
}
```

W panelu zakładka **Dostawcy i cennik** pozwala zarządzać kartoteką i narzutem każdego dostawcy.

Endpointy: `GET/POST/PUT/DELETE …/suppliers[/{id}]`, `PUT …/suppliers/{id}/margin`,
`PUT …/suppliers/margins`, `POST …/suppliers/{id}/base-prices`, `POST …/products/bulk`,
`GET/POST/PUT/DELETE …/categories[/{key}]`,
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

### 6.6 Handel: płatności online, SKU, klasy wysyłki

Trzy migracje wieczorem, wszystkie wokół tego, żeby sklep dało się realnie prowadzić.

**`ShopCommerceUx`** dokłada do produktu `Sku`, `ManufacturerName` (w bazie zgodnie wstecznie jako
kolumna `Brand`), `Specifications` i `Compatibility`
oraz `DispatchTime`, a do zamówienia dane do faktury (`CompanyName`, `TaxId`, adres rozliczeniowy,
`WantsInvoice`) i ślad płatności (`PaymentMethod`, `PaymentProviderReference`).

**`ProductShippingClasses`** zamienia pojedynczą klasę wysyłki na **maskę bitową**
`ShippingClasses` (`Parcel` 1, `Oversize` 2, `Pallet` 4, `PickupOnly` 8). Jeden produkt może
teraz jechać kurierem *i* być do odbioru osobistego — wcześniej trzeba było wybrać jedno.
Migracja przepisuje dawną klasę na maskę i **każdemu produktowi dokłada odbiór osobisty**,
bo tak działało to poprzednio dla wszystkich.

**Płatności online przez Stripe** (`Payments:Stripe`) — karta, BLIK i Przelewy24 w PLN, obok
dotychczasowego przelewu tradycyjnego. Domyślnie **wyłączone**: bez `Enabled=true` i kompletu
sekretów checkout w ogóle ich nie pokazuje. Webhook: `POST /api/shop/payments/stripe/webhook`.

### 6.7 Warianty produktu — jeden wpis w sklepie, osobne SKU

Migracja **`ProductVariants`** dodaje `VariantGroupKey` i `VariantLabel`. Sześć grup obejmuje
17 wkładów, które różnią się wyłącznie kolorem lub wykończeniem.

Rozstrzygnięcie, które warto znać: **każdy wariant zostaje osobnym produktem z własnym slugiem,
SKU i ceną**. Grupa jest tylko warstwą prezentacji — przełącznik na karcie produktu prowadzi
pod `/sklep/{slug}` wariantu. Dzięki temu import cen od dostawcy, historia zamówień i panel
działają dalej na płaskiej liście, bez pojęcia „wariantu" w zamówieniu.

SQL migracji przypisuje grupy **po slugach**, a nie po nazwach — literówka w slugu nie wywoła
błędu, tylko cicho niczego nie zaktualizuje. Dlatego powstał `tools/sprawdz-migracje.py`
(patrz sekcja 8).

### 6.8 Koszyk pyta, zamiast przejmować stronę

„Dodaj do koszyka" nie przenosi już od razu do koszyka — pokazuje okno z wyborem
**Kontynuuj zakupy / Przejdź do koszyka**, ze stanem całego koszyka pod spodem.
Skutek uboczny na plus: przeliczenie koszyka po stronie serwera odpala się dopiero wtedy,
gdy ktoś naprawdę wejdzie na `/koszyk`.

## 6b. Co powstało 27 sierpnia 2026

### 6b.1 Konta operatorów zamiast kluczy API

Klucz w nagłówku dzielił się jak hasło i nie dało się go odebrać jednej osobie. Powstał agregat
`Operator` (własna tabela, własny schemat ciasteczka `akhouse.operator`, hasła przez
`PasswordHasher<Operator>`) i cztery niezależne flagi `OperatorPermissions`:
`Leads` (1), `Media` (2), `Shop` (4), `Operators` (8).

Rozstrzygnięcia warte zapamiętania:

- **Uprawnienia czyta się z bazy przy każdym żądaniu**, nigdy z ciasteczka — odebranie dostępu
  działa natychmiast.
- **Personel to `Operator`, nie Identity.** Identity trzyma klientów sklepu; jedna tabela na oba
  rodzaje postawiłaby publiczną rejestrację i uprawnienia do panelu za tymi samymi drzwiami.
- **Klucze przeszły do roli maszynowej** (`Operators:MachineKeys:*`), domyślnie wyłączone, żeby
  `wgraj-ceny.ps1` dalej działał. Żaden z nich nie zarządza kontami.
- Logowanie **wyrównuje czas odpowiedzi** — dla nieznanego adresu też liczy hash — więc nie da się
  z niego wyczytać, które adresy istnieją. Pięć nieudanych prób blokuje konto na 15 minut.
- `UseAuthentication()` uruchamia wyłącznie **domyślny** schemat, więc ciasteczko operatora czyta
  osobne `UseOperatorAuthentication()`. Bez tego panel logował się i natychmiast „zapominał", kto to.

Migracja `OperatorAccounts` (czysto addytywna: tabela + unikalny indeks na `Email`).
Instrukcja uruchomienia i lista endpointów: `KONTA-OPERATOROW.md`.

### 6b.2 Płatność online widoczna, choć jeszcze nieaktywna

Checkout pokazywał sam przelew tradycyjny, co czytało się jak „ten sklep nie przyjmuje kart".
Teraz metoda online jest **widoczna, ale zablokowana**, z dopiskiem „w przygotowaniu", dopóki
bramka Stripe nie zostanie podpięta. Brak informacji o dostępności czyta się po stronie
przeglądarki jako „dostępne" — inaczej starsza wersja API wyszarzałaby wszystko, łącznie
z przelewem, którym można zapłacić od zawsze.

### 6b.3 Przełączanie wariantu bez migotania

Kliknięcie innego koloru zmienia adres, więc karta produktu znikała na moment i wracała —
z ekranu zostawał sam pasek nawigacji. `useShopProduct` **nie wraca już do stanu „ładowanie"**,
gdy coś stoi na ekranie: poprzedni produkt zostaje widoczny i tylko przygasa (`aria-busy`),
a `RouteScrollManager` honoruje `state: { preserveScroll: true }`, więc czytający nie wraca na górę.

### 6b.4 Stan magazynowy i limit sztuk

Do 27.08 górny limit istniał **wyłącznie w przeglądarce** (`max={99}` w polu „Ilość"). To samo
żądanie wysłane z curla przechodziło na dowolną liczbę sztuk. Migracja `ProductStock` przywraca
kolumnę `StockQuantity` — tym razem jako pole **opcjonalne obok** przełącznika dostępności,
a nie zamiast niego:

| Wartość | Znaczenie | Ile klient kupi jednym zamówieniem |
|---|---|---|
| `null` | pozycja bez magazynu — sprowadzana od dostawcy albo sprzedawana na metry | 99 szt. |
| `n > 0` | tyle sztuk stoi na półce | `n` |
| `0` | wyprzedane — znika ze sprzedaży, ale zostaje widoczne w sklepie | 0 |

Wszystkie dotychczasowe 155 pozycji dostaje `NULL`, czyli **dokładnie dotychczasowe zachowanie**.
Magazyn prowadzi się dopiero tam, gdzie ktoś świadomie wpisze liczbę w panelu (pole w formularzu
produktu, przyciski *Ustaw stan* / *Bez magazynu* w operacjach zbiorczych).

- Licznik **schodzi przy złożeniu zamówienia i wraca przy anulowaniu**; cofnięcie anulowania
  rezerwuje go ponownie i kończy się `400`, jeśli w międzyczasie towar zszedł.
- `ShopOrder.Place` sprawdza **cały koszyk, zanim zdejmie pierwszą sztukę** — inaczej odrzucona
  ostatnia pozycja zostawiłaby poprzednie już pomniejszone.
- Nieudana sesja płatności online kasuje zamówienie **razem ze zwrotem towaru na półkę**.
- Koszyk (`POST /api/shop/cart/validate`) **przycina** ilość do stanu i mówi o tym klientowi,
  zamiast przepuszczać 400 sztuk aż do kasy.
- Karta produktu pokazuje „Zostały 2 sztuki" dopiero **przy stanie ≤ 5** — liczba ma pomóc podjąć
  decyzję, a nie wywołać popłoch.

Znane ograniczenie: rezerwacja **nie ma blokady optymistycznej**. Dwa zamówienia złożone w tej
samej sekundzie mogą odczytać ten sam stan i zejść poniżej zera. Przy dzisiejszym ruchu to ryzyko
teoretyczne, ale przed kampanią z ograniczoną serią warto dołożyć znacznik współbieżności.

### 6b.5 Zamówienia: filtry i kolory statusów

Zakładka pokazywała wszystko jednym ciągiem — świeże zapytanie o wpłatę stało obok zamówienia
anulowanego trzy tygodnie temu i niczym się od niego nie różniło.

- **Rząd plakietek nad listą** jest jednocześnie podsumowaniem i filtrem: „Oczekuje na wpłatę 4",
  „Anulowane 1". Liczby idą z **całej** listy, nie z przefiltrowanej — licznik, który zmieniałby
  się od własnego filtra, nie mówiłby nic o stanie sklepu. Status bez ani jednego zamówienia
  zostaje widoczny, tylko wygaszony, więc nie pada pytanie „gdzie się podziały anulowane".
- **Kolory statusów** (`orderStatusColors` w `app/theme.ts`) — jedyne miejsce, gdzie paleta wychodzi
  poza brązy, bo odcienie jednego brązu nie odróżnią „czeka na wpłatę" od „anulowane". Każdy kolor
  niesie znaczenie: bursztyn = wymaga działania, zieleń = pieniądze wpłynęły, morski = w przygotowaniu,
  błękit = jedzie do klienta, szarość = zamknięte, czerwień = anulowane. Te same kolory są
  w plakietkach filtrów, więc legenda nie musi istnieć osobno.
- **Wyszukiwarka** obejmuje numer, klienta, e-mail, telefon, miasto, NIP **i nazwy kupionych
  pozycji** — „kto zamówił tę grzałkę" pada częściej niż „pokaż SKL-2026-3F9A".
- Filtry płatności (online / przelew) i sortowanie po dacie albo kwocie; nagłówek pokazuje sumę
  tego, co widać.
- **Znacznik „nowe"** (`IsRead`) był w bazie i w API od dawna, ale nic go nie ustawiało — teraz
  panel ma przycisk *Oznacz jako obsłużone*, filtr „Nieprzeczytane" i brązowy pasek przy karcie.
  To celowo **osobna oś od statusu**: status opisuje zamówienie, znacznik — czy ktoś się nim zajął.
  Oznaczenie jest jawnym kliknięciem; automatyczne czyszczenie kolejki przy samym wyrenderowaniu
  listy zabrałoby jedyną informację o tym, czego jeszcze nikt nie widział.

### 6b.6 Adresy paneli rozdzielone

`/admin` nazywało obszar, który przestał być jednym obszarem. Każde narzędzie ma teraz własną
trasę najwyższego poziomu:

| Było | Jest | Co to |
|---|---|---|
| `/admin` | **`/crm`** | Zlecenia na domki, sauny i meble — **nie** sklep |
| `/admin/zdjecia` | **`/zdjecia`** | Zdjęcia strony i galeria realizacji |
| `/admin/operatorzy` | **`/operatorzy`** | Konta i uprawnienia |
| `/sklep/panel` | bez zmian | Sklep |

Zdjęcia obsługują całą stronę, nie CRM, więc nie miały po co pod nim wisieć. **Przekierowania
ze starych adresów celowo nie ma** — `/admin` trafia na 404, zamiast cicho działać dalej.
Zakładki trzeba zaktualizować.

Przy okazji: pływający dymek WhatsApp znikał dotąd tylko pod `/admin*`, więc wisiał nad panelem
sklepu i zasłaniał przyciski w prawym dolnym rogu. Teraz zna wszystkie cztery trasy wewnętrzne.

### 6b.7 Jeden przycisk wylogowania

Cztery panele dorobiły się czterech brzmień tego samego przycisku: „Wyloguj", „Zamknij sesję",
„Wyloguj (Tomasz)" i „Wyloguj się" w bramie. Powstał wspólny
`features/admin/auth/SignOutButton.tsx`, który trzyma **treść**; wygląd zostaje przy panelu,
bo każdy ma własny system przycisków (`ak-btn-outline`, `ak-media-btn`, `ak-cat-btn`).

Etykieta brzmi wszędzie **„Wyloguj (Imię)"**. Nazwa konta jest w niej celowo: panele bywają
otwierane na wspólnym komputerze w biurze i „kim jestem w tej karcie" to pytanie, które lepiej
mieć odpowiedziane, zanim ktoś skasuje produkt. Sklep zachował własną obsługę wylogowania
(`onSignOut`), bo wywołuje ją też po odmowie 401.

### 6b.8 Naprawione przy okazji

- `GET /api/admin/auth/me` odsyłał **puste ciało** zamiast `null` (helper `Results.Ok` krótkuje
  na wartości null) — mimo że kontrakt tego endpointu brzmi „zwracam null, gdy nikt nie jest zalogowany".
- Trzy testy zostały po zmianach z 27.08 z nieaktualnymi oczekiwaniami (płatność online,
  porównywanie ProblemDetails razem z unikalnym `traceId`, panel zdjęć bez `MemoryRouter`).
- **Build w konfiguracji Release nie przechodził** — dwa ostrzeżenia `CS8602` w testach, a Release
  traktuje ostrzeżenia jak błędy. Naprawione; `zweryfikuj.ps1` ma teraz czystą drogę.

## 7. Reguły, których nie wolno złamać

Pełna lista w `CLAUDE.md`. Najkosztowniejsze w skrócie:

- **Pieniądze to grosze w `int`**, narzut to punkty bazowe w `int`. Nigdy `decimal` w bazie.
- **Cenę liczy serwer.** Z przeglądarki przychodzą wyłącznie `productId` i `quantity`.
- **Pozycja zamówienia trzyma kopię nazwy i ceny** — zmiana cennika nie przepisuje historii.
- **Każdy endpoint administracyjny musi mieć `.RequireOperator(...)`.** Brak bramy = publiczny
  dostęp do danych klientów.
- **Cztery uprawnienia zostają osobne**, a czyta się je z bazy przy każdym żądaniu — nie z ciasteczka.
- **Żaden klucz maszynowy nie zarządza kontami** — mógłby nadać sobie każde inne uprawnienie.
- **Sesja klienta to ciasteczko `HttpOnly`**, nie token w `localStorage`.
- **Wariant to osobny produkt, nie pozycja w zamówieniu.** `VariantGroupKey` jest warstwą
  prezentacji; zamówienia, import cen i panel pracują na płaskiej liście SKU.
- **`StockQuantity` jest opcjonalny, a `null` znaczy co innego niż `0`.** `null` = pozycja bez
  magazynu z limitem 99 szt. na zamówienie; `0` = wyprzedane. Limitu pilnuje `ShopOrder.Place`,
  a nie pole „Ilość" w przeglądarce ani przycinanie koszyka — jedno i drugie jest uprzejmością
  wobec klienta, nie zabezpieczeniem.
- **`ShippingClasses` to maska bitowa**, nie pojedyncza wartość. Produkt bez ustawionego
  `PickupOnly` staje się nieodbierany osobiście — sprawdź to, zanim zdejmiesz ten bit.
- **Płatności online muszą dać się wyłączyć.** Bez `Payments:Stripe:Enabled` i kompletu sekretów
  checkout nie pokazuje ich wcale — to nie jest tryb awaryjny, tylko stan domyślny.
- **Publiczny endpoint przyjmujący dane** = zgoda RODO + honeypot + rate-limit per IP. Wszystkie trzy.
- **SVG w uploadzie jest odrzucany celowo** (wykonuje JavaScript po otwarciu wprost). JPG/PNG/WEBP/AVIF do 8 MB.
- **Konfigurator 3D jest zawieszony** — kod zostaje, nie prowadzi do niego trasa, nie rozwijaj go bez decyzji.
- Sekrety nigdy do repo: `Operators:Bootstrap:Password`, `Operators:MachineKeys:*`,
  `Email:SmtpPassword`, `Shop:BankAccountNumber`, `Payments:Stripe:SecretKey`,
  `Payments:Stripe:WebhookSecret`.

### Pułapki środowiska (kosztowały czas)

- **Nie uruchamiaj `git` przez powłokę zdalną** w tym projekcie — zostawia pliki `index.lock`,
  których ten mount nie pozwala usunąć, i psuje natywnego gita.
- **`rm` nie działa na zamontowanym katalogu** („Operation not permitted") — używaj `mv` do `_to_delete/`.
- W PowerShellu kontynuacja wiersza to **backtick**, nie `^` (to CMD).
- `dotnet user-secrets` **wymaga `--project`**, inaczej szuka `.csproj` w bieżącym katalogu.
- **`dotnet ef database update` nie sprawdza, czy migracja cokolwiek zmieniła.** `UPDATE … WHERE`
  z błędnym slugiem kończy się sukcesem i zerem wierszy — patrz sekcja 8.

## 8. Co zweryfikowane, a co nie

| | Stan |
|---|---|
| Backend: `dotnet build` (Debug i Release) | ✅ przechodzi |
| Backend: `dotnet test` | ✅ **232/232** (uruchomione w Debug i Release) |
| Frontend: `typecheck`, `test`, `lint` | ✅ **164/164**, 0 błędów lint (40 zastanych ostrzeżeń) |
| Migracje od zera na świeżej bazie | ✅ testy integracyjne startują aplikację, a ta wykonuje `MigrateAsync` na pustym SQLite — cała ścieżka 24 migracji przechodzi przy każdym uruchomieniu |
| SQL migracji wariantów na realnych danych | ✅ `tools/sprawdz-migracje.py` — wszystkie sprawdzenia |
| Frontend: `npm run build` | ✅ przechodzi |

Testy backendu, przez pierwsze dwa tygodnie pisane „na ślepo" (NuGet był zablokowany
w środowisku roboczym), zostały uruchomione 27.08 i przechodzą. Obejście, gdyby przydało się
ponownie: biblioteki EF Core i xUnit dają się wziąć z `backend/**/bin/Debug/net10.0` po lokalnym
buildzie i podłączyć przez `<Reference>` zamiast `<PackageReference>`, a `MvcTestingAppManifest.json`
trzeba wtedy napisać ręcznie, żeby `WebApplicationFactory` znalazło katalog treści.

### Dlaczego SQL migracji ma osobny test

`dotnet ef database update` powie, czy migracja **się wykonała** — nie powie, czy **coś zrobiła**.
`UPDATE … WHERE "Slug" = 'literówka'` kończy się sukcesem i zmienia zero wierszy. Przy
`ProductVariants` oznaczałoby to sklep bez wariantów, wykryty dopiero przez klienta.

`tools/sprawdz-migracje.py` wycina operacje wprost z plików migracji (nie przepisuje ich
ręcznie), odtwarza je **w kolejności ze źródła** na pustej bazie SQLite wypełnionej realnym
`shop-catalog.json` i sprawdza wynik. Kolejność jest istotna: `ShippingClass` znika dopiero po
tym, jak SQL przepisze z niej maski. Potwierdzone:

- wszystkie **17 slugów istnieje** — żadne trafienie w próżnię;
- powstaje **6 grup**, żadna z jednym wariantem, każdy wariant ma unikalną etykietę;
- każda ze 155 pozycji dostała maskę zgodną ze swoją dawną klasą, a **odbiór osobisty**
  pozostał możliwy dla wszystkich;
- `Down()` obu migracji odtwarza stan wyjściowy co do wiersza.

Uruchomienie: `python tools/sprawdz-migracje.py` z katalogu głównego projektu.

## 9. Otwarte punkty

### Techniczne — do zrobienia najbliżej

0. **WYMIENIĆ KLUCZ z pliku `klucz upload/Nowy Dokument tekstowy.txt`.** Plik był zacommitowany
   i wypchnięty na GitHub — siedzi w historii, więc `git rm --cached` go stamtąd nie usunie.
   Jedyne skuteczne rozwiązanie to wygenerowanie nowego `Admin:MediaApiKey`.
1. **Uruchomić `zweryfikuj.ps1`** — cała bramka przeszła zdalnie (build Release, 232 testy
   backendu, migracje od zera, typecheck, 148 testów frontendu, lint, build). Skrypt warto puścić
   lokalnie, żeby potwierdzić to samo na Windows, na prawdziwym `dotnet ef`.
2. **Uruchomić `posprzataj.ps1`** — śmieci są już zebrane w `_do-usuniecia/` (115 MB), ale zdalna
   powłoka nie ma prawa kasować plików. Skrypt kasuje ten katalog, wynosi `elementy/` do
   `..\ak-house-materialy` i wypisuje z gita to, co przestało być częścią projektu.
   Podgląd bez zmian: `posprzataj.ps1 -NaSucho`.
3. **Ujednolicić etykiety wariantów.** Te same trzy wykończenia nazywają się inaczej w każdej
   z trzech grup włókna szklanego: `fiberglass-round` → Biały / Perła / Szary Granitcoat,
   `fiberglass-rectangle` → Grafit / Grafit perła / Biały Granitcoat, `fiberglass-lounger` →
   Standard / Biała perła / **Czarna perła** (tu informacja o granitcoacie ginie zupełnie).
   Klient przełączający się między grupami zobaczy trzy różne słowniki na to samo.
4. **`acrylic-round-200-jets` ma dwa warianty w tej samej cenie** (3 839,81 zł). Przełącznik
   zadziała, ale ceny nie ruszy — u Balii istnieje tylko wersja „szara perła" (3 899 zł),
   białą dopisaliście sami. Do potwierdzenia, czy ta cena jest właściwa.
5. ~~Zastąpić klucze API kontami operatorów.~~ **Zrobione 27.08** — sekcja 6b.1.
6. **Znacznik współbieżności na `ShopProducts`** przed pierwszą kampanią z ograniczoną serią:
   rezerwacja stanu nie ma dziś blokady optymistycznej (sekcja 6b.4).
7. **Usunąć `Operators:MachineKeys:*` z konfiguracji**, gdy `wgraj-ceny.ps1` przestanie być
   potrzebny — im mniej poświadczeń, które da się skopiować, tym lepiej.

### Ceny — 87 produktów wciąż bez ceny detalicznej

30 wątpliwych + 57 bez dopasowania (jedna pozycja zniknęła z katalogu przy scalaniu wariantów). Pięć pytań czeka na rozstrzygnięcie:

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

Pełna weryfikacja przed commitem — build Release, testy, migracje na świeżej bazie, frontend:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File zweryfikuj.ps1
```

Skrypt przerywa na pierwszym błędzie i mówi, który etap padł. Pojedynczy etap da się pominąć
(`-Pomin frontend`), a poszczególne komendy są w `CLAUDE.md`. Gdy wszystko zielone:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File zacommituj.ps1 -NaSucho
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
