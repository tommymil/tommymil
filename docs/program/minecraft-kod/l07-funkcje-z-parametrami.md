# Funkcje z parametrami — jeden przepis, wiele budowli
Subject: Minecraft Education
Level: Poziom 2 — 10-12 lat, kod
Czas: 95 min
Tags: Parametry, Argumenty, Generator, Wielokrotne użycie
Opis: Funkcja dostaje parametry i z jednego przepisu powstaje dowolna liczba różnych budowli. Dzieci budują generator domków.
Cel: Dziecko definiuje funkcję z parametrami, wywołuje ją z różnymi wartościami i buduje generator, który jednym przepisem stawia wiele różnych obiektów.

### Po zajęciach dziecko potrafi
- dodać parametry do definicji funkcji i użyć ich w środku
- wywołać tę samą funkcję z różnymi wartościami
- zbudować generator stawiający wiele różnych budowli z jednego przepisu
- odróżnić parametr od zmiennej globalnej i powiedzieć, kiedy który jest lepszy

### Przygotuj przed zajęciami
- gotowe demo: generator ulicy z dziesięcioma różnymi domkami z jednej funkcji
- światy dzieci z lekcji 6
- przygotowany zły przykład: pięć prawie identycznych funkcji różniących się jedną liczbą
- kartka dla każdego dziecka

### Zadanie domowe
- napisz funkcję z trzema parametrami i wywołaj ją pięć razy z różnymi wartościami

## [intro] Powitanie i demo generatora (8 min)

### Co robić teraz
- [mów] Cześć. Dziś wasze funkcje przestaną budować zawsze to samo.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** uruchom generator ulicy. Powstaje dziesięć domków — różnej wielkości, z różnych materiałów, w rzędzie.
- [mów] Ile funkcji budujących domek jest w tym programie? Jedna.
- Pokaż kod: jedna definicja funkcji i dziesięć wywołań z różnymi liczbami.
- [mów] To jest przepis. Jak przepis na ciasto: ten sam, ale możecie zrobić duże albo małe, z jabłkami albo ze śliwkami.
- **Zły przykład:** pokaż drugą wersję, w której jest pięć prawie identycznych funkcji: zbudujDomek5, zbudujDomek7, zbudujDomek9.
- [mów] Ten program działa tak samo, a jest pięć razy dłuższy i pięć razy łatwiej w nim o błąd.
- Odczytaj kilka zadań domowych: na ile funkcji dzieci podzieliły swoje programy.

### Wskazówki
- [podpowiedź] Porównanie z przepisem na ciasto trafia do tej grupy wiekowej lepiej niż jakiekolwiek pojęcie techniczne.
- [tempo] Zły przykład z pięcioma funkcjami różniącymi się jedną liczbą jest realnym scenariuszem po lekcji 6. Ktoś w grupie prawdopodobnie tak zrobił.

## [review] Wracamy do funkcji (5 min)

### Co robić teraz
- Wszyscy otwierają świat z lekcji 6 i Code Builder w widoku JavaScript. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie program budujący pokój z funkcji.
- Szybka powtórka: co to jest funkcja? Kiedy na pewno jej potrzebujesz? Jaka powinna być nazwa?
- Wszyscy odlatują na duże, puste miejsce.
- [mów] Dziś będziecie budować w rzędzie. Potrzebujecie długiego, wolnego pasa terenu.

### Wskazówki
- [gdy nie zdążysz] Sam start i przelot.

## [concept] Parametr, czyli dziurka w przepisie (14 min)

### Co robić teraz
- [mów] Wasza funkcja budująca ścianę ma długość wpisaną w środku. Zrobimy z tego dziurkę, którą wypełnia ten, kto funkcję wywołuje.
- Krok 1: w definicji funkcji, w nawiasach okrągłych po nazwie, napiszcie słowo dlugosc.
- [mów] To jest **parametr**. Do tej pory nawiasy były puste. Teraz coś w nich jest.
- Krok 2: w środku funkcji zamieńcie liczbę na nazwę parametru.
- Krok 3: w wywołaniu, w nawiasach, wpiszcie konkretną liczbę.
- [mów] Liczba w wywołaniu nazywa się **argument**. Parametr to dziurka w przepisie, argument to to, co w nią wkładacie.
- Krok 4: uruchomcie. Ściana ma taką długość, jaką podaliście w wywołaniu.
- Krok 5: dopiszcie drugie wywołanie, z inną liczbą, i przesuńcie Agenta między nimi.
- Krok 6: uruchomcie. Dwie ściany różnej długości, z **jednej** funkcji. **Czekaj na kciuki.**
- [mów] To jest dokładnie to, czego brakowało wam tydzień temu.
- Krok 7: dodajcie drugi parametr — materiał — i przenieście do funkcji ustawienie ekwipunku.
- Krok 8: wywołajcie funkcję trzy razy, z trzema różnymi materiałami i długościami.
- [mów] Zwróćcie uwagę na kolejność. Argumenty trafiają do parametrów **po kolei**. Pierwszy do pierwszego, drugi do drugiego.

### Wskazówki
- [podpowiedź] Rozróżnienie parametru od argumentu warto wprowadzić nazwami, ale nie egzekwuj ich sztywno. Ważne jest pojęcie dziurki i tego, co się w nią wkłada.
- [błąd] Dziecko wywołuje funkcję bez podania argumentu. Edytor podkreśli — dobra okazja na przeczytanie komunikatu.
- [błąd] Argumenty podane w odwrotnej kolejności. Program się uruchomi i zrobi coś dziwnego. To jest błąd logiczny, nie składniowy — powiedz to.
- [błąd] Dziecko zostawia w środku funkcji starą liczbę obok parametru. Ściana ma stałą długość mimo parametru.
- [błąd] Parametr nazwany tak samo jak zmienna globalna. Zadziała, ale myli. Zaproponujcie inne nazwy.
- [dla szybszych] Niech dodadzą trzeci parametr — wysokość — i sprawdzą, czy funkcja nadal działa dla wszystkich kombinacji.

### Materiały
- [kod] Funkcja z parametrami | javascript:
  ```
  function zbudujSciane(dlugosc: number, material: number) {
      agent.setItem(material, 512, 1)
      agent.setSlot(1)
      for (let i = 0; i < dlugosc; i++) {
          agent.move(FORWARD, 1)
          agent.place(BACK)
      }
  }

  player.onChat("sciany", function () {
      agent.teleportToPlayer()
      zbudujSciane(10, STONE)
      agent.turn(RIGHT)
      zbudujSciane(5, PLANKS)
  })
  ```

## [guided] Generator domków (18 min)

### Co robić teraz
- [mów] Teraz zbudujemy generator. Jedna funkcja, dziesięć domków.
- Krok 1: na kartce zaplanujcie, jakie parametry ma mieć wasz domek. Zwykle trzy: szerokość, głębokość, wysokość. Czasem czwarty: materiał.
- Krok 2: napiszcie funkcję zbudujDomek z tymi parametrami. W środku wykorzystajcie funkcje z zeszłego tygodnia — podłoga i ściany.
- [mów] Uwaga: te funkcje też potrzebują parametrów. Funkcja bez parametrów nie da się użyć wewnątrz funkcji z parametrami.
- Krok 3: przerobicie najpierw zbudujPodloge, dodając jej dwa parametry. Potem zbudujSciany.
- Krok 4: w funkcji zbudujDomek wywołajcie je, przekazując dalej swoje parametry.
- [mów] Popatrzcie, co się właśnie stało: funkcja przekazuje otrzymane wartości innej funkcji. Tak buduje się prawdziwe programy — z warstw.
- Krok 5: wywołajcie zbudujDomek raz i sprawdźcie, czy działa.
- Krok 6: dopiszcie drugie i trzecie wywołanie z innymi wymiarami. Między nimi przesuńcie Agenta o kilka bloków w bok.
- Krok 7: uruchomcie. Trzy różne domki obok siebie. **Czekaj na kciuki.**
- Krok 8: dopiszcie jeszcze siedem wywołań. Ulica gotowa.

### Wskazówki
- [podpowiedź] Przekazywanie parametrów dalej, do kolejnej funkcji, jest tu największym skokiem pojęciowym. Zrób to bardzo powoli, na jednym przykładzie.
- [błąd] Domki nakładają się na siebie, bo przesunięcie między wywołaniami jest mniejsze niż szerokość domku. Przesunięcie musi zależeć od parametru.
- [błąd] Drugi domek jest przekrzywiony, bo funkcja nie zostawia Agenta w przewidywalnym stanie. Reguła z zeszłego tygodnia: zapisz w komentarzu, gdzie funkcja zostawia Agenta.
- [błąd] Dziecko zapomina przekazać parametr dalej i wewnętrzna funkcja używa starej, stałej wartości.
- [błąd] Program robi się długi, bo dziecko wpisuje dziesięć wywołań ręcznie. To jest w porządku dziś — ale zapytaj, czy nie da się tego zrobić pętlą. To zapowiedź lekcji 13.
- [dla szybszych] Niech wywołają funkcję w pętli, przekazując licznik jako jeden z parametrów. Wtedy każdy domek jest o jeden większy.
- [gdy nie zdążysz] Trzy domki zamiast dziesięciu.

### Materiały
- [kod] Generator domków | javascript:
  ```
  function zbudujPodloge(dlugosc: number, szerokosc: number) {
      for (let rzad = 0; rzad < szerokosc; rzad++) {
          for (let kolumna = 0; kolumna < dlugosc; kolumna++) {
              agent.move(FORWARD, 1)
              agent.place(DOWN)
          }
          agent.turn(RIGHT)
          agent.move(FORWARD, 1)
          agent.turn(RIGHT)
      }
  }

  function zbudujDomek(dlugosc: number, szerokosc: number, material: number) {
      agent.teleportToPlayer()
      agent.setItem(material, 512, 1)
      agent.setSlot(1)
      zbudujPodloge(dlugosc, szerokosc)
  }

  player.onChat("ulica", function () {
      zbudujDomek(6, 5, PLANKS)
      zbudujDomek(8, 4, STONE)
      zbudujDomek(5, 5, BRICKS)
  })
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały edytora.
- Zapowiedź: „Po przerwie wasze funkcje zaczną zwracać wyniki i dowiecie się, kiedy parametr jest lepszy od zmiennej globalnej.”

### Wskazówki
- [tempo] Przekazywanie parametrów przez warstwy funkcji jest wyczerpujące. Przerwa jest tu obowiązkowa.

## [concept] Parametr kontra zmienna globalna (13 min)

### Co robić teraz
- [mów] Macie teraz dwa sposoby, żeby przekazać funkcji liczbę: zmienna na górze programu albo parametr. Kiedy który?
- Krok 1: eksperyment. Wywołajcie swoją funkcję dwa razy z różnymi parametrami. Działa — dwa różne domki.
- Krok 2: teraz spróbujcie zrobić to samo zmienną globalną: ustawcie ją przed pierwszym wywołaniem, potem zmieńcie i wywołajcie znowu.
- Krok 3: to też działa, ale zauważcie, ile linijek trzeba napisać i jak łatwo o pomyłkę.
- [mów] Zasada praktyczna: **jeśli wartość jest inna przy każdym wywołaniu — parametr. Jeśli jest taka sama dla całego programu — zmienna globalna.**
- Krok 4: przykłady do rozstrzygnięcia. Podaję, wy piszecie na czat, co lepsze.
- Rozmiar domku. Materiał podłogi we wszystkich budynkach. Nazwa świata. Wysokość konkretnej wieży.
- [mów] I jeszcze jedna zaleta parametru: patrząc na wywołanie, od razu widzicie, z czym funkcja działa. Przy zmiennej globalnej musicie szukać po całym programie.
- Krok 5: przejrzyjcie swój program. Znajdźcie zmienną globalną, która powinna być parametrem, i przerobcie ją.

### Wskazówki
- [podpowiedź] Reguła „inne przy każdym wywołaniu to parametr” jest prosta i praktyczna. Nie wchodź w niuanse zasięgu zmiennych.
- [błąd] Dziecko przerabia wszystkie zmienne globalne na parametry i funkcja ma osiem parametrów. To druga skrajność — nazwij ją i zaproponuj granicę trzech-czterech.
- [błąd] Zmienna globalna zmieniona wewnątrz funkcji psuje kolejne wywołania. To realny problem i dobry argument za parametrami.
- [błąd] Dziecko nie rozumie, dlaczego parametr znika po zakończeniu funkcji. Powiedz krótko, że parametr żyje tylko w środku funkcji — bez wchodzenia w teorię.
- [dla szybszych] Niech sprawdzą, co się stanie, gdy nazwa parametru będzie taka sama jak nazwa zmiennej globalnej. Który wygra?

## [guided] Funkcja, która coś zwraca (14 min)

### Co robić teraz
- [mów] Do tej pory wasze funkcje coś **robiły**. Teraz zrobimy taką, która coś **oblicza i oddaje**.
- Krok 1: napiszcie funkcję policzBloki z dwoma parametrami: dlugosc i szerokosc.
- Krok 2: w środku, zamiast budować, napiszcie słowo return i wyrażenie: dlugosc razy szerokosc.
- [mów] Słowo return znaczy: oddaj tę wartość temu, kto mnie wywołał. Funkcja kończy się w tym momencie.
- Krok 3: wywołajcie ją i wyświetlcie wynik na czacie.
- Krok 4: uruchomcie. Gra wypisuje liczbę bloków. **Czekaj na kciuki.**
- [mów] Zauważcie różnicę: tej funkcji nie wywołujecie samodzielnie w linijce. Ona **jest** wartością, więc wstawiacie ją tam, gdzie potrzebna jest liczba.
- Krok 5: użyjcie jej wewnątrz swojej funkcji budującej — do sprawdzenia, czy Agent ma dość materiału.
- Krok 6: napiszcie drugą funkcję zwracającą wartość: policzSciany, biorącą trzy parametry i zwracającą obwód razy wysokość.
- Krok 7: wyświetlcie na czacie łączną liczbę bloków całego domku, sumując wyniki obu funkcji.
- [mów] Wasz program teraz nie tylko buduje. On potrafi też policzyć, co zbuduje, zanim to zrobi.

### Wskazówki
- [podpowiedź] Rozróżnienie funkcji, która robi, od funkcji, która zwraca, jest ważne i dzieci łapią je szybciej, niż się wydaje — pod warunkiem że pokażesz obie obok siebie.
- [błąd] Dziecko pisze return, ale nie używa zwróconej wartości. Wynik przepada bez śladu i bez błędu.
- [błąd] Dziecko pisze funkcję zwracającą i jednocześnie budującą. Zadziała, ale to zły nawyk. Zaproponuj podział.
- [błąd] Kod po słowie return nigdy się nie wykonuje. To zaskoczenie — pokaż na żywo.
- [błąd] Funkcja zwracająca wywołana jak zwykła, w osobnej linijce. Nic się nie dzieje.
- [dla szybszych] Niech napiszą funkcję zwracającą wartość, która korzysta z innej funkcji zwracającej.
- [gdy nie zdążysz] Jedna funkcja zwracająca, bez łączenia wyników.

### Materiały
- [kod] Funkcja zwracająca wartość | javascript:
  ```
  function policzBloki(dlugosc: number, szerokosc: number) {
      return dlugosc * szerokosc
  }

  function policzSciany(dlugosc: number, szerokosc: number, wysokosc: number) {
      return (2 * dlugosc + 2 * szerokosc) * wysokosc
  }

  player.onChat("licz", function () {
      let podloga = policzBloki(10, 8)
      let sciany = policzSciany(10, 8, 3)
      player.say("Podloga: " + podloga + ", sciany: " + sciany + ", razem: " + (podloga + sciany))
  })
  ```

## [challenge] Twój generator (8 min)

### Co robić teraz
- Zadanie samodzielne: napisz generator czegoś własnego.
- Wymagania: jedna funkcja z co najmniej trzema parametrami, wywołana co najmniej cztery razy z różnymi wartościami.
- Propozycje: generator wież, generator mostów, generator ogrodzeń, generator kolumn.
- Dodatkowo: napisz funkcję zwracającą, która policzy, ile bloków zużyje cała budowa.
- Kto skończy, wkleja na czat **same wywołania**, bez definicji funkcji.

### Wskazówki
- [tempo] Wklejanie samych wywołań pokazuje, jak czytelny jest kod. Jeśli z wywołań nie widać, co powstanie, nazwy są złe.
- [dla szybszych] Niech wywołają funkcję w pętli, przekazując licznik jako parametr, i zbudują rząd rosnących obiektów.
- [gdy nie zdążysz] Dwa parametry i trzy wywołania.
- [błąd] Wszystkie wywołania mają te same wartości. Wtedy parametry są niepotrzebne — zapytaj, po co je dodało.

## [challenge] Pokaz i wyjście ze świata (5 min)

### Co robić teraz
- Przypomnij o wyjściu przez menu.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje swoje wywołania, mówi, co powstanie, i uruchamia program.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Przewidywanie efektu z samych wywołań przed uruchomieniem jest tu najlepszym sprawdzeniem czytelności.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: czym różni się parametr od argumentu? Kiedy lepszy jest parametr, a kiedy zmienna globalna? Co robi słowo return?
- [mów] Dzisiaj wasze funkcje przestały być sztywne. Jeden przepis, dowolnie wiele różnych efektów.
- [mów] To jest ostatni duży element, którego wam brakowało. Od teraz macie wszystko, czego trzeba, żeby pisać naprawdę duże programy.
- Zajawka: „Na następnych zajęciach — ostatnich z tej części kursu — poznacie współrzędne. Wasze budowle przestaną zależeć od tego, gdzie akurat stoicie.”
- Przypomnij zadanie domowe: funkcja z trzema parametrami, pięć wywołań.

### Wskazówki
- [błąd] Wywołanie bez argumentu — edytor podkreśli.
- [błąd] Argumenty w odwrotnej kolejności — błąd logiczny, bez podkreślenia.
- [błąd] Stara liczba zostawiona w środku funkcji obok parametru.
- [błąd] Parametr nieprzekazany dalej do funkcji wewnętrznej.
- [błąd] Wynik funkcji zwracającej nigdzie nieużyty — przepada bez śladu.
