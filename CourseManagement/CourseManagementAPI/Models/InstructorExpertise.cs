using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class InstructorExpertise
{
    //
    public int CategoryId { get; set; }

    public int InstructorId { get; set; }

    public virtual Instructor Instructor { get; set; } = null!;

    public virtual CourseCategory Category { get; set; } = null!;
}
