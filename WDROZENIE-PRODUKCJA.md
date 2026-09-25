# Wdrożenie A.K. HOUSE — checklista produkcyjna

## 1. Kopia przed wdrożeniem

- zatrzymaj aplikację albo wykonaj spójną kopię SQLite;
- skopiuj `/home/data/akhouse.db` poza App Service;
- skopiuj `/home/data/uploads`;
- sprawdź, czy obie kopie dają się odczytać i zapisz datę wykonania.

Migracje uruchamiają się automatycznie podczas startu API. Kopię trzeba wykonać **przed** publikacją.

## 2. Ustawienia aplikacji w Azure

Wprowadź jako App Settings / Connection Strings, bez zapisywania sekretów w repozytorium:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Default=Data Source=/home/data/akhouse.db
Media__UploadRoot=/home/data/uploads
Security__DataProtectionKeysPath=/home/data/dataprotection-keys
Shop__SiteBaseUrl=https://akhouse.pl

Email__StudioInbox=kontakt@akhouse.pl
Email__FromName=A.K. HOUSE
Email__FromAddress=no-reply@akhouse.pl
Email__SmtpHost=<host dostawcy poczty>
Email__SmtpPort=587
Email__SmtpUseSsl=true
Email__SmtpUser=<użytkownik SMTP>
Email__SmtpPassword=<sekret SMTP>
```

Przy pierwszym wdrożeniu trwałych kluczy obecne sesje operatorów mogą zostać jednorazowo
wylogowane. Kolejne restarty i publikacje zachowają sesje, o ile katalog kluczy nie zostanie usunięty.

Na pierwszym uruchomieniu pustej bazy ustaw również:

```text
Operators__Bootstrap__Email=<adres właściciela>
Operators__Bootstrap__DisplayName=<nazwa właściciela>
Operators__Bootstrap__Password=<jednorazowe silne hasło>
```

Po utworzeniu konta, pierwszym logowaniu i zmianie hasła usuń `Operators__Bootstrap__Password`
z konfiguracji. Klucze maszynowe pozostaw wyłączone, jeśli żadna integracja ich nie używa.

## 3. Zmienne builda frontendu

Opcjonalne identyfikatory analityczne muszą istnieć w środowisku procesu publikującego — są
wbudowywane w pliki JavaScript i ustawienie ich później w App Service niczego nie zmieni:

```text
VITE_GA_MEASUREMENT_ID=<GA4>
VITE_META_PIXEL_ID=<Meta Pixel>
VITE_PLAUSIBLE_DOMAIN=<opcjonalnie>
VITE_PLAUSIBLE_SRC=<opcjonalnie>
```

Nie wolno umieszczać haseł, connection stringów ani innych sekretów w zmiennych `VITE_*`.

## 4. Budowa paczki

Z katalogu repozytorium:

```powershell
dotnet restore backend/AkHouse.slnx
dotnet test backend/AkHouse.slnx --no-restore
Push-Location frontend
npm.cmd test -- --run
Pop-Location
dotnet publish backend/src/AkHouse.Api/AkHouse.Api.csproj -c Release --no-restore -o .publish-check
```

Publikacja API automatycznie buduje frontend i umieszcza go w `wwwroot`. Nie używaj
`SkipFrontendBuild=true` dla paczki wysyłanej na produkcję.

W paczce sprawdź co najmniej:

- `AkHouse.Api.dll` i `web.config`;
- `wwwroot/index.html`, `wwwroot/sitemap.xml`, `wwwroot/robots.txt`;
- statyczne strony PL/EN oraz katalog `wwwroot/images`;
- brak pliku bazy, uploadów, haseł i plików `.env`.

## 5. Publikacja i test po starcie

1. Opublikuj z włączonym App Offline i backupem MSDeploy.
2. Poczekaj na zakończenie migracji i sprawdź log startowy.
3. Sprawdź `/`, `/en`, `/galeria`, `/domki-drewniane` oraz `/api/content`.
4. Zaloguj się przez `/admin` i przejdź kolejno do CRM, panelu strony, sklepu, Generatora ofert
   i kont operatorów.
5. Utwórz szkic oferty, pobierz PDF i dopiero po potwierdzeniu SMTP wykonaj kontrolną wysyłkę.
6. Wyślij jedno kontrolne zgłoszenie formularza, potwierdź rekord w CRM i wiadomość w skrzynce
   `kontakt@akhouse.pl`.
7. Sprawdź telefon 390 px, canonical `https://akhouse.pl` oraz przekierowanie z `www`.

## 6. Cofnięcie wdrożenia

Jeżeli start, migracja albo smoke test nie przejdą:

1. zatrzymaj aplikację;
2. przywróć poprzednią paczkę;
3. jeśli migracja zmieniła bazę, przywróć wykonaną wcześniej kopię SQLite;
4. przywróć odpowiadający jej katalog uploadów;
5. uruchom aplikację i ponów test podstawowych tras oraz logowania.

## Dane wymagające uzupełnienia przed produkcją

- dane dostępowe SMTP;
- docelowy adres właściciela i jednorazowe hasło pierwszego operatora;
- identyfikatory GA4/Meta/Plausible, jeżeli analityka ma działać od pierwszego dnia;
- numer rachunku bankowego dopiero przed uruchomieniem publicznego sklepu.
