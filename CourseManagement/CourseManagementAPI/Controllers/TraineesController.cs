using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraineesController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public TraineesController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trainee>>> GetTrainees()
        {
            return await _context.Trainees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Trainee>> GetTrainee(int id)
        {
            var trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null)
            {
                return NotFound();
            }
            return trainee;
        }

        [HttpPost]
        public async Task<ActionResult<Trainee>> CreateTrainee(Trainee trainee)
        {
            _context.Trainees.Add(trainee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTrainee), new { id = trainee.TraineeId }, trainee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Trainee>> UpdateTrainee(int id, Trainee updatedTrainee)
        {
            if (id != updatedTrainee.TraineeId) return BadRequest();

            var exists = await _context.Trainees.AnyAsync(a => a.TraineeId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedTrainee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainee(int id)
        {
            var trainee = await _context.Trainees.FindAsync(id);
            if (trainee == null) return NotFound();

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
