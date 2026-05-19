using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Services.Validation
{
    public class AssessmentValidationService
    {
        private readonly CourseManagementDbContext _context;

        public AssessmentValidationService(CourseManagementDbContext context)
        {
            _context = context;
        }

        public async Task<string?> ValidateCreateAsync(CreateAssessmentDto dto)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.EnrollmentId == dto.EnrollmentId);

            if (enrollment == null)
            {
                return "Enrollment does not exist.";
            }

            var instructorExists = await _context.Instructors
                .AnyAsync(i => i.InstructorId == dto.InstructorId);

            if (!instructorExists)
            {
                return "Instructor does not exist.";
            }

            if (enrollment.Session.InstructorId != dto.InstructorId)
            {
                return "This instructor is not assigned to this course session.";
            }

            if (enrollment.Session.EndDateTime > DateTime.Now)
            {
                return "Assessment can only be recorded after the session has ended.";
            }

            var assessmentExists = await _context.Assessments
                .AnyAsync(a => a.EnrollmentId == dto.EnrollmentId);

            if (assessmentExists)
            {
                return "Assessment already exists for this enrollment.";
            }

            return null;
        }
    }
}
