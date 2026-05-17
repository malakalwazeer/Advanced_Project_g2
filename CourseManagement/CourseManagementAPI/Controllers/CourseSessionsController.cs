using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseSessionsController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public CourseSessionsController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseSession>>> GetCourseSessions()
        {
            return await _context.CourseSessions.ToListAsync();
        }

        [HttpGet("{id}")]
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
        public async Task<ActionResult<CourseSession>> CreateCourseSession(CourseSession courseSession)
        {
            _context.CourseSessions.Add(courseSession);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourseSession), new { id = courseSession.SessionId }, courseSession);
        }

        [HttpPut("{id}")]
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
