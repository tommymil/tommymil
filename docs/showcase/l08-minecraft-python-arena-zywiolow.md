# Pokazowa: Arena Żywiołów - Lodowa Tarcza
Rodzaj: pokazowa
Subject: Minecraft Education - MakeCode Python
Level: Pokazowa - 10-12 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Python, Gra, Współrzędne, Energia
Opis: Podczas lekcji zdalnej dziecko najpierw uruchamia własnego Minecrafta, a potem ogląda FINAL tylko na ekranie instruktora. Po pokazie otrzymuje START, sprawdza niebieską runę bez widocznej bariery i w MakeCode Python programuje lodową ścianę z dwiema lampami.
Cel: Dziecko buduje trójwymiarową konstrukcję względem centrum areny i łączy ją z gotowym kosztem energii oraz czasem działania.

### Po zajęciach dziecko potrafi
- wypełnić prostopadłościan blokami między dwiema pozycjami
- dodać pojedyncze bloki jako oznaczenia krawędzi
- dobrać rozmiar i czas bariery do kosztu energii

### Przygotuj przed zajęciami
- na swoim komputerze zaimportuj i przetestuj `projekty/08-Arena-Zywiolow-FINAL.mkcd`; START przygotuj, lecz nie wysyłaj przed pokazem
- dziecko najpierw udostępnia cały ekran, uruchamia Minecraft Education, płaski świat i MakeCode; potem instruktor pokazuje FINAL na własnym ekranie
- dopiero po pokazie FINAL dziecku wyślij tylko `projekty/08-Arena-Zywiolow-START.mkcd`; dziecko importuje go i pracuje w MakeCode Python
- nie przełączaj na Blocks; sprawdź `arena`, `start`, `energia`, `fala`, `boss`, `balkon` i `reset`
- w START odnajdź funkcję `moc_lodu()` oraz komentarz po gotowym tytule; koszt energii i usuwanie bariery pozostają gotowe
- zachowaj czysty START; FINAL przesyłaj tylko na końcu jako osobny wariant awaryjny po zapisaniu częściowej pracy dziecka

### Zadanie domowe
- zaprojektuj inną szerokość tarczy i podaj dwie pozycje narożników

## [intro] Połączenie i środowisko dziecka (5 min)

### Co robić teraz
- Dziecko udostępnia ekran, uruchamia Minecrafta i MakeCode oraz sprawdza czat. Nie wysyłaj jeszcze projektu.
- Zapytaj, jak można opisać ścianę w kodzie: ustawiając każdy blok osobno czy podając dwa przeciwległe narożniki.
- [mów] Na moim ekranie zobaczysz cztery gotowe moce. Potem w Twoim START niebieska runa zużyje energię, ale nie zbuduje tarczy, dopóki nie dopiszesz konstrukcji.
- Przełącz udostępnianie na ekran instruktora z FINAL bez pokazywania kodu.

### Wskazówki
- [błąd] Moc uruchamia wejście na runę; aby użyć jej ponownie, trzeba zejść i wejść jeszcze raz.
- [podpowiedź] Udostępniaj cały ekran, aby widzieć przejście między Pythonem a areną.
- [tempo] Najpóźniej w 4. minucie rozpocznij pokaz FINAL.

## [demo] FINAL wyłącznie na ekranie instruktora (7 min)

### Co robić teraz
- Instruktor uruchamia `arena` i `start`, a dziecko głosem wybiera kolejność run. Krótko pokaż burzę, wiatr i odnowę.
- Na niebieskiej runie zatrzymaj uwagę na koszcie 20 energii, ścianie 5 × 3 bloków, dwóch lampach w górnych rogach i automatycznym zniknięciu.
- Użyj `energia`, jeśli potrzebny jest ponowny test. Zapytaj, które wymiary pozostają stałe, a które określają szerokość.
- [mów] W START system energii i czasu już działa, ale tarcza będzie niewidoczna. Zbudujesz ją trzema instrukcjami.
- Po pokazie wyślij tylko START i wróć do udostępnionego ekranu dziecka.

### Wskazówki
- [tempo] Nie trzeba kończyć wszystkich fal; celem demo jest porównanie czterech run.
- [podpowiedź] Gdy dziecko pyta, czemu tarcza znika, wyjaśnij, że gotowy system sprząta konstrukcję po czasie; jego zadaniem jest jej zbudowanie.
- [błąd] Nie pokazuj gotowej funkcji FINAL i nie przesyłaj FINAL razem ze START.

## [concept] Dlaczego runa w START nic nie buduje? (6 min)

### Co robić teraz
- Dziecko importuje START, uruchamia arenę, uzupełnia energię i wchodzi na niebieską runę. Energia spada i moc się kończy, ale ściany nie widać — to oczekiwany brak.
- [mów] Obsługa runy działa. Brakuje tylko bloków, które wypełnią przestrzeń lodem i postawią lampy.
- Dziecko odnajduje `moc_lodu()` i komentarz po komunikacie tytułowym.
- Odczytajcie narożniki `(-2, 0, 2)` oraz `(2, 2, 2)` względem centrum.
- Dziecko oblicza szerokość 5, wysokość 3 i grubość 1 blok.

### Materiały
- Zakres pracy dziecka: jedno wieloargumentowe wypełnienie bryły oraz dwa osobne ustawienia lamp, łącznie z sześcioma współrzędnymi narożników.
- Gotowe w START: sprawdzanie energii i stanu, pobranie kosztu, tytuł, licznik czasu bariery oraz jej późniejsze usuwanie.
- [kod] Funkcja „moc_lodu()” — budowa Lodowej Tarczy | Python:
```python
# MIEJSCE: funkcja moc_lodu(), po tytule „LODOWA TARCZA!”, przed ustawieniem czas_barykady
blocks.fill(
    PACKED_ICE,
    punkt(-2, 0, 2),
    punkt(2, 2, 2),
    FillOperation.REPLACE
)
blocks.place(GLOWSTONE, punkt(-2, 2, 2))
blocks.place(GLOWSTONE, punkt(2, 2, 2))
```

### Wskazówki
- [podpowiedź] Zakres od -2 do 2 zawiera pięć pozycji: -2, -1, 0, 1, 2.
- [podpowiedź] W obu narożnikach z wynosi 2, dlatego konstrukcja ma grubość jednego bloku.
- [błąd] `punkt(...)` liczy pozycję względem centrum areny; nie zastępuj go bezpośrednim `world(...)`.

## [guided] Budujemy Lodową Tarczę (15 min)

### Co robić teraz
- Cały kod wpisuje dziecko w MakeCode Python na własnym udostępnionym ekranie.
- Dziecko tworzy `blocks.fill` z `PACKED_ICE`, dwoma wywołaniami `punkt` i `FillOperation.REPLACE`. Po domknięciu nawiasów testuje samą ścianę.
- Jeśli ściana ma właściwy rozmiar, dodaje `blocks.place(GLOWSTONE, punkt(-2, 2, 2))` i analogiczną lampę w `(2, 2, 2)`.
- Uruchamia `reset`, `arena`, `energia` i wchodzi na niebieską runę. Sprawdza budowę, dwie lampy oraz usunięcie po około 2,5 sekundy.
- Dziecko wskazuje, która instrukcja buduje wiele bloków, a które stawiają pojedyncze znaczniki.

### Wskazówki
- [podpowiedź] W obu narożnikach `z = 2`, dlatego powstaje ściana, a nie sześcian.
- [podpowiedź] Przy kodzie wieloliniowym najpierw wpisz całą strukturę `blocks.fill(...)`, a potem uzupełniaj argumenty.
- [błąd] Przecinki i nawiasy muszą domykać każde wywołanie przed uruchomieniem gry.
- [błąd] Jeśli ściana jest pod ziemią lub za wysoka, sprawdź wartości y: od 0 do 2.
- [błąd] Jeśli energia znika, ale konstrukcji nie ma, najpierw sprawdź błąd składni i wcięcie nowych linii wewnątrz `moc_lodu()`.
- [gdy brakuje czasu] Sama ściana lodu jest obowiązkowa; lampy są drugim krokiem.

## [challenge] Własny kształt i czas (10 min)

### Co robić teraz
- Dziecko wybiera szerokość 3, 5 albo 7 bloków i zmienia współrzędne x symetrycznie.
- Zmienia `czas_barykady` od 30 do 70 kroków skanera.
- Przed testem zapisuje spodziewane wartości narożników, a potem sprawdza, czy ściana znika i nie zamyka gracza po złej stronie.
- Uzasadnia, czy większa tarcza powinna kosztować więcej energii, nawet jeśli gotowego kosztu nie zmienia.

### Wskazówki
- [dla szybszych] Zmień jedną lampę na `SEA_LANTERN` albo dodaj trzecią pośrodku.
- [gdy brakuje czasu] Pozostaw rozmiar 5 × 3 i personalizuj tylko rodzaj jednej lampy.

## [demo] Fala z własną tarczą (10 min)

### Co robić teraz
- Dziecko uruchamia test fali, czeka na przeciwników i aktywuje swoją runę.
- Sprawdźcie kryteria: energia spada o 20, ściana ma ustalone wymiary i położenie, lampy są na górnych rogach, a konstrukcja automatycznie znika.
- Przy błędzie dziecko rozdziela problem na materiał, narożniki, lampy albo czas i testuje jedną poprawkę.
- Nazwijcie rezultat: START ma Lodową Tarczę z FINAL oraz własny wybór rozmiaru, czasu lub oświetlenia.
- Zapisz projekt pod nazwą z imieniem, bez nadpisywania czystego START.

### Wskazówki
- [tempo] Jedna aktywacja oraz jedno poprawne zniknięcie wystarczą.
- [błąd] Aby ponowić moc, uzupełnij energię i zejdź z runy przed ponownym wejściem.

## [summary] Pokaz, zapis i wariant awaryjny (7 min)

### Co robić teraz
- Jeśli START jest ukończony, dziecko pokazuje tarczę na własnym ekranie i wyjaśnia, jak dwa narożniki określają jej wymiary. FINAL pozostaje u instruktora.
- [mów] Połączyłeś kod budowania z systemem energii i czasem działania. Twój START odtwarza teraz niebieską moc z FINAL.
- Zapytaj, którą współrzędną trzeba zmienić, aby tarcza była szersza, wyższa albo grubsza.
- Jeśli zabrakło czasu lub wystąpiła awaria, najpierw zapisz częściowy START z dopiskiem `NIEDOKONCZONE` i nazwij działające elementy.
- Dopiero wtedy wyślij `08-Arena-Zywiolow-FINAL.mkcd` jako osobny materiał. Nie nadpisuj nim kodu dziecka i nie przedstawiaj FINAL jako jego ukończonej pracy.
- Przypomnij nazwę pliku i zadanie domowe.

### Wskazówki
- [podpowiedź] Rodzicowi wskaż konkret: bryła z dwóch narożników, dwa pojedyncze bloki i test współpracy z gotowym systemem energii.
- [błąd] Nie wysyłaj FINAL przy pierwszym błędzie składni; pomóż domknąć wywołania i przetestować samą ścianę.
- [tempo] Po 55. minucie nie zmieniaj wymiarów; zapisz stabilną wersję i wykonaj pokaz.
