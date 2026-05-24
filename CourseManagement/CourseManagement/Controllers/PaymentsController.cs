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

        var vm = payments.Select(p => new PaymentIndexViewModel
        {
            PaymentId        = p.PaymentId,
            TraineeName      = p.Enrollment?.Trainee?.FullName,
            CourseName       = p.Enrollment?.Session?.Course?.CourseName,
            AmountPaid       = p.AmountPaid,
            PaymentDate      = p.PaymentDate,
            BalanceRemaining = p.BalanceRemaining,
            StatusName       = p.PaymentStatus?.StatusName
        }).ToList();

        return View(vm);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var p = await _context.Payments
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(p => p.PaymentStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (p == null) return NotFound();

        var vm = new PaymentDetailsViewModel
        {
            PaymentId        = p.PaymentId,
            TraineeName      = p.Enrollment?.Trainee?.FullName,
            CourseName       = p.Enrollment?.Session?.Course?.CourseName,
            AmountPaid       = p.AmountPaid,
            PaymentDate      = p.PaymentDate,
            BalanceRemaining = p.BalanceRemaining,
            StatusName       = p.PaymentStatus?.StatusName
        };

        return View(vm);
    }

    public IActionResult Create()
    {
        var vm = new PaymentCreateViewModel
        {
            PaymentDate = DateOnly.FromDateTime(DateTime.Today)
        };
        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentCreateViewModel vm)
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

        var payment = await _context.Payments.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentId == id);
        if (payment == null) return NotFound();

        var vm = new PaymentEditViewModel
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
    public async Task<IActionResult> Edit(int id, PaymentEditViewModel vm)
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

        var p = await _context.Payments
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Trainee)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(p => p.PaymentStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (p == null) return NotFound();

        var vm = new PaymentDeleteViewModel
        {
            PaymentId        = p.PaymentId,
            TraineeName      = p.Enrollment?.Trainee?.FullName,
            CourseName       = p.Enrollment?.Session?.Course?.CourseName,
            AmountPaid       = p.AmountPaid,
            PaymentDate      = p.PaymentDate,
            BalanceRemaining = p.BalanceRemaining,
            StatusName       = p.PaymentStatus?.StatusName
        };

        return View(vm);
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

    // ─── Helpers ─────────────────────────────────────────────────────────────

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

    private void LoadDropdowns(PaymentCreateViewModel vm)
    {
        vm.Enrollments = _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .AsNoTracking()
            .OrderBy(e => e.Trainee.FullName)
            .AsEnumerable()
            .Select(e => new SelectListItem
            {
                Value = e.EnrollmentId.ToString(),
                Text  = e.Trainee.FullName + " — " + e.Session.Course.CourseName
            }).ToList();
    }

    private void LoadDropdowns(PaymentEditViewModel vm)
    {
        vm.Enrollments = _context.Enrollments
            .Include(e => e.Trainee)
            .Include(e => e.Session)
                .ThenInclude(s => s.Course)
            .AsNoTracking()
            .OrderBy(e => e.Trainee.FullName)
            .AsEnumerable()
            .Select(e => new SelectListItem
            {
                Value = e.EnrollmentId.ToString(),
                Text  = e.Trainee.FullName + " — " + e.Session.Course.CourseName
            }).ToList();
    }

    private async Task<bool> PaymentExists(int id) =>
        await _context.Payments.AnyAsync(p => p.PaymentId == id);
}
