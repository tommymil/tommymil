# Moduł „Zamówienia" — co dodano i jeden krok do wykonania

Nowy, wieloetapowy ekran składania zamówień (`/zamowienie`) z pełną obsługą po stronie backendu.
Dolny formularz kontaktowy na stronie głównej **zostaje bez zmian** — jako szybka opcja.

## ⚠️ Jedyny krok ręczny: migracja bazy

Dodano nowe encje (`Order`, `OrderActivity`), więc trzeba raz wygenerować migrację EF Core.
Aplikacja przy starcie sama ją zastosuje (`MigrateAsync`). W katalogu `backend`:

```bash
# jeśli nie masz narzędzi EF:
dotnet tool install --global dotnet-ef

dotnet ef migrations add AddOrders \
  --project src/AkHouse.Infrastructure \
  --startup-project src/AkHouse.Api

# potem normalnie:
dotnet run --project src/AkHouse.Api
```

Po starcie w bazie pojawią się tabele `Orders` i `OrderActivities`.

## Nowe endpointy API

- `POST /api/orders` — publiczny, przyjmuje zamówienie z kreatora (rate-limit jak leady, honeypot anty-spam).
- `GET /api/orders` — chroniony (X-Api-Key), lista ostatnich zamówień.
- `GET /api/orders/{id}` — chroniony, zamówienie + oś czasu.
- `PATCH /api/orders/{id}/status` — chroniony, zmiana statusu.
- `POST /api/orders/{id}/notes` — chroniony, notatka.
- `PATCH /api/orders/{id}/read` — chroniony, oznaczenie przeczytane/nie.

Każde zamówienie dostaje czytelny numer, np. `ZAM-2026-3F9A`, wysyłany klientowi w potwierdzeniu
i pokazywany na ekranie sukcesu.

## Frontend — punkty wejścia prowadzące na `/zamowienie`

- „Umów konsultację" (nagłówek + menu mobilne)
- „Zapytaj o wycenę" w konfiguratorze 3D (przenosi konfigurację do kreatora — naprawia brak przewijania)
- CTA z sekcji: oferta (modal), „O firmie", „Finansowanie"
- Kafelki oferty pre-wybierają typ produktu w kroku 1

Kreator ma 5 kroków (Produkt → Szczegóły → Termin i budżet → Dane → Podsumowanie),
pasek postępu, walidację, możliwość powrotu i cofania, wersję PL/EN oraz responsywność.
