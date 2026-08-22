# Wycena projektu — A.K. HOUSE

**Dokument pomocniczy do ustalenia uczciwej ceny (dla obu stron).**
Data: 12.08.2026 (zastępuje wersje z 23.07.2026 i 29.07.2026)

> Zastrzeżenie: to oszacowanie oparte na realnym zakresie kodu i typowych
> polskich stawkach. Ma być podstawą do rozmowy, a nie „wyceną urzędową".
> Kwoty w PLN **netto**, o ile nie zaznaczono inaczej.

---

## 0. Stan faktyczny repozytorium (12.08.2026)

| Metryka | 29.07.2026 | **Dziś** |
|---|---|---|
| Kod produkcyjny (`backend/src` + `frontend/src`) | ~18 100 linii | **~22 700 linii** |
| Kod testów | (w powyższym) | **~4 300 linii** |
| Pliki źródłowe | 183 | **255** |
| Moduły frontendu | 7 | **11** |
| Endpointy API | — | **61** |
| Trasy frontendu | — | **35** |
| Migracje bazy | — | **17** |
| Testy automatyczne | 88 | **252** (145 backend + 107 frontend) |

Stos: C# / .NET 10 (Clean Architecture, EF Core, xUnit) + React 19 / TypeScript
(Vite, MVVM), SQLite wymienialne na PostgreSQL, ASP.NET Core Identity.

**Wzrost od ostatniej wyceny to nie kosmetyka** — doszedł kompletny sklep
internetowy z zamówieniami i kontami klientów, moduł mediów oraz import
asortymentu z cennika dostawcy. Liczba testów wzrosła prawie trzykrotnie.

---

## 1. Co realnie zawiera projekt

To nie jest „strona wizytówka" — to cztery produkty w jednym.

### a) Strona prezentacyjna
Kilkanaście sekcji, dwujęzyczna (PL / EN), animowane tło 3D, parallax, liczniki.
Responsywność, modale, galeria, lightbox. SEO: `robots.txt`, `sitemap.xml`,
Open Graph / Twitter, JSON-LD `LocalBusiness`. Dostępność: focus-trap, `Esc`,
`aria-invalid`, `prefers-reduced-motion`.

### b) Katalog produktów
Trasy PL i EN dla kategorii i produktów (domki, sauny, kuchnie), 9 stron
generowanych statycznie przy buildzie dla SEO, canonicale i hreflang,
dane strukturalne `CollectionPage` / `Product` / `Offer`, kreator wariantu
w SVG, przejścia do wyceny.

### c) Backend z prawdziwą architekturą
Clean Architecture (Domain → Application → Infrastructure → Api), EF Core,
17 migracji, seeder, 61 endpointów. Walidacja RODO, honeypot, limit zgłoszeń
na IP, wysyłka e-maili SMTP. Testy jednostkowe domeny i integracyjne przez
`WebApplicationFactory` — **145 testów backendu**.

### d) Panel CRM dla studia (`/admin`)
Widok „Na dziś", pulpit KPI, lejek, tablica kanban z przeciąganiem, panel
szczegółów leada, oś czasu działań, wysyłka wiadomości z szablonami, ręczne
dodawanie zleceń, rozróżnienie ostatniego kontaktu i następnego działania.

### e) Sklep internetowy — **nowe od ostatniej wyceny**
- katalog z bazy, strony `/sklep` i `/sklep/:slug`, filtry kategorii,
  wyszukiwanie i sortowanie, powiększanie zdjęć;
- koszyk w `localStorage` z **przeliczaniem cen po stronie serwera** —
  z przeglądarki przychodzą wyłącznie `productId` i `quantity`;
- checkout: przelew tradycyjny, numer `SKL-…`, mail potwierdzający,
  wymagana akceptacja regulaminu, wzorce `regulamin.html` i `zwroty.html`;
- **konta klientów** na ASP.NET Core Identity (ciasteczko `HttpOnly`), reset
  hasła, historia zamówień, dane do wysyłki — zakup jako gość nadal możliwy;
- **zakładka Sklep w panelu**: asortyment ze zdjęciami, metody dostawy,
  obsługa zamówień ze statusami i osią czasu;
- snapshot nazwy i ceny w pozycji zamówienia — zmiana cennika nie przepisuje
  historii sprzedaży.

### f) Moduł mediów i import asortymentu — **nowe**
Upload zdjęć do slotów strony i galerii z poziomu panelu. Skrypt importu
hurtowego cennika XLSX: **156 pozycji, 13 kategorii, 156 zdjęć**
przekonwertowanych z 240 MB do 13 MB WebP, z automatycznym przeliczeniem
marży i VAT.

### g) Konfigurator 3D — **zawieszony**
Szkielet i architektura gotowe, kod zostaje w repozytorium, ale nie jest
wystawiany publicznie. Pełna implementacja z silnikiem wyceny (CPQ) to
osobny etap.

---

## 2. Nakład pracy

| Moduł | Godziny |
|---|---|
| Strona prezentacyjna (2 języki, animacje, SEO, a11y) | 55 – 75 |
| Katalog produktów (trasy PL/EN, prerender, kreator SVG) | 35 – 50 |
| Backend: architektura, baza, migracje, e-mail, testy | 45 – 60 |
| Panel CRM (kanban, pulpit, lejek, oś czasu, e-maile) | 60 – 85 |
| Kreator zamówienia | 15 – 25 |
| Sklep: katalog, koszyk, checkout, zamówienia, maile | 70 – 100 |
| Konta klientów (Identity, reset hasła, historia) | 25 – 35 |
| Zakładka Sklep w panelu administracyjnym | 30 – 45 |
| Moduł mediów i galerii | 15 – 25 |
| Import cennika dostawcy | 12 – 20 |
| Konfigurator 3D (obecny szkielet) | 25 – 40 |
| **Razem** | **390 – 560 h** |

Kontrola z drugiej strony: ~27 000 linii kodu produkcyjnego i testów przy
tempie 55 – 70 linii na godzinę pracy o jakości produkcyjnej daje 385 – 490 h.
Obie metody spotykają się w okolicy **400 – 520 h**.

---

## 3. Widełki rynkowe — obecny stan projektu

| Kto wycenia | Stawka | Za obecny stan |
|---|---|---|
| Software house | 300 – 450 zł/h | **120 000 – 235 000** |
| Freelancer senior | 150 – 220 zł/h | **60 000 – 115 000** |
| Realna cena rynkowa dla MŚP | — | **45 000 – 75 000** |
| Cena „po znajomości" | 60 – 90 zł/h | **25 000 – 47 000** |

### Wartość odtworzenia — gdyby zamawiać moduł po module

| Element | Wartość rynkowa (PLN) |
|---|---|
| Strona prezentacyjna | 8 000 – 18 000 |
| Katalog produktów z SEO | 6 000 – 12 000 |
| Backend + baza + e-mail + testy | 6 000 – 15 000 |
| Panel CRM | 12 000 – 30 000 |
| Kreator zamówienia | 3 000 – 6 000 |
| **Sklep internetowy** | **15 000 – 35 000** |
| **Konta klientów** | **5 000 – 12 000** |
| **Panel sklepu w `/admin`** | **8 000 – 18 000** |
| Moduł mediów i galerii | 3 000 – 7 000 |
| Import cennika dostawcy | 3 000 – 6 000 |
| Konfigurator 3D (szkielet) | 5 000 – 12 000 |
| **Razem jako projekt komercyjny** | **~74 000 – 171 000** |

Obie metody — stawka godzinowa i suma modułów — wskazują ten sam przedział:
**realna wartość rynkowa to 70 000 – 150 000 zł**.

---

## 4. Rekomendowany model rozliczenia

| Pozycja | Kwota |
|---|---|
| **Budowa obecnego stanu** (kotwica negocjacyjna) | **30 000 zł** jednorazowo — z miejscem do zejścia do 26 000 |
| Wdrożenie produkcyjne (hosting, domena, HTTPS, SMTP, kopie zapasowe, utwardzenie logowania) | 5 000 – 9 000 zł — lub w pakiecie przy pełnej cenie |
| **Utrzymanie i wsparcie** | **600 – 1 200 zł / mies.** |
| Konfigurator 3D + CPQ | osobna umowa, etap 2 |

Poprzednia kotwica wynosiła 18 000 zł przy 200 – 300 h. Nakład wzrósł
około 1,7 raza, stąd 30 000 zł — to ta sama stawka za godzinę, nie podwyżka.

Abonament utrzymaniowy wzrósł, bo **sklep to inna klasa odpowiedzialności niż
strona**: awaria zamówień albo maili to utracone pieniądze klienta tego samego
dnia, a nie odłożona niedogodność.

---

## 5. Co jeszcze zostało do produkcji

- [ ] Wdrożenie: hosting, domena, HTTPS, konfiguracja SMTP.
- [ ] Utwardzenie logowania do panelu (dziś prosty klucz `X-Api-Key`; konta
      klientów mają już pełne Identity, panel administracyjny nie).
- [ ] `Shop:BankAccountNumber` i nazwa banku — bez tego mail nie zawiera danych
      do przelewu, więc sprzedaż nie ruszy.
- [ ] `Shop:SiteBaseUrl` na produkcji — używany w linku resetu hasła.
- [ ] Weryfikacja prawna `regulamin.html` i `zwroty.html`, uzupełnienie pól
      `[w nawiasach]`.
- [ ] Prawdziwe zdjęcia własnych realizacji i finalna treść.
- [ ] Panel CMS do edycji treści (treść jest w bazie, ale bez UI do edycji).
- [ ] Weryfikacja marży i cen zaimportowanego asortymentu przed publikacją.
- [x] ~~Sklep internetowy~~ — **zrobione**.
- [x] ~~Asortyment z cennika dostawcy~~ — **zrobione**.

Stąd wybór: albo cena bliżej dolnej granicy „jako MVP do dokończenia", albo
cena wyższa, ale z tymi pozycjami w pakiecie.

---

## 6. Możliwe kolejne etapy (osobne wyceny)

| Etap | Wartość |
|---|---|
| Płatności online (Przelewy24 / Stripe) zamiast przelewu | 6 000 – 12 000 |
| Panel CMS do edycji treści strony | 8 000 – 15 000 |
| Integracja z kurierem: etykiety, śledzenie przesyłek | 5 000 – 10 000 |
| Angielska wersja sklepu (cennik dostawcy ma gotowy arkusz EN) | 5 000 – 9 000 |
| Konfigurator 3D + CPQ (pełna implementacja) | 25 000 – 60 000 |

---

## 7. Ustalenia, które trzeba spisać (jedna strona A4)

„Uczciwie" to nie tylko kwota — to jasne ustalenie, **co wchodzi w cenę**:

- Czy w cenie jest wdrożenie i konfiguracja, czy osobno.
- Ile rund poprawek jest wliczonych.
- Kto płaci za hosting / domenę / SMTP (koszty cykliczne właściciela).
- Czy jest utrzymanie i wsparcie, w jakim zakresie i z jakim czasem reakcji —
  przy sklepie to najważniejszy punkt umowy.
- Że klient otrzymuje **własność kodu** (to backend, sklep i panel, nie szablon).
- Kto odpowiada za zgodność prawną sklepu: regulamin, zwroty, RODO, ceny.
- Co **nie** wchodzi: konfigurator 3D, CMS, płatności online, zdjęcia, treści
  marketingowe.

---

## 8. Podsumowanie jednym zdaniem

Rynkowo praca warta jest **70 000 – 150 000 zł** (a w software housie nawet
235 000); uczciwa cena po znajomości za obecny stan to **26 000 – 35 000 zł**
jednorazowo (kotwica: **30 000**), plus **600 – 1 200 zł/mies.** utrzymania —
a płatności online, CMS i konfigurator 3D z CPQ to osobne etapy warte łącznie
kolejne **39 000 – 87 000 zł**.
