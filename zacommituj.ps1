<#
.SYNOPSIS
  Commituje zmiany w trzech repozytoriach A.K. HOUSE we właściwej kolejności.

.DESCRIPTION
  `frontend` i `backend` to osobne repozytoria. Commit tylko w repo nadrzędnym zapisuje wskaźnik
  na stan, którego nikt inny nie ma — dlatego najpierw submoduły, potem korzeń.

  Skrypt nie wypycha zmian, dopóki nie podasz -Wypchnij.

      powershell -NoProfile -ExecutionPolicy Bypass -File zacommituj.ps1 -NaSucho   # sam podgląd
      powershell -NoProfile -ExecutionPolicy Bypass -File zacommituj.ps1            # commituje
      powershell -NoProfile -ExecutionPolicy Bypass -File zacommituj.ps1 -Wypchnij  # commit + push
#>

[CmdletBinding()]
param(
    [switch] $NaSucho,
    [switch] $Wypchnij
)

$ErrorActionPreference = 'Stop'

function Krok($t)  { Write-Host "`n=== $t ===" -ForegroundColor Cyan }
function Ok($t)    { Write-Host "    $t" -ForegroundColor Green }
function Uwaga($t) { Write-Host "    $t" -ForegroundColor Yellow }

if (-not (Test-Path 'backend\AkHouse.slnx')) {
    throw "Uruchom z katalogu glownego projektu (tam, gdzie jest backend\AkHouse.slnx)."
}

$FRONTEND = @'
Add supplier-price filter, product links and add-to-cart confirmation

Shop panel (/sklep/panel):
- filter the assortment by whether a Balia retail price is known; 88 of 156
  items have none and their price is still the wholesale one
- every row links to its public product page, opened in a new tab
- category and stock badges are block boxes, so a wrapped label no longer
  splits its background across two ragged stripes

Shop:
- adding to the cart now asks whether to carry on shopping or go to the
  cart, instead of taking over the page; the server-side re-pricing is
  therefore deferred until somebody actually opens the cart
'@

$BACKEND = @'
Drop the working log from the repository
'@

$ROOT = @'
Refresh the project state, tidy the repository, re-check supplier prices

- STAN-PRAC.md rewritten as the entry point for a new working session:
  what exists, what is verified, what is left
- working junk gathered into _do-usuniecia/ and .gitignore extended so it
  does not grow back; source material moved out of the repository
- Balia catalogue scraped again on 25.08: no price moved since 22.08 and
  all 68 loaded base prices still match, so nothing needs re-importing
- tools/import-balia: fresh catalogue plus a review sheet for the 88
  items that still have no retail price
'@

function ZacommitujRepo($sciezka, $nazwa, $komunikat) {
    Krok "$nazwa"
    Push-Location $sciezka
    try {
        $stan = git status --porcelain
        if (-not $stan) { Ok "nic do zacommitowania"; return $false }

        $stan | ForEach-Object { Write-Host "    $_" -ForegroundColor DarkGray }

        if ($NaSucho) {
            Uwaga "[na sucho] git add -A; git commit"
            Uwaga "komunikat: $(($komunikat -split "`n")[0])"
            return $true
        }

        git add -A
        git commit -m $komunikat | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "$nazwa - commit sie nie powiodl." }
        Ok "zacommitowane: $(git rev-parse --short HEAD)"

        if ($Wypchnij) {
            git push
            if ($LASTEXITCODE -ne 0) { throw "$nazwa - push sie nie powiodl." }
            Ok "wypchniete"
        }
        return $true
    }
    finally { Pop-Location }
}

# Kolejnosc jest istotna: submoduly, potem korzen z ich nowymi wskaznikami.
$zmienionyFront = ZacommitujRepo 'frontend' 'frontend (submodul)' $FRONTEND
$zmienionyBack  = ZacommitujRepo 'backend'  'backend (submodul)'  $BACKEND
ZacommitujRepo '.' 'ak-house (repo nadrzedne)' $ROOT | Out-Null

Krok "Na koniec"
if ($NaSucho) {
    Uwaga "Na sucho - nic nie zostalo zapisane."
} else {
    if (-not $Wypchnij) { Uwaga "Nie wypchnieto. Gdy przejrzysz commity: git push (w kazdym repo)." }
    Write-Host @"

Zanim wypchniesz - dwie rzeczy z listy w STAN-PRAC.md:

  1. 'klucz upload\Nowy Dokument tekstowy.txt' jest sledzony przez gita i byl juz
     wypchniety. Klucz siedzi w historii - wymien go na nowy zamiast liczyc, ze
     'git rm --cached' cos tu zalatwi.

  2. 'dotnet test backend\AkHouse.slnx' nie byl jeszcze nigdy uruchomiony -
     148 metod testowych czeka na pierwszy przebieg.
"@ -ForegroundColor Gray
}
