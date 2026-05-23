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
        // Enrollment and PaymentStatus are non-nullable nav properties not posted from the form.
        ModelState.Remove(nameof(Payment.Enrollment));
        ModelState.Remove(nameof(Payment.PaymentStatus));

        if (ModelState.IsValid)
        {
            var result = await CalculatePaymentStatus(payment.EnrollmentId, payment.AmountPaid);

            if (result.Error != null)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                LoadDropdowns(payment.EnrollmentId);
                return View(payment);
            }

            payment.BalanceRemaining = result.BalanceRemaining;
            payment.PaymentStatusId = result.PaymentStatusId;

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
            var result = await CalculatePaymentStatus(
                payment.EnrollmentId, payment.AmountPaid, excludePaymentId: id);

            if (result.Error != null)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                LoadDropdowns(payment.EnrollmentId);
                return View(payment);
            }

            payment.BalanceRemaining = result.BalanceRemaining;
            payment.PaymentStatusId = result.PaymentStatusId;

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

    // ─── Business Rule Helpers ────────────────────────────────────────────────

    private record PaymentCalculationResult(
        decimal BalanceRemaining,
        int PaymentStatusId,
        string? Error);

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

        var prevQuery = _context.Payments.Where(p => p.EnrollmentId == enrollmentId);
        if (excludePaymentId.HasValue)
            prevQuery = prevQuery.Where(p => p.PaymentId != excludePaymentId.Value);

        var previousTotal = await prevQuery.SumAsync(p => (decimal?)p.AmountPaid) ?? 0m;
        var totalPaid = previousTotal + newAmountPaid;
        var balance = totalPaid >= courseFee ? 0m : courseFee - totalPaid;

        var statusName = totalPaid <= 0 ? "Pending"
                       : totalPaid >= courseFee ? "Paid"
                       : "Partial";

        var status = await _context.PaymentStatuses.AsNoTracking()
            .FirstOrDefaultAsync(s => s.StatusName == statusName);

        if (status == null)
            return new PaymentCalculationResult(0, 0,
                $"Payment status '{statusName}' not found. Ensure the database has been seeded.");

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
