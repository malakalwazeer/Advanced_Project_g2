using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Services.Validation;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseSessionsController : ControllerBase
    {
        

        private readonly CourseManagementDbContext _context;
        private readonly CourseSessionValidationService _sessionValidator;

        public CourseSessionsController(
            CourseManagementDbContext context,
            CourseSessionValidationService sessionValidator)
        {
            _context = context;
            _sessionValidator = sessionValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseSession>>> GetCourseSessions()
        {
            return await _context.CourseSessions.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseSession>> GetCourseSession(int id)
        {
            var courseSession = await _context.CourseSessions.FindAsync(id);
            if (courseSession == null)
            {
                return NotFound();
            }
            return courseSession;
        }

        

        [HttpPost]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        [ProducesResponseType(typeof(CourseSession), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CourseSession>> CreateCourseSession(CreateCourseSessionDto dto)
        {
            var validationError = await _sessionValidator.ValidateCreateAsync(dto);

            if (validationError != null)
            {
                return BadRequest(new { message = validationError });
            }

            var courseSession = new CourseSession
            {
                InstructorId = dto.InstructorId,
                CourseId = dto.CourseId,
                ClassroomId = dto.ClassroomId,
                StartDateTime = dto.StartDateTime,
                EndDateTime = dto.EndDateTime,
                Capacity = dto.Capacity,
                CreatedAt = DateTime.Now
            };

            _context.CourseSessions.Add(courseSession);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourseSession), new { id = courseSession.SessionId }, new
            {
                courseSession.SessionId,
                courseSession.InstructorId,
                courseSession.CourseId,
                courseSession.ClassroomId,
                courseSession.StartDateTime,
                courseSession.EndDateTime,
                courseSession.Capacity,
                courseSession.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        public async Task<ActionResult<CourseSession>> UpdateCourseSession(int id, CourseSession updatedCourseSession)
        {
            if (id != updatedCourseSession.SessionId) return BadRequest();

            var exists = await _context.CourseSessions.AnyAsync(a => a.SessionId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedCourseSession).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole}")]
        public async Task<IActionResult> DeleteCourseSession(int id)
        {
            var courseSession = await _context.CourseSessions.FindAsync(id);
            if (courseSession == null) return NotFound();

            _context.CourseSessions.Remove(courseSession);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
