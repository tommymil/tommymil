# Przygotowanie projektu: Agent — Ekspedycja po Kryształ

Plik dotyczy `l06-minecraft-agent-gornik.md`.

## Tryb pracy

Kod przygotuj w MakeCode Python, następnie przełącz i zapisz wszystkie wersje w Blocks: `Agent-Ekspedycja-start.mcworld`, `-polprodukt.mcworld`, `-final.mcworld`.

Tryb końcowy: **MakeCode Blocks**. Python służy tutaj tylko do szybkiego przygotowania projektu.

## Świat i fabuła

Zbuduj trzy krótkie, połączone odcinki:

1. tunel 3×2 długości 10 bloków z kamieniem, złotem i diamentem;
2. bezpiecznie oznaczona przepaść szerokości 3, nad którą Agent stawia most;
3. oświetlona komnata z zamkniętym Kryształem Rdzenia i efektem finału.

Obok przygotuj niewidoczny/oddzielony tor testowy długości 5. `reset` odbudowuje testowe bloki i teleportuje Agenta na start.

## Pełna gra START

Agent już:

- teleportuje się na początek i wybiera właściwy kierunek;
- kopie blok przed sobą oraz nad sobą, tworząc przejście 2 bloki wysokie;
- co cztery kroki stawia pochodnię;
- na oznaczonym odcinku wybiera bloki i buduje most;
- liczy przebytą odległość, pokazuje etapy i dociera do komnaty;
- uruchamia finał z glowstone/tytułem i daje graczowi Kryształ.

Nie usuwaj kopania ani mostu z START. Mod ucznia ma poprawić inteligencję robota, a nie naprawiać trasę.

## Rdzeń pętli

```python
skarby = 0

def kop_tunel(kroki):
    for numer in range(kroki):
        # MOD UCZNIA znajduje się tutaj, przed niszczeniem.
        if agent.detect(AgentDetection.BLOCK, FORWARD):
            agent.destroy(FORWARD)
        if agent.detect(AgentDetection.BLOCK, UP):
            agent.destroy(UP)
        agent.move(FORWARD, 1)

        if numer % 4 == 3:
            agent.set_item(TORCH, 1, 1)
            agent.set_slot(1)
            agent.place(LEFT)
```

Ustaw Agenta jeden blok ponad podłożem i sprawdź, czy `UP` rzeczywiście tworzy przejście na wysokość gracza.

## Mod ucznia

START ma licznik `Skarby` widoczny jako 0, ale nie ma rozpoznawania rudy. Półprodukt ma pusty warunek. FINAL, umieszczony przed `destroy`:

```python
global skarby
if agent.inspect(AgentInspection.BLOCK, FORWARD) == GOLD_BLOCK:
    skarby += 1
    player.say("ZŁOTO! Skarby: " + str(skarby))
elif agent.inspect(AgentInspection.BLOCK, FORWARD) == DIAMOND_BLOCK:
    skarby += 3
    player.say("DIAMENT! Skarby: " + str(skarby))
```

Jeśli `elif` konwertuje się do zbyt złożonego bloczka dla poziomu, pozostaw dwa niezależne warunki `if`. Najważniejsza jest kolejność: inspect przed destroy.

## Komendy

- `start` — intro i ustawienie Agenta;
- `ekspedycja` — pełna sekwencja;
- `test` — pięć kroków w torze testowym;
- `komnata` — teleport do finału;
- `reset` — odbudowa trasy i stanu;
- `lewo`/`prawo` — awaryjne obrócenie Agenta.

## Kryteria jakości

- START przechodzi całą trasę bez ingerencji instruktora;
- nie ma pustki pod Agentem poza kontrolowanym mostem;
- pochodnie nie blokują ruchu;
- most powstaje przed wejściem Agenta na pustkę;
- skaner liczy każdy blok raz i działa przed zniszczeniem;
- kamień nie zmienia wyniku;
- `reset` umożliwia minimum trzy identyczne testy;
- pełna ekspedycja trwa 60–100 sekund;
- po konwersji wszystkie używane kategorie są dostępne jako Blocks.

## Dokumentacja API

- ruch Agenta: https://minecraft.makecode.com/tutorials/python/agent-moves
- wykrywanie i budowanie: https://minecraft.makecode.com/tutorials/python/agent-build
- inspect: https://minecraft.makecode.com/reference/agent/inspect
