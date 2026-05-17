using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorAvailabilitiesController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public InstructorAvailabilitiesController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstructorAvailability>>> GetInstructorAvailabilities()
        {
            return await _context.InstructorAvailabilities.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InstructorAvailability>> GetInstructorAvailability(int id)
        {
            var instructorAvailability = await _context.InstructorAvailabilities.FindAsync(id);
            if (instructorAvailability == null)
            {
                return NotFound();
            }
            return instructorAvailability;
        }

        [HttpPost]
        public async Task<ActionResult<InstructorAvailability>> CreateInstructorAvailability(InstructorAvailability instructorAvailability)
        {
            _context.InstructorAvailabilities.Add(instructorAvailability);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInstructorAvailability), new { id = instructorAvailability.AvailabilityId }, instructorAvailability);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InstructorAvailability>> UpdateInstructorAvailability(int id, InstructorAvailability updatedInstructorAvailability)
        {
            if (id != updatedInstructorAvailability.AvailabilityId) return BadRequest();

            var exists = await _context.InstructorAvailabilities.AnyAsync(a => a.AvailabilityId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedInstructorAvailability).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstructorAvailability(int id)
        {
            var instructorAvailability = await _context.InstructorAvailabilities.FindAsync(id);
            if (instructorAvailability == null) return NotFound();

            _context.InstructorAvailabilities.Remove(instructorAvailability);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
