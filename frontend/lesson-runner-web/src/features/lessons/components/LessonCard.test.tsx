import { describe, expect, it, vi } from "vitest";
import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import { LessonCard } from "./LessonCard";
import type { LessonSummary } from "../../../types/lesson";

function makeLesson(overrides: Partial<LessonSummary> = {}): LessonSummary {
  return {
    id: "abc",
    title: "Pierwsza gra",
    subject: "Scratch",
    level: "Poziom 1",
    description: "Opis lekcji",
    order: 1,
    status: "ready",
    statusLabel: "Gotowa",
    stepCount: 7,
    durationMinutes: 53,
    ...overrides,
  };
}

function renderCard(lesson: LessonSummary, props: Partial<Parameters<typeof LessonCard>[0]> = {}) {
  return render(
    <MemoryRouter>
      <LessonCard lesson={lesson} {...props} />
    </MemoryRouter>,
  );
}

describe("LessonCard", () => {
  it("shows lesson metadata", () => {
    renderCard(makeLesson());

    expect(screen.getByRole("heading", { name: "Pierwsza gra" })).toBeInTheDocument();
    expect(screen.getByText("Scratch")).toBeInTheDocument();
    expect(screen.getByText("7 kroków")).toBeInTheDocument();
    expect(screen.getByText("53 min")).toBeInTheDocument();
  });

  it("shows the order badge when the lesson has an assigned order", () => {
    renderCard(makeLesson({ order: 3 }));

    expect(screen.getByText("#3")).toBeInTheDocument();
  });

  it("hides the order badge when the lesson has no assigned order", () => {
    renderCard(makeLesson({ order: 0 }));

    expect(screen.queryByText(/^#\d+$/)).not.toBeInTheDocument();
  });

  it("links to the presenter when the lesson is ready", () => {
    renderCard(makeLesson({ status: "ready", statusLabel: "Gotowa" }));

    const runLink = screen.getByRole("link", { name: "Prowadź lekcję" });
    expect(runLink).toHaveAttribute("href", "/presenter/abc");
  });

  it("disables running for a draft lesson", () => {
    renderCard(makeLesson({ status: "draft", statusLabel: "Szkic" }));

    expect(screen.queryByRole("link", { name: "Prowadź lekcję" })).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Prowadź lekcję" })).toBeDisabled();
  });

  it("hides management actions by default", () => {
    renderCard(makeLesson());

    expect(screen.queryByRole("link", { name: "Edytuj" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Usuń" })).not.toBeInTheDocument();
  });

  it("shows edit and delete actions for administrators", async () => {
    const onDelete = vi.fn();
    const lesson = makeLesson();
    renderCard(lesson, { canManage: true, onDelete });

    const editLink = screen.getByRole("link", { name: "Edytuj" });
    expect(editLink).toHaveAttribute("href", "/admin/lessons/abc/edit");
    expect(within(editLink).getByRole("button", { name: "Edytuj" })).toBeInTheDocument();

    await userEvent.click(screen.getByRole("button", { name: "Usuń" }));

    expect(onDelete).toHaveBeenCalledWith(lesson);
  });
});
