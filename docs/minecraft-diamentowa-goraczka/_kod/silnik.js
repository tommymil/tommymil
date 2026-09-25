// ==================================================
//  SILNIK GRY "DIAMENTOWA GORĄCZKA"
//  Przygotowany przez nauczyciela. Nie zmieniamy!
//  Supermoce znajdziesz w kategorii Funkcje.
// ==================================================
let punkty = 0
let rekord = 0
let czas = 0
let gra = false

function pioruny () {
    player.execute("summon lightning_bolt 10 -60 10")
    loops.pause(300)
    player.execute("summon lightning_bolt -10 -60 10")
    loops.pause(300)
    player.execute("summon lightning_bolt -10 -60 -10")
    loops.pause(300)
    player.execute("summon lightning_bolt 10 -60 -10")
}

function dekoracje () {
    blocks.fill(SEA_LANTERN, world(-9, -60, -9), world(-9, -52, -9), FillOperation.Replace)
    blocks.fill(SEA_LANTERN, world(9, -60, -9), world(9, -52, -9), FillOperation.Replace)
    blocks.fill(SEA_LANTERN, world(-9, -60, 9), world(-9, -52, 9), FillOperation.Replace)
    blocks.fill(SEA_LANTERN, world(9, -60, 9), world(9, -52, 9), FillOperation.Replace)
    blocks.fill(GOLD_BLOCK, world(-2, -61, 11), world(2, -61, 15), FillOperation.Replace)
    // Podłoga kopalni z bedrocka - nie da się przez nią przekopać.
    blocks.fill(BEDROCK, world(-8, -61, -8), world(8, -61, 8), FillOperation.Replace)
}

// Kilof, który w trybie przygody niszczy tylko kamień, rudy i TNT - szkła i podłogi nie ruszy.
function dajKilof () {
    player.execute("give @p diamond_pickaxe 1 0 {\"minecraft:can_destroy\":{\"blocks\":[\"stone\",\"diamond_ore\",\"gold_ore\",\"emerald_ore\",\"tnt\"]}}")
}

function odliczanie () {
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
}

function pokazCzas () {
    if (czas <= 10) {
        player.execute("title @p actionbar §c§lCzas: " + czas)
        player.execute("playsound random.click @p")
    } else {
        player.execute("title @p actionbar §eCzas: " + czas)
    }
}

function przygotujTablice () {
    player.execute("scoreboard objectives remove wynik")
    player.execute("scoreboard objectives add wynik dummy §bPunkty")
    player.execute("scoreboard objectives setdisplay sidebar wynik")
    pokazPunkty()
    zapiszRekord()
}

function pokazPunkty () {
    player.execute("scoreboard players set @p wynik " + punkty)
}

function zapiszRekord () {
    player.execute("scoreboard players set REKORD wynik " + rekord)
}

function supermoc () {
    player.execute("effect @p haste 10 2 true")
    player.execute("effect @p speed 10 1 true")
    player.execute("particle minecraft:totem_particle ~ ~1 ~")
    player.execute("playsound random.totem @p")
    gameplay.title(mobs.target(LOCAL_PLAYER), "§aSUPERMOC!", "Szybkie kopanie przez 10 s")
}

function wybuch () {
    player.execute("particle minecraft:huge_explosion_emitter ~ ~1 ~")
    player.execute("playsound random.explode @p")
    player.execute("camerashake add @p 1 1 positional")
    player.execute("effect @p blindness 3 0 true")
    gameplay.title(mobs.target(LOCAL_PLAYER), "§cBUM!", "Pułapka!")
}

function fajerwerki () {
    for (let i = 0; i < 10; i++) {
        player.execute("particle minecraft:huge_explosion_emitter " + randint(-7, 7) + " -48 " + randint(-7, 7))
        player.execute("particle minecraft:totem_particle " + randint(-7, 7) + " -50 " + randint(-7, 7))
        player.execute("summon fireworks_rocket " + randint(-7, 7) + " -55 " + randint(-7, 7))
        player.execute("playsound firework.large_blast @p")
        loops.pause(300)
    }
    player.execute("playsound firework.twinkle @p")
}
