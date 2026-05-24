namespace CourseManagement.ViewModels;

public class EnrollmentDetailsViewModel
{
    public int EnrollmentId { get; set; }
    public string? TraineeName { get; set; }
    public string? CourseName { get; set; }
    public string? InstructorName { get; set; }
    public DateTime? SessionStart { get; set; }
    public DateTime? SessionEnd { get; set; }
    public DateOnly EnrollmentDate { get; set; }
    public string? StatusName { get; set; }

    public List<EnrollmentPaymentRow> Payments { get; set; } = new();
    public List<EnrollmentAssessmentRow> Assessments { get; set; } = new();
}

public class EnrollmentPaymentRow
{
    public decimal AmountPaid { get; set; }
    public DateOnly PaymentDate { get; set; }
    public decimal? BalanceRemaining { get; set; }
    public string? StatusName { get; set; }
}

public class EnrollmentAssessmentRow
{
    public string? InstructorName { get; set; }
    public int? Score { get; set; }
    public int? Result { get; set; }
}
