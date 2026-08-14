# Warunki i operatory logiczne — gra podejmuje decyzje
Subject: Scratch
Level: Poziom 2 — 10-12 lat
Czas: 95 min
Tags: Warunki, Operatory, Logika, Zasady gry
Opis: Gra przestaje wykonywać polecenia po kolei i zaczyna decydować. Dzieci poznają jeżeli, jeżeli-w przeciwnym razie, porównania oraz operatory i, lub, nie.
Cel: Dziecko zapisuje zasady gry jako warunki, buduje wyrażenia logiczne z porównań i rozumie różnicę między jeżeli a jeżeli-w przeciwnym razie.

### Po zajęciach dziecko potrafi
- zbudować warunek z porównaniem liczb i użyć go w pętli gry
- odróżnić jeżeli od jeżeli-w przeciwnym razie i wskazać, kiedy potrzebne jest drugie
- połączyć dwa warunki operatorem i oraz lub
- zamienić zasadę gry opisaną słowami na warunek w kodzie

### Przygotuj przed zajęciami
- gotowe demo: gra kończąca się przy zerze życia i przy zerze czasu, z różnymi komunikatami
- przygotowane trzy zasady gry opisane słowami, do przetłumaczenia na warunki na żywo
- projekty dzieci z lekcji 4
- kartka i długopis dla każdego dziecka

### Zadanie domowe
- zapisz słowami trzy zasady swojej gry i sprawdź, czy każda da się zapisać jednym warunkiem

## [intro] Powitanie i demo decyzji (8 min)

### Co robić teraz
- [mów] Cześć. Wasza gra pamięta punkty i życie, ale jest kompletnie obojętna. Możecie stracić całe życie i nic się nie stanie.
- **Sprawdzenie obecności:** odczytaj listę uczestników i zaznacz obecnych.
- **DEMO:** zagraj tak, żeby stracić życie do zera. Gra kończy się komunikatem o przegranej. Potem zagraj tak, żeby skończył się czas — inny komunikat.
- [mów] Ta sama gra, dwa różne zakończenia. Program sam sprawdził, co się stało, i sam wybrał, co zrobić.
- [mów] Do tej pory pisaliście przepisy: zrób to, potem to, potem to. Dziś zaczniecie pisać zasady: **jeżeli** stanie się to, **to** zrób tamto.
- Zapytaj grupę: wymieńcie zasadę z dowolnej gry, w którą gracie. Zbierz trzy i zapisz na udostępnionym ekranie.

### Wskazówki
- [tempo] Zapisanie zasad z prawdziwych gier na starcie daje materiał, do którego można wracać przez całą lekcję. Zostaw je widoczne.
- [podpowiedź] Sformułowanie „przepis kontra zasada” dobrze trafia do tej grupy wiekowej i wraca przy każdym warunku.

## [review] Otwieramy projekt i przypominamy zmienne (5 min)

### Co robić teraz
- Wszyscy otwierają projekt z lekcji 4. Poczekaj na potwierdzenie od każdego dziecka.
- Uruchomcie. Sprawdźcie, czy wynik zeruje się na starcie i czy zegar odlicza.
- Szybka powtórka: czym różni się ustaw od zmień? Dlaczego zmienne zerujemy? Co się stanie, jeśli zapomnicie?
- Kto zrobił zadanie domowe z czwartą zmienną, wpisuje jej nazwę na czat.

### Wskazówki
- [błąd] U kogoś wynik nadal nie zeruje się na starcie. To najlepszy możliwy moment, żeby to naprawić razem — z zeszłotygodniowym błędem świeżo w pamięci.
- [gdy nie zdążysz] Sam start projektu.

## [concept] Jeżeli, czyli decyzja (14 min)

### Co robić teraz
- [mów] Znacie już bloczek jeżeli z pierwszej lekcji — używaliśmy go do sprawdzania klawiszy. Dziś zobaczycie, co jeszcze można w niego wstawić.
- Pokaż kategorię Wyrażenia i trzy bloczki porównania: mniejsze niż, równa się, większe niż. Wszystkie mają sześciokątny kształt.
- [mów] Sześciokąt znaczy: to jest odpowiedź prawda albo fałsz. Nic pomiędzy. I tylko takie rzeczy wchodzą do dziurki w bloczku jeżeli.
- Zbudujcie na żywo: przeciągnijcie zmienną życie do lewej strony bloczka porównania, w prawą wpiszcie 1. Kliknijcie sam bloczek — Scratch pokaże prawda albo fałsz.
- [mów] Wypróbujcie to. Zmieńcie życie na 0 i kliknijcie ponownie. Odpowiedź się zmienia.
- Krok 1: na Scenie zbudujcie: kiedy kliknięto zieloną flagę, pętla zawsze, w niej jeżeli życie mniejsze niż 1 to, a w środku powiedz „Koniec gry” oraz zatrzymaj wszystko.
- Krok 2: uruchomcie i klikajcie przeciwnika, aż stracicie życie. Gra kończy się sama. Wpiszcie na czat G albo znak zapytania.
- [mów] Uwaga na coś ważnego: dlaczego mniejsze niż 1, a nie równa się 0? Bo jeśli coś odejmie dwa życia naraz, warunek „równa się zero” nigdy się nie spełni i gra nie skończy się nigdy.
- [mów] To jest sposób myślenia, którego chcę was nauczyć: nie pytajcie „czy jest dokładnie tak”, tylko „czy już przekroczyliśmy granicę”.

### Wskazówki
- [podpowiedź] Uwaga o „mniejsze niż” zamiast „równa się” jest najcenniejszą rzeczą w tej lekcji. Ten błąd potrafi zniszczyć projekt, a jest praktycznie niewidoczny przy testowaniu.
- [błąd] Warunek sprawdzany raz zamiast w pętli — bloczek jeżeli musi być w środku pętli zawsze, inaczej program sprawdzi go w chwili startu i nigdy więcej.
- [błąd] Bloczek porównania nie wchodzi do bloczka jeżeli, bo dziecko nie trafia w sześciokątną dziurkę.
- [błąd] Zmienna wstawiona do prawej strony porównania zamiast do lewej. Działa, ale znaczy co innego. Warto pokazać.
- [błąd] Bloczek powiedz na Scenie nie istnieje — Scena nie ma dymków. Zbuduj ten skrypt na duszku albo użyj zmiany tła.
- [dla szybszych] Niech zbudują warunek sprawdzający, czy wynik przekroczył 10, i wyświetlą gratulacje.

### Materiały
- [kod] Koniec gry przy zerze życia | scratch:
  ```
  kiedy kliknięto zieloną flagę
    zawsze
      jeżeli <(życie) < (1)> to
        powiedz [Koniec gry!] przez (2) sek
        zatrzymaj [wszystko]
  ```

## [guided] Jeżeli-w przeciwnym razie i dwa zakończenia (18 min)

### Co robić teraz
- [mów] Macie jedno zakończenie. Teraz zrobimy dwa i przy okazji poznacie drugi kształt warunku.
- Krok 1: pokaż bloczek jeżeli-w przeciwnym razie. Ma dwie kieszenie zamiast jednej.
- [mów] Różnica jest prosta i ważna. Zwykłe jeżeli robi coś albo nie robi nic. Jeżeli-w przeciwnym razie zawsze robi jedną z dwóch rzeczy. Nigdy obie i nigdy żadnej.
- Krok 2: zamieńcie odliczanie czasu z lekcji 4 na wersję z warunkiem. Zamiast powtórz 60 wstawcie pętlę zawsze, a w niej: czekaj 1 sek, zmień czas o -1, a pod tym jeżeli czas mniejszy niż 1 to zatrzymaj wszystko.
- [mów] Zauważcie, co właśnie zniknęło: liczba 60 w dwóch miejscach. Teraz wystarczy zmienić wartość startową i wszystko się zgadza. To jest realna korzyść z warunku, nie ozdobnik.
- Krok 3: sprawdźcie. Zmieńcie czas startowy na 20 i uruchomcie. Gra kończy się po dwudziestu sekundach bez żadnej innej poprawki.
- Krok 4: teraz dwa zakończenia. Zbudujcie na duszku gracza skrypt: kiedy kliknięto zieloną flagę, pętla zawsze, w niej jeżeli życie mniejsze niż 1 to powiedz „Przegrana!”, zatrzymaj wszystko.
- Krok 5: drugi warunek, obok: jeżeli czas mniejszy niż 1 to jeżeli-w przeciwnym razie sprawdzające wynik. Gdy wynik większy niż 10 — „Wygrana!”. W przeciwnym razie — „Za mało punktów”.
- Krok 6: przetestujcie oba zakończenia. Ustawcie czas startowy na 10, żeby nie czekać minuty.
- [mów] Ustawianie krótkiego czasu do testowania to normalna praktyka. Nikt nie testuje gry, grając w nią uczciwie.

### Wskazówki
- [podpowiedź] Wskazówka o skracaniu czasu do testów jest ważniejsza, niż wygląda. Dzieci potrafią dwadzieścia razy czekać pełną minutę, żeby sprawdzić zakończenie.
- [błąd] Warunek zadziałał wiele razy i komunikat pojawia się w kółko — brakuje bloczka zatrzymaj wszystko albo jest poza warunkiem.
- [błąd] Dziecko użyło dwóch osobnych bloczków jeżeli tam, gdzie potrzebne jest jeżeli-w przeciwnym razie. Efekt: przy pewnych wartościach nie dzieje się nic albo dzieje się jedno i drugie. Bardzo dobra okazja, żeby pokazać różnicę na konkretnym przypadku.
- [błąd] Zatrzymaj wszystko zatrzymuje też komunikat, zanim zdąży się pokazać. Kolejność ma znaczenie: najpierw powiedz przez 2 sek, potem zatrzymanie.
- [dla szybszych] Niech dodadzą trzecie zakończenie: wynik powyżej 25 to „Rekord!”. Wymaga zagnieżdżenia warunku w warunku.
- [gdy nie zdążysz] Jedno zakończenie plus przerobienie odliczania na warunek. Drugie zakończenie wraca na lekcji 9.

### Materiały
- [kod] Odliczanie z warunkiem — skrypt na Scenie | scratch:
  ```
  kiedy kliknięto zieloną flagę
    ustaw [czas] na (60)
    zawsze
      czekaj (1) sek
      zmień [czas] o (-1)
      jeżeli <(czas) < (1)> to
        zatrzymaj [wszystko]
  ```
- [kod] Dwa zakończenia | scratch:
  ```
  kiedy kliknięto zieloną flagę
    zawsze
      jeżeli <(życie) < (1)> to
        powiedz [Przegrana!] przez (2) sek
        zatrzymaj [wszystko]
      jeżeli <(czas) < (1)> to
        jeżeli <(wynik) > (10)> to
          powiedz [Wygrana!] przez (2) sek
        w przeciwnym razie
          powiedz [Za mało punktów. Spróbuj jeszcze raz!] przez (2) sek
        zatrzymaj [wszystko]
  ```

## [break] Przerwa (5 min)

### Co robić teraz
- 5 minut przerwy. Dzieci wstają od ekranów.
- Zapowiedź: „Po przerwie połączymy warunki ze sobą. Zbudujecie zasady, których nie da się zapisać jednym sprawdzeniem.”

### Wskazówki
- [tempo] Warunki są najbardziej abstrakcyjną częścią pierwszego semestru. Przerwa w połowie jest tu obowiązkowa, nawet jeśli grupa jest rozpędzona.

## [concept] Operatory i, lub, nie (13 min)

### Co robić teraz
- [mów] Niektórych zasad nie da się zapisać jednym sprawdzeniem. „Wygrywasz, jeśli masz ponad dziesięć punktów **i** zostało ci życie” to dwa warunki naraz.
- Pokaż trzy bloczki w kategorii Wyrażenia: i, lub, nie. Wszystkie sześciokątne, z sześciokątnymi dziurkami.
- [mów] Do tych dziurek wkłada się inne warunki. Warunek w warunku — dokładnie jak pętla w pętli z zeszłego tygodnia.
- Zbudujcie na żywo i kliknijcie, żeby zobaczyć prawda albo fałsz.
- wynik większy niż 10 **i** życie większe niż 0 — prawda tylko wtedy, gdy oba są prawdziwe.
- wynik większy niż 10 **lub** czas mniejszy niż 5 — prawda, gdy chociaż jeden jest prawdziwy.
- **nie** klawisz spacja naciśnięty — odwraca odpowiedź.
- Ćwiczenie na czat. Podajesz zasadę słowami, dzieci piszą, którego operatora użyć.
- „Gracz wygrywa, gdy ma ponad dwadzieścia punktów i został mu czas.”
- „Gra się kończy, gdy życie spadnie do zera lub skończy się czas.”
- „Postać rusza się tylko wtedy, gdy nie jest zamrożona.”
- Krok końcowy: przerobcie warunek wygranej na wersję z operatorem i, żeby wygrana wymagała punktów **oraz** pozostałego życia.

### Wskazówki
- [podpowiedź] Ćwiczenie ze słów na operator jest lepsze niż budowanie kodu. Zrób pięć zdań, jeśli grupa łapie temat wolno — to najlepiej wykorzystany czas tej lekcji.
- [błąd] Dziecko wkłada do operatora zmienne zamiast warunków. Sześciokątna dziurka nie przyjmie owalu i to jest dobra podpowiedź samego Scratcha — pokaż ją.
- [błąd] Mylenie i z lub. Najlepszy sposób sprawdzenia: podaj konkretne liczby i policzcie razem, czy wychodzi prawda.
- [błąd] Podwójne zaprzeczenie w warunku z nie. Jeśli dziecko się w tym gubi, zaproponuj odwrócenie porównania zamiast negacji.
- [dla szybszych] Niech zbudują warunek z trzema składnikami, zagnieżdżając operator w operatorze, i sprawdzą, czy działa zgodnie z oczekiwaniem.
- [gdy nie zdążysz] Sam operator i, na jednym przykładzie. Lub oraz nie zostaw na lekcję 6.

### Materiały
- [kod] Warunek złożony | scratch:
  ```
  jeżeli <<(wynik) > (10)> i <(życie) > (0)>> to
    powiedz [Wygrana!] przez (2) sek
    zatrzymaj [wszystko]
  ```

## [guided] Zasady twojej gry (14 min)

### Co robić teraz
- [mów] Teraz przetłumaczycie zasady na kod. Najpierw kartka, potem bloczki.
- Krok 1: każde dziecko zapisuje na kartce trzy zasady swojej gry, zwykłymi zdaniami, zaczynając od słowa „jeżeli”. Trzy minuty.
- Przykłady do pokazania na ekranie, gdyby ktoś nie miał pomysłu.
- Jeżeli wynik przekroczy 15, moneta pojawia się szybciej.
- Jeżeli życie spadnie do jednego, gracz miga na czerwono.
- Jeżeli czas spadnie poniżej 10, zegar zmienia się na duży odczyt i gra przyspiesza.
- Krok 2: przy każdej zasadzie zapiszcie, jakiego porównania użyjecie i czy potrzebny jest operator.
- Krok 3: zbudujcie **jedną** z nich, tę, która wam się najbardziej podoba.
- Krok 4: przetestujcie, ustawiając zmienne tak, żeby warunek spełnił się od razu.
- Krok 5: kto skończy, buduje drugą.
- Chodź po grupie i pytaj po imieniu, którą zasadę wybrali.

### Wskazówki
- [podpowiedź] Zapisanie zasady słowami przed kodem to nie formalność. To jest umiejętność, którą ta lekcja ma wykształcić — kod jest tylko zapisem.
- [błąd] Zasada zapisana tak ogólnie, że nie da się jej sprawdzić, na przykład „jeżeli gra robi się trudna”. Pomóż doprecyzować do konkretnej liczby.
- [błąd] Warunek działa, ale wykonuje się kilkadziesiąt razy na sekundę i efekt miga. To temat, do którego wrócicie — na razie wystarczy dołożyć czekanie albo zmienną pilnującą, czy już zadziałało.
- [dla szybszych] Niech zbudują zasadę, która zadziała **tylko raz**, mimo że warunek jest prawdziwy przez cały czas. To realny problem i dobre wyzwanie.
- [gdy nie zdążysz] Jedna zasada, zapisana i zbudowana.

## [challenge] Test cudzej gry (8 min)

### Co robić teraz
- Zadanie w parach, na czacie. Każde dziecko opisuje jedną zasadę swojej gry słowami, ale **nie pokazuje kodu**.
- Drugie dziecko pisze, jak by ją zbudowało — jaki warunek, jakie porównanie, czy potrzebny operator.
- Potem porównujecie z tym, co jest w kodzie naprawdę.
- [mów] Bardzo często wyjdzie, że da się to zrobić inaczej i też dobrze. To normalne. W programowaniu rzadko jest jedno poprawne rozwiązanie.

### Wskazówki
- [tempo] To ćwiczenie wymaga sprawnego czatu i grupy, która już się zna. W pierwszym miesiącu może nie zadziałać — wtedy zrób je z całą grupą, a nie w parach.
- [dla szybszych] Niech znajdą przypadek, w którym ich zasada nie działa — na przykład skrajną wartość zmiennej.
- [gdy nie zdążysz] Zrób jedną zasadę wspólnie, na forum, zamiast w parach.
- [błąd] Dziecko podaje rozwiązanie inne niż w kodzie i uznaje, że się pomyliło. Podkreśl, że dwa różne poprawne rozwiązania to norma.

## [challenge] Pokaz prac i zapis (5 min)

### Co robić teraz
- Zapis: Plik, Zapisz teraz. Poczekaj na potwierdzenie od każdego dziecka.
- „Scena dla każdego”: po kolei, po imieniu, każde dziecko pokazuje zakończenie swojej gry i mówi, jaki warunek je wywołuje.
- Brawa po każdym pokazie.

### Wskazówki
- [podpowiedź] Pokazanie zakończenia wymaga zagrania — poproś, żeby dzieci ustawiły krótki czas przed pokazem.
- [tempo] Zapis przed pokazem, zawsze.

## [summary] Podsumowanie (5 min)

### Co robić teraz
- Zbierz grupę i zapytaj: kiedy potrzebne jest jeżeli-w przeciwnym razie zamiast zwykłego jeżeli? Dlaczego lepiej sprawdzać „mniejsze niż 1” niż „równa się 0”? Czym różni się i od lub?
- [mów] Dzisiaj wasza gra przestała być przepisem i stała się zbiorem zasad. To jest moment, w którym program zaczyna zachowywać się jak coś, co myśli — choć oczywiście nie myśli, tylko sprawdza.
- Zajawka: „Na następnych zajęciach zajmiemy się dotykaniem. Wasza postać wreszcie zauważy, że wpadła na przeciwnika — bez klikania.”
- Przypomnij zadanie domowe: trzy zasady zapisane słowami.

### Wskazówki
- [błąd] Warunek poza pętlą — sprawdza się raz, w chwili startu.
- [błąd] Porównanie na równość zamiast na przekroczenie granicy — warunek może nigdy się nie spełnić.
- [błąd] Dwa osobne jeżeli tam, gdzie potrzebne jest jeżeli-w przeciwnym razie.
- [błąd] Komunikat nie zdąży się pokazać, bo zatrzymaj wszystko jest przed nim.
- [błąd] Efekt miga, bo warunek jest prawdziwy przez cały czas i wykonuje się w kółko.
