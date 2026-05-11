using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class Certification
{
    public int CertificationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    //
    public virtual ICollection<CertificationCourse> CertificationCourses { get; set; } = new List<CertificationCourse>();

    public virtual ICollection<TraineeCertificationProgress> TraineeCertificationProgresses { get; set; } = new List<TraineeCertificationProgress>();
}
