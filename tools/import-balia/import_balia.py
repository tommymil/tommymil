#!/usr/bin/env python3
"""
Dopasowuje asortyment A.K. HOUSE do cen detalicznych Balia Technic.

Nazwy po obu stronach pochodzą z różnych źródeł — nasze z arkusza hurtowego, ich z opisów
w sklepie — więc slugi się nie zgadzają i literówki są po obu stronach. Skrypt **niczego nie
zapisuje do sklepu**: produkuje listę do przejrzenia i dopiero zaakceptowana część idzie do API.
To celowe. Zła cena u klienta jest droższa niż dziesięć minut czytania listy.

Wejście
-------
  --nasze     JSON z naszymi pozycjami: lista obiektów z `slug` i `name`.
              Pasuje zarówno `shop-catalog.json`, jak i odpowiedź `GET /api/admin/shop/products`.
  --balia     Jeden lub więcej plików zapisanych z ich Store API, np.
              https://baliatechnic.pl/wp-json/wc/store/v1/products?per_page=100&page=1

Wyjście (katalog --out)
-----------------------
  dopasowania-pewne.csv     — do zaakceptowania hurtem
  dopasowania-watpliwe.csv  — do przejrzenia wiersz po wierszu
  bez-dopasowania.csv       — nasze pozycje, których u nich nie ma
  base-prices.json          — ładunek dla POST /api/admin/shop/pricing/base-prices
                              (domyślnie tylko pewne; --uwzglednij-watpliwe dokłada resztę)

Użycie
------
  python tools/import-balia/import_balia.py \\
      --nasze backend/src/AkHouse.Infrastructure/Persistence/Data/shop-catalog.json \\
      --balia balia-1.json balia-2.json balia-3.json \\
      --out tools/import-balia/wynik
"""

from __future__ import annotations

import argparse
import csv
import json
import re
import sys
import unicodedata
from dataclasses import dataclass
from difflib import SequenceMatcher
from pathlib import Path

# Powyżej tego progu przyjmujemy dopasowanie bez pytania. Dobrane tak, żeby przeszły przypadki
# typu „wytłoczeniemi" vs „wytłoczeniami" i różna kolejność członów, a nie przeszły pozycje
# różniące się rozmiarem albo kolorem — te wyglądają podobnie, a mają inną cenę.
PEWNE = 0.86
# Nisko celowo. Wasze nazwy pochodzą z arkusza hurtowego i niosą wymiary („Kwadrat 172 cm x 202 cm"),
# słowo „komplet" i myślniki, których w sklepie Balii nie ma — poprawne pary lądują przez to nisko
# („Regulator hydromasażu" vs „Regulator do hydromasażu AISI 316" to 0,61). Do importu i tak nic
# nie wchodzi bez akceptacji, więc lepiej pokazać za dużo kandydatów niż zgubić trafienie.
WATPLIWE = 0.50

# Najgroźniejszy przypadek to nie słabe dopasowanie, tylko dwa dobre. „Wkład akrylowy okrągły
# szara perła" (3799 zł) i „…z wytłoczeniami pod dysze szara perła" (3899 zł) różnią się dwoma
# słowami i stówą. Jeśli druga najlepsza kandydatka depcze pierwszej po piętach, trafia do
# przejrzenia niezależnie od wyniku — wysoka punktacja nie znaczy, że wybór był jednoznaczny.
MINIMALNA_PRZEWAGA = 0.08

# Człony czysto techniczne, które w jednym cenniku są, a w drugim nie, i nic nie wnoszą
# do tożsamości produktu.
SZUM = {"szt", "kpl", "mb", "cm", "mm", "m", "kw", "kg", "l", "do", "z", "na", "i", "w", "pod", "od"}


def bez_ogonkow(text: str) -> str:
    text = text.replace("ł", "l").replace("Ł", "L")
    rozlozone = unicodedata.normalize("NFD", text)
    return "".join(c for c in rozlozone if unicodedata.category(c) != "Mn")


def normalizuj(nazwa: str) -> str:
    """Nazwa sprowadzona do postaci porównywalnej: bez ogonków, wielkości liter i interpunkcji."""
    plaska = bez_ogonkow(nazwa).lower()
    plaska = re.sub(r"[^a-z0-9]+", " ", plaska)
    return re.sub(r"\s+", " ", plaska).strip()


def tokeny(nazwa: str) -> set[str]:
    return {t for t in normalizuj(nazwa).split() if t not in SZUM}


def podobienstwo(a: str, b: str) -> float:
    """
    Łączy dwie miary, bo każda z osobna myli się inaczej:
    - Jaccard na tokenach radzi sobie z inną kolejnością członów, ale literówka to dla niego
      zupełnie inne słowo;
    - SequenceMatcher wybacza literówki, ale gubi się przy przestawionych członach.
    Bierzemy średnią ważoną — tokeny ważniejsze, bo to one niosą znaczenie.
    """
    ta, tb = tokeny(a), tokeny(b)
    jaccard = len(ta & tb) / len(ta | tb) if ta | tb else 0.0
    sekwencja = SequenceMatcher(None, normalizuj(a), normalizuj(b)).ratio()
    return 0.6 * jaccard + 0.4 * sekwencja


@dataclass
class Pozycja:
    slug: str
    nazwa: str


@dataclass
class Oferta:
    slug: str
    nazwa: str
    grosze: int


@dataclass
class Dopasowanie:
    nasza: Pozycja
    ich: Oferta | None
    wynik: float
    przewaga: float = 1.0
    """Ile najlepsza kandydatka wygrywa z drugą. Mała przewaga = wybór był niejednoznaczny."""

    @property
    def niejednoznaczne(self) -> bool:
        return self.przewaga < MINIMALNA_PRZEWAGA


def wczytaj_nasze(sciezka: Path) -> list[Pozycja]:
    dane = json.loads(sciezka.read_text(encoding="utf-8"))
    lista = dane.get("products", dane) if isinstance(dane, dict) else dane
    pozycje = [
        Pozycja(slug=str(p["slug"]), nazwa=str(p["name"]))
        for p in lista
        if p.get("slug") and p.get("name")
    ]
    if not pozycje:
        raise SystemExit(f"Nie znalazłem pozycji ze `slug` i `name` w {sciezka}")
    return pozycje


def wczytaj_balie(sciezki: list[Path]) -> list[Oferta]:
    """
    Czyta odpowiedzi WooCommerce Store API. Ceny są tam już w groszach (`prices.price` przy
    `currency_minor_unit: 2`), więc nic nie przeliczamy — i nie ma jak pomylić się o 100x.
    """
    oferty: dict[str, Oferta] = {}
    for sciezka in sciezki:
        surowe = json.loads(sciezka.read_text(encoding="utf-8"))
        produkty = surowe.get("products", surowe) if isinstance(surowe, dict) else surowe
        if not isinstance(produkty, list):
            raise SystemExit(f"{sciezka}: spodziewałem się listy produktów")

        for p in produkty:
            ceny = p.get("prices") or {}
            surowa_cena = ceny.get("price")
            jednostka = ceny.get("currency_minor_unit", 2)
            if surowa_cena in (None, "", "0"):
                continue
            if jednostka != 2:
                raise SystemExit(
                    f"{sciezka}: nieoczekiwany currency_minor_unit={jednostka}. "
                    "Ceny mogą nie być w groszach — przerywam, zamiast zgadywać."
                )
            oferty[str(p["slug"])] = Oferta(
                slug=str(p["slug"]), nazwa=str(p["name"]), grosze=int(surowa_cena)
            )

    if not oferty:
        raise SystemExit("Pliki z Balii nie zawierają żadnych produktów z ceną.")
    return list(oferty.values())


def dopasuj(nasze: list[Pozycja], ich: list[Oferta]) -> list[Dopasowanie]:
    wyniki: list[Dopasowanie] = []
    for pozycja in nasze:
        oceny = sorted(
            ((podobienstwo(pozycja.nazwa, oferta.nazwa), oferta) for oferta in ich),
            key=lambda para: -para[0],
        )
        if not oceny:
            wyniki.append(Dopasowanie(pozycja, None, 0.0))
            continue

        najlepszy_wynik, najlepsza = oceny[0]
        drugi_wynik = oceny[1][0] if len(oceny) > 1 else 0.0
        wyniki.append(Dopasowanie(pozycja, najlepsza, najlepszy_wynik, najlepszy_wynik - drugi_wynik))
    return wyniki


def zapisz_csv(sciezka: Path, dopasowania: list[Dopasowanie]) -> None:
    with sciezka.open("w", encoding="utf-8-sig", newline="") as f:
        pisarz = csv.writer(f, delimiter=";")
        pisarz.writerow(
            ["wynik", "uwaga", "nasz slug", "nasza nazwa", "ich nazwa", "ich cena (zl)", "ich slug"])
        for d in sorted(dopasowania, key=lambda x: -x.wynik):
            pisarz.writerow([
                f"{d.wynik:.2f}",
                "podobny wariant obok" if d.niejednoznaczne and d.ich else "",
                d.nasza.slug,
                d.nasza.nazwa,
                d.ich.nazwa if d.ich else "",
                f"{d.ich.grosze / 100:.2f}".replace(".", ",") if d.ich else "",
                d.ich.slug if d.ich else "",
            ])


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--nasze", type=Path, required=True)
    parser.add_argument("--balia", type=Path, nargs="+", required=True)
    parser.add_argument("--out", type=Path, default=Path("wynik"))
    parser.add_argument(
        "--uwzglednij-watpliwe",
        action="store_true",
        help="Dokłada dopasowania wątpliwe do base-prices.json. Tylko po przejrzeniu CSV.",
    )
    args = parser.parse_args()

    nasze = wczytaj_nasze(args.nasze)
    ich = wczytaj_balie(args.balia)
    print(f"Nasze pozycje: {len(nasze)}   Pozycje Balii: {len(ich)}")

    wszystkie = dopasuj(nasze, ich)
    # Niejednoznaczne schodzą do wątpliwych nawet przy wysokim wyniku — patrz MINIMALNA_PRZEWAGA.
    pewne = [d for d in wszystkie if d.ich and d.wynik >= PEWNE and not d.niejednoznaczne]
    watpliwe = [
        d for d in wszystkie
        if d.ich and d.wynik >= WATPLIWE and (d.wynik < PEWNE or d.niejednoznaczne)
    ]
    brak = [d for d in wszystkie if not d.ich or d.wynik < WATPLIWE]

    args.out.mkdir(parents=True, exist_ok=True)
    zapisz_csv(args.out / "dopasowania-pewne.csv", pewne)
    zapisz_csv(args.out / "dopasowania-watpliwe.csv", watpliwe)
    zapisz_csv(args.out / "bez-dopasowania.csv", brak)

    do_wgrania = pewne + (watpliwe if args.uwzglednij_watpliwe else [])
    ladunek = {"prices": [{"slug": d.nasza.slug, "basePriceGrosze": d.ich.grosze} for d in do_wgrania]}
    (args.out / "base-prices.json").write_text(
        json.dumps(ladunek, ensure_ascii=False, indent=2), encoding="utf-8"
    )

    print(f"  pewne          {len(pewne):>4}  -> dopasowania-pewne.csv")
    niejednoznaczne = sum(1 for d in watpliwe if d.niejednoznaczne)
    print(f"  watpliwe       {len(watpliwe):>4}  -> dopasowania-watpliwe.csv  (PRZEJRZYJ)")
    if niejednoznaczne:
        print(f"     w tym {niejednoznaczne} z podobnym wariantem obok — te sprawdz w pierwszej kolejnosci")
    print(f"  bez dopasowania{len(brak):>4}  -> bez-dopasowania.csv  (zostaja z ceną ręczną)")
    print(f"\nDo wgrania: {len(ladunek['prices'])} pozycji w {args.out / 'base-prices.json'}")

    if watpliwe and not args.uwzglednij_watpliwe:
        print("\nWatpliwe NIE weszly do base-prices.json. Przejrzyj CSV i uruchom ponownie")
        print("z --uwzglednij-watpliwe, albo popraw recznie plik JSON.")

    return 0


if __name__ == "__main__":
    sys.exit(main())
