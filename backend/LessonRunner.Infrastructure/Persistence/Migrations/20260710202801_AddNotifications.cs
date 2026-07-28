using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Channel = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Recipient = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    DedupeKey = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                    Error = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RemindersEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AbsenceEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReminderLeadHours = table.Column<int>(type: "INTEGER", nullable: false),
                    FromName = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    FromEmail = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    ReminderSubject = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    ReminderBody = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    AbsenceSubject = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    AbsenceBody = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_CreatedAt",
                table: "NotificationLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_DedupeKey",
                table: "NotificationLogs",
                column: "DedupeKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationLogs");

            migrationBuilder.DropTable(
                name: "NotificationSettings");
        }
    }
}
