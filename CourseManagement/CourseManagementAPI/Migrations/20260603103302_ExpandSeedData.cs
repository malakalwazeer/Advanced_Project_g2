using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Certification",
                columns: new[] { "certificationID", "description", "name" },
                values: new object[,]
                {
                    { 3, "Validates advanced threat management profiles", "Enterprise Security Administrator" },
                    { 4, "Validates cross-platform enterprise cloud designs", "Cloud Infrastructure Architect" }
                });

            migrationBuilder.InsertData(
                table: "Classroom",
                columns: new[] { "classroomID", "capacity", "isActive", "location" },
                values: new object[,]
                {
                    { 3, 15, true, "Lab C305" },
                    { 4, 12, true, "Lab D401 (Hardware)" },
                    { 5, 150, true, "Auditorium Max" }
                });

            migrationBuilder.InsertData(
                table: "Course",
                columns: new[] { "courseID", "capacity", "categoryID", "courseCode", "courseName", "description", "durationHours", "enrollmentFee" },
                values: new object[,]
                {
                    { 3, 15, 1, "WEB201", "Advanced React & API Engineering", "State patterns, performance tuning, and robust backend integrations", 24, 240m },
                    { 4, 12, 2, "NET101", "Routing & Switching Essentials", "Managing topology layouts, VLAN segmentation, and base gateway configs", 16, 180m }
                });

            migrationBuilder.InsertData(
                table: "CourseCategory",
                columns: new[] { "categoryID", "categoryName", "description" },
                values: new object[,]
                {
                    { 4, "Cybersecurity", "Information security and ethical penetration testing" },
                    { 5, "Cloud Systems", "Cloud computing architectures and DevOps practices" }
                });

            migrationBuilder.InsertData(
                table: "CourseSession",
                columns: new[] { "sessionID", "capacity", "classroomID", "courseID", "createdAt", "endDateTime", "instructorID", "startDateTime" },
                values: new object[,]
                {
                    { 6, 25, 2, 2, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 9, 17, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2026, 6, 9, 14, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, 20, 1, 1, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2026, 6, 15, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, 25, 2, 2, new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 22, 12, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2026, 6, 22, 9, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "equipmentID", "description", "equipmentName" },
                values: new object[,]
                {
                    { 4, "Hardware layer physical routing rig", "Cisco Router Rack" },
                    { 5, "Virtual Reality development testing tools", "VR Hardware Suite" }
                });

            migrationBuilder.InsertData(
                table: "Instructor",
                columns: new[] { "instructorID", "email", "fullName", "hireDate", "password", "phone", "qualifications" },
                values: new object[] { 3, "faisal.m@example.com", "Faisal Mahmood", new DateTime(2025, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Temp123!", "+97333998877", "PhD Cybersecurity, CCIE Security" });

            migrationBuilder.UpdateData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 1,
                column: "endTime",
                value: new TimeOnly(15, 0, 0));

            migrationBuilder.UpdateData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 2,
                columns: new[] { "availableDate", "endTime", "instructorID", "startTime" },
                values: new object[] { new DateOnly(2026, 6, 5), new TimeOnly(22, 0, 0), 1, new TimeOnly(16, 0, 0) });

            migrationBuilder.InsertData(
                table: "InstructorAvailability",
                columns: new[] { "availabilityID", "availableDate", "endTime", "instructorID", "isAvailable", "startTime" },
                values: new object[,]
                {
                    { 3, new DateOnly(2026, 6, 8), new TimeOnly(15, 0, 0), 1, true, new TimeOnly(8, 0, 0) },
                    { 4, new DateOnly(2026, 6, 15), new TimeOnly(15, 0, 0), 1, true, new TimeOnly(8, 0, 0) },
                    { 5, new DateOnly(2026, 6, 26), new TimeOnly(20, 0, 0), 1, true, new TimeOnly(12, 0, 0) },
                    { 6, new DateOnly(2026, 6, 2), new TimeOnly(14, 0, 0), 2, true, new TimeOnly(8, 0, 0) },
                    { 7, new DateOnly(2026, 6, 9), new TimeOnly(18, 0, 0), 2, true, new TimeOnly(12, 0, 0) },
                    { 8, new DateOnly(2026, 6, 22), new TimeOnly(15, 0, 0), 2, true, new TimeOnly(8, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "Trainee",
                columns: new[] { "traineeID", "email", "fullName", "organizationName", "password", "phone", "registrationDate", "traineeStatusID" },
                values: new object[] { 3, "mariam@example.com", "Mariam Al-Sayed", "XYZ Tech", "Temp123!", "+97336665544", new DateOnly(2026, 5, 4), 1 });

            migrationBuilder.InsertData(
                table: "CertificationCourse",
                columns: new[] { "certificationID", "courseID", "isRequired" },
                values: new object[] { 1, 3, false });

            migrationBuilder.InsertData(
                table: "ClassroomEquipment",
                columns: new[] { "classroomID", "equipmentID", "quantity" },
                values: new object[,]
                {
                    { 3, 1, 1 },
                    { 3, 5, 5 },
                    { 4, 4, 3 }
                });

            migrationBuilder.InsertData(
                table: "Course",
                columns: new[] { "courseID", "capacity", "categoryID", "courseCode", "courseName", "description", "durationHours", "enrollmentFee" },
                values: new object[,]
                {
                    { 5, 15, 4, "SEC101", "Introduction to Ethical Hacking", "Network footprinting, scanning mechanisms, and fundamental vulnerabilities", 20, 300m },
                    { 6, 40, 5, "CLD101", "Cloud Practitioner Fundamentals", "Core architectural baselines across multi-tenant enterprise cloud instances", 14, 200m }
                });

            migrationBuilder.InsertData(
                table: "CoursePrerequisite",
                columns: new[] { "courseID", "coursePrerequisiteID" },
                values: new object[] { 3, 1 });

            migrationBuilder.InsertData(
                table: "CourseReqEquipment",
                columns: new[] { "courseID", "equipmentID", "quantity" },
                values: new object[,]
                {
                    { 3, 2, 15 },
                    { 4, 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "CourseSession",
                columns: new[] { "sessionID", "capacity", "classroomID", "courseID", "createdAt", "endDateTime", "instructorID", "startDateTime" },
                values: new object[,]
                {
                    { 3, 12, 4, 4, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 3, 17, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2026, 6, 3, 13, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 15, 1, 3, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 8, 13, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2026, 6, 8, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, 12, 4, 4, new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 17, 17, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2026, 6, 17, 13, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "InstructorAvailability",
                columns: new[] { "availabilityID", "availableDate", "endTime", "instructorID", "isAvailable", "startTime" },
                values: new object[,]
                {
                    { 9, new DateOnly(2026, 6, 3), new TimeOnly(18, 0, 0), 3, true, new TimeOnly(12, 0, 0) },
                    { 10, new DateOnly(2026, 6, 11), new TimeOnly(16, 0, 0), 3, true, new TimeOnly(8, 0, 0) },
                    { 11, new DateOnly(2026, 6, 17), new TimeOnly(18, 0, 0), 3, true, new TimeOnly(12, 0, 0) },
                    { 12, new DateOnly(2026, 6, 24), new TimeOnly(16, 0, 0), 3, true, new TimeOnly(8, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "InstructorExpertise",
                columns: new[] { "categoryID", "instructorID" },
                values: new object[,]
                {
                    { 2, 3 },
                    { 4, 3 },
                    { 5, 1 }
                });

            migrationBuilder.InsertData(
                table: "TraineeCertificationProgress",
                columns: new[] { "certificationID", "traineeID", "achievedDate", "progressPercentage" },
                values: new object[] { 3, 3, null, 0m });

            migrationBuilder.InsertData(
                table: "CertificationCourse",
                columns: new[] { "certificationID", "courseID", "isRequired" },
                values: new object[,]
                {
                    { 3, 5, true },
                    { 4, 6, true }
                });

            migrationBuilder.InsertData(
                table: "CoursePrerequisite",
                columns: new[] { "courseID", "coursePrerequisiteID" },
                values: new object[] { 5, 4 });

            migrationBuilder.InsertData(
                table: "CourseReqEquipment",
                columns: new[] { "courseID", "equipmentID", "quantity" },
                values: new object[] { 5, 2, 5 });

            migrationBuilder.InsertData(
                table: "CourseSession",
                columns: new[] { "sessionID", "capacity", "classroomID", "courseID", "createdAt", "endDateTime", "instructorID", "startDateTime" },
                values: new object[,]
                {
                    { 4, 40, 5, 6, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 5, 21, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2026, 6, 5, 18, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, 15, 3, 5, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 11, 14, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2026, 6, 11, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, 15, 3, 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 24, 14, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2026, 6, 24, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, 40, 5, 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 26, 18, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2026, 6, 26, 15, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Enrollment",
                columns: new[] { "enrollmentID", "enrollmentDate", "enrollmentStatusID", "sessionID", "traineeID" },
                values: new object[] { 3, new DateOnly(2026, 5, 7), 2, 3, 3 });

            migrationBuilder.InsertData(
                table: "Assessment",
                columns: new[] { "assessmentID", "enrollmentID", "instructorID", "result", "score" },
                values: new object[] { 2, 3, 3, 0, 52 });

            migrationBuilder.InsertData(
                table: "Enrollment",
                columns: new[] { "enrollmentID", "enrollmentDate", "enrollmentStatusID", "sessionID", "traineeID" },
                values: new object[] { 4, new DateOnly(2026, 5, 10), 5, 7, 1 });

            migrationBuilder.InsertData(
                table: "Payment",
                columns: new[] { "paymentID", "amountPaid", "balanceRemaining", "enrollmentID", "paymentDate", "paymentStatusID" },
                values: new object[,]
                {
                    { 3, 180m, 0m, 3, new DateOnly(2026, 5, 8), 2 },
                    { 4, 0m, 300m, 4, new DateOnly(2026, 5, 10), 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assessment",
                keyColumn: "assessmentID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CertificationCourse",
                keyColumns: new[] { "certificationID", "courseID" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "CertificationCourse",
                keyColumns: new[] { "certificationID", "courseID" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "CertificationCourse",
                keyColumns: new[] { "certificationID", "courseID" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "classroomID", "equipmentID" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "classroomID", "equipmentID" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "ClassroomEquipment",
                keyColumns: new[] { "classroomID", "equipmentID" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "CoursePrerequisite",
                keyColumns: new[] { "courseID", "coursePrerequisiteID" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "CoursePrerequisite",
                keyColumns: new[] { "courseID", "coursePrerequisiteID" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "CourseReqEquipment",
                keyColumns: new[] { "courseID", "equipmentID" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "CourseReqEquipment",
                keyColumns: new[] { "courseID", "equipmentID" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "CourseReqEquipment",
                keyColumns: new[] { "courseID", "equipmentID" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "InstructorExpertise",
                keyColumns: new[] { "categoryID", "instructorID" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "InstructorExpertise",
                keyColumns: new[] { "categoryID", "instructorID" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "InstructorExpertise",
                keyColumns: new[] { "categoryID", "instructorID" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "Payment",
                keyColumn: "paymentID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Payment",
                keyColumn: "paymentID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TraineeCertificationProgress",
                keyColumns: new[] { "certificationID", "traineeID" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Certification",
                keyColumn: "certificationID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Certification",
                keyColumn: "certificationID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Classroom",
                keyColumn: "classroomID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Course",
                keyColumn: "courseID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Course",
                keyColumn: "courseID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Enrollment",
                keyColumn: "enrollmentID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Enrollment",
                keyColumn: "enrollmentID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "equipmentID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "equipmentID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CourseCategory",
                keyColumn: "categoryID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CourseSession",
                keyColumn: "sessionID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Trainee",
                keyColumn: "traineeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Classroom",
                keyColumn: "classroomID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Classroom",
                keyColumn: "classroomID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Course",
                keyColumn: "courseID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Course",
                keyColumn: "courseID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Instructor",
                keyColumn: "instructorID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CourseCategory",
                keyColumn: "categoryID",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 1,
                column: "endTime",
                value: new TimeOnly(14, 0, 0));

            migrationBuilder.UpdateData(
                table: "InstructorAvailability",
                keyColumn: "availabilityID",
                keyValue: 2,
                columns: new[] { "availableDate", "endTime", "instructorID", "startTime" },
                values: new object[] { new DateOnly(2026, 6, 2), new TimeOnly(14, 0, 0), 2, new TimeOnly(8, 0, 0) });
        }
    }
}
