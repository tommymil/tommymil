# Pokazowa: Obrona Magicznej Fortecy
Rodzaj: pokazowa
Subject: Minecraft Education — MakeCode Blocks
Level: Pokazowa — 9–11 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Blocks, Gra, Pętle, Fale
Opis: Uczeń broni gotowej fortecy w trzyfalowej grze, a następnie programuje własną falę pająków.
Cel: Uczeń używa pętli do wielokrotnego tworzenia przeciwników i reguluje poziom trudności liczbą oraz odstępem czasu.

### Po zajęciach dziecko potrafi
- wyjaśnić, jak pętla zastępuje kilka identycznych poleceń
- utworzyć falę mobów z bezpiecznych punktów wejścia
- zmienić liczbę przeciwników oraz przerwę między nimi

### Przygotuj przed zajęciami
- płaski świat i projekty `Obrona-Fortecy-start`, `-polprodukt`, `-final`, zapisane w widoku Blocks
- w START pełna forteca i trzy grywalne fale; brak tylko fali pająków ucznia
- skróty instruktorskie `start`, `fala2`, `boss`, `reset`
- poziom trudności Easy, wyłączone niszczenie świata przez moby, łuk i zapas strzał
- test na tym samym koncie Minecraft Education

#### Gotowe w projekcie START
- komenda `start` buduje fortecę z fosą, murami, czterema wieżami, bramą i punktem obrony
- gracz dostaje łuk, miecz, strzały, jedzenie oraz przechodzi do survival
- trzy fale zombie i szkieletów wchodzą z różnych stron, między falami działa odliczanie
- komunikaty, nocny klimat, punkt naprawy, warunek zwycięstwa i restart
- boss pojawia się po trzeciej fali; forteca pozostaje grywalna bez modu ucznia

#### Mały mod uczestnika
Dodatkowa fala 4: 3–6 pająków pojawia się z bocznej bramy. Uczeń buduje jedną pętlę, wybiera liczbę mobów i czas między spawnami.

### Zadanie domowe
- zaprojektuj falę 5: wybierz moba, liczbę, miejsce wejścia i zasadę ostrzeżenia

## [intro] Start i bufor techniczny (7 min)

### Co robić teraz
- Sprawdź Code Builder, czat, sterowanie, łuk i komendę `reset`.
- [mów] Forteca i cała gra już istnieją. Dzisiaj zostaniesz projektantem fal i zdecydujesz, jak zaatakują nowe potwory.
- Ustal, czy uczeń zna pętlę „powtórz”.

## [demo] Grywalny szturm WOW (5 min)

### Co robić teraz
- Uruchom `start`. Uczeń broni bramy podczas skróconej fali 1.
- Pokaż ostrzeżenie, spawn z dwóch stron, trafienie z łuku i 10 sekund bossa przez skrót.
- [mów] Kod zbudował arenę, przygotował ekwipunek i steruje tempem ataku. Ty dodasz nową falę.

### Wskazówki
- [tempo] Demo ma pokazać akcję, nie zwiedzanie każdej wieży.
- [podpowiedź] Bezpieczeństwo: Creeper nie należy do obowiązkowej fali; mob griefing pozostaje wyłączony.

## [concept] Pętla jako generator fali (6 min)

### Co robić teraz
- Pokaż gotową funkcję `fala_zombie` w Blocks i zasłoń pozostały kod.
- Uczeń przewiduje różnicę między `powtórz 3` i `powtórz 8`.
- Wyjaśnij, że pauza rozdziela pojawienie się mobów; bez niej cała fala powstanie naraz.

### Materiały
- [kod] Wzorzec fali | MakeCode Blocks:
```text
funkcja fala_pajakow
  powtórz (4) razy
    stwórz [pająk] w pozycji (8, 0, 6)
    pauza (900) ms
  powiedz „Fala pająków na arenie!”
```

## [guided] Misja: fala pająków (17 min)

### Co robić teraz
- Uczeń duplikuje małą funkcję istniejącej fali i nazywa ją `fala_pajakow`.
- Zmienia moba na pająka, pozycję na boczną bramę i liczbę powtórzeń na 4.
- Dodaje pauzę 900 ms i komunikat ostrzegawczy przed pętlą.
- Podłącza funkcję do gotowego miejsca `MOD_UCZNIA` po fali 3.
- Testuje najpierw w Creative, potem w Easy Survival.

### Wskazówki
- [błąd] Potwory nie pojawią się na Peaceful.
- [błąd] Punkt spawnu nie może leżeć w murze ani przy graczu.
- [gdy brakuje czasu] Otwórz półprodukt; uczeń wybiera moba, liczbę i pauzę.
- [dla szybszych] Co drugi pająk pojawia się z przeciwnej bramy.

## [challenge] Personalizacja i balans (13 min)

### Co robić teraz
- Uczeń wybiera: 3–6 pająków, pauzę 600–1400 ms, porę ataku i treść ostrzeżenia.
- Rozegrajcie falę dwa razy. Jeśli gracz nie może zareagować, zwiększcie pauzę lub odległość.
- Nazwijcie falę, np. „Nocny Rój”.

### Wskazówki
- [podpowiedź] Więcej mobów nie zawsze oznacza lepszą grę; czytelne tempo robi większe wrażenie.

## [challenge] Samodzielny krok (6 min)

### Co robić teraz
- Uczeń sam dodaje jednego szkieleta jako dowódcę po zakończeniu pętli albo zmienia stronę wejścia.
- Zapisz stabilną wersję i przywróć fortecę komendą `reset`.

## [summary] Finał dla rodzica (6 min)

### Co robić teraz
- Uczeń uruchamia pełną falę 4, broni bramy i pokazuje pętlę.
- [mów] Jedna pętla stworzyła całą falę, a Ty ustawiłeś jej tempo i trudność.
- W finałowej wypowiedzi nazwij własny wkład ucznia, nie gotową fortecę.
