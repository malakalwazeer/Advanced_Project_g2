using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorExpertisesController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public InstructorExpertisesController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstructorExpertise>>> GetInstructorExpertises()
        {
            return await _context.InstructorExpertises.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InstructorExpertise>> GetInstructorExpertise(int id)
        {
            var instructorExpertise = await _context.InstructorExpertises.FindAsync(id);
            if (instructorExpertise == null)
            {
                return NotFound();
            }
            return instructorExpertise;
        }

        [HttpPost]
        public async Task<ActionResult<InstructorExpertise>> CreateInstructorExpertise(InstructorExpertise instructorExpertise)
        {
            _context.InstructorExpertises.Add(instructorExpertise);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInstructorExpertise), new { id = instructorExpertise.InstructorId }, instructorExpertise);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InstructorExpertise>> UpdateInstructorExpertise(int id, InstructorExpertise updatedInstructorExpertise)
        {
            if (id != updatedInstructorExpertise.InstructorId) return BadRequest();

            var exists = await _context.InstructorExpertises.AnyAsync(a => a.InstructorId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedInstructorExpertise).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstructorExpertise(int id)
        {
            var instructorExpertise = await _context.InstructorExpertises.FindAsync(id);
            if (instructorExpertise == null) return NotFound();

            _context.InstructorExpertises.Remove(instructorExpertise);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
