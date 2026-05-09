using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models;

[Keyless]
[Table("Trainee")]
public partial class Trainee
{
    [Column("traineeID")]
    public int TraineeId { get; set; }

    [Column("fullName")]
    public string FullName { get; set; } = null!;

    [Column("organizationName")]
    public string? OrganizationName { get; set; }

    [Column("registrationDate")]
    public DateOnly RegistrationDate { get; set; }

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("phone")]
    public int Phone { get; set; }

    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("tStatusID")]
    public int TStatusId { get; set; }
}
