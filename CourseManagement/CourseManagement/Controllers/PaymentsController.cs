using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

public class PaymentsController : Controller
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

    // GET: Payments
    public async Task<IActionResult> Index()
    {
        var payments = await _context.Payments
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(p => p.PaymentStatus)
            .AsNoTracking()
            .ToListAsync();

        return View(payments);
    }

    // GET: Payments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var payment = await _context.Payments
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(p => p.PaymentStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null) return NotFound();

        return View(payment);
    }

    // GET: Payments/Create
    public IActionResult Create()
    {
        LoadDropdowns();
        return View();
    }

    // POST: Payments/Create
    // Flow:
    //  1. Resolve the correct PaymentStatus (Paid/Partial/Pending) from totals.
    //  2. Map to CreatePaymentDto and delegate to PaymentValidationService.
    //  3. The service returns (ErrorMessage, BalanceRemaining) — use its balance if valid.
    //  4. Save via EF Core directly.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("EnrollmentId,AmountPaid,PaymentDate")]
        Payment payment)
    {
        ModelState.Remove(nameof(Payment.Enrollment));
        ModelState.Remove(nameof(Payment.PaymentStatus));

        if (ModelState.IsValid)
        {
            // Step 1: determine which PaymentStatus applies after this payment.
            var statusResult = await ResolvePaymentStatus(payment.EnrollmentId, payment.AmountPaid);
            if (statusResult.Error != null)
            {
                ModelState.AddModelError(string.Empty, statusResult.Error);
                LoadDropdowns(payment.EnrollmentId);
                return View(payment);
            }

            // Step 2: build the DTO the service expects and run validation.
            var dto = new CreatePaymentDto
            {
                EnrollmentId    = payment.EnrollmentId,
                AmountPaid      = payment.AmountPaid,
                PaymentDate     = payment.PaymentDate,
                PaymentStatusId = statusResult.PaymentStatusId
            };

            var (errorMessage, balanceRemaining) = await _paymentValidator.ValidateCreateAsync(dto);
            if (errorMessage != null)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                LoadDropdowns(payment.EnrollmentId);
                return View(payment);
            }

            // Step 3: apply computed values and save.
            payment.PaymentStatusId  = statusResult.PaymentStatusId;
            payment.BalanceRemaining = balanceRemaining;

            _context.Add(payment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Payment recorded successfully.";
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(payment.EnrollmentId);
        return View(payment);
    }

    // GET: Payments/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return NotFound();

        LoadDropdowns(payment.EnrollmentId, payment.PaymentStatusId);
        return View(payment);
    }

    // POST: Payments/Edit/5
    // Edit does NOT use PaymentValidationService — the service's "enrollment already fully paid"
    // check would false-positive because it counts the existing payment being edited.
    // Custom ResolvePaymentStatus (with excludePaymentId) handles balance recalculation instead.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("PaymentId,EnrollmentId,AmountPaid,PaymentDate")]
        Payment payment)
    {
        if (id != payment.PaymentId) return NotFound();

        ModelState.Remove(nameof(Payment.Enrollment));
        ModelState.Remove(nameof(Payment.PaymentStatus));

        if (ModelState.IsValid)
        {
            var statusResult = await ResolvePaymentStatus(
                payment.EnrollmentId, payment.AmountPaid, excludePaymentId: id);

            if (statusResult.Error != null)
            {
                ModelState.AddModelError(string.Empty, statusResult.Error);
                LoadDropdowns(payment.EnrollmentId);
                return View(payment);
            }

            payment.PaymentStatusId  = statusResult.PaymentStatusId;
            payment.BalanceRemaining = statusResult.BalanceRemaining;

            try
            {
                _context.Update(payment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Payment updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PaymentExists(payment.PaymentId))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(payment.EnrollmentId, payment.PaymentStatusId);
        return View(payment);
    }

    // GET: Payments/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var payment = await _context.Payments
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(p => p.PaymentStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null) return NotFound();

        return View(payment);
    }

    // POST: Payments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Payment deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private record StatusResolution(
        int PaymentStatusId,
        decimal BalanceRemaining,
        string? Error);

    /// <summary>
    /// Determines Paid / Partial / Pending based on previous payments + the new amount,
    /// resolves the matching PaymentStatus row, and returns the remaining balance.
    /// Used for both Create (no excludePaymentId) and Edit (pass excludePaymentId to
    /// exclude the record being updated from the previous-total sum).
    /// </summary>
    private async Task<StatusResolution> ResolvePaymentStatus(
        int enrollmentId, decimal newAmountPaid, int? excludePaymentId = null)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null)
            return new StatusResolution(0, 0, "Enrollment not found.");

        var courseFee = enrollment.Session.Course.EnrollmentFee;

        var prevQuery = _context.Payments.Where(p => p.EnrollmentId == enrollmentId);
        if (excludePaymentId.HasValue)
            prevQuery = prevQuery.Where(p => p.PaymentId != excludePaymentId.Value);

        var previousTotal = await prevQuery.SumAsync(p => (decimal?)p.AmountPaid) ?? 0m;
        var totalPaid     = previousTotal + newAmountPaid;
        var balance       = totalPaid >= courseFee ? 0m : courseFee - totalPaid;

        var statusName = totalPaid <= 0         ? "Pending"
                       : totalPaid >= courseFee ? "Paid"
                                                : "Partially Paid";

        var status = await _context.PaymentStatuses.AsNoTracking()
            .FirstOrDefaultAsync(s => s.StatusName == statusName);

        if (status == null)
            return new StatusResolution(0, 0,
                $"Payment status '{statusName}' not found. Ensure the database is seeded.");

        return new StatusResolution(status.PaymentStatusId, balance, null);
    }

    private void LoadDropdowns(int? selectedEnrollmentId = null, int? selectedStatusId = null)
    {
        ViewData["EnrollmentId"] = new SelectList(
            _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .AsNoTracking()
                .OrderBy(e => e.Trainee.FullName)
                .Select(e => new
                {
                    e.EnrollmentId,
                    Display = e.Trainee.FullName + " — " + e.Session.Course.CourseName
                }),
            "EnrollmentId", "Display", selectedEnrollmentId);

        ViewData["PaymentStatusId"] = new SelectList(
            _context.PaymentStatuses.AsNoTracking(),
            "PaymentStatusId", "StatusName", selectedStatusId);
    }

    private async Task<bool> PaymentExists(int id) =>
        await _context.Payments.AnyAsync(p => p.PaymentId == id);
}
