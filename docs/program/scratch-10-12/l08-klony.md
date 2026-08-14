# Klony — wiele obiektów naraz
Subject: Scratch
Level: Poziom 2 — 10-12 lat
Czas: 95 min
Tags: Klony, Zmienne lokalne, Strumień przeciwników, Podsumowanie miesiąca
Opis: Ostatnia lekcja dwóch pierwszych miesięcy. Jeden skrypt tworzy dziesiątki obiektów, a dzieci kończą kurs startowy z grą, która ma strumień przeciwników i pełny cykl rozgrywki.
Cel: Dziecko tworzy klony jednym skryptem, rozumie, że każdy klon wykonuje ten sam kod niezależnie, i wie, do czego służy zmienna „tylko dla tego duszka”.

### Po zajęciach dziecko potrafi
- utworzyć klon i obsłużyć go osobnym skryptem
- wyjaśnić, dlaczego klon musi się usuwać, i co się dzieje, gdy tego nie robi
- użyć zmiennej „tylko dla tego duszka”, żeby każdy klon miał własną wartość
- złożyć strumień przeciwników w działającą grę

### Przygotuj przed zajęciami
- gotowe demo: deszcz przeciwników spadających z góry, każdy o innej prędkości
- przygotowany zły przykład: projekt z tysiącem klonów, w którym Scratch zwalnia
- projekty dzieci z lekcji 7
- lista tego, co dzieci umieją po ośmiu tygodniach — do odczytania na koniec

### Zadanie domowe
- pokaż grę rodzicom i zapytaj ich o jedną rzecz, którą by zmienili

## [intro] Powitanie i demo deszczu (8 min)

### Co robić teraz
- [mów] Cześć. Dziś ostatnia lekcja z tej części kursu i najmocniejsze narzędzie, jakie poznacie w pierwszym semestrze.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** uruchom grę z deszczem przeciwników. Kilkanaście obiektów spada z góry, każdy z inną prędkością, gracz ucieka.
- [mów] Ile duszków widzicie na scenie? Kilkanaście. A ile jest w projekcie? **Jeden.**
- Pokaż panel duszków — rzeczywiście jeden przeciwnik.
- [mów] To są klony. Jeden duszek, jeden skrypt, dowolnie wiele kopii działających jednocześnie. Dziś zbudujecie to samo.
- Zapytaj grupę: co byście musieli zrobić bez klonów, żeby mieć dwadzieścia przeciwników?

### Wskazówki
- [tempo] Pytanie o liczbę duszków i pokazanie panelu robi tu całą robotę. Zaskoczenie jest szczere i uzasadnione.
- [podpowiedź] Klony są najbardziej lubianym narzędziem tej grupy wiekowej. Po tej lekcji pojawią się w każdym projekcie, czasem nawet tam, gdzie nie trzeba.

## [review] Otwieramy projekt i przypominamy balans (5 min)

### Co robić teraz
- Wszyscy otwierają projekt z lekcji 7. Poczekaj na potwierdzenie od każdego dziecka.
- Zróbcie test trzech uruchomień. Sprawdźcie, czy poziom rośnie i czy da się wygrać.
- Szybka powtórka: co robi rosnący poziom? Dlaczego zmieniamy jedną liczbę naraz? Kiedy losowość psuje grę?
- Kto zrobił zadanie domowe, wpisuje na czat, w której chwili domownik się poddał.

### Wskazówki
- [podpowiedź] Odpowiedzi z zadania domowego to najlepszy materiał do rozmowy o balansie, jaki dostaniesz. Zbierz je i wróć do nich na koniec.
- [gdy nie zdążysz] Sam test trzech uruchomień.

## [concept] Klon i jego własny skrypt (14 min)

### Co robić teraz
- [mów] Klon to kopia duszka, tworzona w trakcie działania programu. Kopia ma ten sam kod, tę samą pozycję i te same kostiumy co oryginał — od momentu utworzenia żyje własnym życiem.
- Pokaż trzy bloczki w kategorii Kontrola: utwórz klon z siebie, kiedy zaczynam jako klon, usuń tego klona.
- [mów] Drugi z nich to **kapelusz zdarzenia** — jak zielona flaga, tylko że zdarzeniem jest powstanie klonu. To tam idzie cały kod, który wykonuje klon.
- Krok 1: na przeciwniku zbudujcie skrypt tworzący: kiedy kliknięto zieloną flagę, ukryj, a potem pętla zawsze, w niej czekaj 1 sek oraz utwórz klon z siebie.
- [mów] Zwróćcie uwagę na bloczek ukryj. Oryginał ma być niewidoczny — jest tylko fabryką klonów. Widoczne mają być kopie.
- Krok 2: zbudujcie skrypt klonu: kiedy zaczynam jako klon, idź do x: losuj od -200 do 200, y: 180, pokaż, a potem powtórz 60, w niej zmień y o -6.
- Krok 3: pod pętlą, poza nią, wstawcie **usuń tego klona**.
- Krok 4: uruchomcie. Obiekty spadają z góry co sekundę. Wpiszcie na czat G albo znak zapytania.
- [mów] Teraz najważniejsze zdanie tej lekcji: **klon, który się nie usuwa, zostaje na zawsze**. Scratch pozwala na trzysta klonów naraz. Po przekroczeniu tej liczby nowe po prostu przestają powstawać i gra wygląda na zepsutą.
- **Zły przykład, na żywo:** usuń bloczek usuwania i uruchom. Po pół minucie nic nowego nie spada, a projekt zwalnia.

### Wskazówki
- [podpowiedź] Pokazanie projektu bez usuwania klonów jest tu obowiązkowe. Bez tego dzieci trafią na ten błąd same, w domu, i nie będą wiedziały, czego szukać.
- [błąd] Klony pojawiają się w miejscu oryginału, bo brakuje ustawienia pozycji w skrypcie klonu.
- [błąd] Oryginał widoczny i wisi na scenie — brakuje bloczka ukryj. Uwaga: bloczek pokaż w skrypcie klonu jest wtedy konieczny, bo klon dziedziczy ukrycie.
- [błąd] Kod klonu podpięty pod zieloną flagę zamiast pod kapelusz klonu. Wtedy nic nie działa i trudno to zauważyć.
- [błąd] Klony powstają za szybko i jest ich sto na ekranie. Wydłuż czekanie w fabryce.
- [dla szybszych] Niech sprawdzą, co się stanie, gdy klon **sam** utworzy klon. To najprostszy sposób, żeby zobaczyć limit trzystu w akcji.

### Materiały
- [kod] Fabryka klonów | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ukryj
    zawsze
      czekaj (1) sek
      utwórz klon z [siebie]
  ```
- [kod] Życie pojedynczego klonu | scratch:
  ```
  kiedy zaczynam jako klon
    idź do x: (losuj od (-200) do (200)) y: (180)
    pokaż
    powtórz (60)
      zmień y o (-6)
    usuń tego klona
  ```

## [guided] Każdy klon inny — zmienne lokalne (18 min)

### Co robić teraz
- [mów] Wasze klony spadają identycznie. Prawdziwy deszcz tak nie wygląda. Każda kropla ma swoją prędkość.
- Krok 1: spróbujcie naiwnie. Utwórzcie zwykłą zmienną **prędkość spadania** i ustawcie ją losowo w skrypcie klonu, a potem użyjcie w bloczku zmień y.
- Krok 2: uruchomcie. **Wszystkie klony zmieniają prędkość jednocześnie.** Wyglądają jak stado, nie jak deszcz.
- [mów] Dlaczego? Bo to jest **jedna** zmienna, wspólna dla wszystkich. Każdy nowy klon nadpisuje ją wszystkim pozostałym.
- Krok 3: usuńcie tę zmienną i utwórzcie nową, zaznaczając w okienku **tylko dla tego duszka**.
- [mów] Pamiętacie, że na lekcji czwartej mówiłem, że wrócimy do tej opcji przy klonach? To jest ten moment. Taka zmienna istnieje osobno w każdym klonie. Dwadzieścia klonów to dwadzieścia niezależnych wartości pod tą samą nazwą.
- Krok 4: w skrypcie klonu ustawcie ją losowo, zaraz po ustawieniu pozycji, i użyjcie w bloczku zmień y.
- Krok 5: uruchomcie. Teraz to jest deszcz. Wpiszcie na czat G albo znak zapytania.
- Krok 6: dodajcie drugą zmienną tylko dla tego duszka — na przykład rozmiar. Klony bliższe niech będą większe i szybsze.
- [mów] To, co właśnie zrobiliście, nazywa się w programowaniu zmienną lokalną. Nauczycie się tego słowa na każdym kursie programowania na świecie, w każdym języku.

### Wskazówki
- [podpowiedź] Kolejność „najpierw naiwnie, potem poprawnie” jest tu absolutnie kluczowa. Bez zobaczenia wspólnej zmiennej w akcji różnica jest czysto teoretyczna.
- [błąd] Zmienna lokalna nie ma podglądu na scenie dla klonów — Scratch pokazuje tylko wartość oryginału. To nie jest awaria, tylko ograniczenie podglądu.
- [błąd] Dziecko utworzyło zmienną lokalną, ale ustawia ją w skrypcie z zieloną flagą, a nie w skrypcie klonu. Wtedy wszystkie klony dziedziczą jedną wartość z chwili utworzenia.
- [błąd] Klon jest większy, ale porusza się tak samo — rozmiar zmieniono, a prędkości nie podpięto pod tę samą zmienną.
- [dla szybszych] Niech zbudują efekt głębi: klon losuje jedną wartość, a rozmiar, prędkość i jasność wyliczają się z niej. Bliższe obiekty są większe, szybsze i jaśniejsze.
- [gdy nie zdążysz] Sama losowa prędkość. Rozmiar i głębia zostają.

### Materiały
- [kod] Klon z własną prędkością | scratch:
  ```
  kiedy zaczynam jako klon
    idź do x: (losuj od (-200) do (200)) y: (180)
    ustaw [moja prędkość] na (losuj od (3) do (9))
    ustaw rozmiar na ((30) + ((moja prędkość) * (8))) %
    pokaż
    powtórz (60)
      zmień y o ((0) - (moja prędkość))
    usuń tego klona
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Zapowiedź: „Po przerwie wasze klony zaczną reagować na gracza i zamienimy to w pełną grę. A na koniec podsumujemy, co umiecie po dwóch miesiącach.”

### Wskazówki
- [tempo] Zmienne lokalne to najtrudniejsze pojęcie tych ośmiu tygodni. Przerwa po nich jest potrzebna.

## [concept] Klony w grze — kolizje i sprzątanie (13 min)

### Co robić teraz
- [mów] Klony spadają ładnie, ale nic nie robią. Zamienimy je w prawdziwe zagrożenie.
- Krok 1: w skrypcie klonu, w środku pętli spadania, dołóżcie warunek: jeżeli dotyka gracza to zmień życie o -1 oraz usuń tego klona.
- [mów] Zauważcie, że usunięcie klonu rozwiązuje problem wielokrotnego naliczania z lekcji szóstej. Obiekt znika, więc warunek przestaje być prawdziwy. Ten sam sposób co z monetą.
- Krok 2: uruchomcie i dajcie się trafić. Jedno życie za jeden klon. Wpiszcie na czat G albo znak zapytania.
- Krok 3: teraz problem, który zauważycie sami: po zakończeniu gry klony zostają zawieszone na scenie.
- [mów] Bloczek zatrzymaj wszystko zatrzymuje skrypty, ale **nie usuwa klonów**. Zostają tam, gdzie były.
- Krok 4: dołóżcie do stanu początkowego na przeciwniku bloczek usuń klony tego duszka. Uruchomcie grę dwa razy pod rząd i sprawdźcie.
- [mów] To jest kolejny element stanu początkowego. Lista rośnie: zmienne, pozycje, efekty, a teraz klony.
- Krok 5: dodajcie klonom sposób na zniknięcie po wylocie poza scenę. Zamiast pętli powtórz 60 użyjcie powtórz aż y mniejsze niż -170, a pod nią usuń tego klona.
- [mów] To jest lepsze niż liczenie powtórzeń, bo działa tak samo przy każdej prędkości. Klon szybki zniknie szybciej, wolny później — i żaden nie zniknie w połowie ekranu.

### Wskazówki
- [podpowiedź] Krok 5 to ważne przejście od „powtórz określoną liczbę razy” do „powtórz, aż coś się stanie”. Ta sama zmiana myślenia co przy odliczaniu czasu na lekcji 5.
- [błąd] Klony zostają na scenie po zakończeniu gry — brak usuwania klonów w stanie początkowym. Bardzo częste i bardzo widoczne.
- [błąd] Gracz traci wszystkie życia naraz, bo klon nie usuwa się po trafieniu.
- [błąd] Klon znika w połowie ekranu — pętla powtórz z liczbą powtórzeń niedopasowaną do prędkości. To argument za wersją z warunkiem.
- [błąd] Bloczek powtórz aż jest w Kontroli i wygląda podobnie do powtórz. Pokaż różnicę na ekranie.
- [dla szybszych] Niech dodadzą klonom, które gracz może zniszczyć kliknięciem, i policzą je osobną zmienną.
- [gdy nie zdążysz] Kolizja i usuwanie klonów w stanie początkowym. Wersja z powtórz aż zostaje.

### Materiały
- [kod] Klon jako zagrożenie | scratch:
  ```
  kiedy zaczynam jako klon
    idź do x: (losuj od (-200) do (200)) y: (180)
    ustaw [moja prędkość] na (losuj od (3) do (9))
    pokaż
    powtórz aż <(pozycja y) < (-170)>
      zmień y o ((0) - (moja prędkość))
      jeżeli <dotyka [Gracz]?> to
        zmień [życie] o (-1)
        usuń tego klona
    usuń tego klona
  ```

## [guided] Składamy grę końcową (14 min)

### Co robić teraz
- [mów] Macie wszystko, czego potrzeba na pierwsze dwa miesiące. Teraz to poskładamy w jedną, skończoną grę.
- Krok 1: przejdźcie listę kontrolną. Czytaj punkt po punkcie, dzieci potwierdzają kciukiem.
- Stan początkowy zeruje zmienne, ustawia pozycje, czyści efekty i **usuwa klony**.
- Gracz porusza się płynnie, w pętli gry.
- Coś daje punkty, coś odbiera życie.
- Trudność rośnie wraz z postępem.
- Gra kończy się i przy zerze życia, i przy zerze czasu, z różnymi komunikatami.
- Krok 2: podepnijcie klony pod poziom trudności z lekcji 7 — im wyższy poziom, tym krótsza przerwa między klonami.
- Krok 3: sprawdźcie wartość skrajną: co się stanie przy poziomie 10? Czekanie nie może zejść poniżej zera.
- Krok 4: test trzech uruchomień pod rząd.
- Krok 5: dostrójcie liczby tak, żeby gra była do przejścia za trzecim albo czwartym podejściem.

### Wskazówki
- [podpowiedź] To jest gra zamykająca kurs startowy. Warto powiedzieć wprost, że dzieci będą ją pokazywać rodzicom — to podnosi staranność bardziej niż jakakolwiek prośba.
- [błąd] Po podpięciu klonów pod poziom gra staje się nie do przejścia po trzydziestu sekundach. Ogranicz minimalne czekanie warunkiem.
- [błąd] Klony przechodzą przez gracza, bo obrazek ma przezroczyste marginesy. Wraca temat z lekcji 6.
- [błąd] Za trzecim uruchomieniem gra jest łatwiejsza, bo poziom nie zeruje się na starcie.
- [dla szybszych] Niech dodadzą drugi rodzaj klonu — na przykład bonusowy, dający punkty. Wymaga rozróżnienia klonów kostiumem i warunku w skrypcie klonu.
- [gdy nie zdążysz] Sama lista kontrolna i test trzech uruchomień. Podpięcie klonów pod poziom pomiń.

## [challenge] Ostatnie szlify (8 min)

### Co robić teraz
- Zadanie samodzielne: dopracuj grę do pokazania. Wybierz dwie rzeczy.
- Dźwięk przy każdym istotnym zdarzeniu — punkt, trafienie, koniec gry.
- Czytelna plansza: duży odczyt wyniku, widoczne życie i czas.
- Ekran startowy: tło z instrukcją, znikające po pierwszym ruchu.
- Drugi rodzaj klonu o innym zachowaniu.
- Kto skończy, wpisuje na czat, co dodał.

### Wskazówki
- [tempo] Ostatnie osiem minut przed pokazem podsumowującym. Pilnuj, żeby nikt nie zaczynał przebudowy całej gry.
- [dla szybszych] Ekran startowy jest najtrudniejszy z listy i wymaga komunikatów, których jeszcze nie było. Skieruj tam dzieci, które chcą wyzwania — to zapowiedź lekcji 9.
- [gdy nie zdążysz] Sam dźwięk przy zdarzeniach.
- [błąd] Dziecko zaczyna dużą zmianę na pięć minut przed pokazem i kończy z niedziałającą grą. Reaguj wcześnie.

## [challenge] Pokaz gier i zapis (5 min)

### Co robić teraz
- Zapis: Plik, Zapisz teraz. Poczekaj na potwierdzenie od każdego dziecka.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko udostępnia ekran i gra w swoją grę przez dwadzieścia sekund, mówiąc jednym zdaniem, co w niej jest najlepsze.
- Brawa po każdym pokazie.

### Wskazówki
- [tempo] Przy dziesięciorgu dzieci to jest ciasne. Skróć do piętnastu sekund, ale nie pomijaj nikogo — to pokaz zamykający dwa miesiące.
- [podpowiedź] Poproś o wysłanie linków rodzicom jeszcze dziś. Po tej lekcji jest naprawdę co pokazywać.

## [summary] Podsumowanie dwóch miesięcy (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: dlaczego klon musi się usuwać? Czym różni się zmienna wspólna od zmiennej tylko dla tego duszka? Co należy do stanu początkowego?
- Odczytaj listę tego, co dzieci umieją po ośmiu tygodniach: pętla gry, współrzędne, pętle zagnieżdżone, zmienne, warunki, operatory logiczne, kolizje, balans, klony.
- [mów] Osiem tygodni temu część z was widziała Scratcha pierwszy raz. Dziś każde z was ma grę z rosnącą trudnością, strumieniem przeciwników i pełnym cyklem rozgrywki. To nie jest mały postęp.
- [mów] I jedna uwaga na przyszłość: wasze skrypty zaczynają być długie. Niektóre fragmenty powtarzają się w kilku miejscach. To zaczyna przeszkadzać — i to jest temat następnego miesiąca.
- Zajawka: „Od następnych zajęć porządkujemy kod. Poznacie własne bloki, listy i zbudujecie platformówkę z prawdziwą fizyką skoku.”
- Przypomnij zadanie domowe: pokaż grę rodzicom i zapytaj o jedną rzecz do zmiany.

### Wskazówki
- [błąd] Klony zostają na scenie po zakończeniu gry — brak usuwania w stanie początkowym.
- [błąd] Klony przestają powstawać po chwili — przekroczony limit trzystu, bo klony się nie usuwają.
- [błąd] Wszystkie klony mają tę samą prędkość — zmienna wspólna zamiast tylko dla tego duszka.
- [błąd] Kod klonu pod zieloną flagą zamiast pod kapeluszem klonu.
- [błąd] Oryginał widoczny na scenie — brak bloczka ukryj w fabryce klonów.
