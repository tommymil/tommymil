# Pokazowa: Arena Żywiołów w Pythonie
Rodzaj: pokazowa
Subject: Minecraft Education — MakeCode Python
Level: Pokazowa — 11–13 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Python, Gra, Warunki, Moce
Opis: Uczeń walczy w gotowej arenie, wybiera moce kolorowymi runami i dodaje czwartą moc za pomocą `elif`.
Cel: Uczeń rozszerza instrukcję `if / elif / else` o nowy przypadek i reakcję.

### Po zajęciach dziecko potrafi
- wyjaśnić, dlaczego wykonywana jest tylko jedna pasująca gałąź
- dodać `elif` przed końcowym `else`
- zaprojektować bezpieczny efekt i ustawić jego koszt energii

### Przygotuj przed zajęciami
- projekty `Arena-Zywiolow-start`, `-polprodukt` i `-final` w Pythonie
- arena 25×25 z czterema runami, bramami mobów i bezpiecznym balkonem testowym
- START: trzy gotowe moce, energia, fale, boss, ekwipunek i restart
- niebieska/czwarta runa nie ma efektu; to mod ucznia
- sprawdzone `start`, `moc`, `boss`, `energia` i `reset`

#### Gotowe w projekcie START
- trzy fale potworów, wynik, energia 0–100, dropy i finałowy boss
- złota runa leczy, czerwona przywołuje błyskawicę przed graczem, zielona daje szybkość
- wybór mocy przez blok pod graczem, koszt energii i komunikat cooldownu
- arena, łuk, miecz, Easy Survival, zwycięstwo i restart
- zwykły blok trafia do `else` bez efektu

#### Mały mod uczestnika
Czwarta moc na niebieskiej runie: `elif` rozpoznaje niebieski beton i tworzy wybrany bezpieczny efekt, np. lodową barykadę 3×2 przed graczem. Uczeń ustawia nazwę oraz koszt.

### Zadanie domowe
- opisz piątą runę: warunek, efekt, koszt i sposób sprawdzenia

## [intro] Start i bufor techniczny (7 min)

### Co robić teraz
- Sprawdź kod, czat i arenę.
- [mów] Dzisiaj dostajesz gotową walkę z trzema mocami. Dopiszesz czwartą, własną gałąź prawdziwego kodu Python.
- Ustal, czy uczeń rozpoznaje wcięcia.

## [demo] Arena WOW (5 min)

### Co robić teraz
- Uruchom skróconą falę. Uczeń używa czerwonej oraz zielonej runy i obserwuje spadek energii.
- Pokaż bossa i pustą niebieską runę.
- [mów] Gra nie zna jeszcze tej runy. Za chwilę nauczysz ją nowej reakcji.

### Wskazówki
- [tempo] Dwie moce i krótki boss wystarczą.
- [podpowiedź] Bezpieczeństwo: Efekty powstają przed graczem i nie używają TNT.

## [concept] Program wybiera jedną gałąź (6 min)

### Co robić teraz
- Pokaż wyłącznie funkcję `uruchom_moc`.
- Uczeń przewiduje wynik dla złotego, zielonego i zwykłego bloku.
- Zaznacz, że nowy `elif` musi znaleźć się przed `else` i mieć takie samo wcięcie.

### Materiały
- [kod] Gałąź lodowej mocy | Python:
```python
elif blocks.test_for_block(BLUE_CONCRETE, pos(0, -1, 0)):
    energia -= 25
    player.say("Lodowa barykada!")
    blocks.fill(PACKED_ICE, pos(-1, 0, 3), pos(1, 1, 3),
                FillOperation.Replace)
```

## [guided] Misja: czwarta moc (17 min)

### Co robić teraz
- Uczeń dodaje `elif` przed `else` i sprawdza niebieski blok.
- Najpierw reakcją jest tylko `player.say`; testuje wybór właściwej gałęzi.
- Dodaje koszt 25 energii i gotowe sprawdzenie `energia >= 25`.
- Dodaje barykadę lub glowstone/błyskawicę w bezpiecznej pozycji.
- Testuje na pustej arenie, potem podczas jednej fali.

### Wskazówki
- [błąd] `elif` pod `else` jest błędem składni/logiki.
- [błąd] Niewłaściwe wcięcie uruchomi efekt poza warunkiem.
- [gdy brakuje czasu] Półprodukt ma pustą gałąź; uczeń wybiera blok, efekt, nazwę i koszt.
- [dla szybszych] Barykada znika po 4 sekundach przez zastąpienie jej powietrzem.

## [challenge] Balans mocy (13 min)

### Co robić teraz
- Uczeń wybiera koszt 15–40, rozmiar efektu i nazwę.
- Próbuje pokonać falę używając każdej mocy raz i porównuje ich przydatność.
- Jeśli nowa moc zawsze jest najlepsza, zwiększa koszt albo zmniejsza efekt.

### Wskazówki
- [podpowiedź] Dobra moc jest ciekawym wyborem, nie przyciskiem automatycznej wygranej.

## [challenge] Samodzielny krok (6 min)

### Co robić teraz
- Uczeń sam zmienia jeden parametr lub dodaje komunikat o braku energii.
- Zapisz czysty stan areny do finału.

## [summary] Finał dla rodzica (6 min)

### Co robić teraz
- Uczeń uruchamia falę, staje na swojej runie i prezentuje efekt.
- [mów] Rozszerzyłeś gotową grę o nową regułę. Python rozpoznaje Twój blok i wybiera właściwą reakcję.
- Uczeń wskazuje `elif`, warunek i ciało gałęzi.
