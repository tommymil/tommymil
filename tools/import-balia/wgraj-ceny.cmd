@echo off
REM Klikalny skrót do wgraj-ceny.ps1 — omija problemy ze składnią powłoki.
REM Uruchom dwuklikiem albo: tools\import-balia\wgraj-ceny.cmd 8,5
REM Argument (opcjonalny) to narzut w procentach, np. 8,5 albo -3.

setlocal
cd /d "%~dp0..\.."

if "%~1"=="" (
    powershell -NoProfile -ExecutionPolicy Bypass -File "tools\import-balia\wgraj-ceny.ps1"
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -File "tools\import-balia\wgraj-ceny.ps1" -Narzut "%~1"
)

echo.
echo Nacisnij dowolny klawisz, zeby zamknac to okno.
pause > nul
endlocal
