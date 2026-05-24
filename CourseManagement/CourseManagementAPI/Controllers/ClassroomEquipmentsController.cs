using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ClassroomEquipment>>> GetClassroomEquipments()
        {
            return await _context.ClassroomEquipments
                .Include(ce => ce.Classroom)
                .Include(ce => ce.Equipment)
                .ToListAsync();
        }

        [HttpGet("{classroomId}/{equipmentId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ClassroomEquipment>> GetClassroomEquipment(int classroomId, int equipmentId)
        {
            var classroomEquipment = await _context.ClassroomEquipments.FindAsync(classroomId, equipmentId);
            if (classroomEquipment == null)
            {
                return NotFound();
            }
            return classroomEquipment;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ClassroomEquipment>> CreateClassroomEquipment(ClassroomEquipment classroomEquipment)
        {
            _context.ClassroomEquipments.Add(classroomEquipment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClassroomEquipment), 
                new { classroomId = classroomEquipment.ClassroomId, equipmentId = classroomEquipment.EquipmentId }, 
                classroomEquipment);
        }

        [HttpPut("{classroomId}/{equipmentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateClassroomEquipment(int classroomId, int equipmentId, ClassroomEquipment updatedClassroomEquipment)
        {
            if (classroomId != updatedClassroomEquipment.ClassroomId || equipmentId != updatedClassroomEquipment.EquipmentId) return BadRequest();

            var exists = await _context.ClassroomEquipments.AnyAsync(a => a.ClassroomId == classroomId && a.EquipmentId == equipmentId);
            if (!exists) return NotFound();

            _context.Entry(updatedClassroomEquipment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{classroomId}/{equipmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClassroomEquipment(int classroomId, int equipmentId)
        {
            var classroomEquipment = await _context.ClassroomEquipments.FindAsync(classroomId, equipmentId);
            if (classroomEquipment == null) return NotFound();

            _context.ClassroomEquipments.Remove(classroomEquipment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

