# Współrzędne i precyzyjny ruch
Subject: Scratch
Level: Poziom 2 — 10-12 lat
Czas: 95 min
Tags: Współrzędne, Ruch, Płynne przejścia, Celowanie
Opis: Postać przestaje chodzić na oślep. Dzieci poznają układ współrzędnych sceny i budują ruch do konkretnego punktu, patrolowanie trasy i celowanie w mysz.
Cel: Dziecko posługuje się współrzędnymi sceny, odróżnia ruch skokowy od płynnego i potrafi kazać postaci dojść do wskazanego punktu.

### Po zajęciach dziecko potrafi
- odczytać i wpisać współrzędne punktu na scenie
- odróżnić bloczki idź do, leć do i przesuń o oraz powiedzieć, kiedy którego użyć
- zbudować patrol między dwoma punktami
- skierować postać w stronę wskaźnika myszy

### Przygotuj przed zajęciami
- gotowe demo: postać patrolująca trasę i druga, która zawsze patrzy na mysz
- przygotowany rysunek sceny z zaznaczonymi zakresami współrzędnych — udostępnisz go na ekranie
- projekty dzieci z lekcji 1
- lista dzieci, które na lekcji 1 sygnalizowały wcześniejsze doświadczenie ze Scratchem

### Zadanie domowe
- wyznacz w swoim projekcie trasę z czterech punktów i spraw, żeby postać ją obeszła

## [intro] Powitanie i demo precyzji (8 min)

### Co robić teraz
- [mów] Cześć. Wasza postać umie już chodzić płynnie. Ale nadal chodzi tam, gdzie ją popchniecie — nie tam, gdzie chcecie.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** pokaż dwie rzeczy. Pierwsza — postać patrolująca trasę między czterema punktami, w kółko, bez waszego udziału. Druga — postać, która zawsze patrzy na kursor myszy, gdziekolwiek go przesuniecie.
- [mów] Żadnej z tych dwóch rzeczy nie da się zrobić strzałkami. Potrzebny jest sposób, żeby powiedzieć postaci: „idź dokładnie tutaj”.
- [mów] Dziś poznacie układ współrzędnych. Brzmi jak matematyka i jest matematyką, ale za dwadzieścia minut będziecie go używać bez zastanowienia.

### Wskazówki
- [tempo] Patrolujący przeciwnik to bezpośrednia zapowiedź lekcji 21. Powiedz to — starsze dzieci lubią wiedzieć, do czego prowadzi materiał.
- [podpowiedź] Nie zaczynaj od słowa „matematyka”, ale nie ukrywaj go, gdy padnie. Dziesięciolatek świetnie to znosi, jeśli widzi zastosowanie.

## [review] Otwieramy projekt i przypominamy pętlę gry (5 min)

### Co robić teraz
- Wszyscy otwierają projekt z lekcji 1. Poczekaj na potwierdzenie od każdego dziecka.
- Kliknijcie flagę, sprawdźcie sterowanie i ruch po skosie.
- Szybka powtórka: co to jest pętla gry? Dlaczego sterowanie w pętli jest płynniejsze niż na zdarzeniach? Po co jest stan początkowy?
- Kto zrobił zadanie domowe, pokazuje na czacie jednym zdaniem, co dołożył.

### Wskazówki
- [błąd] Dziecko bez projektu z lekcji 1, bo nie miało konta. Niech odbuduje pętlę gry z gotowego kodu — to pięć minut i najlepsza możliwa powtórka. Sprawdź, czy konto już jest.
- [gdy nie zdążysz] Sam start projektu, pytania pomiń.

## [concept] Układ współrzędnych sceny (14 min)

### Co robić teraz
- [mów] Scena Scratcha to prostokąt opisany liczbami. Każdy punkt ma dokładnie jeden adres i ten adres to dwie liczby.
- Udostępnij rysunek sceny i pokaż zakresy: poziomo, czyli x, od -240 po lewej do 240 po prawej. Pionowo, czyli y, od -180 na dole do 180 na górze. Środek to x równe 0 i y równe 0.
- [mów] Zapamiętajcie znaki. W lewo to minus x. W dół to minus y. To jedyna rzecz, którą trzeba tu zapamiętać.
- Pokaż, że nie musicie zgadywać: przeciągnijcie duszka myszką po scenie i patrzcie na pola x i y w panelu nad sceną. Liczby zmieniają się na żywo.
- Ćwiczenie na rozgrzewkę, całą grupą. Podajesz miejsce, dzieci przeciągają tam swojego duszka i wpisują na czat odczytane liczby.
- Lewy górny róg. Prawy dolny róg. Dokładny środek. Środek dolnej krawędzi.
- [mów] I najważniejsze: bloczek idź do x: y: sam wypełnia się liczbami tam, gdzie stoi wasz duszek. Nie musicie ich wpisywać z głowy — ustawcie postać myszką i przeciągnijcie bloczek do skryptu.
- Sprawdzenie: każde dziecko ustawia postać w wybranym miejscu, bierze bloczek idź do i sprawdza, czy liczby się zgadzają.

### Wskazówki
- [podpowiedź] Podpowiedź o samowypełniającym się bloczku oszczędza połowę lekcji. Powiedz ją, ale dopiero po ćwiczeniu z odczytywaniem — inaczej dzieci nie nauczą się czytać współrzędnych wcale.
- [błąd] Dziecko myli x z y. Najlepsza pamięciówka: x jest jak poziom podłogi, idzie w bok. Litera y ma ogonek w dół.
- [błąd] Postać wychodzi poza scenę przy dużych liczbach i przykleja się do krawędzi. Scratch nie pozwala duszkowi wyjść całkiem poza widok — to nie jest błąd.
- [dla szybszych] Niech sprawdzą, co się stanie przy x równym 1000. Niech wyjaśnią, dlaczego postać zatrzymuje się przy krawędzi, a nie ucieka.

### Materiały
- [link] Scratch — nowy projekt | https://scratch.mit.edu/projects/editor
- Scena: x od -240 do 240, y od -180 do 180. Środek to 0, 0. W lewo i w dół to wartości ujemne.

## [guided] Trzy sposoby na ruch (18 min)

### Co robić teraz
- [mów] Do przemieszczania postaci są trzy bloczki i różnią się bardziej, niż wygląda. Zbudujemy je obok siebie i porównamy.
- Krok 1: zbudujcie skrypt pod klawiszem 1: idź do x: 150 y: 100. Wciśnijcie. Postać **teleportuje się** natychmiast.
- Krok 2: skrypt pod klawiszem 2: leć przez 1 sek do x: 150 y: 100. Wciśnijcie. Postać **płynnie przelatuje** w to samo miejsce.
- Krok 3: skrypt pod klawiszem 3: przesuń o 50 kroków. Wciśnijcie kilka razy. Postać przesuwa się **względem miejsca, w którym stoi**, w stronę, w którą patrzy.
- [mów] I to jest cała różnica. Pierwsze dwa mówią **dokąd**. Trzeci mówi **o ile**. Pierwsze dwa to adres, trzeci to krok.
- Krok 4: sprawdzenie zrozumienia. Pytanie na czat: którym bloczkiem ustawić postać na starcie gry? A którym zrobić, żeby przeciwnik podpłynął do gracza?
- Krok 5: zbudujcie patrol. Skrypt: kiedy kliknięto zieloną flagę, pętla zawsze, w niej leć przez 2 sek do pierwszego punktu, potem leć przez 2 sek do drugiego punktu.
- Krok 6: sprawdźcie. Postać kursuje w kółko między dwoma miejscami.
- Krok 7: dołóżcie trzeci i czwarty punkt, żeby trasa była ciekawsza.

### Wskazówki
- [podpowiedź] Rozróżnienie „adres kontra krok” jest sednem tej lekcji. Wróci przy przeciwnikach, przy pociskach i przy kamerze podążającej za postacią.
- [błąd] Patrol wygląda na zepsuty, bo postać skacze zamiast lecieć — dziecko użyło idź do zamiast leć do. To dobry moment, żeby porównać oba na żywo.
- [błąd] Postać patroluje, ale jednocześnie działa sterowanie z lekcji 1 i wszystko się kłóci. Powiedz wprost, że dwa programy przesuwające tę samą postać zawsze będą walczyć. Na dziś: wyłączcie sterowanie albo zróbcie patrol na drugim duszku.
- [błąd] Postać obraca się dziwnie w trakcie lotu — bloczek leć do nie zmienia kierunku, więc jeśli coś obraca postać, to inny skrypt.
- [dla szybszych] Niech zrobią patrol o zmiennej prędkości: dłuższy czas lotu na dłuższym odcinku. To wymaga oszacowania odległości i jest dobrym zadaniem.
- [gdy nie zdążysz] Patrol między dwoma punktami wystarczy, trzeci i czwarty pomiń.

### Materiały
- [kod] Trzy sposoby na ruch | scratch:
  ```
  kiedy klawisz [1] naciśnięty
    idź do x: (150) y: (100)
  kiedy klawisz [2] naciśnięty
    leć przez (1) sek do x: (150) y: (100)
  kiedy klawisz [3] naciśnięty
    przesuń o (50) kroków
  ```
- [kod] Patrol między czterema punktami | scratch:
  ```
  kiedy kliknięto zieloną flagę
    idź do x: (-180) y: (120)
    zawsze
      leć przez (2) sek do x: (180) y: (120)
      leć przez (2) sek do x: (180) y: (-120)
      leć przez (2) sek do x: (-180) y: (-120)
      leć przez (2) sek do x: (-180) y: (120)
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały okna ze Scratchem.
- Zapowiedź: „Po przerwie wasza postać zacznie was śledzić. Będzie patrzeć na mysz i iść za nią.”

### Wskazówki
- [tempo] Śledzenie myszy jest efektem, który starsze dzieci uznają za „prawdziwe programowanie”. Warto na nie zostawić drugą połowę w komplecie.

## [concept] Kierunek i celowanie w mysz (13 min)

### Co robić teraz
- [mów] Do tej pory sami wpisywaliście, dokąd ma iść postać. Teraz cel będzie się ruszał, a postać ma sama się do niego dostosować.
- Pokaż bloczek skieruj się w stronę i rozwiń listę. Są tam wskaźnik myszy oraz wszystkie duszki w projekcie.
- Zbudujcie: kiedy kliknięto zieloną flagę, pętla zawsze, a w niej skieruj się w stronę wskaźnika myszy.
- Ruszajcie myszą po scenie. Postać obraca się za kursorem. Wpiszcie na czat G albo znak zapytania.
- [mów] Teraz dołóżcie ruch: pod skierowaniem wstawcie przesuń o 3 kroki. Postać zaczyna gonić mysz.
- [mów] Popatrzcie, co się właśnie stało. Trzy bloczki i macie przeciwnika, który ściga gracza. Tak działa większość prostych wrogów w grach.
- Wypróbujcie różne prędkości: 1, 3, 10. Przy 10 nie da się uciec, przy 1 jest za łatwo.
- Pokaż wersję z odległością: bloczek odległość od wskaźnika myszy w Czujnikach. Wstawcie go do bloczka powiedz i patrzcie, jak liczba się zmienia.

### Wskazówki
- [podpowiedź] To pierwszy raz, gdy dzieci widzą wartość zmieniającą się w czasie rzeczywistym. Wyświetlenie odległości w dymku jest świetnym wstępem do zmiennych z lekcji 4.
- [błąd] Postać obraca się do góry nogami przy pościgu w lewo — styl obrotu lewo-prawo. Uwaga: przy śledzeniu myszy styl lewo-prawo sprawia, że postać nie pochyla się w pionie. To zwykle jest pożądane, ale powiedz o tym, żeby nikt nie szukał błędu.
- [błąd] Postać drga w miejscu, gdy kursor jest bardzo blisko — postać przeskakuje cel w każdą stronę. Naturalne rozwiązanie to warunek „jeżeli odległość większa niż 10”, ale warunki są dopiero na lekcji 5. Na dziś wystarczy nazwać zjawisko.
- [dla szybszych] Niech spróbują sami zbudować to zabezpieczenie z bloczkiem jeżeli, szukając go w Kontroli. Kto zrobi, ma gotowy materiał na lekcję 5.
- [gdy nie zdążysz] Samo skierowanie w stronę myszy, bez ruchu.

### Materiały
- [kod] Pościg za myszą | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw styl obrotu na [lewo-prawo]
    zawsze
      skieruj się w stronę [wskaźnik myszy]
      przesuń o (3) kroków
  ```
- [kod] Podgląd odległości | scratch:
  ```
  kiedy kliknięto zieloną flagę
    zawsze
      powiedz (odległość od [wskaźnik myszy])
  ```

## [guided] Uciekaj — pierwsza mini-gra (14 min)

### Co robić teraz
- [mów] Macie wszystko, żeby zrobić grę. Wy sterujecie jedną postacią, druga was goni.
- Krok 1: dodajcie drugiego duszka. Będzie graczem. Przenieście na niego pętlę gry ze sterowaniem z lekcji 1 albo zbudujcie ją od nowa.
- [mów] Przenoszenie skryptu między duszkami: przeciągnijcie stos bloczków na miniaturkę drugiego duszka w prawym dolnym rogu. Kod się skopiuje.
- Krok 2: na pierwszym duszku, tym goniącym, zmieńcie cel z wskaźnika myszy na nazwę drugiego duszka.
- Krok 3: uruchomcie. Uciekacie strzałkami, przeciwnik goni. **To jest gra.**
- Krok 4: dobierzcie prędkości. Wasza postać musi być szybsza od przeciwnika, ale nie za bardzo. Wypróbujcie: gracz 5, przeciwnik 3.
- Krok 5: dołóżcie przeciwnikowi punkt startowy w stanie początkowym, daleko od gracza.
- Krok 6: zagrajcie we własną grę dwie minuty i dostrójcie liczby.

### Wskazówki
- [podpowiedź] Kopiowanie skryptu przez przeciągnięcie na miniaturkę duszka to jedna z najbardziej przydatnych sztuczek Scratcha, a większość dzieci sama jej nie znajdzie.
- [błąd] Przeciwnik dogania natychmiast i gra jest nie do wygrania — różnica prędkości za duża albo start za blisko.
- [błąd] Po skopiowaniu skryptu obie postacie reagują na strzałki. Trzeba usunąć sterowanie z duszka goniącego.
- [błąd] Gra nie ma końca, bo nic się nie dzieje po złapaniu. Powiedz, że to jest temat lekcji 6, i zostaw grę bez zakończenia — dziś liczy się ruch.
- [dla szybszych] Niech dodadzą drugiego przeciwnika o innej prędkości albo zrobią, żeby przeciwnik przyspieszał z czasem.
- [gdy nie zdążysz] Zostawcie pościg za myszą z poprzedniego kroku. Gra z drugim duszkiem wraca na lekcji 6.

## [challenge] Twoja trasa (8 min)

### Co robić teraz
- Zadanie samodzielne: wybierz jedno z dwóch.
- Trasa: zaprojektuj patrol z co najmniej czterech punktów, tak żeby postać obeszła kształt — kwadrat, trójkąt, zygzak. Wypisz sobie współrzędne, zanim zaczniesz budować.
- Celownik: zrób postać, która stoi w miejscu i tylko obraca się za myszą, a po wciśnięciu spacji strzela — czyli druga postać leci z jej pozycji w kierunku kursora.
- Kto skończy, wpisuje na czat, które zadanie wybrał.

### Wskazówki
- [tempo] Wypisanie współrzędnych przed budowaniem jest częścią zadania, nie przygotowaniem do niego. To pierwszy krok w stronę planowania kodu przed pisaniem.
- [dla szybszych] Zadanie z celownikiem jest trudniejsze i wymaga bloczka idź do duszka. Skieruj tam dzieci z doświadczeniem.
- [gdy nie zdążysz] Trasa z trzech punktów wystarczy.
- [błąd] Kształt wychodzi krzywy, bo dziecko pomyliło znak przy jednej współrzędnej. Świetna okazja, żeby pokazać szukanie błędu przez sprawdzanie punkt po punkcie.

## [challenge] Pokaz prac i zapis (5 min)

### Co robić teraz
- Zapis: Plik, Zapisz teraz. Poczekaj na potwierdzenie od każdego dziecka.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje swoją trasę albo celownik i mówi jednym zdaniem, jak dobrało współrzędne.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Pytanie „jak dobrałeś te liczby” jest ważniejsze niż sam efekt. Odpowiedź „przeciągnąłem duszka i odczytałem” jest w pełni poprawna i warto ją pochwalić.
- [tempo] Zapis przed pokazem, zawsze.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: czym różni się idź do od przesuń o? Jakie są zakresy x i y na scenie? Jak najprościej poznać współrzędne punktu?
- [mów] Dziś przestaliście pchać postać i zaczęliście ją adresować. To jest różnica między „idź trochę w prawo” a „bądź tutaj”. Od tego zaczyna się każda gra, w której cokolwiek musi trafić w cokolwiek.
- Zajawka: „Na następnych zajęciach zajmiemy się pętlami na poważnie — pętla w pętli i rysowanie wzorów. Zobaczycie, jak z dziesięciu bloczków zrobić rzeczy, których nie da się narysować ręcznie.”
- Przypomnij zadanie domowe: trasa z czterech punktów.

### Wskazówki
- [błąd] Postać skacze zamiast płynnie lecieć — idź do zamiast leć do.
- [błąd] Dwa skrypty przesuwają tę samą postać i walczą ze sobą.
- [błąd] Pomylone x z y albo znak przy współrzędnej — sprawdzaj punkt po punkcie.
- [błąd] Postać drga przy bardzo bliskim celu — przeskakuje go w obie strony, potrzebny warunek na minimalną odległość.
- [błąd] Po skopiowaniu skryptu na drugiego duszka obie postacie robią to samo — usuń zbędny kod.
