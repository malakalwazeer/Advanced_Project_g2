using System;
using System.Collections.Generic;
using CourseManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Data;

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
        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}