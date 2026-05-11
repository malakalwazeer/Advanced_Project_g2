using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseCode { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string? Description { get; set; }

    public int Capacity { get; set; }

    public decimal EnrollmentFee { get; set; }

    public int CategoryId { get; set; }

    public virtual CourseCategory Category { get; set; } = null!;

    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    //
    public virtual ICollection<CourseReqEquipment> CourseReqEquipments { get; set; } = new List<CourseReqEquipment>();

    public virtual ICollection<CertificationCourse> CertificationCourses { get; set; } = new List<CertificationCourse>();

    public virtual ICollection<CoursePrerequisite> CoursePrerequisites { get; set; } = new List<CoursePrerequisite>();

    public virtual ICollection<CoursePrerequisite> RequiredForCourses { get; set; } = new List<CoursePrerequisite>();
}
