using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Link do spotkania i nagranie na poziomie pojedynczego terminu.
    ///
    /// `MeetingUrl` nadpisuje link grupy - potrzebne przy zastępstwie (prowadzący ma własny pokój),
    /// przy odrabianiu i przy jednorazowej zmianie platformy. Puste = obowiązuje link grupy.
    /// `RecordingUrl` jest pokazywany rodzicowi po zakończeniu zajęć.
    ///
    /// Migracja napisana ręcznie; atrybuty i model docelowy w pliku *.Designer.cs.
    /// </summary>
    public partial class AddSessionMeetingAndRecordingUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeetingUrl",
                table: "ScheduledSessions",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordingUrl",
                table: "ScheduledSessions",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingUrl",
                table: "ScheduledSessions");

            migrationBuilder.DropColumn(
                name: "RecordingUrl",
                table: "ScheduledSessions");
        }
    }
}
