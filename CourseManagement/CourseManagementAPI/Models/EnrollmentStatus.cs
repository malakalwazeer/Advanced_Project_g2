using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class EnrollmentStatus
{
    public int EnrollmentStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
