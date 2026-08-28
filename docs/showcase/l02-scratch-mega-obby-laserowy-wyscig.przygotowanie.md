# Przygotowanie projektu: Mega Obby — Laserowy Wyścig

Plik dotyczy `l02-scratch-mega-obby-laserowy-wyscig.md`.

## Zasada wersji

`START` musi być ukończoną, trzysektorową grą z działającymi checkpointami. Uczeń nie dodaje mechanizmu koniecznego do przejścia trasy; dokłada jedną bramę pulsującą przed metą.

Pliki: `Mega-Obby-start.sb3`, `Mega-Obby-polprodukt.sb3` i `Mega-Obby-final.sb3`.

## Duszki i systemy

Główne duszki to `Gracz`, `LaserObrotowy`, `LaserPrzesuwny`, `BramaPuls`, `Platforma`, `Checkpoint`, `Meta` i `HUD`.

## Układ trasy

1. `Fabryka` — ruchome platformy, jeden obracający laser i checkpoint 1.
2. `Neonowy tunel` — dwa lasery przesuwne o różnych fazach, strefa szybkości i checkpoint 2.
3. `Lawa` — wąskie platformy, znikający most, pusta rama na mod ucznia i meta.

Jeśli gra mieści się na jednej scenie, sektory mogą być oddzielone wizualnie. Jeśli używa przewijania, przygotuj stabilny system kamery i skrót instruktorski do każdego sektora.

## Systemy obowiązkowe

- ruch z blokadą wychodzenia poza ściany;
- `CheckpointX`, `CheckpointY` i `CheckpointNr` działające w START;
- zegar, licznik upadków, rekord i kara +2 s po trafieniu;
- natychmiastowy respawn z krótką nietykalnością;
- ruchome platformy i co najmniej dwa rodzaje laserów;
- dźwięk ostrzeżenia, animacja trafienia, konfetti na mecie;
- ekran sterowania, restart i skróty testowe `1`, `2`, `3`.

## Zmienne i komunikaty

Globalne: `GraTrwa`, `Czas`, `Upadki`, `Rekord`, `CheckpointX`, `CheckpointY`, `CheckpointNr`, `Predkosc`, `LaserAktywny`.

Komunikaty: `START_GRY`, `TRAFIENIE`, `CHECKPOINT`, `SEKTOR_2`, `SEKTOR_3`, `WYGRANA`, `RESTART`.

## Mod ucznia

W START rama przed metą jest dekoracją i można ją bezpiecznie ominąć. W półprodukcie znajduje się duszek `BramaPuls` z trzema kostiumami, ale bez pętli. FINAL zawiera:

```text
kiedy otrzymam [START_GRY v]
zawsze
  ustaw [LaserAktywny v] na (0)
  zmień kostium na [ostrzeżenie v]
  czekaj (0.5) sekundy
  ustaw [LaserAktywny v] na (1)
  zmień kostium na [aktywny v]
  zagraj dźwięk [laser v]
  czekaj (0.8) sekundy
  ustaw [LaserAktywny v] na (0)
  zmień kostium na [wyłączony v]
  czekaj (1.2) sekundy
```

Kolizja Gracza:

```text
jeżeli <<dotyka [BramaPuls v]?> i <(LaserAktywny) = (1)>> to
  nadaj [TRAFIENIE v] i czekaj
```

Jeżeli w projekcie występuje więcej pulsujących bram, każda musi mieć lokalny stan albo osobną zmienną. W wersji pokazowej jedna brama wystarczy.

## Balans i telemetria instruktorska

- pierwszy sektor: 20–30 s dla początkującego;
- pełny tor: 60–100 s;
- bezpieczne okno bramy co najmniej 1 s;
- respawn poniżej 0.6 s;
- testuj trzy razy bez użycia skrótów;
- zapisz orientacyjny czas ukończenia i sprawdź, czy checkpointy obniżają frustrację.

## Kryteria jakości

- START można ukończyć i pobić rekord bez modu;
- każdy checkpoint zapisuje się raz i daje wyraźny sygnał;
- gracz nie otrzymuje wielu trafień podczas nietykalności;
- aktywny wygląd bramy zawsze zgadza się ze stanem kolizji;
- meta zatrzymuje zegar i wszystkie zagrożenia;
- restart resetuje checkpoint do początku;
- w 90 sekundach demo da się pokazać ruchomą platformę, laser, trafienie, checkpoint i metę.
