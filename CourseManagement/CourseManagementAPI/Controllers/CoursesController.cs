using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;
        private readonly CourseValidationService _courseValidator;

        public CoursesController(
            CourseManagementDbContext context,
            CourseValidationService courseValidator)
        {
            _context = context;
            _courseValidator = courseValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return course;
        }

        [HttpPost]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Course>> CreateCourse(CreateCourseDto dto)
        {
            var validationError = await _courseValidator.ValidateCreateAsync(dto);

            if (validationError != null)
            {
                return BadRequest(new { message = validationError });
            }

            var course = new Course
            {
                CourseCode = dto.CourseCode,
                CourseName = dto.CourseName,
                Description = dto.Description,
                DurationHours = dto.DurationHours,
                Capacity = dto.Capacity,
                EnrollmentFee = dto.EnrollmentFee,
                CategoryId = dto.CategoryId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, new
            {
                course.CourseId,
                course.CourseCode,
                course.CourseName,
                course.Description,
                course.DurationHours,
                course.Capacity,
                course.EnrollmentFee,
                course.CategoryId
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        public async Task<ActionResult<Course>> UpdateCourse(int id, Course updatedCourse)
        {
            if (id != updatedCourse.CourseId) return BadRequest();

            var exists = await _context.Courses.AnyAsync(a => a.CourseId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedCourse).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
