# Losowość i tempo gry — trudność, która rośnie
Subject: Scratch
Level: Poziom 2 — 10-12 lat
Czas: 95 min
Tags: Losowość, Balans, Trudność, Projektowanie gier
Opis: Lekcja o tym, co odróżnia grę dobrą od działającej. Dzieci używają losowości świadomie, budują rosnącą trudność i testują balans na cudzej grze.
Cel: Dziecko używa losowości do budowania nieprzewidywalności, potrafi uzależnić trudność od postępu gracza i ocenia balans gry na podstawie testów, a nie przeczucia.

### Po zajęciach dziecko potrafi
- wstawić losowanie w miejsce stałej wartości i dobrać sensowny zakres
- zbudować trudność zależną od wyniku albo od upływu czasu
- wskazać, kiedy losowość psuje grę, a kiedy ją ratuje
- ocenić cudzą grę i sformułować konkretną uwagę o balansie

### Przygotuj przed zajęciami
- gotowe demo: ta sama gra w trzech wersjach — za łatwa, za trudna, wyważona
- przygotowany przykład złej losowości: przeciwnik teleportujący się w losowe miejsca co pół sekundy
- projekty dzieci z lekcji 6
- zebrane zadania domowe — lista rzeczy, które dzieci uznały za denerwujące we własnych grach

### Zadanie domowe
- daj komuś w domu zagrać w swoją grę i zapisz, w której chwili się poddał albo znudził

## [intro] Powitanie i demo trzech wersji (8 min)

### Co robić teraz
- [mów] Cześć. Wasze gry działają. Dziś pytanie brzmi inaczej: czy są dobre?
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO — trzy wersje tej samej gry.** Zagraj w każdą po dwadzieścia sekund.
- Wersja pierwsza: przeciwnik wolny, monety wszędzie. Wygrywasz bez wysiłku.
- Wersja druga: przeciwnik szybszy od gracza. Przegrywasz w trzy sekundy.
- Wersja trzecia: wyważona. Da się wygrać, ale trzeba się postarać.
- [mów] To jest ten sam kod. Różnią się tylko liczbami. A grywalność zmienia się kompletnie.
- Odczytaj na głos kilka uwag z zadania domowego, bez podawania imion.
- [mów] Dziś zajmiemy się właśnie tym. To nie jest lekcja o nowych bloczkach, tylko o tym, jak decydować, które liczby wpisać.

### Wskazówki
- [tempo] Ta lekcja ma najmniej nowego materiału w całym semestrze i najwięcej myślenia. Nie wypełniaj jej bloczkami na siłę.
- [podpowiedź] Anonimowe odczytanie uwag z zadania domowego pokazuje dzieciom, że wszyscy mają podobne problemy. Robi więcej dla atmosfery w grupie niż jakiekolwiek ćwiczenie.

## [review] Otwieramy projekt i przypominamy kolizje (5 min)

### Co robić teraz
- Wszyscy otwierają projekt z lekcji 6. Poczekaj na potwierdzenie od każdego dziecka.
- Zróbcie test trzech uruchomień pod rząd. Kto ma problem ze stanem początkowym, mówi teraz.
- Szybka powtórka: dlaczego jedno dotknięcie potrafiło odebrać kilka żyć? Jakie znacie sposoby, żeby coś stało się raz?
- Kto zrobił zadanie domowe, wpisuje na czat jedną rzecz, która go denerwuje we własnej grze.

### Wskazówki
- [podpowiedź] Lista uwag z czatu jest materiałem na całą drugą połowę zajęć. Skopiuj ją sobie na bok.
- [gdy nie zdążysz] Sam test trzech uruchomień.

## [concept] Losowość dobra i zła (14 min)

### Co robić teraz
- [mów] Bloczek losuj znacie od lekcji 4. Dziś nie chodzi o to, jak go użyć, tylko **kiedy**.
- **Zły przykład, na żywo:** pokaż przeciwnika, który co pół sekundy teleportuje się w losowe miejsce sceny.
- [mów] Zagrajcie w to. Da się? Nie da. Nie dlatego, że jest trudne, tylko dlatego, że nie da się niczego zaplanować.
- [mów] Zasada pierwsza: **losowość ma dawać różnorodność, a nie odbierać kontrolę**. Gracz musi móc podjąć decyzję, która ma sens.
- **Dobry przykład:** ten sam przeciwnik, który goni gracza, ale co kilka sekund losuje sobie nową prędkość z wąskiego zakresu.
- [mów] Zasada druga: losujcie z **wąskiego zakresu wokół sensownej wartości**, a nie z całego możliwego przedziału.
- Ćwiczenie z całą grupą. Podaję element gry, wy piszecie na czat, czy warto go losować.
- Miejsce pojawienia się monety. Prędkość gracza. Czas do pojawienia się bonusu. Kierunek, w którym patrzy przeciwnik. Liczba punktów za monetę.
- Omów odpowiedzi. Prędkość gracza losowa to katastrofa — gracz nie kontroluje własnej postaci. Miejsce monety losowe to oczywista wartość dodana.
- Krok praktyczny: w swojej grze zamieńcie **jedną** stałą wartość na losowanie z wąskiego zakresu i sprawdźcie, czy gra na tym zyskała.

### Wskazówki
- [podpowiedź] Ćwiczenie „co warto losować” jest sednem tej lekcji. Poświęć mu więcej czasu niż kodowaniu, jeśli grupa dyskutuje.
- [błąd] Dziecko losuje wszystko, co się da, i gra staje się chaosem. To normalny etap. Zamiast zabraniać, każ zagrać i opisać wrażenie.
- [błąd] Zakres losowania zaczyna się od zera, na przykład losuj od 0 do 5 dla prędkości. Wtedy przeciwnik czasem stoi. Zwykle to błąd, ale czasem ciekawy efekt — zapytaj, czy było zamierzone.
- [dla szybszych] Niech zbudują przeciwnika, który losowo wybiera jedno z trzech zachowań: goni, patroluje, stoi. Wymaga warunku na wylosowanej liczbie.

### Materiały
- [kod] Losowość z wąskiego zakresu | scratch:
  ```
  kiedy kliknięto zieloną flagę
    zawsze
      ustaw [prędkość wroga] na (losuj od (2) do (4))
      czekaj (3) sek
  ```

## [guided] Trudność, która rośnie (18 min)

### Co robić teraz
- [mów] Dobra gra nie jest jednakowo trudna przez cały czas. Zaczyna łatwo i podkręca tempo, gdy gracz się rozgrzeje.
- Krok 1: utwórzcie zmienną **poziom** i wyzerujcie ją do 1 w stanie początkowym na Scenie.
- Krok 2: na Scenie zbudujcie: pętla zawsze, w niej jeżeli wynik większy niż 5 razy poziom to zmień poziom o 1.
- [mów] Zwróćcie uwagę na wyrażenie w warunku. Bloczek mnożenia jest w kategorii Wyrażenia i wchodzi do porównania. Wartość w wartości — jak pętla w pętli.
- Krok 3: sprawdźcie, czy poziom rośnie w trakcie gry. Wpiszcie na czat G albo znak zapytania.
- Krok 4: teraz podepnijcie coś pod poziom. Na przeciwniku zamieńcie stałą prędkość na wyrażenie: 2 plus poziom.
- Krok 5: zagrajcie. Przez pierwsze kilkanaście sekund jest spokojnie. Potem robi się gorąco.
- [mów] To jest **krzywa trudności** i tak zbudowana jest każda gra, w jaką graliście. Pierwszy poziom uczy, ostatni sprawdza.
- Krok 6: podepnijcie pod poziom drugą rzecz — na przykład czas, po którym moneta zmienia miejsce: 2 minus poziom razy 0.2.
- Krok 7: przetestujcie i sprawdźcie skrajność. Co się stanie przy poziomie 10? Jeśli czekanie zejdzie poniżej zera, gra się zepsuje.
- [mów] To jest ważny nawyk: zawsze sprawdzajcie, co się stanie przy bardzo dużej i bardzo małej wartości. Tam mieszkają błędy.
- Krok 8: ograniczcie poziom warunkiem, żeby nie rósł w nieskończoność.

### Wskazówki
- [podpowiedź] Sprawdzanie wartości skrajnych to jedna z najbardziej przenośnych umiejętności w tym kursie. Nazwij ją i wracaj do niej.
- [błąd] Poziom rośnie w kółko, kilkadziesiąt razy na sekundę, bo warunek pozostaje prawdziwy. Rozwiązanie: po zmianie poziomu dołóż czekanie albo porównuj z wynikiem przemnożonym przez nowy poziom.
- [błąd] Wyrażenie z mnożeniem nie chce wejść do porównania — dziecko trafia w złą dziurkę. Owal wchodzi do owalnego pola.
- [błąd] Przy wysokim poziomie czekanie schodzi poniżej zera i moneta miga bez przerwy. To jest właśnie wartość skrajna, o której mowa w kroku 7.
- [błąd] Gra staje się nie do przejścia po dwudziestu sekundach. Zmniejsz tempo wzrostu — poziom co 10 punktów zamiast co 5.
- [dla szybszych] Niech zbudują trudność zależną **jednocześnie** od wyniku i od pozostałego czasu, tak żeby końcówka rundy była najtrudniejsza.
- [gdy nie zdążysz] Sam poziom podpięty pod prędkość przeciwnika. Druga zależność i ograniczenie zostają na później.

### Materiały
- [kod] Rosnący poziom — skrypt na Scenie | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw [poziom] na (1)
    zawsze
      jeżeli <(wynik) > ((5) * (poziom))> to
        zmień [poziom] o (1)
        czekaj (0.5) sek
  ```
- [kod] Prędkość zależna od poziomu — skrypt na przeciwniku | scratch:
  ```
  kiedy kliknięto zieloną flagę
    zawsze
      skieruj się w stronę [Gracz]
      przesuń o ((2) + (poziom)) kroków
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Zapowiedź: „Po przerwie zagracie w gry kolegów i powiecie im, co poprawić. To jest praca, którą w firmach robią testerzy.”

### Wskazówki
- [tempo] Zapowiedź testowania cudzych gier działa lepiej niż zapowiedź nowego bloczka. Dzieci są ciekawe, co zrobili inni.

## [concept] Balans — jak ocenić własną grę (13 min)

### Co robić teraz
- [mów] Największy problem twórcy gier: on zna swoją grę na pamięć i przez to nie umie ocenić, czy jest trudna.
- [mów] Dlatego istnieją testerzy. Ktoś, kto widzi grę pierwszy raz i mówi, co jest niejasne.
- Podaj cztery pytania, na które ma odpowiedzieć dobry test. Zapisz je na udostępnionym ekranie — dzieci będą z nich korzystać w następnym kroku.
- Czy w pierwszych dziesięciu sekundach wiadomo, co robić?
- Czy da się wygrać? Czy da się przegrać?
- Czy jest moment, w którym gracz nudzi się albo się poddaje?
- Czy coś zaskakuje w złym sensie — działa inaczej, niż się spodziewasz?
- [mów] Zwróćcie uwagę, że żadne z tych pytań nie brzmi „czy fajna”. Odpowiedź „fajna” nikomu nie pomaga.
- Ćwiczenie: każde dziecko odpowiada na te cztery pytania **o własnej grze**, na kartce. Cztery minuty.
- Zbierz kilka odpowiedzi na forum. Zapytaj, kto odkrył coś, czego wcześniej nie zauważył.
- [mów] Teraz najtrudniejsza część: poprawcie jedną rzecz, którą sami u siebie znaleźliście. Nie tę najłatwiejszą — tę najważniejszą.

### Wskazówki
- [podpowiedź] Lista czterech pytań jest przenośna. Można ją zostawić na ekranie na każdych kolejnych zajęciach z pokazem prac.
- [błąd] Dziecko odpowiada na wszystko „jest dobrze”, bo nie chce krytykować własnej pracy. Zadaj konkretne pytanie: „ile sekund zajmuje ci wygrana?”.
- [tempo] Czas na kartkę jest tu obowiązkowy. Odpowiedzi ustne będą powierzchowne.
- [dla szybszych] Niech zapiszą, którą liczbę w kodzie trzeba zmienić dla każdej ze swoich uwag. Nie każdą uwagę da się rozwiązać jedną liczbą i to też jest odkrycie.

## [guided] Testujemy gry nawzajem (14 min)

### Co robić teraz
- [mów] Teraz zagracie w gry kolegów. Zasada jest jedna: żadnych „fajne”. Konkretne uwagi z listy czterech pytań.
- Krok 1: dobierzcie się w pary. Dzieci udostępniają sobie linki do projektów na czacie — Plik, Udostępnij, potem skopiowanie adresu.
- Krok 2: każde dziecko gra w grę partnera przez trzy minuty. **Nie tłumacząc, co robić** — o to właśnie chodzi.
- Krok 3: każde wpisuje na czat trzy uwagi, po jednej z pytań z listy.
- Krok 4: wracacie do własnych gier i czytacie, co dostaliście.
- Krok 5: wybierzcie jedną uwagę i popraw ją teraz. Zostało wam na to pięć minut.
- [mów] Nie musicie zgadzać się z każdą uwagą. Ale jeśli dwie osoby mówią to samo, to prawie zawsze mają rację.

### Wskazówki
- [podpowiedź] Zakaz tłumaczenia, jak grać, jest najważniejszą regułą tego ćwiczenia. To właśnie wtedy wychodzi, że gra jest niezrozumiała bez autora obok.
- [błąd] Dzieci piszą sobie same miłe rzeczy. Powiedz wprost, że uwaga bez konkretu jest bezużyteczna, i pokaż przykład dobrej: „nie wiedziałem, że mam zbierać kule, przez pierwsze pół minuty uciekałem”.
- [błąd] Link do projektu nie działa, bo dziecko nie kliknęło Udostępnij. Sprawdź to przed rozpoczęciem ćwiczenia.
- [błąd] Uwaga jest krzywdząca albo złośliwa. Reaguj natychmiast i przeformułuj ją na forum na wersję konstruktywną — to jest lekcja, nie tylko incydent.
- [dla szybszych] Niech przetestują dwie gry zamiast jednej i porównają, która ma lepszą krzywą trudności.
- [gdy nie zdążysz] Testowanie w parach bez poprawek. Poprawki zostają jako zadanie domowe.

## [challenge] Popraw balans swojej gry (8 min)

### Co robić teraz
- Zadanie samodzielne: dostrój swoją grę tak, żeby dało się w nią wygrać po pewnym wysiłku.
- Cel liczbowy: gra ma być **do przejścia za trzecim albo czwartym podejściem**. Nie za pierwszym i nie za dziesiątym.
- Zagrajcie, zmieńcie jedną liczbę, zagrajcie ponownie. Po jednej naraz — inaczej nie będziecie wiedzieć, co zadziałało.
- Zapiszcie na czacie, którą liczbę zmieniliście i z czego na co.

### Wskazówki
- [podpowiedź] Reguła „zmieniaj po jednej rzeczy naraz” to podstawa szukania błędów i strojenia. Powiedz wprost, że tak samo szuka się przyczyny awarii.
- [dla szybszych] Niech przygotują dwa zestawy liczb — łatwy i trudny — i przełączają je klawiszem. To zapowiedź poziomów trudności z lekcji 22.
- [gdy nie zdążysz] Jedna zmieniona liczba wystarczy.
- [błąd] Dziecko zmienia pięć liczb naraz i nie wie, co pomogło. To jest właśnie moment, żeby pokazać, po co jest zasada jednej zmiany.

## [challenge] Pokaz prac i zapis (5 min)

### Co robić teraz
- Zapis: Plik, Zapisz teraz. Poczekaj na potwierdzenie od każdego dziecka.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko mówi, jaką uwagę dostało i co z nią zrobiło.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Dziś pokaz jest o poprawce, nie o efekcie. Pochwal dzieci, które przyjęły trudną uwagę i coś z nią zrobiły.
- [tempo] Zapis przed pokazem, zawsze.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: kiedy losowość pomaga, a kiedy psuje grę? Po co sprawdzać wartości skrajne? Dlaczego autor nie umie ocenić własnej gry?
- [mów] Dzisiaj nie nauczyliście się prawie żadnego nowego bloczka. Nauczyliście się czegoś trudniejszego: patrzeć na własną pracę oczami kogoś, kto widzi ją pierwszy raz.
- [mów] To jest umiejętność, która przyda się wam wszędzie, nie tylko w Scratchu.
- Zajawka: „Na następnych zajęciach poznacie klony. Jednym skryptem zbudujecie dwadzieścia przeciwników naraz — i skończycie pierwsze dwa miesiące pełną grą.”
- Przypomnij zadanie domowe: daj komuś w domu zagrać i zapisz, kiedy się poddał.

### Wskazówki
- [błąd] Losowość zastosowana do rzeczy, które gracz ma kontrolować.
- [błąd] Zbyt szeroki zakres losowania — chaos zamiast różnorodności.
- [błąd] Poziom rosnący kilkadziesiąt razy na sekundę — brak czekania albo błędny warunek.
- [błąd] Wartość skrajna psuje grę: czekanie poniżej zera, prędkość większa od rozmiaru sceny.
- [błąd] Pięć zmian naraz i brak wiedzy, która zadziałała.
