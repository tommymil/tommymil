# Stan prac — A.K. HOUSE

Aktualizacja: **12 sierpnia 2026**

## Sklep internetowy — zrealizowany

Dodano osobną zakładkę **Sklep** na gotowe produkty i akcesoria, niezależną od lejka zapytań
ofertowych. Zakres i decyzje opisuje README (sekcje „Sklep"). W skrócie:

- katalog sklepu z bazy (`ShopProduct`, `ShippingMethod`), strony `/sklep` i `/sklep/:slug`;
- koszyk w `localStorage` (`akhouse.cart.v1`) z przeliczaniem cen po stronie serwera;
- checkout z płatnością przelewem tradycyjnym, numerem `SKL-…` i mailem potwierdzającym;
- konta klientów na ASP.NET Core Identity (ciasteczko `HttpOnly`), zakup jako gość nadal możliwy;
- zakładka **Sklep** w panelu `/admin`: asortyment ze zdjęciami, metody dostawy, obsługa zamówień;
- wzorce `regulamin.html` i `zwroty.html` + akceptacja regulaminu wymagana przy zamówieniu.

Kluczowe decyzje techniczne: **kwoty w groszach jako `int`** (SQLite zapisuje `decimal` jako TEXT),
**cena zawsze liczona na serwerze** (z przeglądarki przychodzą tylko `productId` i `quantity`),
**snapshot nazwy i ceny w pozycji zamówienia** (zmiana cennika nie przepisuje historii).

Nowe migracje: `AddShopCatalog`, `AddShopOrders`, `AddCustomerAccounts`.

Testy po zmianach: backend **149/149**, frontend **106/106**, produkcyjny build frontendu poprawny.

### Do uzupełnienia przed uruchomieniem sprzedaży

- `Shop:BankAccountNumber` (oraz nazwa banku) — bez tego mail nie zawiera danych do przelewu.
- `Shop:SiteBaseUrl` na produkcji — używany w linku resetu hasła.
- Weryfikacja prawna `regulamin.html` i `zwroty.html` oraz uzupełnienie pól `[w nawiasach]`.
- Realny asortyment: produkty, ceny, stany magazynowe i zdjęcia (panel `/admin` → Sklep).
- Stawki dostawy — seed zakłada kurier 25 zł (gratis od 500 zł), paleta 199 zł, odbiór osobisty 0 zł.

---

## Stan wcześniejszy (29 lipca 2026)

## Status ogólny

Projekt jest w aktywnym stanie roboczym. Zmiany nie zostały jeszcze zapisane w commicie Git.
Frontend i backend mają lokalne, niezatwierdzone modyfikacje. Ostatnia pełna walidacja zakończyła
się powodzeniem:

- frontend: **30/30 testów**,
- backend: **58/58 testów**,
- produkcyjny build frontendu: poprawny,
- migracje EF Core: poprawne i sprawdzone testami integracyjnymi.

## Zrealizowane prace

### 1. Dane firmy i podstawy produkcyjne

- Dane rejestrowe firmy zostały scentralizowane we frontendzie:
  - A.K. HOUSE SPÓŁKA Z OGRANICZONĄ ODPOWIEDZIALNOŚCIĄ,
  - KRS 0001121503,
  - NIP 7182170992,
  - REGON 529386091,
  - Mątwica 79C, 18-414 Mątwica.
- Informacja `12+` pozostaje celowo bez zmian, zgodnie z ustaleniem — będzie poprawiona osobno.
- Polityki prywatności i dane kontaktowe korzystają ze wspólnego źródła danych.
- Publiczne formularze mają zgodę, honeypot oraz ograniczenie liczby zgłoszeń.
- Dostęp administracyjny jest chroniony nagłówkiem `X-Api-Key`; sekret nie jest wpisany do repozytorium.

### 2. Wspólny lejek sprzedażowy i panel `/admin`

- Formularz kontaktowy, kreator wyceny i zgłoszenia ręczne trafiają do jednego lejka leadów.
- Zachowane są źródło zgłoszenia, numer zapytania, budżet, termin realizacji, lokalizacja,
  preferowany kanał kontaktu i konfiguracja produktu.
- Istnieją widoki: `Na dziś`, `Pulpit`, `Tablica` i `Lista`.
- Obsługiwane są etapy lejka, priorytet, wartość, notatki, wysyłka e-maila, historia zmian
  i oznaczenie przeczytania.
- Migracja `UnifySalesPipeline` przenosi historyczne zamówienia do wspólnego lejka.

### 3. Ostatni kontakt i następne działanie

- Rozdzielono trzy niezależne informacje:
  - ostatnia aktywność przy zgłoszeniu,
  - ostatni rzeczywisty kontakt z klientem,
  - następny zaplanowany kontakt lub działanie.
- Dodano akcję **„Kontakt wykonany”**, która:
  - zapisuje datę kontaktu,
  - dodaje wpis do historii,
  - rozlicza poprzednie przypomnienie,
  - opcjonalnie zapisuje notatkę i kolejny termin.
- Wysłanie e-maila aktualizuje datę ostatniego kontaktu.
- Oznaczenie zgłoszenia jako przeczytane nie zmienia daty ostatniej aktywności.
- Dodano endpoint `POST /api/leads/{id}/contacts`.
- Dodano migrację `AddLeadLastContact` z polem `LastContactAt`.
- Lista pokazuje osobno ostatni kontakt, ostatnią aktywność, następne działanie i datę wpływu.
- Dostępne sortowania:
  - najdawniej obsługiwane — domyślne,
  - najdawniej kontaktowane,
  - najbliższe działanie,
  - ostatnio aktualizowane,
  - najnowsze zgłoszenia,
  - najstarsze zgłoszenia.

### 4. Katalog, kategorie i produkty

Gotowe są polskie i angielskie trasy:

- `/produkty`,
- `/domki-drewniane`,
- `/domki-drewniane/domek-28`,
- `/sauny-ogrodowe`,
- `/sauny-ogrodowe/sauna-panorama-12`,
- `/kuchnie-na-wymiar`,
- `/kuchnie-na-wymiar/kuchnia-indywidualna`,
- lustrzane wersje pod `/en/...`.

Strony zawierają korzyści, zakres bazowy, opcje dodatkowe, dane techniczne, informacje
organizacyjne, FAQ, kreator wariantu (SVG, bez 3D) oraz przejście do wyceny. Konfigurator 3D
jest zawieszony i nie jest wystawiany — patrz README. Ceny są pokazywane tylko tam,
gdzie istniały wcześniej potwierdzone wartości konfiguracyjne:

- Domek 28 — 129 000 zł,
- Sauna Panorama 12 — 49 000 zł,
- kuchnia — wycena indywidualna po pomiarze.

Nie dodano niepotwierdzonych ocen, certyfikatów, obietnic prawnych ani sztucznych cen.

### 5. SEO katalogu

- Dodano canonicale, hreflang PL/EN i `x-default`.
- Dodano Open Graph i Twitter Cards.
- Kategorie mają dane strukturalne `CollectionPage`.
- Produkty mają `Product`, a `Offer` tylko przy rzeczywistej cenie.
- Sitemap zawiera wszystkie nowe trasy.
- Build generuje 14 statycznych stron wejściowych dla katalogu PL/EN.
- Katalog jest ładowany jako osobne chunki.

### 6. Nawigacja i przewijanie

- Nawigacja strony głównej prowadzi bezpośrednio do kategorii i wyceny.
- Dodano globalny mechanizm resetowania pozycji przewijania przy każdej zwykłej zmianie trasy.
- Przejścia np. `/produkty` → `/domki-drewniane` zaczynają się od samej góry.
- Deep-linki z hashem, np. `/#kontakt` i `/realizacje#galeria-pelna`, nadal przewijają do sekcji.
- Poprawiono konflikt programowego przewijania z wykrywaniem ruchu użytkownika w realizacjach.

## Ważne pliki i migracje

- katalog: `frontend/src/features/catalog/`,
- routing: `frontend/src/App.tsx`,
- globalny scroll: `frontend/src/components/RouteScrollManager.tsx`,
- panel administracyjny: `frontend/src/features/admin/`,
- model leada: `backend/src/AkHouse.Domain/Leads/Lead.cs`,
- API leadów: `backend/src/AkHouse.Api/Endpoints/LeadEndpoints.cs`,
- migracje:
  - `20260729152741_UnifySalesPipeline`,
  - `20260729163511_AddLeadLastContact`.

## Rzeczy wymagające uzupełnienia przed publikacją

- Podmienić tymczasowy numer telefonu, WhatsApp i profile społecznościowe na rzeczywiste dane.
- Potwierdzić produkcyjny adres e-mail i konfigurację SMTP.
- Ustawić `Admin:ApiKey` poza repozytorium, np. przez `dotnet user-secrets` lub zmienną środowiskową.
- Dostarczyć pełne galerie zdjęć, rzuty, warianty i zatwierdzone specyfikacje produktów.
- Zatwierdzić handlowo ceny bazowe, zakresy i dopłaty.
- Osobno poprawić pozostawioną informację `12+`.
- Zoptymalizować ciężki chunk `ConfiguratorScene` — obecnie około 956 kB przed gzipem.
- Przed commitem przejrzeć pliki lokalnej bazy `akhouse.db`, `-shm`, `-wal` i nie dodawać
  danych roboczych do wersji produkcyjnej bez świadomej decyzji.

## Polecenia kontrolne

```powershell
cd frontend
npm.cmd test -- --run
npm.cmd run build

cd ../backend
dotnet test AkHouse.slnx -c Release --no-restore
```

Migracje są uruchamiane automatycznie podczas startu backendu.
