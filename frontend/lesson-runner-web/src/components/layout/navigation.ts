import type { LucideIcon } from "lucide-react";
import {
  BookCopy,
  BookOpenText,
  Bell,
  CalendarDays,
  Contact,
  CreditCard,
  FilePlus2,
  GraduationCap,
  LayoutDashboard,
  Library,
  FolderKanban,
  ShieldAlert,
  ShieldCheck,
  Upload,
  UserCog,
  UserRound,
  UsersRound,
} from "lucide-react";

export type SidebarItem = {
  to: string;
  label: string;
  description: string;
  icon: LucideIcon;
  adminOnly?: boolean;
  staffOnly?: boolean;
  end?: boolean;
};

export type SidebarSection = {
  title: string;
  items: SidebarItem[];
};

/**
 * Nawigacja panelu roboczego — wyłącznie dla administratora i instruktora.
 *
 * Pozycji rodzica tu nie ma: rola `Parent` korzysta z osobnego układu
 * (`ParentShell`) i nie widzi bocznego menu w ogóle. Wcześniej rodzic dostawał
 * 292-pikselowy pasek z dwiema pozycjami i sekcją zatytułowaną „Rodzic”,
 * czyli informacją o tym, kim jest.
 *
 * „Kalendarz” i „Grafik” zostały połączone w jedną pozycję: były to dwa wejścia
 * do tego samego zadania, w dodatku z tą samą ikoną, więc ikona przestawała
 * cokolwiek odróżniać. Podgląd konspektu poza terminem („Szybki start”) wrócił
 * do biblioteki lekcji, gdzie jest jego naturalne miejsce.
 */
export const sidebarSections: SidebarSection[] = [
  {
    title: "Prowadzenie zajęć",
    items: [
      {
        to: "/instructor/schedule",
        label: "Grafik",
        description: "Dzień, tydzień i przebieg kursów",
        icon: CalendarDays,
        staffOnly: true,
      },
      {
        to: "/instructor/trials",
        // Lekcja próbna to badanie, nie zajęcia: jedno dziecko, bez kursu i listy obecności.
        // W grafiku grup byłaby terminem, który udaje coś, czym nie jest.
        label: "Lekcje próbne",
        description: "Spotkania 1:1 z kandydatami i diagnoza",
        icon: GraduationCap,
        staffOnly: true,
      },
      {
        to: "/calendar",
        // Nie „szkoły”: to zajęcia online prowadzone w całej Polsce, a nie placówka.
        // Pozycja odróżnia się od „Grafiku” zasięgiem (wszystkie grupy), nie miejscem.
        label: "Kalendarz ogólny",
        description: "Terminy wszystkich grup i dni wolne",
        icon: CalendarDays,
        staffOnly: true,
      },
      {
        to: "/instructor/lessons",
        label: "Konspekty",
        description: "Podgląd scenariuszy poza terminem",
        icon: BookOpenText,
        staffOnly: true,
      },
      {
        to: "/materials",
        label: "Materiały",
        description: "Pliki i linki dla zespołu",
        icon: FolderKanban,
        staffOnly: true,
      },
    ],
  },
  {
    title: "Lekcje",
    items: [
      {
        to: "/admin/lessons",
        label: "Biblioteka",
        description: "Lista gotowych konspektów",
        icon: Library,
        adminOnly: true,
        end: true,
      },
      {
        to: "/admin/courses",
        label: "Kursy",
        description: "Programy i sekwencje lekcji",
        icon: BookCopy,
        adminOnly: true,
      },
      {
        to: "/admin/lessons/new",
        label: "Nowa lekcja",
        description: "Edytor konspektu krok po kroku",
        icon: FilePlus2,
        adminOnly: true,
      },
      {
        to: "/admin/lessons/import",
        label: "Import konspektu",
        description: "Wklej materiał i utwórz lekcję",
        icon: Upload,
        adminOnly: true,
      },
    ],
  },
  {
    title: "Administracja",
    items: [
      {
        to: "/admin/dashboard",
        label: "Pulpit",
        description: "Co wymaga uwagi i najbliższe terminy",
        icon: LayoutDashboard,
        adminOnly: true,
      },
      {
        to: "/admin/groups",
        label: "Grupy",
        description: "Harmonogram i obecność",
        icon: UsersRound,
        adminOnly: true,
      },
      {
        to: "/admin/participants",
        label: "Uczestnicy",
        description: "Centralna baza i przypisywanie do grup",
        icon: Contact,
        adminOnly: true,
      },
      {
        to: "/admin/trials",
        label: "Lekcje próbne",
        description: "Zgłoszenia, diagnozy i zapis do systemu",
        icon: GraduationCap,
        adminOnly: true,
      },
      {
        to: "/admin/billing",
        label: "Płatności",
        description: "Cenniki, faktury, wpłaty i kredyty",
        icon: CreditCard,
        adminOnly: true,
      },
      {
        to: "/admin/users",
        label: "Konta",
        description: "Administratorzy i instruktorzy",
        icon: UserCog,
        adminOnly: true,
      },
      {
        to: "/admin/notifications",
        label: "Powiadomienia",
        description: "E-maile i log wysyłek",
        icon: Bell,
        adminOnly: true,
      },
      {
        to: "/admin/safety",
        label: "Bezpieczeństwo",
        description: "Incydenty i zgłoszenia techniczne",
        icon: ShieldAlert,
        adminOnly: true,
      },
      {
        to: "/admin/operations",
        label: "Operacje",
        description: "Audyt, health i backup",
        icon: ShieldCheck,
        adminOnly: true,
      },
    ],
  },
  {
    title: "Konto",
    items: [
      {
        to: "/profile",
        label: "Mój profil",
        description: "Dane konta i zmiana hasła",
        icon: UserRound,
      },
    ],
  },
];

export type Crumb = { label: string; to?: string };

const staticCrumbs: Record<string, Crumb[]> = {
  "/admin/dashboard": [{ label: "Pulpit" }],
  "/admin/groups": [{ label: "Grupy" }],
  "/admin/groups/new": [{ label: "Grupy", to: "/admin/groups" }, { label: "Nowa grupa" }],
  "/admin/participants": [{ label: "Uczestnicy" }],
  "/admin/trials": [{ label: "Lekcje próbne" }],
  "/admin/billing": [{ label: "Płatności" }],
  "/admin/users": [{ label: "Konta" }],
  "/admin/notifications": [{ label: "Powiadomienia" }],
  "/admin/safety": [{ label: "Bezpieczeństwo" }],
  "/admin/operations": [{ label: "Operacje" }],
  "/admin/lessons": [{ label: "Biblioteka konspektów" }],
  "/admin/lessons/new": [{ label: "Biblioteka konspektów", to: "/admin/lessons" }, { label: "Nowa lekcja" }],
  "/admin/lessons/import": [{ label: "Biblioteka konspektów", to: "/admin/lessons" }, { label: "Import" }],
  "/admin/courses": [{ label: "Kursy" }],
  "/calendar": [{ label: "Kalendarz ogólny" }],
  "/instructor/schedule": [{ label: "Grafik" }],
  "/instructor/trials": [{ label: "Lekcje próbne" }],
  "/instructor/lessons": [{ label: "Konspekty" }],
  "/materials": [{ label: "Materiały" }],
  "/profile": [{ label: "Mój profil" }],
};

/**
 * Okruszki wyliczane ze ścieżki.
 *
 * Ekran szczegółów grupy i kokpit terminu leżą trzy poziomy w głąb, a jedynym
 * powrotem była strzałka wewnątrz treści. Nazwy zasobów (grupy, lekcji) nie są
 * tutaj znane — strony dopisują je same przez `useBreadcrumbTail`.
 */
export function crumbsForPath(pathname: string): Crumb[] {
  const exact = staticCrumbs[pathname];

  if (exact) {
    return exact;
  }

  if (pathname.startsWith("/admin/groups/")) {
    return [{ label: "Grupy", to: "/admin/groups" }, { label: "Szczegóły grupy" }];
  }

  if (pathname.startsWith("/admin/lessons/")) {
    return [{ label: "Biblioteka konspektów", to: "/admin/lessons" }, { label: "Edytor lekcji" }];
  }

  if (pathname.startsWith("/instructor/sessions/")) {
    return [{ label: "Grafik", to: "/instructor/schedule" }, { label: "Kokpit zajęć" }];
  }

  if (pathname.startsWith("/presenter/")) {
    return [{ label: "Konspekty", to: "/instructor/lessons" }, { label: "Prowadzenie" }];
  }

  return [];
}
