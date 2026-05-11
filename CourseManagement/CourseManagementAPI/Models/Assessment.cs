using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class Assessment
{
    public int AssessmentId { get; set; }

    public int EnrollmentId { get; set; }

    public int InstructorId { get; set; }

    public int? Result { get; set; }

    public int? Score { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;

    public virtual Instructor Instructor { get; set; } = null!;
}
