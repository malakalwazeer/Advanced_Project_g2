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
    public class EnrollmentsController : ControllerBase
    {
        
        private readonly CourseManagementDbContext _context;
        private readonly EnrollmentValidationService _enrollmentValidator;

        public EnrollmentsController(
            CourseManagementDbContext context,
            EnrollmentValidationService enrollmentValidator)
        {
            _context = context;
            _enrollmentValidator = enrollmentValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollments()
        {
            return await _context.Enrollments.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Enrollment>> GetEnrollment(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return enrollment;
        }

        

        [HttpPost]
        [Authorize]
        //[AllowAnonymous] //only for testing
        [ProducesResponseType(typeof(Enrollment), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Enrollment>> CreateEnrollment(CreateEnrollmentDto dto)
        {
            var validationError = await _enrollmentValidator.ValidateCreateAsync(dto);

            if (validationError != null)
            {
                return BadRequest(new { message = validationError });
            }

            var enrollment = new Enrollment
            {
                TraineeId = dto.TraineeId,
                SessionId = dto.SessionId,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.Today),
                EnrollmentStatusId = 1
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.EnrollmentId }, new
            {
                enrollment.EnrollmentId,
                enrollment.TraineeId,
                enrollment.SessionId,
                enrollment.EnrollmentDate,
                enrollment.EnrollmentStatusId
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Enrollment>> UpdateEnrollment(int id, Enrollment updatedEnrollment)
        {
            if (id != updatedEnrollment.EnrollmentId) return BadRequest();

            var exists = await _context.Enrollments.AnyAsync(a => a.EnrollmentId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedEnrollment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
