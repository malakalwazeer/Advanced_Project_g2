using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Services.Validation
{
    public class PaymentValidationService
    {
        private readonly CourseManagementDbContext _context;

        public PaymentValidationService(CourseManagementDbContext context)
        {
            _context = context;
        }

        public async Task<(string? ErrorMessage, decimal BalanceRemaining)> ValidateCreateAsync(CreatePaymentDto dto)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Session)
                .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == dto.EnrollmentId);

            if (enrollment == null)
            {
                return ("Enrollment does not exist.", 0);
            }

            var paymentStatusExists = await _context.PaymentStatuses
                .AnyAsync(ps => ps.PaymentStatusId == dto.PaymentStatusId);

            if (!paymentStatusExists)
            {
                return ("Payment status does not exist.", 0);
            }

            if (dto.PaymentDate < enrollment.EnrollmentDate)
            {
                return ("Payment date cannot be before enrollment date.", 0);
            }

            var courseFee = enrollment.Session.Course.EnrollmentFee;

            var totalPaidBefore = await _context.Payments
                .Where(p => p.EnrollmentId == dto.EnrollmentId)
                .SumAsync(p => p.AmountPaid);

            var remainingBeforePayment = courseFee - totalPaidBefore;

            if (remainingBeforePayment <= 0)
            {
                return ("This enrollment is already fully paid.", 0);
            }

            if (dto.AmountPaid > remainingBeforePayment)
            {
                return ($"Payment amount exceeds remaining balance. Remaining balance is {remainingBeforePayment}.", remainingBeforePayment);
            }

            var balanceRemaining = remainingBeforePayment - dto.AmountPaid;

            return (null, balanceRemaining);
        }
    }
}
