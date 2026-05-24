using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseDurationHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "durationHours",
                table: "Course",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "Course",
                keyColumn: "courseID",
                keyValue: 1,
                column: "durationHours",
                value: 12);

            migrationBuilder.UpdateData(
                table: "Course",
                keyColumn: "courseID",
                keyValue: 2,
                column: "durationHours",
                value: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "durationHours",
                table: "Course");
        }
    }
}
