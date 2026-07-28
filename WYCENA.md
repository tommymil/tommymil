# Wycena projektu — A.K. HOUSE

**Dokument pomocniczy do ustalenia uczciwej ceny (dla obu stron).**
Data: 23.07.2026

> Zastrzeżenie: to oszacowanie oparte na realnym zakresie kodu i typowych
> polskich stawkach. Ma być podstawą do rozmowy, a nie „wyceną urzędową".
> Kwoty podano w PLN, brutto/netto do ustalenia między stronami.

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
- EF Core (SQLite, wymienialne na PostgreSQL), migracje, seeder.
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
- Konfigurator 3D (WebGL / Three.js) — obecnie schowany, gotowy na przyszłość.

---

## 2. Wartość rynkowa poszczególnych części

Gdyby całość budował freelancer od zera:

| Element | Wartość rynkowa (PLN) |
|---|---|
| Strona prezentacyjna (custom, 2 języki, animacje, SEO/a11y) | 8 000 – 18 000 |
| Backend + baza + formularze + e-mail + testy | 6 000 – 15 000 |
| Panel CRM (kanban, pulpit, lejek, e-maile) | 12 000 – 30 000 |
| Kreator zamówienia | 3 000 – 6 000 |
| Konfigurator 3D (demo) | 5 000 – 12 000 |
| **Razem jako projekt komercyjny** | **~35 000 – 80 000** |

- Software house: górna część przedziału lub wyżej.
- Przeciętny freelancer mid: 25 000 – 45 000 zł.

---

## 3. Uczciwa cena „po znajomości"

Dwie niezależne metody dają zbieżny wynik:

- **Godzinowo (wartość dostarczona):** efekt ok. 150–220 h pracy o jakości
  produkcyjnej × przyjazna stawka 100–140 zł/h → **15 000 – 30 000 zł**.
- **Procent ceny rynkowej:** rabat „dla znajomego" 40–60% → **15 000 – 35 000 zł**.

### 👉 Sugerowany środek dla gotowego MVP: **15 000 – 20 000 zł** jednorazowo.

---

## 4. Co obniża cenę (uczciwie wobec znajomego)

Projekt to **bardzo zaawansowane MVP, ale jeszcze nie „wdrożony produkt"**.
Do produkcji brakuje:

- [ ] Wdrożenia: hosting, domena, HTTPS, konfiguracja SMTP.
- [ ] Utwardzenia logowania do panelu admina (dziś prosty klucz API).
- [ ] Repozytorium git (obecnie **nie istnieje** — brak historii i kopii).
- [ ] Prawdziwych zdjęć (obecne są poglądowe) i finalnej treści.
- [ ] Panelu CMS do edycji treści (treść jest w bazie, ale bez UI do edycji).

Dlatego albo cena bliżej dolnej granicy „jako MVP do dokończenia",
albo cena wyższa, ale z tymi pozycjami w pakiecie.

---

## 5. Jak poukładać, żeby było fair dla obu stron

„Uczciwie" to nie tylko kwota — to jasne ustalenie **co wchodzi w cenę**:

- Czy w cenie jest wdrożenie i konfiguracja, czy osobno.
- Ile rund poprawek jest wliczonych.
- Kto płaci za hosting / domenę / SMTP (koszty cykliczne właściciela).
- Czy jest utrzymanie / wsparcie (np. 200–500 zł/mies. lub pakiet godzin).
- Że klient otrzymuje **własność kodu** (to backend + panel, nie szablon).

### Rekomendowany model rozliczenia (najbardziej „równy")

| Pozycja | Kwota |
|---|---|
| Budowa (to, co jest teraz) | 12 000 – 15 000 zł jednorazowo |
| Dokończenie do produkcji (wdrożenie, git, hardening, CMS) | wg godzin, osobno |
| Wsparcie miesięczne (opcjonalnie) | 200 – 500 zł / mies. |

---

## 6. Podsumowanie jednym zdaniem

Rynkowo praca warta jest **35 000 – 80 000 zł**; uczciwa cena po znajomości za
obecny stan (zaawansowane MVP) to **~15 000 – 20 000 zł jednorazowo**, a jeśli
rozbić to na „budowa + wdrożenie + wsparcie", budowa spokojnie broni się w
okolicach **12 000 – 15 000 zł**.
