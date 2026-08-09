using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Postępy dzieci i ich projekty — rozdział 11 dokumentu koncepcyjnego.
    ///
    /// Dotąd po pół roku zajęć jedynym śladem tego, czego dziecko się nauczyło, była lista
    /// obecności. `ProgressEntries` niosą poziom samodzielności, ukończenie materiału i notatkę
    /// dla rodzica; `Projects` z `ProjectSubmissions` trzymają kolejne wersje projektu.
    ///
    /// `ProjectSubmissions` są dopisywane, nigdy nadpisywane — reklamacja „projekt dziecka
    /// zniknął" prawie zawsze znaczy „został zastąpiony gorszą wersją".
    ///
    /// Klucz obcy jest jeden: submission → project (kaskada), bo wersja bez projektu nie ma
    /// sensu. Do `Participants`, `Groups` i `ScheduledSessions` kluczy nie ma — dorobek dziecka
    /// ma przetrwać usunięcie grupy albo terminu.
    ///
    /// Migracja napisana ręcznie; model docelowy w pliku *.Designer.cs.
    /// </summary>
    public partial class AddProgressAndProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgressEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    GroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LessonId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Autonomy = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    LessonCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoteForParent = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    NextStep = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    GroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LessonId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FileUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 260, nullable: true),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: true),
                    DownloadToken = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SubmittedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    InstructorComment = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSubmissions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgressEntries_ParticipantId",
                table: "ProgressEntries",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressEntries_SessionId_ParticipantId",
                table: "ProgressEntries",
                columns: new[] { "SessionId", "ParticipantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ParticipantId",
                table: "Projects",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSubmissions_DownloadToken",
                table: "ProjectSubmissions",
                column: "DownloadToken");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSubmissions_ProjectId_Version",
                table: "ProjectSubmissions",
                columns: new[] { "ProjectId", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ProgressEntries");
            migrationBuilder.DropTable(name: "ProjectSubmissions");
            migrationBuilder.DropTable(name: "Projects");
        }
    }
}
