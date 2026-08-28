#!/usr/bin/env python3
"""
Wykonuje SQL z dwóch nowych migracji na świeżej, tymczasowej bazie SQLite.

Po co osobne narzędzie, skoro `dotnet ef database update` zrobiłby to samo: kontener nie ma
dostępu do NuGeta, więc EF się tu nie zbuduje. Ryzyko w tych migracjach nie leży jednak w EF,
tylko w surowym SQL-u — literówka w slugu nie wywoła błędu, po prostu cicho niczego nie zaktualizuje.
To sprawdzamy tutaj, na prawdziwych danych z `shop-catalog.json`.

SQL nie jest przepisywany ręcznie — jest wycinany z plików migracji, żeby test patrzył
dokładnie na to, co pojedzie na produkcję.
"""

from __future__ import annotations

import json
import re
import sqlite3
import sys
from pathlib import Path

def korzen() -> Path:
    """Katalog główny projektu — szukany w górę od położenia skryptu, z możliwością podania wprost."""
    if len(sys.argv) > 1:
        return Path(sys.argv[1]).resolve()
    for katalog in [Path(__file__).resolve(), *Path(__file__).resolve().parents]:
        if (katalog / "backend" / "AkHouse.slnx").exists():
            return katalog
    raise SystemExit(
        "Nie znalazlem katalogu glownego projektu.\n"
        "Uruchom: python tools/sprawdz-migracje.py [sciezka-do-ak-house]"
    )


PERSISTENCE = korzen() / "backend/src/AkHouse.Infrastructure/Persistence"
MIGRACJE = PERSISTENCE / "Migrations"
KATALOG = PERSISTENCE / "Data/shop-catalog.json"

# ShippingClassSupport z ShopShipping.cs
PARCEL, OVERSIZE, PALLET, PICKUP = 1, 2, 4, 8

bledy: list[str] = []
uwagi: list[str] = []


def sprawdz(warunek: bool, opis: str) -> None:
    print(f"  {'OK  ' if warunek else 'BLAD'}  {opis}")
    if not warunek:
        bledy.append(opis)


def cialo_metody(plik: Path, kierunek: str) -> str:
    tekst = plik.read_text(encoding="utf-8-sig")
    poczatek = tekst.index(f"void {kierunek}(MigrationBuilder")
    if kierunek == "Up":
        return tekst[poczatek:tekst.index("void Down(MigrationBuilder")]
    return tekst[poczatek:]


def argument(wywolanie: str, nazwa: str) -> str | None:
    m = re.search(rf'\b{nazwa}:\s*("(?:[^"\\]|\\.)*"|[^,\)]+)', wywolanie)
    if not m:
        return None
    wartosc = m.group(1).strip()
    return wartosc[1:-1].replace('\\"', '"') if wartosc.startswith('"') else wartosc


def operacje(plik: Path, kierunek: str) -> list[tuple[str, str]]:
    """
    Operacje migracji **w kolejności ze źródła**. Kolejność jest tu istotna: w `ProductShippingClasses`
    kolumna `ShippingClass` jest kasowana dopiero po tym, jak SQL przepisze z niej maski — odwrócenie
    tych dwóch kroków dałoby same wartości domyślne i test by tego nie zauważył.
    """
    cialo = cialo_metody(plik, kierunek)
    wynik: list[tuple[str, str]] = []

    wzorzec = re.compile(
        r'migrationBuilder\.(Sql|AddColumn(?:<[^>]+>)?|DropColumn|CreateIndex|DropIndex)\s*\(', re.S)
    for m in wzorzec.finditer(cialo):
        operacja = m.group(1).split("<")[0]
        # Domknięcie nawiasu z pominięciem tego, co siedzi w literałach.
        i, glebokosc, tresc = m.end(), 1, []
        while i < len(cialo) and glebokosc:
            if cialo.startswith('"""', i):
                koniec = cialo.index('"""', i + 3) + 3
                tresc.append(cialo[i:koniec]); i = koniec; continue
            znak = cialo[i]
            if znak == '"':
                koniec = i + 1
                while cialo[koniec] != '"' or cialo[koniec - 1] == '\\':
                    koniec += 1
                tresc.append(cialo[i:koniec + 1]); i = koniec + 1; continue
            glebokosc += (znak == "(") - (znak == ")")
            if glebokosc:
                tresc.append(znak)
            i += 1
        wynik.append((operacja, "".join(tresc)))
    return wynik


def na_sql(operacja: str, argumenty: str) -> list[str]:
    if operacja == "Sql":
        surowy = re.search(r'"""(.*?)"""', argumenty, re.S)
        if surowy:
            return [surowy.group(1)]
        pojedynczy = re.match(r'\s*"((?:[^"\\]|\\.)*)"', argumenty)
        return [pojedynczy.group(1).replace('\\"', '"')] if pojedynczy else []

    tabela = argument(argumenty, "table")
    nazwa = argument(argumenty, "name")

    if operacja == "AddColumn":
        typ = argument(argumenty, "type") or "TEXT"
        nullable = (argument(argumenty, "nullable") or "true").strip() == "true"
        domyslna = argument(argumenty, "defaultValue")
        czesci = [f'ALTER TABLE "{tabela}" ADD COLUMN "{nazwa}" {typ}']
        if not nullable:
            czesci.append("NOT NULL")
        if domyslna is not None:
            literal = domyslna if re.fullmatch(r"-?\d+", domyslna.strip()) else f"'{domyslna}'"
            czesci.append(f"DEFAULT {literal}")
        return [" ".join(czesci)]

    if operacja == "DropColumn":
        return [f'ALTER TABLE "{tabela}" DROP COLUMN "{nazwa}"']
    if operacja == "CreateIndex":
        kolumna = argument(argumenty, "column")
        return [f'CREATE INDEX "{nazwa}" ON "{tabela}" ("{kolumna}")']
    if operacja == "DropIndex":
        return [f'DROP INDEX "{nazwa}"']
    return []


def zbuduj_baze(produkty: list[dict]) -> sqlite3.Connection:
    """Schemat sprzed obu migracji, wypełniony realnym asortymentem."""
    db = sqlite3.connect(":memory:")
    db.executescript("""
        CREATE TABLE "ShopProducts" (
            "Id" TEXT NOT NULL PRIMARY KEY,
            "Slug" TEXT NOT NULL,
            "Name" TEXT NOT NULL,
            "ShortDescription" TEXT NOT NULL,
            "PriceGrosze" INTEGER NOT NULL,
            "ShippingClass" TEXT NOT NULL DEFAULT 'Parcel'
        );
        CREATE TABLE "ShippingMethods" (
            "Id" TEXT NOT NULL PRIMARY KEY,
            "Name" TEXT NOT NULL,
            "SupportedClasses" INTEGER NOT NULL DEFAULT 15
        );
    """)

    # Klasy wysyłki rozkładamy tak, żeby każda gałąź CASE miała co najmniej jeden wiersz.
    klasy = ["Parcel", "Oversize", "Pallet", "PickupOnly"]
    for i, p in enumerate(produkty):
        db.execute(
            'INSERT INTO "ShopProducts" VALUES (?, ?, ?, ?, ?, ?)',
            (f"p{i}", p["slug"], p["name"], p.get("shortDescription", ""),
             p["priceGrosze"], klasy[i % 4]),
        )
    for i, nazwa in enumerate(["Kurier", "Paleta", "Odbiór osobisty"]):
        db.execute('INSERT INTO "ShippingMethods" VALUES (?, ?, 15)', (f"m{i}", nazwa))
    db.commit()
    return db


def zastosuj(db: sqlite3.Connection, plik: Path, kierunek: str) -> None:
    for operacja, argumenty in operacje(plik, kierunek):
        for polecenie in na_sql(operacja, argumenty):
            db.executescript(polecenie)
    db.commit()


def main() -> int:
    produkty = json.loads(KATALOG.read_text(encoding="utf-8"))["products"]
    print(f"Asortyment z seedu: {len(produkty)} pozycji\n")

    klasy_plik = MIGRACJE / "20260825172407_ProductShippingClasses.cs"
    warianty_plik = MIGRACJE / "20260825174051_ProductVariants.cs"

    db = zbuduj_baze(produkty)
    przed = {slug: kl for slug, kl in db.execute('SELECT "Slug", "ShippingClass" FROM "ShopProducts"')}

    # --- Migracja 1: ShippingClass (tekst) -> ShippingClasses (maska bitowa) ------------------
    print("Migracja ProductShippingClasses")
    zastosuj(db, klasy_plik, "Up")

    oczekiwane = {
        "Parcel": PARCEL | PICKUP,
        "Oversize": OVERSIZE | PICKUP,
        "Pallet": PALLET | PICKUP,
        "PickupOnly": PICKUP,
    }
    po = dict(db.execute('SELECT "Slug", "ShippingClasses" FROM "ShopProducts"'))
    zle = [(s, przed[s], po[s]) for s in przed if po[s] != oczekiwane[przed[s]]]
    sprawdz(not zle, f"każda z {len(przed)} pozycji dostała maskę zgodną ze swoją dawną klasą")
    for s, a, b in zle[:5]:
        print(f"          {s}: {a} -> {b}")

    brak_odbioru = db.execute(
        'SELECT COUNT(*) FROM "ShopProducts" WHERE ("ShippingClasses" & ?) = 0', (PICKUP,)).fetchone()[0]
    sprawdz(brak_odbioru == 0, "odbiór osobisty pozostał możliwy dla każdego produktu")

    odbior = db.execute(
        'SELECT "SupportedClasses" FROM "ShippingMethods" WHERE "Name" = ?', ("Odbiór osobisty",)).fetchone()[0]
    sprawdz(odbior == PICKUP, f"metoda „Odbiór osobisty” obsługuje wyłącznie klasę PickupOnly (jest {odbior})")

    kurier = db.execute(
        'SELECT "SupportedClasses" FROM "ShippingMethods" WHERE "Name" = ?', ("Kurier",)).fetchone()[0]
    sprawdz(kurier == 15, "pozostałe metody dostawy nie zostały ruszone")

    kolumny = [r[1] for r in db.execute('PRAGMA table_info("ShopProducts")')]
    sprawdz("ShippingClass" not in kolumny, "stara kolumna ShippingClass została usunięta")

    # --- Migracja 2: grupy wariantów ----------------------------------------------------------
    print("\nMigracja ProductVariants")
    zastosuj(db, warianty_plik, "Up")

    grupy = dict(db.execute("""
        SELECT "VariantGroupKey", COUNT(*) FROM "ShopProducts"
        WHERE "VariantGroupKey" IS NOT NULL GROUP BY "VariantGroupKey"
    """))
    sprawdz(len(grupy) == 6, f"powstało 6 grup wariantów (jest {len(grupy)})")
    sprawdz(sum(grupy.values()) == 17, f"przypisano 17 produktów (jest {sum(grupy.values())})")

    sierotki = [k for k, ile in grupy.items() if ile < 2]
    sprawdz(not sierotki, f"żadna grupa nie ma jednego wariantu {sierotki or ''}")

    bez_etykiety = db.execute("""
        SELECT COUNT(*) FROM "ShopProducts"
        WHERE "VariantGroupKey" IS NOT NULL AND ("VariantLabel" IS NULL OR "VariantLabel" = '')
    """).fetchone()[0]
    sprawdz(bez_etykiety == 0, "każdy wariant ma etykietę do pokazania na przełączniku")

    zdublowane = db.execute("""
        SELECT "VariantGroupKey", "VariantLabel", COUNT(*) c FROM "ShopProducts"
        WHERE "VariantGroupKey" IS NOT NULL
        GROUP BY 1, 2 HAVING c > 1
    """).fetchall()
    sprawdz(not zdublowane, f"etykiety w obrębie grupy są unikalne {zdublowane or ''}")

    etykieta_bez_grupy = db.execute("""
        SELECT COUNT(*) FROM "ShopProducts" WHERE "VariantGroupKey" IS NULL AND "VariantLabel" IS NOT NULL
    """).fetchone()[0]
    sprawdz(etykieta_bez_grupy == 0, "nie ma etykiety wariantu bez grupy")

    print()
    for klucz in sorted(grupy):
        wiersze = db.execute("""
            SELECT "VariantLabel", "PriceGrosze" FROM "ShopProducts"
            WHERE "VariantGroupKey" = ? ORDER BY "PriceGrosze"
        """, (klucz,)).fetchall()
        ceny = {c for _, c in wiersze}
        znacznik = "  <- jedna cena we wszystkich wariantach" if len(ceny) == 1 else ""
        print(f"    {klucz:32s} {len(wiersze)} war.{znacznik}")
        for etykieta, cena in wiersze:
            print(f"        {cena / 100:9.2f} zl   {etykieta}")
        if len(ceny) == 1:
            uwagi.append(f"{klucz}: przełącznik nie zmieni ceny — wszystkie warianty kosztują tyle samo")

    # --- Powrót ------------------------------------------------------------------------------
    print("\nCofanie obu migracji")
    zastosuj(db, warianty_plik, "Down")
    zastosuj(db, klasy_plik, "Down")

    wrocilo = dict(db.execute('SELECT "Slug", "ShippingClass" FROM "ShopProducts"'))
    rozne = {s: (przed[s], wrocilo[s]) for s in przed if przed[s] != wrocilo[s]}
    sprawdz(not rozne, f"Down() odtwarza pierwotne klasy wysyłki {list(rozne.items())[:3] or ''}")

    odbior_po = db.execute(
        'SELECT "SupportedClasses" FROM "ShippingMethods" WHERE "Name" = ?', ("Odbiór osobisty",)).fetchone()[0]
    sprawdz(odbior_po == 15, "Down() przywraca metodzie odbioru pełen zakres klas")

    print()
    if uwagi:
        print("Do decyzji (nie błędy):")
        for u in uwagi:
            print(f"  - {u}")
        print()
    print("WYNIK:", "wszystko przeszło" if not bledy else f"{len(bledy)} nieudanych sprawdzeń")
    return 1 if bledy else 0


if __name__ == "__main__":
    sys.exit(main())
