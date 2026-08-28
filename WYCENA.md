# Wycena projektu — A.K. HOUSE

**Dokument pomocniczy do ustalenia uczciwej ceny (dla obu stron).**
Data: 27.08.2026 (zastępuje wersje z 23.07, 29.07, 12.08 i 23.08.2026)

> Zastrzeżenie: to oszacowanie oparte na realnym zakresie kodu i publikowanych
> polskich stawkach z 2026 roku. Ma być podstawą do rozmowy, a nie „wyceną
> urzędową". Kwoty w PLN **netto**, o ile nie zaznaczono inaczej.

---

## 0. Stan faktyczny repozytorium

| Metryka | 29.07 | 12.08 | 23.08 | **27.08** |
|---|---|---|---|---|
| Kod produkcyjny (bez migracji) | ~18 100 | ~22 700 | 24 618 | **26 076** |
| Migracje bazy danych (kod) | — | — | — | **1 883** |
| Style globalne (CSS) | — | — | 2 329 | **2 461** |
| Kod testów | — | ~4 300 | 4 956 | **5 337** |
| Narzędzia (Python / PowerShell) | — | ~300 | 935 | **1 756** |
| **Razem w repozytorium** | — | ~27 000 | 31 903 | **37 513** |
| Pliki źródłowe | 183 | 255 | 281 | **274** |
| Moduły frontendu | 7 | 11 | 12 | **12** |
| Endpointy API | — | 61 | 70 | **73** |
| Trasy frontendu | — | 35 | 36 | **36** |
| Migracje bazy | — | 17 | 17 + 2 | **21** |
| Metody testowe | 88 | 252 | 271 | **290** (176 + 135 przypadków) |

Stos: C# / .NET 10 (Clean Architecture, EF Core, xUnit) + React 19 / TypeScript
(Vite, MVVM), SQLite wymienialne na PostgreSQL, ASP.NET Core Identity, Stripe.

**Liczba plików spadła, a kodu przybyło** — to nie błąd pomiaru. Poprzedni pomiar
liczył też pliki migracji; ten je wydziela. Scalenie duplikatów w katalogu
(156 → 155 pozycji) i przeniesienie starego panelu sklepu też zabrało po pliku.

**Co przybyło od 23.08:** płatności online (karta, BLIK, Przelewy24), dane do
faktury w zamówieniu, warianty produktu z przełącznikiem koloru, klasy wysyłki
jako zbiór zamiast pojedynczej wartości, serwerowe wyszukiwanie z filtrami
i stronicowaniem, rozbudowana karta produktu, komplet testów **uruchomiony
i przechodzący** oraz narzędzie sprawdzające SQL migracji na świeżej bazie.

---

## 1. Podział na moduły

Projekt to nie „strona firmowa". To **cztery produkty w jednym**: witryna
sprzedażowa, katalog SEO, system obsługi zapytań (CRM) i sklep internetowy
z własnym zapleczem. Poniżej podział na moduły, które dają się wycenić
i kupić osobno.

### A. Fundament techniczny
Clean Architecture w czterech projektach (`Domain → Application → Infrastructure → Api`),
EF Core z 21 migracjami, seeder, wstrzykiwanie zależności, konfiguracja przez
`user-secrets` i zmienne środowiskowe. Globalny handler błędów mapujący wyjątki
domenowe na RFC 7807. Ograniczenie liczby zgłoszeń na IP, CORS, OpenAPI, health check.

Frontend: Vite + TypeScript w trybie ścisłym, wzorzec MVVM, tokeny projektowe
jako jedno źródło prawdy o kolorach i typografii, dwujęzyczność PL/EN, routing,
warstwa klienta HTTP.

### B. Strona prezentacyjna
Kilkanaście sekcji, animowane tło (siatka heksagonalna generowana skryptem),
parallax, liczniki, modale z pułapką focusu i obsługą `Esc`, galeria z filtrami
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

### E. Panel obsługi zleceń (CRM, `/crm`)
Cztery widoki: **Na dziś** (lista robocza operatora), **Pulpit** (kafelki KPI),
**Tablica** (kanban z przeciąganiem kart, optymistyczna zmiana etapu z wycofaniem
przy błędzie) i **Lista** (filtry, sortowanie). Panel szczegółów z osią czasu
działań, edycją danych kontaktowych, wysyłką e-maila z szablonami i ręcznym
dodawaniem zleceń spoza strony.

To jest **mały CRM**, nie „podgląd formularzy".

### F. Sklep — część klienta
Katalog z bazy, koszyk w `localStorage` pod wersjonowanym kluczem — działa dla
gościa, bez sesji. **Ceny liczone wyłącznie po stronie serwera**: z przeglądarki
przychodzą tylko identyfikator produktu i ilość. Checkout z numerem `SKL-…`,
mail potwierdzający, wymagana akceptacja regulaminu weryfikowana także na serwerze.
Pozycja zamówienia trzyma własną kopię nazwy i ceny — zmiana cennika nie
przepisuje historii sprzedaży.

### G. Konta klientów
ASP.NET Core Identity, sesja w ciasteczku `HttpOnly` (nie token w `localStorage`),
rejestracja, logowanie, reset hasła mailem, profil z danymi do wysyłki, historia
zamówień. Zakup jako gość pozostaje możliwy.

### H. Panel sprzedaży (`/sklep/panel`)
Osobne wejście z **własnym kluczem dostępu**. Wcześniej sklepem zarządzało się
zakładką w panelu zleceń, chronioną tym samym hasłem — kto miał prowadzić
asortyment, dostawał wgląd we wszystkie zapytania klientów, ich telefony,
budżety i historię kontaktów. Teraz obie role nadaje się i odbiera niezależnie.

Pięć zakładek. **Asortyment** to tabela z wyszukiwarką, filtrami kategorii,
widoczności i cennika, sortowaniem i stronicowaniem — przy 155 pozycjach z importu
to różnica między „da się pracować" a „przewiń 154 karty, żeby zmienić cenę
uszczelki". Zaznaczenie wielu wierszy odsłania **operacje masowe**: pokaż / ukryj,
dostępność, przeniesienie do kategorii, procentowa zmiana ceny.

### I. Kategorie zarządzalne z panelu
Kategorie były wyliczeniem w kodzie C# — nowa półka wymagała zmiany kodu
i wdrożenia. Teraz są tabelą w bazie: dodawanie, zmiana nazwy i adresu,
kolejność, ukrywanie, usuwanie. Usunięcie półki z produktami wymaga wskazania,
dokąd je przenieść.

Przejście wykonano tak, że **nie przepisano ani jednego wiersza produktów** —
migracja jest czysto addytywna.

### J. Mechanizm cen: cennik dostawcy plus narzut
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

### L. Integracja z dostawcą
Dwa narzędzia:

1. **Import cennika hurtowego** — arkusz XLSX na dane katalogu: 155 pozycji,
   13 kategorii, 156 zdjęć przekonwertowanych z 240 MB do 13 MB WebP,
   z przeliczeniem marży i VAT.
2. **Pobranie i dopasowanie cen detalicznych** — 228 pozycji ze sklepu dostawcy,
   dopasowanie rozmyte z zabezpieczeniem przed najgroźniejszym błędem: gdy dwa
   warianty tego samego produktu różnią się ceną, narzędzie **odmawia wyboru**
   zamiast strzelać. Wynik podzielony na pewne / do przejrzenia / bez odpowiednika,
   plus skrypt wgrywający całość jednym uruchomieniem.

Powód, dla którego to było potrzebne: pierwotna formuła `hurt × 1,10 × 1,23`
rozjeżdża się z cenami detalicznymi dostawcy **od −18% do +64%** — żaden
mnożnik tego nie naprawi.

### M. Standardy i dokumentacja techniczna
Projekt nie miał żadnego pliku opisującego konwencje. Powstały: instrukcje
dla całego repozytorium i osobno dla backendu i frontendu, `.editorconfig`,
wspólne właściwości projektów .NET z ostrzeżeniami traktowanymi jak błędy
w konfiguracji wydania oraz konfiguracja lintera frontendu. Do tego dokument
stanu prac, który pozwala przejąć projekt bez rozmowy z autorem.

### N. Konfigurator 3D — zawieszony
Szkielet na Three.js / React Three Fiber, ładowanie modelu FBX, parametryczny
taras, leniwe ładowanie. Kod zostaje w repozytorium, ale **nie jest wystawiany
publicznie** — nie prowadzi do niego żadna trasa.

### O. Płatności online — **nowe**
Integracja ze Stripe: **karta, BLIK i Przelewy24** w złotówkach, obok
dotychczasowego przelewu tradycyjnego. Zamówienie zapisuje wybraną metodę
i identyfikator płatności u operatora, a status zmienia **webhook**, nie powrót
klienta z bramki — bo klient potrafi zamknąć kartę przed przekierowaniem.

Płatności są **domyślnie wyłączone**. Bez kompletu sekretów i jawnej zgody
w konfiguracji checkout ich nie pokazuje — to nie tryb awaryjny, tylko stan
wyjściowy, żeby przypadkowe wdrożenie nie wystawiło niedziałającej bramki.

### P. Dane do faktury — **nowe**
Zamówienie przyjmuje nazwę firmy, NIP i osobny adres rozliczeniowy, gdy klient
zaznaczy, że chce fakturę. Pola pojawiają się dopiero po zaznaczeniu i są
walidowane po obu stronach.

### Q. Klasy wysyłki jako zbiór — **nowe**
Produkt miał **jedną** klasę wysyłki, więc towar nadający się i do kuriera,
i do odbioru osobistego trzeba było przypisać do jednej z nich. Teraz klasa to
**zbiór**: kurier paczkowy, gabaryt, paleta, odbiór osobisty — w dowolnej
kombinacji. Metody dostawy w koszyku filtrują się do tych, które obsłużą
wszystkie pozycje zamówienia.

Migracja przepisała istniejące dane i **każdemu produktowi dołożyła odbiór
osobisty**, bo tak działało to wcześniej dla wszystkich.

### R. Warianty produktu — **nowe**
Sześć grup obejmujących 17 wkładów, które różnią się wyłącznie kolorem lub
wykończeniem. Na karcie produktu jest przełącznik: klient widzi wszystkie
warianty z cenami i zmienia je jednym kliknięciem.

Rozstrzygnięcie, które ma znaczenie na lata: **każdy wariant zostaje osobnym
produktem z własnym adresem, SKU i ceną**. Grupa jest tylko warstwą prezentacji.
Dzięki temu import cen od dostawcy, historia zamówień i panel nadal pracują na
płaskiej liście i nie muszą wiedzieć, co to wariant.

### S. Karta produktu — **rozszerzone**
SKU, marka, tabela parametrów, informacja o kompatybilności, deklarowany czas
wysyłki liczony z klasy przesyłki, galeria z powiększeniem i nawigacją
strzałkami, sekcja „dobierz do produktu". Dane strukturalne `Product` / `Offer`
z realną ceną i dostępnością.

### T. Wyszukiwanie i filtrowanie po stronie serwera — **nowe**
Osobny endpoint wyszukiwania: fraza, kategoria, przedział cenowy, tylko dostępne,
sposób dostawy, sortowanie i stronicowanie — **wszystko liczone w bazie**, nie
w przeglądarce. Stan filtrów siedzi w adresie, więc wynik wyszukiwania da się
wysłać linkiem i wraca po odświeżeniu strony.

### U. Dopracowanie sprzedażowe — **nowe**
Okno po dodaniu do koszyka z wyborem „kontynuuj zakupy / przejdź do koszyka"
zamiast porywania klienta na stronę koszyka. Wyszukiwarka przeniesiona do
nagłówka sklepu, żeby towar był widoczny od razu po wejściu. Poprawka błędu,
przez który lista skakała do góry po każdym wciśniętym klawiszu. W panelu:
filtr pozycji bez ceny dostawcy i podgląd karty produktu wprost z tabeli.

### V. Weryfikacja i narzędzia jakości — **nowe**
Cały zestaw testów został **uruchomiony i przechodzi**: 176 przypadków backendu
i 135 frontendu. Do tego powstało narzędzie, które wykonuje SQL z migracji na
świeżej bazie i sprawdza, czy **cokolwiek faktycznie zmienił** — bo `UPDATE`
z błędnym adresem produktu kończy się sukcesem i zerem zmienionych wierszy.
Taki błąd zobaczyłby dopiero klient.

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
| H. Panel sprzedaży z osobnym wejściem | 30 – 41 |
| I. Kategorie zarządzalne z panelu | 18 – 25 |
| J. Mechanizm cen: baza dostawcy + narzut | 18 – 24 |
| K. Moduł mediów i galerii | 15 – 25 |
| L. Integracja z dostawcą (dwa importy) | 34 – 50 |
| M. Standardy, konwencje, dokumentacja | 14 – 19 |
| N. Konfigurator 3D (obecny szkielet) | 25 – 40 |
| O. Płatności online (Stripe, webhook, tryb wyłączony) | 25 – 35 |
| P. Dane do faktury | 8 – 12 |
| Q. Klasy wysyłki jako zbiór + migracja danych | 10 – 14 |
| R. Warianty produktu | 16 – 24 |
| S. Karta produktu: SKU, parametry, galeria, powiązane | 16 – 24 |
| T. Wyszukiwanie i filtrowanie po stronie serwera | 14 – 20 |
| U. Dopracowanie sprzedażowe i poprawki UX | 12 – 18 |
| V. Weryfikacja, testy, narzędzia jakości | 12 – 18 |
| **Razem** | **572 – 819 h** |

**Kontrola z drugiej strony — i miejsce, gdzie się nie zgadza.**
37 513 linii w repozytorium przy tempie 55 – 70 linii na godzinę pracy o jakości
produkcyjnej daje 536 – 682 h. Dla całego projektu obie metody się spotykają.

Ale dla **ostatniego etapu metoda „linia na godzinę" zawodzi**: przybyło około
1 800 linii, co dałoby ~30 h, a realna praca to 113 – 165 h. Powód jest prosty
i warto go powiedzieć wprost: integracja z bramką płatniczą, przepisanie
istniejących danych migracją, uruchomienie i naprawa zestawu testów, pobranie
i dopasowanie 228 cen dostawcy oraz przejrzenie i sklasyfikowanie zdjęć
katalogowych **prawie nie zostawiają po sobie kodu**. To nie jest argument za
podniesieniem ceny — to wyjaśnienie, dlaczego nie liczę tego projektu z linijek.

---

## 3. Widełki rynkowe

Stawki i przedziały z publikacji branżowych z 2026 roku (źródła na końcu).

| Kto wycenia | Stawka | Za 572 – 819 h |
|---|---|---:|
| Software house | 150 – 350 zł/h | **86 000 – 287 000** |
| Freelancer senior (B2B) | 140 – 220 zł/h | **80 000 – 180 000** |
| Utrzymanie i rozwój po wdrożeniu | 200 – 600 zł/h | — |

Dla porównania, publikowane widełki całych projektów:

- system średniej złożoności (kilka modułów, role, integracje): **80 000 – 250 000 zł**;
- rozbudowana platforma wielomodułowa: **250 000 zł i więcej**;
- sklep dedykowany na gotowym silniku: PrestaShop ok. **100 000 zł**, Magento ok. **150 000 zł**.

A.K. HOUSE ma dwanaście modułów frontendu, siedemdziesiąt trzy endpointy, dwa
panele administracyjne z rozdzielonymi uprawnieniami, płatności online i sklep
z wariantami — mieści się w górnej części pasma „średniej złożoności".

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
| O. Płatności online | 8 000 – 18 000 |
| P. Dane do faktury | 2 500 – 5 000 |
| Q. Klasy wysyłki jako zbiór | 4 000 – 8 000 |
| R. Warianty produktu | 6 000 – 13 000 |
| S. Karta produktu (SKU, parametry, galeria) | 6 000 – 12 000 |
| T. Wyszukiwanie i filtrowanie serwerowe | 5 000 – 11 000 |
| U. Dopracowanie sprzedażowe | 3 500 – 7 000 |
| V. Weryfikacja i narzędzia jakości | 3 000 – 6 000 |
| **Razem jako projekt komercyjny** | **130 000 – 290 000** |

Obie metody — stawka godzinowa i suma modułów — wskazują ten sam przedział.

> ### Realna wartość rynkowa obecnego stanu: **120 000 – 280 000 zł**

---

## 4. Cena

### 4.1. Co obejmuje oferta

Oferta jest **kompleksowa i ryczałtowa**: cena obejmuje nie tylko to, co już
powstało, ale doprowadzenie projektu do stanu, w którym **sklep i strona działają
publicznie pod własnym adresem, a studio może z nich korzystać bez mojego udziału**.

W cenie zawiera się:

| Zakres | Godziny | Wartość (87 zł/h) |
|---|---:|---:|
| Moduły A – V (obecny stan projektu) | 572 – 819 | 49 800 – 71 300 |
| Wdrożenie produkcyjne: hosting, domena, HTTPS, SMTP, kopie zapasowe | 25 – 40 | 2 200 – 3 500 |
| Uruchomienie płatności online: konto, weryfikacja, webhook, testy | 12 – 18 | 1 000 – 1 600 |
| Logowanie na konta operatorów zamiast kluczy w nagłówku | 40 – 60 | 3 500 – 5 200 |
| Dokończenie cennika: pozostałe 87 pozycji i uruchomienie sprzedaży | 15 – 25 | 1 300 – 2 200 |
| Odbiór, poprawki, szkolenie z paneli, przekazanie dostępów | 20 – 35 | 1 700 – 3 000 |
| **Razem** | **684 – 997 h** | **59 500 – 86 800** |

**Środek widełek — 800 h — daje cenę standardową 70 000 zł.**

### 4.2. Cena z rabatem partnerskim

Jeden ze współwłaścicieli A.K. HOUSE jest moim znajomym i będzie wykonywał
meble do mojego mieszkania. Z tego tytułu obniżam cenę.

| Pozycja | Kwota |
|---|---:|
| Cena standardowa (800 h × 87 zł) | 70 000 zł |
| **Rabat partnerski** | **−12 000 zł** |
| **Cena końcowa — ryczałt, wszystko wliczone** | **58 000 zł** |

Stawka pozostaje **87 zł/h** — poniżej dolnej granicy rynku dla freelancera
seniora i wielokrotnie poniżej stawek software house'ów. Rabat nie polega na
obniżeniu stawki, tylko na tym, że **w tej samej cenie mieści się więcej pracy**:
wdrożenie, uruchomienie płatności, utwardzenie logowania i doprowadzenie sklepu
do sprzedaży wchodzą w pakiet, zamiast być dopłatą.

Odniesienie do wartości rynkowej: **58 000 zł to 21 – 48% tego, co ten sam
zakres kosztowałby na rynku.**

### 4.3. Jak to wygląda przy poprzedniej wycenie

| | 23.08 | **27.08** |
|---|---:|---:|
| Zakres (godziny z wdrożeniem) | 555 – 808 | **684 – 997** |
| Cena standardowa | 59 000 | **70 000** |
| Cena partnerska | 48 000 | **58 000** |

Różnica **10 000 zł** to osiem nowych modułów: płatności online, faktury,
klasy wysyłki, warianty produktu, rozbudowana karta, wyszukiwanie serwerowe,
dopracowanie sprzedażowe i uruchomiona weryfikacja. Za każdy z osobna rynek
policzyłby od 2 500 do 18 000 zł — łącznie **38 000 – 80 000 zł**.

Jeżeli zakres z 23.08 był już uzgodniony, uczciwym rozwiązaniem jest rozmowa
o tej różnicy osobno, a nie przedstawianie nowej ceny jako podwyżki.

### 4.4. Co dokładnie dostaje klient

- Działającą stronę i sklep pod własną domeną, na skonfigurowanym serwerze,
  z certyfikatem HTTPS i działającą wysyłką e-maili.
- Sklep z uzupełnionym cennikiem, gotowy do przyjmowania zamówień, z płatnością
  kartą, BLIK-iem, Przelewami24 i przelewem tradycyjnym.
- Trzy panele: obsługa zleceń, sprzedaż, zdjęcia — każdy z osobnym dostępem
  i **logowaniem na konto operatora**, nie kluczem w nagłówku.
- Cały kod źródłowy i prawa do niego.
- Dokumentację techniczną i instrukcję obsługi paneli.
- Szkolenie z obsługi (zdalne, do 2 godzin).
- Dwie rundy poprawek po odbiorze.
- Projekt w stanie, w którym inny programista może go przejąć.

### 4.5. Czego cena nie obejmuje

Żeby „wszystko wliczone" znaczyło coś konkretnego, tu jest druga strona listy:

- **Opłat zewnętrznych**: domena, hosting, certyfikat wykraczający poza Let's Encrypt,
  konto SMTP, prowizje operatora płatności (Stripe pobiera je od transakcji).
- **Treści**: zdjęcia produktów, opisy, teksty marketingowe, tłumaczenia
  poza już istniejącymi.
- **Obsługi prawnej**: regulamin i polityka zwrotów są wzorcami do weryfikacji
  przez prawnika. Sklep sprzedaje konsumentom — to nie jest miejsce na oszczędność.
- **Wznowienia konfiguratora 3D** — szkielet zostaje, ale jego dokończenie to
  osobny temat i osobna wycena.
- **Utrzymania po odbiorze** — patrz niżej.

### 4.6. Po wdrożeniu

Rynek za utrzymanie i rozwój liczy 200 – 600 zł/h. Proponuję **120 zł/h** na
zmiany zgłaszane po odbiorze, bez abonamentu i bez minimalnego pakietu godzin —
płacisz za to, co faktycznie zrobione.

Poprawki błędów w tym, co zostało wydane, są **bezpłatne przez 3 miesiące** od
odbioru. Błąd to działanie niezgodne z tym, co zostało uzgodnione — nie nowa
funkcja, o której nikt wcześniej nie mówił.

---

## 5. Co jeszcze zostało do zrobienia

Uczciwość wymaga powiedzenia, że projekt nie jest skończony. Otwarte punkty,
wszystkie wliczone w cenę z punktu 4.1:

- **87 ze 155 produktów nie ma jeszcze ceny detalicznej dostawcy** — 30 pozycji
  czeka na potwierdzenie dopasowania, 57 nie ma odpowiednika u Balii i wymaga
  ręcznej wyceny.
- **Klucze API do zastąpienia kontami operatorów.** Obecny model działa, ale
  klucz w nagłówku dzieli się jak hasło i nie da się go odebrać jednej osobie.
- **Wdrożenie produkcyjne** — projekt działa lokalnie; serwer, domena i poczta
  czekają na decyzje.
- **Konto Stripe i webhook** — kod jest gotowy i wyłączony do czasu podpięcia konta.
- **Weryfikacja prawna regulaminu i polityki zwrotów.**
- Dane kontaktowe: telefon, WhatsApp i profile społecznościowe są tymczasowe.

---

## 6. Źródła stawek

Widełki z punktu 3 pochodzą z publikowanych w 2026 roku zestawień stawek
polskich software house'ów i freelancerów oraz z cenników projektów wdrożeniowych
na PrestaShop i Magento. Zebrane 23.08.2026 i niezmienione w tej wersji —
cztery dni nie robią różnicy na rynku, który porusza się rocznie.

Kwoty modułowe z tabeli „wartość odtworzenia" są moim oszacowaniem tego, ile
kosztowałoby zamówienie każdego elementu osobno, przy dolnej i górnej stawce
z tego samego przedziału.
