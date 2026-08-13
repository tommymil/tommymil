using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Rozszerzony status obecności zamiast samego „był / nie był”.
    ///
    /// Na zajęciach online dziecko potrafi dołączyć 20 minut później, wypaść przez zerwane łącze
    /// albo siedzieć na spotkaniu bez pracy. Kolumna `Present` zostaje jako zdenormalizowany skrót
    /// dla frekwencji i KPI, a `Status` niesie pełną informację.
    ///
    /// Istniejące wiersze dostają status wyprowadzony z `Present`, więc historia obecności
    /// nie zamienia się w zera. (Repozytorium ma dodatkowo ten sam fallback przy odczycie -
    /// na wypadek bazy, w której backfill z jakiegoś powodu nie zadziałał.)
    ///
    /// Migracja napisana ręcznie; atrybuty i model docelowy w pliku *.Designer.cs.
    /// </summary>
    public partial class AddAttendanceStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AttendanceRecords",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "AttendanceRecords",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JoinedAt",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeftAt",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE AttendanceRecords SET Status = CASE WHEN Present = 1 THEN 'Present' ELSE 'UnexcusedAbsence' END WHERE Status = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LeftAt", table: "AttendanceRecords");
            migrationBuilder.DropColumn(name: "JoinedAt", table: "AttendanceRecords");
            migrationBuilder.DropColumn(name: "Note", table: "AttendanceRecords");
            migrationBuilder.DropColumn(name: "Status", table: "AttendanceRecords");
        }
    }
}
