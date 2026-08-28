# Przygotowanie projektu: Sieć Podniebnych Baz

Plik dotyczy `l09-minecraft-python-teleporter-do-podniebnej-bazy.md`. Projekty pozostają w widoku Python.

Tryb końcowy: **MakeCode Python**. Podczas przygotowania i zajęć nie przełączaj projektu na Blocks.

## Pliki

- `Siec-Baz-start.mcworld` — hub i trzy pełne lokacje, czwarty portal nieaktywny;
- `Siec-Baz-polprodukt.mcworld` — szkielet czwartej funkcji;
- `Siec-Baz-final.mcworld` — czwarta lokacja ucznia.

## Najważniejsza zasada

W plikach nie mogą pozostać przykładowe współrzędne. Autor wpisuje i testuje realne `world(x, y, z)` dla konkretnego świata. Każdy cel ma gotową podłogę przed teleportem i działający powrót.

## Świat START

Centralny hub 19×19 zawiera mapę osi X/Y/Z, cztery oznaczone portale i przycisk/komendę `powrot`. Trzy lokacje są od siebie oddalone minimum 40 bloków:

- `baza` — wysokość 90–110, szklana podłoga, kopuła, beacon i balkon widokowy;
- `jaskinia` — bezpieczna istniejąca lub zbudowana komnata z kryształami i skrzynią;
- `wieza` — wysoki taras, glowstone i kontrolowany efekt pogody;
- `wyspa` — portal nieaktywny w START, miejsce na mod ucznia.

Każda lokacja ma tablicę/nazwę, jedną atrakcję, światło i komendę powrotu. START jest pełną, 2–4-minutową wycieczką.

## Funkcje wspólne

```python
HUB_X = 0
HUB_Y = 70
HUB_Z = 0

def zbuduj_bezpieczny_cel(x, y, z, material):
    blocks.fill(
        material,
        world(x - 3, y - 1, z - 3),
        world(x + 3, y - 1, z + 3),
        FillOperation.Replace
    )
    blocks.fill(
        GLASS,
        world(x - 3, y, z - 3),
        world(x + 3, y + 2, z + 3),
        FillOperation.Outline
    )
    blocks.fill(
        AIR,
        world(x - 2, y, z - 2),
        world(x + 2, y + 1, z + 2),
        FillOperation.Replace
    )

def teleportuj_bezpiecznie(nazwa, x, y, z, material):
    zbuduj_bezpieczny_cel(x, y, z, material)
    player.teleport(world(x, y, z))
    player.say(nazwa + " — wpisz powrot")

def powrot():
    player.teleport(world(HUB_X, HUB_Y, HUB_Z))

player.on_chat("powrot", powrot)
```

Jeśli `FillOperation.Outline` nie istnieje w używanej wersji, zbuduj podłogę i cztery ściany osobnymi `fill(...Replace)`. Nie odkładaj tej weryfikacji na zajęcia.

## Trzy gotowe lokacje

```python
def do_bazy():
    teleportuj_bezpiecznie("Podniebna baza", BAZA_X, BAZA_Y, BAZA_Z, GLASS)

def do_jaskini():
    teleportuj_bezpiecznie("Kryształowa jaskinia",
                           JASKINIA_X, JASKINIA_Y, JASKINIA_Z, STONE)

def do_wiezy():
    teleportuj_bezpiecznie("Wieża burz",
                           WIEZA_X, WIEZA_Y, WIEZA_Z, STONE_BRICKS)

player.on_chat("baza", do_bazy)
player.on_chat("jaskinia", do_jaskini)
player.on_chat("wieza", do_wiezy)
```

Dekoracje buduj osobnymi funkcjami wywoływanymi przed pierwszą demonstracją albo przygotuj je w świecie. Teleport ma pozostać krótki i czytelny.

## Mod ucznia

Półprodukt zawiera:

```python
def do_wyspy():
    # MISJA: wybierz X, Y, Z, nazwę i materiał.
    player.say("Portal czeka na adres")
```

FINAL:

```python
def do_wyspy():
    x = 140
    y = 105
    z = -60
    teleportuj_bezpiecznie("Wyspa Smoka", x, y, z, END_STONE)

player.on_chat("wyspa", do_wyspy)
```

Wartości są przykładem dla autora. Przed przekazaniem projektu zastąp je współrzędnymi zweryfikowanymi w świecie.

## Procedura testowa

1. Uruchom projekt w hubie i sprawdź `powrot`.
2. Odwiedź każdą lokację po ponownym otwarciu świata.
3. Upewnij się, że gracz nie spawnuje w ścianie i może się ruszyć.
4. Zmień X o 10, Y o 10 i Z o 10 na kopii testowej; zanotuj orientację.
5. Sprawdź czwartą lokację w Creative, potem w trybie docelowym.
6. Przywróć finalne współrzędne i czysty hub.

## Kryteria jakości

- START oferuje trzy wyraźnie różne, kompletne lokacje;
- każda podróż kończy się na pełnym podłożu;
- platforma powstaje przed teleportem;
- `world` daje stały adres niezależny od miejsca startu;
- `powrot` zawsze prowadzi do hubu;
- czwarty portal nie jest wymagany do obejrzenia START;
- nazwy funkcji, komend i tablic są spójne;
- żadna baza nie nachodzi na inną;
- po zmianie osi efekt jest zgodny z mapą w hubie.

## Dokumentacja API

- `world(x, y, z)`: https://minecraft.makecode.com/reference/positions/world
- pozycje: https://minecraft.makecode.com/reference/positions
- teleport: https://minecraft.makecode.com/reference/player/teleport
- pozycja gracza: https://minecraft.makecode.com/reference/player/position
