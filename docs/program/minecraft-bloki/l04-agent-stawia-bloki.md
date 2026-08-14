# Agent stawia bloki — pierwsza budowla kodem
Subject: Minecraft Education
Level: Poziom 1 — 8-10 lat, bloki
Czas: 95 min
Tags: Agent, Ekwipunek Agenta, Stawianie bloków, Ścieżka
Opis: Agent dostaje do ręki materiał i zaczyna budować. Dzieci uczą się ładować ekwipunek Agenta, stawiać bloki w wybranym kierunku i budują ścieżkę oraz mostek.
Cel: Dziecko ładuje bloki do ekwipunku Agenta, każe mu stawiać je w wybranym kierunku i buduje pierwszą prawdziwą budowlę kodem.

### Po zajęciach dziecko potrafi
- ustawić blok w wybranym slocie ekwipunku Agenta
- kazać Agentowi postawić blok pod sobą, przed sobą i nad sobą
- zbudować ścieżkę, po której da się przejść
- powiedzieć, dlaczego Agent nie stawia bloków, i sprawdzić trzy najczęstsze przyczyny

### Przygotuj przed zajęciami
- gotowe demo: Agent budujący ścieżkę przez przepaść i wracający
- światy dzieci z lekcji 3
- przygotowana lista nazw bloków do wpisania na czat: kamień, deski, szkło, wełna
- zapasowy świat płaski, gdyby czyjś świat był zabudowany

### Zadanie domowe
- zaprogramuj Agentowi ścieżkę z co najmniej dziesięciu bloków w kolorze, którego dziś nie używaliśmy

## [intro] Powitanie i demo budowania (8 min)

### Co robić teraz
- [mów] Cześć! Tydzień temu Agent nauczył się chodzić. Dziś dostanie do ręki bloki.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** wykop przed sobą przepaść, a potem uruchom program. Agent przechodzi przez przepaść, stawiając pod sobą most.
- [mów] Zauważcie, co robi: idzie i stawia, idzie i stawia. Dwa polecenia w kółko.
- Odczytaj kilka pomysłów z zadania domowego, bez podawania imion.
- [mów] Dziś zbudujecie ścieżkę, po której będzie się dało naprawdę przejść. Ale najpierw musimy dać Agentowi materiał — bo pusty robot nic nie postawi.

### Wskazówki
- [tempo] Most nad przepaścią wygląda spektakularnie i jest technicznie prosty. To dobre demo na tę lekcję.
- [podpowiedź] Zdanie „pusty robot nic nie postawi” jest zapowiedzią najczęstszego błędu dnia. Powiedz je teraz, wróci za dwadzieścia minut.

## [review] Wracamy do Agenta (5 min)

### Co robić teraz
- Wszyscy otwierają świat z lekcji 3 i Code Builder klawiszem C. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie program z zeszłego tygodnia — kwadrat albo swoją trasę.
- Szybka powtórka: jak uruchamia się program? Czyj przód ma na myśli Agent? Od czego zaczyna się każdy program?
- Odpowiedź na ostatnie: od przeniesienia Agenta do gracza.
- Wszyscy odlatują na puste miejsce i lądują.

### Wskazówki
- [błąd] Program z zeszłego tygodnia zniknął. Projekty MakeCode zapisują się w świecie — jeśli dziecko założyło nowy świat, kodu tam nie ma. Wtedy odbudowuje trasę z trzech bloczków, to dwie minuty.
- [podpowiedź] Powiedz wprost, że kod mieszka w świecie. To niespodzianka dla wszystkich i lepiej ją wyjaśnić teraz niż przy panice.
- [gdy nie zdążysz] Sam start świata i Code Buildera.

## [concept] Ekwipunek Agenta (14 min)

### Co robić teraz
- [mów] Agent ma własny ekwipunek. Dwadzieścia siedem slotów, czyli szufladek na materiał. Zanim coś postawi, musi mieć to w szufladce.
- Krok 1: z kategorii **Agent** weźcie bloczek „agent set slot”, czyli „ustaw slot Agenta”. Wstawcie go do programu, zaraz po przeniesieniu Agenta.
- Krok 2: w bloczku są trzy rzeczy do ustawienia: numer slotu, rodzaj bloku i ilość. Ustawcie slot 1, blok kamień, ilość 64.
- [mów] Sześćdziesiąt cztery to pełny stos. W trybie kreatywnym możecie wpisać dowolną liczbę.
- Krok 3: dołóżcie bloczek „agent set active slot 1”, czyli „Agent używa slotu 1” — to mówi, z której szufladki ma brać.
- Krok 4: wróćcie do gry i uruchomcie komendę. Nic widocznego się nie stało.
- [mów] I dobrze. Załadowaliście Agentowi plecak. Teraz sprawdźmy, czy naprawdę tam coś jest.
- Krok 5: pokaż, jak zajrzeć do ekwipunku Agenta — stańcie obok i kliknijcie na niego prawym przyciskiem.
- Krok 6: sprawdźcie, czy w pierwszym slocie jest kamień. Wpiszcie na czat literę K, jeśli tak.
- [mów] Ta czynność uratuje wam mnóstwo czasu. Kiedy Agent nie stawia bloków, **pierwsze**, co robicie, to zaglądacie mu do plecaka.

### Wskazówki
- [podpowiedź] Nauczenie dzieci zaglądania do ekwipunku Agenta jest tu ważniejsze niż sam bloczek. To ich pierwsze narzędzie diagnostyczne w całym kursie.
- [błąd] Nazwa bloku w bloczku wybierana jest z listy, a nie wpisywana. Jeśli dziecko czegoś nie znajduje, lista ma pole wyszukiwania.
- [błąd] Dziecko ustawiło slot 1, ale aktywny slot ma inny numer. Agent bierze wtedy z pustej szufladki. To najczęstsza przyczyna „nic nie działa” dziś.
- [błąd] Ładowanie ekwipunku wstawione **po** stawianiu bloków. Kolejność ma znaczenie: najpierw plecak, potem budowa.
- [dla szybszych] Niech załadują trzy różne bloki do trzech slotów i sprawdzą, że przełączanie aktywnego slotu zmienia materiał.
- [gdy nie zdążysz] Jeden slot z jednym materiałem wystarczy na całą lekcję.

### Materiały
- [kod] Ładowanie ekwipunku Agenta | makecode:
  ```
  on chat command "start"
    agent teleport to player
    agent set slot 1 item STONE amount 64
    agent set active slot 1
  ```

## [guided] Pierwsza ścieżka (18 min)

### Co robić teraz
- [mów] Teraz budujemy. Agent ma materiał, więc może zacząć stawiać.
- Krok 1: z kategorii Agent weźcie bloczek „agent place on”, czyli „Agent postaw blok”. Ma listę kierunków: forward, back, left, right, up, down.
- [mów] Sześć kierunków. Ale uwaga — to znowu kierunki **Agenta**, nie wasze. Tak samo jak w zeszłym tygodniu.
- Krok 2: dołóżcie do programu: postaw blok **down**, czyli pod sobą. Uruchomcie.
- Krok 3: podejdźcie zobaczyć. Pod Agentem leży kamień. **Czekaj na kciuki.**
- [mów] Zauważcie: Agent stanął na tym bloku. Postawił go pod sobą i sam się na nim znalazł.
- Krok 4: teraz ścieżka. Dołóżcie: idź w przód o 1, postaw blok down. I jeszcze raz. I jeszcze raz.
- Krok 5: uruchomcie. Za Agentem ciągnie się ścieżka z czterech bloków.
- [mów] Policzcie bloczki w waszym programie. Osiem, żeby postawić cztery bloki. Ścieżka na sto bloków to byłoby dwieście bloczków.
- [mów] Zapamiętajcie to. Za tydzień poznacie bloczek, który to skróci do trzech.
- Krok 6: przejdźcie się po swojej ścieżce. Działa jak prawdziwa.
- Krok 7: zmieńcie materiał na inny — wełnę w wybranym kolorze albo szkło — i uruchomcie ponownie.

### Wskazówki
- [podpowiedź] Kolejność „idź, postaw” zamiast „postaw, idź” daje ładniejszą ścieżkę bez dziury na starcie. Warto pokazać obie i porównać.
- [błąd] Agent nie stawia nic. Trzy przyczyny w kolejności sprawdzania: pusty ekwipunek, zły aktywny slot, bloczek stawiania poza programem.
- [błąd] Ścieżka ma dziurę na początku albo na końcu — kwestia kolejności ruchu i stawiania.
- [błąd] Agent stawia bloki w powietrzu, bo dziecko wybrało kierunek forward zamiast down. To dobra okazja, żeby pokazać wszystkie sześć kierunków.
- [błąd] Ścieżka idzie w innym miejscu niż dziecko chciało, bo obróciło się przed uruchomieniem. Wraca temat ruchu względnego.
- [błąd] Agent nie chce postawić bloku, bo w tym miejscu już coś stoi. Agent nie nadpisuje bloków. Trzeba budować na pustym terenie albo kazać mu najpierw zniszczyć.
- [dla szybszych] Niech zbudują ścieżkę, która skręca — z bloczkiem skręcania w środku.
- [gdy nie zdążysz] Ścieżka z dwóch bloków wystarczy do zrozumienia zasady.

### Materiały
- [kod] Ścieżka z czterech bloków | makecode:
  ```
  on chat command "sciezka"
    agent teleport to player
    agent set slot 1 item STONE amount 64
    agent set active slot 1
    agent move forward 1
    agent place down
    agent move forward 1
    agent place down
    agent move forward 1
    agent place down
    agent move forward 1
    agent place down
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów, piją wodę.
- Poproś, żeby nie zamykały gry.
- Zapowiedź: „Po przerwie Agent zbuduje most nad przepaścią i ścianę. A wy zbudujecie mu przepaść.”

### Wskazówki
- [tempo] Zapowiedź, że dzieci będą kopać przepaść, jest zaskakująco skuteczna. Kopanie dziur jest ulubioną czynnością tej grupy wiekowej.

## [concept] Sześć kierunków i most (13 min)

### Co robić teraz
- [mów] Agent umie stawiać w sześciu kierunkach. Zrobimy z tego użytek.
- Krok 1: każde dziecko kopie przed sobą przepaść — dziurę na trzy bloki szerokości i dwa w głąb. Ręcznie, lewym przyciskiem. Dwie minuty.
- Krok 2: stańcie na krawędzi, twarzą do przepaści.
- Krok 3: zmodyfikujcie program: idź w przód o 1, postaw blok down — powtórzone tyle razy, ile ma szerokości przepaść.
- Krok 4: uruchomcie. Agent przechodzi nad przepaścią, budując pod sobą most.
- [mów] Zauważcie coś ważnego: Agent **nie spada**. On lewituje. Może iść tam, gdzie wy byście spadli.
- Krok 5: przejdźcie po moście. Działa.
- Krok 6: teraz kierunek **forward**. Zmieńcie jeden bloczek na „postaw blok forward” i uruchomcie na płaskim terenie.
- [mów] Blok pojawia się przed Agentem, na wysokości jego oczu. Tak się buduje ściany.
- Krok 7: kierunek **up**. Blok nad Agentem. Tak się buduje sufity.
- Krok 8: ćwiczenie na przewidywanie. Podaję kierunek, wy piszecie na czat, do czego by się przydał: down, forward, up.

### Wskazówki
- [podpowiedź] Informacja, że Agent nie spada, otwiera całą klasę budowli — mosty, wieże, latające platformy. Powiedz to wprost.
- [błąd] Agent spada do przepaści. To znaczy, że dziecko użyło bloczka ruchu innego niż zwykłe „move forward” albo Agent stanął w dziurze przed uruchomieniem.
- [błąd] Most ma dziurę, bo przepaść jest szersza niż liczba kroków w programie. Policzcie razem.
- [błąd] Agent buduje ścianę zamiast mostu, bo dziecko zostawiło kierunek forward. Dobra okazja do porównania.
- [błąd] Dziecko wykopało przepaść tak głęboką, że nie widzi dna. To nie przeszkadza — Agent i tak lewituje.
- [dla szybszych] Niech zbudują most z barierkami: blok down i blok forward w jednym przebiegu.
- [gdy nie zdążysz] Sam most, bez omawiania kierunków up i forward — wracają na lekcji 7.

### Materiały
- [kod] Most nad przepaścią | makecode:
  ```
  on chat command "most"
    agent teleport to player
    agent set slot 1 item PLANKS amount 64
    agent set active slot 1
    agent move forward 1
    agent place down
    agent move forward 1
    agent place down
    agent move forward 1
    agent place down
    agent move forward 1
    agent place down
    agent move forward 1
    agent place down
  ```

## [guided] Kolorowa ścieżka i pierwszy porządek w kodzie (14 min)

### Co robić teraz
- [mów] Zbudujemy ścieżkę z dwóch kolorów, na przemian. I przy okazji zrobimy porządek w programie.
- Krok 1: załadujcie Agentowi dwa materiały: slot 1 wełna czerwona, slot 2 wełna biała.
- Krok 2: zbudujcie ścieżkę, w której przed każdym postawieniem bloku zmieniacie aktywny slot: raz 1, raz 2.
- Krok 3: uruchomcie. Ścieżka jest w paski. **Czekaj na kciuki.**
- [mów] Popatrzcie teraz na swój program. Ile ma bloczków? Dwadzieścia? Trzydzieści?
- Krok 4: policzcie razem, ile bloczków przypada na jeden blok ścieżki. Trzy: zmień slot, idź, postaw.
- [mów] Ścieżka na sto bloków to trzysta bloczków. Nie da się tego przewinąć na ekranie. To jest granica tego, co ma sens robić w ten sposób.
- Krok 5: pokaż, jak uporządkować program: przeciągnijcie go tak, żeby był czytelny, i sprawdźcie, czy da się go w ogóle objąć wzrokiem.
- Krok 6: nadajcie komendzie sensowną nazwę zamiast „start” — na przykład „pasy”.
- [mów] Nazwa komendy to jak nazwa pliku. Za miesiąc będziecie mieli dziesięć programów w jednym świecie i „start” nic wam nie powie.

### Wskazówki
- [podpowiedź] Doprowadzenie do programu, którego nie da się objąć wzrokiem, jest celem tego kroku. To najlepsza możliwa motywacja do pętli na lekcji 5.
- [błąd] Ścieżka jest jednokolorowa, bo dziecko zapomniało przełączyć aktywny slot przed drugim blokiem.
- [błąd] Agent stawia powietrze, bo w drugim slocie nie ma materiału — ładowanie zostało tylko dla slotu 1.
- [błąd] Program stał się tak długi, że dziecko gubi się przy przeciąganiu bloczków. To jest właśnie problem, o którym mowa.
- [dla szybszych] Niech zrobią ścieżkę z trzech kolorów w powtarzającym się wzorze i policzą, ile bloczków to kosztuje.
- [gdy nie zdążysz] Ścieżka dwukolorowa z sześciu bloków. Liczenie i wniosek zrób i tak.

## [challenge] Twoja budowla kodem (8 min)

### Co robić teraz
- Zadanie samodzielne: zaprogramuj Agentowi coś własnego. Wybierz jedno.
- Ścieżka od twojego domku do wieży, w wybranym kolorze.
- Most nad wykopaną przez ciebie przepaścią.
- Kwadratowa ramka na ziemi — ścieżka, która wraca do punktu startu.
- Schody: idź w przód, postaw blok forward, wejdź wyżej.
- Kto skończy, wpisuje na czat, co zbudował i z ilu bloczków.

### Wskazówki
- [tempo] Zbieranie liczby bloczków od wszystkich dzieci daje twardy materiał do porównania na lekcji 5. Zapisz sobie te liczby.
- [dla szybszych] Schody są najtrudniejsze z listy i wymagają połączenia ruchu z dwoma kierunkami stawiania. Skieruj tam dzieci, którym idzie sprawnie.
- [gdy nie zdążysz] Ścieżka z pięciu bloków wystarczy.
- [błąd] Dziecko chce zbudować coś dużego i po dziesięciu bloczkach się zniechęca. To jest właściwa reakcja i warto ją nazwać: „masz rację, że to za dużo pracy, i za tydzień to rozwiążemy”.

## [challenge] Pokaz i wyjście ze świata (5 min)

### Co robić teraz
- Przypomnij o wyjściu przez menu, nie przez zamknięcie okna.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko udostępnia ekran, uruchamia komendę i pokazuje, jak Agent buduje.
- Poproś, żeby powiedziało, ile bloczków ma jego program.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Oglądanie budującego Agenta jest samo w sobie satysfakcjonujące. Nie skracaj pokazu bardziej, niż musisz.
- [błąd] Program nie działa na pokazie, bo dziecko stoi w innym miejscu niż przy testowaniu — zwykle na już zabudowanym terenie. Agent nie nadpisuje bloków.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: co trzeba zrobić, zanim Agent postawi blok? Jakie znacie trzy przyczyny tego, że Agent nic nie stawia? Ile bloczków kosztuje jeden blok ścieżki?
- [mów] Dzisiaj Agent naprawdę zaczął budować. Ale zauważyliście problem: żeby postawić dziesięć bloków, potrzeba dwudziestu bloczków.
- [mów] To nie ma sensu. Programista, który pisze dwadzieścia poleceń, żeby dziesięć razy zrobić to samo, robi coś źle.
- Zajawka: „Na następnych zajęciach poznacie bloczek, który to wszystko zmieni. Trzema bloczkami zbudujecie mur o długości stu bloków.”
- Przypomnij zadanie domowe: ścieżka z dziesięciu bloków w nowym kolorze.

### Wskazówki
- [błąd] Agent nie stawia bloków — sprawdź po kolei: ekwipunek, aktywny slot, miejsce bloczka w programie.
- [błąd] Agent nie nadpisuje istniejących bloków — buduj na pustym terenie.
- [błąd] Ładowanie ekwipunku po stawianiu — kolejność ma znaczenie.
- [błąd] Bloki lądują w powietrzu — wybrany kierunek forward zamiast down.
- [błąd] Kod zniknął — projekty MakeCode mieszkają w konkretnym świecie.
