using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificationsController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public CertificationsController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certification>>> GetCertifications()
        {
            return await _context.Certifications.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Certification>> GetCertification(int id)
        {
            var Certifications = await _context.Certifications.FindAsync(id);
            if (Certifications == null)
            {
                return NotFound();
            }
            return Certifications;
        }

        [HttpPost]
        public async Task<ActionResult<Certification>> CreateCertification(Certification certification)
        {
            _context.Certifications.Add(certification);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCertifications), new { id = certification.CertificationId }, certification);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Certification>> UpdateCertification(int id, Certification updatedCertification)
        {
            if (id != updatedCertification.CertificationId) return BadRequest();

            var exists = await _context.Certifications.AnyAsync(a => a.CertificationId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedCertification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertification(int id)
        {
            var certification = await _context.Certifications.FindAsync(id);
            if (certification == null) return NotFound();

            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
