# System do prowadzenia zajęć online z programowania dla dzieci

## Założenia

- zajęcia grupowe dla dzieci,
- jeden instruktor,
- czas trwania zajęć: 90 minut,
- krótka przerwa w połowie,
- nauka programowania w środowiskach takich jak Scratch, Minecraft Education, Roblox Studio lub podobnych,
- rodzic jest klientem i opiekunem konta, a dziecko uczestnikiem zajęć.

System powinien być czymś więcej niż kalendarzem i narzędziem do wideospotkań. Powinien łączyć funkcje:

- CRM,
- dziennika zajęć,
- platformy edukacyjnej,
- systemu płatności,
- komunikacji z rodzicami,
- zarządzania grupami,
- obsługi problemów technicznych,
- obsługi wyjątków i incydentów.

---

## 1. Terminy, odwołania i przesuwanie zajęć

To jeden z najbardziej problematycznych obszarów.

### Typowe sytuacje

- instruktor musi przełożyć zajęcia,
- instruktor zachoruje tuż przed zajęciami,
- rodzic prosi o zmianę terminu tylko dla swojego dziecka,
- większość grupy nie może uczestniczyć w danym dniu,
- zajęcia wypadają w święto, ferie lub dzień wolny,
- instruktor spóźnia się,
- zajęcia zaczynają się później, ale kończą o normalnej godzinie,
- spotkanie trwa krócej z powodu awarii,
- dziecko dołącza 20–30 minut po rozpoczęciu,
- zajęcia trzeba przesunąć na inny tydzień, przez co zmienia się data zakończenia kursu.

### Mniej typowe sytuacje

- tylko część grupy może przyjść w nowym terminie,
- rodzic akceptuje zmianę, ale dziecko dołącza do starego linku,
- termin zostaje zmieniony kilkukrotnie,
- rodzic twierdzi, że nie otrzymał informacji o zmianie,
- zajęcia są zaplanowane w okresie zmiany czasu letniego lub zimowego,
- dziecko przebywa czasowo w innej strefie czasowej,
- zajęcia zostają przypadkowo zdublowane,
- instruktor ma konflikt dwóch spotkań,
- rodzeństwo korzystające z jednego urządzenia ma zajęcia w tym samym czasie.

### Co powinien obsługiwać system

Każde pojedyncze zajęcia powinny być osobnym rekordem, niezależnym od cyklicznego planu grupy. Dzięki temu można zmienić tylko jeden termin bez zmiany całego harmonogramu.

Przykładowe statusy zajęć:

- zaplanowane,
- potwierdzone,
- w trakcie,
- zakończone,
- przełożone,
- odwołane przez instruktora,
- odwołane przez rodzica,
- przerwane technicznie,
- niezrealizowane,
- oczekujące na ustalenie nowego terminu.

Przy każdej zmianie system powinien zapisywać:

- poprzedni termin,
- nowy termin,
- osobę dokonującą zmiany,
- powód,
- datę zmiany,
- sposób powiadomienia rodziców,
- potwierdzenia rodziców.

Warto obsłużyć trzy oddzielne mechanizmy:

1. Przełożenie całych zajęć grupy.
2. Indywidualne odrobienie przez jedno dziecko.
3. Przyznanie kredytu lub dodatkowych zajęć zamiast odrabiania.

Nie należy łączyć zmiany terminu z rozliczeniem. Zajęcia mogą zostać odwołane, ale system osobno powinien decydować, czy należy się:

- zwrot,
- kredyt,
- odrobienie,
- dodatkowe zajęcia,
- brak rekompensaty.

---

## 2. Obecność dzieci

Samo oznaczenie „obecny” lub „nieobecny” będzie niewystarczające.

### Przydatne statusy obecności

- obecny,
- spóźniony,
- nieobecność zgłoszona,
- nieobecność niezgłoszona,
- problemy techniczne,
- opuścił zajęcia wcześniej,
- uczestniczył częściowo,
- obecny, ale nieaktywny,
- odrabiał zajęcia z inną grupą.

System powinien zapisywać orientacyjny czas dołączenia i opuszczenia zajęć. W pierwszej wersji instruktor może zaznaczać to ręcznie.

### Możliwe problemy

- rodzic zgłasza nieobecność po rozpoczęciu zajęć,
- dziecko dołącza, ale nie odpowiada i nie wykonuje zadań,
- dziecko pojawia się tylko na końcu,
- rodzic twierdzi, że dziecko było obecne, a instruktor tego nie potwierdza,
- dziecko dołącza pod nieznaną nazwą,
- rodzeństwo korzysta z jednego konta,
- dziecko dołącza bez dźwięku i przez długi czas nie zauważa problemu,
- dziecko opuszcza spotkanie i nie potrafi ponownie wejść,
- rodzic zapisuje dziecko na kurs, ale ono regularnie nie uczestniczy.

W panelu instruktora powinien być szybki dziennik obecności oraz miejsce na krótką notatkę, na przykład:

- „problemy z mikrofonem”,
- „wyszedł po 40 minutach”,
- „odrobić projekt”,
- „dołączył 15 minut później”.

---

## 3. Problemy techniczne

W zajęciach programowania problemy techniczne będą częstsze niż w zwykłych korepetycjach.

### Internet i urządzenie

- słabe lub niestabilne połączenie,
- zerwane spotkanie,
- brak kamery albo mikrofonu,
- echo i sprzężenie dźwięku,
- dziecko korzysta z telefonu zamiast komputera,
- bardzo wolny komputer,
- brak miejsca na dysku,
- komputer aktualizuje się przed zajęciami,
- rozładowana bateria,
- zepsuta klawiatura lub mysz,
- brak dwóch monitorów utrudniający równoczesne oglądanie i programowanie.

### Programy i konta

- brak zainstalowanego programu,
- nieaktualna wersja programu,
- dziecko nie pamięta hasła,
- rodzic nie ma dostępu do poczty potrzebnej do odzyskania konta,
- konto zostało zablokowane,
- aplikacja wymaga uprawnień administratora,
- kontrola rodzicielska blokuje instalację,
- antywirus lub firewall blokuje połączenie,
- komputer szkolny lub służbowy blokuje instalacje,
- dziecko korzysta z innego systemu operacyjnego niż instruktor,
- projekt utworzony w nowszej wersji nie otwiera się w starszej,
- licencja Minecrafta, Roblox Studio lub innego narzędzia wygasła.

### Pliki i projekty

- dziecko usunęło projekt,
- nadpisało prawidłową wersję błędną,
- zapisało projekt w nieznanym folderze,
- pracowało na innym komputerze,
- projekt nie zsynchronizował się z chmurą,
- rodzic wyczyścił komputer,
- plik jest uszkodzony,
- dziecko wysyła instruktorowi zły plik,
- system nie przyjmuje dużego projektu.

### Funkcje systemu

Przed pierwszymi zajęciami powinien istnieć test techniczny obejmujący:

- test mikrofonu,
- test kamery,
- sprawdzenie przeglądarki,
- sprawdzenie wymaganego programu,
- potwierdzenie konta,
- pobranie pliku testowego,
- uruchomienie przykładowego projektu.

Przy każdych zajęciach warto zapewnić:

- główny link do spotkania,
- zapasowy link,
- instrukcję dołączenia,
- listę wymaganych programów,
- pliki przygotowawcze,
- awaryjne ćwiczenie niewymagające danego programu.

---

## 4. Prowadzenie zajęć i różny poziom dzieci

Nawet dzieci w tym samym wieku będą pracowały w bardzo różnym tempie.

### Typowe problemy

- jedno dziecko kończy zadanie po 10 minutach, inne po 40,
- dziecko nie potrafi sprawnie pisać na klawiaturze,
- dziecko słabo czyta,
- dziecko zna już materiał z wcześniejszego kursu,
- dziecko nie rozumie podstawowych pojęć,
- dziecko kopiuje kod bez rozumienia,
- instruktor zbyt długo pomaga jednej osobie,
- pozostali uczestnicy zaczynają się nudzić,
- dziecko boi się udostępnić ekran,
- dziecko nie zgłasza problemu, tylko czeka,
- dziecko rozprasza się grą lub YouTube,
- uczestnicy przeszkadzają sobie na czacie lub głosowo.

### Mniej typowe problemy

- dziecko celowo psuje wspólny projekt,
- usuwa elementy stworzone przez innych,
- udostępnia nieodpowiednie treści,
- podszywa się pod innego uczestnika,
- zna temat lepiej od instruktora w jednym obszarze i zaczyna podważać prowadzącego,
- rodzic wykonuje zadania za dziecko,
- rodzic cały czas podpowiada i przeszkadza,
- dziecko nagrywa innych bez ich wiedzy,
- dziecko ma trudności wymagające innego sposobu prowadzenia zajęć.

### Co powinien wspierać system

Każda lekcja powinna mieć:

- cel podstawowy,
- zadanie obowiązkowe,
- prostszą wersję zadania,
- zadania dodatkowe dla szybszych dzieci,
- gotowy projekt startowy,
- oczekiwany efekt końcowy,
- materiały dla instruktora,
- instrukcję krok po kroku,
- zadanie awaryjne.

Przy dziecku warto przechowywać:

- poziom,
- tempo pracy,
- znajomość poszczególnych środowisk,
- informacje o problemach technicznych,
- ostatnio ukończony etap,
- projekty,
- notatki instruktora,
- rekomendację dalszego poziomu.

Dobrze, żeby instruktor widział podczas zajęć prosty panel:

- kto jest obecny,
- kto potrzebuje pomocy,
- kto skończył zadanie,
- kto ma problem techniczny,
- komu należy przygotować dodatkowe ćwiczenie.

---

## 5. Specyfika zajęć 90-minutowych

Dla dzieci 90 minut przy komputerze to długo. System powinien wspierać sensowną strukturę zajęć.

### Przykładowy układ zajęć

- 5–10 minut: przywitanie i przypomnienie,
- 15 minut: pokaz nowego elementu,
- 20 minut: praca dzieci,
- 5 minut: przerwa,
- 25 minut: projekt główny,
- 10 minut: zadania dodatkowe i pomoc,
- 5 minut: prezentacja efektów.

### Możliwe problemy

- instruktor zapomni o przerwie,
- pierwsza część zajęć się przedłuży,
- konfiguracja techniczna zajmie 30 minut,
- dzieci stracą koncentrację,
- nie zostanie czas na zapisanie projektu,
- spotkanie zakończy się automatycznie po określonym czasie.

Przydatny byłby widok scenariusza lekcji z:

- minutnikiem,
- listą etapów,
- możliwością oznaczania wykonanych części,
- przypomnieniem o przerwie,
- możliwością zapisania, czego nie udało się zrealizować.

---

## 6. Komunikacja z rodzicami

Rodzic jest klientem i osobą decyzyjną, ale uczestnikiem jest dziecko. System musi rozdzielać te role.

### Potrzebne wiadomości

- przypomnienie o zajęciach,
- informacja o zmianie terminu,
- prośba o potwierdzenie,
- informacja o nieobecności,
- wiadomość o problemach technicznych,
- krótkie podsumowanie zajęć,
- informacja o zadaniu domowym,
- ostrzeżenie o niewłaściwym zachowaniu,
- przypomnienie o płatności,
- informacja o końcu pakietu lub kursu.

### Możliwe problemy

- wiadomość trafia do spamu,
- rodzic zmienia numer telefonu,
- wiadomości otrzymuje tylko jeden z opiekunów,
- jeden rodzic zgadza się na zmianę, drugi nie,
- rodzice są skonfliktowani,
- rodzic kontaktuje się bezpośrednio z instruktorem późnym wieczorem,
- wiadomości są wysyłane w wielu kanałach i ustalenia się rozjeżdżają,
- rodzic twierdzi, że instruktor obiecał coś telefonicznie.

Najważniejsze ustalenia powinny być zapisywane w systemie. Instruktor nie powinien być zmuszony do obsługiwania spraw równocześnie przez:

- telefon,
- e-mail,
- komunikator,
- wiadomości prywatne,
- panel systemu.

---

## 7. Płatności, odrabianie i rozliczenia

Ten obszar szybko stanie się skomplikowany.

### Możliwe modele i scenariusze

- opłata miesięczna,
- pakiet określonej liczby zajęć,
- płatność za pojedyncze zajęcia,
- pierwsze zajęcia próbne,
- dołączenie w połowie miesiąca,
- rezygnacja w połowie miesiąca,
- zawieszenie uczestnictwa,
- rabat dla rodzeństwa,
- promocja,
- nieudana płatność,
- nadpłata,
- zwrot,
- reklamacja płatności,
- zajęcia odwołane przez instruktora,
- nieobecność dziecka,
- nieobecność zgłoszona odpowiednio wcześnie,
- awaria techniczna po stronie organizatora,
- awaria po stronie uczestnika,
- przyznanie bezpłatnego odrobienia.

### Kredyty zajęciowe

W systemie warto zastosować mechanizm kredytów zajęciowych.

Kredyt może być przyznany za odwołane zajęcia i wykorzystany na:

- zajęcia indywidualne,
- odrabianie z inną grupą,
- warsztaty dodatkowe,
- przedłużenie kursu,
- pomniejszenie kolejnej płatności.

Każda operacja finansowa powinna mieć:

- powód,
- datę,
- osobę wykonującą,
- wartość,
- powiązanie z konkretnymi zajęciami,
- historię zmian.

Instruktor lub administrator nie powinien ręcznie zmieniać kwot bez pozostawienia śladu.

---

## 8. Bezpieczeństwo dzieci i prywatność

To krytyczny obszar.

System powinien ograniczać bezpośredni kontakt prywatny między dzieckiem a instruktorem. Główna komunikacja powinna przechodzić przez konto opiekuna albo oficjalny kanał platformy.

### Należy przewidzieć

- zgody opiekunów,
- zgodę lub brak zgody na nagrywanie,
- oddzielną zgodę na publikację prac dziecka,
- ograniczenie widoczności danych innych uczestników,
- bezpieczne nazwy użytkowników,
- kontrolę dostępu do spotkania,
- poczekalnię przed wejściem,
- możliwość usunięcia nieznanej osoby,
- blokadę prywatnych wiadomości między dziećmi,
- historię działań administratora i instruktora,
- procedurę zgłaszania niewłaściwego zachowania.

### Trudne sytuacje

- nieznana osoba dołącza do spotkania,
- dziecko udostępnia adres, numer telefonu lub inne dane,
- na ekranie dziecka pojawiają się prywatne wiadomości,
- uczestnik pokazuje nieodpowiednią treść,
- dochodzi do wyśmiewania lub nękania,
- dziecko zgłasza instruktorowi niepokojącą sytuację domową,
- rodzic żąda usunięcia danych i nagrań,
- ktoś publikuje nagranie zajęć poza systemem.

Powinien istnieć moduł incydentów zawierający:

- datę,
- osoby,
- opis,
- działania podjęte,
- załączniki,
- status sprawy,
- osobę odpowiedzialną.

Takich notatek nie należy mieszać ze zwykłymi uwagami edukacyjnymi.

---

## 9. Zarządzanie grupami

### Problemy, które mogą pojawić się z czasem

- dziecko jest za słabe lub za mocne dla grupy,
- grupa jest zbyt liczna,
- grupa ma zbyt duże różnice wieku,
- dziecko chce przejść na inny dzień,
- dwie grupy realizują materiał w innym tempie,
- grupy rozpoczynają kurs w różnych terminach,
- dziecko odrabia w grupie będącej kilka lekcji dalej,
- grupa zostaje rozwiązana z powodu małej liczby uczestników,
- dziecko chce wrócić po kilkumiesięcznej przerwie,
- nowy uczestnik dołącza do istniejącej grupy.

### System powinien wspierać

- limit uczestników,
- listę rezerwową,
- poziomy zaawansowania,
- przenoszenie uczestników między grupami,
- historię członkostwa w grupie,
- indywidualny postęp niezależny od postępu grupy,
- lekcje wyrównawcze,
- oznaczenie materiału wymaganego przed dołączeniem,
- grupy próbne,
- grupy tymczasowe do odrabiania.

---

## 10. Nieobecność instruktora

Nawet przy jednym instruktorze system powinien być przygotowany na przyszłego zastępcę.

### Potrzebne elementy

- scenariusz każdej lekcji,
- ostatni ukończony temat,
- notatki o grupie,
- lista problemów dzieci,
- pliki startowe,
- przykładowe rozwiązanie,
- informacja, czego nie zdążono zrobić,
- uprawnienia czasowe dla zastępcy.

Gdy nie ma zastępcy, system powinien umożliwiać jednym działaniem:

1. odwołanie zajęć,
2. podanie powodu,
3. wysłanie informacji rodzicom,
4. przyznanie kredytu lub zaproponowanie terminu,
5. przesunięcie kolejnych numerów lekcji.

---

## 11. Postępy i projekty dzieci

Rodzic będzie chciał wiedzieć, czego dziecko się nauczyło.

### Warto przechowywać

- ukończone lekcje,
- zdobyte umiejętności,
- projekty,
- zrzuty ekranu lub linki do projektów,
- stopień samodzielności,
- zadania do poprawy,
- krótkie podsumowania okresowe.

Nie należy opierać oceny wyłącznie na tym, czy projekt działa. Ważne może być również:

- czy dziecko rozumie rozwiązanie,
- czy potrafi samodzielnie znaleźć błąd,
- czy współpracuje,
- czy potrafi rozbudować projekt,
- czy korzysta z gotowych rozwiązań bez zrozumienia.

### Przykładowa skala postępu

- wymaga pełnej pomocy,
- wykonuje z pomocą,
- wykonuje samodzielnie,
- potrafi wyjaśnić innym,
- potrafi rozbudować rozwiązanie.

---

## 12. Obsługa reklamacji i sporów

### Przykładowe reklamacje

- „Zajęcia się nie odbyły”.
- „Instruktor przez większość czasu pomagał innemu dziecku”.
- „Moje dziecko niczego się nie nauczyło”.
- „Link nie działał”.
- „Nie dostaliśmy informacji o zmianie”.
- „Dziecko zostało usunięte ze spotkania”.
- „Pobrano opłatę mimo rezygnacji”.
- „Projekt dziecka zniknął”.
- „Instruktor zachował się nieodpowiednio”.

### Potrzebne dane

- historia wiadomości,
- historia terminów,
- obecności,
- notatki instruktora,
- log wysłanych powiadomień,
- historia płatności,
- historia zmian danych,
- możliwość oznaczenia sprawy jako reklamacji,
- status i osoba odpowiedzialna,
- historia rozwiązania sprawy.

---

## 13. Najważniejsze encje w bazie danych

System prawdopodobnie będzie potrzebował co najmniej następujących encji:

- `Child` – dziecko,
- `Guardian` – opiekun,
- `GuardianChildRelation` – powiązanie dziecka z opiekunami,
- `Instructor` – instruktor,
- `Course` – kurs,
- `Group` – grupa,
- `GroupMembership` – członkostwo dziecka w grupie,
- `LessonPlan` – scenariusz lekcji,
- `LessonOccurrence` – konkretne zajęcia w konkretnym terminie,
- `Attendance` – obecność,
- `RescheduleRequest` – prośba o zmianę terminu,
- `MakeupLesson` – zajęcia odrabiane,
- `LessonCredit` – kredyt zajęciowy,
- `Payment` – płatność,
- `Invoice` – faktura lub dokument rozliczeniowy,
- `Project` – projekt dziecka,
- `ProjectSubmission` – przesłanie projektu,
- `ProgressEntry` – wpis o postępie,
- `Message` – wiadomość,
- `Notification` – powiadomienie,
- `Consent` – zgoda,
- `TechnicalCheck` – test techniczny,
- `SupportTicket` – zgłoszenie techniczne,
- `Incident` – incydent,
- `AuditLog` – historia działań.

### Ważne rozróżnienie

Należy rozdzielić:

- kurs, na przykład „Roblox Studio – podstawy”,
- grupę, czyli konkretne dzieci i instruktor,
- plan lekcji, na przykład „Budowanie toru przeszkód”,
- wystąpienie lekcji, czyli zajęcia 8 września o 17:00.

---

## 14. Zakres sensownego MVP

Na pierwszą wersję najważniejsze byłyby:

1. Konta opiekunów i dzieci.
2. Grupy i przypisywanie dzieci.
3. Cykliczny kalendarz z możliwością zmiany pojedynczych zajęć.
4. Link do spotkania online.
5. Automatyczne przypomnienia.
6. Obecność i spóźnienia.
7. Odwołanie, przełożenie i odrabianie.
8. Notatka po zajęciach.
9. Scenariusze i materiały lekcji.
10. Przechowywanie projektów lub linków do nich.
11. Płatności, kredyty i podstawowe rozliczenia.
12. Historia komunikacji.
13. Podstawowe zgody i uprawnienia.
14. Rejestr problemów technicznych i incydentów.

---

## 15. Funkcje na później

W kolejnych etapach można dodać:

- automatyczne tworzenie spotkań,
- listy rezerwowe,
- dobieranie terminu odrabiania,
- zastępstwa instruktorów,
- panel postępów dla rodzica,
- certyfikaty,
- odznaki i elementy grywalizacji,
- automatyczne kopie projektów,
- nagrania wybranych fragmentów,
- raporty rentowności grup,
- ankiety satysfakcji,
- aplikację mobilną dla rodziców,
- automatyczne wykrywanie dzieci wymagających zmiany poziomu.

---

## 16. Najważniejsza zasada projektowa

Nie należy budować systemu, w którym zajęcia są tylko wydarzeniem w kalendarzu.

Każde zajęcia powinny mieć własne:

- termin,
- status,
- obecności,
- rozliczenie,
- materiały,
- notatkę,
- problemy techniczne,
- historię zmian,
- informacje o odrabianiu,
- powiązane wiadomości,
- powiązane projekty dzieci.

Dzięki temu większość typowych i nietypowych sytuacji będzie można obsłużyć bez ręcznego poprawiania danych w bazie.
