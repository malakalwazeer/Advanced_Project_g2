using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Services.Validation
{
    public class CourseValidationService
    {
        //this custom validation checks
        //Does this CategoryId exist?
        //Is this CourseCode already used?
        //Is this CourseName already used?
        
        private readonly CourseManagementDbContext _context;

        public CourseValidationService(CourseManagementDbContext context)
        {
            _context = context;
        }

        public async Task<string?> ValidateCreateAsync(CreateCourseDto dto)
        {
            var categoryExists = await _context.CourseCategories
                .AnyAsync(c => c.CategoryId == dto.CategoryId);

            if (!categoryExists)
            {
                return "Course category does not exist.";
            }

            var courseCodeExists = await _context.Courses
                .AnyAsync(c => c.CourseCode == dto.CourseCode);

            if (courseCodeExists)
            {
                return "Course code already exists.";
            }

            var courseNameExists = await _context.Courses
                .AnyAsync(c => c.CourseName == dto.CourseName);

            if (courseNameExists)
            {
                return "Course name already exists.";
            }

            return null;
        }
    }
}
