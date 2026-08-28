# Pokazowa: Agent — Ekspedycja po Kryształ
Rodzaj: pokazowa
Subject: Minecraft Education — MakeCode Blocks
Level: Pokazowa — 9–11 lat
Czas: 60 min
Tags: Pokazowa, Minecraft, Blocks, Agent, Gra, Warunki
Opis: Uczeń prowadzi gotową ekspedycję robota przez kopalnię i dodaje wykrywacz cennej rudy.
Cel: Uczeń stosuje warunek IF, aby Agent inaczej reagował na cenny blok niż na zwykłą przeszkodę.

### Po zajęciach dziecko potrafi
- odróżnić wykrywanie przeszkody od rozpoznawania rodzaju bloku
- zbudować warunek `jeżeli znaleziono rudę`
- zwiększyć licznik przed zniszczeniem bloku

### Przygotuj przed zajęciami
- światy `Agent-Ekspedycja-start`, `-polprodukt` i `-final` zapisane w Blocks
- gotowa kopalnia z trzema odcinkami: tunel, przepaść do zmostkowania, komnata kryształu
- w START działający pełny Agent: kopie, stawia pochodnie, buduje most i dociera do końca
- umieść po drodze złoto i diament, ale pozostaw wykrywacz uczniowi
- sprawdź komendy `start`, `ekspedycja`, `komnata` i `reset`

#### Gotowe w projekcie START
- fabularne wejście, mapa kopalni i widoczny cel: odzyskać Kryształ Rdzenia
- Agent kopie tunel 3×2, stawia pochodnie co cztery kroki i buduje most nad oznaczoną przepaścią
- gracz idzie za robotem; pojawiają się komunikaty etapów, licznik odległości i finałowa komnata
- restart ustawia Agenta oraz odbudowuje testowy odcinek
- projekt jest ukończoną misją nawet bez liczenia rud

#### Mały mod uczestnika
`Skaner skarbów`: przed zniszczeniem bloku Agent sprawdza, czy to złoto/diament. Jeśli tak, wyświetla alarm i zwiększa `Skarby`. To jeden warunek w gotowej pętli.

### Zadanie domowe
- wymyśl trzeci blok i inną reakcję Agenta, np. zatrzymanie przy lawie

## [intro] Start i bufor techniczny (7 min)

### Co robić teraz
- Sprawdź ustawienie Agenta i komendę `reset`.
- [mów] Robot umie już przejść całą ekspedycję. Ty dodasz mu czujnik, dzięki któremu nie przeoczy skarbu.
- Zapytaj, gdzie w prawdziwym robocie przydałby się warunek.

## [demo] Ekspedycja WOW (5 min)

### Co robić teraz
- Uruchom skróconą trasę: Agent kopie, zapala światło i buduje trzy bloki mostu.
- Pokaż zamknięte drzwi komnaty oraz błysk Kryształu Rdzenia.
- [mów] Wszystkie ruchy ekspedycji są gotowe. Brakuje jednej inteligentnej decyzji: rozpoznania cennego bloku.

### Wskazówki
- [tempo] Demo maksymalnie 5 minut; pełną trasę zostaw na finał.

## [concept] Najpierw spójrz, potem działaj (6 min)

### Co robić teraz
- Porównaj `agent wykrywa blok z przodu` z `agent sprawdza rodzaj bloku z przodu`.
- Uczeń układa kolejność: sprawdź rudę → dodaj punkt → zniszcz → idź.
- [mów] Gdyby Agent najpierw zniszczył blok, nie miałby już czego rozpoznać.

### Materiały
- [kod] Skaner | MakeCode Blocks:
```text
jeżeli <(blok z przodu) = [blok złota]> to
  zmień [Skarby] o (1)
  powiedz „SKARB!”
jeżeli <agent wykrywa blok z przodu> to
  agent niszczy z przodu
```

## [guided] Misja: Skaner skarbów (17 min)

### Co robić teraz
- Uczeń tworzy zmienną `Skarby` i zeruje ją na początku ekspedycji.
- W gotowej pętli kopania, przed niszczeniem, dodaje warunek dla złota.
- Wewnątrz zwiększa licznik i wyświetla komunikat.
- Testuje na krótkim odcinku: kamień nie zmienia wyniku, złoto zmienia go dokładnie raz.
- Kopiuje warunek dla diamentu albo rozszerza go operatorem LUB.

### Wskazówki
- [błąd] Skaner po niszczeniu zawsze zobaczy powietrze.
- [błąd] Komunikat poza warunkiem będzie pojawiał się przy każdym bloku.
- [gdy brakuje czasu] Półprodukt ma gotowy warunek; uczeń wybiera blok i reakcję.
- [dla szybszych] Złoto daje 1 punkt, diament 3.

## [challenge] Zachowanie specjalne (13 min)

### Co robić teraz
- Uczeń wybiera: zatrzymanie na sekundę, położenie glowstone obok znaleziska albo osobny dźwięk/komunikat.
- Uruchamia odcinek z dwoma rudami i sprawdza wynik końcowy.
- Nadaje skanerowi własną nazwę.

### Wskazówki
- [dla szybszych] Warunek dla lawy zatrzymuje Agenta i prosi gracza o decyzję, ale nie zmienia świata automatycznie.

## [challenge] Samodzielny krok (6 min)

### Co robić teraz
- Uczeń sam dodaje drugi materiał albo zmienia jego wartość punktową.
- Zapisz i odbuduj trasę komendą `reset`.

## [summary] Finał dla rodzica (6 min)

### Co robić teraz
- Uruchom pełną ekspedycję. Uczeń idzie za Agentem, pokazuje skaner i otwarcie komnaty.
- [mów] Robot nie wykonuje już tylko rozkazów — podejmuje inną decyzję zależnie od tego, co widzi.
- Uczeń wskazuje kolejność sprawdzenia i niszczenia.
