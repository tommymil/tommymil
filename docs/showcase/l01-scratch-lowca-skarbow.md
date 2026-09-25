# Pokazowa: Łowca Skarbów - Rzadka Gwiazda +3
Rodzaj: pokazowa
Subject: Scratch
Level: Pokazowa - 8-10 lat
Czas: 60 min
Tags: Pokazowa, Scratch, Gra, Punkty, Kolizje, Zmienne, Komunikaty, Balans
Opis: Podczas lekcji zdalnej dziecko najpierw ogląda FINAL wyłącznie na ekranie instruktora, następnie otrzymuje START, uruchamia go na własnym udostępnionym ekranie i odkrywa brak reakcji rzadkiej gwiazdy. W duszku `nagroda rzadka` programuje zamknięcie stanu `aktywna`, dodanie 3 punktów, dźwięk, komunikat uruchamiający `popup +3`, zniknięcie i krótkie zabezpieczenie przed ponownym zebraniem.
Cel: Dziecko rozumie, że duszki współpracują przez zmienne, warunki i komunikaty, oraz buduje w rzadkiej nagrodzie kompletną, jednorazową reakcję na dotknięcie gracza.

### Po zajęciach dziecko potrafi
- wskazać duszka `nagroda rzadka` i odróżnić jego skrypt pojawiania od skryptu zebrania
- wyjaśnić, dlaczego warunek sprawdza jednocześnie `aktywna = 1` oraz dotknięcie gracza
- zwiększyć bieżący wynik o 3, zamiast zastępować cały wynik liczbą 3
- wysłać komunikat `rzadka +3`, który uruchamia efekt w innym duszku
- przetestować, czy jedno zebranie nalicza punkty i pokazuje `popup +3` tylko raz

### Przygotuj przed zajęciami
- na swoim komputerze otwórz i przetestuj `projekty/01-Lowca-Skarbow-FINAL.sb3`; trzymaj gotowy także `projekty/01-Lowca-Skarbow-START.sb3`, ale nie wysyłaj żadnego pliku przed połączeniem
- przygotuj komunikator z możliwością przełączania udostępniania całego ekranu oraz ustalony kanał przesyłania plików
- na początku dziecko udostępnia cały ekran i uruchamia własny edytor Scratch; przy lekcji 1 nie uruchamia Minecrafta
- po kontroli technicznej udostępniasz swój ekran i pokazujesz FINAL; dziecko ogląda grę, ale nie otrzymuje tego pliku ani nie widzi gotowego kodu
- dopiero po zakończeniu pokazu FINAL dziecku wyślij tylko `projekty/01-Lowca-Skarbow-START.sb3`; dziecko ponownie udostępnia ekran, zapisuje plik i otwiera go samodzielnie
- FINAL wolno wysłać dziecku dopiero pod koniec jako materiał awaryjny, jeżeli z powodu czasu, awarii lub innej losowej sytuacji nie uda się doprowadzić START do działającej wersji
- w obu plikach sprawdź cały start: zielona flaga, kliknięcie przycisku na scenie, pytanie agenta o imię, powitanie i bezpośrednie przejście do planszy gry
- sprawdź sterowanie strzałkami, licznik `punkty`, trzy zwykłe nagrody po 1 punkcie, rzadką gwiazdę za 3 punkty, dźwięk `Coin`, napis `+3`, TNT i ekran końca gry
- zapamiętaj mapę duszków: `postac minecraft2` to gracz; `nagroda 1`, `nagroda 2` i `nagroda 3` to zwykłe skarby; `nagroda rzadka` to mała gwiazda +3; `popup +3` pokazuje informację o bonusie; `tnt` jest przeszkodą
- sprawdź regułę rzadkości: gwiazda czeka losowo 12-18 sekund, pojawia się tylko na 2 sekundy i dopiero wtedy ma lokalną zmienną `aktywna` równą 1
- START ma być kopią nowego FINAL z jednym wyciętym fragmentem: wnętrzem warunku w skrypcie kolizji duszka `nagroda rzadka`; pozostaw warunek `(aktywna = 1) i (dotyka postac minecraft2)` oraz komentarz `TU PRACUJE UCZEŃ`
- w START pozostaw gotowy duszek `popup +3` reagujący na komunikat `rzadka +3`; dziecko programuje wysłanie komunikatu, a nie samą animację napisu
- zachowaj czystą kopię START na wypadek usunięcia duszka lub gotowego skryptu; częściową pracę dziecka i ewentualnie przesłany FINAL zapisujcie pod różnymi nazwami

### Zadanie domowe
- wymyśl drugą rzadką nagrodę: narysuj ją, nadaj jej wartość i zapisz, jak długo gracz powinien czekać na jej pojawienie oraz jaki komunikat ma uruchamiać

## [intro] Połączenie i przygotowanie ekranu dziecka (5 min)

### Co robić teraz
- Przywitaj dziecko i poproś, aby udostępniło cały ekran. Sprawdź, czy widzisz pulpit, kursor oraz komunikaty systemowe, a nie tylko okno rozmowy.
- Dziecko uruchamia własny edytor Scratch. Jeśli korzysta z wersji przeglądarkowej, otwiera pusty edytor; jeśli z aplikacji, uruchamia Scratch Desktop. Na tym etapie nie wysyłaj jeszcze żadnego projektu.
- Poproś dziecko o kliknięcie w dowolne miejsce i wpisanie jednej cyfry w pustym polu, aby potwierdzić działanie myszy oraz klawiatury. Ustalcie także, gdzie po pobraniu znajdzie plik `.sb3`.
- Zapytaj krótko o doświadczenie ze Scratchem. Jeśli dziecko zna środowisko, prosi się je o wskazanie sceny, listy duszków i zielonej flagi. Jeśli nie zna, zapowiedz, że pokażecie te miejsca po otwarciu START.
- [mów] Za chwilę pokażę Ci na moim ekranie gotową grę. Nie dostaniesz jeszcze jej pliku, bo później uruchomisz własną wersję z jedną brakującą supermocą i sam ją zaprogramujesz.
- Dziecko zatrzymuje udostępnianie, a prowadzący udostępnia swój ekran z przygotowanym FINAL. Nie pokazuj karty Kod.

### Wskazówki
- [podpowiedź] Jeżeli komunikator pozwala udostępnić tylko jedno okno, upewnij się przed wysłaniem START, że dziecko potrafi przełączyć się między rozmową, pobieraniem i edytorem Scratch.
- [podpowiedź] Gdy dziecko pyta „Czy zrobimy całą grę?”, odpowiedz: „Silnik już działa, a Ty połączysz dwa duszki własną zasadą — gwiazdę i napis +3. Tak rozwija się większe gry”.
- [tempo] Kontrola techniczna nie powinna zająć całego spotkania. Najpóźniej w 4. minucie przełącz udostępnianie na ekran instruktora.
- [błąd] Nie wysyłaj teraz ani START, ani FINAL. Wcześniejsze wysłanie pliku odciąga uwagę od pokazu i może ujawnić gotowe rozwiązanie.

## [demo] FINAL wyłącznie na ekranie instruktora (7 min)

### Co robić teraz
- Na udostępnionym ekranie instruktora uruchom FINAL: zielona flaga, przycisk start, wpisanie krótkiego pseudonimu i przejście po powitaniu agenta do planszy gry.
- Instruktor steruje postacią na swoim komputerze, a dziecko kieruje próbą głosem: wybiera, w którą stronę iść, oraz nazywa elementy widoczne na scenie.
- Zbierz zwykłą nagrodę. Dziecko odczytuje wynik przed i po zebraniu i zauważa wzrost o 1 punkt.
- Poczekaj na małą gwiazdę. Pojawia się losowo po 12-18 sekundach i pozostaje na scenie przez 2 sekundy. Zbierz ją, a dziecko nazywa cztery sygnały: +3 punkty, `Coin`, zniknięcie gwiazdy i `popup +3` widoczny około 0.7 sekundy.
- Pokaż również dotknięcie TNT i ekran końca gry, ale nie otwieraj karty Kod ani listy gotowych bloków.
- [mów] Za chwilę otworzymy wersję START. Gwiazda nadal będzie się pojawiać, ale po dotknięciu nie doda punktów, nie zniknie i nie uruchomi napisu. Ty zaprogramujesz całą tę reakcję.
- Zakończ pokaz, zatrzymaj własne udostępnianie i dopiero teraz wyślij dziecku `01-Lowca-Skarbow-START.sb3`. Poproś o ponowne udostępnienie całego ekranu.

### Wskazówki
- [podpowiedź] Gdy dziecko pyta „Skąd przedmiot wie, że go dotknąłem?”, nie podawaj od razu odpowiedzi. Zaproponuj: „Poszukajmy za chwilę w skrypcie tego przedmiotu”.
- [podpowiedź] Gdy dziecko pyta „Dlaczego gwiazda tak rzadko się pojawia?”, odpowiedz: „Jej gotowy skrypt losuje czas czekania od 12 do 18 sekund, a potem daje graczowi tylko 2 sekundy na reakcję”.
- [podpowiedź] Gdy pyta „Skąd bierze się napis +3?”, odpowiedz: „To osobny duszek. Gwiazda wysyła do niego komunikat, tak jak krótkie hasło przez krótkofalówkę”.
- [podpowiedź] Gdy pyta „Czy mogę mieć 100 punktów?”, odpowiedz: „Technicznie tak. W wyzwaniu sprawdzimy, czy byłoby to dobre dla gracza”.
- [tempo] Wystarczy jedna krótka próba na ekranie instruktora. Najpóźniej w 12. minucie wyślij START i wróć do udostępnionego ekranu dziecka.
- [błąd] Nie pomyl małej gwiazdy z trzema zwykłymi nagrodami. Zwykłe skarby celowo dają po 1 punkcie; tylko `nagroda rzadka` daje 3 i uruchamia `popup +3`.
- [błąd] Nie wysyłaj FINAL razem ze START. Dziecko otrzymuje teraz wyłącznie plik z brakującą reakcją.

## [concept] Który duszek i który skrypt? (6 min)

### Co robić teraz
- Na udostępnionym ekranie dziecko odbiera START, zapisuje go w znanym katalogu i samodzielnie otwiera w Scratchu przez `Plik → Wczytaj ze swojego komputera` albo bezpośrednio w aplikacji Scratch Desktop.
- Dziecko uruchamia START: klika zieloną flagę, przycisk start, wpisuje pseudonim i czeka na rzadką gwiazdę. Zwykłe skarby nadal działają, ale dotknięcie gwiazdy nie daje bonusu, dźwięku, zniknięcia ani napisu `+3`.
- [mów] To nie jest awaria pliku. Właśnie znaleźliśmy brakującą funkcję: pojawianie gwiazdy jest gotowe, ale musimy zaprogramować, co stanie się po jej zebraniu.
- Zatrzymaj projekt czerwonym przyciskiem. Na liście duszków dziecko wybiera `nagroda rzadka` i sprawdza kostium `rzadka gwiazda`. `nagroda 1`, `nagroda 2` i `nagroda 3` służą teraz tylko jako działające przykłady.
- W karcie Kod znajdźcie skrypt pojawiania zaczynający się od `kiedy otrzymam [gra]`: ukrywa duszka, ustawia `aktywna` na 0, czeka losowo 12-18 sekund, losuje pozycję, ustawia `aktywna` na 1, pokazuje gwiazdę na 2 sekundy i znowu ją wyłącza. Tego skryptu nie zmieniajcie podczas zadania podstawowego.
- W drugim skrypcie zaczynającym się od `kiedy otrzymam [gra]` znajdźcie pętlę `zawsze` i gotowy warunek: `(aktywna = 1) i (dotyka [postac minecraft2])`. Komentarz `TU PRACUJE UCZEŃ` wskazuje puste wnętrze tego warunku.
- Dziecko odczytuje warunek własnymi słowami: „Zbierz gwiazdę tylko wtedy, gdy jest włączona i dotyka gracza”. Następnie przewiduje kolejność skutków: wyłącz aktywność, dodaj 3 punkty, zagraj `Coin`, nadaj `rzadka +3`, ukryj i poczekaj 0.25 sekundy.

### Materiały
- Zakres pracy dziecka: sześć bloczków wewnątrz gotowego warunku — blokada `aktywna`, punkty, dźwięk, komunikat, ukrycie i pauza.
- Gotowe w START: zdarzenie `gra`, pętla `zawsze`, warunek dotknięcia oraz cały duszek `popup +3`.
- [kod] Duszek „nagroda rzadka” — skrypt zebrania po komunikacie „gra” | Scratch:
```text
MIEJSCE: duszek „nagroda rzadka” → drugi skrypt „kiedy otrzymam gra” → wnętrze gotowego warunku
kiedy otrzymam [gra v]
zawsze
  jeżeli <<(aktywna) = (1)> i <dotyka [postac minecraft2 v]?>> to
    ustaw [aktywna v] na (0)
    zmień [punkty v] o (3)
    zagraj dźwięk [Coin v]
    nadaj [rzadka +3 v]
    ukryj
    czekaj (0.25) sekundy
```

### Wskazówki
- [podpowiedź] Jeżeli dziecko próbuje ponownie otworzyć FINAL widziany na ekranie instruktora, przypomnij, że nie otrzymało tego pliku. Cała praca odbywa się teraz w jego własnym START.
- [podpowiedź] Jeżeli plik otworzył się w nowej karcie lub aplikacji, upewnij się, że udostępnianie nadal pokazuje właściwe okno. Prowadzący musi widzieć scenę, listę duszków i obszar kodu.
- [podpowiedź] `zawsze` oznacza ciągłe sprawdzanie. Operator `i` wymaga dwóch prawdziwych odpowiedzi jednocześnie: gwiazda jest aktywna oraz dotyka gracza.
- [podpowiedź] `aktywna` jest zmienną „tylko dla tego duszka”. Wartość 0 oznacza „nie można zebrać”, a 1 — „gwiazda jest gotowa do zebrania”.
- [podpowiedź] Gdy dziecko pyta „Dlaczego nie programujemy gracza?”, odpowiedz: „To nagroda sprawdza dotknięcie i decyduje, co wtedy zrobić. W Scratchu zachowanie zwykle umieszczamy w obiekcie, którego ono dotyczy”.
- [podpowiedź] Gdy pyta „Dlaczego są dwa skrypty z komunikatem `gra`?”, wyjaśnij, że działają równolegle: jeden steruje pojawianiem, a drugi bez przerwy pilnuje zebrania.
- [dla szybszych] Dziecko może otworzyć `nagroda 3`, porównać jej skrypt kolizji i wskazać różnice: zwykła nagroda daje 1 punkt i nie wysyła komunikatu do popupu.
- [błąd] Nie dodawaj bloków do górnego skryptu losującego pozycję. Zadanie podstawowe wykonujemy wyłącznie wewnątrz pustego `jeżeli` w dolnym skrypcie.

## [guided] Programujemy reakcję rzadkiej gwiazdy (15 min)

### Co robić teraz
- Całe programowanie odbywa się na udostępnionym ekranie dziecka. Dziecko obsługuje mysz i klawiaturę; prowadzący wskazuje kolejny cel głosem i zadaje pytania.
- Dziecko otwiera kategorię Zmienne i jako pierwszy blok wewnątrz warunku dodaje `ustaw [aktywna] na (0)`. Wyjaśnij, że natychmiast zamyka to „bramkę” zebrania, zanim wykonają się efekty.
- Pod spodem umieszcza `zmień [punkty] o (3)`. Upewnij się, że wybrało globalną zmienną `punkty`, a nie lokalną `aktywna` ani `moja zmienna`.
- Z kategorii Dźwięk dodaje `zagraj dźwięk [Coin]`, a ze Zdarzeń — `nadaj [rzadka +3]`. Pokaż na chwilę duszka `popup +3`: jego gotowy skrypt czeka na ten komunikat, wychodzi na wierzch, ustawia pozycję `(0, 130)`, rozmiar 80%, pokazuje się na 0.7 sekundy i znika.
- Dziecko wraca do `nagroda rzadka`, z kategorii Wygląd dodaje `ukryj`, a z Kontroli `czekaj (0.25) sekundy`.
- Do pierwszego testu tymczasowo zmień w skrypcie pojawiania losowanie `12 do 18` na `1 do 2`, aby nie czekać kilkanaście sekund. Po teście przywróć `12 do 18`.
- Dziecko uruchamia projekt i sprawdza dwa wyniki osobno: licznik rośnie o 3 oraz pojawia się `popup +3`. Po udanym teście czyta sześć dodanych bloków od góry i opisuje ich kolejność.

### Wskazówki
- [podpowiedź] Przy problemie najpierw poproś dziecko o powiększenie obszaru kodu albo przesunięcie skryptu w widoczne miejsce. Nie przejmuj zdalnie myszy tylko dlatego, że przeciąganie trwa dłużej.
- [podpowiedź] Kolory kategorii pomagają szukać: Zmienne są pomarańczowe, Dźwięk różowy, Wygląd fioletowy, a Kontrola pomarańczowo-żółta.
- [podpowiedź] Gdy dziecko nie może włożyć bloku do warunku, poproś, by przesuwało go powoli, aż wnętrze `jeżeli` podświetli się białą linią.
- [podpowiedź] Gdy pyta o `zmień punkty o 3` i `ustaw punkty na 3`, pokaż różnicę na przykładzie wyniku 5: pierwszy blok daje 8, drugi kasuje poprzedni wynik i daje 3.
- [podpowiedź] `nadaj [rzadka +3]` nie pokazuje napisu samodzielnie. Wysyła wiadomość; dopiero osobny duszek `popup +3` reaguje na nią swoim skryptem.
- [podpowiedź] Gdy pyta, czy kolejność ma znaczenie, odpowiedz: „Tak. Najpierw blokujemy drugie zebranie, potem przyznajemy nagrodę i uruchamiamy efekty, a na końcu ukrywamy gwiazdę”.
- [podpowiedź] Jeżeli dziecko zna Scratcha, podaj tylko listę sześciu skutków i pozwól mu samodzielnie dobrać bloki; nazwę kategorii zdradzaj dopiero po chwili szukania.
- [błąd] Wszystkie sześć bloków ma być wewnątrz `jeżeli`. Poza warunkiem wynik rósłby bez zebrania gwiazdy.
- [błąd] Pierwszy blok ustawia lokalną `aktywna` na 0. Nie używaj tu `zmień aktywna o 0`, bo taki blok niczego nie zmieni.
- [błąd] Po teście przywróć losowanie czasu pojawienia z `1 do 2` na `12 do 18`; inaczej nagroda przestanie być rzadka.
- [gdy brakuje czasu] Minimum to: `ustaw aktywna na 0`, `zmień punkty o 3`, `nadaj rzadka +3` i `ukryj`. Dźwięk oraz pauzę dodaj po działającym teście rdzenia.

## [challenge] Dziecko projektuje rzadkość i efekt WOW (10 min)

### Co robić teraz
- [mów] Kod już działa. Teraz to Ty jesteś projektantem gry: zdecyduj, jak rzadka i jak trudna do złapania ma być gwiazda, ale zachowaj nagrodę +3.
- Dziecko wybiera jeden z trzech przedziałów czekania: `8 do 12` sekund — częsta, `12 do 18` — zbalansowana, `18 do 25` — bardzo rzadka. Zmienia obie liczby w bloku `losuj od ... do ...` i uzasadnia wybór.
- Następnie wybiera czas widoczności od 1.5 do 3 sekund. Przed testem przewiduje: krótszy czas zwiększa trudność złapania, ale nie zmienia wartości punktowej.
- W duszku `popup +3` dziecko personalizuje jedną cechę bez przebudowywania efektu: rozmiar 60-120%, pozycję `y`, czas widoczności 0.4-1.2 sekundy albo wygląd kostiumu `+3 punkty`.
- Do szybkich prób można ponownie użyć czekania `1 do 2`, lecz ostatni test i zapis muszą zawierać wybrany docelowy przedział.

### Wskazówki
- [podpowiedź] Gdy dziecko pyta „Co znaczy `losuj od 12 do 18`?”, odpowiedz: „Scratch przy każdym cyklu wybiera inną liczbę sekund z tego zakresu, dlatego nie da się dokładnie przewidzieć momentu pojawienia”.
- [podpowiedź] Gdy pyta „Czy mogę dać 5 punktów?”, wyjaśnij, że można, ale trzeba wtedy zmienić także grafikę `popup +3`, aby informacja na ekranie zgadzała się z wynikiem.
- [podpowiedź] Gdy pyta „Czy mogę dać punkty ujemne?”, odpowiedz: „Tak, liczba może być ujemna, ale wtedy ten przedmiot staje się pułapką, a nie nagrodą”.
- [podpowiedź] Pierwsze `czekaj losuj od ... do ...` ustala przerwę przed pojawieniem. Drugie `czekaj 2 sekundy`, po `pokaż`, ustala okno na złapanie. Nie pomyl tych dwóch parametrów.
- [dla szybszych] W `popup +3` dodaj prostą animację: ustaw rozmiar na 60%, pokaż, powtórz 4 razy `zmień rozmiar o 10`, a potem odczekaj i ukryj.
- [dla szybszych] Dziecko może zmienić gwiazdę na nagrodę +5, ale wtedy aktualizuje razem: blok punktów, kostium popupu oraz nazwę komunikatu po obu stronach połączenia.
- [błąd] Zmieniaj jedną cechę naraz i po każdej zmianie testuj. Inaczej trudno ustalić, która zmiana zepsuła reakcję.
- [gdy brakuje czasu] Pozostaw wartości 12-18 sekund i 2 sekundy. Dziecko personalizuje tylko rozmiar albo czas wyświetlania `popup +3`.

## [demo] Test całej gry i diagnoza błędów (10 min)

### Co robić teraz
- Zatrzymaj projekt, kliknij zieloną flagę i ponownie przycisk start. Sprawdźcie, czy wynik zaczyna się od 0 i czy po powitaniu agenta pojawia się gracz oraz nagrody.
- Dziecko bez pomocy zbiera najpierw zwykłą nagrodę za 1 punkt, a potem rzadką gwiazdę. Przed dotknięciem gwiazdy mówi aktualny wynik i przewiduje wynik po zebraniu.
- Sprawdźcie sześć kryteriów: `aktywna` natychmiast zmienia się na 0, wynik rośnie o 3, `Coin` słychać raz, pojawia się `popup +3`, gwiazda znika, a jeden kontakt nie nalicza punktów wielokrotnie.
- Jeśli test nie przechodzi, dziecko najpierw wskazuje konkretny zawiedziony skutek. Potem czyta stos blok po bloku i wraca tylko do miejsca powiązanego z tym skutkiem.
- Jeżeli wszystkie kryteria przechodzą, nazwijcie wprost rezultat: START dziecka zawiera już kompletną reakcję rzadkiej gwiazdy pokazaną wcześniej w FINAL, a dodatkowo ma jego własne ustawienie rzadkości lub popupu.
- Zapisz projekt jako osobny plik, na przykład `Lowca-Skarbow-Ola.sb3`, nie nadpisując czystej kopii START.

### Wskazówki
- [podpowiedź] Brak punktów: sprawdź zmienną `punkty`, wartość 3 i położenie wewnątrz warunku. Brak dźwięku: sprawdź `Coin` oraz głośność systemu. Brak napisu: porównaj dokładną nazwę `rzadka +3` w bloku `nadaj` i w zdarzeniu duszka `popup +3`.
- [podpowiedź] Jeżeli po zielonej fladze nadal widać ekran startowy, wszystko działa prawidłowo — trzeba kliknąć przycisk start na scenie.
- [podpowiedź] Jeżeli gwiazda nie pojawia się od razu, jest to oczekiwane: przy ustawieniu domyślnym trzeba czekać 12-18 sekund. Do diagnozy można chwilowo wrócić do `1 do 2`.
- [błąd] Jeżeli wynik rośnie kilka razy, sprawdź, czy `ustaw aktywna na 0` jest pierwszym blokiem reakcji, a nie jej końcem.
- [błąd] Nie naprawiaj przy okazji zwykłych nagród ani TNT. Test dotyczy tylko współpracy `nagroda rzadka` z `popup +3`.
- [tempo] Stabilną wersję zapisz najpóźniej w 53. minucie, aby zostało miejsce na samodzielny pokaz dziecka.

## [summary] Pokaz i rozmowa z rodzicem (7 min)

### Co robić teraz
- Jeśli START jest ukończony, dziecko na swoim udostępnionym ekranie uruchamia własną wersję, zbiera rzadką gwiazdę i pokazuje oba współpracujące duszki: `nagroda rzadka` oraz `popup +3`. FINAL nadal pozostaje tylko u instruktora.
- Poproś dziecko o dokończenie trzech zdań: „Programowałem w duszku...”, „Warunek sprawdza...”, „Po dotknięciu dzieje się...”. Dziecko wskazuje sześć dodanych bloków i jedną własną decyzję projektową.
- [mów] Zaprogramowałeś pełną reakcję rzadkiego obiektu: zabezpieczyłeś jedno zebranie, zmieniłeś wynik, dodałeś dźwięk i przez komunikat uruchomiłeś drugi duszek. Twój START ma już funkcję, którą na początku widziałeś w FINAL.
- Jeżeli START nie jest ukończony z powodu braku czasu, awarii lub innej losowej sytuacji, najpierw zapisz częściową pracę dziecka pod nazwą zawierającą jego imię i dopisek `NIEDOKONCZONE`. Wskaż konkretnie, które elementy już działają.
- Dopiero po zapisaniu częściowej pracy wyślij dziecku `01-Lowca-Skarbow-FINAL.sb3`. Dziecko zapisuje go osobno, otwiera i — jeśli czas pozwala — uruchamia na swoim komputerze, aby zobaczyć kompletną wersję. FINAL nie może nadpisać projektu dziecka.
- [mów] To jest pełna wersja pomocnicza do uruchamiania po zajęciach. Zachowaj też swój projekt, bo w nim widać dokładnie to, co udało Ci się dzisiaj zaprogramować.
- Jeśli rodzica nie ma przy podsumowaniu, dziecko wykonuje pokaz prowadzącemu. Na końcu przypomnij nazwy zapisanych plików i zadanie domowe.

### Wskazówki
- [podpowiedź] Gdy dziecko nie pamięta pojęcia, pytaj o działanie: „Który klocek zadaje pytanie o dotknięcie?” zamiast wymagać definicji słowa „warunek”.
- [podpowiedź] Gdy rodzic pyta, co dziecko zrobiło samodzielnie, wskaż konkretnie: odnalazło właściwy duszek, ułożyło reakcję z sześciu bloków, połączyło dwa duszki komunikatem, testowało i dobrało rzadkość nagrody.
- [podpowiedź] Jeżeli użyto wariantu awaryjnego, rozdziel wyraźnie własną pracę dziecka od przesłanego FINAL. Nie przedstawiaj uruchomionego FINAL jako ukończonego projektu ucznia.
- [błąd] Nie wysyłaj FINAL przy pierwszym problemie z blokiem. Jest przeznaczony wyłącznie do domknięcia zajęć, gdy mimo pomocy nie da się ukończyć START w dostępnym czasie.
- [tempo] Od 55. minuty nie dodawaj nowych funkcji. Priorytetem jest zapisany projekt i spokojny pokaz działającej wersji.
