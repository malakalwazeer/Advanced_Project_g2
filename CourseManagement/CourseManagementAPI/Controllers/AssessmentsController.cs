using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentsController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public AssessmentsController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Assessment>>> GetAssessments()
        {
            return await _context.Assessments.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Assessment>> GetAssessment(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }
            return assessment;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Assessment>> CreateAssessment(Assessment assessment)
        {
            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAssessment), new { id = assessment.AssessmentId }, assessment);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Assessment>> UpdateAssessment(int id, Assessment updatedAssessment)
        {
            if (id != updatedAssessment.AssessmentId) return BadRequest();
            
            var exists = await _context.Assessments.AnyAsync(a => a.AssessmentId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedAssessment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteAssessment(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null) return NotFound();

            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
