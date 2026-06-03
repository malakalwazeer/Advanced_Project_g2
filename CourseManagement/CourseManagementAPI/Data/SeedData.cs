using System;
using CourseManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Data;

public static class SeedData
{
    public static void Initialize(this ModelBuilder modelBuilder)
    {
        // ============================================================================
        // 1. SEED STATUS TABLES
        // ============================================================================
        modelBuilder.Entity<TraineeStatus>().HasData(
            new TraineeStatus { TraineeStatusId = 1, StatusName = "Active" },
            new TraineeStatus { TraineeStatusId = 2, StatusName = "Inactive" }
        );

        modelBuilder.Entity<EnrollmentStatus>().HasData(
            new EnrollmentStatus { EnrollmentStatusId = 1, StatusName = "Enrolled" },
            new EnrollmentStatus { EnrollmentStatusId = 2, StatusName = "Confirmed" },
            new EnrollmentStatus { EnrollmentStatusId = 3, StatusName = "Attending" },
            new EnrollmentStatus { EnrollmentStatusId = 4, StatusName = "Completed" },
            new EnrollmentStatus { EnrollmentStatusId = 5, StatusName = "Dropped" }
        );

        modelBuilder.Entity<PaymentStatus>().HasData(
            new PaymentStatus { PaymentStatusId = 1, StatusName = "Pending" },
            new PaymentStatus { PaymentStatusId = 2, StatusName = "Paid" },
            new PaymentStatus { PaymentStatusId = 3, StatusName = "Partially Paid" },
            new PaymentStatus { PaymentStatusId = 4, StatusName = "Unpaid" }
        );

        // ============================================================================
        // 2. SEED MAIN LOOKUP TABLES (Expanded)
        // ============================================================================
        modelBuilder.Entity<CourseCategory>().HasData(
            new CourseCategory { CategoryId = 1, CategoryName = "Programming", Description = "Software and coding courses" },
            new CourseCategory { CategoryId = 2, CategoryName = "Networking", Description = "Network and infrastructure courses" },
            new CourseCategory { CategoryId = 3, CategoryName = "Database", Description = "Database management courses" },
            new CourseCategory { CategoryId = 4, CategoryName = "Cybersecurity", Description = "Information security and ethical penetration testing" },
            new CourseCategory { CategoryId = 5, CategoryName = "Cloud Systems", Description = "Cloud computing architectures and DevOps practices" }
        );

        modelBuilder.Entity<Equipment>().HasData(
            new Equipment { EquipmentId = 1, EquipmentName = "Projector", Description = "Classroom projector" },
            new Equipment { EquipmentId = 2, EquipmentName = "Laptop", Description = "Training laptop" },
            new Equipment { EquipmentId = 3, EquipmentName = "Whiteboard", Description = "Classroom whiteboard" },
            new Equipment { EquipmentId = 4, EquipmentName = "Cisco Router Rack", Description = "Hardware layer physical routing rig" },
            new Equipment { EquipmentId = 5, EquipmentName = "VR Hardware Suite", Description = "Virtual Reality development testing tools" }
        );

        modelBuilder.Entity<Classroom>().HasData(
            new Classroom { ClassroomId = 1, Location = "Room A101", Capacity = 25, IsActive = true },
            new Classroom { ClassroomId = 2, Location = "Room B202", Capacity = 30, IsActive = true },
            new Classroom { ClassroomId = 3, Location = "Lab C305", Capacity = 15, IsActive = true },
            new Classroom { ClassroomId = 4, Location = "Lab D401 (Hardware)", Capacity = 12, IsActive = true },
            new Classroom { ClassroomId = 5, Location = "Auditorium Max", Capacity = 150, IsActive = true }
        );

        modelBuilder.Entity<Certification>().HasData(
            new Certification { CertificationId = 1, Name = "Web Development Certificate", Description = "Certificate for web development skills" },
            new Certification { CertificationId = 2, Name = "Database Fundamentals Certificate", Description = "Certificate for database basics" },
            new Certification { CertificationId = 3, Name = "Enterprise Security Administrator", Description = "Validates advanced threat management profiles" },
            new Certification { CertificationId = 4, Name = "Cloud Infrastructure Architect", Description = "Validates cross-platform enterprise cloud designs" }
        );

        // ============================================================================
        // 3. SEED PEOPLE
        // ============================================================================
        modelBuilder.Entity<Instructor>().HasData(
            new Instructor
            {
                InstructorId = 1,
                FullName = "Ahmed Ali",
                Email = "ahmed.ali@example.com",
                Phone = "+97333112244",
                Password = "Temp123!",
                Qualifications = "MSc Computer Science",
                HireDate = new DateTime(2024, 1, 10)
            },
            new Instructor
            {
                InstructorId = 2,
                FullName = "Sara Hassan",
                Email = "sara.hassan@example.com",
                Phone = "+97333225566",
                Password = "Temp123!",
                Qualifications = "BSc Information Technology",
                HireDate = new DateTime(2024, 2, 15)
            },
            new Instructor
            {
                InstructorId = 3,
                FullName = "Faisal Mahmood",
                Email = "faisal.m@example.com",
                Phone = "+97333998877",
                Password = "Temp123!",
                Qualifications = "PhD Cybersecurity, CCIE Security",
                HireDate = new DateTime(2025, 5, 12)
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
                Password = "Temp123!",
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
                Password = "Temp123!",
                TraineeStatusId = 1
            },
            new Trainee
            {
                TraineeId = 3,
                FullName = "Mariam Al-Sayed",
                OrganizationName = "XYZ Tech",
                RegistrationDate = new DateOnly(2026, 5, 4),
                Email = "mariam@example.com",
                Phone = "+97336665544",
                Password = "Temp123!",
                TraineeStatusId = 1
            }
        );

        // ============================================================================
        // 4. SEED COURSES (Expanded Catalog)
        // ============================================================================
        modelBuilder.Entity<Course>().HasData(
            new Course
            {
                CourseId = 1,
                CourseCode = "WEB101",
                CourseName = "Introduction to Web Development",
                Description = "HTML, CSS, and JavaScript basics",
                DurationHours = 12,
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
                DurationHours = 10,
                Capacity = 25,
                EnrollmentFee = 100,
                CategoryId = 3
            },
            new Course
            {
                CourseId = 3,
                CourseCode = "WEB201",
                CourseName = "Advanced React & API Engineering",
                Description = "State patterns, performance tuning, and robust backend integrations",
                DurationHours = 24,
                Capacity = 15,
                EnrollmentFee = 240,
                CategoryId = 1
            },
            new Course
            {
                CourseId = 4,
                CourseCode = "NET101",
                CourseName = "Routing & Switching Essentials",
                Description = "Managing topology layouts, VLAN segmentation, and base gateway configs",
                DurationHours = 16,
                Capacity = 12,
                EnrollmentFee = 180,
                CategoryId = 2
            },
            new Course
            {
                CourseId = 5,
                CourseCode = "SEC101",
                CourseName = "Introduction to Ethical Hacking",
                Description = "Network footprinting, scanning mechanisms, and fundamental vulnerabilities",
                DurationHours = 20,
                Capacity = 15,
                EnrollmentFee = 300,
                CategoryId = 4
            },
            new Course
            {
                CourseId = 6,
                CourseCode = "CLD101",
                CourseName = "Cloud Practitioner Fundamentals",
                Description = "Core architectural baselines across multi-tenant enterprise cloud instances",
                DurationHours = 14,
                Capacity = 40,
                EnrollmentFee = 200,
                CategoryId = 5
            }
        );

        // ============================================================================
        // 5. SEED JUNCTION TABLES (Expanded Pairs)
        // ============================================================================
        modelBuilder.Entity<CertificationCourse>().HasData(
            // Web Development Certificate requires WEB101 + WEB201
            new CertificationCourse { CourseId = 1, CertificationId = 1, IsRequired = true },
            new CertificationCourse { CourseId = 3, CertificationId = 1, IsRequired = true },
            // Database Fundamentals Certificate requires DB101
            new CertificationCourse { CourseId = 2, CertificationId = 2, IsRequired = true },
            new CertificationCourse { CourseId = 5, CertificationId = 3, IsRequired = true },
            new CertificationCourse { CourseId = 6, CertificationId = 4, IsRequired = true }
        );

        modelBuilder.Entity<ClassroomEquipment>().HasData(
            new ClassroomEquipment { ClassroomId = 1, EquipmentId = 1, Quantity = 1 },
            new ClassroomEquipment { ClassroomId = 1, EquipmentId = 3, Quantity = 1 },
            new ClassroomEquipment { ClassroomId = 2, EquipmentId = 1, Quantity = 1 },
            new ClassroomEquipment { ClassroomId = 2, EquipmentId = 2, Quantity = 10 },
            new ClassroomEquipment { ClassroomId = 3, EquipmentId = 1, Quantity = 1 },
            new ClassroomEquipment { ClassroomId = 3, EquipmentId = 5, Quantity = 5 }, // VR devices inside Lab C305
            new ClassroomEquipment { ClassroomId = 4, EquipmentId = 4, Quantity = 3 }  // Routing structures in Lab D401
        );

        modelBuilder.Entity<CourseReqEquipment>().HasData(
            new CourseReqEquipment { CourseId = 1, EquipmentId = 2, Quantity = 10 },
            new CourseReqEquipment { CourseId = 2, EquipmentId = 2, Quantity = 10 },
            new CourseReqEquipment { CourseId = 3, EquipmentId = 2, Quantity = 15 },
            new CourseReqEquipment { CourseId = 4, EquipmentId = 4, Quantity = 1 },
            new CourseReqEquipment { CourseId = 5, EquipmentId = 2, Quantity = 5 }
        );

        modelBuilder.Entity<InstructorExpertise>().HasData(
            new InstructorExpertise { InstructorId = 1, CategoryId = 1 },
            new InstructorExpertise { InstructorId = 1, CategoryId = 5 }, // Ahmed does Programming + Cloud
            new InstructorExpertise { InstructorId = 2, CategoryId = 3 },
            new InstructorExpertise { InstructorId = 3, CategoryId = 2 }, // Faisal does Networking + Security
            new InstructorExpertise { InstructorId = 3, CategoryId = 4 }
        );

        modelBuilder.Entity<CoursePrerequisite>().HasData(
            new CoursePrerequisite { CourseId = 2, CoursePrerequisiteId = 1 },
            new CoursePrerequisite { CourseId = 3, CoursePrerequisiteId = 1 }, // WEB201 demands WEB101
            new CoursePrerequisite { CourseId = 5, CoursePrerequisiteId = 4 }  // SEC101 demands NET101
        );

        // ============================================================================
        // 6. SEED SESSIONS (MASSIVE TIMELINE MATRIX)
        // ============================================================================
        modelBuilder.Entity<CourseSession>().HasData(
            // Week 1 - June 2026
            new CourseSession
            {
                SessionId = 1, InstructorId = 1, CourseId = 1, ClassroomId = 1,
                StartDateTime = new DateTime(2026, 6, 1, 9, 0, 0), EndDateTime = new DateTime(2026, 6, 1, 12, 0, 0),
                Capacity = 20, CreatedAt = new DateTime(2026, 5, 11, 0, 0, 0)
            },
            new CourseSession
            {
                SessionId = 2, InstructorId = 2, CourseId = 2, ClassroomId = 2,
                StartDateTime = new DateTime(2026, 6, 2, 9, 0, 0), EndDateTime = new DateTime(2026, 6, 2, 12, 0, 0),
                Capacity = 25, CreatedAt = new DateTime(2026, 5, 11, 0, 0, 0)
            },
            new CourseSession
            {
                SessionId = 3, InstructorId = 3, CourseId = 4, ClassroomId = 4,
                StartDateTime = new DateTime(2026, 6, 3, 13, 0, 0), EndDateTime = new DateTime(2026, 6, 3, 17, 0, 0),
                Capacity = 12, CreatedAt = new DateTime(2026, 5, 12, 0, 0, 0)
            },
            new CourseSession
            {
                SessionId = 4, InstructorId = 1, CourseId = 6, ClassroomId = 5,
                StartDateTime = new DateTime(2026, 6, 5, 18, 0, 0), EndDateTime = new DateTime(2026, 6, 5, 21, 0, 0),
                Capacity = 40, CreatedAt = new DateTime(2026, 5, 12, 0, 0, 0)
            },

            // Week 2 - June 2026
            new CourseSession
            {
                SessionId = 5, InstructorId = 1, CourseId = 3, ClassroomId = 1,
                StartDateTime = new DateTime(2026, 6, 8, 9, 0, 0), EndDateTime = new DateTime(2026, 6, 8, 13, 0, 0),
                Capacity = 15, CreatedAt = new DateTime(2026, 5, 14, 0, 0, 0)
            },
            new CourseSession
            {
                SessionId = 6, InstructorId = 2, CourseId = 2, ClassroomId = 2,
                StartDateTime = new DateTime(2026, 6, 9, 14, 0, 0), EndDateTime = new DateTime(2026, 6, 9, 17, 0, 0),
                Capacity = 25, CreatedAt = new DateTime(2026, 5, 14, 0, 0, 0)
            },
            new CourseSession
            {
                SessionId = 7, InstructorId = 3, CourseId = 5, ClassroomId = 3,
                StartDateTime = new DateTime(2026, 6, 11, 09, 0, 0), EndDateTime = new DateTime(2026, 6, 11, 14, 0, 0),
                Capacity = 15, CreatedAt = new DateTime(2026, 5, 15, 0, 0, 0)
            },

            // Week 3 - June 2026
            new CourseSession
            {
                SessionId = 8, InstructorId = 1, CourseId = 1, ClassroomId = 1,
                StartDateTime = new DateTime(2026, 6, 15, 09, 0, 0), EndDateTime = new DateTime(2026, 6, 15, 12, 0, 0),
                Capacity = 20, CreatedAt = new DateTime(2026, 5, 15, 0, 0, 0)
            },
            new CourseSession
            {
                SessionId = 9, InstructorId = 3, CourseId = 4, ClassroomId = 4,
                StartDateTime = new DateTime(2026, 6, 17, 13, 0, 0), EndDateTime = new DateTime(2026, 6, 17, 17, 0, 0),
                Capacity = 12, CreatedAt = new DateTime(2026, 5, 16, 0, 0, 0)
            },

            // Week 4 - June 2026
            new CourseSession
            {
                SessionId = 10, InstructorId = 2, CourseId = 2, ClassroomId = 2,
                StartDateTime = new DateTime(2026, 6, 22, 09, 0, 0), EndDateTime = new DateTime(2026, 6, 22, 12, 0, 0),
                Capacity = 25, CreatedAt = new DateTime(2026, 5, 18, 0, 0, 0)
            },
            new CourseSession
            {
                SessionId = 11, InstructorId = 3, CourseId = 5, ClassroomId = 3,
                StartDateTime = new DateTime(2026, 6, 24, 09, 0, 0), EndDateTime = new DateTime(2026, 6, 24, 14, 0, 0),
                Capacity = 15, CreatedAt = new DateTime(2026, 5, 19, 0, 0, 0)
            },
            new CourseSession
            {
                SessionId = 12, InstructorId = 1, CourseId = 6, ClassroomId = 5,
                StartDateTime = new DateTime(2026, 6, 26, 15, 0, 0), EndDateTime = new DateTime(2026, 6, 26, 18, 0, 0),
                Capacity = 40, CreatedAt = new DateTime(2026, 5, 19, 0, 0, 0)
            }
        );

        // ============================================================================
        // 7. SEED INSTRUCTOR AVAILABILITY (DENSE SCHEDULE MATRIX)
        // ============================================================================
        modelBuilder.Entity<InstructorAvailability>().HasData(
            // Instructor 1 Availability Blocks
            new InstructorAvailability { AvailabilityId = 1, InstructorId = 1, AvailableDate = new DateOnly(2026, 6, 1), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(15, 0), IsAvailable = true },
            new InstructorAvailability { AvailabilityId = 2, InstructorId = 1, AvailableDate = new DateOnly(2026, 6, 5), StartTime = new TimeOnly(16, 0), EndTime = new TimeOnly(22, 0), IsAvailable = true },
            new InstructorAvailability { AvailabilityId = 3, InstructorId = 1, AvailableDate = new DateOnly(2026, 6, 8), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(15, 0), IsAvailable = true },
            new InstructorAvailability { AvailabilityId = 4, InstructorId = 1, AvailableDate = new DateOnly(2026, 6, 15), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(15, 0), IsAvailable = true },
            new InstructorAvailability { AvailabilityId = 5, InstructorId = 1, AvailableDate = new DateOnly(2026, 6, 26), StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(20, 0), IsAvailable = true },

            // Instructor 2 Availability Blocks
            new InstructorAvailability { AvailabilityId = 6, InstructorId = 2, AvailableDate = new DateOnly(2026, 6, 2), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(14, 0), IsAvailable = true },
            new InstructorAvailability { AvailabilityId = 7, InstructorId = 2, AvailableDate = new DateOnly(2026, 6, 9), StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(18, 0), IsAvailable = true },
            new InstructorAvailability { AvailabilityId = 8, InstructorId = 2, AvailableDate = new DateOnly(2026, 6, 22), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(15, 0), IsAvailable = true },

            // Instructor 3 Availability Blocks
            new InstructorAvailability { AvailabilityId = 9, InstructorId = 3, AvailableDate = new DateOnly(2026, 6, 3), StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(18, 0), IsAvailable = true },
            new InstructorAvailability { AvailabilityId = 10, InstructorId = 3, AvailableDate = new DateOnly(2026, 6, 11), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(16, 0), IsAvailable = true },
            new InstructorAvailability { AvailabilityId = 11, InstructorId = 3, AvailableDate = new DateOnly(2026, 6, 17), StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(18, 0), IsAvailable = true },
            new InstructorAvailability { AvailabilityId = 12, InstructorId = 3, AvailableDate = new DateOnly(2026, 6, 24), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(16, 0), IsAvailable = true }
        );

        // ============================================================================
        // 8. SEED TRANSACTIONAL DATA (Enrollment, Payment, Assessment, Progress)
        // ============================================================================
        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment { EnrollmentId = 1, TraineeId = 1, SessionId = 1, EnrollmentDate = new DateOnly(2026, 5, 5), EnrollmentStatusId = 2 },
            new Enrollment { EnrollmentId = 2, TraineeId = 2, SessionId = 2, EnrollmentDate = new DateOnly(2026, 5, 6), EnrollmentStatusId = 1 },
            new Enrollment { EnrollmentId = 3, TraineeId = 3, SessionId = 3, EnrollmentDate = new DateOnly(2026, 5, 7), EnrollmentStatusId = 2 },
            new Enrollment { EnrollmentId = 4, TraineeId = 1, SessionId = 7, EnrollmentDate = new DateOnly(2026, 5, 10), EnrollmentStatusId = 5 } // Dropped scenario
        );

        modelBuilder.Entity<Payment>().HasData(
            new Payment { PaymentId = 1, EnrollmentId = 1, AmountPaid = 120, PaymentDate = new DateOnly(2026, 5, 6), PaymentStatusId = 2, BalanceRemaining = 0 },
            new Payment { PaymentId = 2, EnrollmentId = 2, AmountPaid = 50, PaymentDate = new DateOnly(2026, 5, 7), PaymentStatusId = 3, BalanceRemaining = 50 },
            new Payment { PaymentId = 3, EnrollmentId = 3, AmountPaid = 180, PaymentDate = new DateOnly(2026, 5, 8), PaymentStatusId = 2, BalanceRemaining = 0 },
            new Payment { PaymentId = 4, EnrollmentId = 4, AmountPaid = 0, PaymentDate = new DateOnly(2026, 5, 10), PaymentStatusId = 4, BalanceRemaining = 300 }
        );

        modelBuilder.Entity<Assessment>().HasData(
            new Assessment { AssessmentId = 1, EnrollmentId = 1, InstructorId = 1, Result = 1, Score = 85 },
            new Assessment { AssessmentId = 2, EnrollmentId = 3, InstructorId = 3, Result = 0, Score = 52 } // Failure test case
        );

        modelBuilder.Entity<Notification>().HasData(
            new Notification { NotificationId = 1, Title = "Payment Received", Message = "Your payment has been received.", PaymentId = 1, SessionId = null },
            new Notification { NotificationId = 2, Title = "Session Reminder", Message = "Your course session starts soon.", SessionId = 1, PaymentId = null }
        );
        
    }
}