using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("InstructorExpertise")]
public partial class InstructorExpertise
{
    [Column("categoryID")]
    public int CategoryId { get; set; }

    [Column("instructorID")]
    public int InstructorId { get; set; }
}
