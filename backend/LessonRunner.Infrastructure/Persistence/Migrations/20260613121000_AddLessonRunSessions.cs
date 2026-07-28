using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonRunSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonRunSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LessonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StepIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    ElapsedTotalSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    ElapsedStepSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Running = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonRunSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonRunSessions_LessonId_UserId",
                table: "LessonRunSessions",
                columns: new[] { "LessonId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonRunSessions");
        }
    }
}
