# Roadmap sklepu A.K. HOUSE

Stan po przeglądzie UX z 25 sierpnia 2026. P0 i P1 są zakresem bieżącego wdrożenia. Poniższe
punkty są świadomie odłożone — nie powinny blokować uruchomienia testowego sklepu.

## P2 — wzrost i odnajdywanie oferty

- SEO produktowe: osobne adresy i metadane wariantów, mapa produktów generowana z bazy,
  prerender stron sklepu oraz poprawne dane `Product`, `Offer` i `BreadcrumbList`.
- Feed produktowy do Google Merchant Center / Meta po uporządkowaniu GTIN, SKU, marki,
  stanów dostępności i kosztów dostawy.
- Zweryfikowane opinie posprzedażowe, bez importowania anonimowych ocen dostawcy.
- Poradniki zakupowe i porównania (dobór pieca, wkładu, izolacji, pokrywy i chemii), linkowane
  z kategorii i kart produktów.
- Wyszukiwarka części zamiennych po modelu urządzenia, wymiarze i kompatybilności.
- Automatyzacje e-mail: porzucony koszyk, przypomnienie o płatności, prośba o opinię i materiały
  eksploatacyjne — dopiero po ustaleniu podstawy prawnej, częstotliwości i wypisu.
- Kontrolowana synchronizacja dostawcy: cena, dostępność i wycofania z raportem zmian przed
  publikacją, zamiast bezwarunkowego nadpisywania danych sklepu.

## P3 — sprzedaż projektowa i obsługa posprzedażowa

- Pełne modele premium z wariantami, zakresem ceny i jasnym rozdzieleniem „kup online” od
  „zamów wycenę”.
- Porównywarka 2–4 produktów oraz zapisane listy klienta.
- Rozbudowane case studies: koszt, zakres, termin, lokalizacja, transport i zdjęcia z realizacji.
- Ankieta działki / miejsca montażu z możliwością dodania zdjęć i wymiarów.
- Kalendarz konsultacji i wizji lokalnej, po wyborze docelowego narzędzia dla zespołu.
- Finansowanie, gwarancje i pakiety serwisowe dopiero po podpisaniu umów i zatwierdzeniu
  warunków — bez marketingowych obietnic wpisanych „na zapas”.

## Konfigurator 3D — daleka przyszłość

Kod prototypu pozostaje w repozytorium, ale funkcja ma pozostać niewidoczna dla wszystkich:
bez trasy, linków, sekcji na stronie i ładowania paczki WebGL. Powrót do tematu wymaga osobnej
decyzji, modelu danych wariantów, reguł wyceny CPQ, budżetu wydajnościowego i testów na telefonach.
Do tego czasu rolę doboru wariantu pełni lekki kreator SVG oraz formularz wyceny.

