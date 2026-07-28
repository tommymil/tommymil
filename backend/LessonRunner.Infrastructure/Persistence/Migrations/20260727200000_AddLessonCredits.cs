using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Kredyty zajęciowe: „należą się jedne zajęcia".
    ///
    /// Bez nich odrabianie było jedyną formą rekompensaty za odwołane zajęcia i nie zostawiało
    /// żadnego śladu w rozliczeniach. Kredyt zapisuje decyzję: komu, za co, dlaczego, kto ją
    /// podjął i czy została już wykorzystana.
    ///
    /// Tabela celowo bez kluczy obcych do `Participants`, `Groups` i `ScheduledSessions` —
    /// historia rozliczeń ma przetrwać usunięcie grupy albo terminu.
    ///
    /// Migracja napisana ręcznie; atrybuty i model docelowy w pliku *.Designer.cs.
    /// </summary>
    public partial class AddLessonCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonCredits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GroupId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceSessionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    AmountCents = table.Column<long>(type: "INTEGER", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    IssuedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IssuedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Usage = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    UsedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UsedForSessionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UsedForInvoiceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UsageNote = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonCredits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonCredits_ParticipantId",
                table: "LessonCredits",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonCredits_SourceSessionId",
                table: "LessonCredits",
                column: "SourceSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonCredits_ParticipantId_Status",
                table: "LessonCredits",
                columns: new[] { "ParticipantId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "LessonCredits");
        }
    }
}
