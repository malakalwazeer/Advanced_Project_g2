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
    public class PaymentsController : ControllerBase
    {
        

        private readonly CourseManagementDbContext _context;
        private readonly PaymentValidationService _paymentValidator;

        public PaymentsController(
            CourseManagementDbContext context,
            PaymentValidationService paymentValidator)
        {
            _context = context;
            _paymentValidator = paymentValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return await _context.Payments.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return payment;
        }

        [HttpPost]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Payment>> CreatePayment(CreatePaymentDto dto)
        {
            var validationResult = await _paymentValidator.ValidateCreateAsync(dto);

            if (validationResult.ErrorMessage != null)
            {
                return BadRequest(new { message = validationResult.ErrorMessage });
            }

            var payment = new Payment
            {
                EnrollmentId = dto.EnrollmentId,
                AmountPaid = dto.AmountPaid,
                PaymentDate = dto.PaymentDate,
                PaymentStatusId = dto.PaymentStatusId,
                BalanceRemaining = validationResult.BalanceRemaining
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, new
            {
                payment.PaymentId,
                payment.EnrollmentId,
                payment.AmountPaid,
                payment.PaymentDate,
                payment.PaymentStatusId,
                payment.BalanceRemaining
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
        public async Task<ActionResult<Payment>> UpdatePayment(int id, Payment updatedPayment)
        {
            if (id != updatedPayment.PaymentId) return BadRequest();

            var exists = await _context.Payments.AnyAsync(a => a.PaymentId == id);
            if (!exists) return NotFound();

            _context.Entry(updatedPayment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
