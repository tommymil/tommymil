<#
.SYNOPSIS
  Wgrywa ceny detaliczne Balia Technic do sklepu i opcjonalnie ustawia narzut.

.DESCRIPTION
  Robi całą sekwencję za jednym razem: migracje -> build -> start API -> import cen -> narzut.
  Zatrzymuje się na pierwszym błędzie i mówi, co poszło nie tak.

  Uruchamiać z katalogu głównego projektu (D:\moje\ak-house):

      powershell -NoProfile -ExecutionPolicy Bypass -File tools\import-balia\wgraj-ceny.ps1

  Z narzutem od razu (przecinek albo kropka, obojętne):

      ... -File tools\import-balia\wgraj-ceny.ps1 -Narzut 8,5

  Klucz brany jest z `dotnet user-secrets`. Jeśli go nie ma, skrypt powie, jak go ustawić.
#>

[CmdletBinding()]
param(
    # Narzut w procentach nad ceną detaliczną Balii. Pominięty = ceny wchodzą 1:1 jak u Balii.
    [string] $Narzut,

    # Klucz Admin:ShopApiKey. Pominięty = brany z dotnet user-secrets.
    [string] $Klucz,

    [string] $Ceny = "tools\import-balia\wynik\base-prices.json",
    [int]    $Port = 5033
)

$ErrorActionPreference = 'Stop'

# Katalog projektu wyliczamy ze sciezki samego skryptu, a nie z biezacego katalogu.
# Dwa razy z rzedu poszlo nie tak wlasnie dlatego, ze polecenie odpalono z backend\ zamiast
# z katalogu glownego - skrypt nie ma prawa byc na to wrazliwy.
$korzen = Resolve-Path (Join-Path $PSScriptRoot '..\..')
Set-Location $korzen

$api = "http://localhost:$Port"
$projektApi = "backend\src\AkHouse.Api"
$projektInfra = "backend\src\AkHouse.Infrastructure"

function Krok($tekst) { Write-Host "`n=== $tekst ===" -ForegroundColor Cyan }
function Ok($tekst)   { Write-Host "    $tekst" -ForegroundColor Green }
function Uwaga($tekst){ Write-Host "    $tekst" -ForegroundColor Yellow }

Ok "Katalog projektu: $korzen"

# --- 0. Czy jestesmy tam, gdzie trzeba ---------------------------------------
if (-not (Test-Path "backend\AkHouse.slnx")) {
    throw "Nie widze backend\AkHouse.slnx w $korzen. Czy skrypt lezy w tools\import-balia\?"
}
if (-not (Test-Path $Ceny)) {
    throw "Nie znalazlem pliku z cenami: $Ceny"
}
$liczbaCen = (Get-Content $Ceny -Raw | ConvertFrom-Json).prices.Count
Ok "Plik z cenami: $liczbaCen pozycji"

# --- 1. Migracje --------------------------------------------------------------
# JEDNA migracja, nie dwie. Obie zmiany modelu (tabela kategorii i cennik) siedza juz
# w kodzie naraz, wiec pierwsze `migrations add` zlapie je razem, a drugie wygenerowaloby
# pusty plik. Migracja ma byc czysto addytywna: CreateTable / AddColumn / CreateIndex.
Krok "Migracje EF Core"
$katalogMigracji = "$projektInfra\Persistence\Migrations"
$migracje = Get-ChildItem $katalogMigracji -Filter "*.cs" -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -notlike "*ModelSnapshot*" } | ForEach-Object { $_.Name }

# `dotnet ef` bywa niezainstalowany na swiezej maszynie.
dotnet ef --version *> $null
if ($LASTEXITCODE -ne 0) {
    Uwaga "Instaluje narzedzie dotnet-ef..."
    dotnet tool install --global dotnet-ef
}

$nazwaMigracji = "ShopCategoriesAndPricing"
if ($migracje -match $nazwaMigracji) {
    Ok "$nazwaMigracji - juz jest"
} else {
    Uwaga "Generuje migracje $nazwaMigracji..."
    dotnet ef migrations add $nazwaMigracji --project $projektInfra --startup-project $projektApi --output-dir Persistence\Migrations
    if ($LASTEXITCODE -ne 0) { throw "Nie udalo sie wygenerowac migracji." }

    # Kontrola: na zywym katalogu 156 pozycji migracja NIE MOZE kasowac ani przebudowywac kolumn.
    $plik = Get-ChildItem $katalogMigracji -Filter "*_$nazwaMigracji.cs" | Select-Object -First 1
    $tresc = Get-Content $plik.FullName -Raw
    $podejrzane = @('DropColumn', 'DropTable', 'AlterColumn', 'RenameColumn') |
                  Where-Object { $tresc -match $_ }
    if ($podejrzane) {
        throw @"
STOP. Migracja zawiera operacje, ktorych sie nie spodziewam: $($podejrzane -join ', ')
Plik: $($plik.FullName)
Spodziewane sa wylacznie CreateTable / AddColumn / CreateIndex.
Nie uruchamiaj tego na bazie z danymi - przeslij mi ten plik.
"@
    }
    Ok "$nazwaMigracji - wygenerowana, tylko operacje addytywne"
}

# --- 2. Build -----------------------------------------------------------------
# Log leci do pliku, nie tylko na ekran: przy pierwszym buildzie tresc bledow jest
# najwazniejsza rzecza w calym uruchomieniu, a w konsoli przewija sie poza ekran.
Krok "Build"
$logBuildu = "build.log"
dotnet build backend\AkHouse.slnx -p:SkipFrontendBuild=true 2>&1 | Tee-Object -FilePath $logBuildu
if ($LASTEXITCODE -ne 0) {
    Uwaga "Pelny log zapisany w: $((Resolve-Path $logBuildu).Path)"
    Write-Host ""
    Write-Host "--- bledy kompilacji ---" -ForegroundColor Red
    Select-String -Path $logBuildu -Pattern ': error ' | ForEach-Object { Write-Host $_.Line -ForegroundColor Red }
    throw "Build sie nie powiodl. Plik build.log lezy w katalogu projektu - przeslij go albo powiedz, ze jest."
}
Ok "Build OK (log: $logBuildu)"

# --- 3. Klucz -----------------------------------------------------------------
Krok "Klucz do panelu sklepu"
if (-not $Klucz) {
    $sekrety = dotnet user-secrets list --project $projektApi 2>$null
    $wpis = $sekrety | Where-Object { $_ -like "Admin:ShopApiKey =*" }
    if ($wpis) { $Klucz = ($wpis -split '=', 2)[1].Trim() }
}
if (-not $Klucz) {
    throw @"
Brak Admin:ShopApiKey. Ustaw go raz:

    dotnet user-secrets set "Admin:ShopApiKey" "<wymysl-dlugi-losowy-ciag>" --project $projektApi

i uruchom skrypt ponownie.
"@
}
Ok "Klucz znaleziony"

# --- 4. Start API -------------------------------------------------------------
Krok "Uruchamiam API"
$dzialalWczesniej = $false
try {
    Invoke-RestMethod "$api/health" -TimeoutSec 3 | Out-Null
    $dzialalWczesniej = $true
    Ok "API juz dziala na porcie $Port - korzystam z niego"
} catch {
    $proces = Start-Process dotnet -ArgumentList "run","--project",$projektApi,"--no-build" -PassThru -WindowStyle Minimized
    Uwaga "Czekam az wstanie (migracje i zasiew trwaja chwile)..."
    $gotowe = $false
    foreach ($i in 1..60) {
        Start-Sleep -Seconds 2
        try { Invoke-RestMethod "$api/health" -TimeoutSec 3 | Out-Null; $gotowe = $true; break } catch {}
    }
    if (-not $gotowe) {
        if ($proces -and -not $proces.HasExited) { Stop-Process -Id $proces.Id -Force }
        throw "API nie wstalo w 2 minuty. Uruchom recznie: dotnet run --project $projektApi"
    }
    Ok "API dziala"
}

try {
    $naglowki = @{ "X-Api-Key" = $Klucz; "Content-Type" = "application/json" }
    $dostawcy = Invoke-RestMethod "$api/api/admin/shop/suppliers" -Headers @{ "X-Api-Key" = $Klucz }
    $balia = $dostawcy | Where-Object { $_.code -eq "balia-technic" } | Select-Object -First 1
    if (-not $balia) { throw "Nie znaleziono dostawcy o kodzie balia-technic." }

    # --- 5. Import cen bazowych ------------------------------------------------
    Krok "Wgrywam ceny detaliczne Balii"
    $tresc = [System.Text.Encoding]::UTF8.GetBytes((Get-Content $Ceny -Raw))
    $wynik = Invoke-RestMethod "$api/api/admin/shop/suppliers/$($balia.id)/base-prices" -Method Post -Headers $naglowki -Body $tresc

    Ok "Zaktualizowano pozycji: $($wynik.updated)"
    if ($wynik.unknownIdentifiers.Count -gt 0) {
        Uwaga "UWAGA - $($wynik.unknownIdentifiers.Count) identyfikatorow nie istnieje u tego dostawcy:"
        $wynik.unknownIdentifiers | ForEach-Object { Uwaga "  $_" }
        Uwaga "To znaczy, ze dopasowanie sie rozjechalo. Przeslij mi te liste."
    }

    # --- 6. Narzut -------------------------------------------------------------
    if ($Narzut) {
        Krok "Ustawiam narzut"
        $procent = [decimal]($Narzut -replace ',', '.')
        $odp = Invoke-RestMethod "$api/api/admin/shop/suppliers/$($balia.id)/margin" -Method Put -Headers $naglowki `
               -Body (@{ marginPercent = $procent } | ConvertTo-Json)
        Ok "Narzut $($odp.marginPercent)% objal $($odp.productsWithBasePrice) pozycji"
        Ok "$($odp.productsPricedManually) pozycji zostaje z cena reczna"
    } else {
        Krok "Narzut"
        Uwaga "Nie podano - ceny weszly 1:1 jak u Balii."
        Uwaga "Ustaw go w /sklep/panel -> Dostawcy i cennik -> Balia Technic, albo uruchom skrypt z -Narzut 8,5"
    }

    # --- 7. Kontrola -----------------------------------------------------------
    Krok "Stan cennika"
    $stan = (Invoke-RestMethod "$api/api/admin/shop/suppliers" -Headers @{ "X-Api-Key" = $Klucz }) `
        | Where-Object { $_.id -eq $balia.id } | Select-Object -First 1
    Write-Host "    narzut               : $($stan.marginPercent)%"
    Write-Host "    z cena detaliczna    : $($stan.productsWithBasePrice)"
    Write-Host "    wyceniane recznie    : $($stan.productsPricedManually)"

    Write-Host "`nGotowe. Sklep: $api  |  Panel: http://localhost:5173/sklep/panel" -ForegroundColor Green
}
finally {
    # API zostawiamy tylko wtedy, gdy dzialalo przed uruchomieniem skryptu.
    if (-not $dzialalWczesniej -and $proces -and -not $proces.HasExited) {
        Stop-Process -Id $proces.Id -Force
        Write-Host "`n(zatrzymalem API, ktore uruchomil skrypt)" -ForegroundColor DarkGray
    }
}
