# Pokazowa: Złota Piramida - Bezpieczny Zygzak
Rodzaj: pokazowa
Subject: Minecraft Education - MakeCode Blocks
Level: Pokazowa - 10-12 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Blocks, Gra, Warunki, Reszta z dzielenia
Opis: Podczas lekcji zdalnej dziecko uruchamia własnego Minecrafta, a następnie ogląda FINAL tylko na ekranie instruktora. Po pokazie otrzymuje START, odkrywa schody wypełnione samą magmą i w MakeCode Blocks programuje naprzemienny zygzak bezpiecznych pól.
Cel: Dziecko używa reszty z dzielenia przez dwa oraz warunku, aby budować jasne pola raz po lewej, raz po prawej stronie.

### Po zajęciach dziecko potrafi
- rozpoznać liczby parzyste za pomocą operacji modulo
- zbudować dwie gałęzie warunku `jeżeli/w przeciwnym razie`
- wykorzystać zmienne pętli jako współrzędne kolejnych stopni

### Przygotuj przed zajęciami
- na swoim komputerze zaimportuj i sprawdź `projekty/06-Zlota-Piramida-FINAL.mkcd`; przygotuj START, ale nie wysyłaj go przed pokazem
- dziecko najpierw udostępnia cały ekran, uruchamia Minecraft Education, płaski świat i Code Builder; potem instruktor pokazuje FINAL na własnym ekranie
- dopiero po pokazie FINAL dziecku wyślij tylko `projekty/06-Zlota-Piramida-START.mkcd`; import i cała praca odbywają się na ekranie dziecka
- oba projekty otwieraj w MakeCode Blocks; nie przełączaj na Python. Sprawdź `piramida`, `wejscie`, `powrot` i `reset`
- w START odnajdź funkcję `korytarz_pulapek`, pętlę trzech stopni, blok tworzący magmę i komentarz `TU PRACUJE UCZEŃ`
- zachowaj czysty START; FINAL przesyłaj wyłącznie na końcu jako osobny wariant awaryjny po zapisaniu częściowej pracy

### Zadanie domowe
- rozrysuj bezpieczne pola dla sześciu kolejnych stopni

## [intro] Połączenie i Minecraft dziecka (5 min)

### Co robić teraz
- Dziecko udostępnia ekran, uruchamia Minecrafta i MakeCode oraz pokazuje czat. Nie wysyłaj jeszcze projektu.
- Sprawdź, czy dziecko potrafi wrócić z MakeCode do gry i zna miejsce importowania `.mkcd`.
- [mów] Najpierw zobaczysz na moim ekranie gotową zagadkę. Potem w Twoim START schody będą niebezpieczne, dopóki sam nie zaprogramujesz wzoru przejścia.
- Przełącz udostępnianie na ekran instruktora z FINAL, bez pokazywania kodu.

### Wskazówki
- [błąd] Budowa jest liczona względem pozycji gracza; nie przemieszczaj się podczas generowania.
- [podpowiedź] Jeśli dziecko zna liczby parzyste, poproś o przykład; jeśli nie, użyj par skarpet lub kroków 0, 2, 4.
- [tempo] Najpóźniej w 4. minucie rozpocznij pokaz FINAL.

## [demo] FINAL wyłącznie na ekranie instruktora (7 min)

### Co robić teraz
- Instruktor wpisuje `piramida`, potem `wejscie`, i pokazuje strażników, Złotą Pieczęć oraz otwarcie tajnego przejścia.
- Dziecko prowadzi instruktora głosem przez trzy stopnie i zauważa wzór glowstone: lewo, prawo, lewo.
- Poproś o przewidzenie czwartego i piątego pola, gdyby schody były dłuższe. Wróć komendą `powrot`.
- [mów] W START schody z magmy powstaną, lecz zabraknie jasnych pól. Zaprogramujesz regułę, która dla każdego stopnia sama wybierze stronę.
- Po pokazie wyślij tylko START i wróć do udostępnionego ekranu dziecka.

### Wskazówki
- [tempo] Nie trzeba kończyć komnaty faraona podczas pierwszego pokazu.
- [podpowiedź] Nie zdradzaj od razu modulo; najpierw pozwól dziecku opisać zauważony rytm.
- [błąd] Nie pokazuj bloczków FINAL i nie wysyłaj tego pliku razem ze START.

## [concept] Dlaczego START nie ma przejścia? (6 min)

### Co robić teraz
- Dziecko importuje START i uruchamia `reset`, a potem `wejscie`. Widzi trzy stopnie z magmy bez bezpiecznego zygzaka — to celowy brak.
- [mów] Pętla buduje stopnie, ale nie ma jeszcze decyzji, gdzie na każdym z nich postawić bezpieczny blok.
- W MakeCode dziecko odnajduje funkcję `korytarz_pulapek` i komentarz za blokiem tworzącym magmę.
- Dla `i = 0, 1, 2` oblicza resztę z dzielenia przez 2.
- Ustala zasadę: reszta 0 oznacza lewe pole, reszta 1 prawe.

### Materiały
- Zakres pracy dziecka: pełny warunek `jeżeli/w przeciwnym razie`, obliczenie modulo i dwa bloki stawiania glowstone ze zmiennymi współrzędnych.
- Gotowe w START: funkcja, pętla trzech stopni, zmienne `i`, `wysokosc`, `droga` i budowanie magmy.
- [kod] Funkcja „korytarz_pulapek” — warunek zygzaka wewnątrz pętli | MakeCode Blocks:
```text
MIEJSCE: MakeCode Blocks → funkcja „korytarz_pulapek” → pętla trzech stopni → za blokiem magmy
jeżeli (i modulo 2) = 0
  postaw glowstone w punkt(-1, wysokosc, droga)
w przeciwnym razie
  postaw glowstone w punkt(1, wysokosc, droga)
```

### Wskazówki
- [podpowiedź] Rozpiszcie tabelkę w rozmowie: 0 → 0 → lewo, 1 → 1 → prawo, 2 → 0 → lewo.
- [podpowiedź] `wysokosc` i `droga` już wskazują bieżący stopień; zmieniamy tylko x: -1 albo 1.
- [błąd] Warunek musi znaleźć się wewnątrz pętli i po utworzeniu magmy.

## [guided] Budujemy bezpieczną ścieżkę (15 min)

### Co robić teraz
- Całe programowanie wykonuje dziecko w MakeCode na własnym udostępnionym ekranie.
- Po bloku wypełnienia magmą dodaje warunek `jeżeli/w przeciwnym razie` i buduje porównanie `(i modulo 2) = 0`.
- W pierwszej gałęzi stawia glowstone w `punkt(-1, wysokosc, droga)`. Uruchamia test i sprawdza na razie tylko stopnie parzyste.
- Duplikuje blok do drugiej gałęzi, zmienia x z -1 na 1 i testuje cały układ lewo-prawo-lewo.
- Dziecko wskazuje w kodzie, które wartości zmienia pętla, a która liczba odpowiada za stronę.

### Wskazówki
- [podpowiedź] Nie wpisuj wysokości na stałe; zmienna zmienia się na każdym stopniu.
- [błąd] Warunek musi być wewnątrz pętli trzech stopni.
- [błąd] Jeśli wszystkie bloki są po jednej stronie, sprawdź, czy druga gałąź ma x równe 1 i czy istnieje `w przeciwnym razie`.
- [błąd] Jeśli glowstone pojawia się obok schodów, sprawdź kolejność współrzędnych x, y, z oraz nazwy `wysokosc` i `droga`.
- [gdy brakuje czasu] Prowadzący przygotowuje operator modulo, a dziecko buduje dwie gałęzie.

## [challenge] Własny wzór pułapki (10 min)

### Co robić teraz
- Dziecko zamienia glowstone na inny jasny, bezpieczny blok.
- Odwraca zygzak, zamieniając współrzędne -1 oraz 1.
- Przewiduje układ przed uruchomieniem, zapisuje go i dopiero wtedy testuje.
- Wybiera ostateczny wzór oraz uzasadnia, czy początek po lewej czy po prawej jest czytelniejszy dla gracza.

### Wskazówki
- [dla szybszych] Na ostatnim stopniu postaw dodatkowy blok nagrody pośrodku.
- [gdy brakuje czasu] Zmień tylko materiał bezpiecznych pól i pozostaw układ z FINAL.

## [demo] Przejście własnym zygzakiem (10 min)

### Co robić teraz
- Dziecko używa `reset`, potem `wejscie`, otwiera przejście i pokonuje własne schody.
- Sprawdźcie kryteria: trzy stopnie powstają, każdy ma dokładnie jedno bezpieczne pole, strony się zmieniają, wysokość rośnie, a pozostałe elementy piramidy nadal działają.
- Przy błędzie dziecko porównuje przewidywany wzór z wynikiem i wskazuje, czy problem dotyczy warunku, strony czy współrzędnej.
- Nazwijcie rezultat: START odtwarza przejście z FINAL i zawiera wybrany przez dziecko wariant.
- Zapisz projekt pod nową nazwą z imieniem, nie nadpisując czystego START.

### Wskazówki
- [tempo] Test kończy się po wejściu na górne lądowisko; finał piramidy jest opcjonalny.
- [błąd] Po zmianie kodu zbuduj przejście od nowa; istniejące bloki w świecie nie zmienią się automatycznie.

## [summary] Pokaz, zapis i wariant awaryjny (7 min)

### Co robić teraz
- Jeśli START jest ukończony, dziecko przechodzi własnym zygzakiem i wyjaśnia `i modulo 2`. FINAL pozostaje wyłącznie u instruktora.
- [mów] Jedna pętla i warunek zbudowały całą naprzemienną trasę. Twój START ma już bezpieczne przejście widziane w FINAL.
- Zapytaj, po której stronie znajdzie się pole dla `i = 4` i skąd dziecko to wie.
- Gdy zabrakło czasu lub wystąpiła awaria, najpierw zapisz częściowy START z dopiskiem `NIEDOKONCZONE` i wymień działające elementy.
- Dopiero potem wyślij `06-Zlota-Piramida-FINAL.mkcd` jako osobny materiał pomocniczy. Nie nadpisuj nim pracy dziecka i nie przedstawiaj go jako ukończonego projektu.
- Przypomnij nazwę zapisanego pliku oraz zadanie domowe.

### Wskazówki
- [podpowiedź] Rodzicowi wskaż konkret: pętla, modulo, dwie gałęzie i współrzędne zależne od stopnia.
- [błąd] FINAL nie służy do omijania diagnozy pierwszego błędu.
- [tempo] Po 55. minucie nie zmieniaj wzoru; zapisz i pokaż najlepszą działającą wersję.
