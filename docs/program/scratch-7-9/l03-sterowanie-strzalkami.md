# Sterowanie strzałkami — przejmujesz kontrolę
Subject: Scratch
Level: Poziom 1 — 7-9 lat
Czas: 95 min
Tags: Zdarzenia, Klawiatura, Sterowanie, Interakcja
Opis: Postać przestaje chodzić sama i zaczyna słuchać gracza. Dzieci budują sterowanie czterema strzałkami i po raz pierwszy grają we własną grę.
Cel: Dziecko buduje sterowanie postacią z klawiatury i rozumie, że jeden projekt może mieć kilka niezależnych skryptów uruchamianych różnymi zdarzeniami.

### Po zajęciach dziecko potrafi
- zbudować cztery osobne skrypty reagujące na cztery strzałki
- wyjaśnić, że zdarzenie to sygnał, na który program czeka
- ustawić styl obrotu tak, żeby postać nie kładła się na plecach
- odróżnić skrypt duszka od skryptu sceny

### Przygotuj przed zajęciami
- otwarty Scratch z gotowym demo: postać sterowana czterema strzałkami po tle
- projekty dzieci z lekcji 2 — przypomnij na czacie przed zajęciami, żeby wiedziały, gdzie je mają
- przygotowane pytanie na start: w jaką grę graliście ostatnio i czym się w niej sterowało

### Zadanie domowe
- naucz kogoś w domu sterowania swoją postacią i pozwól mu pograć przez minutę

## [intro] Powitanie i demo sterowania (10 min)

### Co robić teraz
- [mów] Cześć! Dziś przestajecie oglądać swoją postać i zaczynacie nią rządzić.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** udostępnij ekran i steruj postacią strzałkami. Poruszaj się na wszystkie cztery strony, wyraźnie i powoli.
- [mów] Patrzcie na moje palce. Wciskam strzałkę, postać idzie. Puszczam, staje. To jest dokładnie to, co robi każda gra, w jaką graliście.
- Zapytaj grupę po imieniu: w jaką grę graliście ostatnio i czym się w niej sterowało? Zbierz trzy odpowiedzi.
- [mów] Za czterdzieści minut będziecie mieli to samo. A za dwa tygodnie zamienimy to w prawdziwą grę z punktami.

### Wskazówki
- [tempo] Sterowanie to najbardziej wyczekiwana lekcja pierwszego semestru. Nie skracaj demo — moment, w którym dziecko widzi, że da się to zrobić samemu, jest paliwem na całe zajęcia.
- [podpowiedź] Jeśli masz kamerę skierowaną na klawiaturę, pokaż ją. Siedmiolatkowi łatwiej zrozumieć związek klawisz-efekt, gdy widzi rękę.

## [review] Otwieramy projekt z poprzednich zajęć (5 min)

### Co robić teraz
- Wszyscy otwierają swój projekt z lekcji 2. Pokaż na swoim ekranie obie drogi: przez Moje rzeczy na koncie albo przez Plik, Wczytaj z komputera.
- Poczekaj, aż każde dziecko potwierdzi, że widzi swoją postać. Zapytaj po imieniu tych, które milczą.
- Szybka powtórka: co robi bloczek zawsze? Po co jest czekaj? Który bloczek ratuje nas przed postacią do góry nogami?
- Kto nie ma projektu, dostaje minutę na wybranie nowego duszka. Nie odtwarzamy dziś animacji z lekcji 2 — dzisiejsze sterowanie jest osobnym skryptem.

### Wskazówki
- [błąd] Dziecko nie znajduje projektu. Najczęstsza przyczyna: zapisało się na innym koncie albo w ogóle bez konta. Nie szukaj długo — nowy duszek i idziemy dalej.
- [tempo] Odzyskiwanie projektów potrafi zjeść piętnaście minut. Daj sobie na to pięć i twardo przejdź dalej.
- [podpowiedź] Zapisz sobie, które dzieci mają problem z odnajdywaniem projektu. To informacja dla rodzica, nie dla dziecka.

## [concept] Zdarzenie — program czeka na sygnał (12 min)

### Co robić teraz
- [mów] Znacie już jeden bloczek zdarzeń: kiedy kliknięto zieloną flagę. Dziś poznacie jego rodzeństwo.
- Pokaż żółtą kategorię Zdarzenia i bloczek kiedy klawisz spacja naciśnięty. Kliknij w słowo spacja i pokaż rozwijaną listę wszystkich klawiszy.
- [mów] To jest lista wszystkiego, na co program może czekać. Wybierzcie z niej strzałkę w prawo.
- Zbuduj razem z dziećmi pierwszy skrypt: kiedy klawisz strzałka w prawo naciśnięty, a pod nim przesuń o 10 kroków.
- Wciśnij strzałkę. Postać idzie w prawo. **Czekaj na kciuki.**
- [mów] Uwaga, teraz najważniejsze. Ten skrypt leży **obok** waszego starego programu, a nie pod nim. Jeden duszek może mieć wiele osobnych programów i każdy czeka na co innego.
- Pokaż to na swoim ekranie: dwa osobne stosy bloczków obok siebie, każdy z własnym żółtym kapeluszem.

### Wskazówki
- [podpowiedź] To pojęciowo najtrudniejszy moment pierwszego semestru: kilka programów naraz, w jednym projekcie. Powiedz to trzy razy, różnymi słowami.
- [błąd] Dziecko podłącza skrypt sterowania pod stary program z zieloną flagą. Wtedy strzałki działają dopiero po kliknięciu flagi i tylko raz. Pokaż na ekranie, jak odczepić i odsunąć na bok.
- [błąd] Strzałki nie działają wcale — najczęściej dlatego, że dziecko kliknęło poza sceną i klawiatura pisze gdzie indziej. Niech kliknie raz na scenę i spróbuje ponownie.
- [tempo] Jeśli grupa łapie to szybko, przejdź od razu do czterech strzałek w następnym kroku i zyskaj czas na wyzwanie.

### Materiały
- [kod] Pierwsze zdarzenie klawiatury | scratch:
  ```
  kiedy klawisz [strzałka w prawo] naciśnięty
    przesuń o (10) kroków
  ```

## [guided] Cztery strzałki krok po kroku (18 min)

### Co robić teraz
- Budujemy cztery skrypty. Po każdym czekasz na kciuki od wszystkich.
- Krok 1: skrypt startowy. Zielona flaga, pod nią ustaw styl obrotu na lewo-prawo i idź do x: 0 y: 0. To jest sprzątanie przed grą. **Czekaj na kciuki.**
- Krok 2: strzałka w prawo. Zdarzenie, pod nim ustaw kierunek na 90, przesuń o 10 kroków, następny kostium.
- Krok 3: sprawdźcie. Postać idzie w prawo, przebierając nogami. **Czekaj na kciuki.**
- Krok 4: strzałka w lewo. To samo, ale kierunek -90.
- Krok 5: sprawdźcie. Postać idzie w lewo i jest odwrócona twarzą w lewo, ale nie leży na plecach — bo ustawiliśmy styl obrotu.
- Krok 6: strzałka w górę. Zdarzenie, pod nim zmień y o 10 i następny kostium. **Bez** ustawiania kierunku.
- Krok 7: strzałka w dół. Zdarzenie, pod nim zmień y o -10 i następny kostium. Też bez kierunku.
- [mów] Dlaczego przy górze i dole nie zmieniamy kierunku? Bo wtedy postać położyłaby się na plecach albo stanęła na głowie. W grach postać patrzy w lewo albo w prawo, nawet gdy idzie do góry.
- Krok 8: dwie minuty na swobodne pobieganie po scenie. Zasłużyliście.

### Wskazówki
- [błąd] Postać do góry nogami przy ruchu w lewo — brakuje bloczka ustaw styl obrotu na lewo-prawo w skrypcie startowym. Najczęstszy problem tej lekcji.
- [błąd] Postać reaguje z opóźnieniem i szarpie przy przytrzymaniu klawisza. To normalne przy tej wersji sterowania — system operacyjny czeka chwilę przed powtórzeniem klawisza. Płynne sterowanie robimy na lekcji 19, powiedz to dzieciom wprost, żeby nie myślały, że coś zepsuły.
- [błąd] Działa tylko jedna strzałka — dziecko zbudowało cztery skrypty, ale we wszystkich zostawiło ten sam klawisz w rozwijanej liście. Niech sprawdzi każdy kapelusz po kolei.
- [błąd] Skrypt wylądował na Scenie zamiast na duszku. Poznasz to po tym, że w obszarze skryptu nie ma kategorii Ruch. Niech kliknie miniaturkę swojej postaci w prawym dolnym rogu.
- [dla szybszych] Niech dobiorą różne prędkości dla różnych kierunków albo zwiększą krok do 20 i sprawdzą, czy nadal da się celnie sterować.
- [gdy nie zdążysz] Zrób dwie strzałki, w lewo i w prawo. Góra i dół wracają po przerwie jako materiał, a nie jako zaległość.

### Materiały
- [kod] Skrypt startowy | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw styl obrotu na [lewo-prawo]
    idź do x: (0) y: (0)
  ```
- [kod] Cztery strzałki | scratch:
  ```
  kiedy klawisz [strzałka w prawo] naciśnięty
    ustaw kierunek na (90)
    przesuń o (10) kroków
    następny kostium
  kiedy klawisz [strzałka w lewo] naciśnięty
    ustaw kierunek na (-90)
    przesuń o (10) kroków
    następny kostium
  kiedy klawisz [strzałka w górę] naciśnięty
    zmień y o (10)
    następny kostium
  kiedy klawisz [strzałka w dół] naciśnięty
    zmień y o (-10)
    następny kostium
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów, piją wodę, rozprostowują się.
- Poproś, żeby nie zamykały okna ze Scratchem.
- Zapowiedź: „Po przerwie wasza postać zacznie zostawiać ślad i skakać — a na koniec zagracie w to, co sami zrobiliście.”

### Wskazówki
- [tempo] Po sterowaniu dzieci są rozgrzane i chcą grać. Przerwa jest tu potrzebna właśnie dlatego — bez niej druga połowa zamienia się w zabawę bez słuchania.

## [concept] Skok po spacji (12 min)

### Co robić teraz
- [mów] Zostały nam jeszcze klawisze. Najważniejszy w grach to spacja — od niej zwykle jest skok.
- Zbuduj razem: kiedy klawisz spacja naciśnięty, a pod spodem cztery bloczki — zmień y o 50, czekaj 0.2 sek, zmień y o -50.
- Wciśnij spację. Postać podskakuje i wraca. **Czekaj na kciuki.**
- [mów] To nie jest prawdziwa grawitacja, tylko skok udawany: do góry, chwila w powietrzu, na dół. Prawdziwą grawitację zrobicie w drugim semestrze.
- Niech każde dziecko dobierze wysokość skoku i czas w powietrzu. Zapytaj dwoje po imieniu, co im wyszło.
- Pokaż, że można dołożyć dźwięk: kategoria Dźwięk, bloczek zagraj dźwięk Pop, wstawiony na początek skoku.

### Wskazówki
- [błąd] Postać po skoku zostaje wyżej, niż była. Przyczyna: dziecko wpisało różne liczby w górę i w dół. Muszą być przeciwne — 50 i -50.
- [błąd] Przy szybkim wciskaniu spacji postać ucieka w górę ekranu. To znany efekt uboczny udawanego skoku — powiedz, że tak ma być na tym etapie, i pokaż, że zielona flaga przywraca porządek.
- [podpowiedź] Minus wpisuje się przed liczbą, ze zwykłego myślnika na klawiaturze. Dla części dzieci to pierwsze spotkanie z liczbą ujemną — pokaż to wolno.
- [dla szybszych] Niech zrobią podwójny skok: dwa razy w górę z krótkim czekaniem, potem powrót.
- [gdy nie zdążysz] Sam skok bez dźwięku wystarczy w zupełności.

### Materiały
- [kod] Skok po spacji | scratch:
  ```
  kiedy klawisz [spacja] naciśnięty
    zagraj dźwięk [Pop]
    zmień y o (50)
    czekaj (0.2) sek
    zmień y o (-50)
  ```

## [guided] Ślad pisaka i sprzątanie sceny (13 min)

### Co robić teraz
- [mów] Teraz zrobimy coś, po czym zobaczycie, gdzie wasza postać naprawdę chodziła.
- Krok 1: pokaż przycisk Dodaj rozszerzenie w lewym dolnym rogu ekranu, na samym dole palety. Wybierzcie razem Pisak. **Czekaj na kciuki.**
- Krok 2: w skrypcie startowym, pod zieloną flagą, dołóż wymaż wszystko i przyłóż pisak.
- Krok 3: kliknij flagę i pobiegaj strzałkami. Postać rysuje linię tam, gdzie idzie.
- [mów] Właśnie zobaczyliście historię swojego ruchu. Wasza postać zostawia ślad jak kredka.
- Krok 4: pokaż bloczki ustaw kolor pisaka i ustaw grubość pisaka na 5. Niech każde dziecko dobierze swój kolor.
- Krok 5: dołóż osobny skrypt czyszczący: kiedy klawisz c naciśnięty, a pod spodem wymaż wszystko.
- [mów] To wasz przycisk do sprzątania. Wciskacie c i scena znowu jest czysta.

### Wskazówki
- [podpowiedź] Rozszerzenia to najbardziej niedoceniana część Scratcha. Dzieci, które raz znajdą ten przycisk, wracają do niego same.
- [błąd] Nic się nie rysuje — brakuje bloczka przyłóż pisak albo dziecko nie kliknęło zielonej flagi po dodaniu skryptu.
- [błąd] Scena zaraz zapełnia się kreskami i nic nie widać — po to jest klawisz c. Przypomnij o nim.
- [błąd] Rysowanie działa, ale ślad zostaje po ponownym starcie — brakuje wymaż wszystko w skrypcie startowym.
- [dla szybszych] Niech dodadzą skrypt: kiedy klawisz p naciśnięty, podnieś pisak — żeby dało się przejść bez rysowania.
- [gdy nie zdążysz] Pomiń kolory i grubość, zostaw sam ślad i klawisz c.

### Materiały
- [kod] Skrypt startowy z pisakiem | scratch:
  ```
  kiedy kliknięto zieloną flagę
    wymaż wszystko
    ustaw styl obrotu na [lewo-prawo]
    idź do x: (0) y: (0)
    ustaw kolor pisaka na [niebieski]
    ustaw grubość pisaka na (5)
    przyłóż pisak
  ```
- [kod] Sprzątanie sceny | scratch:
  ```
  kiedy klawisz [c] naciśnięty
    wymaż wszystko
  ```

## [challenge] Narysuj coś swoją postacią (10 min)

### Co robić teraz
- Zadanie samodzielne: pobiegaj strzałkami tak, żeby narysować literę swojego imienia albo prosty kształt — domek, serce, gwiazdkę.
- Kto się pomyli, wciska c i zaczyna od nowa. O to właśnie chodzi.
- Kto skończy, pokazuje kciuk i czeka z gotowym rysunkiem na ekranie.
- Chodź po grupie i pytaj po imieniu, co rysują.

### Wskazówki
- [tempo] To zadanie wygląda na zabawę i jest zabawą — a przy okazji dziecko dziesięć razy powtarza sterowanie i utrwala je lepiej niż jakimkolwiek ćwiczeniem.
- [dla szybszych] Niech zmieniają kolor pisaka w trakcie rysowania osobnym skryptem pod klawiszem k.
- [gdy nie zdążysz] Wystarczy dowolna kreska. Nie każ kończyć litery.
- [błąd] Dziecko sfrustrowane, bo linia jest krzywa. Powiedz wprost: krzywa linia to nie błąd, to ślad tego, jak naprawdę szła postać.

## [challenge] Pokaz prac i zapisanie projektu (5 min)

### Co robić teraz
- Najpierw zapis: Plik, Zapisz teraz. Poczekaj na potwierdzenie od każdego dziecka.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje swój rysunek i demonstruje sterowanie przez dziesięć sekund.
- Brawa po każdym pokazie. Jedno zdanie pochwały od ciebie, konkretnie o tym projekcie.

### Wskazówki
- [tempo] Zapis przed pokazem, zawsze. Po pokazie kończy się czas i ktoś traci pracę.
- [podpowiedź] Poproś o nazwanie projektu: imię i numer lekcji.

## [summary] Pułapki i podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: co to jest zdarzenie? Ile programów może mieć jedna postać? Który bloczek ratuje przed postacią do góry nogami?
- [mów] Dzisiaj wasza postać zaczęła was słuchać. To jest różnica między filmem a grą — w grze to gracz decyduje.
- Przypomnij zadanie domowe: naucz kogoś w domu sterowania.
- Zajawka: „Na następnych zajęciach wasza postać dostanie prawdziwy świat — tło, muzykę i nastrój. Przestanie biegać po białej pustce.”

### Wskazówki
- [błąd] Postać do góry nogami — brakuje bloczka ustaw styl obrotu na lewo-prawo.
- [błąd] Strzałki nie działają — trzeba raz kliknąć na scenę, żeby klawiatura trafiała do Scratcha.
- [błąd] Działa tylko jedna strzałka — w pozostałych skryptach został ten sam klawisz w rozwijanej liście.
- [błąd] Skrypt trafił na Scenę zamiast na duszka — w obszarze skryptu brakuje wtedy kategorii Ruch.
- [błąd] Postać ucieka poza scenę — zielona flaga przywraca ją na środek, bo mamy bloczek idź do x: 0 y: 0.
