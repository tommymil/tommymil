# Pokazowa: Mega Obby — Laserowy Wyścig
Rodzaj: pokazowa
Subject: Scratch
Level: Pokazowa — 8–10 lat
Czas: 60 min
Tags: Pokazowa, Scratch, Gra, Pętle, Czas, Animacja
Opis: Uczeń przechodzi gotowy trzysektorowy tor Obby i dodaje własną pulsującą bramę laserową.
Cel: Uczeń buduje powtarzalny rytm przeszkody za pomocą pętli, komunikatów i czasu oczekiwania.

### Po zajęciach dziecko potrafi
- opisać cykl `ostrzeżenie → laser aktywny → przerwa`
- użyć pętli i czasu oczekiwania do sterowania przeszkodą
- sprawdzić, czy przeszkoda jest trudna, ale możliwa do przejścia

### Przygotuj przed zajęciami
- `Mega-Obby-start.sb3`: kompletna trasa bez jednej bramy pulsującej
- `Mega-Obby-polprodukt.sb3`: duszek bramy i kostiumy gotowe, brak pętli
- `Mega-Obby-final.sb3`: trasa z bramą ucznia
- ustaw scenę na start, sprawdź wszystkie checkpointy i restart

#### Gotowe w projekcie START
- trzy sektory: fabryka, neonowy tunel i platformy nad lawą
- obracające i przesuwne lasery, ruchome platformy oraz strefy przyspieszenia
- dwa checkpointy, zegar, licznik upadków, rekord i efekt mety
- animacja trafienia, dźwięki ostrzeżeń i natychmiastowa kolejna próba

#### Mały mod uczestnika
Nowa brama laserowa przed metą: pół sekundy ostrzeżenia, faza aktywna i bezpieczne okno. Uczeń wybiera rytm, kolor oraz trudność.

### Zadanie domowe
- zaprojektuj na kartce drugi rytm bramy i oznacz moment, w którym można przebiec

## [intro] Start i bufor techniczny (7 min)

### Co robić teraz
- Sprawdź sterowanie, dźwięk i restart.
- [mów] Tor jest już pełną grą. Dzisiaj zostaniesz projektantem poziomu i dodasz ostatnią pułapkę przed metą.
- Zapytaj, czy uczeń woli szybkie czy logiczne tory przeszkód.

### Wskazówki
- [tempo] Uruchom grę najpóźniej w 4. minucie.
- [gdy brakuje czasu] Skróć rozmowę, nie demo.

## [demo] Grywalny efekt WOW (5 min)

### Co robić teraz
- Uczeń przechodzi pierwszy sektor, aktywuje checkpoint i celowo wpada w laser, aby zobaczyć natychmiastowy powrót.
- Pokaż metę i rekord na gotowej szybkiej próbie instruktora.
- [mów] Nie będziemy budować ruchu ani checkpointów — one już działają. Dodamy nową zasadę do gotowej trasy.

### Wskazówki
- [tempo] Jeśli tor jest trudny, użyj skrótu instruktorskiego do sektora 2.

## [concept] Rytm bezpiecznej pułapki (6 min)

### Co robić teraz
- Pokaż trzy kostiumy bramy: `ostrzeżenie`, `aktywny`, `wyłączony`.
- Ułóżcie słownie cykl i wybierzcie czas na reakcję.
- [mów] Gdyby laser był aktywny zawsze, nie byłby wyzwaniem, tylko ścianą.

### Materiały
- [kod] Cykl bramy | Scratch:
```text
kiedy otrzymam [START_GRY v]
zawsze
  zmień kostium na [ostrzeżenie v]
  czekaj (0.5) sekundy
  zmień kostium na [aktywny v]
  ustaw [LaserAktywny v] na (1)
  czekaj (0.8) sekundy
  zmień kostium na [wyłączony v]
  ustaw [LaserAktywny v] na (0)
  czekaj (1.2) sekundy
```

### Wskazówki
- [błąd] Sam kostium nie wyłącza kolizji; o zagrożeniu decyduje zmienna.
- [tempo] Nie omawiaj kodu innych przeszkód.

## [guided] Misja: Pulsująca Brama (17 min)

### Co robić teraz
- Uczeń umieszcza gotowy duszek w pustej ramie przed metą.
- Buduje pętlę trzech faz i ustawia `LaserAktywny` na 1 tylko podczas czerwonej fazy.
- Do istniejącej kolizji Gracza dodaje warunek: dotyka bramy **i** `LaserAktywny = 1`.
- Testuje: stanie w bramie podczas fazy bezpiecznej, trafienie w aktywnej, pełne przejście od checkpointu.

### Wskazówki
- [podpowiedź] Najpierw ustaw widoczne kostiumy, potem dodaj kolizję.
- [gdy brakuje czasu] Wczytaj półprodukt i pozwól uczniowi ułożyć trzy czasy.
- [dla szybszych] Nadaj `OSTRZEZENIE` 0.2 sekundy przed aktywacją i odtwórz dźwięk.

## [challenge] Personalizacja poziomu (13 min)

### Co robić teraz
- Uczeń wybiera kolor, rytm i położenie bramy.
- Wykonuje trzy próby; jeśli żadna nie kończy się sukcesem, wydłuża bezpieczne okno.
- Nadaje przeszkodzie własną nazwę wyświetlaną przy wejściu do sektora.

### Wskazówki
- [podpowiedź] Projektowanie gry obejmuje balans, nie tylko poprawny kod.
- [dla szybszych] Druga brama korzysta z odwrotnej wartości `LaserAktywny`.

## [challenge] Samodzielny krok (6 min)

### Co robić teraz
- Uczeń bez instrukcji zmienia jeden czas tak, by tor był odrobinę trudniejszy, ale nadal przechodni.
- Zapisz stabilny projekt przed finałem.

## [summary] Finał dla rodzica (6 min)

### Co robić teraz
- Uczeń zaczyna od ostatniego checkpointu, przechodzi swoją bramę i wpada na metę.
- [mów] Zaprogramowałeś rytm przeszkody i sam sprawdziłeś balans jak projektant gry.
- Uczeń wskazuje pętlę oraz zmienną aktywności.
