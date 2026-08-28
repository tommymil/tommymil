# Pokazowa: Star Arena — Nowy Wróg
Rodzaj: pokazowa
Subject: Scratch
Level: Pokazowa — 9–11 lat
Czas: 60 min
Tags: Pokazowa, Scratch, Gra, Klony, Warunki, Przeciwnicy
Opis: Uczeń gra w dynamiczną arenę z falami i bossem, a następnie dodaje nowy typ przeciwnika „Łowca”.
Cel: Uczeń wykorzystuje klony i zmienną typu, aby jeden duszek tworzył przeciwników o różnych zachowaniach.

### Po zajęciach dziecko potrafi
- wyjaśnić, dlaczego gra tworzy klony zamiast dziesiątek duszków
- nadać klonowi typ i ustawić jego prędkość, życie oraz punkty
- dodać nowy typ wroga do istniejącego generatora fal

### Przygotuj przed zajęciami
- `Star-Arena-start.sb3`: pełna gra z Dronem, Tankiem i bossem, bez Łowcy
- `Star-Arena-polprodukt.sb3`: kostium Łowcy i pusta gałąź warunku
- `Star-Arena-final.sb3`: wersja z Łowcą
- sprawdź mysz, WASD, strzał, Super Atak, drop apteczki i skrót do bossa

#### Gotowe w projekcie START
- ruch WASD, celownik myszy, zwykły strzał i naładowany Super Atak
- trzy fale, Drony, Tanki i boss z paskiem życia
- życie gracza, wynik, combo, apteczki, ekran wygranej/przegranej i restart
- dźwięki, błyski trafienia, cząsteczki i coraz szybsza muzyka

#### Mały mod uczestnika
`Łowca`: mniejszy, szybszy klon, który skręca w stronę gracza, ma 1 punkt życia i daje 25 punktów. Uczeń dopisuje tylko konfigurację typu oraz jedno pojawienie w fali 2.

### Zadanie domowe
- wymyśl kontrę na Łowcę: jaki drop albo ruch gracza pomagałby go pokonać?

## [intro] Start i bufor techniczny (7 min)

### Co robić teraz
- Sprawdź sterowanie i dźwięk.
- [mów] Dzisiaj nie będziemy robić pustej strzelanki od zera. Zagrasz w gotową arenę i dodasz przeciwnika, którego sam zaprojektujesz.
- Zapytaj o ulubiony typ wroga: szybki, wytrzymały czy atakujący z dystansu.

## [demo] Grywalny efekt WOW (5 min)

### Co robić teraz
- Uczeń rozgrywa falę 1, podnosi apteczkę i używa Super Ataku.
- Skrótem instruktorskim pokaż przez 15 sekund bossa.
- [mów] Ruch, strzelanie, fale i boss już są. Twoja zmiana będzie mała w kodzie, ale pojawi się wiele razy podczas gry.

### Wskazówki
- [tempo] Nie pozwól, aby walka z bossem zastąpiła misję.

## [concept] Jeden wzorzec, wiele typów (6 min)

### Co robić teraz
- Pokaż duszka `Wróg`, klonowanie i lokalną zmienną `TypWroga`.
- Otwórz istniejące konfiguracje `DRON` i `TANK`. Uczeń wskazuje trzy różnice.
- [mów] Generator wybiera typ, a klon na tej podstawie ustawia własne statystyki.

### Materiały
- [kod] Konfiguracja Łowcy | Scratch:
```text
jeżeli <(TypWroga) = [LOWCA]> to
  zmień kostium na [lowca v]
  ustaw [MojeZycie v] na (1)
  ustaw [MojaPredkosc v] na (4.5)
  ustaw [MojePunkty v] na (25)
  ustaw rozmiar na (65) %
```

### Wskazówki
- [błąd] Statystyki klona muszą być lokalne. Globalna prędkość przyspieszy wszystkich wrogów.
- [tempo] Nie omawiaj silnika pocisków ani bossa.

## [guided] Misja: wróg Łowca (17 min)

### Co robić teraz
- Uczeń duplikuje kostium Drona i tworzy wygląd Łowcy.
- W konfiguracji klona kopiuje gałąź Drona, zmienia typ na `LOWCA` i ustawia cztery parametry.
- W gotowym generatorze fali 2 dodaje: ustaw `TypDoStworzenia` na `LOWCA`, utwórz 3 klony.
- Testuje najpierw jednego Łowcę, potem całą falę.

### Wskazówki
- [błąd] Jeśli Łowca wygląda jak Dron, sprawdź konfigurację wykonywaną „gdy zaczynam jako klon”.
- [podpowiedź] Uczeń kopiuje działający wzorzec i świadomie zmienia tylko typ oraz parametry.
- [gdy brakuje czasu] Użyj półproduktu; uczeń wybiera parametry i liczbę klonów.

## [challenge] Zachowanie i balans (13 min)

### Co robić teraz
- Uczeń wybiera jedną cechę specjalną: zygzak, krótkie przyspieszenie co 2 sekundy albo 10% szansy na drop energii.
- Przeprowadza trzy testy fali 2 i reguluje liczbę Łowców lub szybkość.
- Nadaje własną nazwę wyświetlaną przed falą.

### Wskazówki
- [dla szybszych] Łowca po utracie życia tworzy dwa słabsze Mini-Łowce.
- [tempo] Jedna cecha specjalna wystarczy.

## [challenge] Samodzielny krok (6 min)

### Co robić teraz
- Uczeń sam zmienia jeden parametr i przewiduje wpływ na poziom trudności.
- Zapisz wersję przed uruchomieniem pełnej finałowej fali.

## [summary] Finał dla rodzica (6 min)

### Co robić teraz
- Uczeń uruchamia falę 2, wskazuje Łowcę i mówi, czym różni się od Drona oraz Tanka.
- [mów] Kilka bloczków stworzyło cały nowy rodzaj przeciwnika, bo generator może produkować wiele jego klonów.
- Jeśli rodzic jest obecny, uczeń używa Super Ataku przeciw własnej fali.
