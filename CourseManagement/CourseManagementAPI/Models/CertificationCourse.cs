using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("CertificationCourse")]
public partial class CertificationCourse
{
    [Column("courseID")]
    public int CourseId { get; set; }

    [Column("certificationID")]
    public int CertificationId { get; set; }

    [Column("isRequired")]
    public bool IsRequired { get; set; }
}
