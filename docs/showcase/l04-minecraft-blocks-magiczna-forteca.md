# Pokazowa: Magiczna Forteca - Własna Fala
Rodzaj: pokazowa
Subject: Minecraft Education - MakeCode Blocks
Level: Pokazowa - 9-11 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Blocks, Gra, Funkcje, Parametry
Opis: Podczas lekcji zdalnej dziecko najpierw uruchamia własnego Minecrafta i udostępnia ekran, a następnie ogląda działający FINAL wyłącznie na ekranie instruktora. Po pokazie otrzymuje START i w swoim MakeCode Blocks programuje falę czterech zombie oraz dwóch szkieletów atakujących główną bramę.
Cel: Dziecko wywołuje gotową funkcję z parametrami, aby ustalić typ, liczbę, miejsce i tempo pojawiania się przeciwników.

### Po zajęciach dziecko potrafi
- odnaleźć funkcję wywoływaną w głównej sekwencji gry
- przekazać do funkcji typ moba, liczbę, pozycję i odstęp czasu
- przetestować falę osobną komendą bez rozgrywania całej gry

### Przygotuj przed zajęciami
- na swoim komputerze zaimportuj i przetestuj `projekty/04-Magiczna-Forteca-FINAL.mkcd`; START miej gotowy, lecz nie wysyłaj przed pokazem
- przygotuj Minecraft Education, płaski świat, Code Builder/MakeCode oraz przełączanie udostępniania całego ekranu
- na początku dziecko udostępnia ekran, uruchamia własnego Minecrafta i otwiera MakeCode; potem instruktor pokazuje FINAL na swoim ekranie bez odsłaniania bloczków
- dopiero po pokazie FINAL dziecku wyślij tylko `projekty/04-Magiczna-Forteca-START.mkcd`; dziecko samodzielnie importuje go w MakeCode Blocks
- oba projekty otwieraj w Blocks; nie przełączaj na Python. Sprawdź komendy `start`, `fala`, `boss` oraz `reset`
- w START wskaż funkcję `fala`, pusty warunek z komentarzem i gotową funkcję `spawnuj`; zadanie nie polega na przebudowie fortecy
- zachowaj czysty START; FINAL wolno przesłać dopiero na końcu jako osobny plik awaryjny po zapisaniu częściowej pracy dziecka

### Zadanie domowe
- zaprojektuj drugą falę: wybierz dwa moby, ich liczbę oraz odstęp czasu

## [intro] Połączenie i Minecraft dziecka (5 min)

### Co robić teraz
- Dziecko udostępnia cały ekran, uruchamia Minecraft Education, wchodzi do płaskiego świata i otwiera Code Builder. Sprawdź czat, klawiaturę oraz powrót z MakeCode do gry.
- Na tym etapie nie wysyłaj żadnego projektu. Zapytaj, czy dziecko używało komend czatu i funkcji w MakeCode.
- [mów] Najpierw zobaczysz gotową fortecę na moim ekranie. Potem dostaniesz START, w którym obrona nie ma jeszcze zaprogramowanego składu pierwszej fali.
- Dziecko zatrzymuje udostępnianie, a instruktor pokazuje swój Minecraft z FINAL.

### Wskazówki
- [podpowiedź] Przy lekcji zdalnej udostępniaj cały ekran, bo test wymaga przechodzenia między Minecraftem, czatem i MakeCode.
- [błąd] Komendy wpisuje się na czacie Minecrafta, nie w polu wyszukiwania bloczków.
- [tempo] Kontrolę techniczną zakończ najpóźniej w 4. minucie.

## [demo] FINAL wyłącznie na ekranie instruktora (7 min)

### Co robić teraz
- Instruktor wpisuje `start`, a dziecko kieruje obroną głosem. Pokaż otwarcie bramy, tytuł `FALA`, cztery zombie z lewej i dwa szkielety z prawej.
- Zwróć uwagę na to, że przeciwnicy nie pojawiają się wszyscy naraz: odstępy dają graczowi czas na reakcję.
- Pokaż skrótem `boss` dalszy ciąg gry, a potem użyj `reset`. Nie otwieraj gotowych bloczków.
- [mów] W START forteca, brama i boss pozostaną gotowe, ale pierwsza fala będzie pusta. Zaprogramujesz jej skład, położenie i tempo.
- Po pokazie zatrzymaj własne udostępnianie, wyślij tylko START i poproś dziecko o ponowne udostępnienie ekranu.

### Wskazówki
- [podpowiedź] Gdy dziecko pyta, czy może dodać creepera, zapowiedz, że własny skład wybierze w wyzwaniu po odtworzeniu bezpiecznej fali.
- [tempo] Nie zwiedzaj wszystkich wież; najważniejszy jest moment pojawienia się fali.
- [błąd] Dziecko nie otrzymuje FINAL ani widoku gotowego kodu.

## [concept] Brakująca fala w START (6 min)

### Co robić teraz
- Dziecko importuje START w MakeCode Blocks, wraca do Minecrafta i uruchamia `fala`. Tytuł może się pojawić, lecz oczekiwani przeciwnicy nie nadchodzą — to zaplanowany brak.
- [mów] START działa. Brakuje tylko instrukcji mówiących funkcji `spawnuj`, kogo, ilu, gdzie i w jakim tempie ma tworzyć.
- Dziecko otwiera funkcję `fala` i odnajduje pusty warunek z komentarzem `TU PRACUJE UCZEŃ`.
- Pokaż gotową funkcję `spawnuj` oraz kolejność parametrów: mob, liczba, x, z, odstęp, id.
- Dziecko przewiduje różnicę między czterema mobami co 750 ms i czterema naraz.

### Materiały
- Zakres pracy dziecka: tytuł, pauza i dwa wywołania `spawnuj`, każde z sześcioma świadomie dobranymi parametrami.
- Gotowe w START: funkcja `fala`, warunek ważności gry oraz cała funkcja pomocnicza `spawnuj`.
- [kod] Funkcja „fala” — skład przeciwników w gotowym warunku | MakeCode Blocks:
```text
MIEJSCE: MakeCode Blocks → funkcja „fala” → wnętrze warunku „graTrwa i id = numerGry”
jeżeli graTrwa i id = numerGry
  pokaż tytuł „FALA” / „Obroń główną bramę!”
  pauza 2500 ms
  wywołaj spawnuj(zombie, 4, -2, -11, 750, id)
  wywołaj spawnuj(szkielet, 2, 2, -11, 900, id)
```

### Wskazówki
- [podpowiedź] `id` nie jest numerem przeciwnika; to numer bieżącej gry, który zatrzymuje spóźnione zdarzenia po resecie.
- [podpowiedź] Poproś dziecko, by przed kodowaniem wskazało, który argument odpowiada za liczbę, a który za tempo.
- [błąd] Nie edytuj wnętrza gotowej funkcji `spawnuj`; dodajemy jej dwa wywołania w `fala`.

## [guided] Odtwarzamy falę z FINAL (15 min)

### Co robić teraz
- Całe programowanie odbywa się w MakeCode na udostępnionym ekranie dziecka; ono przeciąga bloczki i wpisuje liczby.
- Dziecko dodaje do pustego warunku tytuł `FALA`, podtytuł `Obroń główną bramę!` i pauzę 2500 ms. Testuje, czy komunikat pojawia się przed atakiem.
- Wstawia wywołanie `spawnuj(zombie, 4, -2, -11, 750, id)`. Wraca do gry, wpisuje `fala` i liczy zombie.
- Dodaje `spawnuj(skeleton, 2, 2, -11, 900, id)`, wykonuje `reset` i ponownie testuje. Dziecko wskazuje lewą oraz prawą stronę bramy.
- Po każdym teście wraca do tej samej funkcji, zamiast szukać kodu od początku.

### Wskazówki
- [podpowiedź] Parametr `id` zostaje bez zmian; chroni grę przed spóźnioną falą po resecie.
- [podpowiedź] Czytaj wywołanie jak zdanie: „stwórz zombie, 4 sztuki, w x -2 i z -11, co 750 ms, dla tej gry”.
- [błąd] Pozycja `z = -11` leży przed bramą; zmiana znaku może przenieść moby do fortecy.
- [błąd] Jeśli widzisz właściwy mob, ale tylko jedną sztukę, sprawdź drugi argument. Jeśli nic się nie pojawia, sprawdź `graTrwa`, `id` i czy test rozpoczął nową grę.
- [gdy brakuje czasu] Zbudujcie obowiązkowo zombie; szkielety mogą zostać bonusem.

## [challenge] Własny skład ataku (10 min)

### Co robić teraz
- Dziecko wybiera od 3 do 6 zombie i od 1 do 3 szkieletów.
- Ustawia odstępy od 600 do 1200 ms.
- Przed uruchomieniem przewiduje, z której strony nadejdzie trudniejsza grupa, po czym testuje falę dwa razy.
- Uzasadnia balans jednym zdaniem: czy gracz ma czas zareagować po otwarciu bramy.

### Wskazówki
- [dla szybszych] Zmień jednego szkieleta na pająka, pozostawiając bezpieczny punkt pojawienia.
- [gdy brakuje czasu] Zmień tylko liczbę zombie i pozostaw wartości z FINAL dla pozostałych parametrów.

## [demo] Pełna obrona własnej fali (10 min)

### Co robić teraz
- Dziecko wpisuje `reset`, potem `start` i broni fortecy przed swoją falą.
- Sprawdźcie: tytuł poprzedza atak, są dwa typy mobów, liczby oraz strony są zgodne z kodem, odstępy są widoczne, a po fali nadal może uruchomić się boss.
- Jeśli wynik jest błędny, dziecko najpierw wskazuje konkretny parametr do sprawdzenia, a dopiero potem wraca do MakeCode.
- Nazwijcie rezultat: START ma już funkcjonalną falę z FINAL oraz decyzję projektową dziecka.
- Zapisz projekt pod nową nazwą zawierającą imię dziecka, bez nadpisywania czystego START.

### Wskazówki
- [tempo] Jeżeli walka trwa za długo, przejdź do trybu Creative dopiero po pokazaniu działania fali.
- [błąd] Po zmianie kodu uruchom ponownie właściwą komendę; stara fala w świecie nie aktualizuje się automatycznie.

## [summary] Pokaz, zapis i wariant awaryjny (7 min)

### Co robić teraz
- Jeśli START jest ukończony, dziecko na własnym ekranie uruchamia `fala`, pokazuje dwa wywołania funkcji i wyjaśnia argument liczby oraz odstępu. FINAL pozostaje u instruktora.
- [mów] Ustawiłeś skład, miejsce i tempo ataku za pomocą parametrów funkcji. Twój START odtwarza teraz falę pokazaną w FINAL.
- Jeżeli zabrakło czasu albo wystąpiła awaria, najpierw zapisz częściową pracę pod nazwą z dopiskiem `NIEDOKONCZONE` i nazwij działające elementy.
- Dopiero po tym wyślij `04-Magiczna-Forteca-FINAL.mkcd` jako osobny materiał do uruchomienia. Nie nadpisuj nim pracy dziecka i nie przedstawiaj go jako ukończonego START.
- Przypomnij nazwę zapisanego pliku i zadanie domowe.

### Wskazówki
- [podpowiedź] Rodzicowi wskaż konkret: dziecko użyło funkcji z sześcioma parametrami, przetestowało dwa typy mobów i dobrało tempo.
- [błąd] FINAL nie jest pomocą przy pierwszym błędzie, tylko awaryjnym domknięciem lekcji.
- [tempo] Od 55. minuty nie rozbudowuj fali; zapisz projekt i przejdź do pokazu.
