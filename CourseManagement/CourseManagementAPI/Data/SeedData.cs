using System;
using CourseManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Data;

public static class SeedData
{
    public static void Initialize(this ModelBuilder modelBuilder)
    {
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
    }
}