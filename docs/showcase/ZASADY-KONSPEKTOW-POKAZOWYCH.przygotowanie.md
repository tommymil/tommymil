# Standard tworzenia kompleksowych konspektów lekcji pokazowych

Ten dokument opisuje sposób projektowania 60-minutowych lekcji pokazowych importowanych do Lesson Runnera. Ma umożliwić poprowadzenie tych samych zajęć przez osobę bardzo doświadczoną i przez instruktora, który widzi dany projekt po raz pierwszy.

Nazwa pliku kończy się na `.przygotowanie.md`, dlatego importer konspektów świadomie go pomija. Dokument jest instrukcją dla autorów, a nie osobną lekcją.

## Ustalony przebieg zdalnej lekcji pokazowej

Każdy konspekt pokazowy ma opisywać poniższy przebieg operacyjny. Nie zakładamy, że instruktor i dziecko pracują przy jednym komputerze.

### 1. Połączenie i ekran dziecka

- Instruktor łączy się z dzieckiem przez ustalone narzędzie do rozmowy.
- Dziecko włącza udostępnianie całego ekranu, nie tylko pojedynczego okna. Dzięki temu prowadzący widzi pobieranie pliku, wybór programu i ewentualne komunikaty systemowe.
- Dziecko uruchamia własne środowisko: Minecraft przy lekcji Minecraft albo edytor Scratch przy lekcji Scratch.
- Instruktor sprawdza, czy widać ekran, słychać rozmowę, działa mysz i klawiatura oraz czy dziecko umie wrócić do okna rozmowy.
- Na tym etapie dziecko nie otrzymuje ani START, ani FINAL. Najpierw potwierdzamy, że środowisko działa.

### 2. Pokaz FINAL na ekranie instruktora

- Dziecko wyłącza udostępnianie albo instruktor rozpoczyna własne udostępnianie zgodnie z możliwościami używanego komunikatora.
- Instruktor uruchamia na swoim komputerze projekt FINAL i pokazuje pełny efekt.
- Dziecko widzi grę, ale nie otrzymuje pliku FINAL i nie ma dostępu do jego kodu.
- Instruktor omawia cel gry, sterowanie, najważniejsze elementy i efekt, który później zaprogramuje dziecko.
- Już podczas FINAL można powiedzieć, czego będzie brakować w START, ale nie pokazujemy gotowego stosu bloków ani rozwiązania.
- Pokaz ma wzbudzić ciekawość i stworzyć jasny wzorzec wyniku: dziecko ma wiedzieć, do jakiego zachowania będzie dążyło.

### 3. Wysłanie i uruchomienie START

- Dopiero po zakończeniu omawiania FINAL instruktor wysyła dziecku projekt START.
- Instruktor kończy własne udostępnianie, a dziecko ponownie udostępnia cały ekran.
- Dziecko samodzielnie odbiera plik, zapisuje go, uruchamia we własnym środowisku i przechodzi standardowy start projektu.
- Nie przechodzimy od razu do kodu. Najpierw dziecko uruchamia START i doświadcza różnicy względem FINAL.
- Instruktor pomaga nazwać brakującą funkcję: co działa tak samo, czego brakuje i co będzie programowane podczas tych zajęć.
- Właściwe pytanie brzmi: „Co w START zachowuje się inaczej niż przed chwilą w FINAL?”.

### 4. Programowanie na ekranie dziecka

- Przez pozostałą część zajęć głównym ekranem jest ekran dziecka.
- Dziecko wybiera duszka lub funkcję, przeciąga bloki, wpisuje kod, uruchamia testy i zapisuje projekt.
- Instruktor prowadzi głosem i pytaniami. Nie przejmuje myszy, jeśli nie wymaga tego problem techniczny albo kończący się czas.
- Kod budujemy małymi fragmentami i po każdym fragmencie testujemy jeden obserwowalny efekt.
- Po ukończeniu zadania START powinien zawierać ten sam mechanizm, który został pokazany w FINAL, oraz bezpieczne personalizacje wybrane przez dziecko.
- Projekt dziecka zapisujemy pod nową nazwą. Nie nadpisujemy czystego START ani otrzymanego później FINAL.

### 5. Domknięcie udane

- Dziecko uruchamia swoją ukończoną wersję START i porównuje zachowanie z zapamiętanym FINAL.
- Dziecko pokazuje właściwy fragment kodu i wyjaśnia, co zrobiło.
- Instruktor podsumowuje konkretną umiejętność i zapisuje, które elementy dziecko wykonało samodzielnie.
- FINAL pozostaje u instruktora, jeśli projekt dziecka jest kompletny i stabilny.

### 6. Domknięcie awaryjne i przekazanie FINAL

Jeżeli nie udało się ukończyć START z powodu braku czasu, problemu technicznego, usunięcia ważnego fragmentu lub innej losowej sytuacji:

1. Najpierw zapisujemy częściowo wykonany projekt dziecka pod osobną nazwą.
2. Krótko nazywamy, co już działa i czego nie udało się dokończyć.
3. Instruktor wysyła dziecku FINAL dopiero teraz, jako materiał awaryjny do uruchomienia po zajęciach.
4. Dziecko zapisuje FINAL pod jego oryginalną nazwą, nie nadpisując swojej pracy.
5. Jeśli czas pozwala, dziecko uruchamia FINAL na własnym komputerze i porównuje brakujący efekt ze swoją wersją.
6. Nie przedstawiamy FINAL jako pracy wykonanej przez dziecko. Rozdzielamy „to udało Ci się zaprogramować” od „to jest kompletna wersja do dalszego oglądania i eksperymentowania”.

FINAL jest zabezpieczeniem dobrego zakończenia spotkania, a nie skrótem używanym w chwili pierwszej trudności. Nie wysyłamy go podczas normalnego debugowania, ponieważ dostęp do gotowego kodu odbiera dziecku odkrywanie rozwiązania.

### 7. Różnica między Scratch i Minecraft

Schemat zajęć jest ten sam, zmienia się tylko środowisko:

- Scratch: dziecko otwiera edytor Scratch i importuje `.sb3`;
- Minecraft Blocks/Python: dziecko uruchamia Minecraft oraz właściwe środowisko MakeCode i importuje `.mkcd`;
- konspekt musi podawać dokładną kolejność właściwą dla danego środowiska, zamiast używać ogólnego polecenia „uruchom projekt”.

### Obowiązkowe informacje w każdym konspekcie

W treści lekcji muszą być zapisane:

- moment pierwszego udostępnienia ekranu dziecka;
- moment przełączenia na ekran instruktora z FINAL;
- wyraźny zakaz wysyłania FINAL przed zakończeniem pracy;
- moment wysłania START;
- ponowne udostępnienie ekranu dziecka i uruchomienie START;
- test pokazujący brakującą funkcję;
- dalsza praca wyłącznie na kopii START dziecka;
- warunek ukończenia, po którym START odpowiada funkcjonalnie FINAL;
- procedura awaryjnego wysłania FINAL na końcu.

## 1. Co ma dawać dobry konspekt pokazowy

Konspekt nie jest tylko listą bloków do ułożenia. Powinien równocześnie:

- prowadzić instruktora przez rzeczywisty projekt START i FINAL;
- powiedzieć, w którym duszku, funkcji lub zdarzeniu odbywa się praca;
- wyjaśnić, co już działa, czego nie wolno przypadkiem przebudować i jaki fragment wykonuje dziecko;
- dawać dziecku szybki efekt „wow”, lecz pozostawiać mu realną sprawczość;
- nauczyć jednego czytelnego mechanizmu programistycznego;
- przewidywać pytania, pomyłki, szybsze tempo i brak czasu;
- zakończyć się działającym projektem, wypowiedzią dziecka i krótkim pokazem dla rodzica;
- być możliwy do wykonania w 60 minut, a nie tylko wyglądać dobrze na papierze.

Najważniejszy test jakości brzmi:

> Czy instruktor, który nie zna projektu, potrafi po jednokrotnym przeczytaniu wskazać właściwe miejsce w kodzie, wyjaśnić dziecku sens zadania, rozpoznać poprawny wynik i naprawić trzy najbardziej prawdopodobne błędy?

Jeśli nie, konspekt wymaga doprecyzowania.

## 2. Podstawa metodyczna

Przyjęty styl jest zgodny z materiałami Scratch Foundation:

- [Scratch Creative Learning Philosophy](https://www.scratchfoundation.org/learn/learning-library/scratch-creative-learning-philosophy) zaleca uczenie przez zabawę i eksperymentowanie, wiele możliwych dróg działania oraz model „low floors, wide walls, high ceilings”;
- ta sama filozofia opisuje spiralę twórczego uczenia: wyobrażanie, tworzenie, zabawę, dzielenie się, refleksję i rozpoczęcie kolejnej iteracji;
- [Debugging Strategies](https://resources.scratch.mit.edu/www/guides/en/ScratchLearningResource_DebuggingStrategies.pdf) zaleca czytanie kodu krok po kroku, dzielenie problemu na mniejsze części oraz cykl „dodaj mały fragment → przetestuj → dodaj kolejny”;
- [Reflection and Sharing Sheets](https://www.scratchfoundation.org/learn/learning-library/reflection-sharing-sheets) traktuje prezentację i refleksję jako część procesu uczenia, a nie dodatek po lekcji;
- oficjalny [Educator Guide](https://resources.scratch.mit.edu/www/guides/en/ImagineGuide.pdf) dla godzinnych warsztatów rezerwuje osobny czas na wprowadzenie, tworzenie oraz pokaz i refleksję;
- [Creative Learning Guide](https://cms.scratchfoundation.org/assets/8215d677-bad1-45bf-87bd-d574be4c00d9) proponuje prowadzącym pytania w rodzaju „Przejdź ze mną przez kod”, „Co program robi teraz?” i „Co chcesz, żeby zrobił?”.

Z tych źródeł wynikają praktyczne zasady dla naszych pokazówek:

1. Najpierw doświadczenie i ciekawość, potem nazwanie pojęcia.
2. Dziecko tworzy i testuje; prowadzący nie demonstruje całej lekcji własną myszą.
3. Zadanie ma łatwe minimum, kilka równorzędnych personalizacji i ambitne rozszerzenie.
4. Błąd jest materiałem do diagnozy, nie sygnałem do przejęcia kontroli.
5. Pokaz i wyjaśnienie własnego kodu są częścią celu zajęć.

## 3. Projekt jest źródłem prawdy

Konspekt piszemy na podstawie otwartego projektu, a nie nazwy pliku, wspomnienia autora ani poprzedniej wersji dokumentu.

Przed pisaniem należy uruchomić FINAL i zinwentaryzować:

- pełny przebieg startu: zielona flaga, przycisk, pytanie, odliczanie i komunikaty — tylko te elementy, które rzeczywiście występują w aktualnym projekcie;
- sposób sterowania i warunek końca gry;
- dokładne nazwy sceny, duszków, kostiumów, dźwięków, zmiennych i komunikatów;
- które zmienne są globalne, a które „tylko dla tego duszka”;
- wszystkie skrypty duszka objętego zadaniem;
- relacje z innymi duszkami, zwłaszcza komunikaty i równoległe skrypty;
- wartości liczbowe ważne dla efektu: punkty, czasy, pozycje, rozmiary, zakresy losowania;
- zachowanie przy zielonej fladze, starcie gry, przegranej i ponownym uruchomieniu;
- widoczny rezultat każdego bloku, który ma dodać dziecko.

W Scratchu warto dodatkowo sprawdzić `project.json` wewnątrz `.sb3`. Pozwala to potwierdzić nazwy i wartości, które na ekranie łatwo przeoczyć. Sam odczyt archiwum nie zastępuje jednak zagrania w projekt — konspekt opisuje doświadczenie dziecka, nie tylko strukturę pliku.

### Mapa projektu przed napisaniem konspektu

Autor powinien najpierw przygotować roboczą mapę:

| Element | Dokładna nazwa | Co robi | Czy dziecko go zmienia |
|---|---|---|---|
| sterowana postać | np. `postac minecraft2` | reaguje na strzałki | nie |
| duszek zadania | np. `nagroda rzadka` | pojawia się i wykrywa zebranie | tak, jeden fragment |
| duszek efektu | np. `popup +3` | reaguje na komunikat | opcjonalna personalizacja |
| zmienna wyniku | np. `punkty` | przechowuje wynik gry | tylko zmiana wartości |
| zmienna lokalna | np. `aktywna` | otwiera i zamyka możliwość zebrania | tak |
| komunikat | np. `rzadka +3` | łączy dwa duszki | tak, wysłanie |

Taka tabela nie musi trafić do importowanego pliku w tej formie, ale jej informacje muszą znaleźć się w przygotowaniu, instrukcjach i diagnostyce.

## 4. Para START i FINAL

Każda pokazówka używa dwóch projektów:

- FINAL służy instruktorowi do krótkiego pokazania pełnego efektu;
- START trafia do dziecka i jest kopią FINAL z jednym świadomie wyciętym fragmentem;
- w miejscu pracy znajduje się dokładnie jeden komentarz `TU PRACUJE UCZEŃ`;
- pozostałe mechanizmy muszą działać identycznie w obu wersjach;
- dziecko nie otrzymuje FINAL przed wykonaniem zadania; FINAL wysyłamy wyłącznie na końcu jako materiał awaryjny, gdy START nie udało się ukończyć z powodów czasowych lub technicznych.

### Dobry fragment do wycięcia

Dobry fragment:

- daje widoczną różnicę między START a FINAL;
- mieści się w około 15 minutach pracy prowadzonej;
- jest wystarczająco duży, aby po wyjaśnieniu dziecko rzeczywiście programowało i wykonywało mikrotesty przez większość etapu `guided`; dwa proste, niemal identyczne bloczki nie są pełnym zadaniem pokazowym;
- uczy jednego głównego pojęcia i najwyżej jednego pojęcia wspierającego;
- ma naturalne punkty pośrednich testów;
- nie wymaga przepisywania długiego, mechanicznego kodu;
- nie psuje startu, sterowania ani możliwości uruchomienia projektu;
- pozostawia przestrzeń na co najmniej dwie bezpieczne personalizacje.

Złym wyborem jest usunięcie całego silnika gry, skryptu resetującego stan albo długiej sekwencji, której dziecko nie zdąży zrozumieć.

Liczba bloczków nie jest celem samym w sobie, ale pomaga wykryć źle dobrany zakres. Dla typowej pokazówki Scratch/Blocks zadanie powinno zwykle obejmować około 6-12 znaczących bloczków. Mniejsza liczba jest dopuszczalna, gdy dziecko naprawdę rozumuje nad złożonym warunkiem, współrzędnymi albo wieloma parametrami i wykonuje kilka niezależnych testów. W Pythonie analogicznie liczymy znaczące instrukcje, nie fizyczne wiersze formatowania. Jeżeli osoba znająca projekt wykonuje cały fragment na czystym START w 2-3 minuty bez zatrzymania na decyzję lub diagnozę, zakres należy rozszerzyć.

### Kontrola pary

Przed publikacją sprawdź:

- czy oba pliki istnieją pod nazwami wskazanymi w konspekcie;
- czy START i FINAL nie mają identycznych skrótów pliku;
- czy FINAL nie zawiera komentarza `TU PRACUJE UCZEŃ`;
- czy START zawiera dokładnie jeden taki komentarz;
- czy w START brakuje tylko zaplanowanego fragmentu;
- czy wykonanie instrukcji w START prowadzi do zachowania widocznego w FINAL;
- czy wszystkie nazwy, liczby i kolejność bloków w konspekcie zgadzają się z aktualną wersją FINAL.
- czy sekcja `Materiały` jawnie rozdziela to, co dziecko dodaje, od kodu pozostawionego już w START;
- czy próbne wykonanie na czystym START potwierdza wystarczającą ilość pracy, a nie tylko atrakcyjny pokaz gotowego silnika.

## 5. Konstrukcja 60-minutowej pokazówki

Obecne reguły projektu i testy automatyczne wymagają:

- rodzaju `pokazowa`;
- dokładnie 60 minut;
- siedmiu kroków;
- braku osobnej przerwy;
- łącznie 25 minut kroków `[guided]` i `[challenge]`;
- kroku `[intro]` oraz `[summary]`;
- przejścia do domknięcia najpóźniej około 55. minuty.

Sprawdzony układ:

| Faza | Typ | Czas | Funkcja |
|---|---:|---:|---|
| powitanie i uruchomienie | `intro` | 5 min | relacja, poziom dziecka, kontrola techniczna |
| efekt FINAL | `demo` | 7 min | ciekawość i widoczny cel |
| mapa kodu i pojęcie | `concept` | 6 min | właściwy duszek, skrypt, przewidywanie |
| budowa krok po kroku | `guided` | 15 min | główny fragment kodu i mikrotesty |
| personalizacja | `challenge` | 10 min | decyzja dziecka i iteracja |
| test całej gry | `demo` | 10 min | diagnoza, zapis stabilnej wersji |
| pokaz i refleksja | `summary` | 7 min | wypowiedź dziecka i informacja dla rodzica |

Suma wynosi 60 minut, a kodowanie prowadzone i wyzwanie — wymagane 25 minut.

Nie trzeba zawsze używać dokładnie tych samych liczb, ale każda zmiana musi zachować reguły importera i realistyczne tempo dziecka.

## 6. Pełny przebieg pedagogiczny

### 6.1. Powitanie

Wprowadzenie powinno zawierać:

- jedno zdanie obietnicy: co dziecko zobaczy i co samo zmieni;
- pytanie pozwalające ocenić znajomość środowiska;
- dokładną ścieżkę uruchomienia projektu;
- prosty cel pierwszej próby;
- granicę: na tym etapie nie pokazujemy rozwiązania w kodzie.

Prowadzący ma wiedzieć, co zrobić zarówno z dzieckiem znającym Scratcha, jak i z osobą, która pierwszy raz widzi scenę oraz listę duszków.

### 6.2. Demo FINAL

Demo ma być krótkie i sterowane pytaniami:

- „Co zmieniło się na ekranie?”
- „Ile punktów było przed i po?”
- „Który element wydaje się najważniejszy?”
- „Jak myślisz, który duszek za to odpowiada?”

Nie omawiamy całego silnika. Pokazujemy tylko zachowania potrzebne do zrozumienia zadania i jedną konsekwencję błędu lub przegranej.

### 6.3. Odkrycie właściwego miejsca

Konspekt musi podać instruktorowi:

- dokładną nazwę duszka lub funkcji;
- jak rozpoznać go wizualnie;
- od jakiego zdarzenia zaczyna się właściwy skrypt;
- jakie są inne skrypty obok i za co odpowiadają;
- którego fragmentu nie zmieniamy;
- gdzie znajduje się komentarz `TU PRACUJE UCZEŃ`;
- jak dziecko ma przeczytać istniejący warunek własnymi słowami.

Sformułowanie „otwórz kod nagrody” jest niewystarczające, jeżeli projekt zawiera kilka podobnych nagród.

### 6.4. Praca prowadzona

Każdy blok opisujemy w schemacie:

1. Gdzie go znaleźć — nazwa kategorii.
2. Gdzie go włożyć — dokładne miejsce w stosie lub warunku.
3. Co wybrać z listy i jaką wpisać wartość.
4. Dlaczego ten blok jest potrzebny.
5. Co powinno być widoczne po teście.
6. Co sprawdzić, jeśli rezultat jest inny.

Po dwóch-trzech nowych blokach powinien wystąpić test. Oficjalne strategie debugowania Scratch zalecają budowę małymi fragmentami, ponieważ ostatnia zmiana jest wtedy pierwszym podejrzanym, gdy pojawia się błąd.

### 6.5. Wyzwanie

Wyzwanie nie może być przypadkowym „dodaj coś od siebie”. Powinno podawać bezpieczne osie personalizacji, na przykład:

- wygląd lub kostium;
- dźwięk;
- wartość punktową;
- zakres losowania;
- szybkość, czas widoczności lub pozycję;
- tekst i animację komunikatu;
- poziom trudności.

Dobra personalizacja wymaga przewidywania przed testem i krótkiego uzasadnienia po teście. Dziecko ma projektować, a nie tylko zmieniać liczby.

### 6.6. Test całej gry

Końcowy test powinien mieć mierzalną listę kryteriów. Zamiast „sprawdź, czy działa” zapisujemy na przykład:

- wynik wzrósł dokładnie o 3;
- dźwięk odtworzył się raz;
- komunikat uruchomił drugi duszek;
- obiekt zniknął;
- jedno dotknięcie nie zostało policzone kilka razy;
- ponowne uruchomienie wyzerowało wynik i stan lokalny.

Diagnoza zaczyna się od nazwania zawiedzionego efektu, a dopiero potem od szukania odpowiedzialnego bloku.

### 6.7. Pokaz i refleksja

Dziecko powinno zakończyć trzy zdania:

- „Programowałem w...”
- „Warunek sprawdza...”
- „Po spełnieniu warunku dzieje się...”

Następnie pokazuje jedną własną decyzję projektową. Rodzic powinien usłyszeć konkretnie, co dziecko zrobiło, a nie ogólne „pracowało w Scratchu”.

## 7. Pisanie dla prowadzących o różnym doświadczeniu

Każdy krok powinien odpowiadać na sześć pytań prowadzącego:

1. Co mam teraz otworzyć lub kliknąć?
2. Co dokładnie ma zrobić dziecko?
3. Co mam powiedzieć, aby nadać sens zadaniu?
4. Jaki rezultat powinienem zobaczyć?
5. Co może pójść źle i po czym to rozpoznam?
6. Co pominąć lub rozszerzyć zależnie od tempa?

### Instrukcje operacyjne

Używaj dokładnych sformułowań:

- dobrze: „Na liście duszków wybierz `nagroda rzadka`, otwórz kartę Kod i znajdź drugi skrypt zaczynający się od `kiedy otrzymam gra`”;
- źle: „Przejdź do kodu gwiazdy”.

Podawaj nazwy widoczne w aktualnej polskiej wersji środowiska. Jeśli nazwa techniczna duszka różni się od wyglądu kostiumu, wyjaśnij obie.

### Wyjaśnienie sensu

Nie wystarczy „dodaj `ustaw aktywna na 0`”. Dopisz sens: „Ten blok jako pierwszy zamyka możliwość drugiego zebrania, zanim wykonają się dźwięk i animacja”.

### Obserwowalny rezultat

Po instrukcji dodaj informację weryfikacyjną: „Po dotknięciu wynik ma wzrosnąć z 4 do 7, a napis +3 pojawić się raz”. Dzięki temu mniej doświadczony instruktor wie, czy może przejść dalej.

## 8. Pytania dziecka i język odpowiedzi

Przewiduj pytania wynikające z projektu, nie tylko definicje pojęć.

Typowe kategorie:

- „Dlaczego programujemy ten duszek, a nie gracza?”
- „Dlaczego są dwa podobne skrypty?”
- „Co robi zmienna `aktywna`?”
- „Dlaczego używamy `zmień`, a nie `ustaw`?”
- „Skąd drugi duszek wie, że ma się pokazać?”
- „Czy mogę wpisać 100, liczbę ujemną albo zero?”
- „Dlaczego obiekt nie pojawia się od razu?”
- „Czy mogę zmienić wygląd, dźwięk lub regułę?”

Dobra odpowiedź:

- jest krótka;
- odnosi się do tego, co dziecko widzi;
- kończy się pytaniem lub małym eksperymentem;
- nie odbiera dziecku decyzji.

Przykład:

> „`nadaj` działa jak krótkie hasło przez krótkofalówkę. Który duszek w projekcie może czekać na hasło `rzadka +3`? Sprawdźmy jego skrypty.”

## 9. Drabina pomocy

Prowadzący nie powinien od razu wskazywać gotowego bloku. Stosujemy pomoc od najmniejszej do największej:

1. Poproś dziecko o opis oczekiwanego efektu.
2. Poproś o przeczytanie obecnego kodu od góry.
3. Zapytaj, który fragment ostatnio się zmienił.
4. Wskaż właściwy skrypt.
5. Wskaż kategorię bloków.
6. Pokaż nazwę bloku, ale dziecko go przeciąga.
7. Dopiero w sytuacji ograniczenia czasu wykonaj wspólnie minimum i pozostaw dziecku ostatnią decyzję.

Przydatne kwestie `[mów]`:

- „Co program robi teraz, a co miał robić?”
- „Przejdźmy przez stos blok po bloku”.
- „Który z efektów już działa?”
- „Co zmieniliśmy od ostatniego udanego testu?”
- „Jak możemy sprawdzić tylko ten jeden fragment?”
- „Nie znam jeszcze odpowiedzi — przetestujmy dwie wersje”.

## 10. Diagnostyka w konspekcie

Wskazówki `[błąd]` powinny mieć postać objaw → prawdopodobna przyczyna → kontrola.

Przykłady:

- wynik rośnie bez dotknięcia → blok punktów znajduje się poza warunkiem → przeciągnij go do wnętrza `jeżeli`;
- wynik rośnie kilka razy → blok zamykający `aktywna` jest za późno → ustaw go jako pierwszy skutek;
- wynik się zmienia, ale brak popupu → nazwy komunikatów nie są identyczne → porównaj blok `nadaj` i zdarzenie odbiorcy;
- obiekt nie pojawia się w teście → działa długi czas losowy → tymczasowo ustaw krótki zakres testowy;
- po zielonej fladze jest tylko ekran startowy → projekt czeka na przycisk w grze → kliknij przycisk, nie przebudowuj kodu.

Nie pisz samego „sprawdź kod”. Instruktor potrzebuje kolejności sprawdzania.

## 11. Różnicowanie poziomu

Każdy konspekt powinien zawierać trzy tory.

### Dziecko początkujące

- poznaje tylko potrzebne elementy interfejsu;
- dostaje nazwy kategorii;
- dodaje jeden blok naraz;
- mówi prostymi zdaniami o efekcie, bez wymagania formalnej definicji;
- testuje po małych zmianach.

### Dziecko doświadczone

- najpierw samo przewiduje potrzebne bloki;
- porównuje podobny działający duszek;
- wyjaśnia kolejność i zależności;
- otrzymuje rozszerzenie zmieniające zachowanie, a nie tylko kolor;
- może zmodyfikować dwa powiązane komponenty, o ile zachowa ich spójność.

### Gdy brakuje czasu

- wskazujemy minimalny działający zestaw bloków;
- pomijamy dekorację przed głównym mechanizmem;
- robimy jeden pełny test zamiast trzech;
- zapis i pokaz mają pierwszeństwo przed kolejnym rozszerzeniem;
- od około 55. minuty nie zaczynamy nowej funkcji.

## 12. Znaczniki obsługiwane przez importer

### Metadane przed pierwszym krokiem

Minimalny nagłówek:

```text
# Pokazowa: Tytuł projektu - nazwa modyfikacji
Rodzaj: pokazowa
Subject: Scratch
Level: Pokazowa - 8-10 lat
Czas: 60 min
Tags: Pokazowa, Scratch, ...
Opis: Jedno lub dwa konkretne zdania o doświadczeniu dziecka i jego pracy.
Cel: Jedno zdanie opisujące najważniejszą umiejętność.
```

Obsługiwane sekcje lekcji przed pierwszym `##`:

- `### Po zajęciach dziecko potrafi`
- `### Przygotuj przed zajęciami`
- `### Zadanie domowe`

Nie twórz w tym miejscu innych nagłówków `###`, ponieważ parser ich nie rozpozna.

### Kroki

Nagłówek kroku ma postać:

```text
## [guided] Nazwa kroku (15 min)
```

Używane typy:

- `[intro]`
- `[demo]`
- `[concept]`
- `[guided]`
- `[challenge]`
- `[summary]`

W pokazówce nie dodajemy `[break]`.

Wewnątrz kroku używaj tylko rozpoznawanych sekcji:

- `### Co robić teraz`
- `### Wskazówki`
- `### Materiały`

### Znaczniki treści

- `[mów]` — gotowa, krótka kwestia do wypowiedzenia;
- `[podpowiedź]` — sposób naprowadzenia lub odpowiedź na pytanie;
- `[błąd]` — diagnoza konkretnego problemu;
- `[tempo]` — granica czasowa;
- `[dla szybszych]` — rozszerzenie;
- `[gdy brakuje czasu]` — bezpieczne minimum;
- `[kod] Etykieta | Scratch:` — materiał kodowy, po którym bezpośrednio występuje blok potrójnych backticków.

W `### Materiały` przed elementem `[kod]` dodaj dwie zwykłe pozycje:

- `Zakres pracy dziecka:` — konkretna liczba lub lista bloczków, instrukcji i decyzji tworzonych podczas zajęć;
- `Gotowe w START:` — zdarzenia, warunki, funkcje pomocnicze i mechanizmy widoczne w pseudokodzie, których dziecko nie buduje.

Pseudokod może pokazywać cały docelowy stos dla zachowania kontekstu, ale prowadzący nie może zgadywać, które jego fragmenty są już gotowe.

Etykieta `[kod]` musi zaczynać się od dokładnej lokalizacji: `Duszek „...”`, `Funkcja „...”` albo `Zdarzenie ...`. Pierwsza linia samego bloku kodu również zaczyna się od `MIEJSCE:`; w Pythonie może to być komentarz `# MIEJSCE:`. Dzięki temu karta „Materiały pomocnicze” pozostaje samodzielna nawet wtedy, gdy prowadzący nie widzi wcześniejszego opisu kroku. W etykiecie nie używaj backticków — interfejs pokazuje ją jako zwykły tekst, a nie Markdown.

Nie wymyślaj nowych znaczników bez zmiany parsera. Nierozpoznany nagłówek lub źle osadzony blok kodu może spowodować utratę treści podczas importu.

## 13. Wzorzec importowanego pliku

Poniższy szkielet pokazuje strukturę, nie gotową treść do kopiowania bez dostosowania do projektu.

````markdown
# Pokazowa: Projekt - modyfikacja
Rodzaj: pokazowa
Subject: Scratch
Level: Pokazowa - 8-10 lat
Czas: 60 min
Tags: Pokazowa, Scratch, Gra, Główne pojęcie
Opis: Dziecko poznaje działający projekt, a następnie programuje konkretny efekt w dokładnie nazwanym duszku.
Cel: Dziecko łączy jedno główne pojęcie z widocznym rezultatem w grze.

### Po zajęciach dziecko potrafi
- wskazać właściwy duszek i skrypt
- wyjaśnić główny mechanizm własnymi słowami
- zbudować, przetestować i spersonalizować reakcję

### Przygotuj przed zajęciami
- otwórz dokładnie nazwane pliki FINAL i START
- przejdź cały start i sprawdź sterowanie
- zapisz mapę duszków, zmiennych i komunikatów
- sprawdź jednoznaczne miejsce `TU PRACUJE UCZEŃ`

### Zadanie domowe
- zaprojektuj rozwinięcie wykorzystujące poznany mechanizm

## [intro] Połączenie i ekrany (5 min)

### Co robić teraz
- Dziecko udostępnia cały ekran i uruchamia własne środowisko.
- Po kontroli technicznej instruktor przejmuje udostępnianie, ale nie wysyła jeszcze plików.
- [mów] Krótka obietnica efektu i sprawczości dziecka.

### Wskazówki
- [tempo] Granica przejścia do demo.
- [błąd] Najczęstszy problem techniczny startu.

## [demo] Efekt FINAL (7 min)

### Co robić teraz
- Instruktor uruchamia FINAL na swoim ekranie; dziecko obserwuje i kieruje próbą głosem.
- Co dziecko ma zaobserwować i jakie wartości porównać przed i po.
- Po pokazie instruktor wysyła wyłącznie START, kończy udostępnianie, a dziecko ponownie udostępnia cały ekran.

### Wskazówki
- [podpowiedź] Odpowiedź na prawdopodobne pytanie.

## [concept] Mapa duszka i pojęcie (6 min)

### Co robić teraz
- Dziecko odbiera, zapisuje i uruchamia START na swoim komputerze.
- Testem porównawczym odkrywa funkcję brakującą względem FINAL.
- Dokładna nazwa duszka, skryptu i miejsca komentarza.
- Co już działa, czego nie zmieniamy i co dziecko przewiduje.

### Materiały
- Zakres pracy dziecka: konkretne bloczki, instrukcje i decyzje tworzone podczas zajęć.
- Gotowe w START: elementy docelowego stosu pozostawione jako rama zadania.
- [kod] Duszek „dokładna nazwa” — dokładny skrypt lub zdarzenie | Scratch:
```text
MIEJSCE: duszek „dokładna nazwa” → skrypt „dokładne zdarzenie” → miejsce komentarza
tu znajduje się dokładny pseudokod zgodny z FINAL
```

### Wskazówki
- [podpowiedź] Wyjaśnienie pojęcia w języku dziecka.
- [błąd] Ostrzeżenie przed pracą w podobnym, niewłaściwym skrypcie.

## [guided] Budowa i mikrotesty (15 min)

### Co robić teraz
- Każdy blok: kategoria, miejsce, wartość, sens i wynik testu.
- Test po małym fragmencie oraz test całej reakcji.

### Wskazówki
- [błąd] Objaw, przyczyna i sposób sprawdzenia.
- [gdy brakuje czasu] Minimalny działający zestaw.

## [challenge] Personalizacja z decyzją dziecka (10 min)

### Co robić teraz
- Dwie lub trzy bezpieczne osie personalizacji.
- Przewidywanie, test i wybór ustawienia końcowego.

### Wskazówki
- [dla szybszych] Rozszerzenie zachowania.
- [gdy brakuje czasu] Jedna szybka, lecz rzeczywista decyzja.

## [demo] Pełny test i zapis (10 min)

### Co robić teraz
- Lista obserwowalnych kryteriów zaliczenia.
- Diagnoza tylko zawiedzionego efektu i zapis nowego pliku.

### Wskazówki
- [tempo] Stabilna wersja zapisana najpóźniej około 53. minuty.

## [summary] Pokaz i refleksja (7 min)

### Co robić teraz
- Dziecko pokazuje efekt, wskazuje kod i wyjaśnia własną decyzję.
- [mów] Konkretne podsumowanie zdobytej umiejętności.
- Jeśli START nie został ukończony z powodów czasowych lub technicznych: zapisz pracę częściową, wyślij FINAL jako osobny plik i pozwól dziecku go uruchomić.

### Wskazówki
- [tempo] Od 55. minuty nie dodawaj nowej funkcji.
````

## 14. Najczęstsze błędy autorów konspektów

### Opisuje pomysł, a nie aktualny projekt

Objawy: stara nazwa duszka, nieistniejący dźwięk, błędna wartość punktów, instrukcja odnosząca się do usuniętego skryptu.

Naprawa: ponowna inwentaryzacja FINAL oraz porównanie START blok po bloku.

### Nie wiadomo, gdzie pracować

Objaw: „otwórz nagrodę i dodaj kod”.

Naprawa: podaj nazwę duszka, kostium rozpoznawczy, zdarzenie rozpoczynające skrypt, istniejący warunek i dokładne miejsce komentarza.

### Jest dużo klikania, ale mało rozumienia

Objaw: długa lista bloków bez pytań „dlaczego?” i bez przewidywania.

Naprawa: po każdym fragmencie dodaj krótkie pytanie o skutek, kolejność albo różnicę względem podobnego skryptu.

### Personalizacja psuje spójność

Objaw: dziecko zmienia nagrodę z +3 na +5, ale popup nadal pokazuje +3.

Naprawa: wskaż wszystkie zależne miejsca albo ogranicz personalizację do parametru niezależnego, np. czasu czy rozmiaru.

### Test odbywa się dopiero na końcu

Objaw: po 20 minutach nie wiadomo, który blok wprowadził błąd.

Naprawa: zaplanuj mikrotesty po małych sekwencjach i osobny test całej gry.

### Prowadzący ma przejąć mysz

Objaw: instrukcja brzmi „pokaż dziecku, jak dodać wszystkie bloki”.

Naprawa: prowadzący wskazuje cel, pyta, ewentualnie podaje kategorię; dziecko przeciąga, wpisuje, uruchamia i wyjaśnia.

### Zbyt wiele nowego materiału

Objaw: jedna lekcja jednocześnie uczy zmiennych, klonów, list, fizyki, komunikatów i własnych bloków.

Naprawa: wybierz jedno pojęcie główne, jedno wspierające, a resztę pozostaw gotową w START.

## 15. Proces autora od projektu do importu

1. Uruchom FINAL i przejdź pełną ścieżkę użytkownika.
2. Zapisz mapę duszków, skryptów, zmiennych, komunikatów i wartości.
3. Wybierz jeden efekt główny i fragment wykonywany przez dziecko.
4. Utwórz START przez usunięcie tylko tego fragmentu.
5. Dodaj dokładnie jeden komentarz `TU PRACUJE UCZEŃ`.
6. Wykonaj zadanie samodzielnie na czystym START, mierząc czas.
7. Rozpisz siedem kroków: doświadczenie, odkrycie, budowa, personalizacja, test, pokaz.
8. Do każdego kroku dopisz pytania, błędy, tempo i wariant poziomu.
9. Porównaj wszystkie nazwy oraz liczby z aktualnym FINAL.
10. Uruchom dry-run importera.
11. Uruchom testy pokazówek i weryfikację par projektów.
12. Dopiero wtedy przekaż konspekt instruktorom.

## 16. Checklista odbioru

### Zgodność z projektem

- [ ] Wszystkie wskazane pliki istnieją.
- [ ] Nazwy duszków, kostiumów, zmiennych, dźwięków i komunikatów są dokładne.
- [ ] Pseudokod odpowiada kolejności bloków w FINAL.
- [ ] `Materiały` rozdzielają `Zakres pracy dziecka` od `Gotowe w START`.
- [ ] Karta `[kod]` podaje w etykiecie duszka, funkcję lub zdarzenie, a blok zaczyna się od `MIEJSCE:`.
- [ ] Zadanie nie sprowadza się do dwóch prostych, powtarzalnych bloczków; próbne wykonanie na czystym START potwierdza realną pracę na etap `guided`.
- [ ] START ma jedno zadanie i jeden komentarz.
- [ ] FINAL pokazuje dokładnie efekt obiecany w tytule i opisie.

### Prowadzenie

- [ ] Konspekt wskazuje moment udostępniania ekranu dziecka, ekranu instruktora i ponownego przełączenia na dziecko.
- [ ] FINAL jest pokazywany na komputerze instruktora i nie jest wysyłany przed pracą.
- [ ] START jest wysyłany dopiero po omówieniu FINAL.
- [ ] Instruktor wie, jak uruchomić projekt od zielonej flagi do gry.
- [ ] Wie, w którym duszku i skrypcie pracuje dziecko.
- [ ] Rozumie rolę gotowych skryptów obok miejsca pracy.
- [ ] Ma gotowe krótkie kwestie `[mów]`.
- [ ] Ma odpowiedzi na co najmniej cztery prawdopodobne pytania dziecka.
- [ ] Ma diagnozę co najmniej trzech konkretnych błędów.

### Dziecko

- [ ] Najpierw doświadcza efektu, potem poznaje kod.
- [ ] Samodzielnie przeciąga bloki i uruchamia testy.
- [ ] Ma łatwe minimum i ambitne rozszerzenie.
- [ ] Podejmuje co najmniej jedną decyzję projektową.
- [ ] Potrafi wskazać miejsce swojej pracy i opisać skutek.

### Czas i importer

- [ ] Procedura awaryjna każe najpierw zapisać częściowy START, a dopiero potem wysłać FINAL jako osobny plik.
- [ ] Jest dokładnie siedem kroków i 60 minut.
- [ ] `[guided]` plus `[challenge]` daje 25 minut.
- [ ] Nie ma nieobsługiwanych nagłówków ani znaczników.
- [ ] Każdy blok kodu ma poprzedzający element `[kod]`.
- [ ] Stabilny projekt jest zapisywany przed końcowym pokazem.
- [ ] Po 55. minucie nie rozpoczyna się nowy materiał.
- [ ] Dry-run importera kończy się bez błędów i uwag.
- [ ] Testy par START/FINAL przechodzą.

## 17. Zasada końcowa

Kompleksowy konspekt nie ma prowadzić dziecka za rękę przez każdą decyzję. Ma dać prowadzącemu na tyle dokładną mapę techniczną i pedagogiczną, aby mógł bezpiecznie oddać dziecku sterowanie.

Instruktor powinien znać projekt dokładniej dzięki dokumentowi. Dziecko powinno natomiast czuć, że projekt staje się jego własny dzięki pytaniom, eksperymentom, testom i wyborom.
