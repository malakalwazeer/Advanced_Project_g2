using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("CoursePrerequisite")]
public partial class CoursePrerequisite
{
    [Column("courseID")]
    public int CourseId { get; set; }

    [Column("CoursePrerequisiteID")]
    public int CoursePrerequisiteId { get; set; }
}
