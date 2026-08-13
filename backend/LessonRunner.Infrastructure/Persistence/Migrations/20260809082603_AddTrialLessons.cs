using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTrialLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrialLessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChildFirstName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    ChildLastName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    ChildBirthDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    GuardianName = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    GuardianEmail = table.Column<string>(type: "TEXT", maxLength: 254, nullable: true),
                    GuardianPhone = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    RequestNote = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InstructorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    MeetingUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LessonId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Reading = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Computer = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Programming = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Recommendation = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    RecommendedLevel = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    DiagnosisNote = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    DiagnosedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    DiagnosedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ParticipantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeclineReason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ClosedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrialLessons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrialLessons_InstructorId",
                table: "TrialLessons",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_TrialLessons_Status",
                table: "TrialLessons",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrialLessons");
        }
    }
}
