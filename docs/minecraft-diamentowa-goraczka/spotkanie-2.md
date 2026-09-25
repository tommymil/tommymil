# Diamentowa Gorączka 2: Zamieniamy świat w grę
Subject: Minecraft Education
Level: Poziom 2 - 10-12 lat
Tags: MakeCode, Zmienne, Zdarzenia, Pętla dopóki, Warunki, Turniej
Opis: Drugie z dwóch spotkań. Dzieci dodają punkty, pułapki i supermoce, odliczanie czasu, koniec gry z fajerwerkami i rekord, a na koniec rozgrywają turniej.

## [review] Powrót do projektu i punkt kontrolny (7 min)

### Co robić teraz
- Sprawdź obecność i czy wszyscy słyszą i widzą Twój ekran.
- Dzieci otwierają świat „Diamentowa Gorączka”, naciskają C i otwierają projekt z zeszłego tygodnia.
- Test: ▶, `start`, `buduj`. Kto ma pioruny i kopalnię, pisze na czacie „działa”.
- Kto nie ma projektu (inny komputer, inne konto) albo coś nie działa: wyślij mu na czacie `diamentowa-goraczka-punkt-kontrolny.mkcd`, a dziecko otwiera go przez **MakeCode → Importuj → Importuj plik**.
- Przypomnienie: zdarzenie, funkcja, pętla, losowanie. „Dziś zamienimy świat w prawdziwą grę”.

### Wskazówki
- [tempo] Nie trać czasu na odtwarzanie starego kodu ręcznie. Punkt kontrolny zajmuje 1 minutę.
- [podpowiedź] Punkt kontrolny robisz przed zajęciami z pliku `diamentowa-goraczka-PUNKT-KONTROLNY.txt`, tak samo jak projekt startowy: wklejasz, przełączasz na Bloki, zapisujesz jako `.mkcd`.
- [błąd] Projektu nie ma na liście: dziecko jest na innym komputerze albo zalogowane na inne konto.

### Materiały
- [obraz] Tak działa gra | infografiki/01-diamentowa-goraczka-gra.png
- [kod] Punkt kontrolny (JavaScript; dla nauczyciela, ten sam kod co w diamentowa-goraczka-PUNKT-KONTROLNY.txt):
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
  ```

## [concept] Zmienna punkty i tablica wyników (6 min)

### Co robić teraz
- „Zmienna to pudełko z naklejką. Na naklejce jest nazwa PUNKTY, a w środku karteczka z liczbą. Możemy zajrzeć do pudełka albo podmienić karteczkę”.
- Jeśli masz pod ręką pudełko i karteczkę, pokaż je na kamerze. Jeśli nie, pokaż infografikę z pudełkiem.
- Zmienna `punkty` już istnieje (jest w silniku). Pokaż ją w kategorii **Zmienne**.
- W bloczku **podczas uruchamiania** dodaj na końcu **wywołanie przygotujTablice**. Po kliknięciu ▶ z prawej strony ekranu pojawi się tablica wyników.

### Wskazówki
- [podpowiedź] „Podczas uruchamiania” uruchamia się raz, zaraz po kliknięciu ▶.
- [błąd] Brak tablicy wyników: bloczek wywołania nie jest w „podczas uruchamiania” albo nie kliknięto ponownie ▶.

### Materiały
- [obraz] Zmienna punkty i tablica wyników | infografiki/09-zmienna-punkty-tablica.png
- [kod] MakeCode, bloczki:
  ```
  podczas uruchamiania
    ustaw punkty na 0
    ... reszta zmiennych silnika
    wywołanie przygotujTablice
  ```
- [kod] JavaScript (dopisz na dole kodu, poza funkcjami):
  ```
  przygotujTablice()
  ```

## [guided] Co daje każdy skarb: zdarzenia (17 min)

### Co robić teraz
- Bloczek **Bloki → gdy [blok] złamano** nie potrzebuje komendy na czacie. Czeka, aż gracz zniszczy wybrany blok.
- Pierwsze zdarzenie robimy razem: ruda diamentu → zmień punkty o 1 → wywołanie pokazPunkty → dźwięk random.orb.
- Kolejne dzieci duplikują i zmieniają same: złoto +3, szmaragd → wywołanie supermoc, TNT → zmień punkty o −2, pokazPunkty, wywołanie wybuch.
- Dzieci projektują zasady: mogą zmienić liczby punktów. Zapytaj: „Czy gra jest wtedy uczciwa? Czy da się w nią fajnie grać?”
- Swoje zasady piszą na czacie, np. „złoto 5, TNT −3”.

### Wskazówki
- [błąd] W zdarzeniu wybrano „diament” albo „blok diamentu” zamiast **rudy diamentu**. Wtedy punkty nie rosną.
- [podpowiedź] Szybki test w trybie kreatywnym: `buduj`, `wejdz`, wykop coś widocznego przez szybę i patrz na tablicę wyników.
- [tempo] BUM i supermoc to efekty wow tej części. Pozwól dzieciom je wypróbować, zanim przejdziecie dalej.

### Materiały
- [obraz] Co daje każdy skarb | infografiki/10-co-daje-skarb.png
- [obraz] Zdarzenia: wykopano skarb | infografiki/11-zdarzenia-skarbow.png
- [kod] MakeCode, bloczki:
  ```
  gdy [ruda diamentu] złamano
    zmień punkty o 1
    wywołanie pokazPunkty
    wykonaj [playsound random.orb @p]

  gdy [ruda złota] złamano
    zmień punkty o 3
    wywołanie pokazPunkty
    wykonaj [playsound random.levelup @p]

  gdy [ruda szmaragdu] złamano
    wywołanie supermoc

  gdy [TNT] złamano
    zmień punkty o -2
    wywołanie pokazPunkty
    wywołanie wybuch
  ```
- [kod] JavaScript:
  ```
  blocks.onBlockBroken(DIAMOND_ORE, function () {
      punkty += 1
      pokazPunkty()
      player.execute("playsound random.orb @p")
  })
  blocks.onBlockBroken(GOLD_ORE, function () {
      punkty += 3
      pokazPunkty()
      player.execute("playsound random.levelup @p")
  })
  blocks.onBlockBroken(EMERALD_ORE, function () {
      supermoc()
  })
  blocks.onBlockBroken(TNT, function () {
      punkty += -2
      pokazPunkty()
      wybuch()
  })
  ```

## [guided] Pierwsza rozgrywka: graj i stop (12 min)

### Co robić teraz
- Komenda `graj`: wyzeruj punkty, wyjdź na platformę, zbuduj kopalnię, schowaj skarby, wyczyść ekwipunek, wejdź na górę, daj kilof, włącz tryb przetrwania.
- Pytanie do grupy: „Dlaczego najpierw wychodzimy, a dopiero potem budujemy?” (Inaczej gracz zostałby zamurowany w kamieniu.)
- Tryb przetrwania jest potrzebny, bo w kreatywnym bloki znikają od jednego kliknięcia i nie trzeba szukać.
- Komenda `stop`: tryb kreatywny i powrót na platformę.
- Test: `graj` → kopiemy → `stop`. Tablica wyników musi się zmieniać, szmaragd dawać supermoc, a TNT robić BUM.

### Wskazówki
- [błąd] Gracz nie może kopać: nie dostał kilofa albo jest w trybie przygody. Sprawdź bloczki „daj” i „zmień tryb gry”.
- [podpowiedź] Diamentowy kilof kopie kamień szybko. Z innym kilofem gra robi się dużo trudniejsza.

### Materiały
- [obraz] Pierwsza rozgrywka: graj | infografiki/12-graj-pierwsza-rozgrywka.png
- [kod] MakeCode, bloczki:
  ```
  przy poleceniu czatu [graj]
    ustaw punkty na 0
    wywołanie pokazPunkty
    teleportuj do (świat 0 -60 13)
    wywołanie zbudujKopalnie
    wywołanie ukryjSkarby
    wykonaj [clear @p]
    teleportuj do (świat 0 -56 0)
    daj [@p] [diamentowy kilof] ilość 1
    zmień tryb gry na [przetrwanie] dla [@p]

  przy poleceniu czatu [stop]
    zmień tryb gry na [kreatywny] dla [@p]
    teleportuj do (świat 0 -60 13)
  ```
- [kod] JavaScript:
  ```
  player.onChat("graj", function () {
      punkty = 0
      pokazPunkty()
      player.teleport(world(0, -60, 13))
      zbudujKopalnie()
      ukryjSkarby()
      player.execute("clear @p")
      player.teleport(world(0, -56, 0))
      mobs.give(mobs.target(LOCAL_PLAYER), DIAMOND_PICKAXE, 1)
      gameplay.setGameMode(SURVIVAL, mobs.target(LOCAL_PLAYER))
  })
  player.onChat("stop", function () {
      gameplay.setGameMode(CREATIVE, mobs.target(LOCAL_PLAYER))
      player.teleport(world(0, -60, 13))
  })
  ```

## [challenge] Kto ma najwięcej? (3 min)

### Co robić teraz
- Szybka runda: każdy gra jeszcze raz i podnosi rękę w rozmowie, gdy przekroczy 5 punktów.
- Zapowiedź po przerwie: „Czegoś jeszcze brakuje tej grze… Co to jest?”

### Wskazówki
- [tempo] Na razie gra nie ma końca. Dzieci same to zauważą i dobrze, bo to wstęp do drugiej części.

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy: dzieci wstają od ekranów, piją wodę, rozprostowują się.

### Wskazówki
- [tempo] Przed przerwą niech każdy wpisze `stop`, żeby nie zostać w trybie przetrwania.

## [concept] Czego brakuje grze? (3 min)

### Co robić teraz
- Zapytaj: „Czego brakuje naszej grze?” Dzieci same mówią: czasu i końca gry.
- „Za chwilę gra będzie trwała dokładnie 60 sekund, zakończy się sama i pokaże wynik z fajerwerkami”.

## [guided] Czas i koniec gry: pętla dopóki (17 min)

### Co robić teraz
- „Pętla DOPÓKI kręci się tak długo, jak długo warunek jest prawdziwy. Dopóki czas jest większy od zera: pokaż czas, poczekaj sekundę, odejmij jeden”.
- Rozbudowujemy komendę `graj`. Całość owijamy w **jeśli gra … w przeciwnym razie** (Logika), żeby nie dało się uruchomić dwóch gier naraz.
- Nowe bloczki w przygotowaniu: ustaw czas na 60, kill @e[type=item] (sprzątanie), wywołanie odliczanie, ustaw gra na prawda.
- Pętla: **dopóki czas > 0** → wywołanie pokazCzas → czekaj 1000 ms → zmień czas o −1.
- Koniec gry: gra na fałsz, tryb kreatywny, teleport na platformę, tytuł KONIEC! z punktami (bloczek „połącz”), wywołanie fajerwerki.
- Do testów ustaw czas na 15, żeby nie czekać minuty.

### Wskazówki
- [podpowiedź] Zmienne `czas` i `gra` są już w silniku. `gra` to zmienna prawda/fałsz: czy gra teraz trwa?
- [podpowiedź] Bloczek „połącz” (join) jest w Zaawansowane → Tekst.
- [błąd] Czas się nie zmienia: brakuje „zmień czas o −1” w pętli albo wpisano +1.
- [błąd] Gra kończy się od razu: „ustaw czas na 60” jest po pętli zamiast przed nią.
- [tempo] To najdłuższy fragment kodu. Pokazuj po kawałku i sprawdzaj po każdym kawałku.

### Materiały
- [obraz] Graj: przygotowanie rundy | infografiki/13-graj-przygotowanie-rundy.png
- [obraz] Pętla czasu i koniec gry | infografiki/14-petla-czasu-i-koniec-gry.png
- [kod] MakeCode, bloczki:
  ```
  przy poleceniu czatu [graj]
    jeśli (gra) to
      powiedz [Gra już trwa!]
    w przeciwnym razie
      ustaw punkty na 0
      ustaw czas na 60
      wywołanie pokazPunkty
      teleportuj do (świat 0 -60 13)
      wywołanie zbudujKopalnie
      wywołanie ukryjSkarby
      wykonaj [kill @e[type=item]]
      wykonaj [clear @p]
      teleportuj do (świat 0 -56 0)
      wywołanie odliczanie
      daj [@p] [diamentowy kilof] ilość 1
      zmień tryb gry na [przetrwanie] dla [@p]
      ustaw gra na (prawda)
      dopóki (czas > 0)
        wywołanie pokazCzas
        pauza (ms) 1000
        zmień czas o -1
      ustaw gra na (fałsz)
      zmień tryb gry na [kreatywny] dla [@p]
      teleportuj do (świat 0 -60 13)
      pokaż tytuł [KONIEC!] podtytuł (połącz [Punkty: ] (punkty))
      wywołanie fajerwerki
  ```
- [kod] JavaScript:
  ```
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
      }
  })
  ```

## [guided] Rekord i uczciwe punkty (8 min)

### Co robić teraz
- Pod „wywołanie fajerwerki” dodaj: **jeśli punkty > rekord** → ustaw rekord na punkty → wywołanie zapiszRekord → tytuł NOWY REKORD! → dźwięk.
- Pytanie: „Co się stanie, jeśli ktoś wykopie diament po końcu gry?” (Dostanie punkty.)
- Dlatego w zdarzeniach diamentu, złota i TNT otaczamy wszystko bloczkiem **jeśli gra**.
- Komenda `koniec` (ustaw czas na 0) pozwala przerwać grę. Fajerwerki i wynik i tak się pokażą.

### Wskazówki
- [błąd] Kliknięcie ▶ zeruje wszystkie zmienne, także rekord. Przed turniejem mówimy: „Kod gotowy, już nic nie klikamy”.
- [podpowiedź] Szmaragdu nie trzeba owijać w warunek, bo nie daje punktów.

### Materiały
- [obraz] Rekord i uczciwe punkty | infografiki/15-rekord-i-warunki.png
- [kod] MakeCode, bloczki:
  ```
  jeśli (punkty > rekord) to
    ustaw rekord na (punkty)
    wywołanie zapiszRekord
    pokaż tytuł [§6NOWY REKORD!] podtytuł (połącz [] (rekord))
    wykonaj [playsound ui.toast.challenge_complete @p]

  gdy [ruda diamentu] złamano
    jeśli (gra) to
      zmień punkty o 1
      wywołanie pokazPunkty
      wykonaj [playsound random.orb @p]
  ... tak samo dla rudy złota i TNT

  przy poleceniu czatu [koniec]
    ustaw czas na 0
  ```
- [kod] JavaScript:
  ```
  // w komendzie "graj", zaraz po fajerwerki():
  if (punkty > rekord) {
      rekord = punkty
      zapiszRekord()
      gameplay.title(mobs.target(LOCAL_PLAYER), "§6NOWY REKORD!", "" + rekord)
      player.execute("playsound ui.toast.challenge_complete @p")
  }

  // zdarzenia z warunkiem:
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
  blocks.onBlockBroken(TNT, function () {
      if (gra) {
          punkty += -2
          pokazPunkty()
          wybuch()
      }
  })
  player.onChat("koniec", function () {
      czas = 0
  })
  ```

## [challenge] Turniej (12 min)

### Co robić teraz
- **Runda 1:** każdy ustawia czas z powrotem na 60, klika ▶ ostatni raz i gra w swoją grę. Wynik pisze na czacie.
- **Runda 2:** druga gra, próba pobicia własnego rekordu. Nowy wynik znowu na czacie.
- **Finał na żywo:** 2–3 osoby z najlepszym wynikiem udostępniają ekran i grają jeszcze raz, a reszta kibicuje na czacie.
- Pokaż na swoim ekranie 3 najlepsze wyniki (np. w notatce albo tablicy w rozmowie).
- Nagrody: Mistrz kopalni (najlepszy wynik), Łowca rekordów (największy skok między rundami), Podstępny projektant (najwięcej pułapek), Reżyser (najładniejsze intro z pierwszego spotkania).

### Wskazówki
- [tempo] Po każdym ogłoszeniu brawa: włączone mikrofony albo emotki w rozmowie. To nagroda za dwa spotkania pracy.
- [błąd] Dziecko udostępnia ekran, ale gra się tnie: niech wyłączy kamerę na czas pokazu.
- [błąd] Gracz zginął w trakcie gry (głód, upadek): odrodzi się na platformie, gra liczy dalej. Wraca komendą `wejdz`.
- [podpowiedź] Różne zasady punktacji u różnych dzieci to zaleta: porównajcie, czyja gra była najtrudniejsza.

### Materiały
- [obraz] Turniej | infografiki/16-turniej.png

## [summary] Czego się nauczyliśmy (5 min)

### Co robić teraz
- Jakie pojęcia poznaliśmy? Zdarzenie, współrzędne, funkcja, pętla, losowanie, zmienna, warunek.
- Niech każdy napisze na czacie jedno pojęcie i gdzie było w grze (np. „pętla: warstwy kamienia”).
- Co było najtrudniejsze, a co najfajniejsze? Co byście dodali do gry?
- Pomysły na kolejne zajęcia: poziomy trudności, bonus czasu za lapis, potwory przy pułapce, dokładnie 10 diamentów.

### Wskazówki
- [tempo] Zakończ pochwałą: „Każdy z was zaprogramował własną grę od zera”.
- [podpowiedź] Po zajęciach wyślij rodzicom projekt pełnej gry `diamentowa-goraczka-pelna-gra.mkcd` („Lekcja końcowa” w aplikacji, link z prezentera). Dziecko może grać i rozwijać ją w domu.

### Materiały
- [obraz] Czego się nauczyliśmy | infografiki/17-czego-sie-nauczylismy.png
- [kod] Pełna gra (JavaScript; dla nauczyciela, ten sam kod co w diamentowa-goraczka-PELNA-GRA.txt):
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
