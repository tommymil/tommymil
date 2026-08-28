# Przygotowanie projektu: Obrona Magicznej Fortecy

Plik dotyczy `l04-minecraft-magiczna-forteca.md`.

## Tryb pracy

Kod przygotuj i przetestuj w MakeCode Python, następnie przełącz na Blocks. Wszystkie trzy wersje zapisz z aktywnym widokiem Blocks. Po konwersji sprawdź, czy funkcje fal, pętle, pauzy i spawnowanie mobów są czytelne.

Tryb końcowy: **MakeCode Blocks**. Python służy tutaj tylko do szybkiego przygotowania projektu.

Pliki: `Obrona-Fortecy-start.mcworld`, `-polprodukt.mcworld`, `-final.mcworld`.

## Standard START

START musi zawierać kompletną grę:

- funkcję budującą podłogę, fosę, mury, wieże, bramę i punkt naprawy;
- `start`: budowa, Easy, survival, łuk, 64 strzały, miecz i jedzenie;
- trzy fale z odliczaniem i spawnem z minimum dwóch stron;
- nocny klimat, wyłączony mob griefing oraz zachowany ekwipunek;
- bossa po fali 3, tytuł zwycięstwa i `reset`;
- skróty `fala2` i `boss` do demo.

Forteca ma być kompaktowa (około 21×21) i powstawać w kilka sekund. Punkty spawnu muszą być 8–12 bloków od gracza, na pełnym podłożu, poza murem.

## Rdzeń techniczny

Dostosuj nazwy stałych do palety bieżącej wersji MakeCode. Poniższy kod jest źródłem do konwersji:

```python
def spawnuj(mob, ile, x, z, odstep):
    for i in range(ile):
        mobs.spawn(mob, pos(x, 0, z))
        loops.pause(odstep)

def fala_1():
    gameplay.title(mobs.target(ALL_PLAYERS), "FALA 1", "Zombie od bramy!")
    spawnuj(ZOMBIE, 4, 0, 0, 900)

def fala_2():
    gameplay.title(mobs.target(ALL_PLAYERS), "FALA 2", "Atak z dwóch stron")
    spawnuj(ZOMBIE, 3, -9, 0, 750)
    spawnuj(SKELETON, 2, 9, 0, 1100)

def fala_3():
    gameplay.title(mobs.target(ALL_PLAYERS), "FALA 3", "Ostatni szturm")
    spawnuj(ZOMBIE, 5, 0, 10, 650)
    spawnuj(SKELETON, 3, 9, 0, 850)
```

W wersji startowej sekwencja fal może używać kontrolowanych pauz. Jeżeli implementujesz licznik żywych mobów, uruchamiaj kolejną falę po spadku do zera. Nie uzależniaj pokazu od eksperymentalnego API.

## Mod ucznia

START nie zawiera `fala_pajakow`. Półprodukt zawiera pustą funkcję podłączoną do komendy `testmod`. FINAL:

```python
def fala_pajakow():
    gameplay.title(mobs.target(ALL_PLAYERS), "NOCNY RÓJ", "Boczna brama!")
    for i in range(4):
        mobs.spawn(SPIDER, pos(9, 0, 0))
        loops.pause(900)
```

Po konwersji uczeń pracuje na jednej funkcji. Wywołaj ją po fali 3 dopiero w FINAL. `testmod` pozwala sprawdzić ją bez rozgrywania całej gry.

## Budowa i bezpieczeństwo

- podłoga areny musi być pełna; pod spawnem nie może być fosy;
- mob griefing wyłączone, trudność Easy;
- nie używaj creeperów w wersji obowiązkowej;
- ustaw punkt odrodzenia wewnątrz fortecy;
- `reset` usuwa moby z obszaru, odbudowuje bramę i przywraca Creative;
- zachowaj kopię świata sprzed budowy.

## Kryteria jakości

- START zapewnia co najmniej 2 minuty gry i ma wygraną/przegraną;
- łuk i strzały są w ekwipunku przed pierwszą falą;
- każda fala ma tytuł, przerwę na reakcję i inny skład;
- boss nie pojawia się podczas demo bez świadomego skrótu;
- mod ucznia można testować osobno;
- cztery pająki nie spawnują w jednym bloku jednocześnie;
- ponowne `reset → start` daje taki sam stabilny stan;
- po konwersji projekt nie wraca samoczynnie do Python.

## Dokumentacja API

- spawnowanie mobów: https://minecraft.makecode.com/reference/mobs/spawn
- trudność: https://minecraft.makecode.com/reference/gameplay/set-difficulty
- tryb gry: https://minecraft.makecode.com/reference/gameplay/set-game-mode
- pauza: https://minecraft.makecode.com/reference/loops/pause
