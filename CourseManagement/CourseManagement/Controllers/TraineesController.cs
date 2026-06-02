using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator,Trainee,Instructor")]
public class TraineesController : Controller
{
    private readonly CourseManagementDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TraineesController(
        CourseManagementDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Trainee"))
            return Forbid();

        IQueryable<Trainee> query = _context.Trainees
            .Include(t => t.TraineeStatus)
            .AsNoTracking();

        if (User.IsInRole("Instructor"))
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);
            if (instructor == null) return Forbid();

            var traineeIds = await _context.Enrollments
                .Include(e => e.Session)
                .Where(e => e.Session.InstructorId == instructor.InstructorId)
                .Select(e => e.TraineeId)
                .Distinct()
                .ToListAsync();

            query = query.Where(t => traineeIds.Contains(t.TraineeId));
        }

        var trainees = await query.ToListAsync();

        var vm = trainees.Select(t => new TraineeIndexViewModel
        {
            TraineeId        = t.TraineeId,
            FullName         = t.FullName,
            OrganizationName = t.OrganizationName,
            RegistrationDate = t.RegistrationDate,
            Email            = t.Email,
            Phone            = t.Phone,
            StatusName       = t.TraineeStatus?.StatusName
        }).ToList();

        return View(vm);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        if (User.IsInRole("Trainee"))
        {
            var user = await _userManager.GetUserAsync(User);
            var ownTrainee = await _context.Trainees.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == user!.Email);
            if (ownTrainee == null || ownTrainee.TraineeId != id)
                return Forbid();
        }
        else if (User.IsInRole("Instructor"))
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user!.Email);
            if (instructor == null) return Forbid();

            var isAssigned = await _context.Enrollments
                .Include(e => e.Session)
                .AnyAsync(e => e.TraineeId == id && e.Session.InstructorId == instructor.InstructorId);
            if (!isAssigned) return Forbid();
        }

        var t = await _context.Trainees
            .Include(t => t.TraineeStatus)
            .Include(t => t.Enrollments)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(t => t.Enrollments)
                .ThenInclude(e => e.EnrollmentStatus)
            .Include(t => t.TraineeCertificationProgresses)
                .ThenInclude(p => p.Certification)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TraineeId == id);

        if (t == null) return NotFound();

        var vm = new TraineeDetailsViewModel
        {
            TraineeId        = t.TraineeId,
            FullName         = t.FullName,
            OrganizationName = t.OrganizationName,
            RegistrationDate = t.RegistrationDate,
            Email            = t.Email,
            Phone            = t.Phone,
            StatusName       = t.TraineeStatus?.StatusName,
            CertificationProgresses = t.TraineeCertificationProgresses.Select(p => new TraineeCertProgressRow
            {
                CertificationName  = p.Certification?.Name,
                ProgressPercentage = p.ProgressPercentage,
                AchievedDate       = p.AchievedDate
            }).ToList(),
            Enrollments = t.Enrollments.Select(e => new TraineeEnrollmentRow
            {
                CourseName      = e.Session?.Course?.CourseName,
                SessionStart    = e.Session?.StartDateTime,
                SessionEnd      = e.Session?.EndDateTime,
                EnrollmentDate  = e.EnrollmentDate,
                StatusName      = e.EnrollmentStatus?.StatusName
            }).ToList()
        };

        return View(vm);
    }

    [Authorize(Roles = "TrainingCoordinator")]
    public IActionResult Create()
    {
        var vm = new TraineeCreateViewModel
        {
            RegistrationDate = DateOnly.FromDateTime(DateTime.Today)
        };
        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> Create(TraineeCreateViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var trainee = new Trainee
            {
                FullName         = vm.FullName,
                OrganizationName = vm.OrganizationName,
                RegistrationDate = vm.RegistrationDate,
                Email            = vm.Email,
                Phone            = vm.Phone,
                Password         = vm.Password,
                TraineeStatusId  = vm.TraineeStatusId
            };

            _context.Add(trainee);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Trainee \"{trainee.FullName}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(vm);
        return View(vm);
    }

    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var trainee = await _context.Trainees.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TraineeId == id);
        if (trainee == null) return NotFound();

        var vm = new TraineeEditViewModel
        {
            TraineeId        = trainee.TraineeId,
            FullName         = trainee.FullName,
            OrganizationName = trainee.OrganizationName,
            RegistrationDate = trainee.RegistrationDate,
            Email            = trainee.Email,
            Phone            = trainee.Phone,
            Password         = trainee.Password,
            TraineeStatusId  = trainee.TraineeStatusId
        };

        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> Edit(int id, TraineeEditViewModel vm)
    {
        if (id != vm.TraineeId) return NotFound();

        if (ModelState.IsValid)
        {
            var trainee = new Trainee
            {
                TraineeId        = vm.TraineeId,
                FullName         = vm.FullName,
                OrganizationName = vm.OrganizationName,
                RegistrationDate = vm.RegistrationDate,
                Email            = vm.Email,
                Phone            = vm.Phone,
                Password         = vm.Password,
                TraineeStatusId  = vm.TraineeStatusId
            };

            try
            {
                _context.Update(trainee);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Trainee \"{trainee.FullName}\" updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TraineeExists(vm.TraineeId)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(vm);
        return View(vm);
    }

    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var trainee = await _context.Trainees
            .Include(t => t.TraineeStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TraineeId == id);

        if (trainee == null) return NotFound();

        var vm = new TraineeDeleteViewModel
        {
            TraineeId        = trainee.TraineeId,
            FullName         = trainee.FullName,
            OrganizationName = trainee.OrganizationName,
            Email            = trainee.Email,
            Phone            = trainee.Phone,
            StatusName       = trainee.TraineeStatus?.StatusName
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TrainingCoordinator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee != null)
        {
            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Trainee \"{trainee.FullName}\" deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private void LoadDropdowns(TraineeCreateViewModel vm)
    {
        vm.TraineeStatuses = _context.TraineeStatuses.AsNoTracking()
            .Select(s => new SelectListItem { Value = s.TraineeStatusId.ToString(), Text = s.StatusName })
            .ToList();
    }

    private void LoadDropdowns(TraineeEditViewModel vm)
    {
        vm.TraineeStatuses = _context.TraineeStatuses.AsNoTracking()
            .Select(s => new SelectListItem { Value = s.TraineeStatusId.ToString(), Text = s.StatusName })
            .ToList();
    }

    private async Task<bool> TraineeExists(int id) =>
        await _context.Trainees.AnyAsync(t => t.TraineeId == id);
}
