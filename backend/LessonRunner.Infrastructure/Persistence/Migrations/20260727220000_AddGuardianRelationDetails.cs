using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Relacja opiekun–dziecko przestaje być samym powiązaniem i zaczyna nieść dane:
    /// kim opiekun jest dla dziecka, czy jest kontaktem pierwszego wyboru i czy ma dostawać
    /// powiadomienia.
    ///
    /// Powód: dane opiekuna żyły w dwóch miejscach — w kolumnach przy dziecku
    /// (`Participant.GuardianName/Email/Phone`) i w koncie `User(Parent)` powiązanym przez
    /// `ParentParticipantLinks`. Powiadomienia szły na kolumny, portal na konto. Przy dwojgu
    /// opiekunów (a dokument koncepcyjny tego wymaga) te dwa światy musiałyby się rozjechać.
    ///
    /// Migracja nie kasuje kolumn przy dziecku — zostają jako zapas dla rodzin bez konta.
    /// Rozstrzyganie odbiorców jest jedno: są powiązania → wygrywają one; nie ma → kolumny.
    ///
    /// Migracja napisana ręcznie; atrybuty i model docelowy w pliku *.Designer.cs.
    /// </summary>
    public partial class AddGuardianRelationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Relation",
                table: "ParentParticipantLinks",
                type: "TEXT",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimaryContact",
                table: "ParentParticipantLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReceivesNotifications",
                table: "ParentParticipantLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            // Istniejące powiązania były jedyne dla swojego dziecka, więc każde z nich
            // jest kontaktem pierwszego wyboru.
            migrationBuilder.Sql("UPDATE ParentParticipantLinks SET IsPrimaryContact = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ReceivesNotifications", table: "ParentParticipantLinks");
            migrationBuilder.DropColumn(name: "IsPrimaryContact", table: "ParentParticipantLinks");
            migrationBuilder.DropColumn(name: "Relation", table: "ParentParticipantLinks");
        }
    }
}
