# Zmienne i typy — jedna liczba steruje budowlą
Subject: Minecraft Education
Level: Poziom 2 — 10-12 lat, kod
Czas: 95 min
Tags: Zmienne, Typy, Parametryzacja, Nazewnictwo
Opis: Dzieci wprowadzają zmienne i odkrywają, że zmiana jednej liczby na górze programu przebudowuje całą konstrukcję.
Cel: Dziecko deklaruje zmienne, używa ich zamiast wpisanych na sztywno liczb i rozumie różnicę między liczbą, tekstem a rodzajem bloku.

### Po zajęciach dziecko potrafi
- utworzyć zmienną i nadać jej sensowną nazwę
- zastąpić powtarzającą się liczbę w programie zmienną
- odróżnić liczbę od tekstu i powiedzieć, po co są cudzysłowy
- wykonać prosty rachunek na zmiennej w kodzie

### Przygotuj przed zajęciami
- gotowe demo: program budujący budynek, w którym zmiana jednej liczby zmienia całą konstrukcję
- światy dzieci z lekcji 2
- przygotowany zły przykład: program z liczbą 10 wpisaną w siedmiu miejscach
- lista złych i dobrych nazw zmiennych do pokazania na ekranie

### Zadanie domowe
- weź dowolny swój program i zamień w nim wszystkie powtarzające się liczby na zmienne

## [intro] Powitanie i demo jednej liczby (8 min)

### Co robić teraz
- [mów] Cześć. Dziś nauczycie się czegoś, co zmieni sposób, w jaki piszecie programy.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** uruchom program budujący budynek. Potem zmień **jedną** liczbę w pierwszej linijce i uruchom ponownie.
- Budynek jest dwa razy większy. Zrób to jeszcze raz, z inną liczbą.
- [mów] Zmieniłem jeden znak. Nie dotknąłem reszty programu.
- **Zły przykład:** pokaż drugi program, w którym ta sama liczba 10 jest wpisana w siedmiu miejscach.
- [mów] Żeby zmienić rozmiar w tym programie, muszę znaleźć siedem miejsc i poprawić każde. A jak przeoczę jedno, budowla wyjdzie krzywa i będę szukał, dlaczego.
- [mów] Dziś nauczycie się, jak nigdy więcej nie mieć takiego programu.

### Wskazówki
- [tempo] Porównanie dwóch wersji tego samego programu jest tu całym uzasadnieniem zmiennych. Pokaż oba, zanim padnie słowo „zmienna”.
- [podpowiedź] Przeoczenie jednego z siedmiu miejsc to realny scenariusz, który dzieci znają z lekcji o pętlach. Odwołaj się do niego.

## [review] Wracamy do kodu (5 min)

### Co robić teraz
- Wszyscy otwierają świat z lekcji 2 i Code Builder w widoku JavaScript. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie program z ramką.
- Szybka powtórka: czym różni się błąd składniowy od logicznego? Do czego służą klamry? Jak sprawdzić, gdzie skończył Agent?
- Kto zrobił zadanie domowe, wpisuje na czat, jaki błąd sam sobie zepsuł i jak szybko go znalazł.

### Wskazówki
- [podpowiedź] Zadanie domowe z psuciem własnego programu bywa najlepiej wykonywanym zadaniem w kursie. Warto o nie zapytać.
- [gdy nie zdążysz] Sam start.

## [concept] Zmienna, czyli nazwane pudełko (14 min)

### Co robić teraz
- [mów] Zmienna to nazwane miejsce, w którym program trzyma wartość. Nazwane — czyli możecie się do niej odwołać po imieniu.
- Pokaż deklarację: słowo let, nazwa, znak równości, wartość.
- [mów] Znak równości nie znaczy tu „równa się” jak w matematyce. Znaczy „włóż do pudełka”. Czytajcie to jako: niech bok wynosi pięć.
- Krok 1: na początku swojego programu, **przed** komendą na czacie, napiszcie deklarację zmiennej o nazwie bok, z wartością 5.
- Krok 2: w programie zamieńcie liczbę 5 w poleceniach ruchu na nazwę zmiennej.
- Krok 3: uruchomcie. Ramka wygląda tak samo jak przedtem.
- [mów] Nic się nie zmieniło i o to chodzi. Ten sam efekt, inny zapis.
- Krok 4: teraz zmieńcie 5 na 12 w deklaracji. Uruchomcie.
- Krok 5: ramka jest dużo większa. Zmieniliście **jedną** liczbę. **Czekaj na kciuki.**
- [mów] To jest cała moc zmiennej. Liczba mieszka w jednym miejscu, a używana jest w wielu.
- Krok 6: teraz o nazwach. Pokaż listę na ekranie i omów.
- Złe nazwy: a, x1, zmienna, liczba, cos.
- Dobre nazwy: bok, wysokosc, liczbaPieter, material.
- [mów] Nazwa zmiennej to wiadomość do człowieka, który będzie czytał wasz kod. Bardzo często tym człowiekiem będziecie wy sami, za miesiąc.
- [mów] I uwaga techniczna: nazwy piszemy bez polskich znaków. To jest identyfikator, nie tekst.

### Wskazówki
- [podpowiedź] Wyjaśnienie znaku równości jako „włóż do pudełka” zapobiega najczęstszemu nieporozumieniu z matematyki. Powiedz to wprost.
- [błąd] Dziecko deklaruje zmienną w środku komendy i nie może jej użyć gdzie indziej. Na tym etapie prościej jest deklarować wszystko na górze.
- [błąd] Dziecko wpisuje nazwę zmiennej w cudzysłowie. Wtedy program dostaje tekst zamiast liczby. Świetny moment na wprowadzenie tematu typów.
- [błąd] Nazwa zmiennej z polskimi znakami. Edytor to zniesie, ale to zły nawyk i warto go wyłapać teraz.
- [błąd] Zmienna zadeklarowana dwa razy tym samym słowem let. Edytor podkreśli — dobra okazja do przeczytania komunikatu.
- [dla szybszych] Niech sprawdzą, co się stanie, gdy zadeklarują zmienną, ale nigdy jej nie użyją.

### Materiały
- [kod] Ramka sterowana zmienną | javascript:
  ```
  let bok = 12

  player.onChat("ramka", function () {
      agent.teleportToPlayer()
      agent.setItem(STONE, 64, 1)
      agent.setSlot(1)
      agent.move(FORWARD, bok)
      agent.turn(RIGHT)
      agent.move(FORWARD, bok)
      agent.turn(RIGHT)
      agent.move(FORWARD, bok)
      agent.turn(RIGHT)
      agent.move(FORWARD, bok)
      agent.turn(RIGHT)
  })
  ```

## [guided] Kilka zmiennych i rachunki (18 min)

### Co robić teraz
- [mów] Jedna zmienna to dobry początek. Prawdziwa siła zaczyna się przy kilku.
- Krok 1: zadeklarujcie trzy zmienne: dlugosc równa 10, szerokosc równa 6, wysokosc równa 3.
- Krok 2: napiszcie program budujący prostokątną ramkę, używając dlugosc i szerokosc zamiast liczb.
- Krok 3: uruchomcie i sprawdźcie proporcje z góry. **Czekaj na kciuki.**
- Krok 4: zmieńcie obie liczby w deklaracjach i uruchomcie ponownie. Budowla zmienia kształt, program zostaje ten sam.
- Krok 5: teraz rachunki. Zadeklarujcie zmienną obwod i przypiszcie jej wynik działania: dwa razy dlugosc plus dwa razy szerokosc.
- Krok 6: wyświetlcie tę wartość graczowi. Skorzystajcie z polecenia wypisującego wiadomość na czacie.
- Krok 7: uruchomcie. Gra pokazuje obwód waszej budowli, policzony przez program.
- [mów] Zauważcie coś ważnego: nie policzyliście tego sami. Program policzył za was i policzy poprawnie także wtedy, gdy zmienicie wymiary.
- Krok 8: dodajcie zmienną liczbaBlokow równą dlugosc razy szerokosc i też ją wyświetlcie.
- [mów] To jest liczba bloków w podłodze o tych wymiarach. Wasz program teraz nie tylko buduje, ale i liczy.

### Wskazówki
- [podpowiedź] Wyświetlanie wyliczonej wartości na czacie jest pierwszym narzędziem diagnostycznym tego kursu. Wraca za każdym razem, gdy trzeba sprawdzić, co program naprawdę policzył.
- [błąd] Rachunek daje zły wynik, bo dziecko pomyliło kolejność działań. Nawiasy rozwiązują sprawę — pokaż je.
- [błąd] Zmienna obwod policzona **przed** przypisaniem wartości do dlugosc. Kolejność linijek ma znaczenie: najpierw wartość, potem rachunek.
- [błąd] Dziecko oczekuje, że obwod przeliczy się sam po zmianie dlugosc. Nie przeliczy — rachunek wykonuje się raz, w chwili gdy program do niego dojdzie. To ważne i trudne pojęcie, powiedz je wprost.
- [błąd] Wiadomość na czacie nie pojawia się, bo dziecko wpisało zmienną w cudzysłowie. Wtedy gra wyświetla nazwę zamiast wartości.
- [dla szybszych] Niech policzą liczbę bloków całego pomieszczenia: podłoga plus sufit plus cztery ściany, jednym wyrażeniem.
- [gdy nie zdążysz] Dwie zmienne i jeden rachunek. Wyświetlanie zostaw na lekcję 4.

### Materiały
- [kod] Zmienne i rachunki | javascript:
  ```
  let dlugosc = 10
  let szerokosc = 6
  let obwod = 0
  let liczbaBlokow = 0

  player.onChat("licz", function () {
      obwod = 2 * dlugosc + 2 * szerokosc
      liczbaBlokow = dlugosc * szerokosc
      player.say("Obwod: " + obwod)
      player.say("Blokow w podlodze: " + liczbaBlokow)
  })
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały edytora.
- Zapowiedź: „Po przerwie dowiecie się, dlaczego nie da się dodać liczby do słowa i co to jest typ.”

### Wskazówki
- [tempo] Zmienne to duży skok pojęciowy. Przerwa w połowie tej lekcji jest ważniejsza niż zwykle.

## [concept] Typy — liczba, tekst, blok (13 min)

### Co robić teraz
- [mów] Zauważyliście, że niektóre rzeczy piszemy w cudzysłowie, a inne nie. To nie jest przypadek.
- [mów] Każda wartość w programie ma swój **typ**, czyli rodzaj. Poznacie dziś trzy.
- **Liczba** — piszemy ją wprost, bez cudzysłowów. Da się na niej liczyć.
- **Tekst** — zawsze w cudzysłowach. Da się go wyświetlić i sklejać z innym tekstem, ale nie da się go pomnożyć.
- **Rodzaj bloku** — to nie jest ani liczba, ani tekst. To jest osobna wartość ze stałej listy, na przykład kamień albo deski.
- Krok 1: eksperyment. Zadeklarujcie zmienną z liczbą 5 i drugą z tekstem „5” w cudzysłowie.
- Krok 2: spróbujcie pomnożyć obie przez dwa i wyświetlić wynik. Zobaczcie, co się dzieje.
- [mów] Liczba pomnożona przez dwa daje dziesięć. Tekst zachowuje się dziwnie, bo mnożenie napisu nie ma sensu.
- Krok 3: teraz sklejanie. Wyświetlcie tekst „Bok ma ” połączony ze zmienną bok. Znak plus łączy tekst z liczbą.
- [mów] Ten sam znak plus robi dwie różne rzeczy w zależności od typu. Przy liczbach dodaje, przy tekstach skleja.
- Krok 4: zadeklarujcie zmienną przechowującą rodzaj bloku i użyjcie jej w poleceniu ładowania ekwipunku.
- Krok 5: zmieńcie materiał w deklaracji i uruchomcie. Cała budowla jest z innego materiału.
- [mów] To jest bardzo praktyczne. Materiał budowli w jednym miejscu, na górze programu.

### Wskazówki
- [podpowiedź] Eksperyment z mnożeniem tekstu jest lepszy niż jakiekolwiek wyjaśnienie. Pozwól dzieciom zobaczyć dziwny wynik.
- [błąd] Dziecko wpisuje nazwę bloku w cudzysłowie. Nie zadziała — rodzaj bloku wybiera się z listy, nie pisze jako tekst. Podpowiadanie edytora pokaże właściwe nazwy.
- [błąd] Sklejanie tekstu z liczbą wygląda dziwnie, bo brakuje spacji w napisie. Spacja musi być w cudzysłowie.
- [błąd] Dziecko dodaje dwie liczby, ale wychodzi sklejony napis, bo jedna z nich jest tekstem.
- [podpowiedź] Nazwy bloków najprościej znaleźć przez bloczki: ustaw w widoku bloków, przełącz na tekst, odczytaj nazwę. To ta sama metoda co na lekcji 1.
- [dla szybszych] Niech sprawdzą, co się stanie przy dodawaniu liczby do tekstu w odwrotnej kolejności, i wyjaśnią różnicę.
- [gdy nie zdążysz] Liczba i tekst. Rodzaj bloku zostaw na lekcję 4.

### Materiały
- [kod] Trzy typy w jednym programie | javascript:
  ```
  let bok = 8
  let nazwa = "Wieza Ani"
  let material = PLANKS

  player.onChat("info", function () {
      agent.setItem(material, 64, 1)
      agent.setSlot(1)
      player.say(nazwa + " ma bok " + bok)
  })
  ```

## [guided] Przebudowa starego programu (14 min)

### Co robić teraz
- [mów] Teraz posprzątamy w waszych starych programach. To się nazywa refaktoryzacja i robią to programiści na całym świecie.
- Krok 1: otwórzcie program z lekcji 2 — ramkę albo ścieżkę.
- Krok 2: przeczytajcie go i **wypiszcie na kartce wszystkie liczby**, które się w nim pojawiają.
- Krok 3: zaznaczcie te, które powtarzają się więcej niż raz. To są kandydaci na zmienne.
- [mów] Zasada praktyczna: liczba użyta raz może zostać. Liczba użyta trzy razy musi być zmienną.
- Krok 4: zadeklarujcie zmienne z sensownymi nazwami i podstawcie je w programie.
- Krok 5: uruchomcie. Efekt musi być **dokładnie taki sam** jak przed zmianą.
- [mów] To jest ważne: refaktoryzacja nie zmienia tego, co program robi. Zmienia tylko to, jak jest napisany.
- Krok 6: teraz sprawdźcie, po co to było. Zmieńcie jedną zmienną i zobaczcie, jak łatwo przebudować całość.
- Krok 7: dopiszcie na górze komentarz wyjaśniający, co program buduje i jakie zmienne można zmieniać.

### Wskazówki
- [podpowiedź] Reguła „raz może zostać, trzy razy musi być zmienną” jest praktyczna i dzieci ją zapamiętują. Nie wchodź w niuanse.
- [podpowiedź] Podkreślenie, że po refaktoryzacji efekt jest identyczny, jest ważne. To odróżnia porządkowanie od zmiany funkcjonalności.
- [błąd] Po podstawieniu zmiennych program działa inaczej, bo dziecko podstawiło zmienną także tam, gdzie liczba znaczyła co innego. Na przykład rozmiar stosu w ekwipunku to nie jest bok budowli.
- [błąd] Dziecko zamienia na zmienne wszystkie liczby, łącznie z numerem slotu i liczbą 1 przy ruchu. To zaciemnia kod zamiast go porządkować — nazwij granicę.
- [błąd] Nazwa zmiennej nie mówi nic, na przykład liczba1. Wróć do listy dobrych i złych nazw.
- [dla szybszych] Niech przebudują dwa programy i sprawdzą, czy da się w obu użyć tych samych nazw zmiennych.
- [gdy nie zdążysz] Jedna zmienna w jednym programie.

## [challenge] Parametryzowana budowla (8 min)

### Co robić teraz
- Zadanie samodzielne: napisz program budujący coś, co da się zmienić **wyłącznie** przez zmianę zmiennych na górze.
- Wymagania: co najmniej dwie zmienne liczbowe i jedna z materiałem.
- Test: podaj kolegom na czacie trzy zestawy wartości i sprawdź, czy twój program zbuduje trzy różne budowle bez zmiany reszty kodu.
- Kto skończy, wkleja swoje deklaracje zmiennych na czat.

### Wskazówki
- [tempo] Test z trzema zestawami wartości jest tu sednem zadania. Bez niego dzieci deklarują zmienne, ale nadal mają liczby wpisane gdzieś na sztywno.
- [dla szybszych] Niech dodadzą zmienną, która steruje czymś nieoczywistym — na przykład odstępem między elementami.
- [gdy nie zdążysz] Jedna zmienna liczbowa wystarczy.
- [błąd] Budowla zmienia rozmiar tylko w jednym wymiarze, bo drugi jest nadal wpisany na sztywno. Test z trzema zestawami to wyłapie.

## [challenge] Pokaz i wyjście ze świata (5 min)

### Co robić teraz
- Przypomnij o wyjściu przez menu.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje swój program, zmienia jedną zmienną na oczach grupy i uruchamia ponownie.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Zmiana wartości na żywo, przed grupą, jest tu najlepszym pokazem. Efekt „jedna liczba, inna budowla” działa za każdym razem.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: co znaczy znak równości w kodzie? Kiedy liczba powinna zostać zmienną? Czym różni się liczba od tekstu?
- [mów] Dzisiaj wasze programy przestały być sztywne. Jedna liczba na górze steruje całą budowlą.
- [mów] To jest pierwszy krok do czegoś większego: programu, który buduje różne rzeczy, choć jest jeden. Za trzy tygodnie zrobicie z tego funkcje.
- Zajawka: „Na następnych zajęciach poznacie pętlę w wersji tekstowej. Zobaczycie, skąd bierze się w niej ta dziwna linijka z trzema częściami.”
- Przypomnij zadanie domowe: zamień powtarzające się liczby na zmienne.

### Wskazówki
- [błąd] Nazwa zmiennej w cudzysłowie — program dostaje tekst zamiast wartości.
- [błąd] Rachunek wykonany przed przypisaniem wartości — kolejność linijek ma znaczenie.
- [błąd] Rachunek nie przelicza się sam po zmianie zmiennej.
- [błąd] Sklejony napis zamiast sumy — jedna z wartości jest tekstem.
- [błąd] Nazwa bloku wpisana jako tekst zamiast wybrana z listy.
