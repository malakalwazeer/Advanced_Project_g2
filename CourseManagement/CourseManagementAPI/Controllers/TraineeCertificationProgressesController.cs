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
    public class TraineeCertificationProgressesController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public TraineeCertificationProgressesController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TraineeCertificationProgress>>> GetTraineeCertificationProgresses()
        {
            return await _context.TraineeCertificationProgresses.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<TraineeCertificationProgress>> GetTraineeCertificationProgress(int id)
        {
            var traineeCertificationProgress = await _context.TraineeCertificationProgresses.FindAsync(id);
            if (traineeCertificationProgress == null)
            {
                return NotFound();
            }
            return traineeCertificationProgress;
        }

        [HttpPost]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        public async Task<ActionResult<TraineeCertificationProgress>> CreateTraineeCertificationProgress(TraineeCertificationProgress traineeCertificationProgress)
        {
            _context.TraineeCertificationProgresses.Add(traineeCertificationProgress);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTraineeCertificationProgress), new { id = traineeCertificationProgress.CertificationId }, traineeCertificationProgress);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        public async Task<ActionResult<TraineeCertificationProgress>> UpdateTraineeCertificationProgress(int id, TraineeCertificationProgress updatedTraineeCertificationProgress)
        {
            if (id != updatedTraineeCertificationProgress.CertificationId) return BadRequest();

            var exists = await _context.TraineeCertificationProgresses.AnyAsync(a => a.CertificationId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedTraineeCertificationProgress).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole}")]
        public async Task<IActionResult> DeleteTraineeCertificationProgress(int id)
        {
            var traineeCertificationProgress = await _context.TraineeCertificationProgresses.FindAsync(id);
            if (traineeCertificationProgress == null) return NotFound();

            _context.TraineeCertificationProgresses.Remove(traineeCertificationProgress);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
