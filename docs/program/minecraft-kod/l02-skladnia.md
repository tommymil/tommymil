# Składnia bez tajemnic — nawiasy, klamry, przecinki
Subject: Minecraft Education
Level: Poziom 2 — 10-12 lat, kod
Czas: 95 min
Tags: Składnia, Debugowanie, Komunikaty błędów, Naprawianie kodu
Opis: Lekcja o znakach, które w kodzie coś znaczą. Dzieci dostają celowo zepsute programy i uczą się czytać komunikaty błędów zamiast się ich bać.
Cel: Dziecko rozpoznaje trzy podstawowe znaki składni, potrafi naprawić program po komunikacie błędu i wie, że błąd składniowy to informacja, a nie porażka.

### Po zajęciach dziecko potrafi
- wyjaśnić, do czego służą nawiasy okrągłe, klamry i cudzysłowy
- naprawić program z brakującym nawiasem, klamrą albo cudzysłowem
- przetłumaczyć trzy najczęstsze komunikaty błędów na polski
- odróżnić błąd składniowy od logicznego

### Przygotuj przed zajęciami
- **pięć celowo zepsutych programów** przygotowanych do wklejenia na czat, po jednym błędzie w każdym
- lista trzech najczęstszych komunikatów błędów z Twojego edytora, z tłumaczeniem
- światy dzieci z lekcji 1
- gotowe demo: program, który wygląda dobrze, a nie działa, bo Agent robi coś innego

### Zadanie domowe
- zepsuj własny program w trzech miejscach, zapisz co zepsułeś, a potem napraw bez zaglądania w notatki

## [intro] Powitanie i demo dwóch rodzajów błędów (8 min)

### Co robić teraz
- [mów] Cześć. Dziś lekcja o czymś, czego wszyscy się boją, a co jest najbardziej pomocną rzeczą w programowaniu: o błędach.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO — błąd pierwszego rodzaju.** Pokaż program z brakującym nawiasem. Edytor podkreśla na czerwono, program nie rusza.
- [mów] To jest błąd składniowy. Napisaliście coś, czego edytor nie rozumie. Dobra wiadomość: on wam **mówi**, gdzie.
- **DEMO — błąd drugiego rodzaju.** Pokaż program bez żadnych podkreśleń, który buduje mur w złą stronę.
- [mów] A to jest błąd logiczny. Napisaliście coś, co edytor rozumie doskonale — tylko że to nie jest to, o co wam chodziło.
- [mów] Pierwszy rodzaj naprawia się w minutę. Drugi bywa trudniejszy. Dziś zajmiemy się głównie pierwszym.
- Odczytaj kilka zadań domowych: kto przepisał program z bloczków i czy się zgadzało.

### Wskazówki
- [podpowiedź] Rozróżnienie „edytor mnie nie rozumie” kontra „edytor mnie rozumie, ale ja się pomyliłem” jest głównym pojęciem tej lekcji. Wraca do końca kursu.
- [tempo] Pokazanie obu rodzajów obok siebie na starcie jest ważniejsze niż jakiekolwiek tłumaczenie.

## [review] Wracamy do edytora (5 min)

### Co robić teraz
- Wszyscy otwierają świat z lekcji 1 i Code Builder w widoku JavaScript. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie program ze ścieżką z zeszłego tygodnia.
- Szybka powtórka: co zrobić, gdy nie wiesz, jak coś napisać? Co oznacza czerwone podkreślenie? Co znaczy, że tekst nagle jest cały w jednym kolorze?
- Wszyscy odlatują na puste miejsce.

### Wskazówki
- [błąd] Program z zeszłego tygodnia zniknął, bo dziecko założyło nowy świat. Kod mieszka w świecie — przypomnij i pozwól odtworzyć, to trzy minuty.
- [gdy nie zdążysz] Sam start i przelot.

## [concept] Trzy rodzaje nawiasów (14 min)

### Co robić teraz
- [mów] W kodzie każdy znak coś znaczy. Dziś poznacie trzy najważniejsze i to wystarczy na pół roku.
- **Klamry** — otwierają i zamykają blok, czyli grupę linijek należących do czegoś.
- Pokaż na przykładzie: wszystko, co jest w klamrach po komendzie na czacie, wykona się po jej wpisaniu.
- [mów] Klamra to jest to, co w bloczkach było ramką. Bloczek obejmował inne bloczki — tutaj klamry obejmują linijki.
- **Nawiasy okrągłe** — zawierają to, co polecenie ma dostać. Nazywa się to argumentami.
- Pokaż: ruch w przód dostaje kierunek i liczbę kroków. Oba w nawiasie, oddzielone przecinkiem.
- [mów] Każde polecenie ma nawias, nawet jeśli nic w nim nie ma. Przeniesienie Agenta do gracza niczego nie potrzebuje, ale nawias i tak jest.
- **Cudzysłowy** — otaczają tekst, czyli coś, co ma być odczytane dosłownie, a nie zrozumiane jako polecenie.
- Pokaż: nazwa komendy jest w cudzysłowie, bo to jest po prostu słowo, a nie polecenie do wykonania.
- Ćwiczenie na czat. Wskazuję fragment kodu, wy piszecie, jaki to znak i po co tam jest. Pięć rund.
- [mów] Zasada, którą warto zapamiętać: **każdy otwierający ma swój zamykający**. Zawsze. Nawias, klamra, cudzysłów.

### Wskazówki
- [podpowiedź] Powiązanie klamer z ramkami bloczków jest tu najważniejsze. Dzieci po kursie bloków rozumieją to natychmiast, gdy padnie porównanie.
- [błąd] Dziecko myli klamry z nawiasami okrągłymi. Pokaż je obok siebie, powiększone.
- [błąd] Dziecko nie wie, gdzie na klawiaturze są klamry. Na polskiej klawiaturze to shift z nawiasem kwadratowym — pokaż, bo to realna przeszkoda.
- [podpowiedź] Edytor podświetla parę do nawiasu, gdy postawisz przy nim kursor. Pokaż to — bardzo pomaga przy szukaniu brakującej pary.
- [dla szybszych] Niech znajdą w swoim kodzie miejsce, w którym w jednej linijce są wszystkie trzy rodzaje znaków.

### Materiały
- [kod] Trzy rodzaje znaków w jednym programie | javascript:
  ```
  player.onChat("mur", function () {
      agent.teleportToPlayer()
      agent.setItem(STONE, 64, 1)
      agent.move(FORWARD, 5)
  })
  ```

## [guided] Naprawiamy zepsute programy (18 min)

### Co robić teraz
- [mów] Teraz dostaniecie ode mnie pięć zepsutych programów. W każdym jest dokładnie jeden błąd. Waszym zadaniem jest go znaleźć i naprawić.
- Zasada pracy: najpierw **przeczytaj komunikat**, potem szukaj. Nie zgaduj.
- Program 1: brakuje klamry zamykającej. Wklej na czat, dzieci wklejają do edytora i naprawiają.
- Po naprawie zapytaj, kto przeczytał komunikat przed szukaniem.
- Program 2: brakuje nawiasu zamykającego przy poleceniu ruchu.
- Program 3: brakuje cudzysłowu przy nazwie komendy. [mów] Zwróćcie uwagę, jak zachowują się kolory. To jest najszybsza wskazówka.
- Program 4: brakuje przecinka między argumentami w nawiasie.
- Program 5: polecenie napisane z błędną wielkością liter.
- Po każdym programie zbierz na czat literę N od tych, którzy naprawili, i zapytaj jedno dziecko, jak znalazło błąd.
- [mów] Zauważcie, że wszystkie pięć błędów to były **brakujące albo pomylone znaki**. Nie logika, nie pomysł. Same znaki.

### Wskazówki
- [podpowiedź] To ćwiczenie jest sercem lekcji. Przygotuj programy przed zajęciami i miej je gotowe do wklejenia — improwizowanie ich na żywo zjada czas.
- [podpowiedź] Pytanie „jak znalazłeś?” po każdym programie jest ważniejsze niż samo naprawienie. Dzieci uczą się od siebie nawzajem metody, nie odpowiedzi.
- [błąd] Dziecko naprawia błąd przez skasowanie całej linijki. Działa, ale nie o to chodzi. Poproś o cofnięcie i znalezienie prawdziwej przyczyny.
- [błąd] Dziecko wkleja program i dostaje inne błędy, niż powinno, bo wkleiło do środka istniejącego kodu. Ustal: przed wklejeniem czyścimy edytor.
- [błąd] Czat gry albo czat spotkania psuje formatowanie wklejanego kodu. Sprawdź to przed zajęciami i miej plan B — udostępnienie ekranu i przepisywanie.
- [dla szybszych] Niech sami przygotują zepsuty program dla kolegi i wkleją go na czat.
- [gdy nie zdążysz] Trzy programy zamiast pięciu. Zostaw ten z cudzysłowem — jest najbardziej pouczający.

### Materiały
- [kod] Program 1 — znajdź błąd | javascript:
  ```
  player.onChat("test", function () {
      agent.teleportToPlayer()
      agent.move(FORWARD, 3)
  ```
- [kod] Program 3 — znajdź błąd | javascript:
  ```
  player.onChat("test, function () {
      agent.teleportToPlayer()
      agent.move(FORWARD, 3)
  })
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały edytora.
- Zapowiedź: „Po przerwie zajmiemy się trudniejszym rodzajem błędów — tymi, których edytor nie widzi.”

### Wskazówki
- [tempo] Szukanie błędów jest męczące i frustrujące. Przerwa po tym ćwiczeniu jest naprawdę potrzebna.

## [concept] Błędy, których edytor nie widzi (13 min)

### Co robić teraz
- [mów] Teraz trudniejsza część. Program bez podkreśleń, który robi coś innego, niż chcecie.
- **Przykład 1:** program buduje mur, ale w drugą stronę, niż dziecko chciało. Przyczyna: Agent patrzy inaczej, bo gracz stał inaczej.
- **Przykład 2:** program buduje ścieżkę o jeden blok za krótką. Przyczyna: liczba w poleceniu.
- **Przykład 3:** Agent nie stawia nic. Przyczyna: pusty ekwipunek albo zły slot.
- [mów] Edytor nie może wam pomóc w żadnym z tych przypadków, bo wszystko jest napisane poprawnie. Musicie sobie pomóc sami.
- Podaj trzy metody i zapisz je na udostępnionym ekranie — będą używane do końca kursu.
- **Zmniejsz liczby.** Zamiast pięćdziesięciu powtórzeń sprawdź na trzech. Błąd widać szybciej.
- **Sprawdź, gdzie stoi Agent.** Po uruchomieniu podejdź i zobacz, gdzie skończył i w którą stronę patrzy.
- **Czytaj kod na głos, linijka po linijce**, i mów, co robi. Bardzo często błąd znajduje się w trakcie czytania.
- Ćwiczenie: dam wam program, który działa, ale buduje coś innego, niż zapowiadam. Znajdźcie różnicę.
- [mów] Ta trzecia metoda brzmi śmiesznie i jest najskuteczniejsza. Programiści na całym świecie tłumaczą swój kod na głos, czasem nawet gumowej kaczce.

### Wskazówki
- [podpowiedź] Metoda czytania kodu na głos jest zaskakująco skuteczna i dzieci ją lubią. Zrób z tego rytuał na kolejnych lekcjach.
- [błąd] Dziecko szuka błędu logicznego, wpatrując się w cały program naraz. Skieruj je na czytanie linijka po linijce.
- [błąd] Dziecko zmienia kilka rzeczy naraz i nie wie, co pomogło. Zasada jednej zmiany naraz — powiedz ją wprost.
- [błąd] Program działa u Ciebie, a nie u dziecka. Prawie zawsze różnica jest w tym, gdzie stoi gracz albo co jest w ekwipunku Agenta.
- [dla szybszych] Niech przygotują program z błędem logicznym dla kolegi i opiszą słowami, co program **miał** robić.
- [gdy nie zdążysz] Same trzy metody, bez ćwiczenia.

## [guided] Piszemy dłuższy program bez błędów (14 min)

### Co robić teraz
- [mów] Teraz napiszecie dłuższy program i zastosujecie wszystko, czego się dziś nauczyliście.
- Zadanie: program budujący kwadratową ramkę o boku pięciu bloków, napisany tekstem.
- Krok 1: zanim zaczniecie pisać, **napiszcie słowami**, w komentarzu albo na kartce, co program ma robić, linijka po linijce.
- Pokaż, jak wygląda komentarz: dwa ukośniki, a po nich tekst, który komputer ignoruje.
- [mów] Komentarz to notatka dla człowieka. Komputer go nie czyta. To jest miejsce, gdzie tłumaczycie sobie, co robi wasz kod.
- Krok 2: napiszcie program, linijka po linijce, zgodnie z planem.
- Krok 3: **zanim uruchomicie**, przeczytajcie go na głos i sprawdźcie, czy zgadza się z planem.
- Krok 4: uruchomcie. Sprawdźcie, czy Agent wrócił na punkt startu.
- Krok 5: jeśli coś nie działa, użyjcie trzech metod z poprzedniego kroku. Nie zgadujcie.
- Krok 6: kto skończy, dopisuje komentarze do swojego programu.

### Wskazówki
- [podpowiedź] Komentarze wprowadzone razem z planowaniem, a nie jako osobny temat, są dużo lepiej przyjmowane. Dziecko widzi, po co one są.
- [błąd] Ramka nie zamyka się, bo brakuje jednego skrętu. Klasyczny błąd logiczny — świetny materiał do zastosowania metody czytania na głos.
- [błąd] Program jest poprawny, ale ramka wychodzi sześć na sześć zamiast pięć na pięć. Zależy od tego, czy Agent stawia blok przed ruchem czy po.
- [błąd] Dziecko pisze komentarze po każdej linijce, także oczywistej. Powiedz, że komentarz jest po to, żeby wyjaśnić **dlaczego**, a nie powtórzyć kod.
- [dla szybszych] Niech napiszą program budujący ramkę prostokątną i sprawdzą, ile linijek musieli zmienić.
- [gdy nie zdążysz] Ramka o boku trzech bloków, bez komentarzy.

### Materiały
- [kod] Ramka z komentarzami | javascript:
  ```
  player.onChat("ramka", function () {
      // przygotowanie: Agent przy graczu, z kamieniem w plecaku
      agent.teleportToPlayer()
      agent.setItem(STONE, 64, 1)
      agent.setSlot(1)
      // pierwszy bok
      agent.move(FORWARD, 1)
      agent.place(DOWN)
      agent.move(FORWARD, 1)
      agent.place(DOWN)
      agent.turn(RIGHT)
      // drugi bok
      agent.move(FORWARD, 1)
      agent.place(DOWN)
      agent.move(FORWARD, 1)
      agent.place(DOWN)
      agent.turn(RIGHT)
  })
  ```

## [challenge] Zepsuj i napraw (8 min)

### Co robić teraz
- Zadanie w parach, na czacie.
- Krok 1: skopiuj swój działający program i zepsuj go w **jednym** miejscu. Zapisz sobie, co zepsułeś.
- Krok 2: wklej zepsutą wersję na czat, adresując do partnera.
- Krok 3: napraw program, który dostałeś, i napisz na czacie, co było zepsute.
- Krok 4: sprawdźcie, czy się zgadza.
- [mów] Zwróćcie uwagę, ile czasu zajęło znalezienie cudzego błędu w porównaniu z własnym. Cudze błędy widać dużo łatwiej — dlatego programiści czytają nawzajem swój kod.

### Wskazówki
- [tempo] To ćwiczenie wymaga sprawnego czatu. Sprawdź przed zajęciami, czy wklejanie kodu nie psuje formatowania.
- [dla szybszych] Niech zepsują program w dwóch miejscach naraz i sprawdzą, czy partner znajdzie oba.
- [gdy nie zdążysz] Zrób jedno wspólne ćwiczenie na forum zamiast pracy w parach.
- [błąd] Dziecko psuje program tak, że nie da się go naprawić bez znajomości oryginału. Ustal zasadę: jeden znak, jedna linijka.

## [challenge] Pokaz i wyjście ze świata (5 min)

### Co robić teraz
- Przypomnij o wyjściu przez menu.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje swój kod i mówi, jaki błąd znalazło u partnera.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Dziś pokaz jest o szukaniu błędów, nie o budowli. Chwal metodę, nie efekt.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: czym różni się błąd składniowy od logicznego? Do czego służą klamry? Jakie znacie trzy metody szukania błędu, którego edytor nie widzi?
- [mów] Dzisiaj nauczyliście się rzeczy, która odróżnia kogoś, kto programuje, od kogoś, kto próbuje: **błąd to informacja**. Czerwone podkreślenie jest po waszej stronie.
- [mów] I zapamiętajcie: każdy programista na świecie spędza więcej czasu na naprawianiu niż na pisaniu. To nie jest oznaka, że coś robicie źle.
- Zajawka: „Na następnych zajęciach poznacie zmienne. Jedną liczbą będziecie sterować całą budowlą.”
- Przypomnij zadanie domowe: zepsuj własny program w trzech miejscach i napraw.

### Wskazówki
- [błąd] Brakująca klamra zamykająca — najczęstszy błąd składniowy.
- [błąd] Brak cudzysłowu — poznasz po kolorach całego programu.
- [błąd] Zła wielkość liter w nazwie polecenia.
- [błąd] Brak przecinka między argumentami w nawiasie.
- [błąd] Program bez podkreśleń, a nie działa — błąd logiczny, użyj trzech metod.
