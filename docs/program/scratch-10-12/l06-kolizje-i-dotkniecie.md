# Kolizje — gra reaguje na dotknięcie
Subject: Scratch
Level: Poziom 2 — 10-12 lat
Czas: 95 min
Tags: Czujniki, Kolizje, Nietykalność, Pełna gra
Opis: Klikanie znika, zostaje prawdziwa gra. Dzieci wykrywają dotknięcie duszków i kolorów, rozwiązują problem wielokrotnego naliczania i kończą lekcję z grywalną całością.
Cel: Dziecko wykrywa kolizje czujnikami, rozumie, dlaczego kolizja naliczana w pętli zdarza się wielokrotnie, i potrafi to ograniczyć.

### Po zajęciach dziecko potrafi
- użyć czujników dotyka duszka i dotyka koloru
- wyjaśnić, dlaczego jedno zetknięcie odejmuje kilka żyć, i naprawić to
- zbudować chwilową nietykalność po trafieniu
- złożyć zbieranie, tracenie życia i zakończenie w jedną grywalną całość

### Przygotuj przed zajęciami
- gotowe demo: pełna gra z monetami, przeciwnikiem, miganiem po trafieniu i zakończeniem
- przygotowany zły przykład: ta sama gra bez nietykalności, tracąca trzy życia w jednym zetknięciu
- projekty dzieci z lekcji 5
- tło z wyraźnym pasem koloru, do ćwiczenia z czujnikiem koloru

### Zadanie domowe
- zagraj we własną grę pięć razy i zapisz, co cię w niej denerwuje — na następnych zajęciach to poprawimy

## [intro] Powitanie i demo prawdziwej gry (8 min)

### Co robić teraz
- [mów] Cześć. Dziś kończy się klikanie. Wasza postać zacznie zauważać, że w coś wpadła.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** zagraj w pełną wersję gry. Zbieraj monety, unikaj przeciwnika, daj się złapać i pokaż miganie po trafieniu.
- **Drugi pokaz, celowo zły:** ta sama gra bez zabezpieczenia. Jedno zetknięcie z przeciwnikiem odbiera całe trzy życia naraz i gra kończy się natychmiast.
- [mów] Popatrzcie uważnie: dotknąłem raz, a straciłem trzy życia. To jest **najczęstszy błąd w grach robionych w Scratchu** i dziś nauczycie się, dlaczego tak się dzieje.
- Zapytaj grupę: jak myślicie, dlaczego? Zbierz dwie odpowiedzi, nie potwierdzaj żadnej.

### Wskazówki
- [tempo] Zadanie pytania bez odpowiedzi na starcie sprawia, że dzieci szukają jej przez pierwszą połowę zajęć. Odpowiedź pada dopiero w kroku po przerwie.
- [podpowiedź] Ten problem jest zdumiewająco trudny do zauważenia samemu, a zdumiewająco prosty do zrozumienia, gdy ktoś nazwie przyczynę.

## [review] Otwieramy projekt i przypominamy warunki (5 min)

### Co robić teraz
- Wszyscy otwierają projekt z lekcji 5. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie. Sprawdźcie, czy zakończenia działają — ustawcie krótki czas do testu.
- Szybka powtórka: kiedy potrzebne jest jeżeli-w przeciwnym razie? Czym różni się i od lub? Dlaczego sprawdzamy „mniejsze niż 1” zamiast „równa się 0”?
- Kto zrobił zadanie domowe, wysyła na czat jedną ze swoich zasad.

### Wskazówki
- [błąd] U kogoś zakończenie nie działa — najczęściej warunek jest poza pętlą. Napraw razem, to powtórka warta więcej niż pytania.
- [gdy nie zdążysz] Sam start projektu.

## [concept] Czujniki dotknięcia (14 min)

### Co robić teraz
- [mów] W kategorii Czujniki jest bloczek, na który czekacie od pierwszej lekcji.
- Pokaż bloczek dotyka duszka. Sześciokąt, więc wartość prawda albo fałsz, więc wchodzi do bloczka jeżeli.
- Rozwiń listę: są tam wszystkie duszki, wskaźnik myszy i krawędź sceny.
- Krok 1: na duszku gracza zbudujcie: kiedy kliknięto zieloną flagę, pętla zawsze, w niej jeżeli dotyka przeciwnika to zmień życie o -1.
- Krok 2: uruchomcie i dajcie się złapać. **Wszystkie życia znikają natychmiast.**
- [mów] I to jest ten błąd z demo. Teraz wiecie, że to nie moja wina — to się dzieje u każdego.
- Zapytaj ponownie: dlaczego? Teraz zbierz odpowiedzi i potwierdź właściwą.
- [mów] Pętla zawsze kręci się kilkadziesiąt razy na sekundę. Wasze zetknięcie z przeciwnikiem trwa może pół sekundy. Warunek był prawdziwy przez dwadzieścia obrotów pętli i zadziałał dwadzieścia razy.
- [mów] Zapamiętajcie tę zasadę: **pętla nie sprawdza raz, sprawdza cały czas**. Jeśli coś ma się stać jeden raz, musicie tego jeden raz dopilnować.
- Krok 3: zbudujcie drugi czujnik dla monety, na duszku gracza: jeżeli dotyka monety to zmień wynik o 1.
- Krok 4: uruchomcie. Wynik skacze o kilkanaście punktów przy jednym dotknięciu. **Ten sam problem, tylko widać go jeszcze wyraźniej.**

### Wskazówki
- [podpowiedź] Doprowadzenie do błędu na oczach dzieci, zamiast go omijania, jest tu sensem lekcji. Dziecko, które zobaczyło, jak traci trzy życia w pół sekundy, zrozumie rozwiązanie natychmiast.
- [błąd] Czujnik nie wykrywa dotknięcia, bo obrazki się nie stykają, mimo że wyglądają blisko. Duszki mają przezroczyste marginesy. Powiedz o tym — to pytanie pada zawsze.
- [błąd] Skrypt wykrywania zbudowany na przeciwniku zamiast na graczu. Zadziała tak samo, ale trudniej się w tym połapać. Ustalcie regułę: kolizje sprawdzamy na graczu.
- [błąd] Warunek poza pętlą — sprawdza się raz, w chwili startu, i nigdy nic nie wykrywa.
- [dla szybszych] Niech sprawdzą, czy dotyka krawędzi zachowuje się tak samo, i zbudują postać, która nie może wyjść poza scenę.

### Materiały
- [kod] Wykrywanie kolizji — wersja z błędem | scratch:
  ```
  kiedy kliknięto zieloną flagę
    zawsze
      jeżeli <dotyka [Przeciwnik]?> to
        zmień [życie] o (-1)
      jeżeli <dotyka [Moneta]?> to
        zmień [wynik] o (1)
  ```

## [guided] Naprawiamy wielokrotne naliczanie (18 min)

### Co robić teraz
- [mów] Są dwa sposoby, żeby coś zdarzyło się raz. Zbudujemy oba, bo przydają się w różnych sytuacjach.
- **Sposób pierwszy — zabierz przyczynę.** Dobry dla monety.
- Krok 1: przenieście naliczanie punktów na duszka monety. Na monecie zbudujcie: pętla zawsze, w niej jeżeli dotyka gracza to zmień wynik o 1, a **zaraz potem** idź do losowego miejsca.
- Krok 2: uruchomcie. Moneta znika z miejsca zetknięcia, więc warunek przestaje być prawdziwy. Wynik rośnie o dokładnie jeden. Wpiszcie na czat G albo znak zapytania.
- [mów] To jest najprostsze rozwiązanie i zawsze warto sprawdzić, czy da się je zastosować. Jeśli obiekt ma zniknąć — niech znika, a problem rozwiązuje się sam.
- **Sposób drugi — nietykalność.** Konieczny dla przeciwnika, bo przeciwnik ma zostać na scenie.
- Krok 3: na duszku gracza, do warunku dotknięcia przeciwnika, dołóżcie **po** odjęciu życia trzy bloczki: powtórz 10, w niej zmień efekt duch o 25, czekaj 0.05 sek, potem zmień efekt duch o -25 i tak dalej.
- Krok 4: prościej i czytelniej: po odjęciu życia wstawcie ustaw efekt duch na 50, czekaj 1 sek, ustaw efekt duch na 0.
- Krok 5: uruchomcie i dajcie się złapać. Tracicie jedno życie, postać przez sekundę jest przezroczysta i w tym czasie nic więcej się nie dzieje.
- [mów] Dlaczego to działa? Bo bloczek czekaj **zatrzymuje ten skrypt** na sekundę. Pętla nie kręci się dalej, więc warunek nie jest sprawdzany. To jest nietykalność, znana z każdej gry platformowej.
- Krok 6: dobierzcie czas nietykalności. Pół sekundy to za mało, żeby uciec. Dwie sekundy to za dużo i gra staje się za łatwa.

### Wskazówki
- [podpowiedź] Pokazanie obu sposobów, a nie jednego, jest tu istotne. Dziecko ma zrozumieć, że wybór zależy od tego, czy obiekt ma zostać na scenie.
- [błąd] Bloczek czekaj wstawiony poza warunkiem — spowalnia całą pętlę i sterowanie zaczyna szarpać.
- [błąd] Przezroczystość zostaje na stałe, bo dziecko zapomniało wyzerować efekt. Wraca zasada z lekcji 1: co zmieniasz, musisz umieć cofnąć. Sprawdźcie też stan początkowy.
- [błąd] Nietykalność działa, ale gracz nadal traci życie, bo drugi skrypt na przeciwniku też to sprawdza. Wyszukajcie duplikaty — wykrywanie ma być w jednym miejscu.
- [błąd] Moneta po zebraniu pojawia się na graczu i od razu jest zbierana ponownie. Losujcie miejsce z zakresu, dodając warunek na minimalną odległość, albo po prostu dołóżcie krótkie czekanie.
- [dla szybszych] Niech zbudują nietykalność bez bloczka czekaj, przy pomocy zmiennej pilnującej stanu. To jest właściwe rozwiązanie na dłuższą metę i wraca w platformówce.
- [gdy nie zdążysz] Sam pierwszy sposób, z monetą. Nietykalność zrób w kroku po przerwie zamiast koloru.

### Materiały
- [kod] Moneta znika po zebraniu — skrypt na monecie | scratch:
  ```
  kiedy kliknięto zieloną flagę
    pokaż
    zawsze
      jeżeli <dotyka [Gracz]?> to
        zmień [wynik] o (1)
        zagraj dźwięk [Pop]
        idź do x: (losuj od (-200) do (200)) y: (losuj od (-140) do (140))
  ```
- [kod] Nietykalność po trafieniu — skrypt na graczu | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw efekt [duch] na (0)
    zawsze
      jeżeli <dotyka [Przeciwnik]?> to
        zmień [życie] o (-1)
        zagraj dźwięk [Oops]
        ustaw efekt [duch] na (50)
        czekaj (1) sek
        ustaw efekt [duch] na (0)
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Zapowiedź: „Po przerwie wasza postać zacznie reagować na kolory. Zbudujecie strefy na planszy — bezpieczne, zabójcze i takie, które dają punkty.”

### Wskazówki
- [tempo] Pierwsza połowa jest gęsta pojęciowo. Nie skracaj przerwy, nawet jeśli grupie idzie dobrze.

## [concept] Czujnik koloru i strefy na planszy (13 min)

### Co robić teraz
- [mów] Kolizje z duszkami znacie. Ale plansza to nie tylko duszki — to też tło. A tło da się sprawdzać po kolorze.
- Pokaż bloczek dotyka koloru. Kliknięcie w kwadrat koloru otwiera próbnik z pipetą.
- [mów] Pipeta jest ważna. Nie wybierajcie koloru na oko z palety — pobierzcie go **prosto ze sceny**, klikając pipetą w to miejsce tła, o które wam chodzi. Kolor musi się zgadzać dokładnie.
- Krok 1: wybierzcie tło z wyraźnymi obszarami kolorów albo narysujcie pas w edytorze tła. Prostokąt, jeden kolor, szerokość na całą scenę.
- Krok 2: na graczu zbudujcie: jeżeli dotyka koloru czerwonego to zmień życie o -1, a potem idź do x: 0 y: 0.
- [mów] Zauważcie, że tu też rozwiązaliśmy problem wielokrotnego naliczania — przeniesieniem postaci poza strefę. Trzeci sposób, ten sam pomysł.
- Krok 3: dołóżcie drugą strefę: jeżeli dotyka koloru zielonego to zmień wynik o 1. Ale tu przeniesienie nie ma sensu, więc dołóżcie czekanie.
- Krok 4: uruchomcie i pobiegajcie po strefach. Wpiszcie na czat G albo znak zapytania.
- [mów] Macie teraz planszę, która ma zasady wynikające z tego, jak wygląda. To jest podstawa każdej gry platformowej — a platformówkę robimy w drugim semestrze.

### Wskazówki
- [błąd] Czujnik koloru nie reaguje. Przyczyna prawie zawsze ta sama: kolor pobrany z palety, a nie pipetą ze sceny. Tło może mieć cieniowanie i sąsiednie piksele różnią się odcieniem.
- [błąd] Czujnik reaguje w przypadkowych miejscach, bo ten sam kolor jest gdzieś jeszcze — na przykład w kostiumie duszka. Dobierzcie kolor nietypowy.
- [błąd] Postać wpada w strefę i przenosi się w kółko, bo punkt startowy też jest w strefie.
- [błąd] Warunki dla dwóch kolorów zbudowane jeden w drugim zamiast obok siebie. Wtedy drugi sprawdza się tylko wtedy, gdy pierwszy jest prawdziwy. Dobre ćwiczenie z czytania kodu.
- [dla szybszych] Niech zrobią strefę spowalniającą — w niej prędkość gracza jest mniejsza. Wymaga zmiennej prędkość z lekcji 4.
- [gdy nie zdążysz] Jedna strefa zamiast dwóch.

### Materiały
- [kod] Strefy na planszy — skrypt na graczu | scratch:
  ```
  kiedy kliknięto zieloną flagę
    zawsze
      jeżeli <dotyka koloru [czerwony]?> to
        zmień [życie] o (-1)
        idź do x: (0) y: (0)
      jeżeli <dotyka koloru [zielony]?> to
        zmień [wynik] o (1)
        czekaj (0.5) sek
  ```

## [guided] Składamy pełną grę (14 min)

### Co robić teraz
- [mów] Macie wszystkie części. Teraz je poskładamy i sprawdzimy, czy gra trzyma się kupy.
- Krok 1: przejrzyjcie swoje skrypty i sprawdźcie listę. Czytaj punkt po punkcie, dzieci potwierdzają kciukiem.
- Stan początkowy zeruje wynik, życie i czas.
- Stan początkowy ustawia pozycje wszystkich duszków i czyści efekty.
- Zbieranie monety działa i nalicza dokładnie jeden punkt.
- Kolizja z przeciwnikiem odbiera dokładnie jedno życie.
- Gra kończy się przy zerze życia i przy zerze czasu.
- Krok 2: zagrajcie trzy razy pod rząd bez odświeżania strony. Sprawdźcie, czy za trzecim razem wszystko jest tak samo jak za pierwszym.
- [mów] Trzy uruchomienia pod rząd to najprostszy test, jaki możecie zrobić. Wyłapuje prawie wszystkie problemy ze stanem początkowym.
- Krok 3: dostrójcie trudność. Prędkość przeciwnika, częstotliwość monety, czas nietykalności, długość rundy.
- Krok 4: zapiszcie na czacie jedną liczbę, którą zmieniliście, i co to dało.

### Wskazówki
- [podpowiedź] Lista kontrolna odczytywana na głos jest tu lepsza niż chodzenie po grupie. Dziecko samo znajduje brak, zamiast czekać, aż ktoś mu powie.
- [podpowiedź] Test „trzy uruchomienia pod rząd” warto wprowadzić jako stały rytuał do końca kursu.
- [błąd] Za trzecim uruchomieniem przeciwnik startuje tam, gdzie skończył — brak pozycji w stanie początkowym.
- [błąd] Gra kończy się natychmiast po starcie, bo warunek zakończenia sprawdza zmienną, zanim skrypt zerujący zdąży ją ustawić. Rozwiązanie: krótkie czekanie na początku skryptu sprawdzającego albo zerowanie w jednym, wspólnym miejscu.
- [dla szybszych] Niech dodadzą drugiego przeciwnika o innym zachowaniu — jeden goni, drugi patroluje trasę z lekcji 2.
- [gdy nie zdążysz] Sama lista kontrolna, bez strojenia trudności.

## [challenge] Twoja gra, twoje zasady (8 min)

### Co robić teraz
- Zadanie samodzielne: dodaj do gry jedną rzecz, której dziś nie robiliśmy. Propozycje, ale własny pomysł jest lepszy.
- Bonus, który dodaje życie, ale pojawia się rzadko.
- Przeciwnik, który po dotknięciu zabiera punkty zamiast życia.
- Strefa na planszy, w której przeciwnik nie może dosięgnąć gracza.
- Efekt dźwiękowy i wizualny przy każdym zdarzeniu w grze.
- Kto skończy, wpisuje na czat, co dodał.

### Wskazówki
- [tempo] Ten moment jest pierwszym, w którym gry dzieci zaczynają się realnie różnić. Zachęcaj do własnych pomysłów mocniej niż zwykle.
- [dla szybszych] Niech dodadzą mechanikę, która wymaga dwóch warunków połączonych operatorem — na przykład bonus działający tylko, gdy życie jest mniejsze niż 2.
- [gdy nie zdążysz] Sam dźwięk przy zdarzeniach wystarczy.
- [błąd] Nowy obiekt wprowadza z powrotem problem wielokrotnego naliczania. Zapytaj, który z trzech sposobów tu pasuje — to najlepsze utrwalenie dzisiejszej lekcji.

## [challenge] Pokaz prac i zapis (5 min)

### Co robić teraz
- Zapis: Plik, Zapisz teraz. Poczekaj na potwierdzenie od każdego dziecka.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko udostępnia ekran i gra w swoją grę przez dwadzieścia sekund.
- Brawa po każdym pokazie. Jedno zdanie od ciebie o tym, co w tej grze działa dobrze.

### Wskazówki
- [podpowiedź] Po tej lekcji gry są już naprawdę grywalne. To dobry moment, żeby poprosić dzieci o wysłanie linku rodzicom.
- [tempo] Zapis przed pokazem, zawsze.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: dlaczego jedno dotknięcie odbierało kilka żyć? Jakie znacie trzy sposoby, żeby coś zdarzyło się raz? Dlaczego czujnik koloru trzeba ustawiać pipetą?
- [mów] Dzisiaj zbudowaliście prawdziwą, grywalną grę. Ale ważniejsze jest coś innego: zrozumieliście, że pętla sprawdza cały czas, a nie raz. To pojęcie wróci w każdym projekcie do końca kursu.
- Zajawka: „Na następnych zajęciach zajmiemy się losowością na poważnie i tym, jak sprawić, żeby gra robiła się coraz trudniejsza, im dłużej gracie.”
- Przypomnij zadanie domowe: zagraj pięć razy i zapisz, co cię denerwuje.

### Wskazówki
- [błąd] Jedno dotknięcie odbiera wiele żyć — brak zabezpieczenia przed powtórnym naliczeniem.
- [błąd] Duszki wyglądają na stykające się, ale czujnik milczy — przezroczyste marginesy w obrazkach.
- [błąd] Czujnik koloru nie działa — kolor wybrany z palety zamiast pobrany pipetą ze sceny.
- [błąd] Przezroczystość po trafieniu zostaje na stałe — brak wyzerowania efektu.
- [błąd] Gra kończy się natychmiast po starcie — warunek sprawdzany przed zerowaniem zmiennych.
