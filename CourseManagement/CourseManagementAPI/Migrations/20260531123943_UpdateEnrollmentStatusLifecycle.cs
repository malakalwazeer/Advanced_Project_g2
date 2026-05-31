using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEnrollmentStatusLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 1,
                column: "statusName",
                value: "Enrolled");

            migrationBuilder.UpdateData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 2,
                column: "statusName",
                value: "Confirmed");

            migrationBuilder.UpdateData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 3,
                column: "statusName",
                value: "Attending");

            migrationBuilder.UpdateData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 4,
                column: "statusName",
                value: "Completed");

            migrationBuilder.InsertData(
                table: "EnrollmentStatus",
                columns: new[] { "enrollmentStatusID", "statusName" },
                values: new object[] { 5, "Dropped" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 1,
                column: "statusName",
                value: "Pending");

            migrationBuilder.UpdateData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 2,
                column: "statusName",
                value: "Enrolled");

            migrationBuilder.UpdateData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 3,
                column: "statusName",
                value: "Completed");

            migrationBuilder.UpdateData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 4,
                column: "statusName",
                value: "Cancelled");
        }
    }
}
