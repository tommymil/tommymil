# Animacja i sterowanie postacią
Subject: Scratch
Level: Poziom 1 - 8-10 lat
Tags: Kostiumy, Sterowanie, Dźwięk, Efekty
Opis: Dzieci wybierają taneczną postać, sterują nią strzałkami i robią mini-układ choreograficzny.

## [intro] Powitanie, obecność i demo (10 min)

### Co robić teraz
- **Powitanie:** przywitaj się z grupą i zbuduj energię na start („Cześć! Gotowi dziś programować?").
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **Co dziś robimy:** krótko zapowiedz temat zajęć — hasło: „Twoja postać zacznie chodzić, mówić i tańczyć — bo TY jej każesz!".
- **DEMO:** pokaż gotową, tańczącą postać sterowaną klawiaturą — „za chwilę zrobicie to samo".
- Wybierz duszka z kilkoma kostiumami — świetnie sprawdzają się postacie z kategorii „Taniec" (Cassy Dancing, Champ99, Anina Dance, Ballerina). Domyślny kot też ma 2 kostiumy i wystarczy na start.
- Dodaj tło: zakładka Scena (prawy dolny róg) → ikona wyboru tła.
- W zakładce Dźwięki duszka dodaj beat do tańca (Dance Around, Drum, Dance Magic) i ewentualnie krótki dźwięk „kroku".

### Wskazówki
- [podpowiedź] Postacie z kategorii „Taniec" mają po kilkanaście póz, więc chodzenie i taniec wyglądają od razu efektownie.
- [podpowiedź] Sprawdź w zakładce Kostiumy, ile ich ma duszek — do animacji chodzenia potrzebne są minimum 2.
- [tempo] Nie zaczynaj od kodu — najpierw demo i wybór postaci, żeby dziecko pomyślało „chcę to mieć".

## [concept] Sterowanie strzałkami - wersja A (15 min)

### Co robić teraz
- Najpierw raz ustaw start, żeby postać się nie obracała do góry nogami.
- Dodaj po jednym bloku zdarzeń na każdą strzałkę (prawo, lewo, góra, dół).
- Dla góry i dołu celowo nie zmieniaj kierunku — inaczej postać kładzie się na plecach.

### Wskazówki
- [błąd] Postać do góry nogami przy ruchu w lewo — zawsze ustaw styl obrotu na lewo-prawo. Najczęstszy problem pierwszej lekcji.
- [podpowiedź] Lewo-prawo robimy przez „ustaw kierunek", a styl lewo-prawo odbija postać lustrzanie.

### Materiały
- [link] Otwórz Scratch - nowy projekt | https://scratch.mit.edu/projects/editor
- [kod] Scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw styl obrotu na [lewo-prawo]
    idź do x: (0) y: (0)
  ```
- [kod] Scratch:
  ```
  kiedy klawisz [strzałka w prawo] naciśnięty
    ustaw kierunek na (90)
    przesuń o (10) kroków
    następny kostium
  kiedy klawisz [strzałka w lewo] naciśnięty
    ustaw kierunek na (-90)
    przesuń o (10) kroków
    następny kostium
  kiedy klawisz [strzałka w górę] naciśnięty
    zmień y o (10)
    następny kostium
  kiedy klawisz [strzałka w dół] naciśnięty
    zmień y o (-10)
    następny kostium
  ```

## [guided] Sterowanie - wersja B, płynny ruch (15 min)

### Co robić teraz
- Kiedy dzieci zrozumieją wersję A, pokaż wersję B: pętla „zawsze" + „jeżeli" daje płynny bieg przy przytrzymaniu klawisza.
- Bloczek „klawisz [...] naciśnięty?" (sześciokąt) jest w sekcji Czujniki — wkładasz go w dziurkę warunku „jeżeli".
- Kroki są mniejsze (5), bo wykonują się w kółko bardzo szybko.

### Wskazówki
- [tempo] To jest efekt WOW — przejdź na wersję B dopiero gdy wersja A jest zrozumiała.
- [błąd] Strzałki nie działają — sprawdź, czy fokus jest na scenie i czy skrypt jest na właściwym duszku, a nie na Scenie.

### Materiały
- [kod] Scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw styl obrotu na [lewo-prawo]
    zawsze
      jeżeli <klawisz [strzałka w prawo] naciśnięty?> to
        ustaw kierunek na (90)
        przesuń o (5) kroków
        następny kostium
      jeżeli <klawisz [strzałka w lewo] naciśnięty?> to
        ustaw kierunek na (-90)
        przesuń o (5) kroków
        następny kostium
      jeżeli <klawisz [strzałka w górę] naciśnięty?> to
        zmień y o (5)
      jeżeli <klawisz [strzałka w dół] naciśnięty?> to
        zmień y o (-5)
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy: dzieci wstają od ekranów, piją wodę, rozprostowują się.
- Zapowiedź: „Po przerwie postać zacznie MÓWIĆ, grać muzykę i TAŃCZYĆ!"

### Wskazówki
- [tempo] Przerwa wypada celowo przed najatrakcyjniejszą częścią — dzieci wracają od razu do najfajniejszego fragmentu i łatwiej się ponownie skupiają.

## [concept] Dymki, dźwięki, efekty (15 min)

### Co robić teraz
- Dodaj powitanie postaci (sekcja Wygląd) — albo „gadanie po kliknięciu", dzieci to uwielbiają.
- Muzykę w tle postaw na Scenie (nie na duszku), żeby grała w pętli.
- Pokaż na żywo efekty graficzne: „zmień efekt [rybie oko] o (25)" oraz „zmień efekt [kolor] o (25)". Do resetu: „wyczyść efekty graficzne".

### Wskazówki
- [podpowiedź] W polskim Scratchu „fisheye" = rybie oko.

### Materiały
- [kod] Powitanie (Wygląd):
  ```
  kiedy kliknięto zieloną flagę
    powiedz [Cześć! Jestem superbohaterem!] przez (2) sek
  ```
- [kod] Gadanie po kliknięciu:
  ```
  kiedy ten duszek kliknięty
    powiedz [Hej, połaskotałeś mnie!] przez (2) sek
  ```
- [kod] Muzyka w tle (na Scenie):
  ```
  kiedy kliknięto zieloną flagę
    zawsze
      zagraj dźwięk [Dance Around] i czekaj
  ```

## [challenge] Taniec po spacji (15 min)

### Co robić teraz
- Zwieńczenie lekcji: sekwencja kostiumów + muzyka + efekt po wciśnięciu spacji.
- Liczbę powtórzeń i czas „czekaj" dobierz na próbę.

### Wskazówki
- [tempo] Z duszkiem z kategorii „Taniec" wygląda to od razu jak prawdziwy układ choreograficzny.

### Materiały
- [kod] Scratch:
  ```
  kiedy klawisz [spacja] naciśnięty
    zagraj dźwięk [Dance Around]
    powtórz (8)
      następny kostium
      zmień efekt [kolor] o (25)
      czekaj (0.3) sek
    wyczyść efekty graficzne
  ```

## [challenge] Pokaz prac (10 min)

### Co robić teraz
- „Scena dla każdego": każde dziecko prezentuje swój układ taneczny na rzutniku — chodzenie, gadanie, taniec.
- Reszta grupy bije brawo. Krótka pochwała za pomysł i styl, nie tylko za technikę.

### Wskazówki
- [tempo] Brawa obowiązkowe — to nagroda za lekcję i powód, żeby chcieć wrócić.
- [podpowiedź] Zapisz projekty (link do gry), żeby dzieci pokazały postać rodzicom.

## [summary] Pułapki i podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj, co dziś zbudowaliśmy: sterowanie, animacja, dźwięk, efekty.
- Przypomnij o zapisaniu projektu.
- Zajawka: zapowiedz cliffhanger następnej lekcji, żeby dzieci chciały wrócić.

### Wskazówki
- [błąd] Postać do góry nogami — zawsze ustaw styl obrotu na lewo-prawo.
- [błąd] Postać znika za krawędzią — w wersji B dodaj „jeżeli na brzegu, odbij się" albo resetuj zieloną flagą (idź do x: 0 y: 0).
- [błąd] Za szybka animacja kostiumów — dorzuć „czekaj (0.1) sek".
- [błąd] Dwa duszki naraz reagują na strzałki — upewnij się, że tylko aktywna postać ma skrypt sterowania.
