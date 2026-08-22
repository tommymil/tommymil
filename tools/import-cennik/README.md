# Import cennika hurtowego do sklepu

Zamienia arkusz `Cennik hurtowy.xlsx` od dostawcy (Balia Technic) na dane katalogu sklepu:

- `backend/src/AkHouse.Infrastructure/Persistence/Data/shop-catalog.json` — pozycje,
- `backend/src/AkHouse.Infrastructure/Persistence/Data/CatalogMedia/*.webp` — zdjęcia.

Oba katalogi są zasobami osadzonymi w `AkHouse.Infrastructure`. Przy starcie API
`ShopCatalogSeed` dokłada brakujące pozycje do bazy i wykłada zdjęcia do katalogu uploadów.

## Uruchomienie

```bash
python -m pip install openpyxl pillow
python tools/import-cennik/import_cennik.py "C:/ścieżka/Cennik hurtowy.xlsx"
```

Przydatne przełączniki: `--skip-media` (sam JSON, bez konwersji zdjęć), `--out-json`, `--out-media`.

Po imporcie przebuduj backend — pliki wchodzą do podzespołu jako zasoby:

```bash
cd backend && dotnet build && dotnet test
```

## Ceny

Arkusz podaje ceny **hurtowe netto**. Cena w sklepie to `netto × 1,10 × 1,23` — 10% marży
A.K. HOUSE i 23% VAT — zaokrąglona do pełnych groszy. Stawki są stałymi `MARGIN` i `VAT`
na górze skryptu; zmiana wymaga ponownego wygenerowania pliku.

## Czego cennik nie zawiera

- **Kodów produktów.** Kluczem jest slug wyprowadzony z nazwy, więc zmiana nazwy w kolejnej
  wersji cennika utworzy nową pozycję zamiast zaktualizować istniejącą. Jeśli dostawca doda
  kolumnę z indeksem, warto przepiąć import na nią.
- **Stanów magazynowych.** Każda pozycja wchodzi ze stanem `DEFAULT_STOCK` (10 szt.);
  oznaczone w arkuszu jako niedostępne wchodzą nieopublikowane i z zerem.
- **Jednostek sprzedaży.** Pozycje rozliczane na metry i komplety mają jednostkę dopisaną do
  nazwy i do opisu („cena za 1 m²"), bo `ShopProduct` nie ma osobnego pola na jednostkę.

## Powtórny import

`ShopCatalogSeed` pomija pozycje, których slug już jest w bazie — nie nadpisze zmian z panelu
administracyjnego. Żeby wczytać katalog od nowa, trzeba najpierw usunąć produkty w panelu
(albo wyczyścić tabele `ShopProducts` i `ShopProductImages` w bazie deweloperskiej).

## Mapowanie kategorii

Sekcje arkusza są przypisane do `ShopCategory` w słowniku `SECTION_CATEGORIES`. Nagłówki
w cenniku bywają mylące — dwie sekcje nazywają się „Węże", a druga „Odpływy" zawiera obejmy —
więc mapowanie jest ręczne, a pojedyncze wiersze można nadpisać przez `ROW_CATEGORIES`.
Nowa sekcja w kolejnej wersji cennika zgłosi się jako uwaga na końcu działania skryptu.
