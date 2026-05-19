using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentsController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;
        private readonly AssessmentValidationService _assessmentValidator;

        public AssessmentsController(
            CourseManagementDbContext context,
            AssessmentValidationService assessmentValidator)
        {
            _context = context;
            _assessmentValidator = assessmentValidator;
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
        //[Authorize]
        [AllowAnonymous] //only for testing
        [ProducesResponseType(typeof(Assessment), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Assessment>> CreateAssessment(CreateAssessmentDto dto)
        {
            var validationError = await _assessmentValidator.ValidateCreateAsync(dto);

            if (validationError != null)
            {
                return BadRequest(new { message = validationError });
            }

            var assessment = new Assessment
            {
                EnrollmentId = dto.EnrollmentId,
                InstructorId = dto.InstructorId,
                Result = dto.Result,
                Score = dto.Score
            };

            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAssessment), new { id = assessment.AssessmentId }, new
            {
                assessment.AssessmentId,
                assessment.EnrollmentId,
                assessment.InstructorId,
                assessment.Result,
                assessment.Score
            });
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
