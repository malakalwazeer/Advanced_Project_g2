using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("TraineeStatus")]
public partial class TraineeStatus
{
    [Column("tStatusID")]
    public int TStatusId { get; set; }

    [Column("statusName")]
    public string StatusName { get; set; } = null!;
}
