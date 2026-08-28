# Konta operatorów — jak to uruchomić

## 1. Wygeneruj migrację

Nowa tabela `Operators` czeka na migrację. Jedna komenda, z katalogu głównego projektu:

```powershell
dotnet ef migrations add OperatorAccounts --project backend\src\AkHouse.Infrastructure --startup-project backend\src\AkHouse.Api --output-dir Persistence\Migrations
```

Powinna być **czysto addytywna**: jedna `CreateTable("Operators")` i jeden unikalny indeks na
`Email`. Jeśli EF wygeneruje cokolwiek innego — `DropColumn`, `AlterColumn`, `RenameColumn` —
przerwij i pokaż mi plik.

## 2. Ustaw pierwsze konto

Pierwszy właściciel powstaje przy starcie API i **tylko wtedy, gdy nie ma jeszcze żadnego konta**.
Lokalnie przez `user-secrets`:

```powershell
dotnet user-secrets set "Operators:Bootstrap:Email" "tomek@akhouse.pl" --project "D:\moje\ak-house\backend\src\AkHouse.Api"
dotnet user-secrets set "Operators:Bootstrap:DisplayName" "Tomasz" --project "D:\moje\ak-house\backend\src\AkHouse.Api"
dotnet user-secrets set "Operators:Bootstrap:Password" "<co-najmniej-10-znakow>" --project "D:\moje\ak-house\backend\src\AkHouse.Api"
```

Na produkcji te same trzy wartości jako zmienne środowiskowe:
`Operators__Bootstrap__Email`, `Operators__Bootstrap__DisplayName`, `Operators__Bootstrap__Password`.

To konto dostaje **wszystkie uprawnienia** i flagę „zmień hasło przy pierwszym logowaniu" — bo
hasło startowe siedzi w konfiguracji serwera, więc jest z założenia półjawne.

## 3. Klucze API — teraz poświadczenie maszynowe

Stare klucze **nie znikają**, ale zmieniają rolę: są dla skryptów, nie dla ludzi. Domyślnie
są wyłączone. Żeby `wgraj-ceny.ps1` dalej działał:

```powershell
dotnet user-secrets set "Operators:MachineKeys:Enabled" "true" --project "D:\moje\ak-house\backend\src\AkHouse.Api"
dotnet user-secrets set "Operators:MachineKeys:Shop" "<dotychczasowy Admin:ShopApiKey>" --project "D:\moje\ak-house\backend\src\AkHouse.Api"
```

Analogicznie `Operators:MachineKeys:Leads` i `:Media`, jeśli będą potrzebne.

**Żaden klucz maszynowy nie zarządza operatorami.** Klucz, który mógłby zakładać konta, mógłby
nadać sobie każde inne uprawnienie — więc ta jedna ścieżka jest wyłącznie dla zalogowanego
człowieka. Pilnuje tego test `NoMachineKeyEverManagesOperators`.

Stare `Admin:ApiKey`, `Admin:MediaApiKey` i `Admin:ShopApiKey` przestały być czytane — możesz je
usunąć z `user-secrets`, gdy przepiszesz je na `Operators:MachineKeys:*`.

## 4. Uruchom testy

```powershell
dotnet test backend\AkHouse.slnx
```

Doszło 27 metod: 13 w `OperatorTests` (domena) i 14 w `OperatorApiTests` (integracja).

## Nowe endpointy

| Metoda | Ścieżka | Kto może |
|---|---|---|
| POST | `/api/admin/auth/login` | każdy (limit na IP) |
| POST | `/api/admin/auth/logout` | każdy |
| GET | `/api/admin/auth/me` | każdy — zwraca `null`, gdy nikt nie jest zalogowany |
| POST | `/api/admin/auth/password` | zalogowany, zmienia własne hasło |
| GET / POST | `/api/admin/operators` | uprawnienie `Operators` |
| PUT / DELETE | `/api/admin/operators/{id}` | uprawnienie `Operators` |
| PUT | `/api/admin/operators/{id}/password` | uprawnienie `Operators` |

Uprawnienia: **Leads** (`/admin`), **Media** (`/admin/zdjecia`), **Shop** (`/sklep/panel`),
**Operators** (zarządzanie kontami). Jedno konto może mieć dowolną kombinację.

## Jak to wygląda w przeglądarce

Wszystkie trzy panele — `/admin`, `/admin/zdjecia`, `/sklep/panel` — mają teraz **jeden formularz
logowania** zamiast trzech okienek na klucz. Po zalogowaniu:

- konto z hasłem nadanym przez kogoś innego (albo z konfiguracji) **musi je najpierw zmienić** —
  ekran nie przepuszcza dalej;
- konto bez uprawnienia do danego obszaru widzi, kim jest, co ma, i do kogo się zgłosić —
  zamiast „nieprawidłowy klucz", które nic nie mówi;
- panel **nie wysyła żadnego żądania**, zanim brama nie przepuści — odmowa nie kosztuje zapytania do API.

Konta prowadzi się pod **`/admin/operatorzy`** (przycisk *Konta* w nagłówku zleceń, widoczny tylko
dla tych, którzy mogą go użyć). Tam: zakładanie konta, cztery przełączniki dostępu, wyłączanie
i włączanie konta, ustawienie nowego hasła i usunięcie.

Uprawnienia przełącza się jednym kliknięciem i działają **natychmiast** — osoba, której odbierzesz
sklep, dostanie odmowę przy następnym żądaniu, bez czekania na wygaśnięcie ciasteczka.

## Sprzątnięte

`ApiKeyForm.tsx` i `useAdminAuth.ts` (frontend) oraz `ApiKeyEndpointFilter.cs` (backend) trafiły do
`_do-usuniecia/stare-klucze-api/`. Martwy kod uwierzytelniający to nie jest coś, co warto trzymać.
