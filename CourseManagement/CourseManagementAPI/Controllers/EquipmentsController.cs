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
    public class EquipmentsController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public EquipmentsController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipments()
        {
            return await _context.Equipments.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Equipment>> GetEquipment(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }
            return equipment;
        }

        [HttpPost]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        public async Task<ActionResult<Equipment>> CreateEquipment(Equipment equipment)
        {
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEquipment), new { id = equipment.EquipmentId }, equipment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        public async Task<ActionResult<Equipment>> UpdateEquipment(int id, Equipment updatedEquipment)
        {
            if (id != updatedEquipment.EquipmentId) return BadRequest();

            var exists = await _context.Equipments.AnyAsync(a => a.EquipmentId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedEquipment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null) return NotFound();

            _context.Equipments.Remove(equipment);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}