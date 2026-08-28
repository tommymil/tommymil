# Pokazowa: Sieć Podniebnych Baz
Rodzaj: pokazowa
Subject: Minecraft Education — MakeCode Python
Level: Pokazowa — 11–13 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Python, Funkcje, Współrzędne, Teleport
Opis: Uczeń eksploruje gotową sieć efektownych lokacji, a następnie dodaje czwarty bezpieczny portal.
Cel: Uczeń rozumie osie X/Y/Z i wywołuje funkcję budującą oraz teleportującą do wybranego punktu świata.

### Po zajęciach dziecko potrafi
- rozróżnić zmianę X, Y i Z
- użyć `world(x, y, z)` do wskazania stałego celu
- dopisać funkcję nowej lokacji i bezpieczny powrót do hubu

### Przygotuj przed zajęciami
- projekty `Siec-Baz-start`, `-polprodukt` i `-final` w Pythonie
- gotowy hub z czterema portalami i oznaczonymi osiami
- trzy ukończone lokacje: podniebna baza, kryształowa jaskinia, wieża burz
- START ma wszystkie teleporty, powroty, tytuły i bezpieczne platformy; czwarty portal jest nieaktywny
- wpisz prawdziwe współrzędne świata zamiast przykładowych i przetestuj każdą lokację po ponownym otwarciu

#### Gotowe w projekcie START
- centralny hub, trzy kompletne lokacje i portal powrotny w każdej z nich
- podniebna baza ma szkło, beacon i widok; jaskinia kryształy i skarb; wieża pogodę oraz balkon
- platforma celu zawsze powstaje przed teleportem
- komendy, tytuły, mapa kierunków oraz `reset`
- pełna wycieczka działa bez czwartej lokacji

#### Mały mod uczestnika
Czwarta lokacja, np. `Wyspa Smoka`: uczeń wybiera bezpieczne X/Y/Z, wywołuje gotowy generator platformy i rejestruje nową komendę. Obowiązkowa zmiana to jedna funkcja 5–7 linii.

### Zadanie domowe
- zaprojektuj piątą bazę i zapisz jej współrzędne, materiał oraz atrakcję

## [intro] Start i bufor techniczny (7 min)

### Co robić teraz
- Włącz współrzędne i sprawdź `hub` oraz `powrot`.
- [mów] Dziś nie zbudujemy pustego teleportu. Dostajesz całą sieć baz i dołączysz do niej własną lokację.
- Poproś ucznia o wskazanie, która oś odpowiada wysokości.

## [demo] Podróż WOW (5 min)

### Co robić teraz
- Odwiedź trzy lokacje skrótami, spędzając w każdej około 30 sekund.
- Pokaż beacon nad chmurami, skarb jaskini i burzę obserwowaną z bezpiecznej wieży.
- Wróć do hubu i stań przy nieaktywnym czwartym portalu.
- [mów] Ta brama jeszcze niczego nie zna. Ty wpiszesz jej cel.

### Wskazówki
- [tempo] To zwiastun, nie zwiedzanie wszystkich dekoracji.

## [concept] Adres w świecie ma trzy liczby (6 min)

### Co robić teraz
- Pokaż gotową funkcję `do_wiezy` i zmieniaj pojedynczo X, Y, Z na kopii testowej.
- Uczeń przewiduje kierunek zmiany.
- Porównaj `world` (stały adres) z `pos` (względem gracza).

### Materiały
- [kod] Nowa lokacja | Python:
```python
def do_wyspy():
    x = 140
    y = 105
    z = -60
    zbuduj_bezpieczny_cel(x, y, z, END_STONE)
    player.teleport(world(x, y, z))

player.on_chat("wyspa", do_wyspy)
```

## [guided] Misja: czwarty portal (17 min)

### Co robić teraz
- Uczeń kopiuje najmniejszą gotową funkcję lokacji i nazywa ją po swojemu.
- Wybiera X i Z co najmniej 40 bloków od istniejących baz oraz Y 80–120.
- Wywołuje `zbuduj_bezpieczny_cel` przed teleportem i wybiera materiał.
- Rejestruje nową komendę czatu.
- Testuje najpierw w Creative, wraca przez `hub` i dopiero potem włącza tryb docelowy.

### Wskazówki
- [błąd] Nigdy nie teleportuj przed zbudowaniem podłogi.
- [błąd] Użycie `pos` sprawi, że ten sam portal będzie prowadził gdzie indziej zależnie od miejsca startu.
- [gdy brakuje czasu] Półprodukt ma gotową funkcję; uczeń ustawia trzy współrzędne, materiał i nazwę.
- [dla szybszych] Dodaj beacon, skrzynię albo pierścień świateł generowany pętlą.

## [challenge] Tożsamość lokacji (13 min)

### Co robić teraz
- Uczeń wybiera nazwę, materiał podłogi, wysokość i jeden gotowy moduł dekoracji.
- Dodaje tytuł po teleportacji oraz wskazówkę powrotu.
- Porównuje odległość od hubu i sprawdza, czy lokacje nie nachodzą na siebie.

### Wskazówki
- [podpowiedź] Jedna mocna atrakcja jest lepsza niż niedokończonych pięć.

## [challenge] Samodzielny krok (6 min)

### Co robić teraz
- Uczeń sam zmienia jedną oś o 10 i przed testem przewiduje wynik.
- Przywraca docelową wartość i zapisuje projekt.

## [summary] Finał dla rodzica (6 min)

### Co robić teraz
- Uczeń startuje w hubie, uruchamia swój portal, pokazuje lokację i wraca.
- [mów] Dodałeś nowy adres do działającej sieci i zadbałeś, aby cel powstał przed teleportem.
- Uczeń wskazuje X/Y/Z, funkcję oraz linię rejestrującą komendę.
