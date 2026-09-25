# Nowe lekcje pokazowe - projekty START i FINAL

Ten katalog zawiera dziesięć konspektów gotowych do importu do Lesson Runnera oraz katalog
`projekty/` z dwudziestoma paczkami: po jednym START i FINAL do każdej pokazówki.

Zasada prowadzenia jest stała:

- instruktor otwiera FINAL i przez kilka minut pokazuje gotowy efekt;
- dziecko otrzymuje wyłącznie START;
- START jest kopią pełnego projektu z jednym wyciętym fragmentem i komentarzem
  `TU PRACUJE UCZEŃ`;
- obowiązkowa praca dziecka zajmuje 25 minut: 15 minut budowy z prowadzącym i 10 minut
  samodzielnej personalizacji;
- na koniec dziecko uruchamia swoją wersję i porównuje ją z FINAL.

## Mapa projektów

| Nr | Środowisko | Projekt | Fragment wycięty ze START |
|---:|---|---|---|
| 1 | Scratch | Łowca Skarbów | reakcja rzadkiej gwiazdy: blokada `aktywna`, +3 punkty, dźwięk, komunikat popupu i znikanie |
| 2 | Scratch | Mega Obby | ruch poziomego lasera `Laser2` |
| 3 | Scratch | Arena Walki | osiem pocisków Super Ataku dookoła gracza |
| 4 | Minecraft Blocks | Magiczna Forteca | fala zombie i szkieletów |
| 5 | Minecraft Blocks | Arena Łowców | Piorunowy Miecz i jego cooldown |
| 6 | Minecraft Blocks | Złota Piramida | bezpieczny zygzak na schodach z magmy |
| 7 | Minecraft Python | Agent Górnik | skaner złota i diamentu |
| 8 | Minecraft Python | Arena Żywiołów | lodowa barykada z lampami |
| 9 | Minecraft Python | Podniebna Baza | wyposażenie Skrzyni Smoka |
| 10 | Scratch | TNT Arena | losowe znikanie klonów TNT z ochroną pola diamentu |

## Podział Minecraft

Dokładnie trzy pary otwierają się w MakeCode Blocks:

- `04-Magiczna-Forteca-*`;
- `05-Arena-Lowcow-*`;
- `06-Zlota-Piramida-*`.

Dokładnie trzy pary otwierają się w MakeCode Python:

- `07-Agent-Gornik-*`;
- `08-Arena-Zywiolow-*`;
- `09-Podniebna-Baza-*`.

## Import konspektów

Pliki `l01-*.md` do `l10-*.md` można wgrywać pojedynczo przez ekran
**Konspekty -> Importuj**. Cały katalog sprawdza i importuje skrypt:

```powershell
cd frontend\lesson-runner-web
npm.cmd run import:lekcje -- --dir ..\..\docs\showcase --dry-run
```

Katalog `projekty/` nie jest importowany automatycznie. Po imporcie konspektu odpowiednia
paczkę START należy dodać do materiałów lekcji albo wysłać dziecku ustalonym kanałem.
FINAL pozostaje materiałem instruktora.

## Odtworzenie paczek

Paczki zostały wygenerowane z nowych projektów w `D:\moje\kodziaki-konspekty`.
Można je odtworzyć bez ręcznego edytowania archiwów:

```powershell
cd tools\showcase-projects
npm.cmd install
npm.cmd run build -- --source D:\moje\kodziaki-konspekty --output D:\moje\kodziaki\docs\showcase\projekty
```
