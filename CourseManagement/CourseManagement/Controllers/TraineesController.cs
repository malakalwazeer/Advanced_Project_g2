using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "TrainingCoordinator")]
public class TraineesController : Controller
{
    private readonly CourseManagementDbContext _context;

    public TraineesController(CourseManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var trainees = await _context.Trainees
            .Include(t => t.TraineeStatus)
            .AsNoTracking()
            .ToListAsync();
        return View(trainees);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var trainee = await _context.Trainees
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

        if (trainee == null) return NotFound();
        return View(trainee);
    }

    public IActionResult Create()
    {
        var vm = new TraineeCreateEditViewModel
        {
            RegistrationDate = DateOnly.FromDateTime(DateTime.Today)
        };
        LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TraineeCreateEditViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var trainee = new Trainee
            {
                FullName          = vm.FullName,
                OrganizationName  = vm.OrganizationName,
                RegistrationDate  = vm.RegistrationDate,
                Email             = vm.Email,
                Phone             = vm.Phone,
                Password          = vm.Password,
                TraineeStatusId   = vm.TraineeStatusId
            };
            _context.Add(trainee);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Trainee \"{trainee.FullName}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }
        LoadDropdowns(vm);
        return View(vm);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee == null) return NotFound();

        var vm = new TraineeCreateEditViewModel
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
    public async Task<IActionResult> Edit(int id, TraineeCreateEditViewModel vm)
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

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var trainee = await _context.Trainees
            .Include(t => t.TraineeStatus)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TraineeId == id);

        if (trainee == null) return NotFound();
        return View(trainee);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
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

    private void LoadDropdowns(TraineeCreateEditViewModel vm)
    {
        vm.TraineeStatuses = _context.TraineeStatuses.AsNoTracking()
            .OrderBy(s => s.StatusName)
            .Select(s => new SelectListItem { Value = s.TraineeStatusId.ToString(), Text = s.StatusName })
            .ToList();
    }

    private async Task<bool> TraineeExists(int id) =>
        await _context.Trainees.AnyAsync(t => t.TraineeId == id);
}
