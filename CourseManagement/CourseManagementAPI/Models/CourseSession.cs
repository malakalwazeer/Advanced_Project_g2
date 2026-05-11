using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class CourseSession
{
    public int SessionId { get; set; }

    public int InstructorId { get; set; }

    public int CourseId { get; set; }

    public int ClassroomId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int Capacity { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Classroom Classroom { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Instructor Instructor { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
