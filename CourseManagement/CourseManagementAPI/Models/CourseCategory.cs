using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("CourseCategory")]
public partial class CourseCategory
{
    [Column("categoryID")]
    public int CategoryId { get; set; }

    [Column("categoryName")]
    public string CategoryName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }
}
