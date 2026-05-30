using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole}")]
    public class ReportsController : ControllerBase
    {
        //This controller will collect report data from the database and send it as JSON to the Reporting app.
        private readonly CourseManagementDbContext _context;

        public ReportsController(CourseManagementDbContext context)
        {
            _context = context;
        }

        //endpoint 1 how many enrollments each course has.
        [HttpGet("enrollment-by-course")]
        public async Task<ActionResult<IEnumerable<EnrollmentByCourseReportDto>>> GetEnrollmentByCourse()
        {
            var report = await _context.Enrollments
                .Include(e => e.Session)
                .ThenInclude(s => s.Course)
                .GroupBy(e => e.Session.Course.CourseName)
                .Select(g => new EnrollmentByCourseReportDto
                {
                    CourseName = g.Key,
                    EnrollmentCount = g.Count()
                })
                .ToListAsync();

            return Ok(report);
        }

        //endpoint 2 how many enrollments each course category has.
        [HttpGet("enrollment-by-category")]
        public async Task<ActionResult<IEnumerable<EnrollmentByCategoryReportDto>>> GetEnrollmentByCategory()
        {
            var report = await _context.Enrollments
                .Include(e => e.Session)
                .ThenInclude(s => s.Course)
                .ThenInclude(c => c.Category)
                .GroupBy(e => e.Session.Course.Category.CategoryName)
                .Select(g => new EnrollmentByCategoryReportDto
                {
                    CategoryName = g.Key,
                    EnrollmentCount = g.Count()
                })
                .ToListAsync();

            return Ok(report);
        }

        //endpoint 3 how many sessions each instructor teaches.
        [HttpGet("instructor-workload")]
        public async Task<ActionResult<IEnumerable<InstructorWorkloadReportDto>>> GetInstructorWorkload()
        {
            var report = await _context.CourseSessions
                .Include(s => s.Instructor)
                .GroupBy(s => s.Instructor.FullName)
                .Select(g => new InstructorWorkloadReportDto
                {
                    InstructorName = g.Key,
                    SessionCount = g.Count()
                })
                .ToListAsync();

            return Ok(report);
        }

        //endpoint 4 shows Total money collected / outstanding
        [HttpGet("revenue-summary")]
        public async Task<ActionResult<RevenueSummaryReportDto>> GetRevenueSummary()
        {
            var totalCollected = await _context.Payments
                .SumAsync(p => p.AmountPaid);

            var totalOutstanding = await _context.Payments
                .SumAsync(p => p.BalanceRemaining ?? 0);

            var report = new RevenueSummaryReportDto
            {
                TotalCollected = totalCollected,
                TotalOutstanding = totalOutstanding
            };

            return Ok(report);
        }

        //endpoint 5 how many trainees completed or are still in progress for each certification.
        [HttpGet("certification-completion")]
        public async Task<ActionResult<IEnumerable<CertificationCompletionReportDto>>> GetCertificationCompletion()
        {
            var report = await _context.TraineeCertificationProgresses
                .Include(p => p.Certification)
                .GroupBy(p => p.Certification.Name)
                .Select(g => new CertificationCompletionReportDto
                {
                    CertificationName = g.Key,
                    CompletedCount = g.Count(p => p.ProgressPercentage >= 100),
                    InProgressCount = g.Count(p => p.ProgressPercentage < 100)
                })
                .ToListAsync();

            return Ok(report);
        }
    }
}

