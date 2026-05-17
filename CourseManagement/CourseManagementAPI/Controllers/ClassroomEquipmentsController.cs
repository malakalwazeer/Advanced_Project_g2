using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomEquipmentsController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public ClassroomEquipmentsController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassroomEquipment>>> GetClassroomEquipments()
        {
            return await _context.ClassroomEquipments.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClassroomEquipment>> GetClassroomEquipment(int id)
        {
            var classroomEquipment = await _context.ClassroomEquipments.FindAsync(id);
            if (classroomEquipment == null)
            {
                return NotFound();
            }
            return classroomEquipment;
        }

        [HttpPost]
        public async Task<ActionResult<ClassroomEquipment>> CreateClassroomEquipment(ClassroomEquipment classroomEquipment)
        {
            _context.ClassroomEquipments.Add(classroomEquipment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClassroomEquipment), new { id = classroomEquipment.ClassroomId }, classroomEquipment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClassroomEquipment>> UpdateClassroomEquipment(int id, ClassroomEquipment updatedClassroomEquipment)
        {
            if (id != updatedClassroomEquipment.ClassroomId) return BadRequest();

            var exists = await _context.ClassroomEquipments.AnyAsync(a => a.ClassroomId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedClassroomEquipment).State = EntityState.Modified;
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

