# ==================================================
#  SILNIK GRY "DIAMENTOWA GORĄCZKA"
#  Przygotowany przez nauczyciela. Nie zmieniamy!
#  Supermoce znajdziesz w kategorii Funkcje.
# ==================================================
punkty = 0
rekord = 0
czas = 0
gra = False

def pioruny():
    player.execute("summon lightning_bolt 10 -60 10")
    loops.pause(300)
    player.execute("summon lightning_bolt -10 -60 10")
    loops.pause(300)
    player.execute("summon lightning_bolt -10 -60 -10")
    loops.pause(300)
    player.execute("summon lightning_bolt 10 -60 -10")

def dekoracje():
    blocks.fill(SEA_LANTERN, world(-9, -60, -9), world(-9, -52, -9), FillOperation.REPLACE)
    blocks.fill(SEA_LANTERN, world(9, -60, -9), world(9, -52, -9), FillOperation.REPLACE)
    blocks.fill(SEA_LANTERN, world(-9, -60, 9), world(-9, -52, 9), FillOperation.REPLACE)
    blocks.fill(SEA_LANTERN, world(9, -60, 9), world(9, -52, 9), FillOperation.REPLACE)
    blocks.fill(GOLD_BLOCK, world(-2, -61, 11), world(2, -61, 15), FillOperation.REPLACE)

def odliczanie():
    gameplay.title(mobs.target(LOCAL_PLAYER), "§e3", "Przygotuj kilof...")
    player.execute("playsound note.pling @p ~ ~ ~ 1 0.5")
    loops.pause(1000)
    gameplay.title(mobs.target(LOCAL_PLAYER), "§62", "Przygotuj kilof...")
    player.execute("playsound note.pling @p ~ ~ ~ 1 0.7")
    loops.pause(1000)
    gameplay.title(mobs.target(LOCAL_PLAYER), "§c1", "Przygotuj kilof...")
    player.execute("playsound note.pling @p ~ ~ ~ 1 0.9")
    loops.pause(1000)
    gameplay.title(mobs.target(LOCAL_PLAYER), "§a§lSTART!", "Szukaj skarbów!")
    player.execute("playsound random.levelup @p")

def pokazCzas():
    if czas <= 10:
        player.execute("title @p actionbar §c§lCzas: " + str(czas))
        player.execute("playsound random.click @p")
    else:
        player.execute("title @p actionbar §eCzas: " + str(czas))

def przygotujTablice():
    player.execute("scoreboard objectives remove wynik")
    player.execute("scoreboard objectives add wynik dummy §bPunkty")
    player.execute("scoreboard objectives setdisplay sidebar wynik")
    pokazPunkty()
    zapiszRekord()

def pokazPunkty():
    player.execute("scoreboard players set @p wynik " + str(punkty))

def zapiszRekord():
    player.execute("scoreboard players set REKORD wynik " + str(rekord))

def supermoc():
    player.execute("effect @p haste 10 2 true")
    player.execute("effect @p speed 10 1 true")
    player.execute("particle minecraft:totem_particle ~ ~1 ~")
    player.execute("playsound random.totem @p")
    gameplay.title(mobs.target(LOCAL_PLAYER), "§aSUPERMOC!", "Szybkie kopanie przez 10 s")

def wybuch():
    player.execute("particle minecraft:huge_explosion_emitter ~ ~1 ~")
    player.execute("playsound random.explode @p")
    player.execute("camerashake add @p 1 1 positional")
    player.execute("effect @p blindness 3 0 true")
    gameplay.title(mobs.target(LOCAL_PLAYER), "§cBUM!", "Pułapka!")

def fajerwerki():
    for i in range(10):
        player.execute("particle minecraft:huge_explosion_emitter " + str(randint(-7, 7)) + " -48 " + str(randint(-7, 7)))
        player.execute("particle minecraft:totem_particle " + str(randint(-7, 7)) + " -50 " + str(randint(-7, 7)))
        player.execute("summon fireworks_rocket " + str(randint(-7, 7)) + " -55 " + str(randint(-7, 7)))
        player.execute("playsound firework.large_blast @p")
        loops.pause(300)
    player.execute("playsound firework.twinkle @p")
