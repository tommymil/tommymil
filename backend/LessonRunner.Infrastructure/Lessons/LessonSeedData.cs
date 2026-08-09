using LessonRunner.Domain.Lessons;

namespace LessonRunner.Infrastructure.Lessons;

internal static class LessonSeedData
{
    public static IReadOnlyList<Lesson> Create()
    {
        return
        [
            new Lesson
            {
                Id = Guid.Parse("5f3f8f6d-5b2c-4bc6-9c4b-b8f7d0997ef1"),
                Title = "Pierwsza gra: ruch postaci",
                Subject = "Scratch",
                Level = "Poziom 1 - 8-10 lat",
                Order = 1,
                Status = LessonStatus.Ready,
                Description = "Dzieci tworzą pierwszą grę, sterują kotkiem strzałkami i zbierają jabłka.",
                Tags = ["Pętle", "Sterowanie", "Wykrywanie dotknięcia"],
                Steps =
                [
                    new LessonStep
                    {
                        Order = 1,
                        Type = LessonStepType.Intro,
                        Title = "Powitanie i cel lekcji",
                        DurationMinutes = 3,
                        Script =
                        [
                            "Przywitaj się z grupą i zapytaj, kto grał dziś w jakąś grę na komputerze albo telefonie.",
                            "Powiedz: Dziś sami zbudujemy grę. Nasz kotek będzie chodził po ekranie, a my zbierzemy nim jabłka.",
                            "Pokaż gotową grę na swoim ekranie przez około 20 sekund, żeby dzieci wiedziały, do czego dążymy."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Cel: zbudujemy grę, w której sterujemy kotkiem strzałkami i zbieramy jabłka." },
                            new StudentItem { Kind = StudentItemKind.Image, Caption = "gotowa gra - kotek i jabłka" }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Link, Label = "Otwórz Scratch - nowy projekt", Url = "https://scratch.mit.edu/projects/editor" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Pace, Text = "Jeśli grupa jest energiczna, skróć pokaz do 10 s." },
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Zapisz na tablicy 3 słowa-cele: RUCH, STEROWANIE, PUNKTY." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 2,
                        Type = LessonStepType.Review,
                        Title = "Co już potrafimy",
                        DurationMinutes = 4,
                        Script =
                        [
                            "Przypomnij wspólnie: gdzie jest scena, gdzie są bloki, jak przeciągamy blok do skryptu.",
                            "Zadaj pytanie kontrolne: Od jakiego bloku zwykle zaczynamy program?",
                            "Poproś, żeby każde dziecko znalazło blok zielonej flagi i położyło go na scenie skryptów."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Przypominamy: zielona flaga uruchamia program. Bloki przeciągamy myszką do obszaru skryptów." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Scratch", Code = "gdy kliknięto zieloną flagę" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Dzieci mylą kategorie Zdarzenia ze Sterowaniem - pokaż na kolorach." },
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Kto skończył szybciej, niech zmieni tło sceny." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 3,
                        Type = LessonStepType.Concept,
                        Title = "Pętla zawsze i bloki ruchu",
                        DurationMinutes = 8,
                        Script =
                        [
                            "Wyjaśnij pętle zawsze: komputer powtarza środek w kółko, bez końca.",
                            "Pokaż bloki zmień x o 10 i zmień y o 10.",
                            "Razem zbudujcie szkielet: zielona flaga -> zawsze -> puste miejsce na ruch."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Pętla zawsze powtarza polecenia w środku bez końca." },
                            new StudentItem { Kind = StudentItemKind.Text, Text = "x = lewo / prawo, y = góra / dół." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Scratch", Code = "gdy kliknięto zieloną flagę\nzawsze\n  // tutaj za chwilę dodamy ruch" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Najczęstszy błąd: blok ruchu poza pętlą zawsze." },
                            new LessonNote { Kind = LessonNoteKind.Pace, Text = "Szybsza grupa: wprowadź od razu obrót o 15 stopni." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 4,
                        Type = LessonStepType.Demo,
                        Title = "Demo: kotek rusza strzałkami",
                        DurationMinutes = 10,
                        Script =
                        [
                            "Buduj na żywo, mówiąc każdy krok na głos.",
                            "Dodaj wewnątrz pętli: jeżeli klawisz strzałka w prawo naciśnięty, zmień x o 10.",
                            "Zapytaj: Co zmienić, żeby kotek chodził w lewo?"
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Dodajemy sterowanie: gdy naciśniesz strzałkę, kotek się przesuwa." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Scratch", Code = "gdy kliknięto zieloną flagę\nzawsze\n  jeżeli <klawisz [strzałka w prawo] naciśnięty?> to\n    zmień x o (10)\n  jeżeli <klawisz [strzałka w lewo] naciśnięty?> to\n    zmień x o (-10)" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Minus przy -10 bywa pomijany." },
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Czasem dzieci wybierają zły klawisz z listy." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 5,
                        Type = LessonStepType.Guided,
                        Title = "Ćwiczenie z prowadzeniem: pełne sterowanie",
                        DurationMinutes = 12,
                        Script =
                        [
                            "Dzieci dodają samodzielnie górę i dół.",
                            "Co 3-4 minuty rób check-in.",
                            "Pomagaj indywidualnie tym, którzy utknęli, zadając pytania naprowadzające."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Twoje zadanie: spraw, by kotek chodził też w górę i w dół." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Scratch", Code = "jeżeli <klawisz [strzałka w górę] naciśnięty?> to\n  zmień y o (10)\njeżeli <klawisz [strzałka w dół] naciśnięty?> to\n  zmień y o (-10)" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Pytania zamiast odpowiedzi: który klawisz wybrałeś? Co robi plus, a co minus?" },
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Dzieci mylą x z y przy ruchu pionowym." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 6,
                        Type = LessonStepType.Challenge,
                        Title = "Samodzielne wyzwanie: zbierz jabłko",
                        DurationMinutes = 12,
                        Script =
                        [
                            "Postaw wyzwanie: dodaj jabłko. Gdy kotek go dotknie, jabłko ucieka w losowe miejsce.",
                            "Podpowiadaj składniki, nie gotowe rozwiązanie.",
                            "Zostaw około 8 minut na pracę i 2 minuty na pokazanie gier."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Wyzwanie: dodaj jabłko. Gdy kotek je dotknie, jabłko przeskakuje w losowe miejsce." },
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Dla chętnych: dodaj licznik punktów." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Scratch", Code = "// skrypt w duszku jabłko\ngdy kliknięto zieloną flagę\nzawsze\n  jeżeli <dotyka [Kotek]?> to\n    idź do losowej pozycji" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Pace, Text = "Miej dodatek dla szybkich i uproszczenie dla wolniejszych." },
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Skrypt jabłka bywa wpisany w kotku." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 7,
                        Type = LessonStepType.Summary,
                        Title = "Podsumowanie i zapisanie projektu",
                        DurationMinutes = 4,
                        Script =
                        [
                            "Zbierz grupę z powrotem i zapytaj, co dziś zbudowaliśmy.",
                            "Powtórzcie 3 słowa z tablicy: RUCH, STEROWANIE, PUNKTY.",
                            "Przypomnij o zapisaniu projektu i zapowiedz następną lekcję."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Dziś nauczyliśmy się: pętła zawsze, sterowanie klawiszami, wykrywanie dotknięcia." },
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Pamiętaj: zapisz projekt." }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Pochwal imiennie 2-3 osoby za konkret." },
                            new LessonNote { Kind = LessonNoteKind.Pace, Text = "Jeśli zostało czasu: szybka runda: pokaz swój ekran." }
                        ]
                    }
                ]
            },
            new Lesson
            {
                Id = Guid.Parse("a1d2c3b4-0001-4f10-9a20-000000000002"),
                Title = "Animacja: kotek mówi i zmienia kostiumy",
                Subject = "Scratch",
                Level = "Poziom 1 - 8-10 lat",
                Order = 2,
                Status = LessonStatus.Ready,
                Description = "Dzieci animują postać: dymek z tekstem, zmiana kostiumów i prosty dźwięk.",
                Tags = ["Kostiumy", "Dźwięk", "Wygląd"],
                Steps =
                [
                    new LessonStep
                    {
                        Order = 1,
                        Type = LessonStepType.Intro,
                        Title = "Co dziś animujemy",
                        DurationMinutes = 3,
                        Script =
                        [
                            "Zapytaj, czy ktoś widział kreskówkę - jak postacie się poruszają i mówią.",
                            "Powiedz: dziś nasz kotek będzie mówił dymkiem i zmieniał wygląd.",
                            "Pokaż efekt końcowy na swoim ekranie przez około 15 sekund."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Cel: kotek przywita się dymkiem i zmieni kostium." },
                            new StudentItem { Kind = StudentItemKind.Image, Caption = "kotek z dymkiem 'Cześć!'" }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Link, Label = "Otwórz Scratch", Url = "https://scratch.mit.edu/projects/editor" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Pace, Text = "Energiczna grupa: skróć pokaz do 8 s." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 2,
                        Type = LessonStepType.Concept,
                        Title = "Blok mów i dymek",
                        DurationMinutes = 6,
                        Script =
                        [
                            "Pokaż kategorie Wygląd i blok 'mów [Cześć!] przez (2) sekundy'.",
                            "Razem dodajcie blok pod zieloną flagę.",
                            "Niech każde dziecko zmieni tekst dymka na swoje imię."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Blok 'mów' pokazuje dymek nad postacią przez podany czas." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Scratch", Code = "gdy kliknięto zieloną flagę\nmów [Cześć, jestem kotek!] przez (2) sekundy" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Dzieci szukają bloku w złej kategorii - to Wygląd, nie Zdarzenia." },
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Kto skończył: niech doda drugi dymek po pierwszym." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 3,
                        Type = LessonStepType.Guided,
                        Title = "Zmiana kostiumów",
                        DurationMinutes = 8,
                        Script =
                        [
                            "Pokaż zakładkę Kostiumy - kotek ma dwa kostiumy.",
                            "Dodajcie pętle 'zawsze' z blokiem 'następny kostium' i 'czekaj (0.3) s'.",
                            "Obserwujcie, jak kotek zaczyna 'chodzić' w miejscu."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Twoje zadanie: spraw, by kotek zmieniał kostiumy w pętli." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Scratch", Code = "zawsze\n  następny kostium\n  czekaj (0.3) sekundy" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Bez bloku 'czekaj' kostiumy migają za szybko." },
                            new LessonNote { Kind = LessonNoteKind.Pace, Text = "Szybsza grupa: dodaj zmianę czasu czekania." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 4,
                        Type = LessonStepType.Challenge,
                        Title = "Dodaj dźwięk",
                        DurationMinutes = 8,
                        Script =
                        [
                            "Postaw wyzwanie: gdy klikniesz kotka, ma zagrać dźwięk 'Meow'.",
                            "Podpowiedź kategorie Dźwięk i blok 'zagraj dźwięk'.",
                            "Zostaw czas na samodzielną próbę i krótki pokaz."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Wyzwanie: kliknięcie kotka odtwarza dźwięk." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Scratch", Code = "gdy kliknięty\nzagraj dźwięk [Meow] do końca" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Pytaj naprowadzająco: od jakiego zdarzenia ma się zacząć?" }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 5,
                        Type = LessonStepType.Summary,
                        Title = "Podsumowanie",
                        DurationMinutes = 3,
                        Script =
                        [
                            "Zapytaj: jakie trzy bloki dziś poznaliśmy?",
                            "Powtórzcie: mów, następny kostium, zagraj dźwięk.",
                            "Przypomnij o zapisaniu projektu."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Dziś: dymek, zmiana kostiumów i dźwięk." }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Pochwal konkretny pomysł jednego dziecka." }
                        ]
                    }
                ]
            },
            new Lesson
            {
                Id = Guid.Parse("a1d2c3b4-0002-4f10-9a20-000000000003"),
                Title = "Python: zmienne i rozmowa z komputerem",
                Subject = "Python",
                Level = "Poziom 2 - 11-13 lat",
                Order = 3,
                Status = LessonStatus.Review,
                Description = "Pierwszy kontakt z Pythonem: print, input i zmienne tekstowe.",
                Tags = ["Zmienne", "Input", "Print"],
                Steps =
                [
                    new LessonStep
                    {
                        Order = 1,
                        Type = LessonStepType.Intro,
                        Title = "Czym jest program tekstowy",
                        DurationMinutes = 4,
                        Script =
                        [
                            "Wyjaśnij różnicę: w Scratch układamy bloki, w Pythonie piszemy linie tekstu.",
                            "Pokaż okno z kodem i konsolę obok.",
                            "Uruchom prosty program 'print', żeby pokazać efekt."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Cel: napiszemy program, który rozmawia z użytkownikiem." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Python", Code = "print(\"Cześć! Jestem Twoim komputerem.\")" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Pace, Text = "Nie tłumacz jeszcze cudzysłowów - wrócimy do tego." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 2,
                        Type = LessonStepType.Concept,
                        Title = "Zmienna trzyma wartość",
                        DurationMinutes = 8,
                        Script =
                        [
                            "Wyjaśnij zmienną jako pudełko z etykietą.",
                            "Pokaż przypisanie 'imię = ...' i użycie w 'print'.",
                            "Niech dzieci podstawią swoje imię."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Zmienna to nazwa, pod która chowamy wartość." },
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Znak '=' to przypisanie, nie równość z matematyki." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Python", Code = "imię = \"Ala\"\nprint(\"Cześć, \" + imię)" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Częsty błąd: pominięty cudzysłów przy tekście." },
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Pokaż, że nazwa zmiennej nie może mieć spacji." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 3,
                        Type = LessonStepType.Guided,
                        Title = "Pytanie do użytkownika: input",
                        DurationMinutes = 10,
                        Script =
                        [
                            "Pokaż 'input' jako sposób, by program zapytał i zapamiętał odpowiedź.",
                            "Razem napiszcie program pytający o imię i witający użytkownika.",
                            "Uruchomcie i przetestujcie różne odpowiedzi."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Zadanie: program pyta o imię i wita po imieniu." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Python", Code = "imię = input(\"Jak masz na imię? \")\nprint(\"Miło Cię poznać, \" + imię + \"!\")" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Dzieci zapominają przypisać wynik input do zmiennej." },
                            new LessonNote { Kind = LessonNoteKind.Pace, Text = "Szybsi: zapytaj też o ulubiony kolor." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 4,
                        Type = LessonStepType.Summary,
                        Title = "Podsumowanie",
                        DurationMinutes = 3,
                        Script =
                        [
                            "Zapytaj: do czego służy print, a do czego input?",
                            "Przypomnij, że zmienna pamięta wartość.",
                            "Zapowiedz następną lekcję o liczbach."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Dziś: print, input i zmienne tekstowe." }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Zbierz 2-3 przykłady programów od dzieci." }
                        ]
                    }
                ]
            },
            new Lesson
            {
                Id = Guid.Parse("a1d2c3b4-0003-4f10-9a20-000000000004"),
                Title = "Python: pętła for i lista zadań",
                Subject = "Python",
                Level = "Poziom 2 - 11-13 lat",
                Order = 4,
                Status = LessonStatus.Draft,
                Description = "Szkic lekcji o pętli for i przeglądaniu listy elementów.",
                Tags = ["Pętle", "Listy"],
                Steps =
                [
                    new LessonStep
                    {
                        Order = 1,
                        Type = LessonStepType.Review,
                        Title = "Powtórka: zmienne",
                        DurationMinutes = 4,
                        Script =
                        [
                            "Przypomnij, czym jest zmienna i jak działa print.",
                            "Zadaj krótkie pytanie kontrolne."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Przypomnienie: zmienna pamięta wartość, print ją wyświetli." }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Pace, Text = "Krótko - to tylko rozgrzewka." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 2,
                        Type = LessonStepType.Concept,
                        Title = "Pętla for i lista",
                        DurationMinutes = 10,
                        Script =
                        [
                            "Wyjaśnij listę jako uporządkowany zbiór elementów.",
                            "Pokaż pętlę for przeglądającą listę element po elemencie."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Pętla for wykonuje te same polecenia dla każdego elementu listy." }
                        ],
                        Resources =
                        [
                            new LessonResource { Kind = LessonResourceKind.Code, Language = "Python", Code = "zadania = [\"matematyka\", \"polski\", \"WF\"]\nfor zadanie in zadania:\n    print(\"Do zrobienia:\", zadanie)" }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Error, Text = "Częsty błąd: brak wcięcia pod for." }
                        ]
                    },
                    new LessonStep
                    {
                        Order = 3,
                        Type = LessonStepType.Challenge,
                        Title = "Wyzwanie: własna lista",
                        DurationMinutes = 8,
                        Script =
                        [
                            "Niech każdy stworzy listę 3 ulubionych rzeczy i wypisze je pętlą.",
                            "Pomagaj przy wcięciach i cudzysłówach."
                        ],
                        StudentItems =
                        [
                            new StudentItem { Kind = StudentItemKind.Text, Text = "Zadanie: lista 3 elementów + pętła wypisująca je." }
                        ],
                        Notes =
                        [
                            new LessonNote { Kind = LessonNoteKind.Hint, Text = "Do uzupełnienia: dodać przykład z licznikiem." }
                        ]
                    }
                ]
            }
        ];
    }
}
