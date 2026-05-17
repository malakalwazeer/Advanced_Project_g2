using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentStatusesController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public EnrollmentStatusesController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentStatus>>> GetEnrollmentStatuses()
        {
            return await _context.EnrollmentStatuses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentStatus>> GetEnrollmentStatus(int id)
        {
            var enrollmentStatus = await _context.EnrollmentStatuses.FindAsync(id);
            if (enrollmentStatus == null)
            {
                return NotFound();
            }
            return enrollmentStatus;
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentStatus>> CreateEnrollment(EnrollmentStatus enrollmentStatus)
        {
            _context.EnrollmentStatuses.Add(enrollmentStatus);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEnrollmentStatus), new { id = enrollmentStatus.EnrollmentStatusId }, enrollmentStatus);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EnrollmentStatus>> UpdateEnrollmentStatus(int id, EnrollmentStatus updatedEnrollmentStatus)
        {
            if (id != updatedEnrollmentStatus.EnrollmentStatusId) return BadRequest();

            var exists = await _context.EnrollmentStatuses.AnyAsync(a => a.EnrollmentStatusId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedEnrollmentStatus).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollmentStatus(int id)
        {
            var enrollmentStatus = await _context.EnrollmentStatuses.FindAsync(id);
            if (enrollmentStatus == null) return NotFound();

            _context.EnrollmentStatuses.Remove(enrollmentStatus);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
