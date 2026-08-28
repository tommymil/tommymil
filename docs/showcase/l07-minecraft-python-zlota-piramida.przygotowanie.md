# Przygotowanie projektu: Klątwa Złotej Piramidy

Plik dotyczy `l07-minecraft-python-zlota-piramida.md`. Projekty pozostają w widoku Python.

Tryb końcowy: **MakeCode Python**. Podczas przygotowania i zajęć nie przełączaj projektu na Blocks.

## Pliki

- `Piramida-Klatwa-start.mcworld` — pełna przygoda z pustym, bezpiecznym korytarzem;
- `Piramida-Klatwa-polprodukt.mcworld` — szkielet funkcji modu;
- `Piramida-Klatwa-final.mcworld` — korytarz pułapek i personalizacja.

## Standard START

Komenda `piramida` w kilka sekund tworzy:

- piramidę o podstawie około 21×21 z warstw budowanych pętlą;
- czytelne wejście, schody i oświetlenie;
- trzy komnaty: mapy, strażników oraz skarbca;
- bezpieczną pułapkę z zapadającym wizualnie/zmienianym podłożem;
- złoty skarb, dwóch strażników, tytuły etapów i komendę powrotu;
- jeden pusty korytarz, który można ominąć i który przyjmuje mod ucznia.

Świat: płaski obszar minimum 50×50, Easy, mob griefing wyłączone, zapis przed generacją. Przygotuj `wejscie`, `skarb` i `reset` do demo.

## Architektura kodu

Podziel generator na krótkie funkcje:

```python
def bryla(wysokosc, material):
    promien = wysokosc * 2
    for poziom in range(wysokosc):
        blocks.fill(
            material,
            pos(-promien + poziom, poziom, 6 + poziom),
            pos(promien - poziom, poziom, 6 + 2 * promien - poziom),
            FillOperation.Replace
        )

def wejscie_i_korytarze():
    # Wycinanie AIR wykonuj po zbudowaniu bryły.
    blocks.fill(AIR, pos(-1, 0, 6), pos(1, 2, 18),
                FillOperation.Replace)

def komnata_skarbu():
    blocks.fill(AIR, pos(-4, 1, 18), pos(4, 4, 26),
                FillOperation.Replace)
    blocks.place(DIAMOND_BLOCK, pos(0, 1, 23))

def zbuduj_piramide():
    bryla(6, SANDSTONE)
    wejscie_i_korytarze()
    komnata_skarbu()
    dodaj_swiatla()
    dodaj_straznikow()
    # MOD UCZNIA — korytarz_pulapek()
```

Dostosuj współrzędne do orientacji rzeczywistego świata. Funkcje muszą być krótkie i zwijalne/czytelnie oddzielone komentarzami.

## Mod ucznia

W półprodukcie:

```python
def korytarz_pulapek():
    # MISJA: użyj for, aby rozmieścić pola według wzoru.
    pass
```

Jeśli edytor nie zachowuje `pass` stabilnie, umieść `player.say("Miejsce na mod")`. FINAL:

```python
def korytarz_pulapek():
    for i in range(5):
        blocks.place(MAGMA_BLOCK, pos(0, 0, 10 + i * 2))
        blocks.place(GLOWSTONE, pos(1, 0, 10 + i * 2))
```

Wywołaj funkcję po utworzeniu podłogi i wycięciu korytarza, aby późniejszy `fill` jej nie nadpisał.

## Strażnicy i bezpieczeństwo

Strażników spawnuj dopiero po utworzeniu pełnej podłogi. Obowiązkowa wersja nie używa TNT, lawy ani ognia. Jeśli `MAGMA_BLOCK` zadaje zbyt duże obrażenia dla wieku uczestnika, zamień go na czerwony beton i wykrywaj błąd trasy fabularnie.

## Kryteria jakości

- START ma początek, cel, przeszkodę, skarb i powrót;
- generator kończy się na używanym sprzęcie w kilka sekund;
- wejście i komnaty powstają po bryle;
- żaden mob nie spawnuje w ścianie;
- pusty korytarz nie blokuje ukończenia START;
- zmiana `range` i mnożnika daje natychmiast widoczny efekt;
- ponowne wywołanie generatora w czystym miejscu daje ten sam wynik;
- wcięcia i komentarz `MOD UCZNIA` pozostają widoczne.

## Dokumentacja API

- `blocks.fill`: https://minecraft.makecode.com/tutorials/python/spleef
- pozycje: https://minecraft.makecode.com/reference/positions
- pętle: https://minecraft.makecode.com/blocks/loops
- moby: https://minecraft.makecode.com/reference/mobs/spawn
