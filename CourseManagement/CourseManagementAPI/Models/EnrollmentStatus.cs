using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("EnrollmentStatus")]
public partial class EnrollmentStatus
{
    [Column("eStatusID")]
    public int EStatusId { get; set; }

    [Column("statusName")]
    public string StatusName { get; set; } = null!;
}
