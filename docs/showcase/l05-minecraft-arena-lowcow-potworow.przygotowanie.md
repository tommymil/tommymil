# Przygotowanie projektu: Arena Łowców Potworów — Piorunowy Miecz

Plik dotyczy `l05-minecraft-arena-lowcow-potworow.md`.

## Tryb pracy

Przygotuj kod w MakeCode Python, przetestuj, przełącz na Blocks i w tym widoku zapisz `Arena-Lowcow-start.mcworld`, `-polprodukt.mcworld` oraz `-final.mcworld`. Uczeń pracuje wyłącznie na bloczkach.

Tryb końcowy: **MakeCode Blocks**. Python służy tutaj tylko do szybkiego przygotowania projektu.

## Świat

Arena 25×25 ma:

- mur wysoki na 4 bloki, cztery oświetlone bramy i trybunę testową;
- pełne, niepalne podłoże i brak TNT;
- centralną runę startu, skrzynię/ekwipunek oraz bezpieczny punkt odrodzenia;
- punkty spawnu minimum 8 bloków od środka;
- mob griefing wyłączone, trudność Easy, keep inventory włączone.

## Pełna gra START

START zawiera trzy fale, licznik, dropy i finałowego przeciwnika.

- `arena` buduje/resetuje planszę;
- `start` daje łuk, 64 strzały, żelazny miecz/diamentowy miecz, jedzenie i uruchamia odliczanie;
- fala 1: zombie, fala 2: zombie + szkielety, fala 3: szybsza mieszana;
- zabicie zombie lub szkieleta zwiększa `Pokonane` i wyświetla postęp;
- co piąte pokonanie daje jedzenie/leczenie;
- finałowy ravager lub inny dostępny boss;
- `boss` i `testmod` jako skróty;
- śmierć gracza pokazuje przegraną i umożliwia `reset`.

Do licznika rejestruj osobne zdarzenia śmierci dla użytych typów mobów. Jeśli w danej wersji zdarzenie nie konwertuje się stabilnie do Blocks, użyj wyniku prowadzonego komendą/etapami, ale nie zostawiaj niewidocznego, niedziałającego HUD-u.

## Kod bazowy fal

```python
pokonane = 0
gotowa_moc = 1

def spawnuj(mob, ile, x, z, odstep):
    for i in range(ile):
        mobs.spawn(mob, pos(x, 0, z))
        loops.pause(odstep)

def start():
    global pokonane, gotowa_moc
    pokonane = 0
    gotowa_moc = 1
    gameplay.set_difficulty(EASY)
    gameplay.set_game_mode(SURVIVAL, mobs.target(LOCAL_PLAYER))
    mobs.give(mobs.target(LOCAL_PLAYER), BOW, 1)
    mobs.give(mobs.target(LOCAL_PLAYER), ARROW, 64)
    mobs.give(mobs.target(LOCAL_PLAYER), DIAMOND_SWORD, 1)
    spawnuj(ZOMBIE, 5, -9, 0, 900)

player.on_chat("start", start)
```

Nazwy stałych i metod sprawdź w palecie używanej wersji przed zapisaniem.

## Mod ucznia

W START diamentowy miecz działa normalnie. Nie rejestruj zdarzenia mocy. Półprodukt ma zdarzenie z samym komunikatem. FINAL:

```python
def naladuj_moc():
    global gotowa_moc
    loops.pause(5000)
    gotowa_moc = 1
    player.say("Moc gotowa!")

def piorunowy_miecz():
    global gotowa_moc
    if gotowa_moc == 1:
        mobs.spawn(LIGHTNING_BOLT, pos(0, 0, 3))
        gotowa_moc = 0
        player.say("PIORUN!")
        naladuj_moc()
    else:
        player.say("Moc się ładuje")

player.on_item_interacted(DIAMOND_SWORD, piorunowy_miecz)
```

Jeśli synchroniczna pauza w `naladuj_moc` blokuje obsługę zdarzenia w konkretnej wersji, przygotuj cooldown przez odliczanie w tle/bloczek „uruchom w tle”. Przetestuj pięć szybkich użyć.

## Warianty personalizacji

Bezpieczne opcje: krótszy cooldown, glowstone, chwilowa pogoda Thunder, efekt 4 bloki przed graczem albo tytuł. Nie używaj TNT, ognia ani pioruna w pozycji `pos(0, 0, 0)`.

## Kryteria jakości

- START to kompletna gra bez Piorunowego Miecza;
- w 90 sekundach widać odliczanie, falę, trafienie, wynik, drop i zapowiedź bossa;
- moc działa tylko przy użyciu właściwego przedmiotu;
- jeden klik nie tworzy kilku efektów;
- cooldown uniemożliwia spam;
- piorun nie trafia gracza ani nie niszczy areny;
- `testmod` tworzy pojedynczego zombie w bezpiecznym miejscu;
- `reset` czyści moby i przywraca stan początkowy.

## Dokumentacja API

- zdarzenie użycia przedmiotu: https://minecraft.makecode.com/reference/player/on-item-used
- moby i selektory: https://minecraft.makecode.com/reference/mobs
- dawanie przedmiotów: https://minecraft.makecode.com/reference/mobs/give
- zasady gry: https://minecraft.makecode.com/reference/gameplay/set-game-rule
