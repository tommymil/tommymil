# Ten sam kod, dwa widoki — bloki i tekst
Subject: Minecraft Education
Level: Poziom 2 — 10-12 lat, kod
Czas: 95 min
Tags: Pierwsze zajęcia, MakeCode, JavaScript, Przełącznik widoku
Opis: Pierwsze zajęcia kursu tekstowego. Dzieci odkrywają, że bloczki i tekst to ten sam program, i piszą pierwsze linijki własnoręcznie.
Cel: Dziecko przełącza widok między blokami a tekstem, rozumie, że to zapisy tego samego programu, i samodzielnie pisze pierwsze polecenia w JavaScripcie.

### Po zajęciach dziecko potrafi
- przełączyć MakeCode między widokiem bloków a widokiem JavaScript
- odczytać program blokowy w wersji tekstowej i wskazać, co czemu odpowiada
- napisać własnoręcznie polecenie ruchu i stawiania bloku
- użyć przełącznika widoku, żeby sprawdzić poprawną nazwę polecenia

### Przygotuj przed zajęciami
- sprawdzone licencje Minecraft Education dla całej grupy
- **sprawdzone w Twojej wersji edytora**, jak dokładnie wyglądają nazwy poleceń po przełączeniu na tekst — zapisz je sobie
- gotowe demo: ten sam program w dwóch widokach, obok siebie
- lista uczestników i informacja, kto przeszedł kurs bloków, a kto Scratcha

### Zadanie domowe
- przepisz ręcznie z bloczków na tekst dowolny program i sprawdź przełącznikiem, czy się zgadza

## [intro] Powitanie, poznajmy się, demo (10 min)

### Co robić teraz
- [mów] Cześć. Witam na kursie, na którym przestajecie klikać bloczki i zaczynacie pisać kod. Prawdziwy, taki sam jak w pracy.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- Rundka: imię, czy było się już na kursie bloków albo Scratcha, czy pisało się kiedyś kod tekstowo. Zanotuj — ten rozrzut jest tu największy z całego programu.
- **DEMO:** pokaż program blokowy budujący mur. Uruchom go.
- Potem kliknij przełącznik na górze ekranu i pokaż ten sam program jako tekst.
- [mów] To nie są dwa programy. To jest **jeden** program pokazany na dwa sposoby. Tak jak liczba pięć i słowo „pięć”.
- Przełącz kilka razy tam i z powrotem, powoli.
- [mów] I najlepsza wiadomość: ten przełącznik działa cały czas. Jeśli nie wiecie, jak coś napisać — ułóżcie z bloczków i przełączcie na tekst.

### Wskazówki
- [podpowiedź] Przełącznik widoku to najważniejsze narzędzie całego kursu. Powiedz to w pierwszych dziesięciu minutach i wracaj do tego przy każdym problemie.
- [tempo] Rozrzut doświadczenia w tej grupie bywa ogromny. Zanotuj, kto pisał już kod — ci uczniowie będą potrzebowali zadań dodatkowych od pierwszej lekcji.

## [concept] Logowanie i pierwsze przełączenie (12 min)

### Co robić teraz
- Wszyscy uruchamiają Minecraft Education i logują się. Podyktuj adresy kont po imieniu, indywidualnie. Haseł nie podajesz i o nie nie prosisz.
- Po zalogowaniu każde dziecko wpisuje na czat literę Z.
- Krok 1: nowy świat. Kreatywny, płaski, w nazwie imię i numer lekcji.
- Krok 2: klawisz C otwiera Code Builder. Wybierzcie MakeCode i nowy projekt.
- Krok 3: **odszukajcie przełącznik widoku** na górze ekranu. Zwykle są tam napisy Blocks i JavaScript.
- Krok 4: kliknijcie JavaScript. Ekran zmienia się w edytor tekstu.
- Krok 5: kliknijcie z powrotem Blocks. Wracacie do bloczków.
- [mów] Poćwiczcie to przełączanie pięć razy. Będziecie go używać kilkadziesiąt razy na każdej lekcji.
- Krok 6: pytanie do grupy: co się dzieje z programem, gdy przełączacie widok? Odpowiedź: nic. Zmienia się tylko sposób pokazania.

### Wskazówki
- [błąd] Brak licencji — nie do naprawienia na zajęciach. Dziecko pracuje dziś obserwując Twój ekran, a Ty zgłaszasz sprawę administracji tego samego dnia.
- [błąd] Przełącznik jest w innym miejscu niż na Twoim ekranie, bo wersja edytora się różni. Sprawdź to przed zajęciami.
- [błąd] Po przełączeniu na tekst i edycji nie da się wrócić do bloczków. To się zdarza, gdy tekst zawiera coś, czego bloczki nie potrafią pokazać. Dziś nie powinno wystąpić, ale uprzedź, że tak bywa i to nie jest awaria.
- [podpowiedź] Okno Code Builder na małym laptopie bywa za ciasne na edytor tekstu. Pokaż, jak je powiększyć — w tym kursie to ma większe znaczenie niż w blokowym.
- [tempo] Jeśli po dwunastu minutach ktoś nie ma działającego edytora, przydziel opiekuna z grupy i idź dalej.

### Materiały
- [link] Minecraft Education — Code Builder | https://education.minecraft.net/

## [guided] Czytamy kod, który sami ułożyliśmy (15 min)

### Co robić teraz
- [mów] Zbudujemy prosty program z bloczków, a potem przetłumaczymy go razem, linijka po linijce.
- Krok 1: z bloczków ułóżcie: komenda na czacie o nazwie start, w niej przeniesienie Agenta do gracza, ustawienie materiału i ruch w przód o 3.
- Krok 2: uruchomcie w grze i sprawdźcie, że działa.
- Krok 3: przełączcie na widok tekstowy. **Nie zmieniajcie nic.**
- Krok 4: czytamy razem, linijka po linijce. Ty czytasz, dzieci wskazują odpowiadający bloczek.
- Pierwsza linijka to komenda na czacie. Nazwa komendy jest w cudzysłowie.
- Kolejne linijki to polecenia dla Agenta, każde w osobnej linii.
- Klamry otwierają i zamykają zawartość komendy.
- [mów] Zauważcie regułę: **wcięcie w tekście to zagnieżdżenie w bloczkach**. Wszystko, co było w środku bloczka komendy, jest w tekście wcięte i objęte klamrami.
- Krok 5: ćwiczenie na czat. Wskazuję bloczek, wy piszecie, która linijka tekstu mu odpowiada. Pięć rund.
- Krok 6: teraz odwrotnie. Czytam linijkę tekstu, wy wskazujecie bloczek.

### Wskazówki
- [podpowiedź] Ćwiczenie w obie strony jest sensem tej lekcji. Dziecko ma zrozumieć odwzorowanie, a nie zapamiętać składnię.
- [podpowiedź] Powiązanie wcięcia z zagnieżdżeniem jest kluczem do całego kursu i wraca przy pętlach, funkcjach i warunkach.
- [błąd] Nazwy w Twoim edytorze mogą wyglądać nieco inaczej niż w konspekcie — wersje MakeCode różnią się w szczegółach. Podawaj to, co dzieci widzą u siebie, a nie to, co jest zapisane.
- [błąd] Dziecko próbuje edytować tekst i psuje program. Dziś jeszcze nic nie edytujemy — czytamy.
- [dla szybszych] Niech policzą, ile znaków ma program w wersji tekstowej, i porównają z liczbą kliknięć potrzebnych do ułożenia go z bloczków.
- [gdy nie zdążysz] Samo czytanie w jedną stronę, bloczek na linijkę.

### Materiały
- [kod] Pierwszy program — widok tekstowy | javascript:
  ```
  player.onChat("start", function () {
      agent.teleportToPlayer()
      agent.setItem(STONE, 64, 1)
      agent.setSlot(1)
      agent.move(FORWARD, 3)
  })
  ```

## [challenge] Pierwsza własna linijka (8 min)

### Co robić teraz
- Zadanie samodzielne: dopisz do programu **jedną** linijkę, własnoręcznie, w widoku tekstowym.
- Sposób pracy, którego użyjesz przez cały kurs: nie zgaduj składni. Przełącz na bloczki, ułóż bloczek, przełącz na tekst, zobacz, jak wygląda, wróć i napisz to sam.
- Zadanie: spraw, żeby Agent postawił blok pod sobą.
- Uruchom i sprawdź, czy działa.
- Kto skończy, wpisuje na czat swoją linijkę.

### Wskazówki
- [tempo] Metoda „ułóż z bloczków, zobacz w tekście, napisz sam” jest głównym narzędziem tego kursu. Powiedz ją wprost i sprawdź, czy każde dziecko jej użyło.
- [dla szybszych] Niech dopiszą trzy linijki bez zaglądania do bloczków i sprawdzą, czy nie zrobili literówki.
- [gdy nie zdążysz] Skopiowanie istniejącej linijki i zmiana jednego słowa wystarczy.
- [błąd] Dziecko pisze polecenie z błędną wielkością liter. Kod rozróżnia wielkie i małe litery — to pierwszy raz, gdy to ma znaczenie.

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Poproś, żeby nie zamykały gry ani edytora.
- Zapowiedź: „Po przerwie napiszecie cały program od zera, nie dotykając bloczków. I dowiecie się, dlaczego czerwone podkreślenie to wasz przyjaciel.”

### Wskazówki
- [tempo] Pierwsza połowa jest techniczna. Przerwa przed pierwszym samodzielnym pisaniem jest tu na właściwym miejscu.

## [concept] Podpowiedzi edytora i czerwone podkreślenie (12 min)

### Co robić teraz
- [mów] Edytor tekstowy nie jest złośliwy. On chce wam pomóc i robi to na trzy sposoby.
- **Sposób pierwszy — podpowiadanie.** Zacznijcie pisać słowo agent i kropkę. Wyskakuje lista wszystkiego, co Agent potrafi.
- Przejdźcie po liście strzałkami. Przy każdej pozycji jest krótki opis.
- [mów] To jest wasz spis poleceń. Nie musicie ich pamiętać — musicie wiedzieć, że lista istnieje.
- **Sposób drugi — czerwone podkreślenie.** Napiszcie celowo coś błędnie, na przykład agent.mowe zamiast agent.move.
- [mów] Edytor podkreśla to na czerwono, **zanim** uruchomicie program. To nie jest kara. To jest ostrzeżenie przed stratą czasu.
- Najedźcie na podkreślenie i przeczytajcie komunikat.
- [mów] Komunikaty są po angielsku i na początku wyglądają groźnie. Ale prawie zawsze mówią jedną z trzech rzeczy: nie znam takiego słowa, brakuje nawiasu, brakuje klamry.
- **Sposób trzeci — kolory.** Zwróćcie uwagę, że tekst w cudzysłowie ma inny kolor niż polecenia, a liczby jeszcze inny.
- [mów] Kolory nie są ozdobą. Jeśli cały wasz program nagle zrobił się jednym kolorem, to znaczy, że gdzieś zgubiliście cudzysłów.
- Ćwiczenie: każde dziecko celowo psuje jedną rzecz, ogląda podkreślenie i naprawia.

### Wskazówki
- [podpowiedź] Podejście „błąd to informacja, nie porażka” trzeba ustawić na pierwszej lekcji. W tej grupie wiekowej czerwone podkreślenie potrafi zniechęcić na cały kurs.
- [podpowiedź] Wskazówka o kolorach jako sygnale zgubionego cudzysłowu jest praktyczna i dzieci szybko zaczynają jej używać same.
- [błąd] Komunikat błędu po angielsku odstrasza dziecko, które nie zna języka. Przetłumacz kilka najczęstszych na głos i zapisz je na udostępnionym ekranie.
- [błąd] Lista podpowiedzi nie pojawia się, bo dziecko wpisało kropkę i od razu zaczęło pisać dalej. Trzeba chwilę poczekać albo wywołać ją skrótem.
- [dla szybszych] Niech przejrzą całą listę poleceń Agenta i wypiszą trzy, których jeszcze nie znają.
- [gdy nie zdążysz] Podpowiadanie i czerwone podkreślenie. O kolorach powiedz w jednym zdaniu.

## [guided] Cały program od zera, tekstem (15 min)

### Co robić teraz
- [mów] Teraz napiszecie program bez dotykania bloczków. Ja dyktuję, wy piszecie i pytacie, gdy coś nie gra.
- Krok 1: usuńcie zawartość edytora i zacznijcie od pustego miejsca.
- Krok 2: pierwsza linijka — komenda na czacie o nazwie „sciezka”. Uwaga na cudzysłowy wokół nazwy i na klamrę otwierającą na końcu linii.
- Krok 3: wciśnijcie Enter. Zauważcie, że edytor **sam** zrobił wcięcie i sam dopisał klamrę zamykającą.
- [mów] To jest ważne. Edytor pilnuje klamr za was. Jeśli któraś zniknie, to prawie zawsze dlatego, że skasowaliście ją ręcznie.
- Krok 4: napiszcie przeniesienie Agenta do gracza. Skorzystajcie z podpowiadania: wpiszcie agent, kropkę i wybierzcie z listy.
- Krok 5: napiszcie ustawienie materiału i aktywnego slotu.
- Krok 6: napiszcie cztery pary linijek: ruch w przód i postawienie bloku pod sobą.
- Krok 7: uruchomcie w grze. Ścieżka z czterech bloków.
- [mów] Napisaliście swój pierwszy program tekstowy. Ma dziewięć linijek i każdą z nich rozumiecie.
- Krok 8: przełączcie na widok bloków i sprawdźcie, czy to, co napisaliście, wygląda tak jak program blokowy.

### Wskazówki
- [podpowiedź] Sprawdzenie na końcu przez przełączenie na bloczki jest bardzo satysfakcjonujące i domyka pętlę z początku lekcji.
- [błąd] Brakująca klamra zamykająca. Najczęstszy błąd dnia. Powiedz, że edytor dopisuje ją sam i że kasowanie jej ręcznie to najczęstsza przyczyna kłopotów.
- [błąd] Nazwa komendy bez cudzysłowów. Kolory od razu pokażą, że coś jest nie tak.
- [błąd] Dziecko pisze polecenia z wielkiej litery, bo tak wygląda w bloczkach. W tekście wielkość liter ma znaczenie.
- [błąd] Program nie działa, a podkreśleń nie ma. Wtedy błąd jest logiczny, nie składniowy — Agent robi coś innego, niż dziecko chciało. To jest inna kategoria błędu i warto ją nazwać.
- [dla szybszych] Niech napiszą program budujący ścieżkę, która skręca, i zrobią to bez patrzenia na bloczki.
- [gdy nie zdążysz] Program z dwoma parami linijek zamiast czterech.

### Materiały
- [kod] Ścieżka napisana tekstem | javascript:
  ```
  player.onChat("sciezka", function () {
      agent.teleportToPlayer()
      agent.setItem(STONE, 64, 1)
      agent.setSlot(1)
      agent.move(FORWARD, 1)
      agent.place(DOWN)
      agent.move(FORWARD, 1)
      agent.place(DOWN)
      agent.move(FORWARD, 1)
      agent.place(DOWN)
      agent.move(FORWARD, 1)
      agent.place(DOWN)
  })
  ```

## [challenge] Twój pierwszy program tekstowy (8 min)

### Co robić teraz
- Zadanie samodzielne: napisz w widoku tekstowym program, który buduje coś twojego.
- Propozycje: ścieżka, która skręca w połowie; słup z bloków w górę; kwadrat z czterech krótkich odcinków.
- Zasada: piszesz tekstem. Do bloczków możesz zajrzeć tylko wtedy, gdy nie wiesz, jak coś się nazywa.
- Kto skończy, wkleja swój program na czat.

### Wskazówki
- [tempo] Wklejanie programów na czat pozwala Ci szybko zobaczyć, kto pisze poprawnie, a kto przekleja z bloczków bez zrozumienia.
- [dla szybszych] Niech napiszą program, w którym Agent buduje literę ich imienia z odcinków.
- [gdy nie zdążysz] Ścieżka z trzech bloków wystarczy.
- [błąd] Dziecko układa wszystko z bloczków i tylko przełącza widok na koniec. To nie jest błąd na tej lekcji, ale zwróć uwagę — od lekcji 2 chcemy pisania.

## [challenge] Pokaz i wyjście ze świata (5 min)

### Co robić teraz
- Przypomnij, że świat zapisuje się przy wyjściu przez menu, a nie przez zamknięcie okna.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko udostępnia ekran, pokazuje swój kod tekstem i uruchamia go w grze.
- Poproś, żeby przeczytało na głos jedną linijkę i powiedziało, co ona robi.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Czytanie własnego kodu na głos jest najlepszym sprawdzeniem zrozumienia. Wprowadź to jako stały element pokazów.
- [błąd] Dziecko nie umie udostępnić ekranu — ustal to raz, na pierwszej lekcji.

## [summary] Podsumowanie i plan roku (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: czym różni się widok bloków od widoku tekstu? Co zrobić, gdy nie wiesz, jak coś napisać? Co oznacza czerwone podkreślenie?
- [mów] Dzisiaj odkryliście najważniejszą rzecz tego kursu: bloczki i tekst to ten sam program. Nie uczycie się nowego języka. Uczycie się go zapisywać.
- Pokaż plan roku w trzech zdaniach: pierwszy semestr to funkcje i generatory budowli, drugi to generowanie całych dzielnic, trzeci to własny projekt miasta i drugi język.
- Zajawka: „Na następnych zajęciach zajmiemy się składnią: nawiasami, klamrami i przecinkami. Dostaniecie zepsute programy do naprawienia.”
- Przypomnij zadanie domowe: przepisz program z bloczków na tekst i sprawdź przełącznikiem.

### Wskazówki
- [błąd] Brakująca klamra zamykająca — najczęstszy błąd pierwszego miesiąca.
- [błąd] Zła wielkość liter w nazwie polecenia.
- [błąd] Nazwa komendy bez cudzysłowów — poznasz po kolorach.
- [błąd] Nie da się wrócić z tekstu do bloczków — tekst zawiera coś, czego bloczki nie potrafią pokazać.
- [błąd] Program bez podkreśleń, a nie działa — to błąd logiczny, nie składniowy.
