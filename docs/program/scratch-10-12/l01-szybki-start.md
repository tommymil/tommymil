# Scratch w 45 minut — szybki start
Subject: Scratch
Level: Poziom 2 — 10-12 lat
Czas: 95 min
Tags: Pierwsze zajęcia, Interfejs, Zdarzenia, Sterowanie
Opis: Pierwsze zajęcia kursu dla starszych. Całe podstawy środowiska w pierwszej połowie, sterowana postać z płynnym ruchem w drugiej.
Cel: Dziecko orientuje się w środowisku Scratcha, buduje niezależne skrypty uruchamiane zdarzeniami i rozumie różnicę między sterowaniem zdarzeniowym a pętlą sprawdzającą klawisze.

### Po zajęciach dziecko potrafi
- nazwać części edytora i powiedzieć, co robi każda kategoria bloczków
- zbudować kilka niezależnych skryptów w jednym duszku
- porównać sterowanie na zdarzeniach klawiszy ze sterowaniem w pętli z warunkiem
- powiedzieć, dlaczego program zaczyna się od ustawienia stanu początkowego

### Przygotuj przed zajęciami
- otwarty Scratch z pustym projektem na Twoim ekranie
- gotowe demo: postać z płynnym sterowaniem, biegająca po tle
- lista uczestników — na pierwszych zajęciach wołaj dzieci po imieniu jak najczęściej
- przygotowana wiadomość do rodziców z prośbą o założenie dziecku konta w Scratchu przed lekcją 2
- sprawdzony dźwięk i mikrofony

### Zadanie domowe
- dorzuć swojej postaci jedną rzecz, której dziś nie robiliśmy, i przynieś ją na następne zajęcia
- jeśli nie masz konta w Scratchu, załóż je z rodzicem — od lekcji 2 zapisujemy projekty

## [intro] Powitanie, poznajmy się, demo (10 min)

### Co robić teraz
- [mów] Cześć. Witam na kursie. Dwie rzeczy na start: pierwsza — na tych zajęciach robimy gry, a nie zadania. Druga — w tym roku dojdziecie do własnej platformówki z poziomami.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- Rundka: każde dziecko mówi imię, ulubioną grę i czy miało już kontakt z programowaniem. Ostatnia część jest dla ciebie ważna — zanotuj.
- **DEMO:** pokaż postać z płynnym sterowaniem. Poruszaj się swobodnie, po skosie, zmieniaj prędkość.
- [mów] To zbudujecie dzisiaj, w drugiej połowie zajęć. Pierwsza połowa to szybkie przejście przez podstawy, żeby wszyscy byli w tym samym miejscu.
- Ustal dwa sygnały na cały kurs: kciuk do kamery, gdy coś działa, i wpisanie na czat znaku zapytania, gdy coś nie działa. Nie musicie przerywać, żeby poprosić o pomoc.

### Wskazówki
- [podpowiedź] Informacja o wcześniejszym doświadczeniu jest najważniejszą rzeczą, jaką zbierzesz na pierwszych zajęciach. W grupie 10-12 lat rozrzut bywa od zera do trzech lat Scratcha.
- [tempo] Nie rozciągaj rundki. Trzydzieści sekund na dziecko, twardo.
- [błąd] Ktoś nie ma dźwięku albo nie widzi udostępnianego ekranu — rozwiąż to teraz. Miej pod ręką numer do rodzica.
- [dla szybszych] Dziecko, które zna już Scratcha, dostaje od razu zadanie dodatkowe z kroku po przerwie i pracuje samodzielnie.

## [concept] Środowisko w pigułce (12 min)

### Co robić teraz
- Podyktuj i wklej na czat adres scratch.mit.edu. Wszyscy klikają Stwórz.
- Nazwij cztery obszary ekranu: paleta bloczków, obszar skryptu, scena, panel duszków i Sceny w prawym dolnym rogu.
- Przejdź przez kategorie bloczków i powiedz, co robi każda. Nie omawiaj pojedynczych bloczków — chodzi o mapę, nie o szczegóły.
- Ruch i Wygląd to to, co widać. Dźwięk to słychać. Zdarzenia uruchamiają programy. Kontrola decyduje, co i ile razy się wykona. Czujniki sprawdzają, co się dzieje. Wyrażenia liczą.
- [mów] Zapamiętajcie podział na dwa rodzaje bloczków: **polecenia** mają kształt klocka i coś robią. **Wartości** mają kształt owalu albo sześciokąta i wchodzą do dziurek w innych bloczkach. To rozróżnienie będzie wam potrzebne przez cały rok.
- Pokaż to na przykładzie: przesuń o 10 kroków to polecenie, a losuj od 1 do 10 to wartość, którą można w nie wstawić.
- Sprawdzenie: podaj nazwę bloczka, dzieci wpisują na czat, czy to polecenie, czy wartość. Trzy rundy.

### Wskazówki
- [błąd] Dziecko ma angielski interfejs — ikona globusa na górnym pasku, wybierz Polski. Zrób to z całą grupą, żeby wszyscy mieli te same nazwy.
- [podpowiedź] Podział na polecenia i wartości to najlepsza rzecz, jaką można dać na pierwszych zajęciach dziesięciolatkowi. Oszczędza pół semestru pytań w rodzaju „dlaczego ten bloczek nie chce się podłączyć”.
- [tempo] To ma być mapa, nie wykład. Dwanaście minut na całe środowisko jest wystarczające, jeśli nie wchodzisz w szczegóły.
- [dla szybszych] Niech znajdą w palecie bloczek sześciokątny i zgadną, do czego służy ten kształt.

### Materiały
- [link] Scratch — nowy projekt | https://scratch.mit.edu/projects/editor
- Polecenia mają kształt klocka i coś robią. Wartości mają kształt owalu albo sześciokąta i wchodzą do dziurek.

## [guided] Pierwsze skrypty i stan początkowy (15 min)

### Co robić teraz
- Krok 1: wybierzcie duszka z kilkoma kostiumami — przycisk w prawym dolnym rogu, kategoria Zwierzęta albo Taniec. Sprawdźcie liczbę kostiumów w zakładce Kostiumy. Dwie minuty.
- Krok 2: dodajcie tło. Kliknijcie Scenę, ikona wyboru tła.
- Krok 3: zbudujcie skrypt startowy: kiedy kliknięto zieloną flagę, a pod tym ustaw styl obrotu na lewo-prawo, idź do x: 0 y: 0, ustaw rozmiar na 100 procent, wyczyść efekty graficzne, pokaż.
- [mów] Pięć bloczków, które nic ciekawego nie robią. A są najważniejsze w całym projekcie. To jest **stan początkowy** — gwarancja, że gra zawsze zaczyna się tak samo.
- [mów] Bez tego wasz projekt będzie działał inaczej za pierwszym razem niż za piątym. To najczęstsza przyczyna „u mnie nie działa” przez cały kurs.
- Krok 4: zbudujcie **osobno** skrypt sterowania na zdarzeniach: cztery kapelusze kiedy klawisz naciśnięty, dla czterech strzałek.
- Krok 5: sprawdźcie. Wpiszcie na czat literę G, jak działa, albo znak zapytania, jak nie.
- [mów] Zwróćcie uwagę na to, co macie na ekranie: pięć niezależnych programów w jednym duszku. Każdy czeka na swoje zdarzenie. To nie jest jeden program z pięcioma częściami.

### Wskazówki
- [błąd] Postać obraca się do góry nogami przy ruchu w lewo — brakuje stylu obrotu lewo-prawo. Wraca przez cały kurs, ale jeśli powiesz o tym raz na pierwszych zajęciach, grupa 10-12 zapamiętuje.
- [błąd] Skrypt sterowania podłączony pod skrypt startowy. Wtedy strzałki działają raz, po kliknięciu flagi. Pokaż, że kapelusz zdarzenia nie może mieć niczego nad sobą.
- [błąd] Sterowanie szarpie przy przytrzymaniu klawisza — to jest **temat** drugiej połowy zajęć. Powiedz to wprost, żeby nikt nie szukał błędu.
- [dla szybszych] Niech dorobią do góry i dołu ruch po skosie: dwa klawisze naraz nie zadziałają w tej wersji, niech sprawdzą i zapamiętają, że tak jest.
- [gdy nie zdążysz] Dwie strzałki zamiast czterech. Reszta wraca po przerwie w lepszej wersji.

### Materiały
- [kod] Stan początkowy | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw styl obrotu na [lewo-prawo]
    idź do x: (0) y: (0)
    ustaw rozmiar na (100) %
    wyczyść efekty graficzne
    pokaż
  ```
- [kod] Sterowanie na zdarzeniach | scratch:
  ```
  kiedy klawisz [strzałka w prawo] naciśnięty
    ustaw kierunek na (90)
    przesuń o (10) kroków
  kiedy klawisz [strzałka w lewo] naciśnięty
    ustaw kierunek na (-90)
    przesuń o (10) kroków
  ```

## [challenge] Dopracuj swoją postać (8 min)

### Co robić teraz
- Zadanie samodzielne: dołóż do swojego projektu trzy rzeczy z listy, wybrane dowolnie.
- Animacja chodzenia — bloczek następny kostium przy każdym ruchu.
- Skok po spacji — zmień y w górę, czekanie, zmień y w dół.
- Dźwięk kroku albo skoku.
- Efekt graficzny pod wybranym klawiszem, z możliwością cofnięcia.
- Nie podaję gotowych bloczków. Szukajcie w palecie po kategoriach — to jest część zadania.
- Kto skończy, wpisuje na czat, co zrobił.

### Wskazówki
- [tempo] Samodzielne szukanie bloczków w palecie jest tu celem, nie przeszkodą. Nie podpowiadaj nazw, dopóki dziecko nie utknie na dobre.
- [dla szybszych] Niech zbudują skok, który wygląda naturalnie: w górę szybciej, w dół wolniej, dwiema pętlami.
- [gdy nie zdążysz] Jedna rzecz z listy wystarczy.
- [błąd] Postać po skoku zostaje wyżej — różne wartości w górę i w dół. Muszą być przeciwne.

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały okna ze Scratchem.
- Zapowiedź: „Po przerwie zamienimy szarpane sterowanie na płynne. Zobaczycie różnicę, o której nie da się zapomnieć.”

### Wskazówki
- [tempo] Przerwa wypada przed najlepszą częścią zajęć — po niej wracamy prosto do efektu, który robi wrażenie.

## [concept] Pętla gry — sterowanie płynne (12 min)

### Co robić teraz
- [mów] Wasze sterowanie szarpie. Wciskacie i przytrzymujecie strzałkę, a postać robi krok, przystaje i dopiero potem rusza. Wiecie dlaczego?
- Wyjaśnij: to nie jest wina Scratcha. Tak działa klawiatura w każdym systemie — po pierwszym naciśnięciu jest krótka pauza, zanim klawisz zacznie się powtarzać. Widać ją, gdy przytrzymacie literę w edytorze tekstu.
- [mów] Rozwiązanie jest inne, niż myślicie. Nie będziemy czekać na zdarzenie. Będziemy **pytać**, czy klawisz jest wciśnięty — i to wiele razy na sekundę.
- Pokaż kategorię Czujniki i bloczek klawisz spacja naciśnięty?. Zwróć uwagę na kształt: sześciokąt, czyli wartość, a nie polecenie. Wchodzi do dziurki w bloczku jeżeli.
- Zbudujcie razem: kiedy kliknięto zieloną flagę, pętla zawsze, a w środku jeżeli klawisz strzałka w prawo naciśnięty? to, a w nim ustaw kierunek na 90 i przesuń o 5 kroków.
- Sprawdźcie. Ruch jest natychmiastowy i płynny. Wpiszcie na czat G albo znak zapytania.
- [mów] To się nazywa **pętla gry**. Kręci się bez przerwy i wiele razy na sekundę sprawdza, co robi gracz. Tak działa każda gra, w jaką w życiu graliście.
- Zwróć uwagę na liczbę kroków: 5 zamiast 10. Pętla wykonuje się bardzo szybko, więc pojedynczy krok musi być mniejszy.

### Wskazówki
- [podpowiedź] Nazwanie tego „pętlą gry” jest warte więcej niż sam kod. To pojęcie wraca w każdej grze do końca kursu i dobrze, żeby dzieci miały na nie słowo.
- [błąd] Bloczek jeżeli wylądował poza pętlą zawsze — sprawdzenie wykona się raz i nic nie zadziała.
- [błąd] Dziecko wstawiło bloczek czujnika w niewłaściwe miejsce, bo nie trafiło w sześciokątną dziurkę. Musi podświetlić się na biało.
- [błąd] Stare skrypty na zdarzeniach nadal działają i postać porusza się dwa razy szybciej. To dobra okazja: niech dzieci usuną stare sterowanie i porównają. Usuwanie robi się przeciągnięciem stosu na paletę.
- [dla szybszych] Niech dodadzą warunek, w którym postać porusza się szybciej przy wciśniętym shifcie — dwa sprawdzenia zagnieżdżone jedno w drugim.
- [gdy nie zdążysz] Zrób prawo i lewo w pętli, góra i dół zostaje na zdarzeniach. Porównanie i tak będzie widoczne.

### Materiały
- [kod] Pętla gry — sterowanie płynne | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw styl obrotu na [lewo-prawo]
    zawsze
      jeżeli <klawisz [strzałka w prawo] naciśnięty?> to
        ustaw kierunek na (90)
        przesuń o (5) kroków
      jeżeli <klawisz [strzałka w lewo] naciśnięty?> to
        ustaw kierunek na (-90)
        przesuń o (5) kroków
  ```

## [guided] Cztery kierunki, ruch po skosie i animacja (15 min)

### Co robić teraz
- Krok 1: dołóżcie do pętli dwa kolejne sprawdzenia: strzałka w górę zmienia y o 5, strzałka w dół zmienia y o -5. Bez zmiany kierunku.
- Krok 2: sprawdźcie coś, czego stare sterowanie nie potrafiło — wciśnijcie **dwie strzałki naraz**. Postać idzie po skosie.
- [mów] To jest darmowa nagroda za pętlę gry. Cztery osobne sprawdzenia w jednym obrocie pętli znaczą, że wszystkie mogą być prawdziwe naraz.
- Krok 3: dołóżcie animację. Bloczek następny kostium przy każdym ruchu, w każdym z czterech sprawdzeń.
- Krok 4: sprawdźcie. Animacja jest za szybka. Dobierzcie czekanie: bloczek czekaj 0.05 sek na końcu pętli, poza sprawdzeniami.
- [mów] Uwaga na miejsce tego bloczka. W środku sprawdzenia spowalniałby tylko jeden kierunek. Na końcu pętli spowalnia całą grę — i o to chodzi.
- Krok 5: dołóżcie jeżeli na brzegu, odbij się albo ograniczenie ruchu, żeby postać nie uciekała ze sceny.
- Krok 6: dwie minuty na swobodne bieganie i dobieranie prędkości.

### Wskazówki
- [podpowiedź] Ruch po skosie jest najlepszym argumentem za pętlą gry, jaki można pokazać. Zatrzymaj się na nim, aż wszyscy spróbują.
- [błąd] Animacja miga jak oszalała — brakuje bloczka czekaj albo wartość jest za duża, na przykład 0.5 zamiast 0.05.
- [błąd] Animacja działa tylko przy jednym kierunku — bloczek następny kostium trafił do jednego sprawdzenia zamiast do wszystkich.
- [błąd] Postać zwalnia przy jednoczesnym wciśnięciu dwóch klawiszy, bo czekanie jest w środku sprawdzeń, a nie na końcu pętli.
- [dla szybszych] Niech dodadzą zmienne tempo: bloczek przesuń z wartością zależną od tego, czy wciśnięty jest shift. To zapowiedź zmiennych z lekcji 4.
- [gdy nie zdążysz] Cztery kierunki bez animacji. Animacja wraca na lekcji 2 przy współrzędnych.

### Materiały
- [kod] Pełna pętla gry z animacją | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw styl obrotu na [lewo-prawo]
    idź do x: (0) y: (0)
    zawsze
      jeżeli <klawisz [strzałka w prawo] naciśnięty?> to
        ustaw kierunek na (90)
        przesuń o (5) kroków
        następny kostium
      jeżeli <klawisz [strzałka w lewo] naciśnięty?> to
        ustaw kierunek na (-90)
        przesuń o (5) kroków
        następny kostium
      jeżeli <klawisz [strzałka w górę] naciśnięty?> to
        zmień y o (5)
        następny kostium
      jeżeli <klawisz [strzałka w dół] naciśnięty?> to
        zmień y o (-5)
        następny kostium
      czekaj (0.05) sek
  ```

## [challenge] Twoja wersja sterowania (8 min)

### Co robić teraz
- Zadanie samodzielne: zrób ze swojego sterowania coś własnego. Wybierz dwie rzeczy.
- Dobierz prędkość i tempo animacji tak, żeby ruch wyglądał naturalnie dla twojej postaci.
- Dodaj przyspieszenie pod wybranym klawiszem.
- Spraw, żeby postać zostawiała ślad — rozszerzenie Pisak, przycisk na dole palety.
- Dodaj dźwięk kroku, ale tak, żeby nie zlewał się w jeden hałas.
- Kto skończy, wpisuje na czat, co wybrał.

### Wskazówki
- [tempo] Ostatni punkt jest podchwytliwy i o to chodzi: dźwięk w pętli gry gra dziesiątki razy na sekundę. Dziecko, które to odkryje samo, zapamięta na stałe.
- [dla szybszych] Niech rozwiążą problem z dźwiękiem — na przykład grając go tylko co dziesiąty obrót pętli, z użyciem licznika. To zapowiedź zmiennych.
- [gdy nie zdążysz] Sama dobrana prędkość wystarczy.
- [błąd] Ślad pisaka rysuje się od startu i zasłania scenę — brakuje bloczka wymaż wszystko w stanie początkowym.

## [challenge] Pokaz prac (5 min)

### Co robić teraz
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko udostępnia ekran i pokazuje swoje sterowanie przez dwadzieścia sekund.
- Poproś, żeby przy okazji powiedziało jednym zdaniem, co dołożyło od siebie.
- Brawa po każdym pokazie.
- Zapisanie projektu: kto ma konto, Plik i Zapisz teraz. Kto nie ma, Plik i Zapisz na swoim komputerze.

### Wskazówki
- [podpowiedź] Proszenie o jedno zdanie wyjaśnienia jest tu ważniejsze niż sam pokaz. Uczy mówienia o własnym kodzie, co przyda się na pokazach dla rodziców.
- [błąd] Dziecko bez konta traci projekt. Przypomnij o wiadomości do rodziców — od lekcji 2 konto jest potrzebne.

## [summary] Podsumowanie i plan roku (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: czym różni się sterowanie na zdarzeniach od sterowania w pętli? Po co jest stan początkowy? Co to jest pętla gry?
- [mów] W pierwszej połowie zajęć zrobiliście podstawy, które zwykle zajmują miesiąc. W drugiej zbudowaliście konstrukcję, na której stoi każda gra.
- Pokaż plan roku w trzech zdaniach: pierwszy semestr to własna gra z punktami i poziomami trudności, drugi to platformówka z fizyką, trzeci to wasze projekty.
- Zajawka: „Na następnych zajęciach zajmiemy się współrzędnymi. Wasza postać przestanie chodzić na oślep i zacznie trafiać dokładnie tam, gdzie chcecie.”
- Przypomnij zadanie domowe i prośbę o konto.

### Wskazówki
- [błąd] Postać do góry nogami — styl obrotu lewo-prawo.
- [błąd] Sterowanie szarpie — to sterowanie na zdarzeniach, przenieś je do pętli gry.
- [błąd] Postać porusza się dwa razy szybciej, niż powinna — zostały stare skrypty na zdarzeniach obok nowej pętli.
- [błąd] Gra działa inaczej za piątym uruchomieniem niż za pierwszym — brakuje pełnego stanu początkowego.
- [błąd] Bloczek czujnika nie wchodzi do bloczka jeżeli — trzeba trafić w sześciokątną dziurkę, aż podświetli się na biało.
