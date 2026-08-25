# Ceny detaliczne z Balia Technic

Wasze ceny powstały jako `hurt netto × 1,10 × 1,23` (patrz `tools/import-cennik`). Ta formuła
rozjeżdża się z detalem dostawcy w obie strony — na czterech sprawdzonych pozycjach było
od −18% do +64% — więc jednym mnożnikiem się tego nie załatwi. Trzeba wziąć ich ceny
pozycja po pozycji, a potem ustawić własny narzut w panelu (`/sklep/panel` → **Cennik**).

To narzędzie robi wyłącznie **dopasowanie**. Do sklepu nic nie zapisuje.

## 1. Pobierz katalog Balii

Otwórz w przeglądarce i zapisz każdą stronę jako plik `.json`:

```
https://baliatechnic.pl/wp-json/wc/store/v1/products?per_page=100&page=1
https://baliatechnic.pl/wp-json/wc/store/v1/products?per_page=100&page=2
...
```

Kolejne strony aż odpowiedź będzie pusta (`[]`). To WooCommerce Store API — ceny są tam
**już w groszach** (`prices.price` przy `currency_minor_unit: 2`), dokładnie w tej samej
jednostce, w której sklep je trzyma. Nic nie jest przeliczane, więc nie ma jak pomylić się o 100×.

## 2. Dopasuj

```bash
python tools/import-balia/import_balia.py \
  --nasze backend/src/AkHouse.Infrastructure/Persistence/Data/shop-catalog.json \
  --balia balia-1.json balia-2.json balia-3.json \
  --out tools/import-balia/wynik
```

Zamiast `shop-catalog.json` możesz podać odpowiedź `GET /api/admin/shop/products` zapisaną do
pliku — wtedy uwzględnisz też produkty dodane ręcznie w panelu.

Powstaną cztery pliki:

| Plik | Co z nim zrobić |
|---|---|
| `dopasowania-pewne.csv` | Przejrzeć pobieżnie. Idą do importu automatycznie. |
| `dopasowania-watpliwe.csv` | **Przeczytać wiersz po wierszu.** Nie wchodzą do importu, dopóki nie każesz. |
| `bez-dopasowania.csv` | Pozycje, których Balia nie ma. Zostają z ceną ustawianą ręcznie. |
| `base-prices.json` | Ładunek do wysłania do API. |

## 3. Przejrzyj wątpliwe

Kolumna **uwaga** z wpisem `podobny wariant obok` jest ważniejsza niż sam wynik. Oznacza, że druga
najlepsza kandydatka była niemal równie dobra — czyli trafiłeś na rodzinę wariantów. U Balii
„Wkład akrylowy okrągły” występuje w szarej, białej i beżowej perle po **3799 / 3899 / 3999 zł**:
nazwy różnią się jednym słowem, ceny dwiema stówami. Narzędzie w takiej sytuacji celowo odmawia
wyboru, zamiast strzelać.

Gdy CSV się zgadza, dołóż wątpliwe do importu:

```bash
python tools/import-balia/import_balia.py ... --uwzglednij-watpliwe
```

Albo popraw `base-prices.json` ręcznie — to zwykły JSON `{"prices": [{"slug", "basePriceGrosze"}]}`.

## 4. Wgraj do sklepu

### Najpierw: migracja

Przy pierwszym uruchomieniu backend zatrzyma się na `PendingModelChangesWarning` — model ma
zmiany (tabela kategorii i cennik), których nie pokrywa żadna migracja. To nie błąd kodu,
tylko brakujący krok. **Jedna** migracja obejmuje obie zmiany:

Z katalogu głównego projektu (`D:\moje\ak-house`), **jedną linią**:

```powershell
dotnet ef migrations add ShopCategoriesAndPricing --project backend/src/AkHouse.Infrastructure --startup-project backend/src/AkHouse.Api --output-dir Persistence/Migrations
```

Z katalogu `backend` ścieżki są krótsze:

```powershell
dotnet ef migrations add ShopCategoriesAndPricing --project src/AkHouse.Infrastructure --startup-project src/AkHouse.Api --output-dir Persistence/Migrations
```

> Nie łam tej komendy znakiem `^` — to składnia `cmd.exe`. W PowerShellu łamie się
> backtickiem `` ` ``, ale najprościej wkleić wszystko w jednej linii.

W wygenerowanym pliku mają być **wyłącznie** `CreateTable`, `AddColumn` i `CreateIndex`.
Jeśli zobaczysz `DropColumn`, `AlterColumn` albo `RenameColumn` — nie uruchamiaj tego na bazie
z danymi. Skrypt poniżej sprawdza to sam i przerywa.

**Najprościej — jedno uruchomienie robi wszystko** (migracja, build, start API, import, narzut):

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\import-balia\wgraj-ceny.ps1 -Narzut 8,5
```

Bez `-Narzut` ceny wejdą 1:1 jak u Balii, a narzut ustawisz później w panelu.
Skrypt zatrzymuje się na pierwszym błędzie i mówi, co poszło nie tak.

Ręcznie, gdyby skrypt zawiódł:

```bash
curl -X POST http://localhost:5033/api/admin/shop/pricing/base-prices \
  -H "X-Api-Key: <Admin:ShopApiKey>" \
  -H "Content-Type: application/json" \
  --data-binary @tools/import-balia/wynik/base-prices.json
```

Odpowiedź podaje `updated` i `unknownSlugs`. **Niepusta lista `unknownSlugs` znaczy, że
dopasowanie się rozjechało** — te slugi nie istnieją w sklepie. Import od razu przepuszcza nowe
ceny bazowe przez obowiązujący narzut, więc sklep nie zostaje ze starymi cenami.

## 5. Ustaw narzut

`/sklep/panel` → **Cennik**. Wpisujesz procent (przecinek działa: `-5,5`), widzisz podgląd na trzech
realnych pozycjach, klikasz raz — i tyle.

Narzut liczy się **zawsze od ceny detalicznej Balii**, nigdy od bieżącej. Dlatego kliknięcie tej
samej wartości drugi raz nic nie zmienia, a wpisanie `0` wraca dokładnie do ich cen. Pozycje bez
ceny bazowej (te z `bez-dopasowania.csv` i wszystko, co dodacie sami) zostają nietknięte.

## Kiedy Balia zmieni ceny

Powtórz kroki 1–4. Dopasowania się nie zmienią, więc jest to głównie pobranie plików i jedno
`curl`. Narzutu nie trzeba ruszać — nowe ceny bazowe wchodzą z nim automatycznie.

## Czego to nie robi

- **Nie pilnuje, czy Balia coś wycofała.** Pozycja, która zniknie z ich sklepu, zostanie u was
  ze starą ceną bazową. Widać to po malejącej liczbie `updated` między importami.
- **Nie dotyka nazw, opisów ani zdjęć.** Wyłącznie ceny.
- **Nie zna wariantów WooCommerce.** Produkt z wariantami wchodzi z ceną wariantu podstawowego.
