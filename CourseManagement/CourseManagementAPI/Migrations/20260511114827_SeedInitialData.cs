using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Certification",
                columns: new[] { "certificationID", "description", "name" },
                values: new object[,]
                {
                    { 1, "Certificate for web development skills", "Web Development Certificate" },
                    { 2, "Certificate for database basics", "Database Fundamentals Certificate" }
                });

            migrationBuilder.InsertData(
                table: "Classroom",
                columns: new[] { "classroomID", "capacity", "isActive", "location" },
                values: new object[,]
                {
                    { 1, 25, true, "Room A101" },
                    { 2, 30, true, "Room B202" }
                });

            migrationBuilder.InsertData(
                table: "CourseCategory",
                columns: new[] { "categoryID", "categoryName", "description" },
                values: new object[,]
                {
                    { 1, "Programming", "Software and coding courses" },
                    { 2, "Networking", "Network and infrastructure courses" },
                    { 3, "Database", "Database management courses" }
                });

            migrationBuilder.InsertData(
                table: "EnrollmentStatus",
                columns: new[] { "enrollmentStatusID", "statusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Enrolled" },
                    { 3, "Completed" },
                    { 4, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "equipmentID", "description", "equipmentName" },
                values: new object[,]
                {
                    { 1, "Classroom projector", "Projector" },
                    { 2, "Training laptop", "Laptop" },
                    { 3, "Classroom whiteboard", "Whiteboard" }
                });

            migrationBuilder.InsertData(
                table: "Instructor",
                columns: new[] { "instructorID", "email", "fullName", "hireDate", "password", "phone", "qualifications" },
                values: new object[,]
                {
                    { 1, "ahmed.ali@example.com", "Ahmed Ali", new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Temp123", "+97333112244", "MSc Computer Science" },
                    { 2, "sara.hassan@example.com", "Sara Hassan", new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Temp123", "+97333225566", "BSc Information Technology" }
                });

            migrationBuilder.InsertData(
                table: "PaymentStatus",
                columns: new[] { "paymentStatusID", "statusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Paid" },
                    { 3, "Partially Paid" },
                    { 4, "Unpaid" }
                });

            migrationBuilder.InsertData(
                table: "TraineeStatus",
                columns: new[] { "traineeStatusID", "statusName" },
                values: new object[,]
                {
                    { 1, "Active" },
                    { 2, "Inactive" }
                });

            migrationBuilder.InsertData(
                table: "ClassroomEquipment",
                columns: new[] { "classroomID", "equipmentID", "quantity" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 1, 3, 1 },
                    { 2, 1, 1 },
                    { 2, 2, 10 }
                });

            migrationBuilder.InsertData(
                table: "Course",
                columns: new[] { "courseID", "capacity", "categoryID", "courseCode", "courseName", "description", "enrollmentFee" },
                values: new object[,]
                {
                    { 1, 20, 1, "WEB101", "Introduction to Web Development", "HTML, CSS, and JavaScript basics", 120m },
                    { 2, 25, 3, "DB101", "Database Fundamentals", "Introduction to relational databases", 100m }
                });

            migrationBuilder.InsertData(
                table: "InstructorAvailability",
                columns: new[] { "availabilityID", "availableDate", "endTime", "instructorID", "isAvailable", "startTime" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 6, 1), new TimeOnly(14, 0, 0), 1, true, new TimeOnly(8, 0, 0) },
                    { 2, new DateOnly(2026, 6, 2), new TimeOnly(14, 0, 0), 2, true, new TimeOnly(8, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "InstructorExpertise",
                columns: new[] { "categoryID", "instructorID" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "Trainee",
                columns: new[] { "traineeID", "email", "fullName", "organizationName", "password", "phone", "registrationDate", "traineeStatusID" },
                values: new object[,]
                {
                    { 1, "noor@example.com", "Noor Mohammed", "ABC Company", "Temp123", "+97339998877", new DateOnly(2026, 5, 1), 1 },
                    { 2, "ali@example.com", "Ali Yusuf", null, "Temp123", "+97338887766", new DateOnly(2026, 5, 2), 1 }
                });

            migrationBuilder.InsertData(
                table: "CertificationCourse",
                columns: new[] { "certificationID", "courseID", "isRequired" },
                values: new object[,]
                {
                    { 1, 1, true },
                    { 2, 2, true }
                });

            migrationBuilder.InsertData(
                table: "CoursePrerequisite",
                columns: new[] { "courseID", "coursePrerequisiteID" },
                values: new object[] { 2, 1 });

            migrationBuilder.InsertData(
                table: "CourseReqEquipment",
                columns: new[] { "courseID", "equipmentID", "quantity" },
                values: new object[,]
                {
                    { 1, 2, 10 },
                    { 2, 2, 10 }
                });

            migrationBuilder.InsertData(
                table: "CourseSession",
                columns: new[] { "sessionID", "capacity", "classroomID", "courseID", "createdAt", "endDateTime", "instructorID", "startDateTime" },
                values: new object[,]
                {
                    { 1, 20, 1, 1, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 25, 2, 2, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 2, 12, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2026, 6, 2, 9, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "TraineeCertificationProgress",
                columns: new[] { "certificationID", "traineeID", "achievedDate", "progressPercentage" },
                values: new object[] { 1, 1, null, 50m });

            migrationBuilder.InsertData(
                table: "Enrollment",
                columns: new[] { "enrollmentID", "enrollmentDate", "enrollmentStatusID", "sessionID", "traineeID" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 5, 5), 2, 1, 1 },
                    { 2, new DateOnly(2026, 5, 6), 1, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Notification",
                columns: new[] { "notificationID", "message", "paymentID", "sessionID", "title" },
                values: new object[] { 2, "Your course session starts soon.", null, 1, "Session Reminder" });

            migrationBuilder.InsertData(
                table: "Assessment",
                columns: new[] { "assessmentID", "enrollmentID", "instructorID", "result", "score" },
                values: new object[] { 1, 1, 1, 1, 85 });

            migrationBuilder.InsertData(
                table: "Payment",
                columns: new[] { "paymentID", "amountPaid", "balanceRemaining", "enrollmentID", "paymentDate", "paymentStatusID" },
                values: new object[,]
                {
                    { 1, 120m, 0m, 1, new DateOnly(2026, 5, 6), 2 },
                    { 2, 50m, 50m, 2, new DateOnly(2026, 5, 7), 3 }
                });

            migrationBuilder.InsertData(
                table: "Notification",
                columns: new[] { "notificationID", "message", "paymentID", "sessionID", "title" },
                values: new object[] { 1, "Your payment has been received.", 1, null, "Payment Received" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assessment",
                keyColumn: "assessmentID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CertificationCourse",
                keyColumns: new[] { "certificationID", "courseID" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "CertificationCourse",
                keyColumns: new[] { "certificationID", "courseID" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "classroomID", "equipmentID" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "classroomID", "equipmentID" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "classroomID", "equipmentID" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "classroomID", "equipmentID" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "CourseCategory",
                keyColumn: "categoryID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CoursePrerequisite",
                keyColumns: new[] { "courseID", "coursePrerequisiteID" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "CourseReqEquipment",
                keyColumns: new[] { "courseID", "equipmentID" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "CourseReqEquipment",
                keyColumns: new[] { "courseID", "equipmentID" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InstructorExpertise",
                keyColumns: new[] { "categoryID", "instructorID" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "InstructorExpertise",
                keyColumns: new[] { "categoryID", "instructorID" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "Notification",
                keyColumn: "notificationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Notification",
                keyColumn: "notificationID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payment",
                keyColumn: "paymentID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "paymentStatusID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "paymentStatusID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TraineeCertificationProgress",
                keyColumns: new[] { "certificationID", "traineeID" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TraineeStatus",
                keyColumn: "traineeStatusID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Certification",
                keyColumn: "certificationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Certification",
                keyColumn: "certificationID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Enrollment",
                keyColumn: "enrollmentID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "equipmentID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "equipmentID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "equipmentID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Payment",
                keyColumn: "paymentID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "paymentStatusID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Enrollment",
                keyColumn: "enrollmentID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "paymentStatusID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Trainee",
                keyColumn: "traineeID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Classroom",
                keyColumn: "classroomID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Course",
                keyColumn: "courseID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EnrollmentStatus",
                keyColumn: "enrollmentStatusID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Instructor",
                keyColumn: "instructorID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Trainee",
                keyColumn: "traineeID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Classroom",
                keyColumn: "classroomID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Course",
                keyColumn: "courseID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CourseCategory",
                keyColumn: "categoryID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Instructor",
                keyColumn: "instructorID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TraineeStatus",
                keyColumn: "traineeStatusID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CourseCategory",
                keyColumn: "categoryID",
                keyValue: 1);
        }
    }
}
