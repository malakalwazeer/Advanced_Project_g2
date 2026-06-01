using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Instructor",
                keyColumn: "instructorID",
                keyValue: 1,
                column: "password",
                value: "Temp123!");

            migrationBuilder.UpdateData(
                table: "Instructor",
                keyColumn: "instructorID",
                keyValue: 2,
                column: "password",
                value: "Temp123!");

            migrationBuilder.UpdateData(
                table: "Trainee",
                keyColumn: "traineeID",
                keyValue: 1,
                column: "password",
                value: "Temp123!");

            migrationBuilder.UpdateData(
                table: "Trainee",
                keyColumn: "traineeID",
                keyValue: 2,
                column: "password",
                value: "Temp123!");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Instructor",
                keyColumn: "instructorID",
                keyValue: 1,
                column: "password",
                value: "Temp123");

            migrationBuilder.UpdateData(
                table: "Instructor",
                keyColumn: "instructorID",
                keyValue: 2,
                column: "password",
                value: "Temp123");

            migrationBuilder.UpdateData(
                table: "Trainee",
                keyColumn: "traineeID",
                keyValue: 1,
                column: "password",
                value: "Temp123");

            migrationBuilder.UpdateData(
                table: "Trainee",
                keyColumn: "traineeID",
                keyValue: 2,
                column: "password",
                value: "Temp123");
        }
    }
}
