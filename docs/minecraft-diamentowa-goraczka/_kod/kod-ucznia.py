# ===== KOD UCZNIA – spotkanie 1 =====
def zbudujKopalnie():
    blocks.fill(GLASS, world(-8, -61, -8), world(8, -54, 8), FillOperation.HOLLOW)
    blocks.fill(AIR, world(-7, -54, -7), world(7, -54, 7), FillOperation.REPLACE)
    dekoracje()
    for index in range(4):
        blocks.fill(STONE, world(-7, -60 + index, -7), world(7, -60 + index, 7), FillOperation.REPLACE)
        player.execute("playsound dig.stone @p")
        loops.pause(400)

def ukryjSkarby():
    for index2 in range(10):
        blocks.place(DIAMOND_ORE, world(randint(-7, 7), randint(-60, -57), randint(-7, 7)))
    for index3 in range(3):
        blocks.place(GOLD_ORE, world(randint(-7, 7), randint(-60, -57), randint(-7, 7)))
    for index4 in range(2):
        blocks.place(EMERALD_ORE, world(randint(-7, 7), randint(-60, -57), randint(-7, 7)))
    for index5 in range(4):
        blocks.place(TNT, world(randint(-7, 7), randint(-60, -57), randint(-7, 7)))

def on_hej():
    player.say("Witaj w mojej grze!")
player.on_chat("hej", on_hej)

def on_start():
    player.teleport(world(0, -60, 13))
    pioruny()
    player.execute("playsound mob.enderdragon.growl @p")
    gameplay.title(mobs.target(LOCAL_PLAYER), "DIAMENTOWA GORĄCZKA", "moja gra")
player.on_chat("start", on_start)

def on_buduj():
    player.teleport(world(0, -60, 13))
    zbudujKopalnie()
    ukryjSkarby()
player.on_chat("buduj", on_buduj)

def on_wejdz():
    player.teleport(world(0, -56, 0))
player.on_chat("wejdz", on_wejdz)

# ===== KOD UCZNIA – spotkanie 2 =====
def on_diament():
    global punkty
    if gra:
        punkty += 1
        pokazPunkty()
        player.execute("playsound random.orb @p")
blocks.on_block_broken(DIAMOND_ORE, on_diament)

def on_zloto():
    global punkty
    if gra:
        punkty += 3
        pokazPunkty()
        player.execute("playsound random.levelup @p")
blocks.on_block_broken(GOLD_ORE, on_zloto)

def on_szmaragd():
    supermoc()
blocks.on_block_broken(EMERALD_ORE, on_szmaragd)

def on_tnt():
    global punkty
    if gra:
        punkty += -2
        pokazPunkty()
        wybuch()
blocks.on_block_broken(TNT, on_tnt)

def on_stop():
    global czas
    czas = 0
player.on_chat("stop", on_stop)

def on_koniec():
    global czas
    czas = 0
player.on_chat("koniec", on_koniec)

def on_graj():
    global punkty, czas, gra, rekord
    if gra:
        player.say("Gra już trwa!")
    else:
        punkty = 0
        czas = 60
        pokazPunkty()
        player.teleport(world(0, -60, 13))
        zbudujKopalnie()
        ukryjSkarby()
        player.execute("kill @e[type=item]")
        player.execute("clear @p")
        player.teleport(world(0, -56, 0))
        odliczanie()
        mobs.give(mobs.target(LOCAL_PLAYER), DIAMOND_PICKAXE, 1)
        gameplay.set_game_mode(SURVIVAL, mobs.target(LOCAL_PLAYER))
        gra = True
        while czas > 0:
            pokazCzas()
            loops.pause(1000)
            czas += -1
        gra = False
        gameplay.set_game_mode(CREATIVE, mobs.target(LOCAL_PLAYER))
        player.teleport(world(0, -60, 13))
        gameplay.title(mobs.target(LOCAL_PLAYER), "KONIEC!", "Punkty: " + str(punkty))
        fajerwerki()
        if punkty > rekord:
            rekord = punkty
            zapiszRekord()
            gameplay.title(mobs.target(LOCAL_PLAYER), "§6NOWY REKORD!", "" + str(rekord))
            player.execute("playsound ui.toast.challenge_complete @p")
player.on_chat("graj", on_graj)

przygotujTablice()
