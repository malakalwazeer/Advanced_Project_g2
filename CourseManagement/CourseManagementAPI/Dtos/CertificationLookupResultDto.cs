namespace CourseManagementAPI.Dtos
{
    public class CertificationLookupResultDto
    {
        public bool IsValid { get; set; }

        public string Message { get; set; } = null!;

        public int TraineeId { get; set; }

        public string? TraineeName { get; set; }

        public int CertificationId { get; set; }

        public string? CertificationName { get; set; }

        public int RequiredCoursesCount { get; set; }

        public int CompletedCoursesCount { get; set; }

        public decimal ProgressPercentage { get; set; }

        public List<string> CompletedCourses { get; set; } = new();

        public List<string> MissingCourses { get; set; } = new();
    }
}
