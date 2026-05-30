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
    public class CourseReqEquipmentsController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public CourseReqEquipmentsController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseReqEquipment>>> GetCourseReqEquipments()
        {
            return await _context.CourseReqEquipments.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseReqEquipment>> GetCourseReqEquipment(int id)
        {
            var courseReqEquipment = await _context.CourseReqEquipments.FindAsync(id);
            if (courseReqEquipment == null)
            {
                return NotFound();
            }
            return courseReqEquipment;
        }

        [HttpPost]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        public async Task<ActionResult<CourseReqEquipment>> CreateCourseReqEquipment(CourseReqEquipment courseReqEquipment)
        {
            _context.CourseReqEquipments.Add(courseReqEquipment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourseReqEquipment), new { id = courseReqEquipment.CourseId }, courseReqEquipment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        public async Task<ActionResult<CourseReqEquipment>> UpdateCourseReqEquipment(int id, CourseReqEquipment updatedCourseReqEquipment)
        {
            if (id != updatedCourseReqEquipment.CourseId) return BadRequest();

            var exists = await _context.CourseReqEquipments.AnyAsync(a => a.CourseId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedCourseReqEquipment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole}")]
        public async Task<IActionResult> DeleteCourseReqEquipment(int id)
        {
            var courseReqEquipment = await _context.CourseReqEquipments.FindAsync(id);
            if (courseReqEquipment == null) return NotFound();

            _context.CourseReqEquipments.Remove(courseReqEquipment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
