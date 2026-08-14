# Program zajęć — mapa całego roku

Ten plik jest mapą treści dla czterech ścieżek zajęć. Każdy wiersz tabeli odpowiada
jednemu plikowi konspektu w formacie importera (`docs/program/<ścieżka>/lNN-*.md`),
gotowemu do wgrania przez ekran importu.

## Kształt pojedynczych zajęć

Wszystkie konspekty mają ten sam kształt, bo taki jest kształt terminu w systemie:
**45 min + 5 min przerwy + 45 min, razem 95 minut**. Parser importu pilnuje tego
sam — blok dłuższy niż 45 minut zgłasza jako uwagę.

Stały rytm zajęć (kolejność typów kroków w pliku):

1. `[intro]` — powitanie, obecność, demo dzisiejszego efektu (10-15 min),
2. `[review]` — przypomnienie z poprzednich zajęć (5 min, od lekcji 2.),
3. `[concept]` — jeden nowy element, pokazany na żywo (10-15 min),
4. `[guided]` — dzieci robią to samo krok w krok razem z prowadzącym (15 min),
5. `[break]` — przerwa (5 min),
6. `[concept]` lub `[demo]` — drugi nowy element (10-15 min),
7. `[challenge]` — samodzielne zadanie na dzisiejszym materiale (15 min),
8. `[challenge]` — pokaz prac, „scena dla każdego” (10 min),
9. `[summary]` — pułapki, zapis projektu, zajawka następnych zajęć (5 min).

Przerwa wypada celowo **przed najatrakcyjniejszą częścią**, a nie po niej — dzieci
wracają od ekranu prosto do najciekawszego fragmentu i łatwiej się ponownie skupiają.

## Zasady, których trzymają się wszystkie konspekty

Zajęcia są zdalne: dziecko siedzi samo przed swoim komputerem, a prowadzący widzi
tylko to, co dziecko pokaże. Z tego wynika kilka reguł, które w konspektach wracają:

- **Nigdy więcej niż trzy kroki instrukcji bez sprawdzenia.** Zdalnie nie widać, że
  dziecko zgubiło się przy drugim bloczku. W każdym kroku `[guided]` jest jawny
  moment sprawdzenia („pokaż kciuk”, „wrzuć na czat literę G”).
- **Efekt widoczny w pierwszych 10 minutach.** Zajęcia zaczynają się od demo
  gotowego efektu, a nie od teorii — dziecko ma pomyśleć „chcę to mieć”.
- **Każda lekcja kończy się czymś, co da się pokazać rodzicowi.** To jest paliwo
  frekwencji, nie ozdobnik.
- **Dwie ścieżki tempa w każdym kroku.** `[dla szybszych]` i `[gdy nie zdążysz]` są
  obowiązkowe wszędzie tam, gdzie dzieci pracują samodzielnie — w grupie zdalnej
  różnica temp jest większa niż w sali.
- **Nazwy bloczków po polsku**, bo dzieci pracują na polskiej wersji Scratcha.
  Przy pierwszym użyciu podajemy też nazwę angielską w nawiasie.
- **Prowadzący nie zakłada, że dziecko umie obsłużyć plik.** Zapis, otwarcie i
  odnalezienie projektu z poprzednich zajęć są osobnymi punktami scenariusza.

## Rok w podziale na moduły

Rok to **48 tygodni**: 36 tygodni rdzenia (wrzesień-czerwiec) plus 12-tygodniowy
moduł letni. Kurs 36-tygodniowy to dokładnie rdzeń — moduł letni jest domykającą
się całością i można go sprzedać osobno albo pominąć bez luki w programie.

| Moduł | Tygodnie | Rola |
|---|---|---|
| Semestr 1 | 1-12 | podstawy środowiska, pierwsza własna gra |
| Semestr 2 | 13-24 | logika: zmienne, warunki, komunikaty |
| Semestr 3 | 25-36 | większe projekty i projekt roczny |
| Moduł letni | 37-48 | projekty tematyczne, mniej nowego materiału |

Tygodnie 12., 24. i 36. to **pokaz prac dla rodziców** — zajęcia bez nowego
materiału, z zaproszonymi rodzicami na ostatnie 20 minut.

---

## Ścieżka A — Scratch, 7-9 lat

Katalog: `docs/program/scratch-7-9/`

Dzieci na tym poziomie często nie czytają jeszcze płynnie. Konspekty opisują
bloczki **po kolorze i kształcie**, a nie tylko po nazwie, i unikają wpisywania
z klawiatury tam, gdzie da się wybrać z listy.

### Semestr 1 — pierwsza gra (tygodnie 1-12)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 1 | Poznajemy Scratcha — pierwszy ruch kota | kot, który chodzi po kliknięciu flagi |
| 2 | Kostiumy i animacja | postać, która naprawdę idzie |
| 3 | Sterowanie strzałkami | postać sterowana z klawiatury |
| 4 | Tło, scena i nastrój | scenka z tłem i muzyką |
| 5 | Dymki i rozmowa postaci | dialog dwóch postaci |
| 6 | Pętla „powtórz” — taniec | układ taneczny na spację |
| 7 | Dźwięki i efekty graficzne | postać zmieniająca kolor i rozmiar |
| 8 | Losowanie — postać skacze po scenie | zabawa „złap postać” |
| 9 | Dotknięcie i znikanie | zbieranie przedmiotów |
| 10 | Licznik punktów | gra z wynikiem na ekranie |
| 11 | Koniec gry i wygrana | pełna mini-gra z zakończeniem |
| 12 | Pokaz prac dla rodziców | dopracowana gra i prezentacja |

### Semestr 2 — logika i opowieść (tygodnie 13-24)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 13 | Zmienna — czym jest pudełko na liczbę | licznik życia |
| 14 | Warunek „jeżeli” | postać reagująca na kolor |
| 15 | Czas i minutnik | gra na czas |
| 16 | Dwie sceny, jedna historia | zmiana tła komunikatem |
| 17 | Komunikaty między postaciami | postacie odpowiadające sobie |
| 18 | Rysowanie pisakiem | wzory geometryczne |
| 19 | Grawitacja dla najmłodszych | postać, która spada i skacze |
| 20 | Platformy i podłoga | prosta platformówka |
| 21 | Przeciwnik, który goni | pościg na scenie |
| 22 | Poziomy trudności | gra z dwoma poziomami |
| 23 | Ekran startowy i instrukcja | gra z menu |
| 24 | Pokaz prac dla rodziców | platformówka i prezentacja |

### Semestr 3 — własne projekty (tygodnie 25-36)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 25 | Kreskówka — planowanie scen | plan historyjki na kartce |
| 26 | Kreskówka — budowa scen | trzy sceny animacji |
| 27 | Kreskówka — dźwięk i głos | animacja z nagraną kwestią |
| 28 | Quiz — pytania i odpowiedzi | quiz z trzema pytaniami |
| 29 | Quiz — punktacja i wynik | quiz z oceną końcową |
| 30 | Klony — deszcz przedmiotów | efekt padającego deszczu |
| 31 | Gra zręcznościowa — start projektu | plan i sterowanie |
| 32 | Gra zręcznościowa — przeciwnicy | przeszkody i kolizje |
| 33 | Gra zręcznościowa — wynik i rekord | zapis najlepszego wyniku |
| 34 | Projekt roczny — praca własna I | wybrany projekt, część 1 |
| 35 | Projekt roczny — praca własna II | projekt gotowy do pokazu |
| 36 | Wielki pokaz roczny | prezentacja projektu rodzicom |

### Moduł letni (tygodnie 37-48)

| # | Temat |
|---|---|
| 37 | Wakacyjna pocztówka — animowany widok |
| 38 | Gra „unikaj przeszkód” |
| 39 | Muzyczna maszyna — bloczki dźwiękowe |
| 40 | Zwierzak wirtualny — karmienie i nastrój |
| 41 | Labirynt — budowa planszy |
| 42 | Labirynt — sterowanie i meta |
| 43 | Rysowanie pisakiem — mandala |
| 44 | Gra „papier, kamień, nożyce” |
| 45 | Wyścig dwóch postaci |
| 46 | Kalkulator dla kota |
| 47 | Projekt wakacyjny — praca własna |
| 48 | Pokaz projektów wakacyjnych |

---

## Ścieżka B — Scratch, 10-12 lat

Katalog: `docs/program/scratch-10-12/`

Ten sam język, inny poziom abstrakcji. Zmienne pojawiają się w pierwszym
semestrze, klony i własne bloki w drugim. Dzieci mają czytać komunikaty
o błędach jako informację, a nie porażkę — stąd w konspektach osobne
kroki na szukanie błędu w cudzym projekcie.

### Semestr 1 — od ruchu do gry ze stanem (tygodnie 1-12)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 1 | Scratch w 45 minut — szybki start | sterowana postać i tło |
| 2 | Współrzędne i precyzyjny ruch | postać poruszająca się po torze |
| 3 | Pętle i zagnieżdżanie | wzory z pisaka |
| 4 | Zmienne — wynik, życie, czas | plansza z trzema wskaźnikami |
| 5 | Warunki i operatory logiczne | reguły gry zapisane warunkami |
| 6 | Kolizje i reakcja na dotknięcie | zbieranie i tracenie życia |
| 7 | Losowość i tempo gry | rosnąca trudność |
| 8 | Klony — wiele obiektów naraz | strumień przeciwników |
| 9 | Komunikaty i stany gry | ekran startu, gry i końca |
| 10 | Dźwięk, efekty i „czucie” gry | dopracowana warstwa audiowizualna |
| 11 | Testowanie i szukanie błędów | lista poprawek i naprawiona gra |
| 12 | Pokaz prac dla rodziców | pierwsza pełna gra |

### Semestr 2 — struktury i platformówka (tygodnie 13-24)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 13 | Własne bloki — porządek w kodzie | kod podzielony na nazwane części |
| 14 | Własne bloki z argumentami | uniwersalna procedura rysująca |
| 15 | Listy — wiele wartości w jednym miejscu | lista pytań quizu |
| 16 | Listy w praktyce — tablica wyników | ranking najlepszych wyników |
| 17 | Platformówka — grawitacja i skok | fizyka postaci |
| 18 | Platformówka — kolizje z podłożem | postać stojąca na platformach |
| 19 | Platformówka — poziomy i kostiumy tła | dwa poziomy do przejścia |
| 20 | Kamera podążająca za postacią | przewijana plansza |
| 21 | Przeciwnicy ze wzorcem ruchu | patrolujący wróg |
| 22 | Punkty kontrolne i respawn | wznowienie gry po śmierci |
| 23 | Balans i testy z kolegą | gra przetestowana przez inne dziecko |
| 24 | Pokaz prac dla rodziców | platformówka z dwoma poziomami |

### Semestr 3 — projekty i publikacja (tygodnie 25-36)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 25 | Projekt zespołowy — podział pracy | plan gry i przydział zadań |
| 26 | Projekt zespołowy — remiks i łączenie | wspólny projekt w studiu |
| 27 | Symulacja — ruch stada | grupa obiektów o wspólnej regule |
| 28 | Sztuczny przeciwnik — pościg i ucieczka | wróg reagujący na gracza |
| 29 | Gra turowa — logika bez zręczności | prosta gra planszowa |
| 30 | Interfejs użytkownika — suwaki i przyciski | ekran ustawień gry |
| 31 | Zapisywanie postępu | wynik pamiętany między sesjami |
| 32 | Optymalizacja — dlaczego gra zwalnia | projekt działający płynnie |
| 33 | Publikacja i licencja pracy | projekt udostępniony w sieci |
| 34 | Projekt roczny — praca własna I | projekt w połowie |
| 35 | Projekt roczny — praca własna II | projekt gotowy |
| 36 | Wielki pokaz roczny | prezentacja z opisem, jak to działa |

### Moduł letni (tygodnie 37-48)

| # | Temat |
|---|---|
| 37 | Gra rytmiczna |
| 38 | Wyścig z torem i czasem okrążenia |
| 39 | Generator poziomów |
| 40 | Gra strategiczna — wieża obronna, część 1 |
| 41 | Gra strategiczna — wieża obronna, część 2 |
| 42 | Fraktale i rekurencja pisakiem |
| 43 | Szyfrowanie wiadomości |
| 44 | Wykresy z listy danych |
| 45 | Gra tekstowa z wyborami |
| 46 | Konwersja projektu na telefon — sterowanie dotykiem |
| 47 | Projekt wakacyjny — praca własna |
| 48 | Pokaz projektów wakacyjnych |

---

## Ścieżka C — Minecraft Education, bloki, 8-10 lat

Katalog: `docs/program/minecraft-bloki/`

Środowisko: **Minecraft Education** z Code Builder i edytorem MakeCode w trybie
bloków. Nacisk na **budowanie światów** — kod jest narzędziem do stawiania
budowli, a nie tematem samym w sobie. Agent jest głównym bohaterem kursu.

Kurs wymaga działających licencji Minecraft Education dla całej grupy — pierwsze
zajęcia mają osobny krok na sprawdzenie logowania, bo zdalnie to najczęstsza
przyczyna straconych 30 minut.

### Semestr 1 — świat, Agent, pierwsze budowle (tygodnie 1-12)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 1 | Logowanie, świat i pierwsze rozejrzenie się | własny świat w trybie kreatywnym |
| 2 | Poruszanie się i stawianie bloków ręcznie | mały domek |
| 3 | Code Builder — poznajemy Agenta | Agent chodzący na komendę |
| 4 | Agent stawia bloki | ścieżka z bloków |
| 5 | Pętla „powtórz” — długi mur | mur o zadanej długości |
| 6 | Skręcanie — kwadrat i prostokąt | fundament budynku |
| 7 | Pętla w pętli — ściana i podłoga | pełna podłoga pomieszczenia |
| 8 | Budowanie w górę — piętra | dwupiętrowa wieża |
| 9 | Agent kopie i zbiera | wykop pod fundament |
| 10 | Materiały i wybór bloku | budowla z kilku materiałów |
| 11 | Własna budowla — projekt na kartce | zaprojektowany i postawiony obiekt |
| 12 | Pokaz prac dla rodziców | zwiedzanie zbudowanych światów |

### Semestr 2 — teren, wieś i mechanizmy (tygodnie 13-24)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 13 | Zmienna — jedna liczba, wiele budowli | budowla o zmiennym rozmiarze |
| 14 | Warunek — Agent sprawdza, co przed nim | Agent omijający przeszkodę |
| 15 | Wypełnianie obszaru | plac i chodnik |
| 16 | Kształtowanie terenu | wyrównana działka |
| 17 | Most nad przepaścią | most zbudowany kodem |
| 18 | Woda, lawa i bezpieczeństwo budowli | fosa wokół zamku |
| 19 | Wieś — plan ulic | siatka ulic |
| 20 | Wieś — domy z jednego przepisu | rząd identycznych domów |
| 21 | Ogród i farma | poletko z uprawą |
| 22 | Oświetlenie i noc | oświetlona osada |
| 23 | Wspólna budowa — świat całej grupy | jedna wieś zbudowana przez grupę |
| 24 | Pokaz prac dla rodziców | wycieczka po wsi |

### Semestr 3 — duże projekty (tygodnie 25-36)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 25 | Zamek — plan i mury | mury obronne |
| 26 | Zamek — wieże i baszty | narożne wieże |
| 27 | Zamek — wnętrze i sala tronowa | wykończone wnętrze |
| 28 | Labirynt — budowa ścian | labirynt do przejścia |
| 29 | Tor przeszkód dla kolegów | tor przetestowany przez grupę |
| 30 | Muzeum — sale i eksponaty | muzeum z tabliczkami |
| 31 | Park rozrywki — kolejka i karuzela | działająca atrakcja |
| 32 | Podwodne miasto | kopuła pod wodą |
| 33 | Latająca wyspa | wyspa w chmurach |
| 34 | Projekt roczny — praca własna I | rozpoczęta budowla |
| 35 | Projekt roczny — praca własna II | gotowy świat |
| 36 | Wielki pokaz roczny | oprowadzanie rodziców po świecie |

### Moduł letni (tygodnie 37-48)

| # | Temat |
|---|---|
| 37 | Wakacyjny kurort nad morzem |
| 38 | Statek i port |
| 39 | Piramida i grobowiec |
| 40 | Stacja kosmiczna |
| 41 | Kolejka górska |
| 42 | Zoo z wybiegami |
| 43 | Wieżowiec — budowanie w pionie |
| 44 | Mapa Polski z bloków |
| 45 | Wioska w drzewach |
| 46 | Park wodny ze zjeżdżalniami |
| 47 | Projekt wakacyjny — praca własna |
| 48 | Pokaz projektów wakacyjnych |

---

## Ścieżka D — Minecraft Education, kod, 10-12 lat

Katalog: `docs/program/minecraft-kod/`

To samo środowisko co ścieżka C, ale dzieci pracują w **tekstowym widoku
MakeCode** (JavaScript) i od 25. tygodnia porównują go z Pythonem. Zasada
kursu zostaje ta sama: budujemy światy, a kod jest do tego narzędziem.
Różnica polega na tym, że budowle są **generowane** — parametrem, funkcją,
pętlą — a nie klikane.

Ścieżka zakłada, że dziecko przeszło kurs bloków albo Scratcha. Dla dziecka
bez tego tła pierwsze cztery lekcje są za szybkie — takie dziecko lepiej
zaczyna od ścieżki C.

### Semestr 1 — od bloków do tekstu (tygodnie 1-12)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 1 | Ten sam kod, dwa widoki — bloki i tekst | pierwszy własnoręcznie napisany wiersz |
| 2 | Składnia bez tajemnic — nawiasy i średniki | poprawiony kod z celowymi błędami |
| 3 | Zmienne i typy | budowla o parametrach z jednej linijki |
| 4 | Pętla `for` i zakres | mur o dowolnej długości |
| 5 | Pętle zagnieżdżone — ściana i podłoga | pomieszczenie z jednej pętli |
| 6 | Funkcje — własny przepis na budowlę | funkcja stawiająca domek |
| 7 | Funkcje z parametrami | domek o zadanym rozmiarze |
| 8 | Współrzędne bezwzględne i względne | budowla stawiana w dowolnym miejscu |
| 9 | Warunki i decyzje w terenie | kod dopasowujący budowlę do gruntu |
| 10 | Tablice — lista materiałów | budowla w losowych materiałach |
| 11 | Czytanie błędów i debugowanie | naprawiony cudzy program |
| 12 | Pokaz prac dla rodziców | generator budowli |

### Semestr 2 — generowanie świata (tygodnie 13-24)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 13 | Generator ulicy — funkcja w pętli | ulica domów |
| 14 | Losowość kontrolowana | dzielnica bez dwóch takich samych domów |
| 15 | Wysokość terenu i dopasowanie budowli | osada na wzgórzu |
| 16 | Wieża z pętli — spirala schodów | wieża ze schodami |
| 17 | Kopuła i kula z równania | kopuła nad miastem |
| 18 | Piramida — pętla malejąca | piramida schodkowa |
| 19 | Most parametryczny | most o zadanej rozpiętości |
| 20 | Labirynt generowany kodem | losowy labirynt |
| 21 | Zdarzenia — reakcja na gracza | pułapka wyzwalana wejściem |
| 22 | Czat jako sterowanie | budowle na komendę z czatu |
| 23 | Porządkowanie kodu — czytelność i nazwy | program zrozumiały dla kolegi |
| 24 | Pokaz prac dla rodziców | wygenerowane miasto |

### Semestr 3 — projekty i drugi język (tygodnie 25-36)

| # | Temat | Co dziecko ma na koniec |
|---|---|---|
| 25 | Python obok JavaScriptu — ten sam program | ten sam efekt w dwóch językach |
| 26 | Python — wcięcia zamiast nawiasów | budowla napisana w Pythonie |
| 27 | Projekt miasta — architektura programu | podział na funkcje |
| 28 | Projekt miasta — dzielnice | trzy dzielnice z jednego programu |
| 29 | Projekt miasta — infrastruktura | drogi, most, oświetlenie |
| 30 | Gra w świecie — zasady i punkty | minigra na własnej mapie |
| 31 | Gra w świecie — start, koniec, wynik | pełna pętla rozgrywki |
| 32 | Wydajność — dlaczego świat się zacina | program działający szybciej |
| 33 | Dzielenie się kodem i światem | projekt oddany innej grupie |
| 34 | Projekt roczny — praca własna I | rozpoczęty projekt |
| 35 | Projekt roczny — praca własna II | gotowy projekt |
| 36 | Wielki pokaz roczny | prezentacja świata i kodu |

### Moduł letni (tygodnie 37-48)

| # | Temat |
|---|---|
| 37 | Generator wysp |
| 38 | Symulacja pogody w świecie |
| 39 | Automatyczna farma |
| 40 | Kolej i tory generowane kodem |
| 41 | Statek kosmiczny i baza na Księżycu |
| 42 | Sortowanie i wykresy z bloków |
| 43 | Gra „szukanie skarbu” z podpowiedziami |
| 44 | Rekurencja — drzewo fraktalne z bloków |
| 45 | Świat z pliku — dane sterujące budową |
| 46 | Wieloosobowa mapa do zabawy |
| 47 | Projekt wakacyjny — praca własna |
| 48 | Pokaz projektów wakacyjnych |

---

## Jak dziecko przechodzi między ścieżkami

- **Scratch 7-9 → Scratch 10-12**: po ukończeniu rdzenia (36 tygodni) i wieku
  co najmniej 10 lat. Dziecko, które w semestrze 3. samodzielnie zbudowało
  projekt roczny, jest gotowe niezależnie od wieku.
- **Scratch → Minecraft bloki**: w każdym momencie, to ścieżka równoległa,
  nie wyższy poziom.
- **Minecraft bloki → Minecraft kod**: po semestrze 2. ścieżki C albo po roku
  Scratcha 10-12.
- **Dziecko, które zna już materiał**: konspekty mają wskazówki `[dla szybszych]`
  w każdym kroku samodzielnej pracy. Jeśli dziecko wyczerpuje je regularnie
  przez trzy zajęcia z rzędu, to sygnał do rozmowy o zmianie grupy, a nie do
  wymyślania dodatkowych zadań na żywo.

## Stan przygotowania konspektów

| Ścieżka | Gotowe pliki |
|---|---|
| A — Scratch 7-9 | lekcje 1-8 |
| B — Scratch 10-12 | lekcje 1-8 |
| C — Minecraft bloki | lekcje 1-8 |
| D — Minecraft kod | lekcje 1-8 |

Bieżący zakres prac to **pierwsze osiem tygodni każdej ścieżki**, czyli dwa miesiące zajęć
dla grupy startującej od zera. Reszta rdzenia powstaje po zebraniu uwag od prowadzących.

Ścieżka Roblox Studio jest odłożona — wymaga osobnego przygotowania
środowiska i licencji.
