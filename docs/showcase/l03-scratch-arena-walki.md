# Pokazowa: Arena Walki - Super Atak 360 stopni
Rodzaj: pokazowa
Subject: Scratch
Level: Pokazowa - 10-12 lat
Czas: 60 min
Tags: Pokazowa, Scratch, Gra, Klony, Pętle, Kąty
Opis: Podczas lekcji zdalnej dziecko ogląda Super Atak FINAL na ekranie instruktora, a następnie otrzymuje START bez serii pocisków. Na własnym udostępnionym ekranie programuje osiem klonów rozłożonych równomiernie na pełnych 360 stopniach.
Cel: Dziecko łączy pętlę, klony i obrót o stały kąt, aby równomiernie wypełnić pełne 360 stopni.

### Po zajęciach dziecko potrafi
- utworzyć wiele pocisków jako klony jednego duszka
- obliczyć kąt `360 / liczba pocisków`
- uruchomić osobny skrypt za pomocą komunikatu

### Przygotuj przed zajęciami
- na swoim komputerze otwórz `projekty/03-Arena-Walki-FINAL.sb3`; START trzymaj gotowy, ale przed pokazem nie wysyłaj plików
- przygotuj przełączanie udostępniania całego ekranu i kanał przesłania `.sb3`
- najpierw dziecko udostępnia ekran i uruchamia własny Scratch, następnie instruktor pokazuje FINAL wyłącznie na swoim ekranie
- dopiero po pokazie dziecku wyślij tylko `projekty/03-Arena-Walki-START.sb3`; FINAL pozostaje u instruktora
- sprawdź celowanie myszką, strzał spacją, ładowanie paska Super i klawisz E
- w START sprawdź duszka `Pocisk`, zdarzenie `SUPER_ATAK`, warunek `czyKlon = 0` i dokładnie jeden komentarz `TU PRACUJE UCZEŃ`
- FINAL przekaż tylko na końcu jako zabezpieczenie, jeśli po zapisaniu częściowej pracy START nadal nie działa z powodów czasowych lub technicznych

### Zadanie domowe
- oblicz kąt obrotu dla 6, 10 i 12 pocisków

## [intro] Połączenie i przygotowanie Scratcha (5 min)

### Co robić teraz
- Dziecko udostępnia cały ekran, uruchamia własny edytor Scratch i pokazuje działanie myszy oraz klawiatury. Nie wysyłaj jeszcze projektu.
- Zapytaj, czy zna pojęcia klonu, kąta i pełnego obrotu. Nie wymagaj definicji — odpowiedzi służą do dobrania tempa.
- [mów] Na moim ekranie zobaczysz za chwilę atak we wszystkich kierunkach. Potem dostaniesz wersję, w której sam zbudujesz jego geometrię.
- Przełącz udostępnianie na ekran instruktora z FINAL otwartym na scenie, bez pokazywania kodu.

### Wskazówki
- [tempo] Najpóźniej w 4. minucie przejdź do ekranu instruktora.
- [błąd] Nie wysyłaj teraz START ani FINAL; dziecko ma najpierw zobaczyć rezultat bez dostępu do rozwiązania.

## [demo] Super Atak FINAL na ekranie instruktora (7 min)

### Co robić teraz
- Instruktor uruchamia FINAL, celuje myszką i strzela spacją. Dziecko kieruje walką głosem i obserwuje pasek `Super`.
- Po osiągnięciu 100 instruktor naciska E. Dziecko liczy osiem pocisków i przewiduje, jak podzielić 360 stopni na osiem równych kierunków.
- Pokaż, że zwykły strzał leci do kursora, a Super Atak rozchodzi się dookoła gracza niezależnie od kursora.
- Pokonaj obu przeciwników i pokaż komunikat zwycięstwa. Nie otwieraj kodu `Pocisk`.
- [mów] W START pasek nadal się naładuje i klawisz E zadziała, ale nie powstanie pierścień pocisków. To będzie Twój fragment.
- Zakończ własne udostępnianie, wyślij START i poproś dziecko o ponowne udostępnienie całego ekranu.

### Wskazówki
- [tempo] Nie analizuj ruchu przeciwników; dzisiejszy temat to duszek `Pocisk`.
- [błąd] Super Atak działa dopiero przy pasku 100; brak reakcji wcześniej nie oznacza błędu projektu.
- [błąd] Dziecko otrzymuje teraz tylko START, nie parę plików.

## [concept] Pełne koło z pętli (6 min)

### Co robić teraz
- Dziecko odbiera i otwiera START na swoim ekranie. Uruchamia grę, ładuje pasek i naciska E: komunikat jest wysyłany, ale pierścień pocisków nie powstaje.
- [mów] START potwierdził brak: sterowanie, pasek i zdarzenie działają, lecz pusty warunek nie tworzy klonów Super Ataku.
- Dziecko zatrzymuje grę, wybiera duszka `Pocisk` z kostiumem `pocisk` i odnajduje zdarzenie `kiedy otrzymam SUPER_ATAK` oraz gotowy warunek `czyKlon = 0` z komentarzem.
- Oblicza `360 / 8 = 45`.
- Układa ustnie algorytm: utwórz klon, obróć o 45 stopni, powtórz osiem razy.

### Materiały
- Zakres pracy dziecka: sześć głównych bloczków wewnątrz gotowego warunku — typ pocisku, pozycja, kierunek, pętla, klonowanie i obrót.
- Gotowe w START: zdarzenie `SUPER_ATAK`, warunek `czyKlon = 0` oraz działanie pojedynczych klonów pocisku.
- [kod] Duszek „Pocisk” — obsługa komunikatu „SUPER_ATAK” | Scratch:
```text
MIEJSCE: duszek „Pocisk” → skrypt „kiedy otrzymam SUPER_ATAK” → wnętrze warunku „czyKlon = 0”
kiedy otrzymam [SUPER_ATAK v]
jeżeli <(czyKlon) = (0)> to
  ustaw [typPocisku v] na (1)
  idź do [Gracz v]
  ustaw kierunek na (0)
  powtórz (8) razy
    utwórz klona z [siebie v]
    obróć w prawo o (45) stopni
```

### Wskazówki
- [podpowiedź] `czyKlon = 0` oznacza duszka wzorcowego. Dzięki temu odebranie komunikatu nie każe każdemu istniejącemu klonowi tworzyć kolejnych klonów.
- [podpowiedź] Gdy dziecko pyta „Dlaczego 45?”, narysuj lub opisz pełne koło: osiem równych części po 45 stopni daje 360.
- [błąd] Upewnij się, że dziecko pracuje w zdarzeniu `SUPER_ATAK`, a nie w podobnym zdarzeniu zwykłego `STRZAL`.

## [guided] Budujemy osiem kierunków (15 min)

### Co robić teraz
- Kodowanie odbywa się wyłącznie na udostępnionym ekranie dziecka; prowadzący pomaga głosem, a dziecko przeciąga i testuje.
- Dziecko dodaje ustawienie typu pocisku, pozycje Gracza i kierunek początkowy 0.
- Wstawia pętlę `powtórz 8 razy`.
- W pętli tworzy klon i obraca duszka o 45 stopni.
- Po zbudowaniu przygotowania testuje samą pozycję, następnie pętlę. Na czas testu ustawia pasek `Super` na 100, naciska E i liczy pociski; po teście przywraca normalne ładowanie.

### Wskazówki
- [podpowiedź] Obraca się duszek wzorcowy; każdy klon zapamiętuje kolejny kierunek.
- [podpowiedź] `idź do Gracz` ustawia niewidoczny wzorzec w centrum ataku, a `ustaw kierunek na 0` daje wszystkim próbom ten sam punkt startowy.
- [błąd] Obracanie musi być wewnątrz pętli, inaczej wszystkie klony polecą w jedną stronę.
- [błąd] Jeśli powstaje siedem wyraźnych kierunków lub jedna luka, policz klony i sprawdź obie liczby: 8 oraz 45.
- [gdy brakuje czasu] Pomiń ustawienie `typPocisku`; w tej wersji gry podstawowy typ wystarczy.

## [challenge] Własny układ pocisków (10 min)

### Co robić teraz
- Dziecko wybiera 6, 10 albo 12 pocisków.
- Oblicza odpowiedni kąt i zmienia obie liczby w skrypcie.
- Testuje, czy po ostatnim obrocie duszek wraca do pełnych 360 stopni.

### Wskazówki
- [podpowiedź] Zasada brzmi `liczba pocisków × kąt = 360`. Zmiana tylko jednej liczby tworzy lukę albo nakłada kierunki.
- [dla szybszych] Dodaj `czekaj 0.03 sekundy` w pętli, aby fala rozchodziła się jak spirala.
- [dla szybszych] Porównaj 6 × 60, 10 × 36 i 12 × 30, a potem wybierz układ najlepszy do wielkości areny.
- [błąd] Po personalizacji wykonaj jeden test bez przeciwników i policz kierunki, zanim rozpoczniesz pełną walkę.

## [demo] Walka z własnym Super Atakiem (10 min)

### Co robić teraz
- Dziecko uruchamia grę bez sztucznego ustawiania paska i ładuje Super normalnie.
- Używa swojego ataku przeciw obu celom.
- Sprawdza: jeden atak po naciśnięciu E, równy pełny okrąg, poprawny ruch klonów i zgodność liczby pocisków z kątem.
- Zapisuje ukończony START pod nową nazwą.

### Wskazówki
- [tempo] Nie zmieniaj teraz życia ani prędkości przeciwników.
- [błąd] Test dotyczy `Pocisk`; nie poprawiaj przy okazji paska Super ani zachowania przeciwników.

## [summary] Pokaz albo domknięcie awaryjne (7 min)

### Co robić teraz
- Jeśli START jest ukończony, dziecko uruchamia Super Atak na swoim ekranie, wskazuje pętlę i wyjaśnia zależność `liczba pocisków × kąt = 360`. Zapytaj o kąt dla czterech pocisków.
- [mów] Wykorzystałeś klony, pętlę i geometrię do zbudowania pełnego ataku. Twój START ma już efekt pokazany wcześniej w FINAL.
- Jeśli zabrakło czasu lub wystąpiła awaria, najpierw zapisz częściowy START z dopiskiem `NIEDOKONCZONE`, nazwij działające elementy i dopiero wtedy wyślij FINAL jako osobny plik do uruchomienia.
- [mów] Zachowaj swoją wersję jako zapis wykonanej pracy; FINAL jest pełnym materiałem pomocniczym, a nie zamiennikiem Twojego projektu.

### Wskazówki
- [podpowiedź] Rodzicowi powiedz konkretnie, czy dziecko samodzielnie obliczyło kąt, zbudowało pętlę i zdiagnozowało kierunki.
- [błąd] Nie wysyłaj FINAL podczas zwykłego debugowania Super Ataku.
- [tempo] Od 55. minuty nie zmieniaj ponownie liczby pocisków.
