# Wycena projektu — A.K. HOUSE

**Dokument pomocniczy do ustalenia uczciwej ceny (dla obu stron).**
Data: 29.07.2026 (aktualizacja wersji z 23.07.2026)

> Zastrzeżenie: to oszacowanie oparte na realnym zakresie kodu i typowych
> polskich stawkach. Ma być podstawą do rozmowy, a nie „wyceną urzędową".
> Kwoty w PLN **netto**, o ile nie zaznaczono inaczej.

---

## 0. Stan faktyczny repozytorium (29.07.2026)

| Metryka | Wartość |
|---|---|
| Kod produkcyjny (`backend/src` + `frontend/src`) | **~18 100 linii** |
| Liczba plików źródłowych | **183** |
| Backend | C# / .NET 10, Clean Architecture, EF Core, xUnit |
| Frontend | React + TypeScript (Vite), MVVM, 7 modułów funkcjonalnych |
| Moduły frontendu | `landing`, `admin`, `configurator`, `order`, `realizations`, `consent`, `notfound` |
| Repozytorium git | **istnieje** |
| Szacowany nakład pracy | **200 – 300 h** pracy o jakości produkcyjnej |

---

## 1. Co realnie zawiera projekt

To nie jest „strona wizytówka" — to trzy produkty w jednym:

### a) Strona prezentacyjna
- Kilkanaście sekcji (hero, oferta, atuty, proces, realizacje, galeria,
  finansowanie, opinie, o firmie, FAQ, kontakt, stopka).
- Dwujęzyczna (PL / EN), animowane tło 3D (plaster miodu), parallax,
  liczniki statystyk.
- Responsywność (mobile / tablet / desktop), modale, galeria, lightbox.
- SEO: `robots.txt`, `sitemap.xml`, Open Graph / Twitter, dane strukturalne
  (JSON-LD LocalBusiness), favicon marki.
- Dostępność (a11y): powiązane label/input, `aria-invalid`, focus-trap,
  obsługa `Esc`, `prefers-reduced-motion`.

### b) Backend z prawdziwą architekturą
- C# / .NET 10, Clean Architecture (Domain → Application → Infrastructure → Api).
- EF Core (SQLite, wymienialne na PostgreSQL jedną linijką), migracje, seeder.
- Formularz leada: walidacja RODO, honeypot anty-spam, limit zgłoszeń/IP.
- Wysyłka e-maili przez SMTP (w dev logowanie).
- Testy jednostkowe (domena) i integracyjne (WebApplicationFactory).

### c) Panel CRM dla studia (`/admin`) — największa ukryta wartość
- Widok „Na dziś", pulpit KPI, lejek sprzedaży.
- Tablica kanban z przeciąganiem kart między etapami.
- Panel szczegółów leada: edycja danych, etapu, priorytetu, kwoty, terminu.
- Oś czasu działań (notatki + automatyczne wpisy).
- Wysyłka wiadomości do klienta z szablonami.
- Ręczne dodawanie zleceń (np. z telefonu).

### d) Dodatkowo
- Kreator zamówienia wieloetapowy (`/zamowienie`).
- Konfigurator 3D (WebGL / Three.js) — **szkielet i architektura gotowe,
  pełna implementacja z silnikiem wyceny (CPQ) to osobny etap 2.**

---

## 2. Widełki rynkowe — obecny stan projektu

| Kto wycenia | Stawka | Za obecny stan |
|---|---|---|
| Software house | 300 – 450 zł/h | **90 000 – 160 000** |
| Freelancer senior | 150 – 220 zł/h | **40 000 – 70 000** |
| Realna cena z tego segmentu | — | **20 000 – 35 000** |
| Cena „po znajomości" | 100 – 140 zł/h | **15 000 – 25 000** |

### Rozbicie na części (wartość odtworzenia u freelancera)

| Element | Wartość rynkowa (PLN) |
|---|---|
| Strona prezentacyjna (custom, 2 języki, animacje, SEO/a11y) | 8 000 – 18 000 |
| Backend + baza + formularze + e-mail + testy | 6 000 – 15 000 |
| Panel CRM (kanban, pulpit, lejek, e-maile) | 12 000 – 30 000 |
| Kreator zamówienia | 3 000 – 6 000 |
| Konfigurator 3D (obecny szkielet) | 5 000 – 12 000 |
| **Razem jako projekt komercyjny** | **~35 000 – 80 000** |

---

## 3. Konfigurator 3D + CPQ — osobna wycena

**25 000 – 60 000 zł** za pełną implementację (modele parametryczne, silnik
wyceny, eksport konfiguracji do zamówienia, optymalizacja mobilna).

---

## 4. Rekomendowany model rozliczenia

| Pozycja | Kwota |
|---|---|
| **Budowa obecnego stanu** (kotwica negocjacyjna) | **18 000 zł** jednorazowo — z miejscem do zejścia do 15 000 |
| Wdrożenie produkcyjne (hosting, domena, HTTPS, SMTP, utwardzenie logowania) | 3 000 – 5 000 zł — lub w pakiecie przy pełnej cenie |
| **Utrzymanie i wsparcie** | **300 – 600 zł / mies.** |
| Konfigurator 3D + CPQ | osobna umowa, etap 2 |

Abonament utrzymaniowy jest **ważniejszy niż 3 000 zł różnicy w cenie
startowej** — daje przewidywalny przychód i naturalny pretekst do dalszej pracy
nad produktem.

---

## 5. Co jeszcze zostało do produkcji

- [ ] Wdrożenie: hosting, domena, HTTPS, konfiguracja SMTP.
- [ ] Utwardzenie logowania do panelu admina (dziś prosty klucz `X-Api-Key`).
- [x] ~~Repozytorium git~~ — **zrobione**.
- [ ] Prawdziwe zdjęcia (obecne są poglądowe) i finalna treść.
- [ ] Panel CMS do edycji treści (treść jest w bazie, ale bez UI do edycji).

Stąd wybór: albo cena bliżej dolnej granicy „jako MVP do dokończenia", albo
cena wyższa, ale z tymi pozycjami w pakiecie.

---

## 6. Ustalenia, które trzeba spisać (jedna strona A4)

„Uczciwie" to nie tylko kwota — to jasne ustalenie, **co wchodzi w cenę**:

- Czy w cenie jest wdrożenie i konfiguracja, czy osobno.
- Ile rund poprawek jest wliczonych.
- Kto płaci za hosting / domenę / SMTP (koszty cykliczne właściciela).
- Czy jest utrzymanie / wsparcie i w jakim zakresie.
- Że klient otrzymuje **własność kodu** (to backend + panel, nie szablon).
- Co **nie** wchodzi: konfigurator 3D, CMS, zdjęcia, treści marketingowe.

---

## 7. Podsumowanie jednym zdaniem

Rynkowo praca warta jest **40 000 – 160 000 zł** zależnie od tego, kto ją
wycenia; uczciwa cena po znajomości za obecny stan to **15 000 – 25 000 zł**
jednorazowo (kotwica: **18 000**), plus **300 – 600 zł/mies.** utrzymania, a
konfigurator 3D z CPQ to osobny etap wart **25 000 – 60 000 zł**.
