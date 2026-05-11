using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;

public partial class CertificationCourse
{
    //
    public int CourseId { get; set; }

    public int CertificationId { get; set; }

    public bool IsRequired { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Certification Certification { get; set; } = null!;
}
