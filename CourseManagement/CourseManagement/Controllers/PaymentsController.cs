using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Dtos;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator,Trainee")]
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

    public IActionResult Create()
    {
        var vm = new PaymentCreateEditViewModel
        {
            PaymentDate = DateOnly.FromDateTime(DateTime.Today)
        };
        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentCreateEditViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var statusResult = await ResolvePaymentStatus(vm.EnrollmentId, vm.AmountPaid);
            if (statusResult.Error != null)
            {
                ModelState.AddModelError(string.Empty, statusResult.Error);
                LoadDropdowns(vm);
                return View(vm);
            }

            var dto = new CreatePaymentDto
            {
                EnrollmentId    = vm.EnrollmentId,
                AmountPaid      = vm.AmountPaid,
                PaymentDate     = vm.PaymentDate,
                PaymentStatusId = statusResult.PaymentStatusId
            };

            var (errorMessage, balanceRemaining) = await _paymentValidator.ValidateCreateAsync(dto);
            if (errorMessage != null)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                LoadDropdowns(vm);
                return View(vm);
            }

            var payment = new Payment
            {
                EnrollmentId     = vm.EnrollmentId,
                AmountPaid       = vm.AmountPaid,
                PaymentDate      = vm.PaymentDate,
                PaymentStatusId  = statusResult.PaymentStatusId,
                BalanceRemaining = balanceRemaining
            };
            _context.Add(payment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Payment recorded successfully.";
            return RedirectToAction(nameof(Index));
        }
        LoadDropdowns(vm);
        return View(vm);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return NotFound();

        var vm = new PaymentCreateEditViewModel
        {
            PaymentId    = payment.PaymentId,
            EnrollmentId = payment.EnrollmentId,
            AmountPaid   = payment.AmountPaid,
            PaymentDate  = payment.PaymentDate
        };
        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PaymentCreateEditViewModel vm)
    {
        if (id != vm.PaymentId) return NotFound();

        if (ModelState.IsValid)
        {
            var statusResult = await ResolvePaymentStatus(vm.EnrollmentId, vm.AmountPaid, excludePaymentId: id);
            if (statusResult.Error != null)
            {
                ModelState.AddModelError(string.Empty, statusResult.Error);
                LoadDropdowns(vm);
                return View(vm);
            }

            var payment = new Payment
            {
                PaymentId        = vm.PaymentId,
                EnrollmentId     = vm.EnrollmentId,
                AmountPaid       = vm.AmountPaid,
                PaymentDate      = vm.PaymentDate,
                PaymentStatusId  = statusResult.PaymentStatusId,
                BalanceRemaining = statusResult.BalanceRemaining
            };
            try
            {
                _context.Update(payment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Payment updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PaymentExists(vm.PaymentId)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        LoadDropdowns(vm);
        return View(vm);
    }

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

    private record StatusResolution(int PaymentStatusId, decimal BalanceRemaining, string? Error);

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

    private void LoadDropdowns(PaymentCreateEditViewModel vm)
    {
        vm.Enrollments = _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .AsNoTracking()
            .OrderBy(e => e.Trainee.FullName)
            .Select(e => new SelectListItem
            {
                Value = e.EnrollmentId.ToString(),
                Text  = e.Trainee.FullName + " — " + e.Session.Course.CourseName
            })
            .ToList();
    }

    private async Task<bool> PaymentExists(int id) =>
        await _context.Payments.AnyAsync(p => p.PaymentId == id);
}
