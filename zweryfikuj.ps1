<#
.SYNOPSIS
  Pełna weryfikacja przed commitem: backend, migracje na świeżej bazie, frontend.

.DESCRIPTION
  Domyka to, na czym urwała się poprzednia sesja. Cztery etapy, każdy przerywa na pierwszym błędzie:

    1. build backendu w konfiguracji Release — tam ostrzeżenia są traktowane jak błędy,
       więc to ostrzejsza bramka niż Debug;
    2. `dotnet test` — pełny zestaw;
    3. obie nowe migracje na ŚWIEŻEJ, tymczasowej bazie, z kontrolą SQL-a, który przypisuje
       istniejące produkty do grup wariantów i przepisuje klasy wysyłki na maski bitowe;
    4. frontend: lint, typy, testy, build produkcyjny.

  Tymczasowa baza powstaje w %TEMP% i jest kasowana na końcu. Deweloperskiego `akhouse.db`
  skrypt nie dotyka.

      powershell -NoProfile -ExecutionPolicy Bypass -File zweryfikuj.ps1
      ... -File zweryfikuj.ps1 -Pomin frontend      # albo: backend, migracje
#>

[CmdletBinding()]
param(
    [ValidateSet('backend', 'testy', 'migracje', 'frontend')]
    [string[]] $Pomin = @()
)

$ErrorActionPreference = 'Stop'

function Krok($t)  { Write-Host "`n=== $t ===" -ForegroundColor Cyan }
function Ok($t)    { Write-Host "    $t" -ForegroundColor Green }
function Uwaga($t) { Write-Host "    $t" -ForegroundColor Yellow }
function Pominiete($t) { Write-Host "    pominiete: $t" -ForegroundColor DarkGray }

if (-not (Test-Path 'backend\AkHouse.slnx')) {
    throw "Uruchom z katalogu glownego projektu (tam, gdzie jest backend\AkHouse.slnx)."
}

$projektApi = "backend\src\AkHouse.Api"
$projektInfra = "backend\src\AkHouse.Infrastructure"
$start = Get-Date

# --- 1. Build -----------------------------------------------------------------------------
if ($Pomin -contains 'backend') { Krok "Build backendu"; Pominiete "-Pomin backend" }
else {
    Krok "Build backendu (Release)"
    dotnet build backend\AkHouse.slnx -c Release -p:SkipFrontendBuild=true
    if ($LASTEXITCODE -ne 0) {
        throw "Build sie nie powiodl. W Release ostrzezenia sa bledami - patrz Directory.Build.props."
    }
    Ok "build OK"
}

# --- 2. Testy backendu --------------------------------------------------------------------
if ($Pomin -contains 'testy') { Krok "Testy backendu"; Pominiete "-Pomin testy" }
else {
    Krok "Testy backendu"
    dotnet test backend\AkHouse.slnx --no-build -c Release
    if ($LASTEXITCODE -ne 0) { throw "Testy backendu nie przeszly." }
    Ok "testy OK"
}

# --- 3. Migracje na swiezej bazie ----------------------------------------------------------
if ($Pomin -contains 'migracje') { Krok "Migracje"; Pominiete "-Pomin migracje" }
else {
    Krok "Migracje na swiezej, tymczasowej bazie"

    $baza = Join-Path $env:TEMP "akhouse-weryfikacja-$(Get-Random).db"
    Uwaga "baza: $baza"

    dotnet ef --version *> $null
    if ($LASTEXITCODE -ne 0) { dotnet tool install --global dotnet-ef }

    try {
        # Podmiana connection stringa idzie przez zmienna srodowiskowa - podwojne podkreslenie
        # zastepuje dwukropek w kluczu konfiguracji.
        $env:ConnectionStrings__Default = "Data Source=$baza"

        dotnet ef database update --project $projektInfra --startup-project $projektApi
        if ($LASTEXITCODE -ne 0) { throw "Migracje nie przeszly na czystej bazie." }
        Ok "wszystkie migracje zastosowane od zera"

        # Zasiew wykonuje sie przy starcie API, wiec podnosimy je na chwile, zeby bylo co sprawdzac.
        Uwaga "Uruchamiam API na chwile, zeby zasiac katalog..."
        $proces = Start-Process dotnet -ArgumentList "run","--project",$projektApi,"--no-build","-c","Release" `
                  -PassThru -WindowStyle Minimized
        $gotowe = $false
        foreach ($i in 1..45) {
            Start-Sleep -Seconds 2
            try { Invoke-RestMethod "http://localhost:5033/health" -TimeoutSec 3 | Out-Null; $gotowe = $true; break } catch {}
        }
        if ($proces -and -not $proces.HasExited) { Stop-Process -Id $proces.Id -Force }
        if (-not $gotowe) { throw "API nie wstalo na tymczasowej bazie." }
        Ok "zasiew wykonany"

        # --- Kontrola danych, nie tylko schematu ---------------------------------------------
        # To jest sedno: literowka w slugu w migracji nie wywola bledu SQL, tylko cicho niczego
        # nie zaktualizuje. Bez tych zapytan migracja "przechodzi", a sklep nie ma wariantow.
        # Pytamy przez sqlite3, jesli jest w PATH - prostsze i czytelniejsze niz mostkowanie
        # ADO.NET z PowerShella.
        $sqlite = Get-Command sqlite3 -ErrorAction SilentlyContinue
        if ($sqlite) {
            $grupy = (sqlite3 $baza 'SELECT COUNT(DISTINCT "VariantGroupKey") FROM "ShopProducts" WHERE "VariantGroupKey" IS NOT NULL;')
            $wariantow = (sqlite3 $baza 'SELECT COUNT(*) FROM "ShopProducts" WHERE "VariantGroupKey" IS NOT NULL;')
            $bezOdbioru = (sqlite3 $baza 'SELECT COUNT(*) FROM "ShopProducts" WHERE ("ShippingClasses" & 8) = 0;')
            $bezEtykiety = (sqlite3 $baza 'SELECT COUNT(*) FROM "ShopProducts" WHERE "VariantGroupKey" IS NOT NULL AND ("VariantLabel" IS NULL OR "VariantLabel" = '''');')

            Write-Host "    grup wariantow          : $grupy   (oczekiwane 6)"
            Write-Host "    produktow w grupach     : $wariantow   (oczekiwane 17)"
            Write-Host "    bez odbioru osobistego  : $bezOdbioru   (oczekiwane 0)"
            Write-Host "    wariantow bez etykiety  : $bezEtykiety   (oczekiwane 0)"

            if ("$grupy" -ne '6' -or "$wariantow" -ne '17' -or "$bezOdbioru" -ne '0' -or "$bezEtykiety" -ne '0') {
                throw "Migracja przeszla, ale dane sie nie zgadzaja - sprawdz slugi w ProductVariants."
            }
            Ok "dane po migracji zgadzaja sie z oczekiwaniami"
        } else {
            Uwaga "Brak sqlite3 w PATH - pomijam kontrole danych."
            Uwaga "Sprawdzenie SQL-a zrobil juz tools/sprawdz-migracje.py na kopii katalogu."
        }
    }
    finally {
        Remove-Item Env:\ConnectionStrings__Default -ErrorAction SilentlyContinue
        Remove-Item "$baza*" -Force -ErrorAction SilentlyContinue
        Ok "tymczasowa baza skasowana"
    }
}

# --- 4. Frontend --------------------------------------------------------------------------
if ($Pomin -contains 'frontend') { Krok "Frontend"; Pominiete "-Pomin frontend" }
else {
    Krok "Frontend"
    foreach ($zadanie in @('lint', 'typecheck', 'test', 'build')) {
        Write-Host "`n  npm run $zadanie" -ForegroundColor DarkGray
        npm --prefix frontend run $zadanie
        if ($LASTEXITCODE -ne 0) { throw "frontend: `npm run $zadanie` nie przeszlo." }
    }
    Ok "frontend OK"
}

Krok "Gotowe"
Ok "czas: $([int]((Get-Date) - $start).TotalSeconds) s"
Write-Host "`nJesli wszystko na zielono - zacommituj.ps1 -NaSucho, a potem bez przelacznika." -ForegroundColor Gray
