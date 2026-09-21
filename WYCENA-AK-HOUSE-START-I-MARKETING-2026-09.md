# A.K. HOUSE - propozycja rozliczenia produktu i startu marketingu

**Wersja robocza do rozmowy, 17.09.2026.** Kwoty w PLN netto, o ile wykonawca rozlicza VAT. Termin, fakturowanie i prawa do kodu należy potwierdzić w umowie.

## Rekomendacja

| Zakres | Cena jednorazowa |
|---|---:|
| Strona i system obsługi zapytań wraz z finalizacją i uruchomieniem na Azure, bez sklepu | **35 000 zł** |
| Uruchomienie pomiaru i kampanii pilotażowej Google Search, z obsługą pierwszych 30 dni | **2 500 zł** |
| **Razem za obecny etap i start reklam** | **37 500 zł** |

Wcześniejsza wycena `WYCENA-ETAPY-2026-09.md` wynosiła 55 000 zł i obejmowała sklep jako osobną pozycję za 11 000 zł. Porównywalny zakres bez sklepu wynosiłby **44 000 zł**; obecna cena **35 000 zł** zawiera dodatkowy, jednorazowy rabat partnerski **9 000 zł**. Dotyczy wartości już wykonanych prac w tym zakresie **oraz** pozostającej finalizacji, nie wyłącznie prac od daty podpisania umowy. Przygotowanie techniczne pod marketing, które już jest w kodzie, nie jest naliczane drugi raz.

### Co obejmuje 35 000 zł

- Stronę firmową PL/EN, katalog produktów i konfiguratory ofertowe, formularze, galerię i podstrony sprzedażowe.
- API, bazę danych, CRM z obsługą leadów oraz panele operatorów i zarządzania mediami.
- Aktualny fundament SEO i pomiaru: statyczne treści podstron, sitemap, metadane, zapis źródła zapytania w CRM, obsługę zgód i zdarzeń po podłączeniu kont.
- Przygotowanie wydania, konfigurację produkcyjną, wdrożenie na **jednej uzgodnionej maszynie Azure**, domenę i HTTPS, podstawowe monitorowanie, kopię zapasową bazy i zdjęć z próbą odtworzenia, kontrolę działania formularzy i paneli, przekazanie dostępu oraz krótkie szkolenie.
- Dwie rundy poprawek do uzgodnionego zakresu po odbiorze i usuwanie błędów tego zakresu przez 3 miesiące.

**Sklep jest poza obecną wyceną.** Jego kod może już znajdować się we wspólnym repozytorium, ale publiczne uruchomienie, panel sklepu, konta zakupowe, checkout, płatności, testy odbiorowe i wsparcie tego modułu nie są przedmiotem obecnego etapu. Sklep pozostaje wyłączony do osobnego uzgodnienia zakresu, ceny i warunków korzystania z kodu. Wcześniejsze 11 000 zł to punkt odniesienia za wykonany moduł, a nie automatyczna cena jego przyszłej aktywacji; dodatkowe wymagania produkcyjne wyceniamy wtedy osobno.

**Założenie Azure:** obecna aplikacja używa SQLite. Wycena wdrożenia zakłada pojedynczą VM z trwałym dyskiem i kopią zapasową, bez klastra i wysokiej dostępności. Azure App Service for Linux z bazą SQLite na współdzielonym systemie plików nie jest wariantem zakładanym w tej cenie. Jeśli klient wymaga App Service i zarządzanej bazy, migrację na PostgreSQL lub Azure SQL wyceniamy po próbie technicznej osobno, orientacyjnie **6 000-12 000 zł**, bez obiecywania ceny końcowej przed analizą.

### Co obejmuje 2 500 zł za start reklam

- Pomoc w podłączeniu należących do klienta kont Google Search Console, GA4 i Google Ads oraz test zdarzeń i zgód na produkcji; identyfikator Meta Pixel można podłączyć, gdy firma dostarczy konto i zatwierdzi politykę prywatności.
- Plan kampanii na jeden priorytetowy produkt lub grupę produktów i uzgodniony obszar geograficzny.
- Jedną pilotażową kampanię **Google Search**: struktura, podstawowe słowa kluczowe i wykluczenia, teksty reklam na bazie zatwierdzonych faktów, formularz/telefon jako mierzone cele.
- Uruchomienie, kontrolę przez pierwsze 30 dni i krótki raport: wydatki, zapytania, ich jakość według CRM oraz decyzja, co zmienić lub wyłączyć.

To nie jest obietnica liczby leadów ani sprzedaży. Kampania Meta, produkcja filmów i zdjęć, rozbudowany content oraz CAPI/konwersje offline są osobnymi zakresami.

## Płatności

| Termin | Kwota |
|---|---:|
| Po zatwierdzeniu zakresu i umowy | 5 000 zł |
| Odbiór strony publicznej, katalogu i formularzy | 10 000 zł |
| Odbiór CRM, paneli i przekazanie kont operatorów | 10 000 zł |
| Produkcja na Azure, test kopii, szkolenie i przekazanie | 10 000 zł |
| **Obecny zakres bez sklepu razem** | **35 000 zł** |

Marketing: **1 000 zł przed konfiguracją kont i kampanii, 1 500 zł po raporcie z pierwszych 30 dni**. Etapy strony i CRM są harmonogramem odbioru i płatności za już wykonane oraz kończone prace, a nie wyceną samego nowego kodowania.

## Koszty poza ofertą

- **Budżet reklamowy:** na pilotaż przyjąć orientacyjnie **2 000-3 000 zł/mies.**, płatne bezpośrednio przez firmę do Google. Kwotę i limit ustalają właściciele; mały test może dać za mało danych do wiarygodnej oceny.
- **Azure, domena, skrzynki, backup i inne usługi:** płatne przez firmę bezpośrednio dostawcom. Miesięczny koszt chmury trzeba policzyć w Azure Pricing Calculator dla wybranej VM, dysku, backupu, transferu i regionu przed akceptacją architektury. Nie wpisujemy pozornie dokładnej kwoty bez konfiguracji.
- **Opcjonalna obsługa po pilotażu:** 1 200 zł netto/mies. za jeden kanał, miesięczny raport i optymalizację; umowa odnawiana miesięcznie, bez obowiązkowego abonamentu. Drugi kanał i nowe kreacje do osobnej wyceny.
- Sklep i jego późniejsza aktywacja, nowe funkcje, prawnik/regulamin, zatwierdzenie polityki prywatności, sesja zdjęciowa, materiały wideo, stałe publikowanie treści i obsługa sprzedaży nie są wliczone.

## Co musi dostarczyć firma

Konto Azure i uprawnienia wdrożeniowe, dostęp do domeny/DNS, konta Google/Meta należące do firmy, produkcyjne dane kontaktowe i e-mail, zdjęcia z prawem użycia, potwierdzone ceny i warunki transportu/gwarancji oraz zatwierdzone dokumenty prawne. Jedna osoba po stronie firmy powinna akceptować treści i reklamę, a obaj właściciele zatwierdzić budżet.

## Zasada rozmowy

Cena 35 000 zł obejmuje zarówno usunięcie sklepu z poprzedniego zakresu, jak i jednorazowy rabat partnerski. Nie obniżać jej dalej bez zmniejszenia zakresu. Jeśli jednorazowa kwota jest dla nich trudna, rozłożyć płatność według odbiorów lub odsunąć start reklam; nie obiecywać sklepu i nieograniczonych zmian w cenie strony. Ustalić zakres, terminy odpowiedzi klienta, sposób zgłaszania poprawek oraz prawa do wspólnego repozytorium i kont przed wdrożeniem.

## Punkty odniesienia

- [Wcześniejsza wycena projektu](WYCENA-ETAPY-2026-09.md) - historyczny podział 55 000 zł na moduły, w tym 11 000 zł za wyłączony teraz sklep.
- [Microsoft Learn: App Service on Linux FAQ](https://learn.microsoft.com/en-us/troubleshoot/azure/app-service/faqs-app-service-linux-new) - ograniczenie bazy plikowej SQLite na współdzielonym systemie plików.
- [Azure Pricing Calculator](https://azure.microsoft.com/en-us/pricing/calculator/) - koszt środowiska zależy od konfiguracji.
- [WelcomeAds, publiczny cennik](https://welcomeads.pl/cennik/) - orientacja dla opłat za konfigurację i miesięczną obsługę reklam; zakresy ofert nie są identyczne.
