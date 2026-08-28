# Zasady projektu A.K. HOUSE

Strona i sklep producenta domków mobilnych, saun i mebli na wymiar.
Ten plik opisuje **jak pisać kod w tym repozytorium**. Opis *co* system robi
(architektura, endpointy, konfiguracja) jest w `README.md` i nie jest tu powtarzany.

## Źródła prawdy — czytaj zanim zaczniesz

| Plik | Co z niego bierzesz |
|---|---|
| `README.md` | Architektura, lista endpointów, konfiguracja, komendy. **Jedyne** miejsce na taki opis. |
| `STAN-PRAC.md` | **Zacznij tutaj.** Pełny stan projektu: co jest zrobione, co zweryfikowane, co zostało. |
| `WYCENA.md` | Metryki i zakres — dokument handlowy, nie techniczny. |
| `tools/import-cennik/README.md` | Import cennika dostawcy, marża i VAT, ograniczenia. |
| `gemini-code-*.md` | Checklista UX/UI (wymagania produktowe, nie reguły kodu). |

Zmieniasz architekturę albo endpoint → **zaktualizuj `README.md` w tym samym commicie**.
Nie zakładaj nowego pliku `.md` na opis, który ma już swoje miejsce w tabeli powyżej.

`ORDERS_SETUP.md` opisuje moduł `Orders`, który został zastąpiony wspólnym lejkiem leadów —
traktuj go jako archiwum, nie jako stan obecny.

## Struktura repozytorium i submoduły

```
ak-house/            ← repo nadrzędne (dokumentacja, tools/, wskaźniki submodułów)
├─ backend/          ← SUBMODUŁ  github.com/tommymil/ak-house-backend  (gałąź master)
└─ frontend/         ← SUBMODUŁ  github.com/tommymil/ak-house-frontend (gałąź main)
```

`backend` i `frontend` to **osobne repozytoria**. Zmiana kodu wymaga zwykle **dwóch commitów**:
najpierw w submodule, potem w repo nadrzędnym (przesunięcie wskaźnika). Commit tylko na górze
zapisuje wskaźnik na nieistniejący u innych stan.

```bash
cd frontend && git add -A && git commit -m "…" && git push
cd ..       && git add frontend && git commit -m "Zaktualizuj frontend" && git push
```

Repo nadrzędne stoi na gałęzi **`1.0`**. Gałąź zdalna `origin/program-1.0` zawiera **inny projekt**
(„Lesson Runner") wraz z własnym `CLAUDE.md` — trafiła tu przez pomyłkę. **Nigdy nie czerp z niej
reguł ani kodu**; docelowo do usunięcia ze zdalnego repo.

## Język

- **Kod jest po angielsku**: nazwy typów, pól, plików, tras, kluczy, wartości enumów,
  komunikatów commitów oraz **komentarzy i dokumentacji XML/JSDoc**. To dominująca konwencja
  w repo (~95% komentarzy) — nie mieszaj.
- **Po polsku, z pełnymi diakrytykami** (ą, ć, ę, ł, ń, ó, ś, ź, ż): wszystko, co widzi
  użytkownik lub operator — teksty UI i i18n, treści seed, e-maile, dokumenty `.md`,
  komentarze w plikach konfiguracyjnych (`.csproj`, `vite.config.ts`).
- Tekstów UI nie wpisuj na sztywno w komponencie — idą przez `app/i18n.tsx` (PL/EN).
  Sklep jest wyłącznie polskojęzyczny i celowo nie ma lustra `/en`.

## Reguły domenowe, które łatwo złamać

Te decyzje kosztowały czas i mają uzasadnienie w `README.md`. Nie odwracaj ich bez rozmowy:

- **Pieniądze to grosze w `int`.** SQLite zapisuje `decimal` jako TEXT, więc `SUM` i `ORDER BY`
  po cenie dawałyby złe wyniki. W panelu wpisuje się złotówki, do bazy idą grosze.
- **Cenę liczy serwer.** Z przeglądarki przychodzą wyłącznie `productId` i `quantity`.
  Cena w koszyku w `localStorage` służy tylko do podglądu.
- **Narzut jest aplikowany, nie kumulowany.** Cena sklepowa = `BasePriceGrosze` przypisanego
  dostawcy przepuszczona przez jego `ShopSupplier.MarginBasisPoints`. Liczy się **zawsze od bazy**, nigdy od bieżącej ceny —
  inaczej dwa kliknięcia dawałyby +21% zamiast +10%, a powrót do cen dostawcy byłby niemożliwy.
  Narzut trzymamy w punktach bazowych jako `int` (1% = 100). Arytmetyka jest zdublowana w panelu
  (`features/shopadmin/pricing.ts`) i **musi zgadzać się z `ShopPricing.Apply` co do grosza**;
  obie strony mają na to testy.
- **Pozycja zamówienia trzyma własną kopię nazwy i ceny.** Zmiana cennika nie przepisuje historii.
- **`ShopProduct.StockQuantity` jest opcjonalny i `null` znaczy co innego niż `0`.** `null` to
  pozycja bez prowadzonego magazynu — sprowadzana od dostawcy albo sprzedawana na metry — z limitem
  `DefaultMaxOrderQuantity` (99 szt.) na zamówienie. `0` to wyprzedane: pozycja znika ze sprzedaży,
  ale zostaje widoczna. `IsAvailable` jest osobnym, ręcznym przełącznikiem i licznik go nie
  przepisuje. Limitu pilnuje **`ShopOrder.Place`**, nie pole „Ilość" w przeglądarce ani przycinanie
  koszyka — jedno i drugie jest uprzejmością wobec klienta, nie zabezpieczeniem.
- **Publiczne formularze**: zgoda RODO + honeypot + rate-limit per IP. Dokładając nowy publiczny
  endpoint przyjmujący dane, dołóż wszystkie trzy.
- **Endpoint administracyjny musi mieć `.RequireOperator(OperatorPermissions.X)`.** Brak bramy
  = publiczny dostęp do danych klientów. Uprawnienia są **cztery i mają takie zostać** — każde
  nadaje się i odbiera niezależnie: `Leads` (CRM, `/crm`), `Media` (zdjęcia,
  `/zdjecia`), `Shop` (sklep, `/sklep/panel`), `Operators` (konta, `/operatorzy`).
  Nie podpinaj `/api/admin/shop` pod uprawnienie od zleceń — to, że obsługa asortymentu nie
  otwiera kartoteki klientów, jest całym sensem rozdziału. Pilnują tego testy w
  `ShopAdminApiTests` i `OperatorApiTests`.
- **Uprawnienia czyta się z bazy przy każdym żądaniu**, nigdy z ciasteczka. Odebranie dostępu ma
  działać natychmiast, a nie dopiero po wygaśnięciu sesji.
- **Klucz w nagłówku to poświadczenie maszynowe** (`Operators:MachineKeys`), domyślnie wyłączone
  i osobne dla każdego obszaru. Żaden klucz nie zarządza operatorami — mógłby nadać sobie każde
  inne uprawnienie.
- **Konta personelu to `Operator`, nie Identity.** Identity trzyma klientów sklepu. Jedna tabela
  na oba rodzaje postawiłaby publiczną rejestrację i uprawnienia do panelu za tymi samymi drzwiami.
- **Kategorie sklepu są w bazie**, nie w enumie. `ShopProducts.Category` trzyma `ShopCategory.Key`
  bez klucza obcego — spójności pilnuje `ShopAdminService`. Nie dodawaj tam FK ani nie zmieniaj
  szerokości tej kolumny: jedno i drugie wymusza przebudowę tabeli SQLite na żywym katalogu.
- **Sesja klienta to ciasteczko `HttpOnly`**, nie token w `localStorage`.
- Upload przyjmuje JPG/PNG/WEBP/AVIF do 8 MB. **SVG jest odrzucany celowo** — wykonuje JavaScript
  po otwarciu bezpośrednio.

## Czego nie ruszać

**Konfigurator 3D (WebGL / Three.js) jest zawieszony.** Nie prowadzi do niego żadna trasa.
Kod zostaje w repo, ale go nie rozwijaj ani nie „naprawiaj" bez decyzji o wznowieniu — lista
zawieszonych plików jest w `README.md`. Wybór wariantu odbywa się bez 3D, w
`frontend/src/features/catalog/variants.tsx`.

## Sekrety

Nigdy nie wpisuj do repo: `Operators:Bootstrap:Password`, `Operators:MachineKeys:*`,
`Email:SmtpPassword`, `Shop:BankAccountNumber`, `Payments:Stripe:SecretKey`,
`Payments:Stripe:WebhookSecret`. Lokalnie idą przez `dotnet user-secrets`, na produkcji przez
zmienne środowiskowe. Baza `akhouse.db` i `*.db-wal` są w `.gitignore` — tak zostaje.

## Weryfikacja zmian

Zmiana jest gotowa dopiero, gdy przechodzi **komplet** poniższych komend:

```bash
dotnet build backend/AkHouse.slnx
dotnet test  backend/AkHouse.slnx        # obecnie 148 metod testowych
npm --prefix frontend run lint           # 0 błędów (ostrzeżenia dopuszczalne, patrz niżej)
npm --prefix frontend run typecheck
npm --prefix frontend test               # obecnie 131 testów
npm --prefix frontend run build
```

Nowa logika domenowa → test w `backend/tests/AkHouse.Tests/Domain`.
Nowy endpoint → test w `.../Integration` (włącznie z bramą uprawnień).
Nowy view-model → test obok pliku (`*.test.ts`).

## Narzędzia wymuszające styl

- `.editorconfig` (root) — wcięcia, końce linii, kodowanie. Obowiązuje oba submoduły.
- `backend/Directory.Build.props` — wspólne `TargetFramework`, `Nullable`, `ImplicitUsings`
  dla wszystkich projektów. **Nie powielaj tych właściwości w `.csproj`.**
  W konfiguracji `Release` ostrzeżenia kompilatora są traktowane jak błędy.
- `frontend/eslint.config.js` — `npm run lint`. Bramka przepuszcza ostrzeżenia
  (`react-refresh/only-export-components`, `react-hooks/set-state-in-effect`,
  `react-hooks/exhaustive-deps` — łącznie 36 sztuk zastanych). **Nie dokładaj nowych**;
  istniejące są do stopniowego wygaszenia.

Reguły szczegółowe warstw: `backend/CLAUDE.md` i `frontend/CLAUDE.md`.
