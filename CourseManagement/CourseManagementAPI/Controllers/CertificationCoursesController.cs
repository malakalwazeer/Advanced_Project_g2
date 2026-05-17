using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificationCoursesController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public CertificationCoursesController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CertificationCourse>>> GetCertificationCourses()
        {
            return await _context.CertificationCourses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CertificationCourse>> GetCertificationCourse(int id)
        {
            var CertificationCourses = await _context.CertificationCourses.FindAsync(id);
            if (CertificationCourses == null)
            {
                return NotFound();
            }
            return CertificationCourses;
        }

        [HttpPost]
        public async Task<ActionResult<CertificationCourse>> CreateCertificationCourse(CertificationCourse certificationCourse)
        {
            _context.CertificationCourses.Add(certificationCourse);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCertificationCourse), new { id = certificationCourse.CertificationId }, certificationCourse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CertificationCourse>> UpdateCertificationCourse(int id, CertificationCourse updatedCertificationCourse)
        {
            if (id != updatedCertificationCourse.CertificationId) return BadRequest();

            var exists = await _context.CertificationCourses.AnyAsync(a => a.CertificationId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedCertificationCourse).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificationCourse(int id)
        {
            var certificationCourse = await _context.CertificationCourses.FindAsync(id);
            if (certificationCourse == null) return NotFound();

            _context.CertificationCourses.Remove(certificationCourse);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
