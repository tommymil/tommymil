using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationalEnrollmentAndMakeup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubstituteInstructorId",
                table: "ScheduledSessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Groups",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "GroupEnrollments",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "Enrolled");

            migrationBuilder.AddColumn<bool>(
                name: "MakeupRequired",
                table: "AttendanceRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "MakeupSessionId",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledSessions_SubstituteInstructorId",
                table: "ScheduledSessions",
                column: "SubstituteInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_MakeupSessionId",
                table: "AttendanceRecords",
                column: "MakeupSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduledSessions_SubstituteInstructorId",
                table: "ScheduledSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_MakeupSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "SubstituteInstructorId",
                table: "ScheduledSessions");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GroupEnrollments");

            migrationBuilder.DropColumn(
                name: "MakeupRequired",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "MakeupSessionId",
                table: "AttendanceRecords");
        }
    }
}
