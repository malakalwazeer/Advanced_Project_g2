using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int TraineeId { get; set; }

    public int SessionId { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public int EnrollmentStatusId { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual EnrollmentStatus EnrollmentStatus { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual CourseSession Session { get; set; } = null!;

    public virtual Trainee Trainee { get; set; } = null!;
}
