# Diamentowa Gorączka 1: Budujemy świat gry
Subject: Minecraft Education
Level: Poziom 2 - 10-12 lat
Tags: MakeCode, Zdarzenia, Współrzędne, Funkcje, Pętle, Losowość
Opis: Pierwsze z dwóch spotkań. Z gotowego silnika z efektami dzieci składają intro gry z piorunami, kopalnię rosnącą warstwami i losowo ukryte skarby.

## [intro] Powitanie i pokaz gotowej gry (11 min)

### Co robić teraz
- **Powitanie:** przywitaj grupę, sprawdź obecność i czy wszyscy słyszą i widzą Twój ekran.
- **Zasady online:** mikrofony wyciszone, pytanie = podniesiona ręka w rozmowie albo wiadomość na czacie, a gdy coś nie działa, udostępniasz mi swój ekran.
- **Zapowiedź:** „Za dwa spotkania każdy z was będzie miał własną grę w Minecrafcie. Dziś budujemy świat gry, a za tydzień zamienimy go w prawdziwą grę”.
- **Pokaz wow:** udostępnij okno Minecrafta, wpisz `start` (pioruny i ryk smoka), potem `graj` i rozegraj jedną pełną rundę.
- W trakcie rundy specjalnie wykop **szmaragd** (supermoc) i **TNT** (BUM!), a na koniec pokaż fajerwerki i napis NOWY REKORD.
- Przełącz udostępnianie na okno **Ekran dla uczniów** z aplikacji (bez Twoich notatek) i pokaż planszę gry: przekrój kopalni i tabelę skarbów (diament +1, złoto +3, szmaragd = supermoc, TNT = −2).
- Szybki test: każdy otwiera u siebie świat „Diamentowa Gorączka” i pisze na czacie „jestem”.

### Wskazówki
- [tempo] Nie zaczynaj od kodu. Najpierw pokaz, żeby każde dziecko pomyślało „chcę to mieć”.
- [podpowiedź] Przed zajęciami (2–3 dni wcześniej) wyślij rodzicom wiadomość z materiałów niżej: świat `.mcworld`, link do **projektu startowego** `.mkcd` (z „Lekcji startowej” w aplikacji), instalacja i logowanie w Minecraft Education, test dźwięku.
- [podpowiedź] Do pokazu otwórz u siebie projekt `diamentowa-goraczka-pelna-gra.mkcd`. Kliknij ▶ i dopiero potem wpisuj komendy.
- [błąd] Jeśli efekt (np. trzęsienie kamery lub cząsteczki) daje czerwony błąd w czacie, to twoja wersja Minecrafta go nie zna. Usuń tę jedną linię z silnika, reszta zadziała.
- [tempo] Online wszystko trwa dłużej. Jeśli ktoś nie ma świata albo Minecrafta, nie zatrzymuj grupy: poproś rodzica o pomoc po zajęciach, a dziecko niech na razie ogląda Twój ekran.
- [podpowiedź] Najwygodniej, gdy dziecko ma dwa ekrany: na jednym rozmowa, na drugim Minecraft. Z jednym ekranem przełącza się klawiszami Alt+Tab. Pokaż to na początku.

### Materiały
- [obraz] Tak działa gra | infografiki/01-diamentowa-goraczka-gra.png
- [kod] Wiadomość do rodziców (wyślij 2–3 dni przed zajęciami):
  ```
  Dzień dobry! W [dzień] o [godzina] zaczynamy kurs „Diamentowa Gorączka”: dziecko
  zaprogramuje własną minigrę w Minecraft Education. Prosimy przed zajęciami:
  1. Zainstalować Minecraft Education i zalogować się kontem szkolnym dziecka.
  2. Pobrać świat z linku: [LINK DO .mcworld] i otworzyć plik dwuklikiem
     (świat pojawi się w grze pod nazwą „Diamentowa Gorączka”).
  3. Pobrać projekt startowy gry (plik .mkcd): [LINK DO PROJEKTU STARTOWEGO]
     (otworzymy go razem na zajęciach, nie trzeba nic z nim robić).
  4. Sprawdzić mikrofon i słuchawki w [Teams/Zoom/Meet].
  Najlepiej, gdy dziecko ma mysz i drugi ekran, ale nie jest to konieczne.
  Link do spotkania: [LINK]
  ```
- [kod] Przygotowanie świata (Twój czat w grze, raz, zanim wyeksportujesz świat dla dzieci):
  ```
  /tp @s 0 -60 13
  /setworldspawn 0 -60 13
  /tickingarea add -16 -64 -16 16 -40 20 kopalnia
  /gamerule dodaylightcycle false
  /time set day
  /gamerule doweathercycle false
  /weather clear
  /gamerule dofiretick false
  /gamerule domobspawning false
  ```
- [kod] Pełna gra do pokazu (JavaScript; dla nauczyciela, ten sam kod co w diamentowa-goraczka-PELNA-GRA.txt):
  ```
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
  ```

## [concept] Code Builder i supermoce silnika (9 min)

### Co robić teraz
- Dzieci wchodzą do świata i naciskają **C**. Otwiera się Code Builder, wybieramy MakeCode.
- **Otwieramy projekt startowy:** dzieci w MakeCode wybierają **MakeCode → Importuj → Importuj plik** i wskazują pobrany `diamentowa-goraczka-start.mkcd` (link z wiadomości albo z czatu). Projekt otwiera się od razu w bloczkach. Pokazuj to krok po kroku na swoim ekranie.
- Pokaż na swoim ekranie: kategorie bloczków, przeciąganie, kosz (wyrzucanie bloczka) i zielony **▶** (uruchomienie kodu).
- Otwórz kategorię **Funkcje** (Zaawansowane → Funkcje) i pokaż supermoce: pioruny, dekoracje, odliczanie, pokazCzas, przygotujTablice, pokazPunkty, zapiszRekord, supermoc, wybuch, fajerwerki.
- Hasło na dziś: „Programiści gier też korzystają z gotowego silnika. Wy jesteście projektantami: decydujecie, KIEDY użyć której supermocy”.

### Wskazówki
- [podpowiedź] Projekt startowy robisz przed kursem z pliku `diamentowa-goraczka-START.txt` (wklejasz, przełączasz na Bloki, zapisujesz jako `.mkcd`) i wgrywasz w aplikacji jako „Lekcja startowa”. W prezenterze skopiujesz link do pobrania.
- [błąd] Dziecko nie może znaleźć pliku: zwykle jest w folderze Pobrane. Poproś, żeby udostępniło ekran, i poprowadź je.
- [podpowiedź] Dzieci pracują tylko na bloczkach. Kod JavaScript/Python widzisz tylko Ty.
- [tempo] Ten krok online bywa najdłuższy. Kto skończył, niech napisze na czacie „mam supermoce”. Pomóż osobno tym, którzy utknęli (niech udostępnią ekran).
- [błąd] Dziecko nie widzi supermocy: projekt jest bez silnika albo otwarty został inny projekt.
- [podpowiedź] Ciało funkcji silnika jest na obszarze roboczym. Poproś dzieci, żeby go nie ruszały i przesunęły na bok.

### Materiały
- [obraz] Supermoce silnika gry | infografiki/02-supermoce-silnika.png
- [kod] Silnik z projektu startowego (JavaScript; dla nauczyciela, ten sam kod co w diamentowa-goraczka-START.txt):
  ```
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
  ```

## [guided] Pierwsza komenda: hej (6 min)

### Co robić teraz
- „Program to przepis. Najpierw mówimy, KIEDY coś ma się stać (zdarzenie), a potem CO ma się stać (instrukcje)”.
- Z kategorii **Gracz** przeciągnij bloczek „przy poleceniu czatu” i wpisz `hej`.
- Do środka włóż „powiedz” z dowolnym powitaniem.
- Kliknij **▶**. W grze naciśnij **T**, wpisz `hej` i Enter.
- Szybkie wyzwanie: dodaj drugi bloczek „powiedz” albo drugą komendę, np. `pa`.

### Wskazówki
- [błąd] Komenda nic nie robi: nie kliknięto ▶ albo w kodzie jest literówka. Komendy piszemy małymi literami, bez spacji i bez polskich znaków.
- [błąd] Po każdej zmianie w kodzie trzeba ponownie kliknąć ▶.

### Materiały
- [obraz] Pierwsza komenda | infografiki/03-komenda-hej.png
- [kod] MakeCode, bloczki:
  ```
  przy poleceniu czatu [hej]
    powiedz [Witaj w mojej grze!]
  ```
- [kod] JavaScript:
  ```
  player.onChat("hej", function () {
      player.say("Witaj w mojej grze!")
  })
  ```

## [concept] Współrzędne X Y Z (5 min)

### Co robić teraz
- Poproś dzieci, żeby przeszły kilka kroków i powiedziały, która liczba w lewym górnym rogu się zmienia. Potem niech polecą w górę.
- **X** i **Z** to kierunki po ziemi, **Y** to wysokość.
- Ważne miejsca: kopalnia stoi w X 0, Z 0, platforma widzów w X 0, Z 13. Na trawie stoimy na Y −60.
- Pozycja **świata** to stały adres, taki sam dla każdego. Pozycja z falką **~** jest liczona od gracza.

### Wskazówki
- [podpowiedź] Jeśli współrzędnych nie widać: ustawienia świata → „Pokaż współrzędne”.
- [podpowiedź] Dla szybkich: komenda `skok` z teleportem na ~ 0 ~ 20 ~ 0. Czym różni się od teleportu na pozycję świata?

### Materiały
- [obraz] Współrzędne X Y Z | infografiki/04-wspolrzedne-xyz.png

## [guided] Intro gry: komenda start (11 min)

### Co robić teraz
- Dzieci składają intro: teleport na platformę widzów, **wywołanie pioruny**, dźwięk smoka i tytuł gry.
- W podtytule każde dziecko wpisuje nazwę swojego studia gier albo imię. To ważne, bo dzięki temu gra staje się „moja”.
- Test: ▶, a potem w grze `start`.
- Dla szybkich: kolorowe napisy (§b błękitny, §6 złoty, §c czerwony, §a zielony) i inne dźwięki intro.

### Wskazówki
- [tempo] To pierwszy efekt wow zrobiony własnoręcznie. Daj chwilę na zachwyt, zanim przejdziecie dalej.
- [podpowiedź] Znak § wpiszesz przez Alt + 21 na klawiaturze numerycznej.
- [podpowiedź] Inne dźwięki do intro: mob.wither.spawn, random.totem, raid.horn.
- [błąd] Teleport w złe miejsce: pomylona pozycja świata z pozycją z falką albo zgubiony minus przy −60.

### Materiały
- [obraz] Intro gry: komenda start | infografiki/05-komenda-start.png
- [kod] MakeCode, bloczki:
  ```
  przy poleceniu czatu [start]
    teleportuj do (świat 0 -60 13)
    wywołanie pioruny
    wykonaj [playsound mob.enderdragon.growl @p]
    pokaż tytuł [DIAMENTOWA GORĄCZKA] podtytuł [gra studia Ola Games]
  ```
- [kod] JavaScript:
  ```
  player.onChat("start", function () {
      player.teleport(world(0, -60, 13))
      pioruny()
      player.execute("playsound mob.enderdragon.growl @p")
      gameplay.title(mobs.target(LOCAL_PLAYER), "DIAMENTOWA GORĄCZKA", "gra studia Ola Games")
  })
  ```

## [challenge] Pokaz intro na żywo (3 min)

### Co robić teraz
- Każdy pisze na czacie nazwę swojego studia gier.
- 2–3 ochotników udostępnia ekran i wpisuje `start`. Reszta reaguje w rozmowie (brawa, emotki).
- Zapowiedź po przerwie: „Teraz zbudujecie własną supermoc: kopalnię, która rośnie na waszych oczach”.

### Wskazówki
- [tempo] Jeśli ktoś nie skończył, dokończy w trakcie pokazów. Poproś, żeby udostępnił ekran, i podpowiedz, który bloczek przeciągnąć.
- [podpowiedź] Przy każdym pokazie zapytaj: „Jak się nazywa twoje studio?”. Dzieci lubią mówić o swojej grze.

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy: dzieci wstają od ekranów, piją wodę, rozprostowują się.

### Wskazówki
- [tempo] Nie zamykajcie Minecrafta ani Code Buildera. Po przerwie od razu wracamy do projektu.

## [review] Powrót i test startu (3 min)

### Co robić teraz
- Kliknij ▶ i wpisz `start`. Czy pioruny działają u wszystkich?
- Krótkie przypomnienie: zdarzenie, instrukcja, pozycja świata.

## [guided] Własna funkcja: zbudujKopalnie (15 min)

### Co robić teraz
- „Na pierwszej części używaliście supermocy od nauczyciela. Teraz zrobicie własną”.
- Pokaż na ekranie dla uczniów infografikę z pudełkiem i dwoma zaznaczonymi narożnikami. Bloczek „wypełnij” potrzebuje tylko tych dwóch punktów.
- **Zaawansowane → Funkcje → Utwórz funkcję**, nazwa `zbudujKopalnie`.
- Szklane pudełko: szkło od −8 −61 −8 do 8 −54 8, tryb **pusty środek** (hollow).
- Zdejmij sufit: powietrze od −7 −54 −7 do 7 −54 7, tryb zamień.
- Na koniec funkcji **wywołanie dekoracje** (supermoc silnika).
- Komenda `buduj`: teleport na platformę widzów, potem **wywołanie zbudujKopalnie**. Test: ▶ i `buduj`.

### Wskazówki
- [błąd] Najczęstszy błąd to zgubiony minus albo pomylone X z Z. Niech dzieci czytają współrzędne na głos w parach.
- [błąd] Brak teleportu na początku komendy: gracz stojący w kopalni może zostać zamurowany.
- [podpowiedź] Tryb „pusty środek” robi ściany, podłogę i sufit, dlatego sufit zdejmujemy osobnym wypełnieniem.

### Materiały
- [obraz] Funkcja zbudujKopalnie | infografiki/06-funkcja-zbuduj-kopalnie.png
- [kod] MakeCode, bloczki:
  ```
  funkcja zbudujKopalnie
    wypełnij [szkło] od (świat -8 -61 -8) do (świat 8 -54 8) [pusty środek]
    wypełnij [powietrze] od (świat -7 -54 -7) do (świat 7 -54 7) [zamień]
    wywołanie dekoracje

  przy poleceniu czatu [buduj]
    teleportuj do (świat 0 -60 13)
    wywołanie zbudujKopalnie
  ```
- [kod] JavaScript:
  ```
  function zbudujKopalnie () {
      blocks.fill(GLASS, world(-8, -61, -8), world(8, -54, 8), FillOperation.Hollow)
      blocks.fill(AIR, world(-7, -54, -7), world(7, -54, 7), FillOperation.Replace)
      dekoracje()
  }
  player.onChat("buduj", function () {
      player.teleport(world(0, -60, 13))
      zbudujKopalnie()
  })
  ```

## [concept] Kopalnia rośnie: pętla z indeksem (10 min)

### Co robić teraz
- „Kamień mógłby pojawić się od razu, ale w grach fajniej, gdy świat buduje się na oczach gracza”.
- Na końcu funkcji `zbudujKopalnie` dodaj pętlę **dla index od 0 do 3**.
- W środku: wypełnij kamień od −7 (−60 + index) −7 do 7 (−60 + index) 7, dźwięk `playsound dig.stone @p`, pauza 400 ms.
- Pokaż na infografice, ile wynosi Y w kolejnych obrotach: −60, −59, −58, −57. Zapytaj na czacie: „Ile wyniesie Y, gdy index = 2?”.
- Test: `buduj` kilka razy z rzędu. Kopalnia za każdym razem znika i rośnie od nowa.

### Wskazówki
- [podpowiedź] Działanie „−60 + index” jest w kategorii Matematyka. Wkłada się je w miejsce liczby Y.
- [błąd] Kamień pojawia się tylko w jednej warstwie: w miejsce Y wpisano liczbę zamiast działania z indeksem.
- [tempo] To efekt wow tej części. Pozwól dzieciom kilka razy zobaczyć budowę.

### Materiały
- [obraz] Kopalnia rośnie: pętla | infografiki/07-kopalnia-rosnie-petla.png
- [kod] MakeCode, bloczki (dopisz na końcu funkcji):
  ```
  dla index od 0 do 3
    wypełnij [kamień] od (świat -7 (-60 + index) -7) do (świat 7 (-60 + index) 7) [zamień]
    wykonaj [playsound dig.stone @p]
    pauza (ms) 400
  ```
- [kod] JavaScript (cała funkcja):
  ```
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
  ```

## [guided] Ukryte skarby: losowanie (10 min)

### Co robić teraz
- Nowa funkcja `ukryjSkarby`: pętla **powtórz 10 razy** z bloczkiem „umieść rudę diamentu” w losowym miejscu.
- W miejsce liczb pozycji świata wkładamy **wybierz losowo … do …** (Matematyka): X i Z od −7 do 7, Y od −60 do −57.
- Kolejne pętle robimy przez prawy przycisk → Duplikuj: złoto 3 razy, szmaragd 2 razy, TNT 4 razy.
- **Dzieci same decydują**, ile ukryć każdego skarbu, i piszą swoje liczby na czacie.
- W komendzie `buduj` dodaj **wywołanie ukryjSkarby**. Dodaj też komendę `wejdz` (teleport na 0 −56 0).

### Wskazówki
- [podpowiedź] To moment, w którym gra staje się „inna za każdym razem”. Zapytaj: „Gdzie jest diament? Nie wiadomo, i o to chodzi!”
- [błąd] Skarby pojawiają się poza kopalnią: zakres losowania szerszy niż −7…7 albo Y poza −60…−57.

### Materiały
- [obraz] Ukryte skarby: losowanie | infografiki/08-ukryte-skarby-losowanie.png
- [kod] MakeCode, bloczki:
  ```
  funkcja ukryjSkarby
    powtórz 10 razy
      umieść [ruda diamentu] w (świat (wybierz losowo -7 do 7) (wybierz losowo -60 do -57) (wybierz losowo -7 do 7))
    powtórz 3 razy
      umieść [ruda złota] w (świat (wybierz losowo -7 do 7) (wybierz losowo -60 do -57) (wybierz losowo -7 do 7))
    powtórz 2 razy
      umieść [ruda szmaragdu] w (świat (wybierz losowo -7 do 7) (wybierz losowo -60 do -57) (wybierz losowo -7 do 7))
    powtórz 4 razy
      umieść [TNT] w (świat (wybierz losowo -7 do 7) (wybierz losowo -60 do -57) (wybierz losowo -7 do 7))

  przy poleceniu czatu [buduj]
    teleportuj do (świat 0 -60 13)
    wywołanie zbudujKopalnie
    wywołanie ukryjSkarby

  przy poleceniu czatu [wejdz]
    teleportuj do (świat 0 -56 0)
  ```
- [kod] JavaScript:
  ```
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
  player.onChat("buduj", function () {
      player.teleport(world(0, -60, 13))
      zbudujKopalnie()
      ukryjSkarby()
  })
  player.onChat("wejdz", function () {
      player.teleport(world(0, -56, 0))
  })
  ```

## [summary] Test kopania i zapowiedź (7 min)

### Co robić teraz
- `buduj` → `wejdz` → kopiemy w trybie kreatywnym. Część skarbów widać przez szybę.
- Zagadka: „Czy w kopalni zawsze jest dokładnie 10 diamentów?” (Nie zawsze, bo losowanie może dwa razy trafić w to samo miejsce.)
- Haczyk na koniec: „Na razie TNT jest nudne, a szmaragd niczego nie daje. Za tydzień będą punkty, pułapki, supermoce i odliczanie czasu”.
- Przypomnij: za tydzień ten sam komputer, ten sam świat i projekt. Niczego nie usuwajcie.

### Wskazówki
- [tempo] Zostaw na koniec chwilę na swobodne kopanie. To nagroda za spotkanie.
- [podpowiedź] Dla szybkich: kolorowe szkło zamiast zwykłego, dźwięk po zbudowaniu kopalni (random.anvil_land).
- [błąd] Projekt zapisuje się sam, ale tylko na tym komputerze i koncie. Kto za tydzień będzie na innym sprzęcie, dostanie projekt „punkt kontrolny” (.mkcd).

### Materiały
- [obraz] Tak działa gra | infografiki/01-diamentowa-goraczka-gra.png
