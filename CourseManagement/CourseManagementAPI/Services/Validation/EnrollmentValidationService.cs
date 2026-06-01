using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Services.Validation
{
    public class EnrollmentValidationService
    {
        //Added by Mariam 
        // Statuses that occupy a seat. Completed and Dropped free the seat.
        // Used for both duplicate-enrollment checks and capacity enforcement.
        private static readonly string[] ActiveStatuses =
            ["Enrolled", "Confirmed", "Attending"];

        private readonly CourseManagementDbContext _context;

        public EnrollmentValidationService(CourseManagementDbContext context)
        {
            _context = context;
        }

        public async Task<string?> ValidateCreateAsync(CreateEnrollmentDto dto)
        {
            var trainee = await _context.Trainees
                .Include(t => t.TraineeStatus)
                .FirstOrDefaultAsync(t => t.TraineeId == dto.TraineeId);

            if (trainee == null)
            {
                return "Trainee does not exist.";
            }

            if (trainee.TraineeStatus.StatusName != "Active")
            {
                return "Only active trainees can enroll in courses.";
            }

            var session = await _context.CourseSessions
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.SessionId == dto.SessionId);

            if (session == null)
            {
                return "Course session does not exist.";
            }

            if (session.StartDateTime <= DateTime.Now)
            {
                return "Cannot enroll in a session that has already started or finished.";
            }

            //Added by Mariam  
            // Duplicate check: trainee already has an ACTIVE enrollment for this session.
            // Dropped and Completed enrollments are ignored
            var alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
                e.TraineeId == dto.TraineeId &&
                e.SessionId == dto.SessionId &&
                ActiveStatuses.Contains(e.EnrollmentStatus.StatusName));

            if (alreadyEnrolled)
            {
                return "Trainee is already enrolled in this session.";
            }

            //Added by Mariam  
            // Capacity check: count only active-status enrollments.
            // Completed and Dropped records do NOT consume a seat.
            var activeCount = await _context.Enrollments
                .CountAsync(e =>
                    e.SessionId == dto.SessionId &&
                    ActiveStatuses.Contains(e.EnrollmentStatus.StatusName));

            if (activeCount >= session.Capacity)
            {
                return "This course session is full. No available seats.";
            }

            var prerequisiteIds = await _context.CoursePrerequisites
                .Where(p => p.CourseId == session.CourseId)
                .Select(p => p.CoursePrerequisiteId)
                .ToListAsync();

            if (prerequisiteIds.Any())
            {
                var passedCourseIds = await _context.Enrollments
                    .Where(e => e.TraineeId == dto.TraineeId)
                    .Where(e => e.Assessments.Any(a => a.Result == 1))
                    .Select(e => e.Session.CourseId)
                    .Distinct()
                    .ToListAsync();

                var missingPrerequisites = prerequisiteIds
                    .Where(prereqId => !passedCourseIds.Contains(prereqId))
                    .ToList();

                if (missingPrerequisites.Any())
                {
                    return "Trainee has not completed the required prerequisite course.";
                }
            }

            return null;
        }
    }
}
