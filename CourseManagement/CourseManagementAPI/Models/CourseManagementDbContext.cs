using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

public partial class CourseManagementDbContext : DbContext
{
    public CourseManagementDbContext()
    {
    }

    public CourseManagementDbContext(DbContextOptions<CourseManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assessment> Assessments { get; set; }
    public virtual DbSet<Certification> Certifications { get; set; }
    public virtual DbSet<CertificationCourse> CertificationCourses { get; set; }
    public virtual DbSet<Classroom> Classrooms { get; set; }
    public virtual DbSet<ClassroomEquipment> ClassroomEquipments { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<CourseCategory> CourseCategories { get; set; }
    public virtual DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
    public virtual DbSet<CourseReqEquipment> CourseReqEquipments { get; set; }
    public virtual DbSet<CourseSession> CourseSessions { get; set; }
    public virtual DbSet<Enrollment> Enrollments { get; set; }
    public virtual DbSet<EnrollmentStatus> EnrollmentStatuses { get; set; }
    public virtual DbSet<Equipment> Equipments { get; set; }
    public virtual DbSet<Instructor> Instructors { get; set; }
    public virtual DbSet<InstructorAvailability> InstructorAvailabilities { get; set; }
    public virtual DbSet<InstructorExpertise> InstructorExpertises { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }
    public virtual DbSet<Trainee> Trainees { get; set; }
    public virtual DbSet<TraineeCertificationProgress> TraineeCertificationProgresses { get; set; }
    public virtual DbSet<TraineeStatus> TraineeStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.ToTable("Assessment");

            entity.HasKey(e => e.AssessmentId);

            entity.Property(e => e.AssessmentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("assessmentID");

            entity.Property(e => e.EnrollmentId).HasColumnName("enrollmentID");
            entity.Property(e => e.InstructorId).HasColumnName("instructorID");
            entity.Property(e => e.Result).HasColumnName("result");
            entity.Property(e => e.Score).HasColumnName("score");

            entity.HasOne(e => e.Enrollment)
                .WithMany(e => e.Assessments)
                .HasForeignKey(e => e.EnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.Instructor)
                .WithMany(e => e.Assessments)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Certification>(entity =>
        {
            entity.ToTable("Certification");

            entity.HasKey(e => e.CertificationId);

            entity.Property(e => e.CertificationId)
                .ValueGeneratedOnAdd()
                .HasColumnName("certificationID");

            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        modelBuilder.Entity<CertificationCourse>(entity =>
        {
            entity.ToTable("CertificationCourse");

            entity.HasKey(e => new { e.CourseId, e.CertificationId });

            entity.Property(e => e.CourseId).HasColumnName("courseID");
            entity.Property(e => e.CertificationId).HasColumnName("certificationID");
            entity.Property(e => e.IsRequired).HasColumnName("isRequired");

            entity.HasOne(e => e.Course)
                .WithMany(e => e.CertificationCourses)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.Certification)
                .WithMany(e => e.CertificationCourses)
                .HasForeignKey(e => e.CertificationId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.ToTable("Classroom");

            entity.HasKey(e => e.ClassroomId);

            entity.Property(e => e.ClassroomId)
                .ValueGeneratedOnAdd()
                .HasColumnName("classroomID");

            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
        });

        modelBuilder.Entity<ClassroomEquipment>(entity =>
        {
            entity.ToTable("ClassroomEquipment");

            entity.HasKey(e => new { e.ClassroomId, e.EquipmentId });

            entity.Property(e => e.ClassroomId).HasColumnName("classroomID");
            entity.Property(e => e.EquipmentId).HasColumnName("equipmentID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(e => e.Classroom)
                .WithMany(e => e.ClassroomEquipments)
                .HasForeignKey(e => e.ClassroomId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.Equipment)
                .WithMany(e => e.ClassroomEquipments)
                .HasForeignKey(e => e.EquipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");

            entity.HasKey(e => e.CourseId);

            entity.Property(e => e.CourseId)
                .ValueGeneratedOnAdd()
                .HasColumnName("courseID");

            entity.Property(e => e.CourseCode).HasColumnName("courseCode");
            entity.Property(e => e.CourseName).HasColumnName("courseName");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Capacity).HasColumnName("capacity");

            entity.Property(e => e.EnrollmentFee)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("enrollmentFee");

            entity.Property(e => e.CategoryId).HasColumnName("categoryID");

            entity.HasOne(e => e.Category)
                .WithMany(e => e.Courses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.ToTable("CourseCategory");

            entity.HasKey(e => e.CategoryId);

            entity.Property(e => e.CategoryId)
                .ValueGeneratedOnAdd()
                .HasColumnName("categoryID");

            entity.Property(e => e.CategoryName).HasColumnName("categoryName");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        modelBuilder.Entity<CoursePrerequisite>(entity =>
        {
            entity.ToTable("CoursePrerequisite");

            entity.HasKey(e => new { e.CourseId, e.CoursePrerequisiteId });

            entity.Property(e => e.CourseId).HasColumnName("courseID");
            entity.Property(e => e.CoursePrerequisiteId).HasColumnName("coursePrerequisiteID");

            entity.HasOne(e => e.Course)
                .WithMany(e => e.CoursePrerequisites)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.PrerequisiteCourse)
                .WithMany(e => e.RequiredForCourses)
                .HasForeignKey(e => e.CoursePrerequisiteId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CourseReqEquipment>(entity =>
        {
            entity.ToTable("CourseReqEquipment");

            entity.HasKey(e => new { e.EquipmentId, e.CourseId });

            entity.Property(e => e.EquipmentId).HasColumnName("equipmentID");
            entity.Property(e => e.CourseId).HasColumnName("courseID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(e => e.Equipment)
                .WithMany(e => e.CourseReqEquipments)
                .HasForeignKey(e => e.EquipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.Course)
                .WithMany(e => e.CourseReqEquipments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CourseSession>(entity =>
        {
            entity.ToTable("CourseSession");

            entity.HasKey(e => e.SessionId);

            entity.Property(e => e.SessionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("sessionID");

            entity.Property(e => e.InstructorId).HasColumnName("instructorID");
            entity.Property(e => e.CourseId).HasColumnName("courseID");
            entity.Property(e => e.ClassroomId).HasColumnName("classroomID");
            entity.Property(e => e.StartDateTime).HasColumnName("startDateTime");
            entity.Property(e => e.EndDateTime).HasColumnName("endDateTime");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

            entity.HasOne(e => e.Instructor)
                .WithMany(e => e.CourseSessions)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.Course)
                .WithMany(e => e.CourseSessions)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.Classroom)
                .WithMany(e => e.CourseSessions)
                .HasForeignKey(e => e.ClassroomId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollment");

            entity.HasKey(e => e.EnrollmentId);

            entity.Property(e => e.EnrollmentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("enrollmentID");

            entity.Property(e => e.TraineeId).HasColumnName("traineeID");
            entity.Property(e => e.SessionId).HasColumnName("sessionID");
            entity.Property(e => e.EnrollmentDate).HasColumnName("enrollmentDate");
            entity.Property(e => e.EnrollmentStatusId).HasColumnName("enrollmentStatusID");

            entity.HasOne(e => e.Trainee)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(e => e.TraineeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.Session)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.EnrollmentStatus)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(e => e.EnrollmentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EnrollmentStatus>(entity =>
        {
            entity.ToTable("EnrollmentStatus");

            entity.HasKey(e => e.EnrollmentStatusId);

            entity.Property(e => e.EnrollmentStatusId)
                .ValueGeneratedOnAdd()
                .HasColumnName("enrollmentStatusID");

            entity.Property(e => e.StatusName).HasColumnName("statusName");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.ToTable("Equipment");

            entity.HasKey(e => e.EquipmentId);

            entity.Property(e => e.EquipmentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("equipmentID");

            entity.Property(e => e.EquipmentName).HasColumnName("equipmentName");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.ToTable("Instructor");

            entity.HasKey(e => e.InstructorId);

            entity.Property(e => e.InstructorId)
                .ValueGeneratedOnAdd()
                .HasColumnName("instructorID");

            entity.Property(e => e.FullName).HasColumnName("fullName");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Qualifications).HasColumnName("qualifications");
            entity.Property(e => e.HireDate).HasColumnName("hireDate");
        });

        modelBuilder.Entity<InstructorAvailability>(entity =>
        {
            entity.ToTable("InstructorAvailability");

            entity.HasKey(e => e.AvailabilityId);

            entity.Property(e => e.AvailabilityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("availabilityID");

            entity.Property(e => e.AvailableDate).HasColumnName("availableDate");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.IsAvailable).HasColumnName("isAvailable");
            entity.Property(e => e.InstructorId).HasColumnName("instructorID");

            entity.HasOne(e => e.Instructor)
                .WithMany(e => e.InstructorAvailabilities)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InstructorExpertise>(entity =>
        {
            entity.ToTable("InstructorExpertise");

            entity.HasKey(e => new { e.CategoryId, e.InstructorId });

            entity.Property(e => e.CategoryId).HasColumnName("categoryID");
            entity.Property(e => e.InstructorId).HasColumnName("instructorID");

            entity.HasOne(e => e.Category)
                .WithMany(e => e.InstructorExpertises)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.Instructor)
                .WithMany(e => e.InstructorExpertises)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.HasKey(e => e.NotificationId);

            entity.Property(e => e.NotificationId)
                .ValueGeneratedOnAdd()
                .HasColumnName("notificationID");

            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.SessionId).HasColumnName("sessionID");
            entity.Property(e => e.PaymentId).HasColumnName("paymentID");

            entity.HasOne(e => e.Session)
                .WithMany(e => e.Notifications)
                .HasForeignKey(e => e.SessionId);

            entity.HasOne(e => e.Payment)
                .WithMany(e => e.Notifications)
                .HasForeignKey(e => e.PaymentId);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.HasKey(e => e.PaymentId);

            entity.Property(e => e.PaymentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("paymentID");

            entity.Property(e => e.EnrollmentId).HasColumnName("enrollmentID");

            entity.Property(e => e.AmountPaid)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("amountPaid");

            entity.Property(e => e.PaymentDate).HasColumnName("paymentDate");
            entity.Property(e => e.PaymentStatusId).HasColumnName("paymentStatusID");

            entity.Property(e => e.BalanceRemaining)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("balanceRemaining");

            entity.HasOne(e => e.Enrollment)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.EnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.PaymentStatus)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.ToTable("PaymentStatus");

            entity.HasKey(e => e.PaymentStatusId);

            entity.Property(e => e.PaymentStatusId)
                .ValueGeneratedOnAdd()
                .HasColumnName("paymentStatusID");

            entity.Property(e => e.StatusName).HasColumnName("statusName");
        });

        modelBuilder.Entity<Trainee>(entity =>
        {
            entity.ToTable("Trainee");

            entity.HasKey(e => e.TraineeId);

            entity.Property(e => e.TraineeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("traineeID");

            entity.Property(e => e.FullName).HasColumnName("fullName");
            entity.Property(e => e.OrganizationName).HasColumnName("organizationName");
            entity.Property(e => e.RegistrationDate).HasColumnName("registrationDate");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.TraineeStatusId).HasColumnName("traineeStatusID");

            entity.HasOne(e => e.TraineeStatus)
                .WithMany(e => e.Trainees)
                .HasForeignKey(e => e.TraineeStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TraineeCertificationProgress>(entity =>
        {
            entity.ToTable("TraineeCertificationProgress");

            entity.HasKey(e => new { e.TraineeId, e.CertificationId });

            entity.Property(e => e.TraineeId).HasColumnName("traineeID");
            entity.Property(e => e.CertificationId).HasColumnName("certificationID");
            entity.Property(e => e.AchievedDate).HasColumnName("achievedDate");

            entity.Property(e => e.ProgressPercentage)
                .HasColumnType("decimal(5,2)")
                .HasColumnName("progressPercentage");

            entity.HasOne(e => e.Trainee)
                .WithMany(e => e.TraineeCertificationProgresses)
                .HasForeignKey(e => e.TraineeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.Certification)
                .WithMany(e => e.TraineeCertificationProgresses)
                .HasForeignKey(e => e.CertificationId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TraineeStatus>(entity =>
        {
            entity.ToTable("TraineeStatus");

            entity.HasKey(e => e.TraineeStatusId);

            entity.Property(e => e.TraineeStatusId)
                .ValueGeneratedOnAdd()
                .HasColumnName("traineeStatusID");

            entity.Property(e => e.StatusName).HasColumnName("statusName");
        });

        // Seed Status Tables
        modelBuilder.Entity<TraineeStatus>().HasData(
            new TraineeStatus { TraineeStatusId = 1, StatusName = "Active" },
            new TraineeStatus { TraineeStatusId = 2, StatusName = "Inactive" }
        );

        modelBuilder.Entity<EnrollmentStatus>().HasData(
            new EnrollmentStatus { EnrollmentStatusId = 1, StatusName = "Pending" },
            new EnrollmentStatus { EnrollmentStatusId = 2, StatusName = "Enrolled" },
            new EnrollmentStatus { EnrollmentStatusId = 3, StatusName = "Completed" },
            new EnrollmentStatus { EnrollmentStatusId = 4, StatusName = "Cancelled" }
        );

        modelBuilder.Entity<PaymentStatus>().HasData(
            new PaymentStatus { PaymentStatusId = 1, StatusName = "Pending" },
            new PaymentStatus { PaymentStatusId = 2, StatusName = "Paid" },
            new PaymentStatus { PaymentStatusId = 3, StatusName = "Partially Paid" },
            new PaymentStatus { PaymentStatusId = 4, StatusName = "Unpaid" }
        );

        // Seed Main Lookup Tables
        modelBuilder.Entity<CourseCategory>().HasData(
            new CourseCategory { CategoryId = 1, CategoryName = "Programming", Description = "Software and coding courses" },
            new CourseCategory { CategoryId = 2, CategoryName = "Networking", Description = "Network and infrastructure courses" },
            new CourseCategory { CategoryId = 3, CategoryName = "Database", Description = "Database management courses" }
        );

        modelBuilder.Entity<Equipment>().HasData(
            new Equipment { EquipmentId = 1, EquipmentName = "Projector", Description = "Classroom projector" },
            new Equipment { EquipmentId = 2, EquipmentName = "Laptop", Description = "Training laptop" },
            new Equipment { EquipmentId = 3, EquipmentName = "Whiteboard", Description = "Classroom whiteboard" }
        );

        modelBuilder.Entity<Classroom>().HasData(
            new Classroom { ClassroomId = 1, Location = "Room A101", Capacity = 25, IsActive = true },
            new Classroom { ClassroomId = 2, Location = "Room B202", Capacity = 30, IsActive = true }
        );

        modelBuilder.Entity<Certification>().HasData(
            new Certification { CertificationId = 1, Name = "Web Development Certificate", Description = "Certificate for web development skills" },
            new Certification { CertificationId = 2, Name = "Database Fundamentals Certificate", Description = "Certificate for database basics" }
        );

        // Seed People
        modelBuilder.Entity<Instructor>().HasData(
            new Instructor
            {
                InstructorId = 1,
                FullName = "Ahmed Ali",
                Email = "ahmed.ali@example.com",
                Phone = "+97333112244",
                Password = "Temp123",
                Qualifications = "MSc Computer Science",
                HireDate = new DateTime(2024, 1, 10)
            },
            new Instructor
            {
                InstructorId = 2,
                FullName = "Sara Hassan",
                Email = "sara.hassan@example.com",
                Phone = "+97333225566",
                Password = "Temp123",
                Qualifications = "BSc Information Technology",
                HireDate = new DateTime(2024, 2, 15)
            }
        );

        modelBuilder.Entity<Trainee>().HasData(
            new Trainee
            {
                TraineeId = 1,
                FullName = "Noor Mohammed",
                OrganizationName = "ABC Company",
                RegistrationDate = new DateOnly(2026, 5, 1),
                Email = "noor@example.com",
                Phone = "+97339998877",
                Password = "Temp123",
                TraineeStatusId = 1
            },
            new Trainee
            {
                TraineeId = 2,
                FullName = "Ali Yusuf",
                OrganizationName = null,
                RegistrationDate = new DateOnly(2026, 5, 2),
                Email = "ali@example.com",
                Phone = "+97338887766",
                Password = "Temp123",
                TraineeStatusId = 1
            }
        );

        // Seed Courses
        modelBuilder.Entity<Course>().HasData(
            new Course
            {
                CourseId = 1,
                CourseCode = "WEB101",
                CourseName = "Introduction to Web Development",
                Description = "HTML, CSS, and JavaScript basics",
                Capacity = 20,
                EnrollmentFee = 120,
                CategoryId = 1
            },
            new Course
            {
                CourseId = 2,
                CourseCode = "DB101",
                CourseName = "Database Fundamentals",
                Description = "Introduction to relational databases",
                Capacity = 25,
                EnrollmentFee = 100,
                CategoryId = 3
            }
        );

        // Seed Junction Tables
        modelBuilder.Entity<CertificationCourse>().HasData(
            new CertificationCourse { CourseId = 1, CertificationId = 1, IsRequired = true },
            new CertificationCourse { CourseId = 2, CertificationId = 2, IsRequired = true }
        );

        modelBuilder.Entity<ClassroomEquipment>().HasData(
            new ClassroomEquipment { ClassroomId = 1, EquipmentId = 1, Quantity = 1 },
            new ClassroomEquipment { ClassroomId = 1, EquipmentId = 3, Quantity = 1 },
            new ClassroomEquipment { ClassroomId = 2, EquipmentId = 1, Quantity = 1 },
            new ClassroomEquipment { ClassroomId = 2, EquipmentId = 2, Quantity = 10 }
        );

        modelBuilder.Entity<CourseReqEquipment>().HasData(
            new CourseReqEquipment { CourseId = 1, EquipmentId = 2, Quantity = 10 },
            new CourseReqEquipment { CourseId = 2, EquipmentId = 2, Quantity = 10 }
        );

        modelBuilder.Entity<InstructorExpertise>().HasData(
            new InstructorExpertise { InstructorId = 1, CategoryId = 1 },
            new InstructorExpertise { InstructorId = 2, CategoryId = 3 }
        );

        modelBuilder.Entity<CoursePrerequisite>().HasData(
            new CoursePrerequisite { CourseId = 2, CoursePrerequisiteId = 1 }
        );

        // Seed Sessions
        modelBuilder.Entity<CourseSession>().HasData(
            new CourseSession
            {
                SessionId = 1,
                InstructorId = 1,
                CourseId = 1,
                ClassroomId = 1,
                StartDateTime = new DateTime(2026, 6, 1, 9, 0, 0),
                EndDateTime = new DateTime(2026, 6, 1, 12, 0, 0),
                Capacity = 20,
                CreatedAt = new DateTime(2026, 5, 11, 0, 0, 0)
            },
            new CourseSession
            {
                SessionId = 2,
                InstructorId = 2,
                CourseId = 2,
                ClassroomId = 2,
                StartDateTime = new DateTime(2026, 6, 2, 9, 0, 0),
                EndDateTime = new DateTime(2026, 6, 2, 12, 0, 0),
                Capacity = 25,
                CreatedAt = new DateTime(2026, 5, 11, 0, 0, 0)
            }
        );

        modelBuilder.Entity<InstructorAvailability>().HasData(
            new InstructorAvailability
            {
                AvailabilityId = 1,
                InstructorId = 1,
                AvailableDate = new DateOnly(2026, 6, 1),
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(14, 0),
                IsAvailable = true
            },
            new InstructorAvailability
            {
                AvailabilityId = 2,
                InstructorId = 2,
                AvailableDate = new DateOnly(2026, 6, 2),
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(14, 0),
                IsAvailable = true
            }
        );

        // Seed Enrollment, Payment, Assessment, Notification
        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment
            {
                EnrollmentId = 1,
                TraineeId = 1,
                SessionId = 1,
                EnrollmentDate = new DateOnly(2026, 5, 5),
                EnrollmentStatusId = 2
            },
            new Enrollment
            {
                EnrollmentId = 2,
                TraineeId = 2,
                SessionId = 2,
                EnrollmentDate = new DateOnly(2026, 5, 6),
                EnrollmentStatusId = 1
            }
        );

        modelBuilder.Entity<Payment>().HasData(
            new Payment
            {
                PaymentId = 1,
                EnrollmentId = 1,
                AmountPaid = 120,
                PaymentDate = new DateOnly(2026, 5, 6),
                PaymentStatusId = 2,
                BalanceRemaining = 0
            },
            new Payment
            {
                PaymentId = 2,
                EnrollmentId = 2,
                AmountPaid = 50,
                PaymentDate = new DateOnly(2026, 5, 7),
                PaymentStatusId = 3,
                BalanceRemaining = 50
            }
        );

        modelBuilder.Entity<Assessment>().HasData(
            new Assessment
            {
                AssessmentId = 1,
                EnrollmentId = 1,
                InstructorId = 1,
                Result = 1,
                Score = 85
            }
        );

        modelBuilder.Entity<Notification>().HasData(
            new Notification
            {
                NotificationId = 1,
                Title = "Payment Received",
                Message = "Your payment has been received.",
                PaymentId = 1,
                SessionId = null
            },
            new Notification
            {
                NotificationId = 2,
                Title = "Session Reminder",
                Message = "Your course session starts soon.",
                SessionId = 1,
                PaymentId = null
            }
        );

        modelBuilder.Entity<TraineeCertificationProgress>().HasData(
            new TraineeCertificationProgress
            {
                TraineeId = 1,
                CertificationId = 1,
                AchievedDate = null,
                ProgressPercentage = 50
            }
        );
        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}