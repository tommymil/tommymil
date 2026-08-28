# 9 lekcji pokazowych programowania dla dzieci

## Cel programu

Lekcja pokazowa ma sprzedać wyobrażenie: **„takie gry będę umieć tworzyć”**. Nie jest pierwszą zwykłą lekcją kursu i nie może wyglądać jak pusty projekt, który dopiero po godzinie zaczyna działać.

Każdy uczestnik otwiera gotową, efektowną grę z dźwiękiem, informacją zwrotną, celem, przegraną i możliwością ponownej rozgrywki. Najpierw gra przez 2–4 minuty, potem poznaje jeden fragment kodu i dodaje niewielki, widoczny „mod”: nowego przeciwnika, pułapkę, falę, moc albo lokację.

**WOW → Zagraj → Odkryj jeden mechanizm → Dodaj własny mod → Pokaż rodzicowi**

## Standard projektu START i FINAL

W tym katalogu słowo `START` nie oznacza projektu niedokończonego. Oznacza kompletną grę **przed modem ucznia**.

Każdy projekt START musi mieć:

- natychmiastowy start bez instalacji i budowania podstaw od zera;
- jasny cel oraz czytelne sterowanie na ekranie;
- co najmniej trzy współdziałające systemy, np. ruch, przeciwnicy i wynik;
- efekt trafienia, zdobycia punktu, wygranej i przegranej;
- dźwięk lub cząsteczki/animacje, zmianę tempa oraz minimum jedną niespodziankę;
- przycisk lub komendę restartu;
- 2–4 minuty grywalnego demo;
- stabilną kopię zapasową.

Projekt FINAL to ten sam kompletny projekt z dołożonym modem ucznia i jednym opcjonalnym bonusem. Obowiązkowa misja ma obejmować 4–8 bloczków Scratch/MakeCode albo 3–8 linii Pythona. Uczeń nie naprawia projektu, nie przepisuje silnika i nie buduje całej planszy.

## Portfolio pokazówek

| Nr | Projekt | Co już działa w START | Mały mod uczestnika | Główny koncept |
|---:|---|---|---|---|
| 1 | Scratch: Minecraft TNT Runner | arena z zapadających kafli, lawa, życia, wynik, rekord, rosnące tempo | nowy rodzaj kafla: Magma TNT | losowanie i warunek |
| 2 | Scratch: Mega Obby — Laserowy Wyścig | trzy sektory, lasery, ruchome platformy, checkpointy, czas, zgony, meta | pulsująca brama laserowa | pętla i rytm czasu |
| 3 | Scratch: Star Arena | celowanie, strzelanie, fale, kilka typów wrogów, życie, dropy, boss | szybki wróg „Łowca” | klony i typ obiektu |
| 4 | Minecraft Blocks: Obrona Magicznej Fortecy | gotowa forteca, ekwipunek, trzy fale, punkty obrony, finał | dodatkowa fala pająków | pętla |
| 5 | Minecraft Blocks: Arena Łowców Potworów | arena, łuk, potwory, wynik, fale i boss | Piorunowy Miecz reagujący na użycie | zdarzenie |
| 6 | Minecraft Blocks: Agent — Ekspedycja po Kryształ | tunel, kopanie, światło, mosty, licznik i skarb | wykrywacz cennej rudy | `if` |
| 7 | Minecraft Python: Klątwa Złotej Piramidy | generowana świątynia, komnaty, pułapki, strażnicy i skarb | własny korytarz pułapek | `for` |
| 8 | Minecraft Python: Arena Żywiołów | fale, trzy klasy mocy, energia, arena i finał | czwarta moc przez `elif` | `if / elif / else` |
| 9 | Minecraft Python: Sieć Podniebnych Baz | hub, trzy gotowe bazy, bezpieczne teleporty, powrót | czwarta lokacja | funkcje i X/Y/Z |

## Co oznacza „dużo się dzieje”

Nie chodzi o przypadkowy chaos. Podczas pierwszych 90 sekund demo uczestnik powinien zobaczyć co najmniej pięć czytelnych zdarzeń, np. pojawienie się fali, reakcję przeciwnika, zmianę wyniku, drop, ostrzeżenie i finał. Instruktor nie pokazuje kodu w trakcie tego fragmentu.

W każdym projekcie przygotuj „moment zwiastuna”: boss w Star Arenie, nocny szturm na fortecę, piorunowy atak w Arenie Łowców albo teleport nad chmury. Nie uruchamiaj wszystkich niespodzianek w pierwszych dziesięciu sekundach.

# Część 1 — Scratch

## Lekcja 1: Minecraft TNT Runner

Gracz walczy o rekord na arenie z kafli, które migają i zapadają się. Ma trzy życia, zbiera emeraldy, omija lawę, a tempo rośnie co 15 sekund. Gotowy START zawiera HUD, rekord i restart. Uczeń dodaje losowany kafel `Magma TNT`, który ma inny kostium i znika dwa razy szybciej.

## Lekcja 2: Mega Obby — Laserowy Wyścig

Trzysektorowy tor zawiera obrotowe lasery, ruchome platformy, checkpointy, zegar, licznik upadków i efekt mety. Uczeń nie tworzy checkpointu potrzebnego do ukończenia gry — dodaje jedną pulsującą bramę laserową z własnym rytmem.

## Lekcja 3: Star Arena

Pełna gra top-down zawiera celownik, pociski-klony, fale dronów i tanków, pasek życia, wynik, apteczki, Super Atak i bossa. Uczeń dodaje nowy typ klona `Łowca`, który jest mniejszy, szybszy i wart więcej punktów.

# Część 2 — Minecraft Education + MakeCode Blocks

Kod można przygotować przez MakeCode Python, ale projekt zapisujemy po konwersji do Blocks. Uczeń podczas pokazówki widzi i zmienia wyłącznie bloczki.

## Lekcja 4: Obrona Magicznej Fortecy

Komenda buduje gotową fortecę z fosą i wieżami, przełącza gracza w tryb survival, daje łuk oraz uruchamia trzy fale potworów. Uczeń dopisuje krótką pętlę tworzącą dodatkową falę pająków. Forteca jest areną gry, a nie jedynym efektem.

## Lekcja 5: Arena Łowców Potworów

Gotowa arena ma bramy potworów, łuk, licznik pokonanych wrogów, fale zombie i szkieletów, dropy oraz finałowego ravagera. Uczeń dodaje zdarzenie użycia diamentowego miecza, które wywołuje piorun w bezpiecznej odległości. Zmienia jedną akcję i od razu widzi spektakularny efekt.

## Lekcja 6: Agent — Ekspedycja po Kryształ

Agent sam kopie korytarz, omija pustkę, stawia most, oświetla trasę i dociera do komnaty kryształu. Uczeń dodaje jeden warunek rozpoznający złoto lub diament przed zniszczeniem bloku i zwiększa licznik znalezisk.

# Część 3 — Minecraft Education + MakeCode Python

Projekty pozostają w widoku Python. Uczeń dostaje prawdziwy kod, lecz edytuje mały, oznaczony fragment. Silnik gry i generatory świata są gotowe.

## Lekcja 7: Klątwa Złotej Piramidy

Program generuje wielką piramidę z wejściem, korytarzem, komnatą, ukrytym skarbem, pułapkami i strażnikami. Uczeń używa krótkiej pętli `for`, aby dodać własny korytarz pułapek lub rząd świateł prowadzących do skarbu.

## Lekcja 8: Arena Żywiołów

Na gotowej arenie gracz odpiera fale i wybiera moc, stając na kolorowej runie. Ogień, teleport i leczenie już działają, podobnie jak energia i boss. Uczeń dodaje czwartą gałąź `elif`, np. lodową falę lub błyskawicę.

## Lekcja 9: Sieć Podniebnych Baz

Gotowy hub teleportuje do trzech kompletnych lokacji: podniebnej bazy, tajnej jaskini i wieży obserwacyjnej. Każda ma platformę, dekoracje i drogę powrotu. Uczeń wyznacza współrzędne i dopisuje czwartą funkcję lokacji.

# Ramowa struktura lekcji pokazowej — 60 minut

## 00:00–00:07 — Wejście i bufor techniczny

Sprawdź dźwięk, sterowanie i udostępnianie. Projekt jest już otwarty. Jeśli wszystko działa wcześniej, pozostały czas przeznacz na krótką rozmowę o ulubionych grach.

## 00:07–00:12 — Grywalny zwiastun WOW

Uczeń gra. Instruktor mówi tylko, jaki jest cel i jak sterować. Pokaż jedną niespodziankę z wersji FINAL, ale nie zdradzaj jej kodu.

## 00:12–00:18 — Dzisiejsza obietnica

Nazwij dokładnie jedną zmianę: „Dzisiaj dodasz własny rodzaj przeciwnika”, „zaprogramujesz nową falę” albo „otworzysz czwarty portal”. Pokaż 4–8 bloczków lub kilka linii odpowiedzialnych za podobny istniejący element.

## 00:18–00:35 — Mod uczestnika

Uczeń wykonuje obowiązkową zmianę i testuje ją po każdym małym kroku. Najpierw powstaje działająca wersja minimalna, dopiero potem wygląd, balans i nazwa.

## 00:35–00:48 — Personalizacja

Uczeń wybiera jedną cechę: szybkość, rodzaj moba, kolor, liczbę punktów, materiał, efekt albo pozycję. To ma być decyzja projektowa, nie kolejny duży system.

## 00:48–00:54 — Samodzielny challenge

Uczeń modyfikuje drugi parametr lub kopiuje poznany wzorzec. Challenge może pozostać niedokończony; działająca misja podstawowa nie może być zagrożona.

## 00:54–01:00 — Finał sprzedażowy

Zapisz stabilną wersję. Uczeń uruchamia pełną grę, wskazuje własny element i pokazuje go rodzicowi. Instruktor podsumowuje konkretnie: „Dodałeś nowy typ wroga za pomocą klonów i warunku”. Na koniec pokazuje 15 sekund projektu z rekomendowanej dalszej ścieżki.

## Zasady prowadzenia

- Projekt musi działać nawet bez modu ucznia.
- Nie omawiaj całego silnika. Gotowy kod jest scenografią, a nie materiałem do wykładu.
- Mysz i klawiatura pozostają po stronie ucznia od momentu rozpoczęcia misji.
- Jeśli po 25. minucie obowiązkowy mod nie działa, wczytaj przygotowany półprodukt i pozwól uczniowi dokończyć ostatnią decyzję.
- Nie rozpoczynaj instalacji, logowania ani budowania assetów podczas spotkania.
- Wersję START, półprodukt i FINAL sprawdź na tym samym koncie i urządzeniu przed zajęciami.

## Kryterium akceptacji pokazówki

Lekcja nadaje się do sprzedażowego użycia dopiero wtedy, gdy osoba niezwiązana z projektem potrafi po 60 sekundach powiedzieć: jaki jest cel gry, co jest niebezpieczne i co zmienia wynik. Po zajęciach dziecko musi umieć wskazać swój mod bez tłumaczenia instruktora.
