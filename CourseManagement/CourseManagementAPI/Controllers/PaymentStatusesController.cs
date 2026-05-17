using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentStatusesController : ControllerBase
    {
        private readonly CourseManagementDbContext _context;

        public PaymentStatusesController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentStatus>>> GetPaymentStatuses()
        {
            return await _context.PaymentStatuses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentStatus>> GetPaymentStatus(int id)
        {
            var paymentStatus = await _context.PaymentStatuses.FindAsync(id);
            if (paymentStatus == null)
            {
                return NotFound();
            }
            return paymentStatus;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentStatus>> CreatePaymentStatus(PaymentStatus paymentStatus)
        {
            _context.PaymentStatuses.Add(paymentStatus);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPaymentStatus), new { id = paymentStatus.PaymentStatusId }, paymentStatus);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentStatus>> UpdatePaymentStatus(int id, PaymentStatus updatedPaymentStatus)
        {
            if (id != updatedPaymentStatus.PaymentStatusId) return BadRequest();

            var exists = await _context.PaymentStatuses.AnyAsync(a => a.PaymentStatusId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedPaymentStatus).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentStatus(int id)
        {
            var paymentStatus = await _context.PaymentStatuses.FindAsync(id);
            if (paymentStatus == null) return NotFound();

            _context.PaymentStatuses.Remove(paymentStatus);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
