using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("CourseSession")]
public partial class CourseSession
{
    [Column("sessionID")]
    public int SessionId { get; set; }

    [Column("instructorID")]
    public int InstructorId { get; set; }

    [Column("courseID")]
    public int CourseId { get; set; }

    [Column("classroomID")]
    public int ClassroomId { get; set; }

    [Column("startDateTime")]
    public DateOnly StartDateTime { get; set; }

    [Column("endDateTime")]
    public DateOnly EndDateTime { get; set; }

    [Column("capacity")]
    public int Capacity { get; set; }

    public DateTime CreatedAt { get; set; }
}
