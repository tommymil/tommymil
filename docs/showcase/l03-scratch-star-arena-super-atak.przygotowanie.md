# Przygotowanie projektu: Star Arena — Nowy Wróg

Plik dotyczy `l03-scratch-star-arena-super-atak.md`.

## Zasada wersji

`START` jest kompletną strzelanką top-down. Zawiera zwykły strzał, Super Atak, fale, dwa typy zwykłych wrogów, bossa, życie, wynik i dropy. Brakuje wyłącznie typu `Łowca`.

Pliki: `Star-Arena-start.sb3`, `Star-Arena-polprodukt.sb3`, `Star-Arena-final.sb3`.

## Duszki

- `Gracz` — ruch WASD, celowanie, obrażenia, chwilowa nietykalność.
- `Celownik` — śledzi mysz i pokazuje cooldown strzału.
- `Pocisk` — klony zwykłe i Super.
- `Wróg` — jeden wzorzec klonów: `DRON`, `TANK`, `BOSS` oraz w FINAL `LOWCA`.
- `Drop` — apteczka i energia.
- `Spawner` — prowadzi fale oraz czeka, aż `WrogowieNaArenie = 0`.
- `HUD` — życie, wynik, combo, fala, pasek Super i pasek bossa.
- `Efekty` — trafienie, wybuch, zapowiedź fali, wygrana/przegrana.

## Systemy gry

- fala 1: 5 Dronów;
- fala 2 START: 4 Drony + 2 Tanki; FINAL dodatkowo 3 Łowców;
- fala 3: mieszana, z krótszym czasem wejścia;
- boss: osobny pasek, faza przy 50% życia;
- trafienia ładują `Super` do 100; klawisz E tworzy atak promieniowy;
- przeciwnik po śmierci zwiększa wynik i z małą szansą tworzy drop;
- restart usuwa klony i zeruje wszystkie stany.

## Model danych klona

Lokalne zmienne `Wróg`: `TypWroga`, `MojeZycie`, `MojaPredkosc`, `MojePunkty`, `MojCooldown`. Globalna `TypDoStworzenia` jest ustawiana tuż przed utworzeniem klona; klon kopiuje ją do `TypWroga` na początku.

```text
gdy zaczynam jako klon
ustaw [TypWroga v] na (TypDoStworzenia)
zmień rozmiar na (100) %
jeżeli <(TypWroga) = [DRON]> to
  ustaw [MojeZycie v] na (2)
  ustaw [MojaPredkosc v] na (2.4)
  ustaw [MojePunkty v] na (10)
jeżeli <(TypWroga) = [TANK]> to
  ustaw [MojeZycie v] na (7)
  ustaw [MojaPredkosc v] na (0.9)
  ustaw [MojePunkty v] na (40)
pokaż
```

## Mod ucznia

Półprodukt zawiera kostium `lowca` i pustą gałąź. FINAL:

```text
jeżeli <(TypWroga) = [LOWCA]> to
  zmień kostium na [lowca v]
  ustaw [MojeZycie v] na (1)
  ustaw [MojaPredkosc v] na (4.5)
  ustaw [MojePunkty v] na (25)
  ustaw rozmiar na (65) %
```

Spawner fali 2 ustawia `TypDoStworzenia = LOWCA` i tworzy trzy klony w odstępach 0.4 s. Ruch wspólny dla wszystkich klonów wskazuje Gracza i przesuwa o `MojaPredkosc`; nie twórz drugiego silnika ruchu.

## Skróty demo

- `1/2/3` uruchamiają fale po wyczyszczeniu areny;
- `B` uruchamia bossa z 50% życia;
- `H` tworzy apteczkę;
- `K` kończy bieżących wrogów — wyłącznie w kopii instruktorskiej.

## Kryteria jakości

- pocisk nadaje jedno trafienie i usuwa swój klon;
- każdy wróg ma niezależne życie;
- licznik aktywnych wrogów rośnie przy utworzeniu i maleje raz po śmierci;
- następna fala nie startuje, dopóki poprzednia trwa;
- Super Atak nie jest dostępny stale;
- Łowca różni się wyglądem, szybkością, życiem i wartością;
- START zapewnia 2–4 minuty pełnej gry bez Łowcy;
- w pierwszych 90 sekundach widać falę, strzał, reakcję trafienia, drop, wynik i Super.

