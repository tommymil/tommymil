# Pokazowa: Klątwa Złotej Piramidy
Rodzaj: pokazowa
Subject: Minecraft Education — MakeCode Python
Level: Pokazowa — 11–13 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Python, Gra, Pętle, Generator
Opis: Uczeń eksploruje generowaną świątynię z pułapkami i strażnikami, a następnie koduje własny korytarz pułapek.
Cel: Uczeń stosuje pętlę `for` do rozmieszczenia serii bloków według wzoru.

### Po zajęciach dziecko potrafi
- rozpoznać elementy powtarzające się w świecie i opisać je pętlą
- zmienić liczbę, odstęp i materiał generowanych elementów
- dopisać małą funkcję do większego gotowego programu

### Przygotuj przed zajęciami
- projekty `Piramida-Klatwa-start`, `-polprodukt` i `-final` w widoku Python
- płaski świat z miejscem 50×50, tryb Easy, wyłączone niszczenie przez moby
- START generuje kompletną świątynię; pozostaw pusty jeden korytarz na mod ucznia
- komendy `piramida`, `wejscie`, `skarb` i `reset`
- test wydajności generatora na urządzeniu ucznia

#### Gotowe w projekcie START
- monumentalna piramida z wejściem, oświetleniem, trzema komnatami i ukrytym skarbcem
- ruchome wrażenie pułapek budowane zmianą podłoża, bez niszczącego TNT
- strażnicy w komnacie, ekwipunek gracza, komunikaty fabularne i powrót
- szybkie teleporty instruktorskie i restart obszaru
- pełna grywalna trasa; pusty korytarz jest miejscem na dodatkową pułapkę, nie brakującym przejściem

#### Mały mod uczestnika
Funkcja `korytarz_pulapek()` używa pętli `for`, aby ułożyć 5 pól magma/glowstone co dwa bloki. Uczeń zmienia trzy parametry i wywołuje gotową funkcję w generatorze.

### Zadanie domowe
- narysuj wzór dziesięciu pól i zapisz, jak obliczyć pozycję pola numer `i`

## [intro] Start i bufor techniczny (7 min)

### Co robić teraz
- Sprawdź Python, czat i komendę `reset`.
- [mów] Ten kod tworzy całą przygodę, nie tylko bryłę. Ty dopiszesz do gotowej świątyni własny korytarz pułapek.
- Zapytaj, czy uczeń widział wcześniej pętlę `for`.

## [demo] Świątynia WOW (5 min)

### Co robić teraz
- Uruchom `piramida` i obserwuj powstawanie bryły, świateł, wejścia i komnat.
- Teleportuj ucznia do wejścia; pokaż jedną pułapkę, strażnika i zamknięty skarbiec.
- [mów] Setki bloków i cała mini-przygoda już działają. Zmienimy tylko jeden moduł.

### Wskazówki
- [tempo] Nie przechodź całego lochu. Zachowaj skarb na finał.

## [concept] Wzór zapisany pętlą (6 min)

### Co robić teraz
- Pokaż istniejącą pętlę świateł, nie generator całej piramidy.
- Uczeń oblicza pozycje dla `i = 0, 1, 2` w wyrażeniu `6 + i * 2`.
- [mów] Pętla to nie kopiowanie tej samej pozycji; numer powtórzenia przesuwa każdy element.

### Materiały
- [kod] Mod ucznia | Python:
```python
def korytarz_pulapek():
    for i in range(5):
        blocks.place(MAGMA_BLOCK, pos(0, 0, 10 + i * 2))
```

## [guided] Misja: korytarz pułapek (17 min)

### Co robić teraz
- Uczeń dopisuje funkcję z pętlą `for i in range(5)`.
- Wewnątrz stawia wybrany blok w pozycji `10 + i * 2`.
- Wywołuje funkcję w oznaczonym miejscu `# MOD UCZNIA` po zbudowaniu podłogi.
- Testuje z `range(1)`, potem `range(5)`.
- Dodaje glowstone obok każdego pola jako czytelne ostrzeżenie.

### Wskazówki
- [błąd] Wywołanie pułapek przed podłogą spowoduje ich nadpisanie.
- [błąd] Brak `* 2` ustawi pola bez przerw.
- [gdy brakuje czasu] Półprodukt ma funkcję i pętlę; uczeń wybiera materiał, liczbę oraz odstęp.
- [dla szybszych] Co drugie pole jest bezpieczne dzięki warunkowi `if i % 2 == 0`.

## [challenge] Projektowanie wzoru (13 min)

### Co robić teraz
- Uczeń wybiera długość 4–8, odstęp 1–3 oraz dwa materiały.
- Przechodzi własny korytarz w survival i poprawia zbyt trudny układ.
- Nadaje pułapce nazwę pokazywaną na wejściu.

### Wskazówki
- [podpowiedź] Bezpieczeństwo: Nie używaj TNT, lawy ani ognia w obowiązkowej wersji.

## [challenge] Samodzielny krok (6 min)

### Co robić teraz
- Uczeń sam dodaje drugi rząd świateł lub zmienia oś wzoru.
- Zapisz kod i wygeneruj czysty świat do finału.

## [summary] Finał dla rodzica (6 min)

### Co robić teraz
- Uczeń generuje piramidę, przechodzi własny korytarz i otwiera skarbiec.
- [mów] Kilka linii kodu utworzyło serię elementów, a Twoja funkcja stała się częścią dużej gry.
- Uczeń wskazuje `range`, `i` i wpływ mnożnika.
