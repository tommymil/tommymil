using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Strukturalne zakończenie zajęć i znacznik pracy na żywo.
    ///
    /// Do tej pory całe podsumowanie zajęć trafiało do jednego pola `InstructorNote`.
    /// Rozdział 5 dokumentu koncepcyjnego oczekuje jednak trzech różnych informacji:
    /// uwag wewnętrznych, tego czego nie zdążyliśmy (potrzebne na kolejnym terminie
    /// i przy zastępstwie) oraz podsumowania dla rodzica. Z jednego bloku tekstu nie da
    /// się wyciągnąć żadnej z nich automatycznie.
    ///
    /// `LiveStatus` przy obecności to znacznik pracy dziecka w trakcie trwających zajęć
    /// (rozdział 4): pracuje / potrzebuje pomocy / skończyło / problem techniczny.
    /// Trzymamy go przy `AttendanceRecords`, bo dotyczy dokładnie tej samej pary
    /// (dziecko, termin) i ma ten sam cykl życia; osobna tabela dokładałaby złączenie
    /// bez żadnej nowej informacji.
    ///
    /// Istniejące wiersze dostają `LiveStatus = 'Working'`. Pusty łańcuch zostaje jako
    /// dopuszczalny stan, bo repozytorium ma przy odczycie ten sam fallback - tak samo
    /// jak przy migracji `AddAttendanceStatus`.
    ///
    /// Migracja napisana ręcznie; model docelowy w pliku *.Designer.cs.
    /// </summary>
    public partial class AddSessionDebriefAndLiveStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnfinishedNote",
                table: "ScheduledSessions",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentSummary",
                table: "ScheduledSessions",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LiveStatus",
                table: "AttendanceRecords",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE AttendanceRecords SET LiveStatus = 'Working' WHERE LiveStatus = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LiveStatus", table: "AttendanceRecords");
            migrationBuilder.DropColumn(name: "ParentSummary", table: "ScheduledSessions");
            migrationBuilder.DropColumn(name: "UnfinishedNote", table: "ScheduledSessions");
        }
    }
}
