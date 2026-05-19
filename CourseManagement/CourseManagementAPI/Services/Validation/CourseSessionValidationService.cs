using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Services.Validation
{
    public class CourseSessionValidationService
    {
        private readonly CourseManagementDbContext _context;

        public CourseSessionValidationService(CourseManagementDbContext context)
        {
            _context = context;
        }

        public async Task<string?> ValidateCreateAsync(CreateCourseSessionDto dto)
        {
            if (dto.EndDateTime <= dto.StartDateTime)
            {
                return "End date/time must be after start date/time.";
            }

            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == dto.CourseId);

            if (!courseExists)
            {
                return "Course does not exist.";
            }

            var instructorExists = await _context.Instructors.AnyAsync(i => i.InstructorId == dto.InstructorId);

            if (!instructorExists)
            {
                return "Instructor does not exist.";
            }

            var classroom = await _context.Classrooms
                .FirstOrDefaultAsync(c => c.ClassroomId == dto.ClassroomId);

            if (classroom == null)
            {
                return "Classroom does not exist.";
            }

            if (!classroom.IsActive)
            {
                return "Classroom is not active.";
            }

            if (dto.Capacity > classroom.Capacity)
            {
                return "Session capacity cannot be greater than classroom capacity.";
            }

            var instructorConflict = await _context.CourseSessions.AnyAsync(s =>
                s.InstructorId == dto.InstructorId &&
                s.StartDateTime < dto.EndDateTime &&
                dto.StartDateTime < s.EndDateTime
            );

            if (instructorConflict)
            {
                return "Conflict: Instructor is already booked during this time.";
            }

            var classroomConflict = await _context.CourseSessions.AnyAsync(s =>
                s.ClassroomId == dto.ClassroomId &&
                s.StartDateTime < dto.EndDateTime &&
                dto.StartDateTime < s.EndDateTime
            );

            if (classroomConflict)
            {
                return "Conflict: Double Classroom Booking! this classroom is already booked during this time.";
            }

            return null;
        }
    }
}
