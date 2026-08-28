# Pokazowa: Arena Łowców Potworów — Piorunowy Miecz
Rodzaj: pokazowa
Subject: Minecraft Education — MakeCode Blocks
Level: Pokazowa — 9–11 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Blocks, Gra, Zdarzenia, Potwory
Opis: Uczeń walczy w gotowej arenie z falami potworów i programuje magiczny efekt diamentowego miecza.
Cel: Uczeń rozumie zdarzenie „kiedy użyto przedmiotu” i podłącza do niego widoczną reakcję świata.

### Po zajęciach dziecko potrafi
- wskazać zdarzenie, które czeka na konkretną akcję gracza
- podłączyć efekt do użycia przedmiotu
- ograniczyć moc prostą zmienną `GotowaMoc` i przetestować jej balans

### Przygotuj przed zajęciami
- projekty `Arena-Lowcow-start`, `-polprodukt` i `-final` zapisane w widoku Blocks
- gotowy świat: arena z czterema bramami, tryb Easy, wyłączony mob griefing
- sprawdzone komendy `arena`, `start`, `boss` i `reset`
- łuk, strzały, diamentowy miecz, jedzenie i ustawiony punkt odrodzenia
- START zawiera pełną grę; miecz działa normalnie, ale nie ma jeszcze mocy

#### Gotowe w projekcie START
- arena, światła bram, odliczanie i trzy fale zombie oraz szkieletów
- łuk z zapasem strzał, ekwipunek, Easy Survival i bezpieczny restart
- licznik `Pokonane`, komunikaty fal, dropy leczenia i finałowy ravager
- osobne zdarzenia śmierci mobów aktualizujące wynik
- efekt zwycięstwa, przegranej i skróty do szybkiego demo

#### Mały mod uczestnika
`Piorunowy Miecz`: po użyciu diamentowego miecza, jeśli moc jest gotowa, piorun uderza trzy bloki przed graczem, pojawia się komunikat, a moc przechodzi w krótki cooldown. To 5–8 bloczków w gotowym miejscu `MOD_UCZNIA`.

### Zadanie domowe
- wymyśl drugi magiczny przedmiot i opisz zdarzenie, efekt oraz ograniczenie mocy

## [intro] Start i bufor techniczny (7 min)

### Co robić teraz
- Sprawdź sterowanie, czat, użycie miecza prawym przyciskiem i działanie `reset`.
- [mów] Dziś od razu walczysz w pełnej grze i dodajesz do niej jedną własną superbroń.
- Ustal, czy uczeń woli moc ognia, pioruna czy odrzutu; podstawowa realizacja używa pioruna.

## [demo] Arena WOW (5 min)

### Co robić teraz
- Uruchom `start` i pozwól uczniowi odeprzeć krótką falę łukiem.
- Pokaż licznik, drop oraz 10 sekund bossa.
- W FINAL użyj raz Piorunowego Miecza z bezpiecznej pozycji.
- [mów] Cała arena już działa. Twoim zadaniem jest dodać jedną reakcję, która zmieni zwykły miecz w magiczną broń.

### Wskazówki
- [podpowiedź] Bezpieczeństwo: Piorun pojawia się minimum 3 bloki przed graczem, nigdy w jego pozycji.
- [tempo] Po jednym użyciu mocy wróć do START.

## [concept] Zdarzenie czeka, reakcja działa (6 min)

### Co robić teraz
- Pokaż bloczek „kiedy użyto diamentowego miecza”.
- Uczeń wskazuje zdarzenie, warunek i reakcję w działającym przykładzie z komunikatem.
- [mów] Program nie sprawdza miecza bez końca. Czeka na konkretną akcję gracza.

### Materiały
- [kod] Minimalna moc | MakeCode Blocks:
```text
kiedy użyto [diamentowego miecza]
  jeżeli <(GotowaMoc) = (1)> to
    stwórz [błyskawicę] w pozycji (0, 0, 3)
    ustaw [GotowaMoc] na (0)
    powiedz „PIORUN!”
```

## [guided] Misja: Piorunowy Miecz (17 min)

### Co robić teraz
- Uczeń odnajduje przygotowane zdarzenie przedmiotu i testuje je samym komunikatem.
- Dodaje warunek `GotowaMoc = 1`.
- Wewnątrz tworzy błyskawicę trzy bloki przed graczem, ustawia moc na 0 i wyświetla własny tekst.
- Podłącza gotową funkcję `naladuj_moc`, która po 5 sekundach ustawia 1.
- Testuje moc bez mobów, potem na jednym zombie, na końcu w krótkiej fali.

### Wskazówki
- [błąd] Jeśli piorun trafia gracza, współrzędna Z/X jest ustawiona na 0 zamiast bezpiecznego odsunięcia.
- [błąd] Brak warunku pozwala spamować efektem.
- [gdy brakuje czasu] Półprodukt ma gotowy cooldown; uczeń wybiera przedmiot, efekt i komunikat.
- [dla szybszych] Wyświetl odliczanie 5–1 albo dodaj drugi słabszy efekt podczas cooldownu.

## [challenge] Własna wersja broni (13 min)

### Co robić teraz
- Uczeń wybiera jedną modyfikację: czas ładowania 3–8 s, pozycję efektu, nazwę, pogodę na 2 sekundy albo dodatkowy glowstone jako ślad.
- Testuje, czy da się wygrać falę bez używania zwykłego łuku.
- Porównuje: efektowna moc kontra moc zbyt silna.

### Wskazówki
- [podpowiedź] Nie dodawaj TNT ani komend niszczących arenę.
- [dla szybszych] Złoty kilof może leczyć gracza, ale korzysta z tej samej zmiennej energii.

## [challenge] Samodzielny krok (6 min)

### Co robić teraz
- Uczeń sam zmienia cooldown lub odległość i uzasadnia decyzję.
- Zapisz projekt, wykonaj `reset` i przygotuj krótką falę finałową.

## [summary] Finał dla rodzica (6 min)

### Co robić teraz
- Uczeń odpiera falę, używa własnej mocy i wskazuje zdarzenie w Blocks.
- [mów] Zwykły przedmiot stał się magiczną bronią, bo podłączyłeś reakcję do zdarzenia i ograniczyłeś ją cooldownem.
- Uczeń nazywa element, który wykonał sam.
