using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class InstructorAvailability
{
    //
    public int AvailabilityId { get; set; }

    public DateOnly AvailableDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsAvailable { get; set; }

    public int InstructorId { get; set; }

    public virtual Instructor Instructor { get; set; } = null!;
}
