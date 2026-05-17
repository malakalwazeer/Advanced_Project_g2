using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraineeStatusesController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public TraineeStatusesController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TraineeStatus>>> GetTraineeStatuses()
        {
            return await _context.TraineeStatuses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TraineeStatus>> GetTraineeStatus(int id)
        {
            var traineeStatus = await _context.TraineeStatuses.FindAsync(id);
            if (traineeStatus == null)
            {
                return NotFound();
            }
            return traineeStatus;
        }

        [HttpPost]
        public async Task<ActionResult<TraineeStatus>> CreateTraineeStatus(TraineeStatus traineeStatus)
        {
            _context.TraineeStatuses.Add(traineeStatus);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTraineeStatus), new { id = traineeStatus.TraineeStatusId }, traineeStatus);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TraineeStatus>> UpdateTraineeStatus(int id, TraineeStatus updatedTraineeStatus)
        {
            if (id != updatedTraineeStatus.TraineeStatusId) return BadRequest();

            var exists = await _context.TraineeStatuses.AnyAsync(a => a.TraineeStatusId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedTraineeStatus).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTraineeStatus(int id)
        {
            var traineeStatus = await _context.TraineeStatuses.FindAsync(id);
            if (traineeStatus == null) return NotFound();

            _context.TraineeStatuses.Remove(traineeStatus);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
