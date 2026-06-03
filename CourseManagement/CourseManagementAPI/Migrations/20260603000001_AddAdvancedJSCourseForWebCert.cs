using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedJSCourseForWebCert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ADVJS101 – Advanced JavaScript (Course 3, Category 1 = Programming)
            migrationBuilder.InsertData(
                table: "Course",
                columns: new[] { "courseID", "capacity", "categoryID", "courseCode", "courseName", "description", "durationHours", "enrollmentFee" },
                values: new object[] { 3, 20, 1, "ADVJS101", "Advanced JavaScript", "ES6+, async/await, and modern JavaScript patterns", 10, 110m });

            // Add Session 3 for ADVJS101 (Instructor 1, Classroom 1)
            migrationBuilder.InsertData(
                table: "CourseSession",
                columns: new[] { "sessionID", "capacity", "classroomID", "courseID", "createdAt", "endDateTime", "instructorID", "startDateTime" },
                values: new object[] { 3, 20, 1, 3,
                    new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    1,
                    new DateTime(2026, 7, 1, 9, 0, 0, 0, DateTimeKind.Unspecified) });

            // Link ADVJS101 to Cert 1 (Web Development Certificate) as required.
            // Cert 1 now requires WEB101 + ADVJS101.
            // Trainee 1 has passed WEB101 → 1/2 = 50% (matches existing seeded progress record).
            migrationBuilder.InsertData(
                table: "CertificationCourse",
                columns: new[] { "courseID", "certificationID", "isRequired" },
                values: new object[] { 3, 1, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CertificationCourse",
                keyColumns: new[] { "courseID", "certificationID" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Course",
                keyColumn: "courseID",
                keyValue: 3);
        }
    }
}
