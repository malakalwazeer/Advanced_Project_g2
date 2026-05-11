using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Certification",
                columns: table => new
                {
                    certificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certification", x => x.certificationID);
                });

            migrationBuilder.CreateTable(
                name: "Classroom",
                columns: table => new
                {
                    classroomID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classroom", x => x.classroomID);
                });

            migrationBuilder.CreateTable(
                name: "CourseCategory",
                columns: table => new
                {
                    categoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCategory", x => x.categoryID);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentStatus",
                columns: table => new
                {
                    enrollmentStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    statusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentStatus", x => x.enrollmentStatusID);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    equipmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    equipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.equipmentID);
                });

            migrationBuilder.CreateTable(
                name: "Instructor",
                columns: table => new
                {
                    instructorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qualifications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hireDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructor", x => x.instructorID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatus",
                columns: table => new
                {
                    paymentStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    statusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStatus", x => x.paymentStatusID);
                });

            migrationBuilder.CreateTable(
                name: "TraineeStatus",
                columns: table => new
                {
                    traineeStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    statusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeStatus", x => x.traineeStatusID);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    courseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    courseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    enrollmentFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    categoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.courseID);
                    table.ForeignKey(
                        name: "FK_Course_CourseCategory_categoryID",
                        column: x => x.categoryID,
                        principalTable: "CourseCategory",
                        principalColumn: "categoryID");
                });

            migrationBuilder.CreateTable(
                name: "ClassroomEquipment",
                columns: table => new
                {
                    classroomID = table.Column<int>(type: "int", nullable: false),
                    equipmentID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomEquipment", x => new { x.classroomID, x.equipmentID });
                    table.ForeignKey(
                        name: "FK_ClassroomEquipment_Classroom_classroomID",
                        column: x => x.classroomID,
                        principalTable: "Classroom",
                        principalColumn: "classroomID");
                    table.ForeignKey(
                        name: "FK_ClassroomEquipment_Equipment_equipmentID",
                        column: x => x.equipmentID,
                        principalTable: "Equipment",
                        principalColumn: "equipmentID");
                });

            migrationBuilder.CreateTable(
                name: "InstructorAvailability",
                columns: table => new
                {
                    availabilityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    availableDate = table.Column<DateOnly>(type: "date", nullable: false),
                    startTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    endTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    isAvailable = table.Column<bool>(type: "bit", nullable: false),
                    instructorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorAvailability", x => x.availabilityID);
                    table.ForeignKey(
                        name: "FK_InstructorAvailability_Instructor_instructorID",
                        column: x => x.instructorID,
                        principalTable: "Instructor",
                        principalColumn: "instructorID");
                });

            migrationBuilder.CreateTable(
                name: "InstructorExpertise",
                columns: table => new
                {
                    categoryID = table.Column<int>(type: "int", nullable: false),
                    instructorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorExpertise", x => new { x.categoryID, x.instructorID });
                    table.ForeignKey(
                        name: "FK_InstructorExpertise_CourseCategory_categoryID",
                        column: x => x.categoryID,
                        principalTable: "CourseCategory",
                        principalColumn: "categoryID");
                    table.ForeignKey(
                        name: "FK_InstructorExpertise_Instructor_instructorID",
                        column: x => x.instructorID,
                        principalTable: "Instructor",
                        principalColumn: "instructorID");
                });

            migrationBuilder.CreateTable(
                name: "Trainee",
                columns: table => new
                {
                    traineeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    organizationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    registrationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    traineeStatusID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainee", x => x.traineeID);
                    table.ForeignKey(
                        name: "FK_Trainee_TraineeStatus_traineeStatusID",
                        column: x => x.traineeStatusID,
                        principalTable: "TraineeStatus",
                        principalColumn: "traineeStatusID");
                });

            migrationBuilder.CreateTable(
                name: "CertificationCourse",
                columns: table => new
                {
                    courseID = table.Column<int>(type: "int", nullable: false),
                    certificationID = table.Column<int>(type: "int", nullable: false),
                    isRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationCourse", x => new { x.courseID, x.certificationID });
                    table.ForeignKey(
                        name: "FK_CertificationCourse_Certification_certificationID",
                        column: x => x.certificationID,
                        principalTable: "Certification",
                        principalColumn: "certificationID");
                    table.ForeignKey(
                        name: "FK_CertificationCourse_Course_courseID",
                        column: x => x.courseID,
                        principalTable: "Course",
                        principalColumn: "courseID");
                });

            migrationBuilder.CreateTable(
                name: "CoursePrerequisite",
                columns: table => new
                {
                    courseID = table.Column<int>(type: "int", nullable: false),
                    coursePrerequisiteID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrerequisite", x => new { x.courseID, x.coursePrerequisiteID });
                    table.ForeignKey(
                        name: "FK_CoursePrerequisite_Course_courseID",
                        column: x => x.courseID,
                        principalTable: "Course",
                        principalColumn: "courseID");
                    table.ForeignKey(
                        name: "FK_CoursePrerequisite_Course_coursePrerequisiteID",
                        column: x => x.coursePrerequisiteID,
                        principalTable: "Course",
                        principalColumn: "courseID");
                });

            migrationBuilder.CreateTable(
                name: "CourseReqEquipment",
                columns: table => new
                {
                    equipmentID = table.Column<int>(type: "int", nullable: false),
                    courseID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseReqEquipment", x => new { x.equipmentID, x.courseID });
                    table.ForeignKey(
                        name: "FK_CourseReqEquipment_Course_courseID",
                        column: x => x.courseID,
                        principalTable: "Course",
                        principalColumn: "courseID");
                    table.ForeignKey(
                        name: "FK_CourseReqEquipment_Equipment_equipmentID",
                        column: x => x.equipmentID,
                        principalTable: "Equipment",
                        principalColumn: "equipmentID");
                });

            migrationBuilder.CreateTable(
                name: "CourseSession",
                columns: table => new
                {
                    sessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    instructorID = table.Column<int>(type: "int", nullable: false),
                    courseID = table.Column<int>(type: "int", nullable: false),
                    classroomID = table.Column<int>(type: "int", nullable: false),
                    startDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSession", x => x.sessionID);
                    table.ForeignKey(
                        name: "FK_CourseSession_Classroom_classroomID",
                        column: x => x.classroomID,
                        principalTable: "Classroom",
                        principalColumn: "classroomID");
                    table.ForeignKey(
                        name: "FK_CourseSession_Course_courseID",
                        column: x => x.courseID,
                        principalTable: "Course",
                        principalColumn: "courseID");
                    table.ForeignKey(
                        name: "FK_CourseSession_Instructor_instructorID",
                        column: x => x.instructorID,
                        principalTable: "Instructor",
                        principalColumn: "instructorID");
                });

            migrationBuilder.CreateTable(
                name: "TraineeCertificationProgress",
                columns: table => new
                {
                    traineeID = table.Column<int>(type: "int", nullable: false),
                    certificationID = table.Column<int>(type: "int", nullable: false),
                    achievedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    progressPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeCertificationProgress", x => new { x.traineeID, x.certificationID });
                    table.ForeignKey(
                        name: "FK_TraineeCertificationProgress_Certification_certificationID",
                        column: x => x.certificationID,
                        principalTable: "Certification",
                        principalColumn: "certificationID");
                    table.ForeignKey(
                        name: "FK_TraineeCertificationProgress_Trainee_traineeID",
                        column: x => x.traineeID,
                        principalTable: "Trainee",
                        principalColumn: "traineeID");
                });

            migrationBuilder.CreateTable(
                name: "Enrollment",
                columns: table => new
                {
                    enrollmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    traineeID = table.Column<int>(type: "int", nullable: false),
                    sessionID = table.Column<int>(type: "int", nullable: false),
                    enrollmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    enrollmentStatusID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollment", x => x.enrollmentID);
                    table.ForeignKey(
                        name: "FK_Enrollment_CourseSession_sessionID",
                        column: x => x.sessionID,
                        principalTable: "CourseSession",
                        principalColumn: "sessionID");
                    table.ForeignKey(
                        name: "FK_Enrollment_EnrollmentStatus_enrollmentStatusID",
                        column: x => x.enrollmentStatusID,
                        principalTable: "EnrollmentStatus",
                        principalColumn: "enrollmentStatusID");
                    table.ForeignKey(
                        name: "FK_Enrollment_Trainee_traineeID",
                        column: x => x.traineeID,
                        principalTable: "Trainee",
                        principalColumn: "traineeID");
                });

            migrationBuilder.CreateTable(
                name: "Assessment",
                columns: table => new
                {
                    assessmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    enrollmentID = table.Column<int>(type: "int", nullable: false),
                    instructorID = table.Column<int>(type: "int", nullable: false),
                    result = table.Column<int>(type: "int", nullable: true),
                    score = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessment", x => x.assessmentID);
                    table.ForeignKey(
                        name: "FK_Assessment_Enrollment_enrollmentID",
                        column: x => x.enrollmentID,
                        principalTable: "Enrollment",
                        principalColumn: "enrollmentID");
                    table.ForeignKey(
                        name: "FK_Assessment_Instructor_instructorID",
                        column: x => x.instructorID,
                        principalTable: "Instructor",
                        principalColumn: "instructorID");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    paymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    enrollmentID = table.Column<int>(type: "int", nullable: false),
                    amountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    paymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    paymentStatusID = table.Column<int>(type: "int", nullable: false),
                    balanceRemaining = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.paymentID);
                    table.ForeignKey(
                        name: "FK_Payment_Enrollment_enrollmentID",
                        column: x => x.enrollmentID,
                        principalTable: "Enrollment",
                        principalColumn: "enrollmentID");
                    table.ForeignKey(
                        name: "FK_Payment_PaymentStatus_paymentStatusID",
                        column: x => x.paymentStatusID,
                        principalTable: "PaymentStatus",
                        principalColumn: "paymentStatusID");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sessionID = table.Column<int>(type: "int", nullable: true),
                    paymentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.notificationID);
                    table.ForeignKey(
                        name: "FK_Notification_CourseSession_sessionID",
                        column: x => x.sessionID,
                        principalTable: "CourseSession",
                        principalColumn: "sessionID");
                    table.ForeignKey(
                        name: "FK_Notification_Payment_paymentID",
                        column: x => x.paymentID,
                        principalTable: "Payment",
                        principalColumn: "paymentID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_enrollmentID",
                table: "Assessment",
                column: "enrollmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_instructorID",
                table: "Assessment",
                column: "instructorID");

            migrationBuilder.CreateIndex(
                name: "IX_CertificationCourse_certificationID",
                table: "CertificationCourse",
                column: "certificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomEquipment_equipmentID",
                table: "ClassroomEquipment",
                column: "equipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_categoryID",
                table: "Course",
                column: "categoryID");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequisite_coursePrerequisiteID",
                table: "CoursePrerequisite",
                column: "coursePrerequisiteID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReqEquipment_courseID",
                table: "CourseReqEquipment",
                column: "courseID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSession_classroomID",
                table: "CourseSession",
                column: "classroomID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSession_courseID",
                table: "CourseSession",
                column: "courseID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSession_instructorID",
                table: "CourseSession",
                column: "instructorID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_enrollmentStatusID",
                table: "Enrollment",
                column: "enrollmentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_sessionID",
                table: "Enrollment",
                column: "sessionID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_traineeID",
                table: "Enrollment",
                column: "traineeID");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorAvailability_instructorID",
                table: "InstructorAvailability",
                column: "instructorID");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorExpertise_instructorID",
                table: "InstructorExpertise",
                column: "instructorID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_paymentID",
                table: "Notification",
                column: "paymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_sessionID",
                table: "Notification",
                column: "sessionID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_enrollmentID",
                table: "Payment",
                column: "enrollmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_paymentStatusID",
                table: "Payment",
                column: "paymentStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Trainee_traineeStatusID",
                table: "Trainee",
                column: "traineeStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeCertificationProgress_certificationID",
                table: "TraineeCertificationProgress",
                column: "certificationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assessment");

            migrationBuilder.DropTable(
                name: "CertificationCourse");

            migrationBuilder.DropTable(
                name: "ClassroomEquipment");

            migrationBuilder.DropTable(
                name: "CoursePrerequisite");

            migrationBuilder.DropTable(
                name: "CourseReqEquipment");

            migrationBuilder.DropTable(
                name: "InstructorAvailability");

            migrationBuilder.DropTable(
                name: "InstructorExpertise");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "TraineeCertificationProgress");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Certification");

            migrationBuilder.DropTable(
                name: "Enrollment");

            migrationBuilder.DropTable(
                name: "PaymentStatus");

            migrationBuilder.DropTable(
                name: "CourseSession");

            migrationBuilder.DropTable(
                name: "EnrollmentStatus");

            migrationBuilder.DropTable(
                name: "Trainee");

            migrationBuilder.DropTable(
                name: "Classroom");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "Instructor");

            migrationBuilder.DropTable(
                name: "TraineeStatus");

            migrationBuilder.DropTable(
                name: "CourseCategory");
        }
    }
}
