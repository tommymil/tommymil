# Pokazowa: TNT Arena - Znikające Pola
Rodzaj: pokazowa
Subject: Scratch
Level: Pokazowa - 10-12 lat
Czas: 60 min
Tags: Pokazowa, Scratch, Gra, Klony, Losowość, Warunki, Animacja
Opis: Podczas lekcji zdalnej dziecko najpierw ogląda FINAL wyłącznie na ekranie instruktora, a po pokazie otrzymuje START i uruchamia go na własnym udostępnionym ekranie. Odkrywa, że na drugim poziomie pola TNT nie znikają, po czym programuje losowe opóźnienie, ochronę pola z diamentem i animację usuwania klonu.
Cel: Dziecko łączy klony, losowanie, warunek i zmienną lokalną, aby każde pole samodzielnie decydowało, czy i kiedy bezpiecznie zniknąć.

### Po zajęciach dziecko potrafi
- wyjaśnić, jak jeden duszek tworzy planszę z wielu klonów
- użyć losowania do wybrania części pól TNT
- zabezpieczyć pole z diamentem przed przypadkowym usunięciem
- zbudować krótką animację ostrzegającą przed zniknięciem pola

### Przygotuj przed zajęciami
- na swoim komputerze otwórz i przetestuj `projekty/10-TNT-Arena-FINAL.sb3`; START miej gotowy, ale nie wysyłaj go przed pokazem
- przygotuj przełączanie udostępniania całego ekranu oraz kanał przesłania pliku `.sb3`
- na początku dziecko udostępnia ekran i uruchamia własny Scratch; potem prowadzący pokazuje FINAL na swoim ekranie bez otwierania karty Kod
- dopiero po pokazie FINAL dziecku wyślij tylko `projekty/10-TNT-Arena-START.sb3`; dziecko otwiera plik samodzielnie i ponownie udostępnia ekran
- sprawdź w FINAL sterowanie strzałkami, przejście między poziomami oraz drugi poziom z diamentem i losowo znikającymi polami
- w START wybierz duszka `TNT`, skrypt `kiedy zaczynam jako klon` i komentarz `TU PRACUJE UCZEŃ`; pozostawione są gotowe warunki poziomu 2 i losowania 1 z 4
- zachowaj czysty START; FINAL wysyłaj wyłącznie pod koniec, gdy czasu lub warunków technicznych zabraknie na ukończenie, i nigdy nie nadpisuj nim pracy dziecka

### Zadanie domowe
- zaprojektuj trzeci poziom i zdecyduj, jaka część pól powinna na nim znikać

## [intro] Połączenie i ekran dziecka (5 min)

### Co robić teraz
- Dziecko udostępnia cały ekran, uruchamia własny Scratch i pokazuje, gdzie zapisują się pobrane pliki. Na tym etapie nie wysyłaj projektu.
- Sprawdź mysz, klawiaturę, dźwięk i możliwość przełączania między rozmową a edytorem.
- Zapytaj, czy dziecko zna klony. Nie wymagaj definicji; wystarczy przykład: jeden wzór pola może stworzyć całą planszę.
- [mów] Najpierw pokażę gotową arenę na moim komputerze. Potem dostaniesz wersję, w której pola na drugim poziomie nie mają jeszcze zaprogramowanego znikania.
- Dziecko zatrzymuje udostępnianie, a prowadzący udostępnia FINAL bez pokazywania kodu.

### Wskazówki
- [podpowiedź] Jeśli dziecko pyta, czy zbuduje całą grę, wyjaśnij, że plansza i sterowanie są silnikiem, a ono tworzy najważniejszą mechanikę drugiego poziomu.
- [tempo] Najpóźniej w 4. minucie rozpocznij pokaz FINAL.
- [błąd] Nie wysyłaj jeszcze START ani FINAL.

## [demo] FINAL na ekranie instruktora (7 min)

### Co robić teraz
- Instruktor uruchamia FINAL i steruje, a dziecko głosem wybiera kierunek ruchu. Pokaż krótko poziom 1 i przejście do poziomu 2.
- Na poziomie 2 dziecko obserwuje diament i pola, które po różnym czasie zmieniają kostiumy, znikają i odsłaniają przepaść.
- Powtórz drugi poziom, aby zauważyć, że nie zawsze znikają te same pola. Poproś dziecko o hipotezę, skąd bierze się różnica.
- [mów] Każdy klon TNT osobno losuje, czy zostanie pułapką. Pole z diamentem musi być chronione niezależnie od wyniku losowania.
- Zapowiedz brak w START: drugi poziom się uruchomi, ale pola pozostaną nieruchome. Zatrzymaj udostępnianie i dopiero teraz wyślij START.
- Dziecko ponownie udostępnia cały ekran, zapisuje plik i otwiera go we własnym Scratchu.

### Wskazówki
- [podpowiedź] Gdy dziecko pyta „które pole zniknie?”, odpowiedz: „Tego przed startem nie wie nawet program — każde pole dopiero wykona własne losowanie”.
- [tempo] Nie próbuj perfekcyjnie przechodzić gry; ważne są różne momenty znikania i bezpieczny diament.
- [błąd] Nie pokazuj gotowego stosu bloków i nie przekazuj FINAL razem ze START.

## [concept] Co dokładnie nie działa w START? (6 min)

### Co robić teraz
- Dziecko uruchamia START od zielonej flagi i dociera do poziomu 2. Nazywa różnicę: pola TNT nie ostrzegają, nie znikają i arena pozostaje bezpieczna.
- [mów] To oczekiwany brak, nie awaria. Sprawimy, że część klonów odczeka losowy czas, sprawdzi bezpieczeństwo i usunie samą siebie.
- Dziecko zatrzymuje projekt, wybiera duszka `TNT` i odnajduje `kiedy zaczynam jako klon`, gotowy warunek poziomu 2 i losowania oraz komentarz `TU PRACUJE UCZEŃ`.
- Omówcie kolejność: losowe czekanie → kontrola `Stan pola = 0` i braku kontaktu z `Diament` → blokada stanu → animacja → ukrycie → usunięcie klonu.

### Materiały
- Zakres pracy dziecka: losowe czekanie, złożony warunek ochronny, blokada stanu, pętla animacji, ukrycie i usunięcie klonu.
- Gotowe w START: zdarzenie klonu oraz zewnętrzny warunek ograniczający mechanikę do poziomu 2 i wyniku losowania 1 z 4.
- [kod] Duszek „TNT” — losowe znikanie w skrypcie klonu | Scratch:
```text
MIEJSCE: duszek „TNT” → skrypt „kiedy zaczynam jako klon” → wnętrze gotowego warunku poziomu 2
kiedy zaczynam jako klon
jeżeli <(Poziom = 2) i <(losuj od 1 do 4) = 1>> to
  czekaj (losuj od 1 do 4) sekund
  jeżeli <(Stan pola = 0) i <nie <dotyka [Diament v]?>>> to
    ustaw [Stan pola v] na (1)
    powtórz (6) razy
      następny kostium
      czekaj (0.08) sekundy
    ukryj
    usuń tego klona
```

### Wskazówki
- [podpowiedź] `Stan pola` jest zmienną tylko dla tego duszka, więc każdy klon pamięta własny stan.
- [podpowiedź] Zapytaj: „Dlaczego samo `nie dotyka Diament` nie wystarcza?” — bez stanu mechanizm mógłby ruszyć ponownie.
- [błąd] Pracujemy w `TNT`, nie w duszku gracza ani diamentu.

## [guided] Programujemy pułapkę na ekranie dziecka (15 min)

### Co robić teraz
- Całe programowanie wykonuje dziecko na swoim udostępnionym ekranie. Prowadzący naprowadza pytaniami i nazwami kategorii, ale nie przejmuje myszy.
- W gotowym zewnętrznym warunku dziecko dodaje `czekaj` z losowaniem od 1 do 4 sekund. Wyjaśnia, dlaczego wszystkie pola nie powinny znikać równocześnie.
- Dodaje wewnętrzny warunek z operatorem `i`: `Stan pola = 0` oraz `nie dotyka Diament`. W razie trudności testuje go tymczasowym `powiedz`.
- Wewnątrz ustawia `Stan pola` na 1, a następnie buduje pętlę `powtórz 6 razy` z `następny kostium` i `czekaj 0.08 sekundy`.
- Po pętli dodaje `ukryj` oraz `usuń tego klona`. Uruchamia próbę poziomu 2 i sprawdza osobno animację, zniknięcie oraz ochronę diamentu.

### Wskazówki
- [podpowiedź] Blok `nie` musi otaczać całe pytanie `dotyka Diament?`, a oba pytania trafiają do operatora `i`.
- [podpowiedź] Jeśli dziecko pyta o 1 z 4, rozpisz możliwe wyniki 1, 2, 3, 4 — tylko jeden uruchamia pułapkę.
- [błąd] `Usuń tego klona` umieść po pętli. W jej wnętrzu klon zniknąłby po pierwszej klatce animacji.
- [błąd] Gdy znika pole z nagrodą, sprawdź nazwę duszka w `dotyka` i obecność operatora `nie`.
- [błąd] Gdy pola znikają już na poziomie 1, sprawdź, czy nowe bloki pozostały wewnątrz gotowego warunku z `Poziom = 2`.
- [gdy brakuje czasu] Najpierw wykonaj warunek, ustawienie stanu, `ukryj` i usunięcie. Animację dołóż po działającym teście rdzenia.

## [challenge] Dziecko balansuje arenę (10 min)

### Co robić teraz
- Dziecko wybiera prawdopodobieństwo: 1 z 3, 1 z 4 albo 1 z 5 pól. Przed testem przewiduje, który wariant jest najtrudniejszy.
- Dobiera zakres opóźnienia, np. 1-2, 1-4 albo 2-5 sekund, i sprawdza, czy gracz ma czas zauważyć zagrożenie.
- Zmienia tempo animacji w zakresie 0.05-0.15 sekundy. Po każdej zmianie uruchamia nową próbę, zamiast zmieniać wszystkie liczby naraz.
- Na końcu zapisuje jedną świadomą decyzję: „Wybrałem..., ponieważ...”.

### Wskazówki
- [dla szybszych] Dodaj krótki dźwięk ostrzegawczy przed animacją, nie naruszając ochrony diamentu.
- [podpowiedź] Trudność zależy jednocześnie od liczby znikających pól, czasu oczekiwania i czytelności ostrzeżenia.
- [gdy brakuje czasu] Pozostaw 1 z 4 oraz 1-4 sekundy; dziecko wybiera tylko tempo animacji.

## [demo] Test całej wersji dziecka (10 min)

### Co robić teraz
- Dziecko uruchamia grę od zielonej flagi i przechodzi oba poziomy bez pomocy prowadzącego.
- Sprawdźcie kryteria: poziom 1 działa jak wcześniej; na poziomie 2 znikają tylko niektóre pola; moment jest losowy; widać sześć zmian kostiumu; diament nie traci swojego pola; usuwany jest wyłącznie właściwy klon.
- Jeśli coś nie działa, dziecko nazywa brakujący efekt i czyta kod od zewnętrznego warunku do usunięcia klonu. Poprawiajcie jedną przyczynę i powtarzajcie krótki test.
- Nazwijcie rezultat: START dziecka ma już mechanikę widzianą w FINAL oraz własne ustawienie trudności.
- Zapisz projekt pod nową nazwą, np. `TNT-Arena-Kuba.sb3`, bez nadpisywania czystego START.

### Wskazówki
- [błąd] Jeśli w powtórkach nic nie znika, może to być poprawny wynik losowania. Wykonaj kilka prób, zanim zmienisz kod.
- [tempo] Stabilną wersję zapisz najpóźniej w 53. minucie.

## [summary] Pokaz, zapis i wariant awaryjny (7 min)

### Co robić teraz
- Jeśli projekt jest ukończony, dziecko na własnym ekranie pokazuje poziom 2, wskazuje warunek ochrony diamentu i wyjaśnia rolę `Stan pola`. FINAL pozostaje wyłącznie u instruktora.
- Poproś o dokończenie zdań: „Losowanie decyduje...”, „Warunek chroni...”, „Klon usuwa...”. Wskaż jedną decyzję dziecka dotyczącą trudności.
- [mów] Twój START ma teraz losową pułapkę z zabezpieczeniem nagrody i animowanym ostrzeżeniem — funkcjonalnie odtworzyłeś mechanikę z FINAL.
- Jeżeli zabrakło czasu albo wystąpiła awaria, najpierw zapisz częściowy START pod nazwą z imieniem i dopiskiem `NIEDOKONCZONE` oraz nazwij elementy, które już działają.
- Dopiero wtedy wyślij `10-TNT-Arena-FINAL.sb3` jako osobny materiał do uruchomienia po lekcji. Nie nadpisuj nim START i nie przedstawiaj FINAL jako pracy dziecka.
- Przypomnij nazwę zapisanego pliku i zadanie domowe; jeśli rodzica nie ma, dziecko robi pokaz prowadzącemu.

### Wskazówki
- [podpowiedź] Rodzicowi powiedz konkretnie, że dziecko połączyło dwa warunki, zmienną lokalną, losowanie i cykl życia klonu.
- [błąd] Nie wysyłaj FINAL przy pierwszym błędzie — to rozwiązanie końcowe tylko dla nieukończonej lekcji.
- [tempo] Po 55. minucie nie dodawaj nowej funkcji; priorytetem jest zapis i spokojne podsumowanie.
