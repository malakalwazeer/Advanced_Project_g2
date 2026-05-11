using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class Instructor
{
    public int InstructorId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Qualifications { get; set; }

    public DateTime HireDate { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    public virtual ICollection<InstructorAvailability> InstructorAvailabilities { get; set; } = new List<InstructorAvailability>();
    //
    public virtual ICollection<InstructorExpertise> InstructorExpertises { get; set; } = new List<InstructorExpertise>();
}
