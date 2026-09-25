# Pokazowa: Podniebna Baza - Skrzynia Smoka
Rodzaj: pokazowa
Subject: Minecraft Education - MakeCode Python
Level: Pokazowa - 10-12 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Python, Funkcje, Parametry, Przedmioty
Opis: Podczas lekcji zdalnej dziecko najpierw uruchamia własnego Minecrafta, potem ogląda FINAL tylko na ekranie instruktora. Po pokazie otrzymuje START, w którym brakuje całej Skrzyni Smoka, i w MakeCode Python oblicza jej pozycję, stawia skrzynię oraz wyposaża ją w miecz, perły Endu i złote jabłka.
Cel: Dziecko buduje kompletną funkcję lokacji: zapisuje współrzędne w zmiennych, stawia blok skrzyni i trzykrotnie wywołuje funkcję z parametrami slotu, przedmiotu oraz liczby.

### Po zajęciach dziecko potrafi
- wywołać funkcję z sześcioma argumentami w prawidłowej kolejności
- obliczyć pozycję skrzyni względem stałych Wyspy Smoka i użyć jej w kilku instrukcjach
- rozmieścić różne przedmioty w osobnych slotach skrzyni
- zmienić zawartość nagrody bez przebudowy całej lokacji

### Przygotuj przed zajęciami
- na swoim komputerze zaimportuj i przetestuj `projekty/09-Podniebna-Baza-FINAL.mkcd`; START miej gotowy, ale nie wysyłaj go przed pokazem
- dziecko najpierw udostępnia cały ekran, uruchamia Minecraft Education, płaski świat i MakeCode; potem instruktor pokazuje FINAL na własnym ekranie
- dopiero po pokazie FINAL dziecku wyślij tylko `projekty/09-Podniebna-Baza-START.mkcd`; dziecko importuje go i pracuje w MakeCode Python
- nie przełączaj na Blocks; sprawdź `siec`, `powrot`, `adresy`, `reset` oraz żółty portal Wyspy Smoka
- w START odnajdź pustą `skrzynia_wyspy()` z komentarzem `TU PRACUJE UCZEŃ` i `pass`; dziecko tworzy całą jej zawartość, a pozostałych lokacji nie przebudowuje
- zachowaj czysty START; FINAL przesyłaj wyłącznie pod koniec jako osobny wariant awaryjny po zapisaniu częściowej pracy

### Zadanie domowe
- zaprojektuj własny zestaw trzech nagród i przypisz im sloty oraz liczby

## [intro] Połączenie i Minecraft dziecka (5 min)

### Co robić teraz
- Dziecko udostępnia ekran, uruchamia Minecrafta i MakeCode oraz pokazuje czat i ekwipunek. Nie wysyłaj jeszcze projektu.
- Zapytaj, jak komputer może rozróżnić miejsca w skrzyni i co mogłoby się stać, gdy dwa przedmioty dostaną ten sam slot.
- [mów] Najpierw zobaczysz gotową sieć na moim ekranie. Potem w Twoim START Wyspa Smoka będzie miała skrzynię, ale bez nagród.
- Przełącz udostępnianie na ekran instruktora z FINAL, bez odsłaniania kodu.

### Wskazówki
- [błąd] Po teleportacji wracamy komendą `powrot`; nie próbuj lecieć ręcznie między lokacjami.
- [podpowiedź] Udostępniaj cały ekran, aby widzieć import, kod, portal i zawartość skrzyni.
- [tempo] Najpóźniej w 4. minucie rozpocznij pokaz FINAL.

## [demo] FINAL wyłącznie na ekranie instruktora (7 min)

### Co robić teraz
- Instruktor wpisuje `siec`; dziecko głosem wybiera portale. Krótko pokaż niebieską Bazę, zieloną Jaskinię i czerwoną Wieżę.
- W żółtym portalu aktywuj kryształ Wyspy Smoka i otwórz skrzynię. Dziecko odczytuje: slot 0 — miecz ×1, slot 1 — perły Endu ×8, slot 2 — złote jabłka ×3.
- Zapytaj, które informacje musiałby dostać kod, aby włożyć jeden przedmiot w dobre miejsce i liczbie.
- [mów] W START portal zadziała, ale na Wyspie Smoka zabraknie całej skrzyni. Zbudujesz funkcję, która wybierze miejsce, postawi skrzynię i włoży trzy nagrody.
- Po pokazie wyślij tylko START i wróć do udostępnionego ekranu dziecka.

### Wskazówki
- [tempo] Pierwsze trzy lokacje oglądaj po kilkanaście sekund; celem jest Skrzynia Smoka.
- [podpowiedź] Gdy dziecko proponuje własne przedmioty, zapisz pomysł do wyzwania; najpierw odtwórzcie trzy nagrody z FINAL.
- [błąd] Nie pokazuj kodu FINAL i nie przesyłaj go razem ze START.

## [concept] Dlaczego w START nie ma Skrzyni Smoka? (6 min)

### Co robić teraz
- Dziecko importuje START, uruchamia `reset` i `siec`, a następnie przechodzi żółtym portalem. W miejscu nagrody nie ma skrzyni — to oczekiwany brak.
- [mów] Sieć i Wyspa działają. Pusta funkcja `skrzynia_wyspy()` nie oblicza jeszcze miejsca, nie stawia bloku i nie dodaje zawartości.
- Dziecko odnajduje pustą `skrzynia_wyspy()`, komentarz oraz `pass`, który usunie po wpisaniu pierwszych instrukcji.
- Porównajcie wywołania `wloz_do_skrzyni` w bazie, jaskini i wieży.
- Ustalcie pełną kolejność: oblicz `x`, `y`, `z`; postaw skrzynię; dodaj trzy przedmioty. Dziecko wskazuje znaczenie ostatnich trzech argumentów funkcji pomocniczej: slot, przedmiot i ilość.

### Materiały
- Zakres pracy dziecka: cała funkcja — trzy zmienne pozycji, postawienie skrzyni i trzy wywołania z sześcioma argumentami.
- Gotowe w START: pusta funkcja `skrzynia_wyspy()` z komentarzem i `pass`; przykłady wyposażania skrzyń w pozostałych lokacjach.
- [kod] Funkcja „skrzynia_wyspy()” — pozycja, skrzynia i trzy nagrody | Python:
```python
# MIEJSCE: pusta funkcja skrzynia_wyspy(), zamiast komentarza „TU PRACUJE UCZEŃ” i „pass”
def skrzynia_wyspy():
    x = WYSPA_X
    y = WYSPA_Y
    z = WYSPA_Z - 3

    blocks.place(
        CHEST,
        world(x, y, z)
    )

    wloz_do_skrzyni(x, y, z, 0, "minecraft:diamond_sword", 1)
    wloz_do_skrzyni(x, y, z, 1, "minecraft:ender_pearl", 8)
    wloz_do_skrzyni(x, y, z, 2, "minecraft:golden_apple", 3)
```

### Wskazówki
- [podpowiedź] Czytaj linię od lewej: pozycja skrzyni, numer slotu, identyfikator przedmiotu, liczba sztuk.
- [podpowiedź] `WYSPA_Z - 3` przesuwa skrzynię o trzy bloki względem punktu bazowego; wynik zapisujemy raz w `z` i wykorzystujemy cztery razy.
- [podpowiedź] Cudzysłowy są częścią tekstowego identyfikatora, np. `"minecraft:ender_pearl"`.
- [błąd] Wszystkie instrukcje od `x = ...` po trzeci przedmiot muszą mieć wcięcie wewnątrz `skrzynia_wyspy()`.

## [guided] Wkładamy trzy nagrody (15 min)

### Co robić teraz
- Cały kod wpisuje dziecko w MakeCode Python na własnym udostępnionym ekranie.
- Dziecko usuwa `pass`, wpisuje `x = WYSPA_X`, `y = WYSPA_Y` i `z = WYSPA_Z - 3`, a następnie testuje, czy kod nie zgłasza błędu składni.
- Dodaje wieloliniowe `blocks.place(CHEST, world(x, y, z))`. Po `reset` i `siec` sprawdza najpierw samą obecność pustej skrzyni.
- Wpisuje pierwsze wywołanie dla diamentowego miecza w slocie 0 i testuje, czy pojawia się dokładnie jedna sztuka.
- Kopiuje linię, zmienia slot na 1, identyfikator na `minecraft:ender_pearl` i ilość na 8. Wykonuje nowy test.
- Dodaje trzecie wywołanie dla `minecraft:golden_apple`, slotu 2 i liczby 3.
- Po każdej zmianie używa `reset`, potem `siec`, przechodzi żółtym portalem i porównuje zawartość z kodem.

### Wskazówki
- [podpowiedź] Współrzędne `x, y, z` pozostają takie same dla wszystkich slotów tej skrzyni.
- [podpowiedź] Test pustej skrzyni rozdziela dwa problemy: najpierw pozycję i budowę, dopiero potem wyposażenie.
- [podpowiedź] Kopiowanie pierwszej poprawnej linii zmniejsza ryzyko pomyłki w nazwie funkcji i liczbie argumentów.
- [błąd] Dwa przedmioty z tym samym numerem slotu nadpiszą się zamiast leżeć obok siebie.
- [błąd] Jeśli przedmiot nie pojawia się, sprawdź kolejno: przecinki, cudzysłowy, przedrostek `minecraft:` i czy skrzynia została odbudowana po zmianie.
- [błąd] Jeśli liczba sztuk jest zła, zmień ostatni argument, nie numer slotu.
- [gdy brakuje czasu] Obowiązkowe są zmienne pozycji, postawienie skrzyni, miecz i jedna dodatkowa nagroda; trzeci przedmiot można dokończyć przez skopiowanie linii.

## [challenge] Własny zestaw wyprawowy (10 min)

### Co robić teraz
- Dziecko wybiera jedną nagrodę do zamiany oraz ustala jej bezpieczną liczbę.
- Zmienia identyfikator przedmiotu i sprawdza, czy pojawia się w oczekiwanym slocie.
- Nadaje zestawowi nazwę, np. „Ekwipunek Łowcy Smoków”, i uzasadnia, do czego służy każdy element.
- Zmienia tylko jedną linię naraz, aby łatwo wykryć ewentualny błędny identyfikator.

### Wskazówki
- [dla szybszych] Dodaj czwarty przedmiot w slocie 3, nie zmieniając poprzednich linii.
- [gdy brakuje czasu] Pozostaw trzy przedmioty z FINAL i personalizuj jedynie ich liczby.

## [demo] Wyprawa po własny skarb (10 min)

### Co robić teraz
- Dziecko używa `reset`, uruchamia `siec`, samodzielnie przechodzi żółtym portalem i otwiera skrzynię.
- Sprawdźcie kryteria: funkcja oblicza trzy współrzędne, skrzynia powstaje we właściwym miejscu, trzy przedmioty są w osobnych slotach, identyfikatory i liczby zgadzają się z kodem, a pozostałe portale nadal działają.
- Przy błędzie dziecko wskazuje wadliwy slot i porównuje tylko odpowiadającą mu linię, zamiast przepisywać całą funkcję.
- Nazwijcie rezultat: START ma zawartość Skrzyni Smoka z FINAL oraz własną decyzję dotyczącą zestawu.
- Zapisz projekt pod nową nazwą z imieniem, nie nadpisując czystego START.

### Wskazówki
- [tempo] Nie przebudowuj Wyspy Smoka; testujemy wyłącznie zawartość skrzyni.
- [błąd] Otwarta wcześniej skrzynia może pokazywać starą zawartość; po zmianie kodu wykonaj pełny reset i ponowne zbudowanie sieci.

## [summary] Pokaz, zapis i wariant awaryjny (7 min)

### Co robić teraz
- Jeśli START jest ukończony, dziecko pokazuje Skrzynię Smoka oraz trzy wywołania funkcji na własnym ekranie. FINAL pozostaje u instruktora.
- [mów] Zaprogramowałeś nagrodę, przekazując funkcji pozycję, slot, przedmiot i ilość. Twój START odtwarza teraz skrzynię z FINAL.
- Zapytaj, dlaczego każdy przedmiot dostał inny slot i który argument zmienia liczbę sztuk.
- Jeśli zabrakło czasu lub wystąpiła awaria, najpierw zapisz częściowy START z dopiskiem `NIEDOKONCZONE` oraz nazwij poprawnie działające sloty.
- Dopiero potem wyślij `09-Podniebna-Baza-FINAL.mkcd` jako osobny materiał pomocniczy. Nie nadpisuj nim kodu dziecka ani nie przedstawiaj FINAL jako jego pracy.
- Przypomnij nazwę pliku i zadanie domowe.

### Wskazówki
- [podpowiedź] Rodzicowi wskaż konkret: dziecko zbudowało całą funkcję — trzy zmienne pozycji, postawienie skrzyni, trzy wywołania z sześcioma argumentami i iteracyjne testy.
- [błąd] Nie wysyłaj FINAL z powodu jednego błędnego identyfikatora; najpierw porównaj tę jedną linię z działającymi przykładami.
- [tempo] Po 55. minucie nie dodawaj czwartego przedmiotu; zabezpiecz działającą wersję i wykonaj pokaz.
