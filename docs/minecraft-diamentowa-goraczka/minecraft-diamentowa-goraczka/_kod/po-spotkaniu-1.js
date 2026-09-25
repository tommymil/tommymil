// ===== KOD UCZNIA – po spotkaniu 1 =====
function zbudujKopalnie () {
    blocks.fill(GLASS, world(-8, -61, -8), world(8, -54, 8), FillOperation.Hollow)
    blocks.fill(AIR, world(-7, -54, -7), world(7, -54, 7), FillOperation.Replace)
    dekoracje()
    for (let index = 0; index <= 3; index++) {
        blocks.fill(STONE, world(-7, -60 + index, -7), world(7, -60 + index, 7), FillOperation.Replace)
        player.execute("playsound dig.stone @p")
        loops.pause(400)
    }
}
function ukryjSkarby () {
    for (let index = 0; index < 10; index++) {
        blocks.place(DIAMOND_ORE, world(randint(-7, 7), randint(-60, -57), randint(-7, 7)))
    }
    for (let index = 0; index < 3; index++) {
        blocks.place(GOLD_ORE, world(randint(-7, 7), randint(-60, -57), randint(-7, 7)))
    }
    for (let index = 0; index < 2; index++) {
        blocks.place(EMERALD_ORE, world(randint(-7, 7), randint(-60, -57), randint(-7, 7)))
    }
    for (let index = 0; index < 4; index++) {
        blocks.place(TNT, world(randint(-7, 7), randint(-60, -57), randint(-7, 7)))
    }
}
player.onChat("hej", function () {
    player.say("Witaj w mojej grze!")
})
player.onChat("start", function () {
    player.teleport(world(0, -60, 13))
    pioruny()
    player.execute("playsound mob.enderdragon.growl @p")
    gameplay.title(mobs.target(LOCAL_PLAYER), "DIAMENTOWA GORĄCZKA", "moja gra")
})
player.onChat("buduj", function () {
    player.teleport(world(0, -60, 13))
    zbudujKopalnie()
    ukryjSkarby()
})
player.onChat("wejdz", function () {
    player.teleport(world(0, -56, 0))
})
