# Pokazowa: Mega Obby - Patrolujący Laser
Rodzaj: pokazowa
Subject: Scratch
Level: Pokazowa - 8-10 lat
Czas: 60 min
Tags: Pokazowa, Scratch, Gra, Pętle, Ruch, Współrzędne
Opis: Podczas lekcji zdalnej dziecko ogląda Mega Obby FINAL na ekranie instruktora, następnie otrzymuje START i odkrywa `Laser2` bez skryptów. Na własnym udostępnionym ekranie buduje od zera uruchomienie duszka, pozycję początkową, pokazanie, cykliczny patrol oraz reset pozycji po wejściu na poziom 2.
Cel: Dziecko tworzy kompletną obsługę ruchomej przeszkody: reaguje na dwa zdarzenia, ustawia stan początkowy i zamyka dwa płynne ruchy w pętli.

### Po zajęciach dziecko potrafi
- wskazać współrzędne dwóch końców trasy duszka
- zbudować skrypt startowy z pozycją, widocznością i pętlą patrolu
- zareagować na komunikat `POZIOM_2`, ustawiając laser ponownie na początku trasy
- regulować trudność przeszkody czasem ruchu

### Przygotuj przed zajęciami
- na swoim komputerze otwórz i przetestuj `projekty/02-Mega-Obby-FINAL.sb3`; START trzymaj gotowy, ale przed pokazem nie wysyłaj żadnego pliku
- przygotuj przełączanie udostępniania całego ekranu oraz kanał przesyłania `.sb3`
- na początku dziecko udostępnia ekran i uruchamia własny edytor Scratch; potem przełączacie się na ekran instruktora z FINAL
- dopiero po pokazie FINAL dziecku wyślij tylko `projekty/02-Mega-Obby-START.sb3`; FINAL pozostaje u instruktora
- sprawdź oba poziomy, checkpoint, monetę, metę i powrót po trafieniu
- w START sprawdź duszka `Laser2`: ma kostium `laser`, lecz nie ma żadnych bloczków; obok komentarza `TU PRACUJE UCZEŃ` dziecko buduje dwa kompletne skrypty
- w FINAL skrypt zielonej flagi zawiera pozycję `(-25, 10)`, pokazanie i pętlę dwóch przelotów, a drugi skrypt po `POZIOM_2` przywraca pozycję `(-25, 10)`
- FINAL wysyłaj tylko awaryjnie na końcu, po zapisaniu częściowej pracy dziecka, jeżeli START nie został ukończony z powodów czasowych lub technicznych

### Zadanie domowe
- narysuj trasę trzeciego lasera i zapisz współrzędne jej końców

## [intro] Połączenie i ekran dziecka (5 min)

### Co robić teraz
- Dziecko udostępnia cały ekran, uruchamia własny edytor Scratch i pokazuje, gdzie otworzy otrzymany plik `.sb3`. Nie wysyłaj jeszcze START ani FINAL.
- Sprawdź dźwięk, widoczność kursora i działanie strzałek poza polem rozmowy. Zapytaj, czy dziecko zna scenę, duszki i współrzędne `x`, `y`.
- [mów] Za chwilę na moim ekranie zobaczysz gotowy tor. Potem dostaniesz własną wersję, w której jeden laser czeka na Twój kod.
- Dziecko zatrzymuje udostępnianie, a instruktor udostępnia swój ekran z FINAL otwartym na karcie sceny, nie kodu.

### Wskazówki
- [podpowiedź] Przy lekcji Scratch dziecko uruchamia edytor Scratch, nie Minecrafta.
- [tempo] Najpóźniej w 4. minucie przełącz ekran na instruktora.
- [błąd] Nie wysyłaj obu wersji razem. FINAL ma być tylko widocznym wzorcem na ekranie prowadzącego.

## [demo] Dwa poziomy FINAL na ekranie instruktora (7 min)

### Co robić teraz
- Instruktor uruchamia FINAL i steruje na swoim komputerze, a dziecko kieruje próbą głosem. Wyjaśnij, że strzałki przesuwają postać w czterech kierunkach bez grawitacji.
- Przejdź przez obracający laser, aktywuj checkpoint i pokaż powrót do niego po celowym trafieniu.
- Na poziomie 2 zatrzymaj uwagę przy `Laser2`: płynnie jedzie poziomo od `x -25, y 10` do `x 160, y 10` i wraca. Dziecko przewiduje, która liczba pozostaje stała.
- Zbierz monetę i pokaż metę. Nie otwieraj gotowego kodu lasera.
- [mów] W START cały tor będzie działał, ale `Laser2` nie ma żadnych instrukcji. Zbudujesz jego zachowanie od uruchomienia gry aż po reset na poziomie 2.
- Zatrzymaj udostępnianie instruktora, wyślij START i poproś dziecko o ponowne udostępnienie całego ekranu.

### Wskazówki
- [tempo] Gdy tor jest trudny, prowadzący może dokończyć drogę do poziomu 2.
- [błąd] Nie oddawaj dziecku FINAL po pokazie; teraz otrzymuje wyłącznie START.

## [concept] Jakie dwa skrypty musi mieć laser? (6 min)

### Co robić teraz
- Dziecko odbiera START, zapisuje go i samodzielnie otwiera w Scratchu. Uruchamia tor: `Laser2` pozostaje w zapisanym miejscu i nie reaguje ani na zieloną flagę, ani na wejście na poziom 2.
- [mów] Znaleźliśmy różnicę względem FINAL. Duszek i kostium istnieją, ale lista kodu jest pusta — zbudujemy całe zachowanie tej przeszkody.
- Dziecko zatrzymuje grę, wybiera `Laser2` z kostiumem `laser` i odnajduje komentarz `TU PRACUJE UCZEŃ` na pustym obszarze kodu.
- Ustalcie dwa zadania. Skrypt zielonej flagi ustawia pozycję, pokazuje laser i stale patroluje. Skrypt `kiedy otrzymam POZIOM_2` przywraca laser na lewy koniec, gdy gracz wchodzi do tego poziomu.
- Odczytajcie pozycje końców: `x -25, y 10` oraz `x 160, y 10`. Dziecko przewiduje skutki pominięcia każdego z bloków.

### Materiały
- Zakres pracy dziecka: osiem bloczków tworzących od zera dwa kompletne skrypty `Laser2`.
- Gotowe w START: duszek, kostium i komentarz zadania; obszar kodu `Laser2` jest pusty.
- [kod] Duszek „Laser2” — skrypt startowy i reset po „POZIOM_2” | Scratch:
```text
MIEJSCE: duszek „Laser2” → pusty obszar kodu obok komentarza „TU PRACUJE UCZEŃ”
kiedy kliknięto zieloną flagę
idź do x: (-25) y: (10)
pokaż
zawsze
  leć przez (2) sekundy do x: (160) y: (10)
  leć przez (2) sekundy do x: (-25) y: (10)

kiedy otrzymam [POZIOM_2 v]
idź do x: (-25) y: (10)
```

### Wskazówki
- [podpowiedź] `x` opisuje ruch lewo-prawo, a `y` dół-góra. To samo `y = 10` na obu końcach daje poziomą trasę.
- [podpowiedź] Gdy dziecko pyta „Dlaczego nie użyć `idź do x y`?”, wyjaśnij, że `idź` teleportuje natychmiast, a `leć przez` pokazuje ruch i pozwala graczowi zareagować.
- [podpowiedź] Pierwsze `idź do` ustala bezpieczny początek przed uruchomieniem pętli; drugie naprawia pozycję dokładnie w chwili wejścia na poziom 2.
- [błąd] Upewnij się, że ekran dziecka nadal pokazuje Scratch po otwarciu nowego pliku, a nie okno pobierania.

## [guided] Ożywiamy Laser2 (15 min)

### Co robić teraz
- Całe kodowanie odbywa się na udostępnionym ekranie dziecka; ono przeciąga bloki i wpisuje wartości.
- Dziecko przeciąga `kiedy kliknięto zieloną flagę`, a pod nim `idź do x: -25 y: 10` i `pokaż`. Pierwszy test potwierdza tylko start w poprawnym miejscu i widoczność.
- Dodaje pętlę `zawsze`, a w niej pierwszy `leć przez 2 sekundy` do `(160, 10)`. Testuje przejazd w prawo.
- Dodaje drugi przelot do `(-25, 10)` i sprawdza pełny cykl tam i z powrotem.
- Buduje osobny skrypt `kiedy otrzymam POZIOM_2` z `idź do (-25, 10)`. Testuje wejście na poziom 2 po chwili działania lasera i obserwuje reset pozycji.
- Na końcu dziecko nazywa rolę ośmiu bloczków: dwa zdarzenia, dwie pozycje początkowe, pokazanie, pętla oraz dwa ruchy.

### Wskazówki
- [podpowiedź] Oba punkty mają to samo `y`, dlatego ruch jest poziomy.
- [podpowiedź] Gdy dziecko pyta, po co pętla `zawsze`, odpowiedz: „Bez niej dwa przejazdy wykonałyby się raz; pętla tworzy ciągły patrol”.
- [podpowiedź] Jeśli trudno znaleźć komunikat, wybierz Zdarzenia → `kiedy otrzymam` i z listy gotowy `POZIOM_2`; nie twórz nowej wiadomości o podobnej nazwie.
- [błąd] Dwa bloki muszą znajdować się wewnątrz pętli `zawsze`, nie obok niej.
- [błąd] Skrypt resetu jest osobnym stosem. Nie wkładaj go do pętli, bo laser teleportowałby się przy każdym obiegu.
- [błąd] Jeśli laser nie pojawia się po zielonej fladze, sprawdź `pokaż` i czy bloczki są pod właściwym zdarzeniem.
- [błąd] Jeśli laser drży na jednym końcu, sprawdź, czy oba bloki nie mają przypadkiem tych samych współrzędnych.
- [gdy brakuje czasu] Minimum to kompletny skrypt zielonej flagi. Skrypt `POZIOM_2` dodaj po działającym patrolu.

## [challenge] Projektujemy trudność (10 min)

### Co robić teraz
- Dziecko wybiera czas przejazdu od 1 do 3 sekund.
- Wykonuje trzy próby przejścia i ocenia, czy gracz ma czas na reakcję.
- Zmienia jeden koniec trasy o 10-30 punktów, aby laser pilnował innego fragmentu przejścia.

### Wskazówki
- [podpowiedź] Mniejsza liczba sekund oznacza szybszy i trudniejszy laser; większa — wolniejszy i łatwiejszy.
- [dla szybszych] Ustaw różne czasy jazdy w prawo i w lewo.
- [dla szybszych] Zmień jednocześnie `y` obu punktów o tę samą wartość, aby przenieść cały patrol bez przechylania trasy.
- [błąd] Zmieniaj jedną wartość i testuj. Jednoczesna zmiana obu końców oraz czasu utrudnia znalezienie błędu.

## [demo] Pełna próba od checkpointu (10 min)

### Co robić teraz
- Dziecko uruchamia tor od początku i dociera do swojej przeszkody.
- Celowo daje się trafić, sprawdza checkpoint, a potem poprawnie omija laser.
- Sprawdza: poprawną pozycję po fladze, widoczność, płynny ruch, dwa różne końce, stałe `y`, powtarzanie, reset po `POZIOM_2` oraz powrót gracza do checkpointu po trafieniu.
- Zapisz ukończony START pod nową nazwą zawierającą imię dziecka.

### Wskazówki
- [tempo] Jedna udana próba po zmianie wystarczy.
- [błąd] Nie poprawiaj przy okazji checkpointu ani innych laserów. START ma odzyskać mechanizm `Laser2` pokazany w FINAL.

## [summary] Pokaz albo domknięcie awaryjne (7 min)

### Co robić teraz
- Jeśli START jest ukończony, dziecko na swoim ekranie pokazuje ruch lasera, pętlę i dwa punkty trasy. Odpowiada, która liczba odpowiada za kierunek poziomy.
- [mów] Zbudowałeś cykliczną przeszkodę i sam ustawiłeś jej szybkość oraz zasięg. Twój START ma już patrol widziany na początku w FINAL.
- Jeśli zabrakło czasu lub wystąpił problem techniczny, najpierw zapisz częściowy projekt dziecka z dopiskiem `NIEDOKONCZONE`. Dopiero potem wyślij FINAL jako osobny plik do uruchomienia; nie nadpisuj pracy dziecka.
- [mów] Zachowaj oba pliki: w Twoim widać wykonaną pracę, a FINAL jest kompletną wersją pomocniczą do dalszego oglądania.

### Wskazówki
- [podpowiedź] Rodzicowi wskaż konkretnie: dziecko zbudowało od zera osiem bloczków w dwóch skryptach, ustaliło stan początkowy, zamknęło ruch w pętli, obsłużyło komunikat poziomu i dobrało prędkość.
- [błąd] Nie wysyłaj FINAL przy pierwszej trudności. Jest awaryjnym domknięciem po zapisaniu częściowego START.
- [tempo] Od 55. minuty nie dodawaj trzeciej trasy ani nowego lasera.
