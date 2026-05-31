namespace CourseManagement.ViewModels;
public class SessionEnrollmentViewModel
{
    public int SessionId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int Capacity { get; set; }
    public int EnrolledCount { get; set; }

    // included in JSON so the JS client doesn't have to recalculate.
    public int RemainingSpots => Math.Max(0, Capacity - EnrolledCount);
    public bool IsFull => RemainingSpots == 0;

    // 0-100, used to drive the Bootstrap progress bar width.
    public int FillPercent => Capacity > 0
        ? (int)Math.Round(EnrolledCount * 100.0 / Capacity)
        : 0;
}
