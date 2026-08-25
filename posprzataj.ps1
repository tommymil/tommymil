<#
.SYNOPSIS
  Kończy sprzątanie repozytorium A.K. HOUSE.

.DESCRIPTION
  Wszystko, co dało się zrobić zdalnie, jest już zrobione: śmieci leżą zebrane
  w `_do-usuniecia\`, `.gitignore` jest uzupełniony. Ten skrypt robi trzy rzeczy,
  których nie da się wykonać przez zdalny mostek:

    1. kasuje `_do-usuniecia\` (zdalna powłoka nie ma prawa usuwać plików),
    2. wynosi `elementy\` obok repozytorium, do `..\ak-house-materialy\`,
    3. wypisuje z gita pliki, które przestały być częścią projektu.

  Uruchom z katalogu głównego projektu:

      powershell -NoProfile -ExecutionPolicy Bypass -File posprzataj.ps1

  Najpierw na sucho, żeby zobaczyć, co się stanie:

      ... -File posprzataj.ps1 -NaSucho
#>

[CmdletBinding()]
param(
    # Tylko pokaż, co zostanie zrobione. Nic nie zmienia.
    [switch] $NaSucho
)

$ErrorActionPreference = 'Stop'

function Krok($t)  { Write-Host "`n=== $t ===" -ForegroundColor Cyan }
function Ok($t)    { Write-Host "    $t" -ForegroundColor Green }
function Uwaga($t) { Write-Host "    $t" -ForegroundColor Yellow }
function Zrob($opis, [scriptblock] $akcja) {
    if ($NaSucho) { Uwaga "[na sucho] $opis" } else { & $akcja; Ok $opis }
}

if (-not (Test-Path 'backend\AkHouse.slnx')) {
    throw "Uruchom z katalogu glownego projektu (tam, gdzie jest backend\AkHouse.slnx)."
}

# --- 1. Kasowanie śmieci ------------------------------------------------------
Krok "Kasuje _do-usuniecia\"
if (Test-Path '_do-usuniecia') {
    $mb = [math]::Round((Get-ChildItem '_do-usuniecia' -Recurse -File |
                         Measure-Object Length -Sum).Sum / 1MB, 1)
    Uwaga "Do skasowania: $mb MB"
    Zrob "usuniete" { Remove-Item '_do-usuniecia' -Recurse -Force }
} else {
    Ok "juz nie ma"
}

# --- 2. Materiały źródłowe poza repo -----------------------------------------
# `elementy\` to 14 MB zdjec z WhatsAppa, grafik i modelu 3D. Materialy zostaja
# na dysku, tylko przestaja byc czescia repozytorium.
Krok "Wynosze elementy\ obok repozytorium"
$cel = Join-Path (Split-Path (Get-Location) -Parent) 'ak-house-materialy'
if (Test-Path 'elementy') {
    if (Test-Path $cel) {
        Uwaga "$cel juz istnieje - scalam zawartosc"
        Zrob "przeniesione do $cel" {
            Get-ChildItem 'elementy' -Force | Move-Item -Destination $cel -Force
            Remove-Item 'elementy' -Recurse -Force
        }
    } else {
        Zrob "przeniesione do $cel" { Move-Item 'elementy' $cel }
    }
} else {
    Ok "juz wyniesione"
}

# --- 3. Porządek w gicie ------------------------------------------------------
# `git rm --cached` wypisuje plik z repozytorium, ale ZOSTAWIA go na dysku.
# Dla `klucz upload\` to celowe: plik zostaje pod reka, przestaje byc commitowany.
Krok "Wypisuje z gita"
$doWypisania = @('elementy', 'klucz upload')
foreach ($sciezka in $doWypisania) {
    Zrob "git rm --cached -r `"$sciezka`"" {
        git rm -r --cached --ignore-unmatch --quiet -- $sciezka
    }
}

Krok "Stan po sprzataniu"
if ($NaSucho) {
    Uwaga "Na sucho - nic nie zmieniono."
} else {
    git status --short
    Write-Host @"

Zostalo do zrobienia recznie:

  1. Przejrzec `git status` powyzej i zacommitowac - najpierw submoduly, potem korzen:
         cd frontend; git add -A; git commit -m "Usun zbedne pliki roboczne"; git push
         cd ..\backend; git add -A; git commit -m "Usun zbedny log"; git push
         cd ..;        git add -A; git commit -m "Posprzataj repozytorium"; git push

  2. WYMIENIC KLUCZ z pliku 'klucz upload\Nowy Dokument tekstowy.txt'.
     Ten plik byl zacommitowany i wypchniety na GitHub, wiec siedzi w historii -
     samo `git rm --cached` go stamtad nie usuwa. Wygeneruj nowy:
         dotnet user-secrets set "Admin:MediaApiKey" "<nowy-dlugi-losowy-ciag>" ``
             --project "D:\moje\ak-house\backend\src\AkHouse.Api"

"@ -ForegroundColor Gray
}
