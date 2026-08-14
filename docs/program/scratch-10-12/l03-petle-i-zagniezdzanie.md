# Pętle i zagnieżdżanie — wzory z pisaka
Subject: Scratch
Level: Poziom 2 — 10-12 lat
Czas: 95 min
Tags: Pętle, Zagnieżdżanie, Pisak, Geometria
Opis: Pętla w pętli na konkretnym, widocznym przykładzie. Dzieci rysują pisakiem wielokąty, rozety i spirale, a potem czytają cudzy kod i przewidują, co narysuje.
Cel: Dziecko potrafi zagnieździć pętlę w pętli, przewidzieć wynik takiego kodu przed uruchomieniem i wyliczyć kąt obrotu dla wielokąta o dowolnej liczbie boków.

### Po zajęciach dziecko potrafi
- zbudować pętlę wewnątrz pętli i wyjaśnić, ile razy wykona się jej wnętrze
- wyliczyć kąt obrotu dla wielokąta o zadanej liczbie boków
- narysować pisakiem wielokąt, rozetę i spiralę
- przeczytać cudzy kod z pętlą i przewidzieć efekt bez uruchamiania

### Przygotuj przed zajęciami
- gotowe demo: kwadrat, sześciokąt, rozeta z kwadratów i spirala
- przygotowane dwa zagadki-kody do odczytania: jedna prosta, jedna z pułapką
- projekty dzieci z lekcji 2
- kartka i długopis dla każdego dziecka — dziś liczymy kąty

### Zadanie domowe
- narysuj pisakiem coś, czego dziś nie robiliśmy, i przynieś na następne zajęcia zrzut ekranu

## [intro] Powitanie i demo wzorów (8 min)

### Co robić teraz
- [mów] Cześć. Dziś pokażę wam coś, co wygląda na trudne, a jest krótsze niż wasze dotychczasowe skrypty.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** uruchom po kolei cztery rysunki — kwadrat, sześciokąt, rozeta z trzydziestu sześciu kwadratów, spirala. Po każdym pokaż kod.
- [mów] Popatrzcie na tę rozetę. Trzydzieści sześć kwadratów, sto czterdzieści cztery boki. Kod ma sześć bloczków.
- Zapytaj grupę: ile bloczków potrzebowalibyście, żeby narysować to bez pętli?
- [mów] To jest dzisiejszy temat: pętla w pętli. Jeden z tych pomysłów, po których widzi się kod inaczej.

### Wskazówki
- [tempo] Pokaż rozetę, a potem jej kod. Zaskoczenie proporcją między efektem a długością kodu jest tu całym paliwem lekcji.
- [podpowiedź] Nie tłumacz na razie, jak to działa. Obietnica wystarczy.

## [review] Otwieramy projekt i przypominamy współrzędne (5 min)

### Co robić teraz
- Wszyscy otwierają Scratcha. Dziś zaczynamy nowy projekt — nazwijcie go „Wzory”.
- Szybka powtórka: jakie są zakresy x i y? Czym różni się idź do od przesuń o? Co robi bloczek skieruj się w stronę?
- Kto zrobił zadanie domowe z trasą, wysyła na czat liczbę punktów, których użył.
- [mów] Dziś zostawiamy grę na boku i zajmujemy się samym kodem. Wrócimy do gry na następnych zajęciach z narzędziem, które ją odmieni.

### Wskazówki
- [podpowiedź] Nowy projekt jest tu celowy: pisak na scenie z grą robi bałagan, a dzieci nie chcą psuć czegoś, co działa.
- [gdy nie zdążysz] Powtórkę pomiń, przejdź do dodania rozszerzenia.

## [concept] Kwadrat, czyli pętla i kąt (14 min)

### Co robić teraz
- Krok 1: dodajcie rozszerzenie Pisak — przycisk na samym dole palety, po lewej.
- Krok 2: zbudujcie stan początkowy: kiedy kliknięto zieloną flagę, wymaż wszystko, idź do x: 0 y: 0, ustaw kierunek na 90, ustaw grubość pisaka na 3, przyłóż pisak.
- Krok 3: narysujcie kwadrat **bez pętli**: cztery razy przesuń o 100 kroków i obróć w prawo o 90 stopni. Osiem bloczków.
- [mów] Działa. Teraz policzmy razem, co się powtarza: przesuń i obróć, cztery razy. To jest wzór, a wzór idzie do pętli.
- Krok 4: zastąpcie osiem bloczków pętlą powtórz 4 z dwoma bloczkami w środku. Uruchomcie — ten sam kwadrat, dwa bloczki plus pętla.
- Krok 5: teraz najważniejsze pytanie lekcji. Zmieńcie 4 na 3 i uruchomcie. Kształt się nie zamyka. Dlaczego?
- Zbierz odpowiedzi. Potem wyjaśnij: żeby wrócić do punktu startu, postać musi obrócić się w sumie o pełne 360 stopni. Cztery razy 90 to 360. Trzy razy 90 to tylko 270.
- [mów] Stąd bierze się reguła, którą zapiszcie na kartce: **kąt obrotu to 360 podzielone przez liczbę boków**.
- Krok 6: ćwiczenie na kartce. Policzcie kąt dla trójkąta, sześciokąta i ośmiokąta. Wpiszcie odpowiedzi na czat.
- Krok 7: sprawdźcie w kodzie, czy wasze liczby są dobre.

### Wskazówki
- [podpowiedź] Kolejność „najprzód bez pętli, potem z pętlą” nie jest stratą czasu. Dziecko, które samo zauważyło powtarzający się wzór, rozumie pętlę zupełnie inaczej niż to, któremu ją podano.
- [błąd] Trójkąt z kątem 60 zamiast 120. To najczęstsza pomyłka: dzieci wpisują kąt wewnętrzny trójkąta z lekcji matematyki, a pisak obraca się o kąt zewnętrzny. Pokaż na żywo, że 60 daje kształt niedomknięty, a 120 domknięty.
- [błąd] Nic się nie rysuje — brakuje bloczka przyłóż pisak albo dziecko nie kliknęło zielonej flagi.
- [błąd] Poprzednie rysunki zostają na scenie — brakuje wymaż wszystko w stanie początkowym.
- [dla szybszych] Niech narysują kształt o dwudziestu bokach i wyjaśnią, dlaczego wygląda jak okrąg.
- [gdy nie zdążysz] Kwadrat plus jeden inny wielokąt. Wzór na kąt podaj wprost, bez wyprowadzania.

### Materiały
- [kod] Kwadrat bez pętli | scratch:
  ```
  kiedy kliknięto zieloną flagę
    wymaż wszystko
    idź do x: (0) y: (0)
    ustaw kierunek na (90)
    przyłóż pisak
    przesuń o (100) kroków
    obróć w prawo o (90) stopni
    przesuń o (100) kroków
    obróć w prawo o (90) stopni
    przesuń o (100) kroków
    obróć w prawo o (90) stopni
    przesuń o (100) kroków
    obróć w prawo o (90) stopni
  ```
- [kod] Wielokąt z pętlą | scratch:
  ```
  kiedy kliknięto zieloną flagę
    wymaż wszystko
    idź do x: (0) y: (0)
    ustaw kierunek na (90)
    ustaw grubość pisaka na (3)
    przyłóż pisak
    powtórz (6)
      przesuń o (80) kroków
      obróć w prawo o (60) stopni
  ```

## [guided] Pętla w pętli — rozeta (18 min)

### Co robić teraz
- [mów] Umiecie narysować jeden kwadrat. A gdybyście chcieli narysować trzydzieści sześć kwadratów, każdy obrócony o dziesięć stopni?
- [mów] Odpowiedź brzmi: to znowu wzór. Wzorem jest tym razem cały kwadrat.
- Krok 1: weźcie swoją pętlę rysującą kwadrat i **otoczcie ją** drugą pętlą powtórz 36. Przeciągnijcie zewnętrzną pętlę tak, żeby wewnętrzna wpadła do jej środka.
- [mów] Robimy to powoli. Zewnętrzna pętla ma objąć całą wewnętrzną, nie stanąć pod nią.
- Krok 2: do zewnętrznej pętli, **pod** wewnętrzną, dołóżcie obróć w prawo o 10 stopni.
- Krok 3: uruchomcie. Rozeta. Wpiszcie na czat G albo znak zapytania.
- Krok 4: policzmy razem, ile bloczków się wykonało. Wewnętrzna pętla to 4 obroty, zewnętrzna 36. Cztery razy trzydzieści sześć to sto czterdzieści cztery pary bloczków.
- [mów] Napisaliście cztery bloczki, a komputer wykonał prawie trzysta operacji. To jest cały sens zagnieżdżania.
- Krok 5: eksperyment. Zmieńcie liczbę w zewnętrznej pętli na 12 i kąt na 30. Uruchomcie. Kształt jest rzadszy, ale nadal domknięty.
- [mów] Zauważcie zależność: liczba powtórzeń razy kąt musi dawać 360. Ta sama reguła co przy wielokącie, tylko o poziom wyżej.
- Krok 6: każde dziecko dobiera własną parę liczb i sprawdza, czy rozeta się domyka. Trzy próby.

### Wskazówki
- [błąd] Zewnętrzna pętla wylądowała **pod** wewnętrzną zamiast ją otoczyć. Efekt: rysuje się jeden kwadrat, a potem postać kręci się w miejscu. Poznasz to natychmiast i warto pokazać na żywo, bo to najczęstszy błąd tej lekcji.
- [błąd] Bloczek obrotu trafił do wewnętrznej pętli zamiast do zewnętrznej. Efekt jest zupełnie inny i też ciekawy — pokaż go jako przykład, że miejsce bloczka to połowa znaczenia kodu.
- [błąd] Rysunek nie domyka się w koło, bo liczba powtórzeń razy kąt nie daje 360.
- [błąd] Rysowanie trwa bardzo długo. Włączcie tryb turbo: shift plus kliknięcie zielonej flagi. To dobra okazja, żeby pokazać, że kod może być za wolny — temat lekcji 32.
- [dla szybszych] Niech zbudują trzy pętle jedna w drugiej i opiszą, ile razy wykona się najgłębsza.
- [gdy nie zdążysz] Sama rozeta z gotowymi liczbami 36 i 10, bez eksperymentowania z parami.

### Materiały
- [kod] Rozeta — pętla w pętli | scratch:
  ```
  kiedy kliknięto zieloną flagę
    wymaż wszystko
    idź do x: (0) y: (0)
    ustaw kierunek na (90)
    ustaw grubość pisaka na (2)
    przyłóż pisak
    powtórz (36)
      powtórz (4)
        przesuń o (80) kroków
        obróć w prawo o (90) stopni
      obróć w prawo o (10) stopni
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Zapowiedź: „Po przerwie dostaniecie cudzy kod i będziecie musieli powiedzieć, co narysuje — bez uruchamiania. A potem zrobimy spirale i kolory.”

### Wskazówki
- [tempo] Rysowanie rozet wciąga i grupa sama nie zrobi sobie przerwy. Przerwij twardo.

## [concept] Czytanie kodu i spirala (13 min)

### Co robić teraz
- [mów] Programista częściej czyta kod niż go pisze. Teraz sprawdzimy, czy umiecie przewidzieć, co zrobi program.
- Zagadka 1: udostępnij na ekranie kod z pętlą powtórz 5, w niej przesuń o 50 i obróć w prawo o 72 stopnie. Nie uruchamiaj. Pytanie na czat: co narysuje?
- Zbierz odpowiedzi, potem uruchom. Pięciokąt — bo 5 razy 72 to 360.
- Zagadka 2, z pułapką: pętla powtórz 10, w niej przesuń o 20 kroków, obróć w prawo o 36 stopni oraz **zmień grubość pisaka o 2**. Pytanie: czym to się różni od poprzedniego?
- Zbierz odpowiedzi, potem uruchom. Kształt jest dziesięciokątem, ale linia grubieje. Zwróć uwagę, ile dzieci to przewidziało.
- [mów] Teraz spirala. Wystarczy jedna zmiana w kwadracie: niech każdy bok będzie **dłuższy** od poprzedniego.
- Pokaż bloczek zmień grubość pisaka i wyjaśnij różnicę: sam bloczek przesuń o ma stałą liczbę. Żeby rosła, potrzebna jest wartość, która się zmienia — a to jest zmienna, temat następnej lekcji.
- [mów] Na dziś jest sztuczka bez zmiennej: zamiast wydłużać bok, po każdym boku obracajcie się o kąt trochę inny od pełnego. Wychodzi spirala.
- Zbudujcie: pętla powtórz 100, w niej przesuń o 30 kroków, obróć w prawo o 89 stopni, zmień kolor pisaka o 2.
- Uruchomcie. Kolorowa spirala z niedomkniętego kwadratu.

### Wskazówki
- [podpowiedź] Ćwiczenie „przewidź, co zrobi kod” jest w tej grupie wiekowej najlepiej wykorzystanym czasem lekcji. Rób je regularnie, nie tylko dziś.
- [podpowiedź] Kąt 89 zamiast 90 to jedna z najładniejszych sztuczek w Scratchu. Pokaż też 91 i 88 — każdy daje inny wzór.
- [błąd] Spirala wychodzi ze sceny i część rysunku ginie. Zmniejsz długość boku albo liczbę powtórzeń.
- [błąd] Kolor nie zmienia się — dziecko użyło ustaw kolor pisaka zamiast zmień kolor pisaka. Znowu ta sama para bloczków co przy efektach.
- [dla szybszych] Niech znajdą kąt, przy którym spirala wygląda najciekawiej, i wyjaśnią, dlaczego kąty bliskie 90 dają gęstsze wzory.
- [gdy nie zdążysz] Jedna zagadka zamiast dwóch, potem od razu spirala.

### Materiały
- [kod] Zagadka — co narysuje? | scratch:
  ```
  powtórz (10)
    przesuń o (20) kroków
    obróć w prawo o (36) stopni
    zmień grubość pisaka o (2)
  ```
- [kod] Kolorowa spirala | scratch:
  ```
  kiedy kliknięto zieloną flagę
    wymaż wszystko
    idź do x: (0) y: (0)
    ustaw kierunek na (90)
    ustaw grubość pisaka na (2)
    przyłóż pisak
    powtórz (100)
      przesuń o (30) kroków
      obróć w prawo o (89) stopni
      zmień kolor pisaka o (2)
  ```

## [guided] Galeria wzorów pod klawiszami (14 min)

### Co robić teraz
- [mów] Zrobimy z tego galerię. Każdy wzór pod swoim klawiszem, żeby dało się je porównywać.
- Krok 1: zbudujcie osobny skrypt sprzątający: kiedy klawisz c naciśnięty, wymaż wszystko, idź do x: 0 y: 0, ustaw kierunek na 90.
- [mów] Sprzątanie w osobnym skrypcie, nie w każdym wzorze. Powtarzanie tego samego kodu w pięciu miejscach to prosta droga do błędu — na lekcji 13 poznacie, jak się tego pozbyć na dobre.
- Krok 2: każdy wzór przenieście pod własny klawisz: 1 to wielokąt, 2 to rozeta, 3 to spirala.
- Krok 3: **każdy** wzór zacznijcie od wymaż wszystko i ustawienia pozycji, żeby dało się przełączać między nimi bez bałaganu.
- Krok 4: sprawdźcie. Wciśnijcie 1, potem 2, potem 3, potem c. Wpiszcie na czat G albo znak zapytania.
- Krok 5: dołóżcie czwarty wzór własnego pomysłu pod klawiszem 4. Zmieńcie kąt, długość boku, kolor, grubość — cokolwiek.
- Krok 6: pokażcie sąsiadowi z grupy, co wam wyszło, wrzucając zrzut ekranu na czat.

### Wskazówki
- [podpowiedź] Uwaga o powtarzającym się kodzie jest tu zasiana celowo. Na lekcji 13, przy własnych blokach, wystarczy do niej wrócić i dzieci od razu wiedzą, po co.
- [błąd] Wzory nakładają się na siebie — brakuje wymaż wszystko na początku każdego z nich.
- [błąd] Drugi wzór rysuje się pod dziwnym kątem, bo postać została obrócona po pierwszym. Ustawienie kierunku na starcie każdego wzoru to rozwiązanie.
- [błąd] Kilka wzorów naraz, bo dziecko wcisnęło dwa klawisze. Scratch uruchomi oba i rysunki się przetną. To nie awaria — dobra okazja, żeby powiedzieć o programach działających równocześnie.
- [dla szybszych] Niech zbudują wzór, w którym rozeta składa się z trójkątów, a nie kwadratów, i porównają wynik.
- [gdy nie zdążysz] Trzy wzory pod klawiszami, czwarty własny pomiń.

## [challenge] Twój wzór (8 min)

### Co robić teraz
- Zadanie samodzielne: zaprojektuj wzór, którego dziś nie robiliśmy.
- Zasada: **najpierw kartka**. Zapisz liczbę powtórzeń zewnętrznej pętli, wewnętrznej i kąty. Policz, czy iloczyn powtórzeń i kąta daje 360.
- Potem buduj. Jeśli wyszło inaczej niż na kartce, znajdź, dlaczego.
- Kto skończy, wrzuca zrzut ekranu na czat.

### Wskazówki
- [tempo] Wymóg policzenia przed zbudowaniem jest tu istotą zadania. Bez niego dzieci będą zgadywać liczby, a to jest lekcja o przewidywaniu.
- [dla szybszych] Niech zrobią wzór z trzema pętlami zagnieżdżonymi i wyliczą, ile bloczków wykona komputer.
- [gdy nie zdążysz] Modyfikacja gotowego wzoru wystarczy za własny.
- [błąd] Wzór wychodzi inaczej, niż dziecko policzyło. To najlepszy moment lekcji — pomóż znaleźć różnicę między planem a kodem, nie podawaj odpowiedzi.

## [challenge] Pokaz prac i zapis (5 min)

### Co robić teraz
- Zapis: Plik, Zapisz teraz. Poczekaj na potwierdzenie od każdego dziecka.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje swój wzór i mówi, ile razy wykonała się najgłębsza pętla.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Pytanie o liczbę wykonań najgłębszej pętli jest najlepszym sprawdzeniem, czy dziecko zrozumiało zagnieżdżanie. Odpowiedź to zawsze iloczyn.
- [tempo] Zapis przed pokazem, zawsze.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: ile razy wykona się wnętrze pętli powtórz 5 zagnieżdżonej w pętli powtórz 10? Jak wyliczyć kąt dla wielokąta? Dlaczego rysunek się nie domyka?
- [mów] Dzisiaj napisaliście kod, który wykonał kilkaset operacji, i zmieściliście go w pięciu bloczkach. To jest różnica między poleceniem a wzorem.
- [mów] I jedna rzecz na koniec: zauważyliście, że w spirali brakowało wam czegoś? Wartości, która sama się zmienia. Tego właśnie brakuje w waszym kodzie od pierwszej lekcji.
- Zajawka: „Na następnych zajęciach poznacie zmienne. Wasza gra dowie się, ile macie punktów, ile życia i jak długo trwa rozgrywka.”
- Przypomnij zadanie domowe.

### Wskazówki
- [błąd] Zewnętrzna pętla pod wewnętrzną zamiast wokół niej — rysuje się tylko jeden kształt.
- [błąd] Kąt wewnętrzny zamiast zewnętrznego — trójkąt z 60 zamiast 120 stopni.
- [błąd] Brak wymaż wszystko — wzory nakładają się na siebie.
- [błąd] Postać zostaje obrócona po poprzednim wzorze — ustawienie kierunku należy do stanu początkowego.
- [błąd] Rysowanie za wolne — tryb turbo przez shift i kliknięcie zielonej flagi.
