using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomsController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public ClassroomsController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Classroom>>> GetClassrooms()
        {
            return await _context.Classrooms.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Classroom>> GetClassroom(int id)
        {
            var Classrooms = await _context.Classrooms.FindAsync(id);
            if (Classrooms == null)
            {
                return NotFound();
            }
            return Classrooms;
        }

        [HttpPost]
        public async Task<ActionResult<Classroom>> CreateClassroom(Classroom classroom)
        {
            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClassrooms), new { id = classroom.ClassroomId }, classroom);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Classroom>> UpdateClassroom(int id, Classroom updatedClassroom)
        {
            if (id != updatedClassroom.ClassroomId) return BadRequest();

            var exists = await _context.Classrooms.AnyAsync(a => a.ClassroomId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedClassroom).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassroom(int id)
        {
            var classroom = await _context.Classrooms.FindAsync(id);
            if (classroom == null) return NotFound();

            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
