using System;
using System.Collections.Generic;

namespace CourseManagementAPI.Models;
public partial class Trainee
{
    //
    public int TraineeId { get; set; }
    public string FullName { get; set; } = null!;
    public string? OrganizationName { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int TraineeStatusId { get; set; }


    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<TraineeCertificationProgress> TraineeCertificationProgresses { get; set; } = new List<TraineeCertificationProgress>();

    public virtual TraineeStatus TraineeStatus { get; set; } = null!;
}