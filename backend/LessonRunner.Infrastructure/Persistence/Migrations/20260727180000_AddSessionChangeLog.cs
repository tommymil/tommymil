using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Historia zmian terminu: poprzedni termin, nowy, kto, dlaczego, kiedy i czy poszła
    /// informacja do opiekunów.
    ///
    /// Bez tego przesunięcie zajęć nadpisuje `ScheduledAt` bez śladu, a przy zdaniu
    /// „nie dostaliśmy informacji o zmianie" nie ma czym odpowiedzieć.
    ///
    /// Tabela celowo nie ma klucza obcego do `ScheduledSessions` ani `Groups` — historia ma
    /// przetrwać usunięcie terminu albo grupy, bo to właśnie wtedy bywa najbardziej potrzebna.
    ///
    /// Migracja napisana ręcznie; atrybuty i model docelowy w pliku *.Designer.cs.
    /// </summary>
    public partial class AddSessionChangeLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionChangeLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduledSessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GroupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChangeType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    PreviousScheduledAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    NewScheduledAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Details = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ChangedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    GuardiansNotified = table.Column<bool>(type: "INTEGER", nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionChangeLogs_ScheduledSessionId",
                table: "SessionChangeLogs",
                column: "ScheduledSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionChangeLogs_GroupId_ChangedAt",
                table: "SessionChangeLogs",
                columns: new[] { "GroupId", "ChangedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SessionChangeLogs");
        }
    }
}
