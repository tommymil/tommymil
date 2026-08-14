# Pętle zagnieżdżone — pomieszczenie z trzech liczb
Subject: Minecraft Education
Level: Poziom 2 — 10-12 lat, kod
Czas: 95 min
Tags: Pętle zagnieżdżone, Podłoga, Pomieszczenie, Złożoność
Opis: Trzy zagnieżdżone pętle budują pełne pomieszczenie. Dzieci uczą się liczyć operacje jako iloczyn i odkrywają, jak szybko rośnie praca komputera.
Cel: Dziecko buduje trójwymiarową konstrukcję trzema zagnieżdżonymi pętlami, przewiduje liczbę operacji jako iloczyn i steruje całością trzema zmiennymi.

### Po zajęciach dziecko potrafi
- zagnieździć pętlę w pętli i w kolejnej pętli
- policzyć liczbę operacji jako iloczyn trzech liczb
- zbudować pomieszczenie o dowolnych wymiarach sterowane zmiennymi
- rozpoznać, kiedy program zrobi za dużo pracy

### Przygotuj przed zajęciami
- gotowe demo: pomieszczenie budowane z trzech liczb, w dwóch wyraźnie różnych rozmiarach
- światy dzieci z lekcji 4
- przygotowana tabelka do wypełniania na ekranie: wymiary i liczba bloków
- kartka dla każdego dziecka

### Zadanie domowe
- policz na kartce, ile bloków ma pomieszczenie 12 na 9 na 4, i sprawdź to programem

## [intro] Powitanie i demo pomieszczenia (8 min)

### Co robić teraz
- [mów] Cześć. Dziś zbudujecie pełne pomieszczenie i będziecie nim sterować trzema liczbami.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** uruchom program budujący pokój. Wejdź do środka. Potem zmień trzy liczby na górze i uruchom w innym miejscu.
- Drugi pokój jest wyraźnie inny — dłuższy, węższy, wyższy.
- [mów] Ten sam program, inne trzy liczby. Nie zmieniłem ani jednej linijki poza deklaracjami.
- [mów] I jeszcze jedno. Ten pierwszy pokój ma niecałe trzysta bloków. Ten drugi ponad tysiąc. Dziś nauczycie się liczyć, ile pracy zadajecie komputerowi.
- Odczytaj kilka zadań domowych: jakie budowle dzieci zrobiły z licznikiem.

### Wskazówki
- [podpowiedź] Pokazanie dwóch bardzo różnych pokoi z tego samego kodu jest tu mocniejsze niż pokazanie jednego. Różnica ma być widoczna od razu.
- [tempo] To lekcja, w której zbiega się wszystko z poprzednich czterech. Warto powiedzieć to wprost na starcie.

## [review] Wracamy do pętli z licznikiem (5 min)

### Co robić teraz
- Wszyscy otwierają świat z lekcji 4 i Code Builder w widoku JavaScript. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie program ze schodami albo murem.
- Szybka powtórka: jakie są trzy części nagłówka pętli? Ile razy wykona się pętla licząca od zera z warunkiem mniejszy niż sześć? Po co używać licznika w środku?
- Wszyscy odlatują na duże, puste, płaskie miejsce.
- [mów] Dziś potrzebujecie naprawdę dużo miejsca. Pomieszczenie dwadzieścia na piętnaście zajmuje więcej, niż się wydaje.

### Wskazówki
- [błąd] Świat zabudowany. Nowy płaski świat to trzydzieści sekund i lepiej to zrobić niż walczyć o miejsce.
- [gdy nie zdążysz] Sam start i przelot.

## [concept] Podłoga — dwie pętle (14 min)

### Co robić teraz
- [mów] Podłoga to rzędy. Rząd umiecie od zeszłego tygodnia. Dziś powtórzymy rząd wiele razy.
- Krok 1: zadeklarujcie zmienne dlugosc równa 10 i szerokosc równa 8.
- Krok 2: napiszcie pętlę budującą jeden rząd o długości dlugosc. To jest kod z zeszłego tygodnia.
- Krok 3: uruchomcie i sprawdźcie, że rząd działa.
- Krok 4: teraz przejście do następnego rzędu. Po pętli dopiszcie: skręt, krok w przód, skręt.
- [mów] Zapamiętajcie tę sekwencję: skręć, krok, skręć. Ona odwraca Agenta i przesuwa go o rząd w bok.
- Krok 5: otoczcie całość — pętlę rzędu **razem z** sekwencją zawracania — zewnętrzną pętlą liczącą do szerokosc.
- Krok 6: uruchomcie. Wlećcie wysoko. Pełna podłoga. **Czekaj na kciuki.**
- Krok 7: policzcie razem. Wewnętrzna pętla wykonuje się dlugosc razy. Zewnętrzna powtarza to szerokosc razy.
- [mów] **Dziesięć razy osiem to osiemdziesiąt.** Tyle bloków postawi wasz program i tyle razy wykona się wnętrze najgłębszej pętli.
- Krok 8: wyświetlcie tę liczbę na czacie, wyliczoną przez program: dlugosc razy szerokosc.
- Krok 9: zmieńcie wymiary na 20 na 15 i sprawdźcie, czy wypisana liczba się zgadza z tym, co widzicie.

### Wskazówki
- [podpowiedź] Wyliczenie liczby bloków przez program i porównanie z rzeczywistością jest tu najlepszym sprawdzeniem zrozumienia. Nie pomijaj tego kroku.
- [błąd] Zewnętrzna pętla otacza tylko pętlę rzędu, bez sekwencji zawracania. Wtedy rzędy budują się jeden na drugim. Bardzo częste — sprawdź wcięcia.
- [błąd] Podłoga wychodzi jak wężyk, bo brakuje jednego ze skrętów.
- [błąd] Rzędy są oddalone od siebie o dwa bloki, bo krok w bok jest za duży.
- [błąd] Dziecko używa tej samej nazwy licznika w obu pętlach. Zadziała, ale przy trzech pętlach zrobi się bałagan. Zaproponuj nazwy: kolumna, rzad, poziom.
- [dla szybszych] Niech zbudują podłogę w szachownicę, używając reszty z dzielenia sumy dwóch liczników przez dwa.
- [gdy nie zdążysz] Podłoga pięć na pięć.

### Materiały
- [kod] Podłoga z dwóch pętli | javascript:
  ```
  let dlugosc = 10
  let szerokosc = 8

  player.onChat("podloga", function () {
      agent.teleportToPlayer()
      agent.setItem(PLANKS, 512, 1)
      agent.setSlot(1)
      for (let rzad = 0; rzad < szerokosc; rzad++) {
          for (let kolumna = 0; kolumna < dlugosc; kolumna++) {
              agent.move(FORWARD, 1)
              agent.place(DOWN)
          }
          agent.turn(RIGHT)
          agent.move(FORWARD, 1)
          agent.turn(RIGHT)
      }
      player.say("Blokow: " + dlugosc * szerokosc)
  })
  ```

## [guided] Trzecia pętla — od podłogi do bryły (18 min)

### Co robić teraz
- [mów] Dwie pętle dają płaszczyznę. Trzecia da bryłę.
- Krok 1: zadeklarujcie trzecią zmienną: wysokosc równa 3.
- Krok 2: otoczcie **cały** kod podłogi trzecią pętlą, liczącą do wysokosc.
- Krok 3: na końcu tej pętli, po zbudowaniu warstwy, dodajcie ruch w górę o 1.
- Krok 4: uruchomcie z małymi wartościami: 4 na 4 na 2. Zobaczcie, co powstaje.
- [mów] To jest pełna bryła — prostopadłościan wypełniony blokami. Nie pokój, tylko lita kostka.
- Krok 5: policzcie razem. Cztery razy cztery razy dwa to trzydzieści dwa bloki.
- [mów] **Trzy pętle to mnożenie trzech liczb.** To jest reguła na cały ten kurs.
- Krok 6: teraz eksperyment z rosnącymi liczbami. Wypełnijcie tabelkę na ekranie razem ze mną.
- 5 na 5 na 5 to sto dwadzieścia pięć. 10 na 10 na 10 to tysiąc. 20 na 20 na 20 to osiem tysięcy.
- [mów] Zwróćcie uwagę, co się dzieje. Podwoiliśmy każdą liczbę, a pracy jest osiem razy więcej. Nie dwa razy — osiem.
- Krok 7: uruchomcie wersję 10 na 10 na 10 i policzcie na głos, ile trwa.
- [mów] Zapamiętajcie to uczucie. Kiedy program działa bardzo wolno, prawie zawsze przyczyną jest pętla w pętli w pętli.
- Krok 8: **nie** uruchamiajcie 20 na 20 na 20. Osiem tysięcy bloków zajęłoby resztę lekcji.

### Wskazówki
- [podpowiedź] Tabelka z rosnącymi liczbami jest najlepszym możliwym wprowadzeniem do pojęcia złożoności. Nie używaj tego słowa, ale pokaż zjawisko.
- [podpowiedź] Wyraźne odradzenie uruchomienia 20 na 20 na 20 sprawi, że ktoś to zrobi. Zaplanuj to i wykorzystaj jako przykład, ale ostrzeż resztę grupy.
- [błąd] Bryła buduje się w bok zamiast w górę, bo ruch w górę trafił do środkowej pętli.
- [błąd] Kolejne warstwy są przesunięte, bo po zbudowaniu warstwy Agent nie wraca na punkt startowy. To jest **temat kroku po przerwie** — powiedz to i na razie zostawcie.
- [błąd] Agentowi kończy się materiał przy tysiącu bloków. Załadujcie duży stos.
- [błąd] Dziecko uruchamia bardzo dużą bryłę i gra przestaje odpowiadać. Wyjście przez menu i ponowne wejście rozwiązuje sprawę.
- [dla szybszych] Niech policzą, ile bloków ma bryła 50 na 50 na 50, i oszacują czas na podstawie pomiaru z 10 na 10 na 10.
- [gdy nie zdążysz] Bryła 4 na 4 na 2 i tabelka. Pomiar czasu pomiń.

### Materiały
- [kod] Bryła z trzech pętli | javascript:
  ```
  let dlugosc = 4
  let szerokosc = 4
  let wysokosc = 2

  player.onChat("bryla", function () {
      agent.teleportToPlayer()
      agent.setItem(PLANKS, 512, 1)
      agent.setSlot(1)
      for (let poziom = 0; poziom < wysokosc; poziom++) {
          for (let rzad = 0; rzad < szerokosc; rzad++) {
              for (let kolumna = 0; kolumna < dlugosc; kolumna++) {
                  agent.move(FORWARD, 1)
                  agent.place(DOWN)
              }
              agent.turn(RIGHT)
              agent.move(FORWARD, 1)
              agent.turn(RIGHT)
          }
          agent.move(UP, 1)
      }
      player.say("Blokow: " + dlugosc * szerokosc * wysokosc)
  })
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały edytora.
- Zapowiedź: „Po przerwie zamienimy litą bryłę w prawdziwy pokój — z pustym środkiem. I rozwiążemy problem Agenta, który gubi pozycję.”

### Wskazówki
- [tempo] Trzy zagnieżdżone pętle to najtrudniejszy materiał tej ścieżki w pierwszym miesiącu. Przerwa jest obowiązkowa.

## [concept] Powrót Agenta na punkt startowy (13 min)

### Co robić teraz
- [mów] Zauważyliście, że kolejne warstwy się rozjeżdżają. To nie jest błąd pętli. To jest to, gdzie kończy Agent.
- Krok 1: uruchomcie program z wysokością równą 1 i sprawdźcie, gdzie Agent stoi po zakończeniu. Zapiszcie to.
- [mów] To jest podstawowa metoda z lekcji drugiej: sprawdź, gdzie stoi Agent. Dziś jest kluczowa.
- Krok 2: policzcie, ile kroków i skrętów potrzeba, żeby wrócił na start.
- [mów] I tu jest problem: ta liczba zależy od tego, czy szerokość jest parzysta czy nieparzysta. Agent kończy raz z jednej, raz z drugiej strony.
- Krok 3: pokaż prostsze rozwiązanie: zamiast liczyć powrót, **przenieście Agenta do gracza** na początku każdej warstwy, a potem podnieście go o tyle bloków, ile wynosi numer warstwy.
- Krok 4: napiszcie to. Na początku pętli warstwy: przeniesienie do gracza, a potem ruch w górę o licznik warstwy.
- Krok 5: uruchomcie z wysokością 3. Warstwy są równo jedna nad drugą. **Czekaj na kciuki.**
- [mów] Zwróćcie uwagę, co zrobiliśmy: zamiast liczyć drogę powrotną, **wróciliśmy do znanego punktu**. To jest bardzo częsty sposób w programowaniu — nie licz, tylko zresetuj do stanu, który znasz.
- Krok 6: warunek działania: gracz musi stać nieruchomo w trakcie budowy. Powiedzcie to sobie na głos.

### Wskazówki
- [podpowiedź] Zasada „nie licz drogi powrotnej, wróć do znanego punktu” jest ważnym sposobem myślenia. Wraca na lekcji 8 przy współrzędnych bezwzględnych, gdzie dostanie lepsze rozwiązanie.
- [błąd] Gracz przesuwa się w trakcie budowy i warstwy się rozjeżdżają. To ograniczenie tej metody — nazwij je wprost, żeby dzieci wiedziały, że to nie awaria.
- [błąd] Warstwy budują się w tym samym miejscu, bo brakuje ruchu w górę o licznik.
- [błąd] Ruch w górę o stałą jedynkę zamiast o licznik — warstwy wchodzą jedna w drugą, bo Agent wraca na dół przy każdym przeniesieniu.
- [dla szybszych] Niech spróbują policzyć drogę powrotną poprawnie dla obu przypadków, parzystego i nieparzystego. To trudne i pouczające.
- [gdy nie zdążysz] Sama metoda z przeniesieniem, bez omawiania parzystości.

## [guided] Pokój z pustym środkiem (14 min)

### Co robić teraz
- [mów] Lita bryła to nie pokój. Pokój ma ściany i pusty środek. Zrobimy to najprościej: zbudujemy tylko brzegi.
- Krok 1: podłoga zostaje pełna — to jest ta sama podwójna pętla co przedtem, uruchomiona raz, na dole.
- Krok 2: ściany to obwód, powtórzony w górę. Napiszcie osobną pętlę: dla każdego poziomu obejdźcie prostokąt, stawiając blok za sobą.
- Krok 3: obejście prostokąta to dwie pary: bok dlugosc, skręt, bok szerokosc, skręt — całość powtórzona dwa razy.
- Krok 4: uruchomcie. Ściany stoją na krawędzi podłogi.
- Krok 5: sufit to podłoga zbudowana na wysokości ścian. Uruchomcie kod podłogi jeszcze raz, po wejściu na górę.
- Krok 6: policzcie razem, ile bloków ma cały pokój. Podłoga plus sufit plus obwód razy wysokość.
- Krok 7: wyświetlcie tę liczbę na czacie, wyliczoną wzorem.
- Krok 8: wykujcie drzwi ręcznie i wejdźcie do środka.
- [mów] Porównajcie z bryłą litą: pokój dziesięć na osiem na trzy ma około trzystu bloków, a lita bryła o tych wymiarach dwieście czterdzieści. Ale w bryle nie da się mieszkać.

### Wskazówki
- [podpowiedź] Wyliczenie liczby bloków wzorem, a nie zliczeniem, jest tu głównym ćwiczeniem matematycznym. Napiszcie wzór razem na ekranie.
- [błąd] Ściany nie stoją na krawędzi podłogi, bo Agent startuje z innego rogu. Ustalcie regułę: ta sama pozycja gracza dla wszystkich części.
- [błąd] Narożniki ścian są zdublowane albo brakuje w nich bloku. Znany problem, zależny od kolejności ruchu i stawiania.
- [błąd] Sufit jest o jeden blok za wysoko albo za nisko. Sprawdźcie, ile razy Agent wszedł w górę.
- [błąd] Wzór na liczbę bloków nie zgadza się z rzeczywistością, bo dziecko policzyło narożniki dwa razy. To dobra, konkretna rozmowa o liczeniu.
- [dla szybszych] Niech zbudują pokój z oknem: przerwa w pętli ściany na wybranym poziomie.
- [gdy nie zdążysz] Podłoga i ściany bez sufitu.

## [challenge] Twój pokój z trzech liczb (8 min)

### Co robić teraz
- Zadanie samodzielne: napisz program budujący pokój, sterowany wyłącznie trzema zmiennymi na górze.
- Test: podaj sobie trzy zestawy wymiarów i sprawdź, czy wszystkie działają bez zmiany reszty kodu.
- Przed każdym uruchomieniem policz na kartce, ile bloków powinno powstać, i sprawdź z tym, co wypisze program.
- Kto skończy, wkleja na czat swój wzór na liczbę bloków.

### Wskazówki
- [tempo] Porównanie własnego rachunku z wynikiem programu jest tu najważniejsze. Kto się pomylił, ma najlepszy możliwy materiał do nauki.
- [dla szybszych] Niech dodadzą czwartą zmienną sterującą grubością ścian.
- [gdy nie zdążysz] Dwa zestawy wymiarów zamiast trzech.
- [błąd] Program działa dla pierwszego zestawu, a dla drugiego się rozjeżdża — gdzieś została liczba wpisana na sztywno.

## [challenge] Pokaz i wyjście ze świata (5 min)

### Co robić teraz
- Przypomnij o wyjściu przez menu.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje kod, zmienia trzy liczby i buduje pokój na oczach grupy.
- Poproś, żeby podało, ile bloków postawi program.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Zmiana liczb na żywo i budowa od zera jest tu najmocniejszym pokazem. Poproś, żeby dzieci odleciały na puste miejsce przed swoją kolejką.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: ile operacji wykona pętla 5 w pętli 4 w pętli 3? Co się dzieje z liczbą operacji, gdy podwoicie wszystkie trzy wymiary? Jak najprościej sprowadzić Agenta na znany punkt?
- [mów] Dzisiaj zbudowaliście trójwymiarową konstrukcję trzema pętlami i trzema liczbami. I zobaczyliście coś ważnego: praca komputera rośnie znacznie szybciej niż liczby w waszym programie.
- [mów] Ale zauważcie też problem. Wasz program robi się długi. Podłoga, ściany, sufit — to trzy podobne kawałki kodu, a wy przeklejacie je i poprawiacie ręcznie.
- Zajawka: „Na następnych zajęciach poznacie funkcje. Napiszecie przepis raz i będziecie go wywoływać, kiedy chcecie.”
- Przypomnij zadanie domowe: policz pokój 12 na 9 na 4 i sprawdź programem.

### Wskazówki
- [błąd] Zewnętrzna pętla nie obejmuje sekwencji zawracania — rzędy budują się na sobie.
- [błąd] Warstwy przesunięte — Agent nie wraca na punkt startowy.
- [błąd] Ruch w górę o stałą wartość zamiast o licznik warstwy.
- [błąd] Kończy się materiał — tysiąc bloków to więcej niż kilkanaście stosów.
- [błąd] Gra przestaje odpowiadać przy bardzo dużych wymiarach — wyjdź przez menu i wróć.
