# Pokazowa: Arena Łowców - Piorunowy Miecz
Rodzaj: pokazowa
Subject: Minecraft Education - MakeCode Blocks
Level: Pokazowa - 9-11 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Blocks, Gra, Zdarzenia, Cooldown
Opis: Podczas lekcji zdalnej dziecko najpierw pokazuje własnego Minecrafta, potem ogląda FINAL tylko na ekranie instruktora. Po otrzymaniu START uruchamia na swoim ekranie niedziałającą moc i w MakeCode Blocks programuje Piorunowy Miecz z efektem trzy bloki przed graczem oraz czterosekundowym odnowieniem.
Cel: Dziecko buduje obsługę zdarzenia z blokadą wielokrotnego użycia, widocznym efektem i odmierzonym czasem odnowienia.

### Po zajęciach dziecko potrafi
- wyjaśnić, czym różni się zdarzenie użycia przedmiotu od komendy czatu
- zablokować moc na czas odnowienia i ponownie ją włączyć
- przywołać efekt w bezpiecznej odległości od gracza

### Przygotuj przed zajęciami
- na swoim komputerze zaimportuj i sprawdź `projekty/05-Arena-Lowcow-FINAL.mkcd`; START miej przygotowany, ale nie wysyłaj przed pokazem
- dziecko najpierw udostępnia cały ekran, uruchamia Minecraft Education, płaski świat i MakeCode; później instruktor przejmuje udostępnianie do pokazu FINAL
- dopiero po pokazie FINAL dziecku wyślij tylko `projekty/05-Arena-Lowcow-START.mkcd`; dziecko importuje go samodzielnie w MakeCode Blocks
- nie przełączaj projektów na Python; sprawdź komendy `arena`, `start`, `testmod`, `boss` i `reset`
- w START odnajdź zdarzenie użycia diamentowego miecza, warunek `gotowa_moc = 1` oraz komentarz `TU PRACUJE UCZEŃ`
- zachowaj czysty START; FINAL można przesłać dopiero pod koniec jako osobny wariant awaryjny po zapisaniu pracy dziecka

### Zadanie domowe
- zaprojektuj drugą moc miecza i określ jej efekt oraz czas odnowienia

## [intro] Połączenie i środowisko dziecka (5 min)

### Co robić teraz
- Dziecko udostępnia ekran, uruchamia Minecrafta i otwiera Code Builder. Sprawdza czat, ekwipunek i możliwość powrotu do MakeCode.
- Na tym etapie nie wysyłaj pliku. Zapytaj, jak w grze można ograniczyć supermoc, aby nie dało się używać jej bez końca.
- [mów] Na moim ekranie zobaczysz gotową arenę. Potem dostaniesz START, w którym miecz jest zwykły, choć zdarzenie i system gry już istnieją.
- Przełącz udostępnianie na ekran instruktora z FINAL, bez pokazywania bloczków.

### Wskazówki
- [błąd] Piorunowy Miecz działa tylko podczas aktywnej gry albo testu `testmod`.
- [podpowiedź] Udostępniaj cały ekran, aby po imporcie widzieć zarówno kod, jak i rezultat w Minecraft.
- [tempo] Najpóźniej w 4. minucie przejdź do FINAL instruktora.

## [demo] FINAL wyłącznie na ekranie instruktora (7 min)

### Co robić teraz
- Instruktor używa `testmod`, pojawia się jeden zombie, a dziecko mówi, kiedy użyć diamentowego miecza.
- Pokaż piorun trzy bloki przed graczem, komunikat `PIORUN!`, brak ponownego efektu podczas ładowania i komunikat `Moc gotowa!` po około czterech sekundach.
- Pokaż skrótem `boss` dalszy fragment gry, nie odsłaniając kodu.
- [mów] W START kliknięcie mieczem nie uruchomi tej sekwencji. Zaprogramujesz blokadę, efekt, czas odnowienia i ponowne włączenie mocy.
- Zatrzymaj własne udostępnianie, wyślij tylko START i wróć do udostępnionego ekranu dziecka.

### Wskazówki
- [tempo] Test pojedynczego zombie jest szybszy i bezpieczniejszy niż pełna fala.
- [podpowiedź] Gdy dziecko pyta, czemu piorun nie uderza w gracza, zapowiedz względną pozycję `(0, 0, 3)`.
- [błąd] Nie wysyłaj FINAL razem ze START i nie pokazuj gotowego zdarzenia.

## [concept] Dlaczego miecz w START milczy? (6 min)

### Co robić teraz
- Dziecko importuje START, uruchamia `testmod` i używa miecza. Zombie się pojawia, lecz nie ma pioruna ani komunikatu — to oczekiwany brak.
- [mów] Zdarzenie rozpoznaje przedmiot i grę, ale puste wnętrze warunku nie mówi jeszcze, co ma zrobić gotowa moc.
- Dziecko odnajduje zdarzenie użycia diamentowego miecza, warunek gry, wewnętrzny warunek `gotowa_moc = 1` i komentarz.
- Odczytajcie znaczenie `gotowa_moc = 1` oraz `gotowa_moc = 0`.
- Dziecko układa kolejność: zablokuj, efekt, pauza, odblokuj.

### Materiały
- Zakres pracy dziecka: sześć bloczków reakcji — blokada, komunikat, piorun, pauza, odblokowanie i komunikat gotowości.
- Gotowe w START: zdarzenie użycia miecza oraz dwa zewnętrzne warunki sprawdzające grę i dostępność mocy.
- [kod] Zdarzenie użycia diamentowego miecza — wnętrze „gotowa_moc = 1” | MakeCode Blocks:
```text
MIEJSCE: MakeCode Blocks → zdarzenie użycia diamentowego miecza → wewnętrzny warunek „gotowa_moc = 1”
gdy gracz użyje diamentowego miecza
  jeżeli gra_trwa = 1
    jeżeli gotowa_moc = 1
      ustaw gotowa_moc na 0
      powiedz „PIORUN!”
      przywołaj piorun w pozycji (0, 0, 3)
      pauza 4000 ms
      ustaw gotowa_moc na 1
      powiedz „Moc gotowa!”
```

### Wskazówki
- [podpowiedź] Porównaj `gotowa_moc` do drzwi: 1 oznacza otwarte, 0 zamknięte na czas ładowania.
- [podpowiedź] Zapytaj, dlaczego ustawienie 0 musi być przed piorunem i pauzą, a nie po nich.
- [błąd] Nowe bloczki umieszczamy w wewnętrznym warunku, nie obok całego zdarzenia.

## [guided] Programujemy moc miecza (15 min)

### Co robić teraz
- Całe programowanie wykonuje dziecko w MakeCode na własnym udostępnionym ekranie; prowadzący kieruje pytaniami, nie myszą.
- W pustym wewnętrznym warunku ustawia `gotowa_moc` na 0 i wykonuje pierwszy test: ponowne użycie nie powinno uruchamiać nowej sekwencji.
- Dodaje komunikat `PIORUN!` i przywołanie pioruna w pozycji względnej `(0, 0, 3)`. Testuje, czy efekt pojawia się przed postacią.
- Dodaje pauzę 4000 ms, ponowne ustawienie `gotowa_moc` na 1 i komunikat `Moc gotowa!`.
- Uruchamia `reset`, potem `testmod`, próbuje użyć mocy natychmiast drugi raz oraz po komunikacie gotowości.

### Wskazówki
- [podpowiedź] Pozycja (0, 0, 3) daje efekt blisko, ale nie bezpośrednio na graczu.
- [podpowiedź] 4000 ms to 4 sekundy; poproś dziecko o głośne odliczenie czasu między próbami.
- [błąd] Bez ustawienia 0 przed pauzą gracz może uruchomić kilka mocy jednocześnie.
- [błąd] Jeśli moc działa tylko raz, najpierw sprawdź ponowne ustawienie 1 po pauzie. Jeśli działa bez przerwy, sprawdź ustawienie 0 na początku.
- [błąd] Jeśli piorun trafia gracza, sprawdź, czy pozycja jest względna i czy z wynosi 3.
- [gdy brakuje czasu] Ostatni komunikat jest opcjonalny; blokada i odblokowanie są obowiązkowe.

## [challenge] Balans czasu odnowienia (10 min)

### Co robić teraz
- Dziecko wybiera cooldown od 2500 do 6000 ms.
- Próbuje użyć miecza podczas ładowania i po jego zakończeniu.
- Dobiera czas tak, aby moc była efektowna, ale nie zastępowała całej walki, i uzasadnia wybór.
- Zmienia jedną wartość na próbę i porównuje co najmniej dwa warianty.

### Wskazówki
- [dla szybszych] Dodaj drugi piorun po przeciwnej stronie, nadal poza pozycją gracza.
- [gdy brakuje czasu] Pozostaw 4000 ms i zmień jedynie tekst komunikatu mocy.

## [demo] Pełna walka z własną mocą (10 min)

### Co robić teraz
- Dziecko wpisuje `reset`, potem `start` i używa mocy podczas fali.
- Sprawdźcie kryteria: piorun jest przed graczem, pierwsze użycie blokuje następne, moc nie działa podczas cooldownu, po komunikacie znów działa, a licznik i boss nie są uszkodzone.
- Jeśli test zawodzi, dziecko nazywa etap sekwencji: wejście do warunku, blokada, efekt, pauza albo odblokowanie. Poprawia tylko powiązany fragment.
- Nazwijcie rezultat: START odtwarza Piorunowy Miecz z FINAL i zawiera wybrany przez dziecko balans.
- Zapisuje projekt pod nową nazwą z imieniem, nie nadpisując czystego START.

### Wskazówki
- [tempo] Jedno poprawne użycie podczas fali wystarczy do akceptacji.
- [błąd] Po zmianie kodu wykonaj nowy `reset`/`testmod`; przeciwnik ze starego testu nie potwierdza aktualnej wersji.

## [summary] Pokaz, zapis i wariant awaryjny (7 min)

### Co robić teraz
- Jeśli START jest ukończony, dziecko uruchamia `testmod`, pokazuje moc i wskazuje bloki 0, pauzy oraz 1. FINAL pozostaje wyłącznie u instruktora.
- [mów] Zbudowałeś zdarzenie z kontrolowanym czasem odnowienia. Twój START ma teraz supermoc widzianą wcześniej w FINAL.
- Zapytaj, co stałoby się bez pierwszego ustawienia 0 oraz bez końcowego ustawienia 1.
- Gdy lekcji nie udało się ukończyć, najpierw zapisz częściowy START z dopiskiem `NIEDOKONCZONE` i nazwij działające elementy.
- Dopiero potem wyślij `05-Arena-Lowcow-FINAL.mkcd` jako osobny materiał pomocniczy. Nie nadpisuj nim pracy dziecka i nie przedstawiaj go jako jej rezultatu.
- Przypomnij nazwę zapisanego projektu i zadanie domowe.

### Wskazówki
- [podpowiedź] Rodzicowi wskaż konkretnie: zdarzenie przedmiotu, zmienną stanu, względną pozycję efektu i cooldown.
- [błąd] Nie sięgaj po FINAL przy pierwszym problemie; wykorzystaj test pojedynczego elementu sekwencji.
- [tempo] Po 55. minucie priorytetem jest zapis i samodzielny pokaz dziecka.
