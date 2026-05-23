using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

public class PaymentsController : Controller
{
    private readonly CourseManagementDbContext _context;

    public PaymentsController(CourseManagementDbContext context)
    {
        _context = context;
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("EnrollmentId,AmountPaid,PaymentDate")]
        Payment payment)
    {
        if (ModelState.IsValid)
        {
            var calculationResult = await CalculatePaymentStatus(payment.EnrollmentId, payment.AmountPaid);

            if (calculationResult.Error != null)
            {
                ModelState.AddModelError(string.Empty, calculationResult.Error);
                LoadDropdowns(payment.EnrollmentId);
                return View(payment);
            }

            payment.BalanceRemaining = calculationResult.BalanceRemaining;
            payment.PaymentStatusId = calculationResult.PaymentStatusId;

            _context.Add(payment);
            await _context.SaveChangesAsync();
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("PaymentId,EnrollmentId,AmountPaid,PaymentDate")]
        Payment payment)
    {
        if (id != payment.PaymentId) return NotFound();

        if (ModelState.IsValid)
        {
            // Recalculate balance excluding this payment's previous amount
            var calculationResult = await CalculatePaymentStatus(
                payment.EnrollmentId, payment.AmountPaid, excludePaymentId: id);

            if (calculationResult.Error != null)
            {
                ModelState.AddModelError(string.Empty, calculationResult.Error);
                LoadDropdowns(payment.EnrollmentId);
                return View(payment);
            }

            payment.BalanceRemaining = calculationResult.BalanceRemaining;
            payment.PaymentStatusId = calculationResult.PaymentStatusId;

            try
            {
                _context.Update(payment);
                await _context.SaveChangesAsync();
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
        }
        return RedirectToAction(nameof(Index));
    }

    // ─── Business Rule Helpers ────────────────────────────────────────────────

    private record PaymentCalculationResult(
        decimal BalanceRemaining,
        int PaymentStatusId,
        string? Error);

    /// <summary>
    /// Calculates remaining balance and determines payment status
    /// (Pending / Partial / Paid) based on the enrollment's course fee.
    /// </summary>
    private async Task<PaymentCalculationResult> CalculatePaymentStatus(
        int enrollmentId, decimal newAmountPaid, int? excludePaymentId = null)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

        if (enrollment == null)
            return new PaymentCalculationResult(0, 0, "Enrollment not found.");

        var courseFee = enrollment.Session.Course.EnrollmentFee;

        // Sum all previous payments for this enrollment, excluding the one being edited
        var previousPaymentsQuery = _context.Payments
            .Where(p => p.EnrollmentId == enrollmentId);

        if (excludePaymentId.HasValue)
            previousPaymentsQuery = previousPaymentsQuery.Where(p => p.PaymentId != excludePaymentId.Value);

        var previousTotal = await previousPaymentsQuery.SumAsync(p => (decimal?)p.AmountPaid) ?? 0m;

        var totalPaid = previousTotal + newAmountPaid;
        var balance = courseFee - totalPaid;
        if (balance < 0) balance = 0;

        // Resolve payment status by name
        string statusName;
        if (totalPaid <= 0)
            statusName = "Pending";
        else if (totalPaid >= courseFee)
            statusName = "Paid";
        else
            statusName = "Partial";

        var status = await _context.PaymentStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StatusName == statusName);

        if (status == null)
            return new PaymentCalculationResult(0, 0,
                $"Payment status '{statusName}' not found in database.");

        return new PaymentCalculationResult(balance, status.PaymentStatusId, null);
    }

    private void LoadDropdowns(int? selectedEnrollmentId = null, int? selectedStatusId = null)
    {
        ViewData["EnrollmentId"] = new SelectList(
            _context.Enrollments
                .Include(e => e.Trainee)
                .Include(e => e.Session)
                    .ThenInclude(s => s.Course)
                .AsNoTracking()
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
