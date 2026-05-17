using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursePrerequisitesController : ControllerBase
    {
            private readonly CourseManagementDbContext _context;

            public CoursePrerequisitesController(CourseManagementDbContext context)
            {
                _context = context;
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<CoursePrerequisite>>> GetCoursePrerequisites()
            {
                return await _context.CoursePrerequisites.ToListAsync();
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<CoursePrerequisite>> GetCoursePrerequisite(int id)
            {
                var coursePrerequisite = await _context.CoursePrerequisites.FindAsync(id);
                if (coursePrerequisite == null)
                {
                    return NotFound();
                }
                return coursePrerequisite;
            }

            [HttpPost]
            public async Task<ActionResult<CoursePrerequisite>> CreateCoursePrerequisite(CoursePrerequisite coursePrerequisite)
            {
                _context.CoursePrerequisites.Add(coursePrerequisite);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCoursePrerequisite), new { id = coursePrerequisite.CoursePrerequisiteId }, coursePrerequisite);
            }

            [HttpPut("{id}")]
            public async Task<ActionResult<CoursePrerequisite>> UpdateCoursePrerequisite(int id, CoursePrerequisite updatedCoursePrerequisite)
            {
                if (id != updatedCoursePrerequisite.CoursePrerequisiteId) return BadRequest();

                var exists = await _context.CoursePrerequisites.AnyAsync(a => a.CoursePrerequisiteId == id);
                if (!exists) return NotFound();

                _context.Entry(updatedCoursePrerequisite).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteCoursePrerequisite(int id)
            {
                var coursePrerequisite = await _context.CoursePrerequisites.FindAsync(id);
                if (coursePrerequisite == null) return NotFound();

                _context.CoursePrerequisites.Remove(coursePrerequisite);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
}
