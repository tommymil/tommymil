# Zmienne — wynik, życie, czas
Subject: Scratch
Level: Poziom 2 — 10-12 lat
Czas: 95 min
Tags: Zmienne, Licznik, Stan gry, Plansza
Opis: Gra zaczyna pamiętać. Dzieci tworzą zmienne, budują licznik punktów, pasek życia i odliczanie czasu, a przy okazji poznają najczęstszy błąd początkującego — brak zerowania.
Cel: Dziecko rozumie zmienną jako nazwane miejsce na wartość, potrafi ją utworzyć, ustawić, zmienić i wyświetlić oraz wie, dlaczego stan początkowy musi ją zerować.

### Po zajęciach dziecko potrafi
- utworzyć zmienną i nadać jej sensowną nazwę
- odróżnić bloczek ustaw od zmień i powiedzieć, kiedy używa się którego
- zbudować licznik punktów, licznik życia i odliczanie czasu
- wyjaśnić, dlaczego zmienną trzeba wyzerować na starcie gry

### Przygotuj przed zajęciami
- gotowe demo: gra z widocznym wynikiem, życiem i czasem na planszy
- przygotowany zły przykład: ta sama gra bez zerowania zmiennych, uruchomiona trzy razy pod rząd
- projekty dzieci z lekcji 2 — dziś wracamy do gry, nie do wzorów
- lista propozycji nazw zmiennych do pokazania na czacie

### Zadanie domowe
- dodaj do swojej gry czwartą zmienną własnego pomysłu i wymyśl, co ma pokazywać

## [intro] Powitanie i demo planszy (8 min)

### Co robić teraz
- [mów] Cześć. Wasze gry mają jeden poważny brak: nic nie pamiętają. Postać biega, przeciwnik goni, ale nikt nie liczy, kto wygrywa.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** pokaż grę z wynikiem, życiem i czasem widocznymi na scenie. Zagraj chwilę, żeby liczby się zmieniały.
- [mów] Trzy liczby w lewym górnym rogu. To jest różnica między zabawką a grą.
- **Drugi pokaz, celowo zły:** uruchom tę samą grę trzy razy pod rząd bez zerowania. Wynik zaczyna od 47, potem od 92.
- [mów] To jest błąd, który dziś popełni każde z was. Pokazuję go teraz, żebyście wiedzieli, czego szukać.

### Wskazówki
- [tempo] Pokazanie zepsutej wersji **przed** lekcją, a nie po, oszczędza dwadzieścia minut szukania błędu na koniec zajęć.
- [podpowiedź] Ta lekcja jest przełomowa w całym kursie. Wszystko od niej dalej opiera się na zmiennych.

## [review] Wracamy do gry i przypominamy pętlę (5 min)

### Co robić teraz
- Wszyscy otwierają projekt z lekcji 2 — ten z pościgiem, nie ten ze wzorami. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie i sprawdźcie, czy gracz ucieka, a przeciwnik goni.
- Szybka powtórka: co robi bloczek skieruj się w stronę? Czym różni się idź do od leć do? Ile razy wykona się wnętrze pętli powtórz 4 zagnieżdżonej w powtórz 10?
- [mów] Wzory z zeszłego tygodnia zostawiamy w osobnym projekcie. Dziś wracamy do gry i zostajemy przy niej do końca semestru.

### Wskazówki
- [błąd] Dziecko otworzyło projekt ze wzorami. Sprawdź nazwy projektów — stąd bierze się nawyk sensownego nazywania.
- [gdy nie zdążysz] Sam start projektu, pytania pomiń.

## [concept] Czym jest zmienna (14 min)

### Co robić teraz
- [mów] Zmienna to nazwane miejsce, w którym program trzyma jedną wartość. Nazwane — czyli możecie się do niej odwołać po imieniu.
- Pokaż kategorię Zmienne i przycisk Utwórz zmienną. Utwórzcie razem zmienną o nazwie **wynik**.
- [mów] Zwróćcie uwagę na okienko: „dla wszystkich duszków” albo „tylko dla tego duszka”. Dziś wszystkie robimy dla wszystkich. Do drugiej opcji wrócimy przy klonach na lekcji 8.
- Po utworzeniu na scenie pojawia się prostokąt z nazwą i wartością. To podgląd, który można włączyć i wyłączyć znacznikiem obok nazwy zmiennej w palecie.
- Pokaż trzy bloczki i wyjaśnij różnice, klikając każdy na żywo.
- Ustaw wynik na 0 — wpisuje konkretną wartość, niezależnie od tego, co było wcześniej.
- Zmień wynik o 1 — dokłada do tego, co już jest.
- Sam owalny bloczek wynik — to **wartość**, wchodzi do dziurek w innych bloczkach.
- [mów] Ta różnica między ustaw a zmień wraca w tym kursie po raz trzeci. Przy efektach, przy rozmiarze, teraz przy zmiennych. To jest ogólna zasada, nie ciekawostka.
- Ćwiczenie: kliknijcie zmień wynik o 1 pięć razy i patrzcie na podgląd. Potem kliknijcie ustaw wynik na 0. Potem znowu zmień.
- Sprawdzenie na czat: mam wynik 10 i klikam „zmień wynik o -3” dwa razy. Ile wyjdzie?

### Wskazówki
- [podpowiedź] Nazwy zmiennych to dobry moment na pierwszą rozmowę o czytelności kodu. „wynik” jest lepsze niż „a”, a „zmienna1” jest najgorsze. Ta grupa wiekowa świetnie to przyjmuje.
- [błąd] Dziecko utworzyło zmienną „tylko dla tego duszka” i potem nie widzi jej u innego duszka. Dziś to zawsze pomyłka — pokaż, jak usunąć i utworzyć na nowo.
- [błąd] Dziecko szuka bloczka z wartością zmiennej i nie może go znaleźć, bo to owal na górze kategorii, wyglądający jak sama nazwa.
- [błąd] Podgląd zmiennej zasłania scenę. Można go przeciągnąć myszką w inne miejsce.
- [dla szybszych] Niech sprawdzą, co się stanie, gdy do zmiennej wpiszą tekst zamiast liczby, i co wtedy zrobi bloczek zmień o 1.

### Materiały
- [kod] Trzy operacje na zmiennej | scratch:
  ```
  ustaw [wynik] na (0)
  zmień [wynik] o (1)
  zmień [wynik] o (-3)
  ```

## [guided] Licznik punktów i życie (18 min)

### Co robić teraz
- Krok 1: utwórzcie drugą zmienną o nazwie **życie**.
- Krok 2: na **Scenie** zbudujcie skrypt startowy gry: kiedy kliknięto zieloną flagę, ustaw wynik na 0, ustaw życie na 3.
- [mów] Dlaczego na Scenie, a nie na duszku? Bo to nie jest sprawa żadnej pojedynczej postaci. Scena jest dobrym miejscem na zerowanie wszystkiego, co dotyczy całej gry.
- Krok 3: sprawdźcie. Kliknijcie flagę kilka razy. Liczby zawsze wracają do 0 i 3. **To jest lekarstwo na błąd z demo.**
- Krok 4: dodajcie do gry duszka-monetę. Prosty obiekt, na przykład Ball albo Star.
- Krok 5: na monecie zbudujcie skrypt: kiedy kliknięto zieloną flagę, pokaż, a potem pętla zawsze, w niej idź do x: losuj od -200 do 200, y: losuj od -140 do 140, czekaj 2 sek.
- Krok 6: na monecie zbudujcie drugi skrypt: kiedy ten duszek kliknięty, zmień wynik o 1, zagraj dźwięk Pop.
- Krok 7: uruchomcie i klikajcie monetę. Wynik rośnie. Wpiszcie na czat G albo znak zapytania.
- Krok 8: teraz życie. Na przeciwniku, tym goniącym, zbudujcie: kiedy ten duszek kliknięty, zmień życie o -1.
- [mów] Na razie tracicie życie przez kliknięcie przeciwnika, co jest bez sensu. Na lekcji 6 zamienimy to na dotknięcie i wtedy gra nabierze sensu.

### Wskazówki
- [podpowiedź] Zerowanie na Scenie zamiast na duszku to decyzja projektowa, nie techniczna. Powiedz to wprost — starsze dzieci lubią rozumieć, dlaczego coś jest tam, gdzie jest.
- [błąd] Wynik rośnie o kilka przy jednym kliknięciu, bo dziecko kliknęło szybko dwa razy. To nie awaria.
- [błąd] Zmienna nie zeruje się przy starcie, bo skrypt zerujący trafił na duszka, który akurat nie startuje. Sprawdź, gdzie leży skrypt.
- [błąd] Życie schodzi poniżej zera i nic się nie dzieje. To normalne — reakcja na zero to lekcja 5, z warunkami.
- [błąd] Moneta skacze, ale nie da się w nią trafić, bo czekanie jest za krótkie. Dwie sekundy to dobra wartość na start.
- [dla szybszych] Niech dodadzą drugą monetę wartą 5 punktów, rzadziej pojawiającą się, i dobiorą częstotliwość tak, żeby gra była wyważona.
- [gdy nie zdążysz] Sam licznik punktów. Życie wraca na lekcji 6, gdzie i tak jest potrzebne.

### Materiały
- [kod] Zerowanie stanu gry — skrypt na Scenie | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw [wynik] na (0)
    ustaw [życie] na (3)
  ```
- [kod] Moneta — pojawianie się i punkty | scratch:
  ```
  kiedy kliknięto zieloną flagę
    pokaż
    zawsze
      idź do x: (losuj od (-200) do (200)) y: (losuj od (-140) do (140))
      czekaj (2) sek
  kiedy ten duszek kliknięty
    zmień [wynik] o (1)
    zagraj dźwięk [Pop]
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Zapowiedź: „Po przerwie wasza gra dostanie zegar. Będziecie mieli minutę na zdobycie jak największej liczby punktów.”

### Wskazówki
- [tempo] Odliczanie czasu zamienia zbiór mechanik w grę z celem. To najlepsza część tej lekcji i warto mieć na nią pełne trzynaście minut.

## [concept] Czas — zmienna, która sama maleje (13 min)

### Co robić teraz
- [mów] Do tej pory zmienne zmieniały się wtedy, gdy coś zrobiliście. Teraz zbudujemy taką, która zmienia się sama.
- Krok 1: utwórzcie zmienną **czas**.
- Krok 2: na Scenie dołóżcie do skryptu startowego ustaw czas na 60.
- Krok 3: na Scenie zbudujcie **osobny** skrypt: kiedy kliknięto zieloną flagę, powtórz 60, w środku czekaj 1 sek oraz zmień czas o -1.
- Krok 4: uruchomcie. Zegar odlicza. Wpiszcie na czat G albo znak zapytania.
- [mów] Zwróćcie uwagę na coś ważnego. To jest **drugi skrypt uruchomiony tą samą zieloną flagą**. Oba działają równocześnie: jeden ustawia stan, drugi odlicza. Scratch nie ma z tym problemu.
- Krok 5: pytanie do grupy: dlaczego powtórz 60, a nie zawsze? Zbierz odpowiedzi.
- Wyjaśnij: bo chcemy, żeby odliczanie się skończyło. Pętla zawsze zeszłaby poniżej zera w nieskończoność.
- Krok 6: pod pętlą, już poza nią, dołóżcie zatrzymaj wszystko z kategorii Kontrola.
- Krok 7: uruchomcie i sprawdźcie. Po minucie gra staje.
- [mów] Macie kompletną pętlę rozgrywki: start, gra, koniec. Brakuje tylko ekranu z wynikiem — to lekcja 9.

### Wskazówki
- [podpowiedź] Zależność „liczba powtórzeń równa się wartości startowej” jest tu ukryta i warto ją nazwać. Jeśli dziecko zmieni czas na 30, musi zmienić też pętlę. Na lekcji 5 zastąpimy to warunkiem i problem zniknie.
- [błąd] Zegar odlicza za szybko — brakuje bloczka czekaj 1 sek albo wartość jest ułamkiem.
- [błąd] Zegar schodzi poniżej zera — pętla zawsze zamiast powtórz albo brak zatrzymania.
- [błąd] Bloczek zatrzymaj wszystko wylądował w środku pętli. Gra staje po pierwszej sekundzie. Bardzo częste i łatwe do przeoczenia.
- [błąd] Dziecko dołożyło odliczanie do skryptu zerującego, pod nim. Wtedy zerowanie działa, ale odliczanie startuje dopiero po nim — na dziś bez różnicy, ale warto pokazać rozdzielenie.
- [dla szybszych] Niech zbudują wersję, w której czas dolicza się przy zebraniu monety — plus 3 sekundy za punkt. To zmienia całą dynamikę gry.
- [gdy nie zdążysz] Odliczanie bez zatrzymania gry. Zatrzymanie wraca na lekcji 5 w wersji z warunkiem.

### Materiały
- [kod] Odliczanie czasu — skrypt na Scenie | scratch:
  ```
  kiedy kliknięto zieloną flagę
    powtórz (60)
      czekaj (1) sek
      zmień [czas] o (-1)
    zatrzymaj [wszystko]
  ```

## [guided] Plansza gry i wygląd zmiennych (14 min)

### Co robić teraz
- [mów] Wasze zmienne działają, ale wyglądają jak surowe prostokąty. Zrobimy z nich planszę.
- Krok 1: kliknijcie prawym przyciskiem na podgląd zmiennej na scenie. Pokażą się trzy tryby: normalny, duży odczyt i suwak.
- Krok 2: ustawcie **wynik** na duży odczyt i przeciągnijcie go w lewy górny róg.
- Krok 3: ustawcie **czas** na duży odczyt i przeciągnijcie w prawy górny róg.
- Krok 4: **życie** zostawcie w trybie normalnym, z nazwą — bo sama liczba bez podpisu nic nie mówi.
- [mów] To jest projektowanie interfejsu. Gracz ma zobaczyć to, co ważne, tam gdzie się spodziewa, i nie musieć czytać etykiet dla rzeczy oczywistych.
- Krok 5: pokaż tryb suwaka. Utwórzcie zmienną **prędkość** i ustawcie ją na suwak.
- Krok 6: w pętli gry gracza zamieńcie stałą liczbę w bloczku przesuń o na owalny bloczek zmiennej prędkość.
- Krok 7: uruchomcie i przesuwajcie suwak w trakcie gry. Postać przyspiesza i zwalnia na żywo.
- [mów] To jest wasze pierwsze narzędzie do strojenia gry. Zamiast zatrzymywać, zmieniać liczbę i uruchamiać od nowa — kręcicie suwakiem i od razu widzicie efekt.

### Wskazówki
- [podpowiedź] Suwak jest jedną z najbardziej niedocenionych funkcji Scratcha i robi ogromne wrażenie na tej grupie wiekowej. Warto poświęcić mu pełne pięć minut.
- [błąd] Zmienna wstawiona do bloczka przesuń o, ale gra stoi — prędkość ma wartość 0. Trzeba ustawić ją w stanie początkowym albo przesunąć suwak.
- [błąd] Suwak nie działa, bo zmienna została ustawiona bloczkiem w pętli i nadpisuje to, co ustawia suwak. Dobre ćwiczenie z szukania przyczyny.
- [błąd] Podglądy zasłaniają postać. Przeciągnijcie je na brzeg sceny.
- [dla szybszych] Niech dodadzą suwak sterujący częstotliwością pojawiania się monety i znajdą ustawienia, przy których gra jest najciekawsza.
- [gdy nie zdążysz] Duży odczyt dla wyniku i czasu. Suwak pomiń.

## [challenge] Twoja plansza (8 min)

### Co robić teraz
- Zadanie samodzielne: dokończ planszę swojej gry. Wybierz dwie rzeczy.
- Dodaj czwartą zmienną własnego pomysłu — na przykład rekord, poziom albo liczbę zebranych monet danego rodzaju.
- Zrób, żeby moneta dawała różną liczbę punktów w zależności od czegoś.
- Dodaj suwak strojący dowolny element gry.
- Ustaw wszystkie podglądy tak, żeby plansza wyglądała jak w prawdziwej grze.
- Zasada obowiązkowa: **każda nowa zmienna musi być zerowana w skrypcie startowym**. Sprawdź to przed końcem.

### Wskazówki
- [tempo] Wymóg zerowania jest tu ważniejszy niż sam efekt. Sprawdź go u każdego dziecka — to nawyk na cały rok.
- [dla szybszych] Niech zbudują rekord, który **nie** zeruje się przy starcie i pamięta najlepszy wynik z całej sesji. To celowe złamanie reguły i świetny temat do rozmowy, dlaczego akurat tu jest uzasadnione.
- [gdy nie zdążysz] Jedna nowa zmienna wystarczy.
- [błąd] Dziecko dodało zmienną i zapomniało o zerowaniu. Nie poprawiaj sam — zapytaj, co się stanie przy trzecim uruchomieniu.

## [challenge] Pokaz prac i zapis (5 min)

### Co robić teraz
- Zapis: Plik, Zapisz teraz. Poczekaj na potwierdzenie od każdego dziecka.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje swoją planszę i mówi, jakie ma zmienne i co każda oznacza.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Proszenie o nazwanie zmiennych na głos wychwytuje dzieci, które nazwały je „a” i „b” — i robi to bez wytykania.
- [tempo] Zapis przed pokazem, zawsze.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: czym różni się ustaw od zmień? Dlaczego zmienne trzeba zerować na starcie? Gdzie najlepiej umieścić skrypt zerujący i dlaczego?
- [mów] Dzisiaj wasza gra zaczęła pamiętać. To jest największy skok w całym pierwszym semestrze — wszystko, co zrobimy dalej, opiera się na zmiennych.
- [mów] I jedna rzecz, która was gryzła przez całą lekcję: gra nie reaguje, gdy życie spadnie do zera, a czas odlicza sztywno 60 razy. Bo brakuje jej umiejętności podejmowania decyzji.
- Zajawka: „Na następnych zajęciach poznacie warunki. Gra zacznie sprawdzać, co się dzieje, i sama decydować, co zrobić.”
- Przypomnij zadanie domowe.

### Wskazówki
- [błąd] Wynik nie startuje od zera — brak zerowania w stanie początkowym. Błąd numer jeden tej lekcji.
- [błąd] Zmienna „tylko dla tego duszka” niewidoczna u innych duszków.
- [błąd] Zatrzymaj wszystko w środku pętli odliczającej — gra staje po sekundzie.
- [błąd] Zegar odlicza za szybko — brak bloczka czekaj 1 sek.
- [błąd] Suwak nie działa, bo skrypt nadpisuje zmienną w pętli.
