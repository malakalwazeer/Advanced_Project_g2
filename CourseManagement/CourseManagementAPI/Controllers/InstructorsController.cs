using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public InstructorsController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instructor>>> GetInstructors()
        {
            return await _context.Instructors.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Instructor>> GetInstructor(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            return instructor;
        }

        [HttpPost]
        public async Task<ActionResult<Instructor>> CreateInstructor(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInstructor), new { id = instructor.InstructorId }, instructor);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Instructor>> UpdateInstructor(int id, Instructor updatedInstructor)
        {
            if (id != updatedInstructor.InstructorId) return BadRequest();

            var exists = await _context.Instructors.AnyAsync(a => a.InstructorId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedInstructor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound();

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
