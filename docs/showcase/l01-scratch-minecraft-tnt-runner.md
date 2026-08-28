# Pokazowa: Minecraft TNT Runner — Magma Mod
Rodzaj: pokazowa
Subject: Scratch
Level: Pokazowa — 8–9 lat
Czas: 60 min
Tags: Pokazowa, Scratch, Gra, Losowanie, Warunki, Klony
Opis: Uczeń gra w kompletną survivalową arenę TNT, a następnie dodaje własny, szybszy rodzaj kafla Magma TNT.
Cel: Uczeń używa losowania i warunku do nadania klonom różnych zachowań.

### Po zajęciach dziecko potrafi
- wyjaśnić, że jeden duszek może tworzyć wiele klonów o różnych cechach
- wylosować typ kafla i sprawdzić go warunkiem
- zmienić szybkość, wygląd i częstotliwość własnej przeszkody

### Przygotuj przed zajęciami
- `TNT-Runner-start.sb3`: kompletna gra bez Magma TNT
- `TNT-Runner-polprodukt.sb3`: gotowy kostium i zmienna `TypKafla`, ale bez warunku ucznia
- `TNT-Runner-final.sb3`: gra z Magma TNT
- projekt otwarty, dźwięk włączony, sterowanie sprawdzone; żadnego logowania podczas zajęć

#### Gotowe w projekcie START
- arena z klonowanych kafli, które ostrzegawczo migają i zapadają się
- trzy życia, lawa, emeraldy, wynik, rekord sesji i rosnące tempo
- efekty trafienia, dźwięki, ekran startu, przegranej i restart
- zwykły TNT oraz rzadki bezpieczny kafel bonusowy

#### Mały mod uczestnika
`Magma TNT`: około 20% nowych kafli ma czerwony kostium, pulsuje i zapada się dwa razy szybciej. Reszta silnika gry pozostaje gotowa.

### Zadanie domowe
- pokaż własny Magma TNT i wymyśl trzeci typ kafla wraz z jedną zasadą

## [intro] Start i bufor techniczny (7 min)

### Co robić teraz
- [mów] Za chwilę zagrasz w gotową grę, a potem dodasz do niej własny rodzaj niebezpiecznego bloku.
- Sprawdź dźwięk, strzałki/WASD i zieloną flagę. Zapytaj o ulubiony tryb survivalowy.
- Nie otwieraj jeszcze kodu.

### Wskazówki
- [tempo] Jeśli środowisko działa, przejdź do gry najpóźniej w 4. minucie.
- [gdy brakuje czasu] Pomiń rozmowę, nigdy demo.

## [demo] Grywalny efekt WOW (5 min)

### Co robić teraz
- Uczeń próbuje przeżyć, zbiera emerald i traci jedno życie w lawie.
- Instruktor uruchamia wersję FINAL na 20 sekund, aby pokazać czerwony Magma TNT.
- [mów] Wszystkie liczniki, restart i arena już działają. Dzisiaj zmienisz zasady świata, dodając własny typ kafla.

### Wskazówki
- [tempo] Jedna próba, maksymalnie 3 minuty.
- [podpowiedź] Nie tłumacz jeszcze wyniku, żyć ani całego systemu klonów.

### Materiały
- `TNT-Runner-start.sb3` oraz krótki podgląd FINAL

## [concept] Jeden duszek, różne klony (6 min)

### Co robić teraz
- Pokaż duszka `TNT` i zmienną lokalną `TypKafla`.
- Porównaj: każdy klon losuje „rolę”, a warunek wybiera jego kostium i czas ostrzeżenia.
- [mów] Jaką liczbę powinien dostać najrzadszy, najgroźniejszy kafel?
- Zmień próg z 20 na 60, przetestuj, a potem cofnij. Uczeń przewiduje efekt.

### Materiały
- [kod] Rdzeń modu | Scratch:
```text
gdy zaczynam jako klon
ustaw [TypKafla v] na (losuj od (1) do (100))
jeżeli <(TypKafla) <= (20)> to
  zmień kostium na [magma v]
  ustaw [CzasZapadania v] na (0.35)
w przeciwnym razie
  zmień kostium na [tnt v]
  ustaw [CzasZapadania v] na (0.75)
```

### Wskazówki
- [błąd] `TypKafla` i `CzasZapadania` muszą być zmiennymi „tylko dla tego duszka”, aby klony nie nadpisywały sobie wartości.
- [tempo] Nie omawiaj skryptów żyć, wyniku ani restartu.

## [guided] Misja: Magma TNT (17 min)

### Co robić teraz
- Uczeń dodaje kostium `magma` przez duplikację istniejącego kostiumu i zmianę koloru.
- Tworzy lokalną zmienną `TypKafla` i losuje liczbę 1–100 na początku klona.
- Dodaje warunek `TypKafla <= 20` oraz wybór kostiumu.
- W gałęzi Magma ustawia krótszy `CzasZapadania`; istniejąca animacja korzysta już z tej zmiennej.
- Test 1: próg `100`, aby każdy kafel był Magmą. Test 2: próg `20`, aby sprawdzić losowość.

### Wskazówki
- [podpowiedź] Wskaż kategorię „Wyrażenia” lub „Kontrola”, ale uczeń przeciąga bloki.
- [gdy brakuje czasu] Otwórz półprodukt po 30. minucie i pozostaw uczniowi wybór progu oraz czasu.
- [dla szybszych] Dodaj Magmie 2 punkty zamiast 1 za przetrwanie jej zapadnięcia.

## [challenge] Personalizacja modu (13 min)

### Co robić teraz
- Uczeń wybiera dwie cechy: szansa 10–35%, czas 0.2–0.6 s, kolor, dźwięk albo liczba punktów.
- Nazwijcie przeciwnika/kafel i dodajcie krótki komunikat pierwszego pojawienia.
- Po każdej zmianie wykonaj jedną 20-sekundową próbę.

### Wskazówki
- [błąd] Nie zmieniaj kilku parametrów bez testu — trudno wtedy znaleźć przyczynę błędu.
- [dla szybszych] Trzeci typ `Obsydian` ma 10% szans i nie zapada się.

## [challenge] Samodzielny krok (6 min)

### Co robić teraz
- [mów] Zmień jedną rzecz tak, żebym nie wiedział z góry, jak będzie wyglądała Twoja wersja.
- Uczeń sam zmienia próg lub czas i przewiduje, czy gra stanie się łatwiejsza.
- Zapisz stabilny projekt najpóźniej w 55. minucie.

## [summary] Finał dla rodzica (6 min)

### Co robić teraz
- Uczeń uruchamia grę, odnajduje Magma TNT i mówi: „Ten kafel dodałem ja; pojawia się dzięki losowaniu i działa inaczej dzięki warunkowi”.
- [mów] Dziś nie tylko zmieniłeś kolor. Stworzyłeś nowy typ obiektu z własną zasadą.
- Zanotuj samodzielność i rekomendowany kolejny projekt.

### Wskazówki
- [tempo] Nie poprawiaj już balansu. Finał ma być pokazem działającej gry.
