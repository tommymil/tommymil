#!/usr/bin/env python3
"""
Usuwa z bazy sklepu palety kolorów i puste placeholdery podpięte pod zdjęcia produktów.

Balia dokłada do wkładów dwa dodatkowe kadry: tabelkę czterech kolorów i pełną paletę RAL.
Wyboru koloru dokonuje się teraz przełącznikiem wariantów, więc te obrazki tylko rozpraszają.
Przy okazji lecą dwa placeholdery „X" (puste kadry z przekreśleniem) i powtórzone zdjęcia,
które importer wstawił dwa razy pod ten sam produkt.

ZOSTAJĄ zbliżenia faktury materiału — perła, granitcoat, ciemny marmur. To nie są palety,
tylko pokazanie z bliska, jak wygląda konkretny wariant.

Skrypt jest idempotentny: uruchomiony drugi raz nie znajdzie już nic do usunięcia.
Pliki .webp zostają na dysku i w zasobach — usuwane są wyłącznie powiązania z produktami,
więc cofnięcie decyzji to kwestia dopisania nazw z powrotem do `shop-catalog.json`.

    python tools/usun-palety-kolorow.py            # z katalogu głównego projektu
    python tools/usun-palety-kolorow.py --na-sucho # sam podgląd, nic nie zapisuje
    python tools/usun-palety-kolorow.py --baza <sciezka-do.db>

UWAGA: przed uruchomieniem zatrzymaj API — SQLite nie lubi dwóch piszących naraz.
"""

from __future__ import annotations

import argparse
import shutil
import sqlite3
import sys
from pathlib import Path

# Nazwy plików, nie adresy: ten sam obrazek bywa podpięty pod różnymi ścieżkami.
PALETY = {
    "katalog-dc55a7b47760ccdf.webp": "paleta czterech kolorów",
    "katalog-81e33cab58403151.webp": "pełna paleta RAL",
    "katalog-cce853eb1666d1fc.webp": "trzy próbki koloru",
    "katalog-2f21f5ca5affaab8.webp": "dwie próbki koloru",
    "katalog-a7aca682cf155abd.webp": "dwie próbki koloru (marmur)",
    "katalog-f0bbe5d3ab84477f.webp": "pojedyncza próbka koloru",
    "katalog-5eabdec9859dff08.webp": "pojedyncza próbka koloru (grafit)",
    "katalog-f30274dc4c19ab74.webp": "pusty placeholder „X”",
    "katalog-8dfc1df64d95b9a6.webp": "pusty placeholder „X”",
}

# Wypisane wprost, żeby było widać, czego skrypt świadomie NIE rusza.
FAKTURY = {
    "katalog-36701101b4e99852.webp": "zbliżenie faktury — perła",
    "katalog-a401929e6f94c67d.webp": "zbliżenie faktury — granitcoat",
    "katalog-dd3f6abeada77553.webp": "zbliżenie faktury — ciemny marmur",
}


def korzen() -> Path:
    for katalog in [Path(__file__).resolve(), *Path(__file__).resolve().parents]:
        if (katalog / "backend" / "AkHouse.slnx").exists():
            return katalog
    raise SystemExit("Nie znalazlem katalogu glownego projektu — podaj bazę przez --baza.")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--baza", type=Path)
    parser.add_argument("--na-sucho", action="store_true", help="Pokaż, co poleci, i nic nie zapisuj.")
    args = parser.parse_args()

    baza = args.baza or korzen() / "backend/src/AkHouse.Api/akhouse.db"
    if not baza.exists():
        raise SystemExit(f"Nie ma bazy: {baza}")

    for towarzyszacy in ("-wal", "-shm"):
        if Path(str(baza) + towarzyszacy).exists() and not args.na_sucho:
            print(f"UWAGA: obok bazy lezy plik {baza.name}{towarzyszacy} — API moze dzialac.")
            print("       Zatrzymaj je i uruchom skrypt ponownie.")
            return 1

    db = sqlite3.connect(baza)
    przed = db.execute("SELECT COUNT(*) FROM ShopProductImages").fetchone()[0]

    do_usuniecia: list[tuple[str, str, str, str]] = []   # (id, slug, plik, powód)
    for pid, slug in db.execute("SELECT Id, Slug FROM ShopProducts"):
        widziane: set[str] = set()
        for iid, url in db.execute(
            "SELECT Id, Url FROM ShopProductImages WHERE ProductId = ? ORDER BY SortOrder", (pid,)
        ):
            plik = url.rsplit("/", 1)[-1]
            if plik in PALETY:
                do_usuniecia.append((iid, slug, plik, PALETY[plik]))
            elif url in widziane:
                do_usuniecia.append((iid, slug, plik, "powtórzone zdjęcie w tym samym produkcie"))
            else:
                widziane.add(url)

    if not do_usuniecia:
        print("Nie ma czego usuwać — baza jest już wyczyszczona.")
        return 0

    from collections import Counter
    print(f"Zdjęć w bazie: {przed}\n")
    print("Do usunięcia:")
    for powod, ile in Counter(p for _, _, _, p in do_usuniecia).most_common():
        print(f"  {ile:3d}x  {powod}")
    print(f"\nDotknięte produkty: {len({s for _, s, _, _ in do_usuniecia})}")
    print("Zostają nietknięte:")
    for plik, opis in FAKTURY.items():
        ile = db.execute("SELECT COUNT(*) FROM ShopProductImages WHERE Url LIKE ?", (f"%{plik}",)).fetchone()[0]
        print(f"  {ile:3d}x  {opis}")

    if args.na_sucho:
        print("\n[na sucho] Nic nie zapisano.")
        return 0

    kopia = baza.with_suffix(baza.suffix + ".przed-czyszczeniem")
    shutil.copy2(baza, kopia)
    print(f"\nKopia zapasowa: {kopia}")

    db.executemany("DELETE FROM ShopProductImages WHERE Id = ?", [(i,) for i, _, _, _ in do_usuniecia])

    # Po usunięciu w środku listy zostają dziury w SortOrder. Galeria sortuje po tym polu,
    # więc luki jej nie zaszkodzą — ale panel dodaje kolejne zdjęcie jako max+1 i bez
    # przenumerowania nowe zdjęcie wskoczyłoby na koniec z numerem 5 przy dwóch zdjęciach.
    for pid, in db.execute("SELECT DISTINCT ProductId FROM ShopProductImages"):
        for kolejny, (iid,) in enumerate(
            db.execute("SELECT Id FROM ShopProductImages WHERE ProductId = ? ORDER BY SortOrder", (pid,)).fetchall()
        ):
            db.execute("UPDATE ShopProductImages SET SortOrder = ? WHERE Id = ?", (kolejny, iid))

    db.commit()
    po = db.execute("SELECT COUNT(*) FROM ShopProductImages").fetchone()[0]
    osierocone = db.execute(
        "SELECT COUNT(*) FROM ShopProducts WHERE Id NOT IN (SELECT ProductId FROM ShopProductImages)"
    ).fetchone()[0]

    print(f"Zdjęć po czyszczeniu: {po} (usunięto {przed - po})")
    print(f"Produktów bez zdjęcia: {osierocone}")
    if osierocone:
        print("UWAGA: ktoregos produktu pozbawiono jedynego zdjecia — przywroc kopie zapasowa.")
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
