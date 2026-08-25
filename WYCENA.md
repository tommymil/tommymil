# Wycena projektu — A.K. HOUSE

**Dokument pomocniczy do ustalenia uczciwej ceny (dla obu stron).**
Data: 23.08.2026 (zastępuje wersje z 23.07, 29.07 i 12.08.2026)

> Zastrzeżenie: to oszacowanie oparte na realnym zakresie kodu i publikowanych
> polskich stawkach z 2026 roku. Ma być podstawą do rozmowy, a nie „wyceną
> urzędową". Kwoty w PLN **netto**, o ile nie zaznaczono inaczej.

---

## 0. Stan faktyczny repozytorium

| Metryka | 29.07 | 12.08 | **23.08** |
|---|---|---|---|
| Kod produkcyjny (`backend/src` + `frontend/src`) | ~18 100 | ~22 700 | **24 618** |
| Style globalne (CSS) | — | — | **2 329** |
| Kod testów | — | ~4 300 | **4 956** |
| **Razem linii** | — | ~27 000 | **31 903** |
| Pliki źródłowe | 183 | 255 | **281** |
| Moduły frontendu | 7 | 11 | **12** |
| Endpointy API | — | 61 | **70** |
| Trasy frontendu | — | 35 | **36** |
| Migracje bazy | — | 17 | **17 + 2 do wygenerowania** |
| Metody testowe | 88 | 252 | **271** |
| Narzędzia (Python / PowerShell) | — | 1 | **3** (935 linii) |

Stos: C# / .NET 10 (Clean Architecture, EF Core, xUnit) + React 19 / TypeScript
(Vite, MVVM), SQLite wymienialne na PostgreSQL, ASP.NET Core Identity.

**Co przybyło od 12.08:** osobny panel sprzedaży z własnym kluczem dostępu,
kategorie sklepu przeniesione z kodu do bazy, mechanizm cen oparty na cenniku
detalicznym dostawcy z globalnym narzutem, pobranie i dopasowanie 228 cen
Balia Technic oraz komplet standardów kodowania, których projekt wcześniej
nie miał wcale.

---

## 1. Podział na moduły

Projekt to nie „strona firmowa". To **cztery produkty w jednym**: witryna
sprzedażowa, katalog SEO, system obsługi zapytań (CRM) i sklep internetowy
z własnym zapleczem. Poniżej podział na moduły, które dają się wycenić
i kupić osobno.

### A. Fundament techniczny
Clean Architecture w czterech projektach (`Domain → Application → Infrastructure → Api`),
EF Core z 17 migracjami, seeder, wstrzykiwanie zależności, konfiguracja przez
`user-secrets` i zmienne środowiskowe. Globalny handler błędów mapujący wyjątki
domenowe na RFC 7807. Ograniczenie liczby zgłoszeń na IP, CORS, OpenAPI, health check.

Frontend: Vite + TypeScript w trybie ścisłym, wzorzec MVVM, tokeny projektowe
jako jedno źródło prawdy o kolorach i typografii, dwujęzyczność PL/EN, routing,
warstwa klienta HTTP.

### B. Strona prezentacyjna
Kilkanaście sekcji, animowane tło 3D (siatka heksagonalna generowana skryptem),
parallax, liczniki, modale z focus-trapem i obsługą `Esc`, galeria z filtrami
i powiększaniem, strona realizacji. Pełna wersja angielska.
SEO: `robots.txt`, `sitemap.xml`, Open Graph, JSON-LD `LocalBusiness`.
Dostępność: powiązane etykiety, `aria-invalid`, `prefers-reduced-motion`.

### C. Katalog produktów
Trasy PL i EN dla trzech kategorii i trzech kart produktów, **dziewięć stron
generowanych statycznie przy budowaniu** — dzięki temu Google widzi treść
bez uruchamiania JavaScriptu. Canonicale, hreflang, dane strukturalne
`CollectionPage` / `Product` / `Offer`. Kreator wariantu w SVG (rysunki elewacji
plus rzut z góry) — zastąpił zawieszony konfigurator 3D.

### D. Lejek zapytań
Formularz kontaktowy i pięciokrokowy kreator wyceny trafiają do jednego lejka.
Zgoda RODO, honeypot i limit zgłoszeń na IP przy każdej publicznej ścieżce.
Numer zapytania `ZAM-…`, wysyłka e-maili przez SMTP.

### E. Panel obsługi zleceń (`/admin`)
Cztery widoki: **Na dziś** (lista robocza operatora), **Pulpit** (kafelki KPI),
**Tablica** (kanban z przeciąganiem kart, optymistyczna zmiana etapu z wycofaniem
przy błędzie) i **Lista** (filtry, sortowanie). Panel szczegółów z osią czasu
działań, edycją danych kontaktowych, wysyłką e-maila z szablonami i ręcznym
dodawaniem zleceń spoza strony.

To jest **mały CRM**, nie „podgląd formularzy".

### F. Sklep — część klienta
Katalog z bazy z filtrami, wyszukiwarką i sortowaniem. Koszyk w `localStorage`
pod wersjonowanym kluczem — działa dla gościa, bez sesji.
**Ceny liczone wyłącznie po stronie serwera**: z przeglądarki przychodzą tylko
identyfikator produktu i ilość. Checkout z przelewem tradycyjnym, numer `SKL-…`,
mail potwierdzający, wymagana akceptacja regulaminu weryfikowana także na serwerze.
Pozycja zamówienia trzyma własną kopię nazwy i ceny — zmiana cennika nie
przepisuje historii sprzedaży.

### G. Konta klientów
ASP.NET Core Identity, sesja w ciasteczku `HttpOnly` (nie token w `localStorage`),
rejestracja, logowanie, reset hasła mailem, profil z danymi do wysyłki, historia
zamówień. Zakup jako gość pozostaje możliwy.

### H. Panel sprzedaży (`/sklep/panel`) — **nowy**
Osobne wejście z **własnym kluczem dostępu**. Wcześniej sklepem zarządzało się
zakładką w panelu zleceń, chronioną tym samym hasłem — kto miał prowadzić
asortyment, dostawał wgląd we wszystkie zapytania klientów, ich telefony,
budżety i historię kontaktów. Teraz obie role nadaje się i odbiera niezależnie.

Cztery zakładki. **Asortyment** to tabela z wyszukiwarką, filtrem kategorii
i widoczności, sortowaniem i stronicowaniem — przy 156 pozycjach z importu to
różnica między „da się pracować" a „przewiń 155 kart, żeby zmienić cenę uszczelki".
Zaznaczenie wielu wierszy odsłania **operacje masowe**: pokaż / ukryj, dostępność,
przeniesienie do kategorii, procentowa zmiana ceny.

### I. Kategorie zarządzalne z panelu — **nowe**
Kategorie były wyliczeniem w kodzie C# — nowa półka wymagała zmiany kodu
i wdrożenia. Teraz są tabelą w bazie: dodawanie, zmiana nazwy i adresu,
kolejność, ukrywanie, usuwanie. Usunięcie półki z produktami wymaga wskazania,
dokąd je przenieść.

Przejście wykonano tak, że **nie przepisano ani jednego ze 156 wierszy
produktów** — migracja jest czysto addytywna.

### J. Mechanizm cen: cennik dostawcy plus narzut — **nowy**
Każdy produkt trzyma **cenę detaliczną dostawcy** jako bazę. Cena w sklepie
powstaje z niej przez **jeden narzut na cały sklep**, ustawiany w panelu
(pole z przecinkiem, podgląd na trzech realnych pozycjach, jedno kliknięcie).

Narzut jest **aplikowany, nie kumulowany**: liczy się zawsze od bazy, nigdy od
ceny bieżącej. Kliknięcie +10% dwa razy to nadal +10%, a wpisanie 0 wraca
dokładnie do cen dostawcy. Zakres ograniczony do −99…+500%, żeby zgubione zero
nie przeceniło sklepu.

### K. Moduł mediów
Podmiana zdjęć i ich nazw w konkretnych punktach strony, z osobnym kluczem
dostępu. Nowe miejsca dochodzą automatycznie. Galeria dodatkowa bez limitu zdjęć,
z kategoriami. Walidacja uploadu: JPG/PNG/WEBP/AVIF do 8 MB, SVG odrzucany,
bo wykonuje JavaScript po otwarciu bezpośrednio.

### L. Integracja z dostawcą — **rozszerzone**
Dwa narzędzia:

1. **Import cennika hurtowego** — arkusz XLSX na dane katalogu: 156 pozycji,
   13 kategorii, 156 zdjęć przekonwertowanych z 240 MB do 13 MB WebP,
   z przeliczeniem marży i VAT.
2. **Pobranie i dopasowanie cen detalicznych** — pobrane 228 pozycji ze sklepu
   dostawcy, dopasowanie rozmyte do naszego asortymentu z zabezpieczeniem przed
   najgroźniejszym błędem: gdy dwa warianty tego samego produktu różnią się
   ceną, narzędzie **odmawia wyboru** zamiast strzelać. Wynik podzielony na
   pewne / do przejrzenia / bez odpowiednika, plus skrypt wgrywający całość
   jednym uruchomieniem.

Powód, dla którego to było potrzebne: pierwotna formuła `hurt × 1,10 × 1,23`
rozjeżdża się z cenami detalicznymi dostawcy **od −18% do +64%** — żaden
mnożnik tego nie naprawi.

### M. Standardy i dokumentacja techniczna — **nowe**
Projekt nie miał żadnego pliku opisującego konwencje. Powstały: instrukcje
dla całego repozytorium i osobno dla backendu i frontendu, `.editorconfig`,
wspólne właściwości projektów .NET z ostrzeżeniami traktowanymi jak błędy
w konfiguracji wydania oraz konfiguracja lintera frontendu — sprawdzona na
realnym kodzie, przechodzi bez błędów.

### N. Konfigurator 3D — zawieszony
Szkielet na Three.js / React Three Fiber, ładowanie modelu FBX, parametryczny
taras, leniwe ładowanie. Kod zostaje w repozytorium, ale **nie jest wystawiany
publicznie** — nie prowadzi do niego żadna trasa.

---

## 2. Nakład pracy

| Moduł | Godziny |
|---|---:|
| A. Fundament: architektura, baza, migracje, bezpieczeństwo | 45 – 60 |
| B. Strona prezentacyjna PL/EN (animacje, SEO, dostępność) | 55 – 75 |
| C. Katalog produktów (trasy PL/EN, prerender, kreator SVG) | 35 – 50 |
| D. Lejek zapytań: formularz i kreator wyceny | 15 – 25 |
| E. Panel obsługi zleceń (CRM) | 60 – 85 |
| F. Sklep — część klienta | 70 – 100 |
| G. Konta klientów | 25 – 35 |
| H. Panel sprzedaży z osobnym wejściem | 28 – 38 |
| I. Kategorie zarządzalne z panelu | 18 – 25 |
| J. Mechanizm cen: baza dostawcy + narzut | 18 – 24 |
| K. Moduł mediów i galerii | 15 – 25 |
| L. Integracja z dostawcą (dwa importy) | 34 – 50 |
| M. Standardy, konwencje, dokumentacja | 12 – 16 |
| N. Konfigurator 3D (obecny szkielet) | 25 – 40 |
| **Razem** | **455 – 648 h** |

**Kontrola z drugiej strony.** 31 903 linie kodu produkcyjnego i testów przy
tempie 55 – 70 linii na godzinę pracy o jakości produkcyjnej daje 456 – 580 h.
Obie metody spotykają się w okolicy **460 – 620 h**.

Uwaga metodyczna: część pracy z ostatniego etapu — pobranie 228 cen, dopasowanie
ich do asortymentu, przegląd wyników i weryfikacja krzyżowa — **nie zostawia po
sobie linii kodu**, a zajęła realny czas. Metoda „linia na godzinę" ją zaniża.

---

## 3. Widełki rynkowe

Stawki i przedziały z publikacji branżowych z 2026 roku (źródła na końcu).

| Kto wycenia | Stawka | Za 460 – 620 h |
|---|---|---:|
| Software house | 150 – 350 zł/h | **69 000 – 217 000** |
| Freelancer senior (B2B) | 140 – 220 zł/h | **64 000 – 136 000** |
| Utrzymanie i rozwój po wdrożeniu | 200 – 600 zł/h | — |

Dla porównania, publikowane widełki całych projektów:

- system średniej złożoności (kilka modułów, role, integracje): **80 000 – 250 000 zł**;
- rozbudowana platforma wielomodułowa: **250 000 zł i więcej**;
- sklep dedykowany na gotowym silniku: PrestaShop ok. **100 000 zł**, Magento ok. **150 000 zł**.

A.K. HOUSE ma dwanaście modułów frontendu, siedemdziesiąt endpointów, dwa panele
administracyjne z rozdzielonymi uprawnieniami i sklep — mieści się w górnej
części pasma „średniej złożoności".

### Wartość odtworzenia — gdyby zamawiać moduł po module

| Moduł | Wartość rynkowa |
|---|---:|
| A. Fundament techniczny | 6 000 – 15 000 |
| B. Strona prezentacyjna PL/EN | 8 000 – 18 000 |
| C. Katalog produktów z SEO | 6 000 – 12 000 |
| D. Lejek zapytań i kreator wyceny | 3 000 – 6 000 |
| E. Panel obsługi zleceń (CRM) | 12 000 – 30 000 |
| F. Sklep — część klienta | 15 000 – 35 000 |
| G. Konta klientów | 5 000 – 12 000 |
| H. Panel sprzedaży z osobnym wejściem | 10 000 – 22 000 |
| I. Kategorie zarządzalne | 4 000 – 9 000 |
| J. Mechanizm cen: baza + narzut | 5 000 – 11 000 |
| K. Moduł mediów i galerii | 3 000 – 7 000 |
| L. Integracja z dostawcą | 7 000 – 14 000 |
| M. Standardy i dokumentacja | 3 000 – 7 000 |
| N. Konfigurator 3D (szkielet) | 5 000 – 12 000 |
| **Razem jako projekt komercyjny** | **92 000 – 210 000** |

Obie metody — stawka godzinowa i suma modułów — wskazują ten sam przedział.

> ### Realna wartość rynkowa obecnego stanu: **90 000 – 200 000 zł**

---

## 4. Cena

### 4.1. Co obejmuje oferta

Oferta jest **kompleksowa i ryczałtowa**: cena obejmuje nie tylko to, co już
powstało, ale doprowadzenie projektu do stanu, w którym **sklep i strona działają
publicznie pod własnym adresem, a studio może z nich korzystać bez mojego udziału**.

W cenie zawiera się:

| Zakres | Godziny | Wartość (87 zł/h) |
|---|---:|---:|
| Moduły A – N (obecny stan projektu) | 455 – 648 | 39 600 – 56 400 |
| Wdrożenie produkcyjne: hosting, domena, HTTPS, SMTP, kopie zapasowe | 25 – 40 | 2 200 – 3 500 |
| Logowanie na konta operatorów zamiast kluczy w nagłówku | 40 – 60 | 3 500 – 5 200 |
| Dokończenie cennika: pozostałe 88 pozycji i uruchomienie sprzedaży | 15 – 25 | 1 300 – 2 200 |
| Odbiór, poprawki, szkolenie z paneli, przekazanie dostępów | 20 – 35 | 1 700 – 3 000 |
| **Razem** | **555 – 808 h** | **48 300 – 70 300** |

**Środek widełek — 680 h — daje cenę standardową 59 000 zł.**

### 4.2. Cena z rabatem partnerskim

Jeden ze współwłaścicieli A.K. HOUSE jest moim znajomym i będzie wykonywał
meble do mojego mieszkania. Z tego tytułu obniżam cenę.

| Pozycja | Kwota |
|---|---:|
| Cena standardowa (680 h × 87 zł) | 59 000 zł |
| **Rabat partnerski** | **−11 000 zł** |
| **Cena końcowa — ryczałt, wszystko wliczone** | **48 000 zł** |

Stawka pozostaje **87 zł/h** — poniżej dolnej granicy rynku dla freelancera
seniora i wielokrotnie poniżej stawek software house'ów. Rabat nie polega na
obniżeniu stawki, tylko na tym, że **w tej samej cenie mieści się więcej pracy**:
wdrożenie, utwardzenie logowania i doprowadzenie sklepu do sprzedaży wchodzą
w pakiet, zamiast być dopłatą.

Odniesienie do wartości rynkowej: **48 000 zł to 24 – 53% tego, co ten sam
zakres kosztowałby na rynku.**

### 4.3. Co dokładnie dostaje klient

- Działającą stronę i sklep pod własną domeną, na skonfigurowanym serwerze,
  z certyfikatem HTTPS i działającą wysyłką e-maili.
- Sklep z uzupełnionym cennikiem, gotowy do przyjmowania zamówień.
- Trzy panele: obsługa zleceń, sprzedaż, zdjęcia — każdy z osobnym dostępem
  i **logowaniem na konto operatora**, nie kluczem w nagłówku.
- Cały kod źródłowy i prawa do niego.
- Dokumentację techniczną i instrukcję obsługi paneli.
- Szkolenie z obsługi (zdalne, do 2 godzin).
- Dwie rundy poprawek po odbiorze.
- Projekt w stanie, w którym inny programista może go przejąć.

### 4.4. Czego cena nie obejmuje

Żeby „wszystko wliczone" znaczyło coś konkretnego, tu jest druga strona listy:

- **Koszty cykliczne**: hosting, domena, konto SMTP, certyfikaty. Konfiguruję je,
  ale opłaca właściciel — to jego infrastruktura, nie moja usługa.
- **Weryfikacja prawna** regulaminu i polityki zwrotów. Dostarczam wzorce
  z polami do uzupełnienia; ocena prawnika to osobny koszt po stronie klienta.
- **Treść i zdjęcia**: teksty o firmie i zdjęcia własnych realizacji dostarcza
  studio. Wgranie ich i osadzenie jest w cenie.
- **Etapy z rozdziału 6** — płatności online, CMS, kurier, wersja EN sklepu,
  konfigurator 3D. To osobne zamówienia.
- Zmiany zakresu zgłoszone po odbiorze — rozliczane godzinowo.

### 4.5. Harmonogram płatności

| Etap | Udział | Kwota |
|---|---:|---:|
| Podpisanie umowy / start prac wdrożeniowych | 40% | 19 200 zł |
| Odbiór: strona i sklep działają publicznie | 60% | 28 800 zł |

### 4.6. Utrzymanie i wsparcie

| Pozycja | Kwota |
|---|---:|
| Abonament utrzymaniowy | **800 – 1 200 zł / mies.** |

**Ta pozycja nie jest objęta rabatem** — i to jest świadome. Rabat dotyczy
jednorazowej budowy, bo to jednorazowy gest. Utrzymanie to praca wykonywana
co miesiąc przez lata; upust na nim zamieniłby przysługę w zobowiązanie bez końca.

Wysokość wynika stąd, że **sklep to inna klasa odpowiedzialności niż strona**:
awaria zamówień albo maili to utracone pieniądze klienta tego samego dnia,
a nie odłożona niedogodność.

Abonament obejmuje: monitorowanie dostępności, aktualizacje bezpieczeństwa,
kopie zapasowe i ich odtwarzanie, drobne poprawki do 2 godzin miesięcznie
oraz reakcję na awarię blokującą sprzedaż w ciągu jednego dnia roboczego.

## 5. Droga do odbioru

### Po mojej stronie — w cenie

- [ ] Pierwsze zbudowanie i uruchomienie najnowszych zmian (dwie migracje
      do wygenerowania: tabela kategorii i cennik).
- [ ] Wdrożenie: hosting, domena, HTTPS, konfiguracja SMTP, kopie zapasowe.
- [ ] Zamiana kluczy w nagłówku na logowanie na konta operatorów.
- [ ] Dokończenie cennika: 68 pozycji ma już cenę detaliczną dostawcy,
      pozostałe 88 wymaga decyzji o wariantach i wgrania.
- [ ] Osadzenie dostarczonych treści i zdjęć.
- [ ] Szkolenie z obsługi paneli i przekazanie dostępów.

### Po stronie studia — bez tego sprzedaż nie ruszy

- [ ] Numer konta bankowego i nazwa banku — bez tego mail do klienta nie
      zawiera danych do przelewu.
- [ ] Weryfikacja prawna `regulamin.html` i `zwroty.html`, uzupełnienie pól
      `[w nawiasach]`.
- [ ] Zdjęcia własnych realizacji i finalna treść „o firmie".
- [ ] Decyzja o marży: narzut nad cenami detalicznymi dostawcy ustawia się
      w panelu jednym kliknięciem, ale wysokość to decyzja handlowa.

### Zrobione

- [x] ~~Sklep internetowy~~ — zrobione.
- [x] ~~Asortyment z cennika dostawcy~~ — zrobione.
- [x] ~~Panel sprzedaży z osobnym dostępem~~ — zrobione.
- [x] ~~Kategorie zarządzalne z panelu~~ — zrobione.
- [x] ~~Mechanizm cen oparty na cenniku detalicznym dostawcy~~ — zrobione.

---

## 6. Możliwe kolejne etapy (osobne wyceny)

Ceny partnerskie z tym samym rabatem co oferta główna.

| Etap | Wartość rynkowa | Cena partnerska |
|---|---:|---:|
| Płatności online (Przelewy24 / Stripe) zamiast przelewu | 6 000 – 12 000 | 4 800 – 9 600 |
| Panel CMS do edycji treści strony | 8 000 – 15 000 | 6 400 – 12 000 |
| Integracja z kurierem: etykiety, śledzenie | 5 000 – 10 000 | 4 000 – 8 000 |
| Angielska wersja sklepu | 5 000 – 9 000 | 4 000 – 7 200 |
| Konfigurator 3D + wycena konfiguracji (CPQ) | 25 000 – 60 000 | 20 000 – 48 000 |

---

## 7. Ustalenia, które warto spisać

„Uczciwie" to nie tylko kwota — to jasne ustalenie, **co wchodzi w cenę**:

- Kto płaci za hosting, domenę i SMTP — to koszty cykliczne właściciela, nie wykonawcy.
- Zakres i czas reakcji w ramach utrzymania.
- Co się dzieje, gdy potrzebna jest zmiana spoza zakresu — stawka godzinowa i tryb zgłaszania.
- Przekazanie dostępów i kodu na wypadek zakończenia współpracy.

---

## Źródła stawek rynkowych

- [Complaia Systems — *Ile kosztuje dedykowane oprogramowanie dla firmy? Widełki 2026*](https://www.complaia.systems/ile-kosztuje-dedykowane-oprogramowanie) — stawki 150 – 350 zł/h, przedziały projektów 30 – 80 tys. / 80 – 250 tys. / 250 tys.+
- [Satisfly — *Ile kosztuje sklep internetowy w 2026 roku?*](https://satisfly.co/pl/blog/ile-kosztuje-sklep-internetowy/) — sklep dedykowany: PrestaShop ok. 100 tys., Magento ok. 150 tys.; utrzymanie 200 – 600 zł/h
- [ARDURA Consulting — *Stawki React developera w Polsce 2026*](https://ardura.pl/blog/stawki-react-developera-polska-2026/) — senior B2B 22 – 35 tys. zł/mies.
