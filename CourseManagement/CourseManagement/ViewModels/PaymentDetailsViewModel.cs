namespace CourseManagement.ViewModels;

public class PaymentDetailsViewModel
{
    public int PaymentId { get; set; }
    public string? TraineeName { get; set; }
    public string? CourseName { get; set; }
    public decimal AmountPaid { get; set; }
    public DateOnly PaymentDate { get; set; }
    public decimal? BalanceRemaining { get; set; }
    public string? StatusName { get; set; }
}
