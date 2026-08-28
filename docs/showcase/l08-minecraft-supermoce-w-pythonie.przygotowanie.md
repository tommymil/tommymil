# Przygotowanie projektu: Arena Żywiołów w Pythonie

Plik dotyczy `l08-minecraft-supermoce-w-pythonie.md`. Projekty zapisuj z aktywnym widokiem Python.

Tryb końcowy: **MakeCode Python**. Podczas przygotowania i zajęć nie przełączaj projektu na Blocks.

## Pliki i świat

Zapisz `Arena-Zywiolow-start.mcworld`, `-polprodukt.mcworld` i `-final.mcworld`.

Arena 25×25 ma pełną podłogę, cztery kolorowe runy 3×3, cztery bramy mobów, bezpieczny balkon do testów i punkt odrodzenia. Ustaw Easy, wyłącz mob griefing i przygotuj ekwipunek.

## Pełna gra START

- `start`: odliczanie, energia 100, wynik 0, ekwipunek, fala 1;
- trzy fale zombie/szkieletów i boss;
- złota runa: leczenie lub jedzenie, koszt 30;
- czerwona: błyskawica 4 bloki przed graczem, koszt 35;
- zielona: efekt szybkości, koszt 20;
- niebieska: końcowe `else` informuje, że runa nie ma jeszcze mocy;
- `moc` rozpoznaje blok pod graczem, sprawdza energię i uruchamia jedną gałąź;
- energia odnawia się między falami; `energia` pokazuje stan;
- `reset` oraz skróty `fala` i `boss`.

Nie zostawiaj w START gry bez fal. Runy są systemem wyboru broni wewnątrz gotowej walki.

## Czytelna architektura

```python
energia = 100
wynik = 0

def ma_energie(koszt):
    if energia >= koszt:
        return True
    player.say("Za mało energii")
    return False

def uruchom_moc():
    global energia

    if blocks.test_for_block(GOLD_BLOCK, pos(0, -1, 0)):
        if ma_energie(30):
            energia -= 30
            mobs.give(mobs.target(LOCAL_PLAYER), COOKED_BEEF, 2)
            player.say("Moc odnowy!")

    elif blocks.test_for_block(RED_CONCRETE, pos(0, -1, 0)):
        if ma_energie(35):
            energia -= 35
            mobs.spawn(LIGHTNING_BOLT, pos(0, 0, 4))
            player.say("Moc burzy!")

    elif blocks.test_for_block(GREEN_CONCRETE, pos(0, -1, 0)):
        if ma_energie(20):
            energia -= 20
            mobs.apply_effect(Effect.SPEED,
                              mobs.target(LOCAL_PLAYER), 6, 1)
            player.say("Moc wiatru!")

    else:
        player.say("Ta runa czeka na nową moc")

player.on_chat("moc", uruchom_moc)
```

Jeśli `return` lub pomocnicza funkcja nie działa stabilnie w używanym MakeCode Python, zastosuj zagnieżdżone `if energia >= koszt` w każdej gałęzi. Priorytetem jest stabilne demo.

## Mod ucznia

Półprodukt zawiera komentarz przed `else`. FINAL:

```python
elif blocks.test_for_block(BLUE_CONCRETE, pos(0, -1, 0)):
    if ma_energie(25):
        energia -= 25
        player.say("Lodowa barykada!")
        blocks.fill(
            PACKED_ICE,
            pos(-1, 0, 3),
            pos(1, 1, 3),
            FillOperation.Replace
        )
```

Barykada musi powstać na pełnym podłożu i nie zamykać jedynego wyjścia. W wariancie szybszym usuń ją po 4 sekundach.

## Liczenie wyniku

Rejestruj `mobs.on_mob_killed` dla typów użytych w falach. Każde zdarzenie zwiększa `wynik` raz. Jeśli API zdarzeń zachowuje się inaczej w wersji szkolnej, użyj sprawdzonego mechanizmu etapów i jasno opisz zmianę w pliku projektu.

## Kryteria jakości

- START zapewnia pełną walkę z wygraną i przegraną;
- każda z trzech run uruchamia dokładnie jedną moc;
- zwykły blok i niebieska runa w START nie zużywają energii;
- efekt powstaje przed graczem, nie na graczu;
- energia nigdy nie spada poniżej zera;
- nowy `elif` ma właściwe wcięcie i znajduje się przed `else`;
- arena nie jest niszczona przez moce;
- `reset` czyści moby, barykady i stan energii.

## Dokumentacja API

- sprawdzanie bloku: https://minecraft.makecode.com/reference/blocks/test-for-block
- efekty mobów: https://minecraft.makecode.com/reference/mobs
- użycie błyskawicy: https://minecraft.makecode.com/reference/player/on-item-used
- zasady gry: https://minecraft.makecode.com/reference/gameplay/set-game-rule
