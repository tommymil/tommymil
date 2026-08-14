# Pętla „powtórz” — układ taneczny
Subject: Scratch
Level: Poziom 1 — 7-9 lat
Czas: 95 min
Tags: Pętla, Powtarzanie, Taniec, Kostiumy
Opis: Zamiast układać dwadzieścia bloczków, dzieci piszą jeden i każą go powtórzyć. Efektem jest układ taneczny na spację.
Cel: Dziecko rozumie, że pętla zastępuje wielokrotne przepisywanie tych samych bloczków, i potrafi dobrać liczbę powtórzeń do zamierzonego efektu.

### Po zajęciach dziecko potrafi
- zastąpić powtarzające się bloczki jedną pętlą powtórz
- odróżnić pętlę powtórz od pętli zawsze i powiedzieć, kiedy której użyć
- zbudować sekwencję taneczną z kostiumów, dźwięku i efektu
- dobrać liczbę powtórzeń i czas czekania tak, żeby efekt wyglądał dobrze

### Przygotuj przed zajęciami
- gotowe demo: postać tańcząca do muzyki po wciśnięciu spacji
- przygotowany celowo zły przykład: dwadzieścia tych samych bloczków jeden pod drugim
- projekty dzieci z lekcji 5
- sprawdzony dźwięk — dziś gra muzyka i wszyscy muszą ją słyszeć

### Zadanie domowe
- zatańcz razem ze swoją postacią i pokaż układ komuś w domu

## [intro] Powitanie i demo tańca (10 min)

### Co robić teraz
- [mów] Cześć! Dziś wasze postacie zatańczą. I nauczycie się przy tym najważniejszego bloczka w całym programowaniu.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** wciśnij spację i pokaż tańczącą postać z muzyką i zmieniającym się kolorem. Zrób to dwa razy.
- [mów] A teraz pokażę wam, jak NIE należy tego robić.
- Pokaż na swoim ekranie celowo zły przykład: dwadzieścia razy ten sam bloczek jeden pod drugim, tak długi, że nie mieści się na ekranie.
- Zapytaj grupę: ile czasu zajęłoby wam ułożenie tego? A co, gdybym chciał sto powtórzeń zamiast dwudziestu?
- [mów] Programiści są leniwi i to jest ich zaleta. Zamiast pisać sto razy to samo, piszą raz i każą komputerowi powtórzyć.

### Wskazówki
- [tempo] Pokazanie złego przykładu **przed** dobrym jest kluczem tej lekcji. Bez tego pętla jest kolejnym bloczkiem. Z tym jest ulgą.
- [podpowiedź] Przewiń ten długi stos bloczków w górę i w dół na oczach dzieci. Efekt „ojej, ile tego” działa lepiej niż słowa.

## [review] Otwieramy projekt i przypominamy dialog (5 min)

### Co robić teraz
- Wszyscy otwierają projekt z lekcji 5. Poczekaj na potwierdzenie od każdego dziecka.
- Kliknijcie flagę i obejrzyjcie swoją scenkę.
- Szybka powtórka: po co jest bloczek czekaj w rozmowie? Co zrobić, gdy postać zniknęła? Jak sprawdzić, którego duszka edytujesz?
- Zapytaj, kto pamiętał o zadaniu domowym i wymyślił trzecie zdanie. Kto ma, dopisuje je teraz — dwie minuty.

### Wskazówki
- [podpowiedź] Wracanie do zadania domowego na następnych zajęciach jest jedynym sposobem, żeby ktokolwiek je robił. Jeśli raz odpuścisz, przestaną.
- [gdy nie zdążysz] Sam start projektu wystarczy, pytania pomiń.

## [concept] Pętla powtórz — jedno polecenie zamiast dwudziestu (12 min)

### Co robić teraz
- [mów] Znacie już bloczek zawsze z lekcji drugiej. Dziś poznajecie jego młodszego brata, który potrafi liczyć.
- Pokaż pomarańczową kategorię Kontrola i bloczek powtórz 10. Zwróć uwagę na kształt — to ramka, która obejmuje inne bloczki.
- Zbuduj razem na pierwszej postaci osobny skrypt: kiedy klawisz spacja naciśnięty, a pod tym powtórz 10, a w środku ramki następny kostium oraz czekaj 0.2 sek.
- Wciśnijcie spację. Postać przebiera kostiumami dziesięć razy i staje. **Czekaj na kciuki.**
- [mów] Trzy bloczki. A gdybyście robili to bez pętli, potrzebowalibyście dwudziestu.
- Zmieńcie 10 na 30. Wciśnijcie spację. Taniec trwa trzy razy dłużej, a wy nie dołożyliście ani jednego bloczka.
- [mów] I to jest cała moc pętli. Zmieniacie jedną liczbę, a zmienia się cały efekt.
- Wyjaśnij różnicę wprost: powtórz robi coś **określoną liczbę razy** i kończy, a zawsze robi coś **bez końca**, dopóki nie wciśniesz czerwonego znaku stop.

### Wskazówki
- [błąd] Bloczki wylądowały **pod** ramką zamiast w środku. Poznasz to po tym, że taniec dzieje się raz. Pokaż różnicę na swoim ekranie — bloczki w środku są wcięte w prawo.
- [podpowiedź] Dobre pytanie sprawdzające dla siedmiolatka: „chcesz, żeby to się kiedyś skończyło?”. Jeśli tak, to powtórz. Jeśli nie, to zawsze.
- [tempo] To jest najważniejsze pojęcie pierwszego semestru. Jeśli któryś krok ma dostać dodatkowe minuty, niech to będzie ten.
- [dla szybszych] Niech sprawdzą, co się stanie, gdy wstawią pętlę powtórz w środek innej pętli powtórz. Nie tłumacz — niech odkryją.

### Materiały
- [kod] Pierwszy taniec w pętli | scratch:
  ```
  kiedy klawisz [spacja] naciśnięty
    powtórz (10)
      następny kostium
      czekaj (0.2) sek
  ```

## [guided] Budujemy układ taneczny krok po kroku (18 min)

### Co robić teraz
- Budujemy razem. Po każdym kroku czekasz na kciuki od wszystkich.
- Krok 1: sprawdź, czy twoja postać ma dużo kostiumów. Zakładka Kostiumy, lista po lewej. Kto ma mniej niż cztery, wybiera teraz nowego duszka z kategorii Taniec. **Czekaj na kciuki.**
- Krok 2: dodaj muzykę. Zakładka Dźwięki, ikona wyboru dźwięku, propozycje: Dance Around, Dance Magic, Drum Machine.
- Krok 3: na początek skryptu ze spacją, **przed** pętlą, wstaw zagraj dźwięk z wybraną muzyką.
- Krok 4: wciśnij spację. Muzyka gra, postać tańczy. **Czekaj na kciuki.**
- [mów] Zwróćcie uwagę: użyliśmy zagraj dźwięk bez „i czekaj”. Tu tak ma być — chcemy, żeby muzyka grała, a taniec działał się w tym samym czasie.
- Krok 5: dołóż do środka pętli bloczek zmień efekt kolor o 25. Wciśnij spację. Postać tańczy w tęczy.
- Krok 6: **pod** pętlą, już poza nią, wstaw wyczyść efekty graficzne. Postać wraca do normalnego wyglądu po tańcu.
- [mów] Popatrzcie, gdzie stoi ten ostatni bloczek. W środku pętli czyściłby kolor po każdym kroku i nic byśmy nie zobaczyli. Miejsce bloczka znaczy tyle samo co sam bloczek.
- Krok 7: dobierzcie liczby. Ile powtórzeń? Ile czekania? Wypróbujcie trzy różne wersje.

### Wskazówki
- [błąd] Postać nie zmienia kostiumów — duszek ma tylko jeden kostium. Sprawdzenie z kroku 1 istnieje właśnie po to.
- [błąd] Kolor nie wraca do normalnego — bloczek wyczyść efekty graficzne wylądował w środku pętli albo go brakuje.
- [błąd] Muzyka urywa się w połowie tańca — jest po prostu krótsza niż taniec. Albo skróć taniec, albo wybierz dłuższy utwór.
- [błąd] Przy wielokrotnym wciskaniu spacji muzyka nakłada się sama na siebie. Powiedz, że tak działa ten bloczek, i pokaż czerwony znak stop jako ratunek.
- [dla szybszych] Niech zbudują dwie różne sekwencje pod dwoma klawiszami — spacja i litera d — i zrobią z tego dwa układy do wyboru.
- [gdy nie zdążysz] Zatrzymaj się na kroku 5. Sprzątanie efektów po tańcu zrób razem w kroku po przerwie.

### Materiały
- [kod] Układ taneczny | scratch:
  ```
  kiedy klawisz [spacja] naciśnięty
    zagraj dźwięk [Dance Around]
    powtórz (16)
      następny kostium
      zmień efekt [kolor] o (25)
      czekaj (0.2) sek
    wyczyść efekty graficzne
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów, piją wodę, rozprostowują się.
- Wyłączcie muzykę czerwonym znakiem stop przed przerwą.
- Zapowiedź: „Po przerwie wasza postać zacznie tańczyć po całej scenie i zmieniać rozmiar. A na koniec zrobimy dyskotekę całej grupy.”

### Wskazówki
- [tempo] Dziesięć niewyciszonych mikrofonów z muzyką w tle to hałas nie do słuchania. Wyciszenie przed przerwą jest praktyczne, nie kosmetyczne.

## [concept] Taniec w ruchu i zmiana rozmiaru (12 min)

### Co robić teraz
- [mów] Wasz taniec jest w miejscu. Prawdziwy tancerz rusza się po całej scenie.
- Pokaż bloczek zmień rozmiar o 10 w kategorii Wygląd. Wstaw go do środka pętli i wciśnij spację — postać rośnie w trakcie tańca.
- [mów] Uwaga, pułapka: jeśli postać tylko rośnie, po trzech tańcach nie zmieści się na scenie. Trzeba ją zmniejszyć z powrotem.
- Pokaż dwa rozwiązania i pozwól dzieciom wybrać:
- Pierwsze: pod pętlą, poza nią, wstaw ustaw rozmiar na 100 procent.
- Drugie, ładniejsze: zrób dwie pętle jedna pod drugą — pierwsza powiększa osiem razy, druga zmniejsza osiem razy. Postać oddycha.
- Dołóż ruch: do środka pętli wstaw przesuń o 20 kroków oraz jeżeli na brzegu, odbij się.
- Nie zapomnijcie o ustaw styl obrotu na lewo-prawo — inaczej wiecie, co się stanie.

### Wskazówki
- [podpowiedź] Druga wersja z dwiema pętlami wygląda dużo lepiej i jest doskonałym ćwiczeniem. Pokaż ją nawet tym dzieciom, które zostaną przy pierwszej.
- [błąd] Postać rośnie do gigantycznych rozmiarów i zasłania scenę — brakuje przywrócenia rozmiaru po pętli.
- [błąd] Postać tańczy do góry nogami — styl obrotu lewo-prawo, stary znajomy z lekcji 2 i 3.
- [błąd] Postać wytańczyła się poza scenę i już nie wraca — dodaj idź do x: 0 y: 0 na początku skryptu ze spacją.
- [dla szybszych] Niech zbudują trzy pętle pod rząd: powiększanie, obracanie, zmniejszanie. To jest już prawdziwa choreografia.
- [gdy nie zdążysz] Sama zmiana rozmiaru z przywróceniem po pętli wystarczy, ruch pomiń.

### Materiały
- [kod] Taniec z oddechem — dwie pętle | scratch:
  ```
  kiedy klawisz [spacja] naciśnięty
    ustaw styl obrotu na [lewo-prawo]
    zagraj dźwięk [Dance Around]
    powtórz (8)
      następny kostium
      zmień rozmiar o (10)
      czekaj (0.2) sek
    powtórz (8)
      następny kostium
      zmień rozmiar o (-10)
      czekaj (0.2) sek
    wyczyść efekty graficzne
  ```

## [guided] Dyskoteka — tło też tańczy (13 min)

### Co robić teraz
- [mów] Zostało nam ostatnie: sama scena. Zrobimy dyskotekę.
- Krok 1: kliknij **Scenę** w prawym dolnym rogu. Przypomnij, po czym poznać, że jesteś na Scenie — znika kategoria Ruch. **Czekaj na kciuki.**
- Krok 2: dodaj Scenie drugie i trzecie tło. Ikona wyboru tła, dowolne trzy obrazy.
- Krok 3: zbuduj na Scenie skrypt: kiedy klawisz spacja naciśnięty, a pod tym powtórz 8, a w środku następne tło oraz czekaj 0.4 sek.
- Krok 4: wciśnijcie spację. Postać tańczy, tło miga, muzyka gra. **Czekaj na kciuki.**
- [mów] Zauważcie coś ważnego: postać i Scena wykonują swoje pętle **w tym samym czasie**. Wciśnięcie spacji uruchomiło dwa programy naraz.
- Krok 5: dobierzcie czasy tak, żeby tło zmieniało się do rytmu, a nie za szybko.

### Wskazówki
- [podpowiedź] To pierwszy raz, gdy dzieci widzą dwa programy działające równocześnie po jednym zdarzeniu. To duże pojęcie — wystarczy, że je zobaczą, nie muszą go nazwać.
- [błąd] Tło nie miga — skrypt trafił na duszka zamiast na Scenę. Pierwsze miejsce do sprawdzenia dziś.
- [błąd] Tło zostaje inne po zakończeniu tańca — dołóż na Scenie do skryptu z zieloną flagą bloczek zmień tło na, z tłem startowym.
- [dla szybszych] Niech dodadzą do Sceny czwarte i piąte tło, a liczbę powtórzeń dobiorą tak, żeby taniec kończył się na tym samym tle, na którym się zaczął.
- [gdy nie zdążysz] Dwa tła zamiast trzech, reszta bez zmian.

### Materiały
- [kod] Migające tło — skrypt na Scenie | scratch:
  ```
  kiedy klawisz [spacja] naciśnięty
    powtórz (8)
      następne tło
      czekaj (0.4) sek
  ```

## [challenge] Twoja choreografia (10 min)

### Co robić teraz
- Zadanie samodzielne: ułóż własny taniec. Cztery rzeczy do wyboru, każde dziecko robi tyle, ile zdąży.
- Dobierz liczbę powtórzeń i czas czekania do rytmu swojej muzyki.
- Dołóż drugą pętlę z innym ruchem.
- Spraw, żeby druga postać z lekcji 5 też tańczyła.
- Dobierz tempo migania tła do muzyki.
- Chodź po grupie i pytaj po imieniu: „do jakiej muzyki tańczy twoja postać?”.

### Wskazówki
- [tempo] Dobieranie liczb do rytmu to prawdziwa praca programisty: próba, ocena, poprawka. Nie podawaj gotowych wartości.
- [dla szybszych] Niech spróbują zsynchronizować obie postacie tak, żeby tańczyły na zmianę — jedna czeka, druga tańczy.
- [gdy nie zdążysz] Sama zmiana liczby powtórzeń wystarczy za całe zadanie.
- [błąd] Dziecko dodało tyle bloczków do pętli, że taniec trwa minutę. Zaproponuj mniej powtórzeń zamiast usuwania bloczków.

## [challenge] Dyskoteka całej grupy (5 min)

### Co robić teraz
- Najpierw zapis: Plik, Zapisz teraz. Poczekaj na potwierdzenie od każdego dziecka.
- „Scena dla każdego”, dziś w wersji dyskotekowej: po kolei, po imieniu, każde dziecko udostępnia ekran i wciska spację. Reszta grupy tańczy przed swoimi kamerami.
- Brawa po każdym pokazie.

### Wskazówki
- [tempo] To najweselsze pięć minut pierwszego semestru. Warto na nie pilnować czasu wcześniej.
- [podpowiedź] Zaproponuj, żeby dzieci wstały. Po ośmiu tygodniach siedzenia przy ekranie to jest coś, co zapamiętają.
- [błąd] Muzyka od dziesięciorga dzieci naraz to hałas. Poproś o wyciszenie mikrofonów wszystkim poza pokazującym.

## [summary] Pułapki i podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: czym różni się powtórz od zawsze? Co się dzieje, gdy bloczek jest pod pętlą, a nie w środku? Dlaczego programiści używają pętli?
- [mów] Dzisiaj nauczyliście się rzeczy, której używa każdy programista na świecie, codziennie. Nie przepisuj tego samego dwadzieścia razy — napisz raz i każ powtórzyć.
- Przypomnij zadanie domowe: zatańcz razem z postacią i pokaż układ komuś w domu.
- Zajawka: „Na następnych zajęciach zajmiemy się dźwiękiem i efektami na poważnie. Wasze postacie będą mogły stać się duchami, wielkoludami i krasnoludkami.”

### Wskazówki
- [błąd] Bloczki pod pętlą zamiast w środku — taniec dzieje się raz zamiast wiele razy.
- [błąd] Postać ma jeden kostium i nie ma czym tańczyć.
- [błąd] Kolor albo rozmiar nie wraca do normalnego — sprzątanie musi być poza pętlą.
- [błąd] Skrypt migania tła zbudowany na duszku zamiast na Scenie.
- [błąd] Muzyka nakłada się przy szybkim wciskaniu spacji — czerwony znak stop czyści sytuację.
