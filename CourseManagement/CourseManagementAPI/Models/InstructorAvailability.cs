using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("InstructorAvailability")]
public partial class InstructorAvailability
{
    [Column("availabilityID")]
    public int AvailabilityId { get; set; }

    [Column("availabileDate")]
    public DateOnly? AvailabileDate { get; set; }

    [Column("startTime")]
    public TimeOnly? StartTime { get; set; }

    [Column("endTime")]
    public TimeOnly? EndTime { get; set; }

    [Column("isAvailable")]
    public bool? IsAvailable { get; set; }

    [Column("instructorID")]
    public int? InstructorId { get; set; }
}
