using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleScopedRunSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LessonRunSessions_LessonId_UserId",
                table: "LessonRunSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduledSessionId",
                table: "LessonRunSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LessonRunSessions_LessonId_UserId_ScheduledSessionId",
                table: "LessonRunSessions",
                columns: new[] { "LessonId", "UserId", "ScheduledSessionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LessonRunSessions_LessonId_UserId_ScheduledSessionId",
                table: "LessonRunSessions");

            migrationBuilder.DropColumn(
                name: "ScheduledSessionId",
                table: "LessonRunSessions");

            migrationBuilder.CreateIndex(
                name: "IX_LessonRunSessions_LessonId_UserId",
                table: "LessonRunSessions",
                columns: new[] { "LessonId", "UserId" },
                unique: true);
        }
    }
}
