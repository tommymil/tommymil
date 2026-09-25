// ===== KOD UCZNIA – dopisane na spotkaniu 2 =====
przygotujTablice()

blocks.onBlockBroken(DIAMOND_ORE, function () {
    if (gra) {
        punkty += 1
        pokazPunkty()
        player.execute("playsound random.orb @p")
    }
})
blocks.onBlockBroken(GOLD_ORE, function () {
    if (gra) {
        punkty += 3
        pokazPunkty()
        player.execute("playsound random.levelup @p")
    }
})
blocks.onBlockBroken(EMERALD_ORE, function () {
    supermoc()
})
blocks.onBlockBroken(TNT, function () {
    if (gra) {
        punkty += -2
        pokazPunkty()
        wybuch()
    }
})
player.onChat("stop", function () {
    czas = 0
})
player.onChat("koniec", function () {
    czas = 0
})
player.onChat("graj", function () {
    if (gra) {
        player.say("Gra już trwa!")
    } else {
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
        gameplay.setGameMode(SURVIVAL, mobs.target(LOCAL_PLAYER))
        gra = true
        while (czas > 0) {
            pokazCzas()
            loops.pause(1000)
            czas += -1
        }
        gra = false
        gameplay.setGameMode(CREATIVE, mobs.target(LOCAL_PLAYER))
        player.teleport(world(0, -60, 13))
        gameplay.title(mobs.target(LOCAL_PLAYER), "KONIEC!", "Punkty: " + punkty)
        fajerwerki()
        if (punkty > rekord) {
            rekord = punkty
            zapiszRekord()
            gameplay.title(mobs.target(LOCAL_PLAYER), "§6NOWY REKORD!", "" + rekord)
            player.execute("playsound ui.toast.challenge_complete @p")
        }
    }
})
