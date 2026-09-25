# Pokazowa: Agent Górnik - Skaner Skarbów
Rodzaj: pokazowa
Subject: Minecraft Education - MakeCode Python
Level: Pokazowa - 10-12 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Python, Agent, Warunki, Zmienne
Opis: Podczas lekcji zdalnej dziecko najpierw uruchamia własnego Minecrafta, potem ogląda FINAL tylko na ekranie instruktora. Po pokazie otrzymuje START, testuje Agenta bez punktowania skarbów i w MakeCode Python programuje skaner rozpoznający złoto oraz diament przed wykopaniem bloku.
Cel: Dziecko używa dwóch warunków oraz inspekcji bloku Agenta, aby różnie punktować znalezione skarby.

### Po zajęciach dziecko potrafi
- odczytać blok znajdujący się przed Agentem
- porównać wynik inspekcji z wybranym rodzajem bloku
- zwiększyć licznik o różną wartość zależnie od skarbu

### Przygotuj przed zajęciami
- na swoim komputerze zaimportuj i przetestuj `projekty/07-Agent-Gornik-FINAL.mkcd`; START miej gotowy, ale nie wysyłaj go przed pokazem
- dziecko najpierw udostępnia cały ekran, uruchamia Minecraft Education, płaski świat i MakeCode; potem instruktor pokazuje FINAL na swoim ekranie
- dopiero po pokazie FINAL dziecku wyślij tylko `projekty/07-Agent-Gornik-START.mkcd`; dziecko samodzielnie importuje go i pracuje w MakeCode Python
- nie przełączaj na Blocks; sprawdź komendy `arena`, `start`, `test`, `komnata` i `reset`
- w START odnajdź `sprawdz_skarb()`, gotowe `global skarby`, `pass` i komentarz `TU PRACUJE UCZEŃ`
- zachowaj czysty START; FINAL przesyłaj dopiero pod koniec jako oddzielny wariant awaryjny po zapisaniu częściowego kodu

### Zadanie domowe
- dopisz na kartce trzeci warunek dla bloku żelaza wartego dwa punkty

## [intro] Połączenie i Minecraft dziecka (5 min)

### Co robić teraz
- Dziecko udostępnia ekran, uruchamia Minecrafta, Code Builder i MakeCode. Sprawdź czat, klawiaturę oraz widoczność kodu Python.
- Nie wysyłaj jeszcze pliku. Zapytaj, co Agent powinien zrobić przed zniszczeniem bloku, jeśli chce rozpoznać jego rodzaj.
- [mów] Najpierw zobaczysz gotową wyprawę na moim ekranie. Potem w Twoim START Agent nadal będzie kopał, ale nie rozpozna skarbów, dopóki nie napiszesz skanera.
- Przełącz udostępnianie na ekran instruktora z FINAL, bez pokazywania kodu.

### Wskazówki
- [błąd] W normalnej misji jest tylko jeden teleport gracza; po nim trzeba iść za Agentem.
- [podpowiedź] Udostępniaj cały ekran, bo dziecko będzie przełączać się między Pythonem, czatem i Agentem.
- [tempo] Najpóźniej w 4. minucie rozpocznij FINAL.

## [demo] FINAL wyłącznie na ekranie instruktora (7 min)

### Co robić teraz
- Instruktor uruchamia `arena` i `start`, a dziecko obserwuje ruch Agenta i prowadzi wyprawę głosem.
- Zatrzymaj uwagę przy komunikatach o złocie i diamencie: złoto dodaje 1, diament 3, razem dając klucz 4/4.
- Pokaż świecący most i otwarcie bramy krypty. Jeśli trasa jest za długa, użyj `test` do szybkiego porównania obu bloków.
- [mów] W START Agent wykopie te same bloki, ale licznik i efekty nie zareagują. Napiszesz dwie decyzje wykonywane tuż przed kopaniem.
- Po pokazie wyślij tylko START i poproś dziecko o ponowne udostępnienie całego ekranu.

### Wskazówki
- [tempo] Jeśli pełna trasa trwa za długo, komenda `test` przygotowuje techniczny test skanera.
- [podpowiedź] Gdy dziecko pyta, skąd Agent zna blok, zapowiedz `agent.inspect` jako „spojrzenie” bez niszczenia.
- [błąd] Nie pokazuj kodu FINAL ani nie przesyłaj go razem ze START.

## [concept] Dlaczego START nie liczy skarbów? (6 min)

### Co robić teraz
- Dziecko importuje START i uruchamia `test`. Agent wykopuje przygotowane bloki, ale licznik i efekty nie zmieniają się — to oczekiwany brak.
- [mów] Mechanika kopania działa. Pusta funkcja skanera nie podejmuje jeszcze decyzji dla złota ani diamentu.
- Dziecko odnajduje `sprawdz_skarb()`, `global skarby`, `pass` i komentarz.
- Pokaż, gdzie `kop_tunel()` wywołuje skaner przed `agent.destroy(FORWARD)`.
- Dziecko przewiduje, dlaczego skan po zniszczeniu bloku zawsze widziałby już powietrze.

### Materiały
- Zakres pracy dziecka: dwa samodzielne warunki i po trzy instrukcje reakcji dla złota oraz diamentu.
- Gotowe w START: funkcja `sprawdz_skarb()`, deklaracja `global skarby`, wywołania efektów i miejsce wywołania skanera przed kopaniem.
- [kod] Funkcja „sprawdz_skarb()” — rozpoznawanie złota i diamentu | Python:
```python
# MIEJSCE: funkcja sprawdz_skarb(), pod „global skarby”, zamiast „pass”
if agent.inspect(AgentInspection.BLOCK, FORWARD) == GOLD_BLOCK:
    skarby += 1
    efekt_zlota()
    player.say("Skarby: " + str(skarby) + " / 4")

if agent.inspect(AgentInspection.BLOCK, FORWARD) == DIAMOND_BLOCK:
    skarby += 3
    efekt_diamentu()
    player.say("Skarby: " + str(skarby) + " / 4")
```

### Wskazówki
- [podpowiedź] `inspect(..., FORWARD)` odczytuje blok przed Agentem; nie przesuwa go i nie niszczy bloku.
- [podpowiedź] Dwa niezależne `if` są czytelne, bo jeden blok nie może jednocześnie być złotem i diamentem.
- [błąd] `global skarby` pozostaje na początku funkcji, a `pass` usuń po dodaniu prawdziwych instrukcji.

## [guided] Programujemy skaner (15 min)

### Co robić teraz
- Cały kod wpisuje dziecko w MakeCode Python na własnym udostępnionym ekranie. Prowadzący dyktuje krótkie fragmenty dopiero po pytaniu o ich cel.
- Dziecko usuwa `pass`, wpisuje warunek dla `GOLD_BLOCK`, zwiększa `skarby` o 1, wywołuje `efekt_zlota()` i składa komunikat licznika.
- Uruchamia `test` i sprawdza najpierw tylko złoto. Jeśli działa, kopiuje cały warunek na tym samym poziomie wcięcia.
- W kopii zmienia blok na `DIAMOND_BLOCK`, wartość na 3 i efekt na `efekt_diamentu()`, po czym testuje oba wykrycia.
- Na końcu dziecko czyta funkcję własnymi słowami: „jeśli przed Agentem jest..., to...”.

### Wskazówki
- [podpowiedź] Oba `if` mają takie samo wcięcie; to dwa niezależne rodzaje skarbów.
- [podpowiedź] W MakeCode podświetlenie błędu często wskazuje linię poniżej prawdziwej przyczyny; sprawdź najpierw dwukropek i nawiasy w poprzedniej linii.
- [błąd] Bez `global skarby` przypisanie może utworzyć lokalną wartość zamiast zmienić licznik misji.
- [błąd] Jeśli komunikat działa, ale wynik nie rośnie, sprawdź `+=`, nazwę `skarby` i wcięcie wewnątrz właściwego `if`.
- [błąd] Jeśli Agent widzi tylko powietrze, upewnij się, że `sprawdz_skarb()` nadal jest wywoływane przed `agent.destroy(FORWARD)`.
- [gdy brakuje czasu] Złoto jest obowiązkowe; diament można dokończyć przez skopiowanie gotowej gałęzi.

## [challenge] Własna punktacja ekspedycji (10 min)

### Co robić teraz
- Dziecko wybiera, czy diament ma być wart 2, 3 czy 4 punkty.
- Zmienia komunikat, aby zgadzał się z nową wartością.
- Przewiduje sumę złota i diamentu, a następnie testuje, czy misja osiąga wymagane co najmniej 4 skarby.
- Uzasadnia, czy rzadszy skarb powinien być wart więcej.

### Wskazówki
- [dla szybszych] Dodaj trzeci warunek dla `IRON_BLOCK` i przyznaj dwa punkty.
- [gdy brakuje czasu] Pozostaw punktację 1 i 3, a dziecko personalizuje tylko jeden komunikat.

## [demo] Pełna trasa z własnym skanerem (10 min)

### Co robić teraz
- Dziecko wpisuje `reset`, `arena` i `start`, a następnie podąża za Agentem.
- Sprawdźcie kryteria: złoto rozpoznane przed kopaniem daje właściwą liczbę punktów i efekt; diament robi to samo; licznik osiąga próg; brama krypty się otwiera.
- Przy błędzie dziecko rozdziela problem na: odczyt bloku, zmianę zmiennej, efekt lub komunikat i testuje tylko ten fragment komendą `test`.
- Nazwijcie rezultat: START ma skaner z FINAL oraz własną decyzję punktową lub komunikat.
- Zapisz projekt pod nazwą z imieniem dziecka, bez nadpisywania czystego START.

### Wskazówki
- [tempo] Nie trzeba kończyć całej animacji nagrody, jeśli skaner i brama zadziałały.
- [błąd] Po edycji kodu uruchom nowy test; wynik poprzedniej wyprawy może ukryć problem z aktualną wersją.

## [summary] Pokaz, zapis i wariant awaryjny (7 min)

### Co robić teraz
- Jeśli START jest ukończony, dziecko uruchamia `test`, pokazuje oba wykrycia i wskazuje dwa warunki oraz różne wartości punktów. FINAL pozostaje u instruktora.
- [mów] Napisałeś skaner, który podejmuje decyzję na podstawie bloku przed Agentem. Twój START odtwarza teraz rozpoznawanie skarbów z FINAL.
- Zapytaj, dlaczego `sprawdz_skarb()` działa przed kopaniem i do czego służy `global skarby`.
- Jeśli zabrakło czasu lub wystąpiła awaria, najpierw zapisz częściowy START z dopiskiem `NIEDOKONCZONE` i nazwij działające gałęzie.
- Dopiero później wyślij `07-Agent-Gornik-FINAL.mkcd` jako osobny materiał do uruchomienia. Nie nadpisuj nim kodu dziecka ani nie przedstawiaj FINAL jako jego pracy.
- Przypomnij nazwę zapisanego projektu i zadanie domowe.

### Wskazówki
- [podpowiedź] Rodzicowi wskaż konkretnie: inspekcja bloku, dwa warunki, różna punktacja, funkcje efektów i test przed pełną misją.
- [błąd] Nie wysyłaj FINAL przy pierwszym błędzie składni; najpierw sprawdź wcięcia, nawiasy i dwukropki.
- [tempo] Po 55. minucie nie dodawaj trzeciego skarbu; zabezpiecz plik i wykonaj pokaz.
