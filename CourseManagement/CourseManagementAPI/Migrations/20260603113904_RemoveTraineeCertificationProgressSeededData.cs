using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTraineeCertificationProgressSeededData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TraineeCertificationProgress",
                keyColumns: new[] { "certificationID", "traineeID" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TraineeCertificationProgress",
                keyColumns: new[] { "certificationID", "traineeID" },
                keyValues: new object[] { 3, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TraineeCertificationProgress",
                columns: new[] { "certificationID", "traineeID", "achievedDate", "progressPercentage" },
                values: new object[,]
                {
                    { 1, 1, null, 50m },
                    { 3, 3, null, 0m }
                });
        }
    }
}
