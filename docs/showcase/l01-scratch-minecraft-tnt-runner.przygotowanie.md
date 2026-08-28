# Przygotowanie projektu: Minecraft TNT Runner — Magma Mod

Plik dotyczy `l01-scratch-minecraft-tnt-runner.md`. Odbiorcą jest autor pliku `.sb3`.

## Zasada wersji

`START` jest kompletną grą survivalową. Nie usuwaj licznika, rekordu, żyć ani restartu, aby „zostawić coś uczniowi”. Jedynym brakującym elementem jest typ `Magma TNT`.

Zapisz trzy pliki:

- `TNT-Runner-start.sb3` — pełna gra bez Magmy;
- `TNT-Runner-polprodukt.sb3` — dodatkowo kostium `magma`, lokalne zmienne i pusta gałąź warunku;
- `TNT-Runner-final.sb3` — Magma działa i jest dodana do balansu.

## Duszki i systemy obowiązkowe

- `Gracz`: płynny ruch, krótka nietykalność po trafieniu, 3 życia, animacja upadku.
- `TNT`: klonowana siatka minimum 7×5; zwykłe kafle migają i znikają.
- `Emerald`: pojawia się na bezpiecznym kaflu, daje punkty i znika po czasie.
- `Lawa`: przesuwa się przy krawędzi albo okresowo zajmuje oznaczone pole.
- `HUD`: `Zycia`, `Czas`, `Wynik`, `Rekord`, ostrzeżenie o wzroście tempa.
- `Ekran`: START, PAUZA opcjonalna, KONIEC GRY i przycisk ZAGRAJ PONOWNIE.
- `Efekt`: iskry/pył po zapadnięciu i dwa krótkie dźwięki ostrzegawcze.

## Zmienne i komunikaty

Globalne: `GraTrwa`, `Zycia`, `Czas`, `Wynik`, `Rekord`, `PoziomTempa`, `Nietykalny`.

Lokalne dla `TNT`: `TypKafla`, `CzasZapadania`, `Aktywowany`.

Komunikaty: `START_GRY`, `TRAFIENIE`, `NOWE_ZYCIE`, `TEMPO_PLUS`, `KONIEC_GRY`, `RESTART`.

## Przepływ gotowej gry

1. Zielona flaga pokazuje ekran sterowania; gra zaczyna się przyciskiem.
2. `START_GRY` zeruje wynik, czas i tempo, ale nie rekord.
3. Plansza tworzy klony. Dotknięty kafel ostrzega i zapada się.
4. Co 15 sekund `PoziomTempa` rośnie, a czas zapadania zwykłego TNT maleje do bezpiecznego minimum.
5. Upadek/lawa odejmuje życie i odtwarza gracza na jeszcze istniejącym polu.
6. Przy 0 życia `KONIEC_GRY` aktualizuje rekord oraz pokazuje restart.

## Mod ucznia

W START nie twórz kostiumu ani gałęzi Magmy. W półprodukcie dodaj oba, ale pozostaw liczby do uzupełnienia.

```text
gdy zaczynam jako klon
ustaw [TypKafla v] na (losuj od (1) do (100))
ustaw [CzasZapadania v] na ((0.8) - ((PoziomTempa) * (0.05)))
jeżeli <(TypKafla) <= (20)> to
  zmień kostium na [magma v]
  ustaw [CzasZapadania v] na ((CzasZapadania) / (2))
w przeciwnym razie
  zmień kostium na [tnt v]
```

Dalsza animacja zawsze używa `CzasZapadania`. Nie duplikuj całego skryptu dla Magmy.

## Balans i skróty demo

- zwykły TNT: 0.8 s na początku, minimum 0.45 s;
- Magma: 15–25% kafli i połowa bieżącego czasu;
- emerald: co 7–12 s, widoczny 3 s;
- po trafieniu: 1 s nietykalności;
- skrót `M` ustawia próg Magmy na 100% tylko w kopii instruktorskiej;
- skrót `B` przechodzi do wysokiego tempa.

## Kryteria jakości

- gra jest atrakcyjna i możliwa do ukończenia testu bez modu ucznia;
- restart czyści wszystkie klony, timery i efekty;
- rekord nie zeruje się przy restarcie, tylko po zielonej fladze lub świadomej decyzji;
- jeden kafel aktywuje się raz;
- Magma różni się zachowaniem, nie tylko kolorem;
- lokalne zmienne klonów nie wpływają na pozostałe kafle;
- po 90 sekundach demo widoczne są: zapadnięcie, emerald, utrata życia, zmiana wyniku i wzrost tempa.

