# Współrzędne bezwzględne i względne — budowla niezależna od gracza
Subject: Minecraft Education
Level: Poziom 2 — 10-12 lat, kod
Czas: 95 min
Tags: Współrzędne, Pozycje, Wypełnianie, Podsumowanie miesiąca
Opis: Ostatnia lekcja dwóch pierwszych miesięcy. Budowle przestają zależeć od tego, gdzie stoi gracz, a wypełnianie obszaru zastępuje mozolne stawianie blok po bloku.
Cel: Dziecko rozróżnia współrzędne bezwzględne od względnych, buduje w zadanym miejscu niezależnie od pozycji gracza i używa wypełniania obszaru zamiast pętli po pojedynczych blokach.

### Po zajęciach dziecko potrafi
- odróżnić pozycję bezwzględną od względnej i powiedzieć, kiedy której użyć
- zbudować obiekt w konkretnym, zadanym miejscu świata
- wypełnić prostopadłościenny obszar jednym poleceniem
- opowiedzieć własnymi słowami, czego nauczył się przez dwa miesiące

### Przygotuj przed zajęciami
- gotowe demo: ten sam program uruchomiony z trzech różnych miejsc, budujący zawsze w tym samym punkcie
- **sprawdzone w Twojej wersji edytora**, jak dokładnie zapisuje się pozycje i wypełnianie obszaru
- światy dzieci z lekcji 7
- lista tego, co dzieci umieją po ośmiu tygodniach — do odczytania na koniec

### Zadanie domowe
- zbuduj coś jednym poleceniem wypełniania i zrób zrzut ekranu, żeby pokazać na następnych zajęciach

## [intro] Powitanie i demo niezależności (8 min)

### Co robić teraz
- [mów] Cześć. Ostatnia lekcja tej części kursu i rozwiążemy dziś problem, który towarzyszy wam od pierwszego dnia.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- [mów] Problem brzmi tak: wasz program buduje tam, gdzie akurat stoicie i w którą stronę patrzycie. Wystarczy, że się obrócicie, i wszystko wychodzi gdzie indziej.
- **DEMO:** uruchom ten sam program, stojąc w trzech zupełnie różnych miejscach i patrząc w trzy różne strony.
- Budowla za każdym razem powstaje w **tym samym punkcie**, tak samo obrócona.
- [mów] Dziś nauczycie się mówić „zbuduj **tutaj**”, a nie „zbuduj przede mną”.
- [mów] I drugie: pokażę wam polecenie, które zastąpi całą waszą potrójną pętlę z lekcji piątej. Jedną linijką.

### Wskazówki
- [tempo] Problem zależności od gracza jest dzieciom doskonale znany z siedmiu tygodni pracy. Nazwanie go wprost daje natychmiastowe zrozumienie, po co ta lekcja.
- [podpowiedź] Nie zdradzaj jeszcze polecenia wypełniania. Obietnica „jedna linijka zamiast potrójnej pętli” trzyma uwagę przez pierwszą połowę.

## [review] Wracamy do funkcji z parametrami (5 min)

### Co robić teraz
- Wszyscy otwierają świat z lekcji 7 i Code Builder w widoku JavaScript. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie swój generator.
- Szybka powtórka: czym różni się parametr od argumentu? Co robi słowo return? Kiedy lepsza jest zmienna globalna?
- Zróbcie eksperyment: uruchomcie swój generator dwa razy, stojąc za każdym razem inaczej obróceni.
- [mów] Widzicie? To jest dokładnie ten problem. I dziś go rozwiążemy.

### Wskazówki
- [podpowiedź] Ten eksperyment na starcie jest lepszy niż jakiekolwiek wyjaśnienie. Każde dziecko zobaczy problem we własnym programie.
- [gdy nie zdążysz] Sam eksperyment z obrotem.

## [concept] Dwa rodzaje pozycji (14 min)

### Co robić teraz
- [mów] W Minecrafcie są dwa sposoby na podanie miejsca i różnią się fundamentalnie.
- **Pozycja bezwzględna** — konkretny punkt świata, opisany trzema liczbami. Jak adres domu. Zawsze ten sam, niezależnie od tego, gdzie stoicie.
- **Pozycja względna** — przesunięcie od miejsca, w którym jesteście. Jak „trzy kroki w prawo od ciebie”. Znaczy co innego zależnie od tego, gdzie stoicie.
- Krok 1: sprawdźcie swoje współrzędne w grze i zapiszcie je na kartce.
- Krok 2: napiszcie program, który przenosi Agenta na pozycję bezwzględną — wpiszcie tam swoje trzy liczby.
- Krok 3: uruchomcie. Agent pojawia się w tym miejscu.
- Krok 4: odlećcie sto bloków dalej i uruchomcie ponownie.
- Krok 5: Agent pojawia się **w tym samym miejscu co przedtem**, nie przy was. **Czekaj na kciuki.**
- [mów] To jest siła pozycji bezwzględnej. Powiedzieliście „tam” i Agent idzie tam, gdziekolwiek jesteście.
- Krok 6: teraz pozycja względna. Napiszcie przeniesienie o trzy bloki przed siebie i uruchomcie z dwóch różnych miejsc.
- Krok 7: za każdym razem Agent ląduje gdzie indziej — trzy bloki od was.
- [mów] Żadna z nich nie jest lepsza. Bezwzględna jest do budowania w konkretnym miejscu. Względna do rzeczy, które mają być obok gracza.
- Krok 8: ćwiczenie na czat. Podaję sytuację, wy piszecie, która pozycja pasuje.
- Budowa ratusza w centrum wsi. Postawienie pochodni przy graczu. Wygenerowanie mostu między dwoma zapisanymi punktami. Zbudowanie ściany przed graczem.

### Wskazówki
- [podpowiedź] Nazwy poleceń dla obu rodzajów pozycji różnią się między wersjami edytora. Sprawdź je przed zajęciami i podawaj to, co dzieci widzą u siebie — albo pokaż przez przełącznik bloczków, jak na lekcji 1.
- [błąd] Dziecko wpisuje współrzędne z pamięci i myli się o kilkadziesiąt bloków. Agent ląduje daleko. To nie awaria — niech sprawdzi liczby w grze.
- [błąd] Dziecko myli Y z Z. Y to zawsze wysokość.
- [błąd] Agent ląduje pod ziemią, bo Y jest za małe. Trzeba sprawdzić wysokość terenu.
- [błąd] Dziecko wpisuje współrzędne kolegi ze swojego świata i dziwi się, że tam nic nie ma. Współrzędne są lokalne dla świata.
- [dla szybszych] Niech napiszą program, który zapamiętuje pozycję gracza do zmiennych i buduje względem zapamiętanego punktu, a nie bieżącego.

### Materiały
- [kod] Pozycja bezwzględna kontra względna | javascript:
  ```
  let startX = 100
  let startY = 4
  let startZ = -50

  player.onChat("tam", function () {
      agent.teleport(world(startX, startY, startZ), NORTH)
      agent.setItem(STONE, 64, 1)
      agent.setSlot(1)
      agent.move(FORWARD, 5)
  })

  player.onChat("obok", function () {
      agent.teleportToPlayer()
      agent.move(FORWARD, 3)
  })
  ```

## [guided] Budowla w zadanym miejscu (18 min)

### Co robić teraz
- [mów] Teraz przerobimy wasz generator tak, żeby budował w konkretnych miejscach.
- Krok 1: zadeklarujcie trzy zmienne z współrzędnymi punktu, w którym chcecie zacząć budowę.
- Krok 2: dodajcie do funkcji zbudujDomek trzy nowe parametry: x, y, z.
- Krok 3: na początku funkcji, zamiast przeniesienia Agenta do gracza, wstawcie przeniesienie na podane współrzędne, z ustalonym kierunkiem.
- [mów] Kierunek jest równie ważny jak pozycja. Bez niego domek będzie w dobrym miejscu, ale obrócony losowo.
- Krok 4: w wywołaniach podajcie konkretne współrzędne. Pierwszy domek w punkcie startowym, drugi dziesięć bloków dalej, trzeci dwadzieścia.
- Krok 5: uruchomcie. Trzy domki w ustalonych miejscach.
- Krok 6: odlećcie daleko, obróćcie się i uruchomcie ponownie. **Domki powstają w tych samych miejscach.** Czekaj na kciuki.
- [mów] Problem, który towarzyszył wam siedem tygodni, właśnie zniknął.
- Krok 7: teraz ulepszenie. Zamiast wpisywać współrzędne ręcznie w każdym wywołaniu, policzcie je.
- Krok 8: wywołajcie funkcję w pętli, a jako współrzędną x podajcie startX plus licznik razy odstęp.
- Krok 9: uruchomcie z dziesięcioma domkami. Powstaje równa ulica.
- [mów] Popatrzcie na ten kod: pętla, funkcja z parametrami i rachunek na współrzędnych. To są wszystkie rzeczy z tego miesiąca naraz.

### Wskazówki
- [podpowiedź] Moment, w którym dziecko odlatuje daleko i widzi, że budowla powstaje mimo to we właściwym miejscu, jest kulminacją tej lekcji. Zostaw na to czas.
- [błąd] Domki są w dobrych miejscach, ale każdy obrócony inaczej — brakuje ustalenia kierunku przy przenoszeniu.
- [błąd] Domki nakładają się, bo odstęp w rachunku jest mniejszy niż szerokość domku.
- [błąd] Domki budują się pod ziemią albo w powietrzu, bo Y jest stałe, a teren nierówny. Na płaskim świecie to nie przeszkadza — powiedz, że na nierównym terenie to jest realny problem, do rozwiązania w drugim semestrze.
- [błąd] Dziecko wpisuje współrzędne bezwzględne, ale zapomina zmienić przeniesienia Agenta i nadal używa przeniesienia do gracza.
- [dla szybszych] Niech zbudują siatkę domków w dwóch wymiarach: pętla po x w pętli po z.
- [gdy nie zdążysz] Trzy domki z ręcznie wpisanymi współrzędnymi, bez pętli.

### Materiały
- [kod] Ulica na zadanych współrzędnych | javascript:
  ```
  let startX = 100
  let startY = 4
  let startZ = -50
  let odstep = 10

  function zbudujDomek(x: number, y: number, z: number, bok: number) {
      agent.teleport(world(x, y, z), NORTH)
      agent.setItem(PLANKS, 512, 1)
      agent.setSlot(1)
      for (let rzad = 0; rzad < bok; rzad++) {
          for (let kolumna = 0; kolumna < bok; kolumna++) {
              agent.move(FORWARD, 1)
              agent.place(DOWN)
          }
          agent.turn(RIGHT)
          agent.move(FORWARD, 1)
          agent.turn(RIGHT)
      }
  }

  player.onChat("ulica", function () {
      for (let i = 0; i < 10; i++) {
          zbudujDomek(startX + i * odstep, startY, startZ, 6)
      }
  })
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały edytora.
- Zapowiedź: „Po przerwie pokażę wam polecenie, które zastąpi całą waszą potrójną pętlę jedną linijką. I zbudujecie coś naprawdę dużego.”

### Wskazówki
- [tempo] Ta zapowiedź jest najsilniejszą, jaką masz w całym kursie. Dzieci wiedzą dokładnie, ile pracy kosztowała ich potrójna pętla.

## [concept] Wypełnianie obszaru — jedna linijka zamiast pętli (13 min)

### Co robić teraz
- [mów] Pamiętacie potrójną pętlę z lekcji piątej? Trzydzieści linijek, budowanie po jednym bloku, minuta czekania.
- Krok 1: pokaż polecenie wypełniania obszaru. Podaje się rodzaj bloku oraz dwa narożne punkty prostopadłościanu.
- [mów] Ono nie chodzi po bloczkach. Ono wypełnia cały obszar naraz, natychmiast.
- Krok 2: napiszcie polecenie wypełniające obszar dziesięć na osiem na jeden — czyli podłogę.
- Krok 3: uruchomcie. Podłoga pojawia się **od razu**. **Czekaj na kciuki.**
- [mów] Porównajcie z lekcją piątą. Ta sama podłoga, wtedy osiemdziesiąt kroków Agenta, teraz jedna linijka i zero czekania.
- Krok 4: teraz bryła. Zmieńcie drugi punkt tak, żeby obszar miał wysokość trzy.
- Krok 5: uruchomcie. Lita bryła w ułamku sekundy.
- [mów] I teraz sztuczka na pokój. Zbudujcie litą bryłę, a potem **wypełnijcie jej środek powietrzem**.
- Krok 6: napiszcie drugie polecenie wypełniania, o punktach przesuniętych o jeden do środka, z blokiem powietrza.
- Krok 7: uruchomcie. Pokój z pustym środkiem, dwiema linijkami. **Czekaj na kciuki.**
- [mów] Zapamiętajcie tę technikę: **zbuduj pełne, potem wydrąż**. To jest najszybszy sposób na pomieszczenie i będziecie go używać do końca kursu.
- Krok 8: pytanie do grupy: skoro to jest tak wygodne, po co uczyliśmy się Agenta i pętli?
- Zbierz odpowiedzi. Wypełnianie robi tylko prostopadłościany. Agent zrobi wszystko, tylko wolniej.

### Wskazówki
- [podpowiedź] Pytanie „po co więc uczyliśmy się Agenta” jest ważne i uczciwe. Odpowiedź o granicach obu narzędzi jest lepszą lekcją niż samo polecenie.
- [podpowiedź] Technika „zbuduj pełne, potem wydrąż” to jedna z najbardziej użytecznych rzeczy w całym kursie. Nazwij ją i wracaj do niej.
- [błąd] Dokładny zapis polecenia wypełniania różni się między wersjami edytora. Skorzystaj z metody z lekcji 1: ułóż z bloczków, przełącz na tekst, odczytaj.
- [błąd] Obszar wypełniony w złym miejscu, bo dziecko pomyliło kolejność współrzędnych w punktach.
- [błąd] Wydrążenie usuwa też ściany, bo punkty nie zostały przesunięte o jeden do środka.
- [błąd] Bardzo duży obszar powoduje zacinanie się gry. Wypełnianie ma limit rozmiaru — ostrzeż, zanim ktoś spróbuje wypełnić kilometr.
- [dla szybszych] Niech zbudują pokój z oknami, wydrążając dodatkowe małe obszary w ścianach.
- [gdy nie zdążysz] Samo wypełnianie podłogi i bryły. Wydrążanie zostaw na zadanie domowe.

### Materiały
- [kod] Pokój przez wypełnienie i wydrążenie | javascript:
  ```
  let x = 100
  let y = 4
  let z = -50

  player.onChat("pokoj", function () {
      blocks.fill(STONE, world(x, y, z), world(x + 10, y + 4, z + 8), FillOperation.Replace)
      blocks.fill(AIR, world(x + 1, y + 1, z + 1), world(x + 9, y + 3, z + 7), FillOperation.Replace)
  })
  ```

## [guided] Duży projekt z obu narzędzi (14 min)

### Co robić teraz
- [mów] Teraz zbudujecie coś dużego, używając obu narzędzi tam, gdzie każde jest lepsze.
- Krok 1: zaplanujcie na kartce budynek: bryła główna, wejście, dach, ozdoby.
- Krok 2: **bryłę i wnętrze** zrobcie wypełnianiem — to są prostopadłościany.
- Krok 3: **ozdoby i nieregularne elementy** zrobcie Agentem — schody, blanki, ścieżkę dookoła.
- Krok 4: podzielcie to na funkcje z parametrami, tak jak na zeszłej lekcji.
- Krok 5: w programie głównym wywołajcie je po kolei. Komenda ma czytać się jak spis treści.
- Krok 6: uruchomcie z dwóch różnych miejsc świata i sprawdźcie, czy budowla powstaje tam, gdzie ma.
- Krok 7: zmieńcie współrzędne startowe i zbudujcie drugi taki sam budynek gdzie indziej.
- [mów] Popatrzcie, co macie: program, który buduje kompletny budynek w dowolnym miejscu świata, sterowany kilkoma liczbami. Osiem tygodni temu pisaliście pierwszą linijkę kodu.

### Wskazówki
- [podpowiedź] Świadomy wybór narzędzia do zadania — wypełnianie do prostopadłościanów, Agent do reszty — jest tu głównym materiałem. Nazwij to wprost.
- [błąd] Dziecko robi wszystko wypełnianiem i nie może zrobić schodów. To jest właśnie granica narzędzia.
- [błąd] Dziecko robi wszystko Agentem i czeka minutę na budowę. Zapytaj, którą część dałoby się wypełnić.
- [błąd] Agent buduje ozdoby w złym miejscu, bo funkcja wypełniania nie przenosi Agenta. Trzeba go ustawić osobno.
- [błąd] Program działa, ale drugi budynek nachodzi na pierwszy. Odstęp musi być większy niż rozmiar budynku.
- [dla szybszych] Niech zbudują całą wieś: pętla wywołująca funkcję budynku z wyliczanymi współrzędnymi.
- [gdy nie zdążysz] Sama bryła z wydrążeniem, bez ozdób Agentem.

## [challenge] Ostatnie szlify (8 min)

### Co robić teraz
- Zadanie samodzielne: dokończ swój budynek i przygotuj go do pokazu.
- Wybierz dwie rzeczy: dach, wejście ze schodami, ścieżka dookoła, oświetlenie, drugi budynek obok.
- Sprawdź, czy program działa uruchomiony z dowolnego miejsca świata.
- Kto skończy, wkleja na czat swoją komendę główną — samą listę wywołań.

### Wskazówki
- [tempo] Test „uruchom z dowolnego miejsca” jest tu wymogiem, nie sugestią. To jest główna zdobycz tej lekcji.
- [dla szybszych] Niech dodadzą funkcję, która przed budową wypisuje na czacie, ile bloków zużyje cała konstrukcja.
- [gdy nie zdążysz] Sam dach.
- [błąd] Dziecko zaczyna dużą przebudowę pięć minut przed pokazem. Reaguj wcześnie.

## [challenge] Wielki pokaz i wyjście ze świata (5 min)

### Co robić teraz
- Przypomnij o wyjściu przez menu.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko udostępnia ekran, pokazuje komendę główną, uruchamia ją **z losowego miejsca świata** i pokazuje efekt.
- Poproś o jedno zdanie: co w tym programie było najtrudniejsze do napisania.
- Brawa po każdym pokazie.

### Wskazówki
- [tempo] Uruchomienie z losowego miejsca jest tu celowe i efektowne — pokazuje, że problem z pierwszych siedmiu tygodni został rozwiązany.
- [podpowiedź] To jest pokaz zamykający dwa miesiące. Warto zaprosić rodziców na ostatnie dziesięć minut albo wysłać im zrzuty ekranu tego samego dnia.

## [summary] Podsumowanie dwóch miesięcy (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: czym różni się pozycja bezwzględna od względnej? Kiedy użyć wypełniania, a kiedy Agenta? Na czym polega technika „zbuduj pełne, potem wydrąż”?
- Odczytaj listę tego, co dzieci umieją: przełączanie bloków i tekstu, składnia, czytanie komunikatów błędów, zmienne i typy, pętla for i licznik, pętle zagnieżdżone, funkcje, parametry, wartości zwracane, współrzędne, wypełnianie obszarów.
- [mów] Osiem tygodni temu pisaliście pierwszą linijkę kodu, patrząc na bloczki. Dziś macie programy z funkcjami, parametrami i rachunkami na współrzędnych.
- [mów] I nauczyliście się czegoś, co przyda się w każdym języku programowania, jaki poznacie w życiu: że kod jest dla ludzi. Komputer zrozumie wszystko, byle poprawnie zapisane. Człowiek zrozumie tylko to, co jest napisane czytelnie.
- Zajawka: „Od następnych zajęć budujecie świat. Generator ulic, losowość, dopasowanie do terenu, a w drugim semestrze całe miasto z jednego programu.”
- Przypomnij zadanie domowe: zbuduj coś wypełnianiem i zrób zrzut ekranu.

### Wskazówki
- [błąd] Pomylona kolejność współrzędnych w punktach obszaru.
- [błąd] Wydrążenie usuwa ściany — punkty nieprzesunięte o jeden do środka.
- [błąd] Budowle obrócone różnie — brak ustalenia kierunku przy przenoszeniu Agenta.
- [błąd] Agent po wypełnianiu stoi tam, gdzie stał — wypełnianie go nie przenosi.
- [błąd] Bardzo duży obszar zacina grę — wypełnianie ma limit rozmiaru.
