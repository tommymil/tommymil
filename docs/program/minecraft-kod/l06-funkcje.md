# Funkcje — własny przepis na budowlę
Subject: Minecraft Education
Level: Poziom 2 — 10-12 lat, kod
Czas: 95 min
Tags: Funkcje, Porządek w kodzie, Nazewnictwo, Wywołanie
Opis: Dzieci wydzielają powtarzające się fragmenty do funkcji i odkrywają, że program da się czytać jak spis treści.
Cel: Dziecko definiuje własną funkcję, wywołuje ją z programu głównego i potrafi wskazać w swoim kodzie fragmenty, które nadają się do wydzielenia.

### Po zajęciach dziecko potrafi
- zdefiniować funkcję i wywołać ją z innego miejsca programu
- wydzielić powtarzający się fragment kodu do funkcji
- nadać funkcji nazwę, która mówi, co ona robi
- wyjaśnić, dlaczego program podzielony na funkcje łatwiej się czyta i naprawia

### Przygotuj przed zajęciami
- gotowe demo: dwa programy budujące to samo — jeden długi i płaski, drugi krótki, złożony z funkcji
- światy dzieci z lekcji 5
- przygotowany zły przykład: ten sam kod ściany wklejony trzy razy z jedną różnicą, która psuje efekt
- lista dobrych i złych nazw funkcji

### Zadanie domowe
- podziel dowolny swój program na co najmniej trzy funkcje i sprawdź, czy efekt się nie zmienił

## [intro] Powitanie i demo dwóch programów (8 min)

### Co robić teraz
- [mów] Cześć. Dziś nauczycie się czegoś, co nie zmieni tego, co budujecie, tylko to, jak wygląda wasz kod. I to jest ważniejsze, niż brzmi.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO — dwa programy budujące ten sam dom.** Pokaż pierwszy: siedemdziesiąt linijek, trzeba przewijać, wszystko jednym ciągiem.
- Pokaż drugi: program główny ma cztery linijki. Zbuduj fundament, zbuduj ściany, zbuduj dach, postaw drzwi.
- [mów] Który z nich rozumiecie po pięciu sekundach patrzenia?
- [mów] Ten drugi czyta się jak spis treści. Nie wiecie jeszcze, jak działa każdy kawałek, ale wiecie, co robi cały program.
- Odczytaj kilka zadań domowych: kto policzył pokój i czy się zgadzało.

### Wskazówki
- [podpowiedź] Porównanie „siedemdziesiąt linijek kontra cztery” jest tu całym uzasadnieniem funkcji. Pokaż oba na ekranie, przewijając ten długi.
- [tempo] To lekcja o porządku, nie o nowym efekcie. Powiedz to wprost — dzieci nie powinny szukać nowej budowli.

## [review] Wracamy do pomieszczenia (5 min)

### Co robić teraz
- Wszyscy otwierają świat z lekcji 5 i Code Builder w widoku JavaScript. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie program budujący pokój.
- Szybka powtórka: ile operacji wykona pętla 5 w pętli 4? Co się dzieje z pracą komputera, gdy podwoicie wszystkie wymiary?
- Poproś, żeby każde dziecko policzyło linijki swojego programu i wpisało liczbę na czat.
- [mów] Zapamiętajcie tę liczbę. Na koniec lekcji program będzie miał tyle samo linijek, ale będzie się dało go przeczytać.

### Wskazówki
- [podpowiedź] Uczciwe uprzedzenie, że funkcje nie skracają kodu, tylko go porządkują, jest ważne. Dzieci pamiętają lekcję o pętlach i będą oczekiwały skrócenia.
- [gdy nie zdążysz] Sam start i policzenie linijek.

## [concept] Funkcja, czyli nazwany kawałek programu (14 min)

### Co robić teraz
- [mów] Funkcja to fragment programu, któremu daliście nazwę. Potem możecie ten fragment wywołać, pisząc tę nazwę.
- Pokaż definicję: słowo function, nazwa, nawiasy okrągłe, klamry, a w środku kod.
- [mów] Zwróćcie uwagę: definicja funkcji **nic nie robi**. To jest przepis leżący w szufladzie. Wykonuje się dopiero wtedy, gdy go wywołacie.
- Krok 1: nad komendą na czacie napiszcie definicję funkcji o nazwie przygotujAgenta.
- Krok 2: przenieście do niej trzy linijki: przeniesienie do gracza, ustawienie materiału, ustawienie slotu.
- Krok 3: w komendzie na czacie, w miejscu, gdzie były te linijki, napiszcie samą nazwę funkcji z nawiasami.
- Krok 4: uruchomcie. Program działa dokładnie tak samo. **Czekaj na kciuki.**
- [mów] Nic się nie zmieniło w działaniu. Zmieniło się to, że wasza komenda zaczyna się teraz od jednej czytelnej linijki zamiast trzech technicznych.
- Krok 5: teraz sprawdźcie, po co to naprawdę było. Zmieńcie materiał **w funkcji** i uruchomcie.
- [mów] Materiał zmienił się wszędzie, gdzie wywołujecie tę funkcję. Jedno miejsce, jedna zmiana.
- Krok 6: o nazwach. Pokaż listę i omów.
- Złe nazwy: funkcja1, zrob, cos, test.
- Dobre nazwy: przygotujAgenta, zbudujSciane, postawSlupek, wrocNaStart.
- [mów] Dobra nazwa funkcji to **czasownik**. Ona coś robi, więc jej nazwa mówi, co robi.

### Wskazówki
- [podpowiedź] Zasada „nazwa funkcji to czasownik” jest prosta, praktyczna i dzieci ją zapamiętują. Egzekwuj ją do końca kursu.
- [błąd] Dziecko definiuje funkcję, ale zapomina ją wywołać. Program nic nie robi, a błędu nie ma. Klasyczny błąd logiczny — świetna okazja na metodę czytania kodu na głos.
- [błąd] Wywołanie bez nawiasów. Wtedy nic się nie dzieje albo edytor podkreśla. Nawiasy są obowiązkowe.
- [błąd] Definicja funkcji wstawiona **wewnątrz** komendy na czacie. Zadziała, ale zaciemnia. Ustalcie: definicje na górze, komendy pod nimi.
- [błąd] Dziecko wywołuje funkcję przed jej definicją i dziwi się, że działa. W tym języku to jest w porządku — powiedz to, żeby nie szukali problemu.
- [dla szybszych] Niech napiszą funkcję, która wywołuje inną funkcję, i sprawdzą, czy to działa.

### Materiały
- [kod] Pierwsza funkcja | javascript:
  ```
  let material = PLANKS

  function przygotujAgenta() {
      agent.teleportToPlayer()
      agent.setItem(material, 512, 1)
      agent.setSlot(1)
  }

  player.onChat("start", function () {
      przygotujAgenta()
      agent.move(FORWARD, 5)
  })
  ```

## [guided] Rozbijamy pokój na funkcje (18 min)

### Co robić teraz
- [mów] Teraz weźmiemy wasz program z pokojem i podzielimy go na kawałki.
- Krok 1: przeczytajcie swój program i **wypiszcie na kartce**, z jakich części się składa. Zwykle wyjdą trzy albo cztery.
- Typowe części: przygotowanie Agenta, podłoga, ściany, sufit.
- Krok 2: dla każdej części zapiszcie nazwę funkcji. Czasownik plus rzeczownik.
- Krok 3: utwórzcie pierwszą funkcję — zbudujPodloge — i przenieście do niej odpowiedni fragment kodu.
- Krok 4: w komendzie zostawcie wywołanie. Uruchomcie i sprawdźcie, czy nic się nie zmieniło.
- [mów] Po każdym wydzieleniu **sprawdzajcie**. Jeśli zrobicie wszystkie naraz i coś przestanie działać, nie będziecie wiedzieli które.
- Krok 5: to samo dla ścian i sufitu, po kolei, sprawdzając po każdym.
- Krok 6: spójrzcie teraz na swoją komendę na czacie. Ma cztery linijki i czyta się ją jak zdanie.
- Krok 7: policzcie linijki całego programu i porównajcie z liczbą sprzed lekcji.
- [mów] Linijek jest tyle samo albo trochę więcej. A program jest dużo łatwiejszy do czytania. To jest cała różnica.

### Wskazówki
- [podpowiedź] Zasada „wydzielaj po jednej funkcji i sprawdzaj po każdej” to praktyka, która oszczędza godziny. Egzekwuj ją tutaj bardzo mocno.
- [błąd] Po wydzieleniu ściany nie pasują do podłogi, bo funkcja zakłada, że Agent stoi gdzieś indziej, niż stoi po poprzedniej funkcji. To jest **właściwy** problem tej lekcji — omów go, nie omijaj.
- [błąd] Dziecko wydziela wszystko naraz, nic nie działa i nie wie dlaczego. Cofnijcie i zróbcie po kolei.
- [błąd] Funkcja korzysta ze zmiennej zadeklarowanej wewnątrz komendy i jej nie widzi. Zmienne wspólne deklarujcie na górze programu.
- [błąd] Nazwa funkcji to rzeczownik, na przykład podloga. Zadziała, ale nie mówi, że coś się dzieje. Poproś o czasownik.
- [dla szybszych] Niech wydzielą jeszcze jedną funkcję — wrocNaStart — i użyją jej między pozostałymi.
- [gdy nie zdążysz] Dwie funkcje zamiast czterech.

### Materiały
- [kod] Pokój podzielony na funkcje | javascript:
  ```
  let dlugosc = 10
  let szerokosc = 8
  let wysokosc = 3

  function przygotujAgenta() {
      agent.teleportToPlayer()
      agent.setItem(PLANKS, 512, 1)
      agent.setSlot(1)
  }

  function zbudujPodloge() {
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

  function zbudujSciany() {
      for (let poziom = 0; poziom < wysokosc; poziom++) {
          for (let bok = 0; bok < 2; bok++) {
              for (let i = 0; i < dlugosc; i++) {
                  agent.move(FORWARD, 1)
                  agent.place(BACK)
              }
              agent.turn(RIGHT)
              for (let i = 0; i < szerokosc; i++) {
                  agent.move(FORWARD, 1)
                  agent.place(BACK)
              }
              agent.turn(RIGHT)
          }
          agent.move(UP, 1)
      }
  }

  player.onChat("pokoj", function () {
      przygotujAgenta()
      zbudujPodloge()
      przygotujAgenta()
      zbudujSciany()
  })
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały edytora.
- Zapowiedź: „Po przerwie zobaczycie, po co to naprawdę było — zbudujecie trzy pokoje jednym wywołaniem każdy.”

### Wskazówki
- [tempo] Wydzielanie funkcji jest żmudne i nie daje natychmiastowej nagrody. Zapowiedź drugiej połowy pomaga przetrwać pierwszą.

## [concept] Po co to naprawdę było (13 min)

### Co robić teraz
- [mów] Do tej pory funkcje tylko porządkowały. Teraz zobaczycie, po co są naprawdę.
- **Zły przykład:** pokaż program, w którym kod ściany jest wklejony trzy razy. W jednej z kopii jest inna liczba — literówka.
- [mów] Trzeci pokój ma ścianę o jeden blok krótszą. Ktoś przekleił i poprawił dwa razy zamiast trzech.
- [mów] Z funkcją to jest niemożliwe. Kod jest w **jednym** miejscu, więc nie da się poprawić go częściowo.
- Krok 1: w waszej komendzie wywołajcie funkcję budującą ścianę trzy razy pod rząd, przesuwając Agenta między wywołaniami.
- Krok 2: uruchomcie. Trzy identyczne ściany.
- Krok 3: zmieńcie coś **w funkcji** — na przykład materiał albo długość. Uruchomcie.
- Krok 4: wszystkie trzy ściany zmieniły się naraz. **Czekaj na kciuki.**
- [mów] To jest główny powód, dla którego istnieją funkcje. Jedno miejsce, jedna zmiana, wszędzie efekt.
- [mów] Zasada, którą warto zapamiętać: **jeśli przeklejasz kod, to znaczy, że potrzebujesz funkcji**. Bez wyjątków.
- Krok 5: przejrzyjcie swój program i znajdźcie fragment, który przekleiliście kiedyś. Wydzielcie go.

### Wskazówki
- [podpowiedź] Zasada „przeklejasz kod, czyli potrzebujesz funkcji” to najważniejsze zdanie tej lekcji. Wypisz je na ekranie.
- [podpowiedź] Zły przykład z literówką w jednej z trzech kopii jest realnym scenariuszem, który dzieci znają z własnych programów. Poproś, żeby powiedziały, czy im się to zdarzyło.
- [błąd] Trzy ściany budują się w tym samym miejscu, bo brakuje przesunięcia Agenta między wywołaniami.
- [błąd] Druga i trzecia ściana są przekrzywione, bo funkcja nie zostawia Agenta w przewidywalnym stanie. To jest realny problem projektowy — omów go i zaproponuj regułę: funkcja zostawia Agenta tam, gdzie go zastała.
- [dla szybszych] Niech napiszą funkcję, która buduje wieżę, i wywołają ją cztery razy w rogach prostokąta.
- [gdy nie zdążysz] Dwa wywołania zamiast trzech.

## [guided] Program główny jako spis treści (14 min)

### Co robić teraz
- [mów] Ostatni krok: zróbcie ze swojej komendy spis treści.
- Krok 1: przeczytajcie swoją komendę na czacie. Jeśli są w niej jakiekolwiek pętle albo polecenia Agenta, wydzielcie je.
- [mów] Cel jest prosty: w komendzie mają być **tylko wywołania funkcji**. Nic więcej.
- Krok 2: sprawdźcie, czy każda nazwa funkcji mówi, co ona robi. Popraw te, które nie mówią.
- Krok 3: uruchomcie i sprawdźcie, czy efekt jest ten sam co na początku lekcji.
- Krok 4: pokażcie swój program partnerowi na czacie — **tylko komendę, bez funkcji**.
- Krok 5: partner ma powiedzieć, co ten program buduje, nie widząc szczegółów.
- [mów] Jeśli partner zgadł — wasze nazwy są dobre. Jeśli nie zgadł — nazwy trzeba poprawić.
- Krok 6: popraw nazwy według tego, co powiedział partner.
- [mów] To jest najlepszy test czytelności kodu, jaki istnieje: czy ktoś inny rozumie, co robi wasz program, patrząc tylko na nazwy.

### Wskazówki
- [podpowiedź] Test „partner czyta samą komendę” jest bardzo skuteczny i dzieci go lubią. Wprowadź go jako stały element kolejnych lekcji.
- [błąd] Dziecko wydziela funkcję o nazwie zrobWszystko, w której jest cały program. Technicznie to funkcja, ale nic nie porządkuje. Zapytaj, z czego się składa.
- [błąd] Funkcji jest tak dużo, że każda ma dwie linijki. To druga skrajność — nazwij ją i zaproponuj łączenie.
- [błąd] Partner nie rozumie programu, a dziecko upiera się, że nazwy są dobre. To jest właśnie informacja zwrotna — poproś, żeby przyjęło ją bez obrony.
- [dla szybszych] Niech napiszą program główny, który buduje dwa różne budynki, wywołując te same funkcje w innej kolejności.
- [gdy nie zdążysz] Sam przegląd nazw, bez testu z partnerem.

## [challenge] Twoja biblioteka funkcji (8 min)

### Co robić teraz
- Zadanie samodzielne: zbuduj sobie zestaw funkcji, których będziesz używać do końca kursu.
- Minimum trzy: przygotowanie Agenta, budowanie rzędu, budowanie warstwy.
- Każda ma mieć nazwę zaczynającą się od czasownika.
- Każda ma zostawiać Agenta w przewidywalnym miejscu — zapisz w komentarzu, w jakim.
- Kto skończy, wkleja na czat **same nazwy** swoich funkcji.

### Wskazówki
- [podpowiedź] Komentarz opisujący, gdzie funkcja zostawia Agenta, to bardzo dobry nawyk i rozwiązuje najczęstszy problem tej lekcji.
- [dla szybszych] Niech dodadzą funkcję, która sprawdza i wypisuje na czacie, ile bloków postawi cała budowla.
- [gdy nie zdążysz] Dwie funkcje wystarczą.
- [błąd] Funkcje zostawiają Agenta w różnych miejscach i nie da się ich łączyć w dowolnej kolejności. To jest realny problem projektowy — dobra rozmowa na koniec.

## [challenge] Pokaz i wyjście ze świata (5 min)

### Co robić teraz
- Przypomnij o wyjściu przez menu.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje **tylko swoją komendę na czacie** i czyta ją na głos.
- Grupa zgaduje, co program zbuduje. Potem dziecko uruchamia i sprawdzamy.
- Brawa po każdym pokazie.

### Wskazówki
- [tempo] Zgadywanie przez grupę zamienia pokaz w zabawę i jednocześnie sprawdza jakość nazw. Warto to utrzymać.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: co to jest funkcja? Kiedy na pewno potrzebujesz funkcji? Jaka powinna być nazwa funkcji?
- [mów] Dzisiaj nie zbudowaliście nic nowego. Ale wasze programy zmieniły się z listy poleceń w coś, co da się przeczytać i zrozumieć.
- [mów] I poznaliście zasadę, która obowiązuje wszędzie: jeśli przeklejasz kod, potrzebujesz funkcji.
- [mów] Ale zauważcie ograniczenie. Wasza funkcja budująca ścianę buduje zawsze ścianę **tej samej długości**. Żeby zbudować inną, trzeba by napisać drugą funkcję.
- Zajawka: „Na następnych zajęciach funkcje dostaną parametry. Jedna funkcja zbuduje ścianę dowolnej długości, z dowolnego materiału.”
- Przypomnij zadanie domowe: podziel program na trzy funkcje.

### Wskazówki
- [błąd] Funkcja zdefiniowana, ale nigdy nie wywołana — program nic nie robi i nie ma błędu.
- [błąd] Wywołanie bez nawiasów.
- [błąd] Funkcja nie widzi zmiennej zadeklarowanej wewnątrz komendy.
- [błąd] Funkcje zostawiają Agenta w różnych miejscach i nie da się ich łączyć.
- [błąd] Jedna funkcja zrobWszystko zamiast kilku sensownych.
