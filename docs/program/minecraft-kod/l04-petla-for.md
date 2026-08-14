# Pętla for — skąd się bierze ta dziwna linijka
Subject: Minecraft Education
Level: Poziom 2 — 10-12 lat, kod
Czas: 95 min
Tags: Pętla for, Licznik, Zakres, Mur
Opis: Pętla w wersji tekstowej. Dzieci rozkładają nagłówek pętli for na trzy części i odkrywają licznik, którego w blokach nie było widać.
Cel: Dziecko czyta i pisze pętlę for, rozumie rolę licznika i potrafi użyć go wewnątrz pętli, żeby każdy przebieg był inny.

### Po zajęciach dziecko potrafi
- przeczytać nagłówek pętli for i nazwać jego trzy części
- napisać pętlę o zadanej liczbie powtórzeń
- użyć licznika wewnątrz pętli, żeby zmieniać coś przy każdym przebiegu
- powiedzieć, ile razy wykona się pętla, patrząc na jej nagłówek

### Przygotuj przed zajęciami
- gotowe demo: mur o rosnącej wysokości, zbudowany jedną pętlą z licznikiem
- światy dzieci z lekcji 3
- przygotowane trzy nagłówki pętli do odczytania: ile razy się wykona?
- lista uczestników z zaznaczeniem, kto ma za sobą kurs bloków

### Zadanie domowe
- napisz pętlę, w której każdy kolejny przebieg buduje coś o jeden blok wyżej

## [intro] Powitanie i demo rosnącego muru (8 min)

### Co robić teraz
- [mów] Cześć. Dziś poznajecie pętlę w wersji tekstowej — i przy okazji coś, czego w bloczkach w ogóle nie było widać.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** uruchom program budujący mur, w którym każdy kolejny słupek jest o jeden blok wyższy. Powstają schody.
- [mów] Popatrzcie: każdy przebieg pętli robi coś **innego**. Pierwszy słupek ma jeden blok, drugi dwa, trzeci trzy.
- [mów] W bloczkach tego nie dało się łatwo zrobić, bo pętla powtarzała zawsze dokładnie to samo. W tekście jest to naturalne — i dziś zobaczycie dlaczego.
- Odczytaj kilka zadań domowych: kto ile liczb zamienił na zmienne.

### Wskazówki
- [podpowiedź] Rosnące schody z jednej pętli to najlepsze demo tej lekcji, bo pokazuje coś, czego wersja blokowa nie umiała bez kombinowania.
- [tempo] Dzieci po kursie bloków rozpoznają pętlę natychmiast. Trudność jest w nagłówku, nie w pojęciu.

## [review] Wracamy do zmiennych (5 min)

### Co robić teraz
- Wszyscy otwierają świat z lekcji 3 i Code Builder w widoku JavaScript. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie program z parametryzowaną budowlą i zmieńcie jedną zmienną.
- Szybka powtórka: co znaczy znak równości w kodzie? Kiedy liczba powinna zostać zmienną? Czym różni się liczba od tekstu?
- Wszyscy odlatują na puste miejsce.

### Wskazówki
- [błąd] Program nie działa po tygodniu, bo dziecko zostawiło go w połowie przebudowy. Poświęć minutę na naprawę — to dobra powtórka.
- [gdy nie zdążysz] Sam start i przelot.

## [concept] Nagłówek pętli for, część po części (14 min)

### Co robić teraz
- [mów] Zaczniemy od czegoś, co już znacie. Ułóżcie z bloczków pętlę powtarzającą coś cztery razy i przełączcie na tekst.
- Krok 1: przeczytajcie, co wyszło. To jest linijka z trzema częściami oddzielonymi średnikami.
- [mów] Ta linijka wygląda groźnie, ale ma prostą budowę. Rozłożymy ją na trzy części.
- **Część pierwsza — start.** Tworzymy zmienną licznika i ustawiamy ją na zero. To dzieje się raz, na początku.
- **Część druga — warunek.** Dopóki licznik jest mniejszy od czterech, pętla działa. Sprawdzane przed każdym przebiegiem.
- **Część trzecia — krok.** Po każdym przebiegu licznik rośnie o jeden.
- [mów] Czytajcie to tak: zacznij od zera, kręć się dopóki mniejsze niż cztery, po każdym obrocie dodaj jeden.
- Krok 2: policzcie razem na palcach. Zero, jeden, dwa, trzy — i stop, bo cztery już nie jest mniejsze od czterech. Cztery przebiegi.
- [mów] Zwróćcie uwagę: licznik idzie od zera, nie od jednego. To dlatego warunek jest „mniejszy niż”, a nie „mniejszy lub równy”.
- Krok 3: ćwiczenie na czat. Pokazuję nagłówek, wy piszecie, ile razy pętla się wykona. Pięć rund, w tym jeden podchwytliwy z licznikiem od 1.
- Krok 4: eksperyment. Zmieńcie warunek z czterech na dziesięć i uruchomcie. Potem na sto.

### Wskazówki
- [podpowiedź] Rozłożenie nagłówka na trzy nazwane części — start, warunek, krok — jest sednem tej lekcji. Wypisz je na udostępnionym ekranie i zostaw.
- [podpowiedź] Metoda „przełącz bloczki na tekst” jest tu użyta dokładnie po to, po co ją wprowadziliśmy na lekcji 1. Zwróć na to uwagę dzieci.
- [błąd] Dziecko liczy od jednego i wychodzi mu o jeden przebieg za dużo. To najczęstsze nieporozumienie tej lekcji i wraca przez cały kurs.
- [błąd] Dziecko zmienia warunek na „mniejszy lub równy” i dostaje o jeden przebieg więcej, niż chciało. Pokaż to na żywo, zamiast tłumaczyć.
- [błąd] Średniki w nagłówku zamienione na przecinki. Edytor podkreśli — dobra okazja na przeczytanie komunikatu.
- [dla szybszych] Niech napiszą pętlę liczącą w dół, od dziesięciu do zera, i sprawdzą, co trzeba zmienić w każdej z trzech części.
- [gdy nie zdążysz] Trzy części nagłówka i ćwiczenie z liczeniem przebiegów. Eksperymenty pomiń.

### Materiały
- [kod] Pętla for — nagłówek rozłożony | javascript:
  ```
  player.onChat("mur", function () {
      agent.teleportToPlayer()
      agent.setItem(STONE, 64, 1)
      agent.setSlot(1)
      // start: licznik = 0
      // warunek: dopoki licznik < 20
      // krok: po kazdym obrocie licznik + 1
      for (let licznik = 0; licznik < 20; licznik++) {
          agent.move(FORWARD, 1)
          agent.place(BACK)
      }
  })
  ```

## [guided] Licznik w środku pętli (18 min)

### Co robić teraz
- [mów] I teraz najciekawsze. Licznik nie jest tylko dla pętli. Wy też możecie go użyć.
- Krok 1: w środku swojej pętli wyświetlcie wartość licznika na czacie. Uruchomcie z małą liczbą powtórzeń, na przykład pięcioma.
- Krok 2: przeczytajcie, co wypisała gra. Zero, jeden, dwa, trzy, cztery.
- [mów] Widzicie? Licznik ma inną wartość przy każdym przebiegu. To jest dokładnie ta rzecz, której nie było w bloczkach.
- Krok 3: teraz zbudujcie schody. W pętli: zbudujcie słupek o wysokości równej licznikowi, potem wróćcie na dół i zróbcie krok w bok.
- Krok 4: żeby zbudować słupek o zmiennej wysokości, potrzebna jest **druga pętla w środku pierwszej**, licząca do licznika zewnętrznej.
- Krok 5: napiszcie to razem, linijka po linijce. Zewnętrzna pętla liczy słupki, wewnętrzna buduje jeden słupek.
- Krok 6: pod wewnętrzną pętlą dodajcie zejście na dół o tyle bloków, ile wynosi licznik, i krok w bok.
- Krok 7: uruchomcie z pięcioma słupkami. Powstają schody. **Czekaj na kciuki.**
- [mów] Popatrzcie na strukturę: pętla w pętli, ale wewnętrzna zależy od licznika zewnętrznej. Za każdym obrotem robi coś innego.
- Krok 8: zmieńcie liczbę słupków na dwadzieścia i uruchomcie.

### Wskazówki
- [podpowiedź] Wyświetlenie licznika na czacie przed budowaniem czegokolwiek jest tu kluczowe. Dziecko musi **zobaczyć** zmieniającą się wartość, zanim jej użyje.
- [błąd] Pierwszy słupek ma zero bloków, bo licznik zaczyna od zera. To poprawne działanie — albo zacznijcie licznik od jednego, albo dodajcie jeden w środku. Dobra rozmowa.
- [błąd] Schody rosną, ale Agent nie schodzi na dół i buduje w powietrzu. Zejście musi być o tyle samo bloków, ile wyniosło wznoszenie.
- [błąd] Dziecko używa tej samej nazwy licznika w obu pętlach. Zadziała, ale zaciemnia. Zaproponujcie nazwy slupek oraz blok.
- [błąd] Wewnętrzna pętla ma warunek z liczbą zamiast z licznikiem zewnętrznej — wtedy wszystkie słupki są równe. To jest właśnie ten moment, o który chodzi.
- [dla szybszych] Niech zbudują schody, które rosną, a potem maleją — dwie pętle albo jedna z rachunkiem na liczniku.
- [gdy nie zdążysz] Sam wyświetlony licznik i słupki o stałej wysokości. Rosnące schody zostaw na zadanie domowe.

### Materiały
- [kod] Schody z licznikiem | javascript:
  ```
  let liczbaSlupkow = 8

  player.onChat("schody", function () {
      agent.teleportToPlayer()
      agent.setItem(STONE, 256, 1)
      agent.setSlot(1)
      for (let slupek = 1; slupek <= liczbaSlupkow; slupek++) {
          for (let blok = 0; blok < slupek; blok++) {
              agent.place(DOWN)
              agent.move(UP, 1)
          }
          agent.move(DOWN, slupek)
          agent.move(FORWARD, 1)
      }
  })
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały edytora.
- Zapowiedź: „Po przerwie zrobimy rachunki na liczniku i zbudujecie piramidę.”

### Wskazówki
- [tempo] Pętla z licznikiem użytym w środku to najtrudniejsze pojęcie pierwszego miesiąca. Przerwa jest tu obowiązkowa.

## [concept] Rachunki na liczniku (13 min)

### Co robić teraz
- [mów] Licznik to zwykła zmienna. Można na nim liczyć jak na każdej innej.
- Krok 1: w pętli wyświetlcie na czacie licznik pomnożony przez dwa. Uruchomcie z pięcioma przebiegami.
- Wynik: zero, dwa, cztery, sześć, osiem.
- Krok 2: teraz licznik odjęty od stałej liczby. Wyświetlcie dziesięć minus licznik.
- Wynik: dziesięć, dziewięć, osiem, siedem, sześć. Liczby maleją, choć licznik rośnie.
- [mów] To jest bardzo przydatna sztuczka. Chcecie, żeby coś malało? Odejmijcie licznik od stałej.
- Krok 3: piramida. Każdy kolejny poziom ma być **węższy** od poprzedniego.
- Krok 4: napiszcie pętlę, w której szerokość poziomu to podstawa minus dwa razy licznik.
- Krok 5: w środku zbudujcie kwadratową warstwę o tej szerokości i przesuńcie Agenta o jeden blok w górę i o jeden do środka.
- Krok 6: uruchomcie z podstawą jedenaście. Powstaje piramida schodkowa.
- [mów] Zauważcie, że pisząc dwa razy licznik, zwężacie piramidę z obu stron naraz. Dlatego podstawa powinna być nieparzysta — inaczej szczyt nie wyjdzie równy.

### Wskazówki
- [podpowiedź] Wyświetlanie wyniku rachunku na czacie przed użyciem go w budowli jest tu znowu najlepszą metodą. Dziecko sprawdza matematykę, zanim zobaczy efekt.
- [błąd] Piramida schodzi poniżej zera i Agent zaczyna budować dziwne rzeczy. Trzeba ograniczyć liczbę poziomów do połowy podstawy.
- [błąd] Piramida jest przesunięta, bo Agent wchodzi do środka tylko z jednej strony. Musi przesunąć się po przekątnej.
- [błąd] Dziecko myli mnożenie z dodawaniem i piramida zwęża się za wolno.
- [błąd] Podstawa parzysta i szczyt piramidy wychodzi jako dwa bloki zamiast jednego. To nie jest błąd, tylko konsekwencja — warto omówić.
- [dla szybszych] Niech zbudują piramidę, która zwęża się tylko z jednej strony, i wyjaśnią, jak zmienili rachunek.
- [gdy nie zdążysz] Same rachunki na czacie, bez piramidy.

### Materiały
- [kod] Rachunki na liczniku | javascript:
  ```
  player.onChat("licz", function () {
      for (let i = 0; i < 5; i++) {
          player.say("licznik: " + i + ", podwojony: " + i * 2 + ", malejacy: " + (10 - i))
      }
  })
  ```

## [guided] Mur o zmiennej wysokości (14 min)

### Co robić teraz
- [mów] Połączymy wszystko z dzisiaj: zmienne z zeszłego tygodnia, pętlę i licznik.
- Krok 1: zadeklarujcie trzy zmienne: dlugosc, wysokosc, material.
- Krok 2: napiszcie program budujący mur o zadanej długości i wysokości. Pętla zewnętrzna liczy poziomy, wewnętrzna buduje jeden rząd.
- Krok 3: po każdym poziomie Agent musi wrócić na początek i wejść wyżej. Napiszcie to, używając zmiennej dlugosc do powrotu.
- [mów] Zwróćcie uwagę: powrót o dlugosc bloków. Nie o dziesięć, nie o dwadzieścia. O tyle, ile wynosi zmienna. Dzięki temu działa dla każdej długości.
- Krok 4: uruchomcie i sprawdźcie.
- Krok 5: zmieńcie obie zmienne i uruchomcie ponownie. Mur jest inny, kod ten sam.
- Krok 6: dodajcie wzór: co drugi poziom z innego materiału. Podpowiedź — sprawdźcie resztę z dzielenia licznika przez dwa.
- [mów] To działanie nazywa się modulo i daje resztę z dzielenia. Reszta z dzielenia przez dwa to zawsze zero albo jeden — czyli idealnie na co drugi.

### Wskazówki
- [podpowiedź] Powrót o wartość zmiennej, a nie o wpisaną liczbę, to najważniejsza praktyczna lekcja tego kroku. Bez tego program działa tylko dla jednej długości.
- [podpowiedź] Modulo dla co drugiego elementu jest sztuczką, którą dzieci uwielbiają i używają potem wszędzie. Warto ją wprowadzić, nawet jeśli warunków jeszcze formalnie nie było.
- [błąd] Kolejne poziomy są przesunięte, bo powrót jest o jeden blok za krótki albo za długi.
- [błąd] Mur ma poziomy w powietrzu, bo Agent wchodzi wyżej, ale nie wraca w poziomie.
- [błąd] Po zmianie zmiennej dlugosc mur się rozjeżdża, bo w powrocie została wpisana liczba na sztywno. Dokładnie to, przed czym ostrzegaliśmy.
- [dla szybszych] Niech zbudują mur, w którym wysokość rośnie wzdłuż długości — falę.
- [gdy nie zdążysz] Sam mur o stałej wysokości, bez wzoru z materiałami.

## [challenge] Twoja budowla z pętlą (8 min)

### Co robić teraz
- Zadanie samodzielne: napisz program, w którym pętla robi za każdym obrotem coś **innego**.
- Propozycje: wieża zwężająca się ku górze, schody spiralne, mur z rosnącymi blankami, rząd słupków o losowych wysokościach.
- Wymóg: w środku pętli musi być użyty licznik. Nie tylko jako liczba powtórzeń.
- Przed uruchomieniem napisz na czat, ile bloków według ciebie postawi program.

### Wskazówki
- [tempo] Wymóg użycia licznika w środku pętli jest tu sednem. Bez niego dzieci napiszą to, co umiały już z bloczków.
- [dla szybszych] Niech policzą, ile bloków ma piramida o podstawie jedenaście, wzorem, a nie przez zliczenie.
- [gdy nie zdążysz] Wieża o rosnącej szerokości wystarczy.
- [błąd] Licznik użyty w środku, ale program i tak buduje wszystko jednakowo, bo dziecko użyło go w miejscu, które nic nie zmienia.

## [challenge] Pokaz i wyjście ze świata (5 min)

### Co robić teraz
- Przypomnij o wyjściu przez menu.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje kod i uruchamia program.
- Poproś, żeby przeczytało na głos nagłówek swojej pętli i powiedziało, ile razy się wykona.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Czytanie nagłówka pętli na głos jest najlepszym sprawdzeniem, czy dziecko rozumie trzy części. Rób to na każdym pokazie przez najbliższy miesiąc.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: jakie są trzy części nagłówka pętli? Ile razy wykona się pętla z warunkiem mniejszy niż siedem, licząca od zera? Po co używać licznika w środku pętli?
- [mów] Dzisiaj zobaczyliście coś, czego bloczki przed wami ukrywały: pętla ma licznik i ten licznik jest zwykłą zmienną, której możecie użyć.
- [mów] To dlatego wersja tekstowa jest mocniejsza. Nie dlatego, że jest trudniejsza, tylko dlatego, że pokazuje więcej.
- Zajawka: „Na następnych zajęciach zagnieździmy pętle na poważnie i zbudujecie pomieszczenie o dowolnych wymiarach — jednym programem, sterowanym trzema liczbami.”
- Przypomnij zadanie domowe.

### Wskazówki
- [błąd] Liczenie od jednego zamiast od zera — o jeden przebieg za dużo lub za mało.
- [błąd] Warunek z „mniejszy lub równy” zamiast „mniejszy niż”.
- [błąd] Średniki w nagłówku zamienione na przecinki.
- [błąd] Wewnętrzna pętla z liczbą zamiast z licznikiem zewnętrznej — wszystkie elementy wychodzą jednakowe.
- [błąd] Powrót Agenta o wpisaną liczbę zamiast o wartość zmiennej — program działa tylko dla jednego rozmiaru.
