using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

public class TraineesController : Controller
{
    private readonly CourseManagementDbContext _context;

    public TraineesController(CourseManagementDbContext context)
    {
        _context = context;
    }

    // GET: Trainees
    public async Task<IActionResult> Index()
    {
        var trainees = await _context.Trainees
            .Include(t => t.TraineeStatus)
            .AsNoTracking()
            .ToListAsync();

        return View(trainees);
    }

    // GET: Trainees/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var trainee = await _context.Trainees
            .Include(t => t.TraineeStatus)
            .Include(t => t.Enrollments)
                .ThenInclude(e => e.Session)
                    .ThenInclude(s => s.Course)
            .Include(t => t.TraineeCertificationProgresses)
                .ThenInclude(p => p.Certification)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TraineeId == id);

        if (trainee == null) return NotFound();

        return View(trainee);
    }

    // GET: Trainees/Create
    public IActionResult Create()
    {
        LoadDropdowns();
        return View();
    }

    // POST: Trainees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("FullName,OrganizationName,RegistrationDate,Email,Phone,Password,TraineeStatusId")]
        Trainee trainee)
    {
        if (ModelState.IsValid)
        {
            _context.Add(trainee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(trainee.TraineeStatusId);
        return View(trainee);
    }

    // GET: Trainees/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee == null) return NotFound();

        LoadDropdowns(trainee.TraineeStatusId);
        return View(trainee);
    }

    // POST: Trainees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("TraineeId,FullName,OrganizationName,RegistrationDate,Email,Phone,Password,TraineeStatusId")]
        Trainee trainee)
    {
        if (id != trainee.TraineeId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(trainee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TraineeExists(trainee.TraineeId))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        LoadDropdowns(trainee.TraineeStatusId);
        return View(trainee);
    }

    // GET: Trainees/Delete/5
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

    // POST: Trainees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var trainee = await _context.Trainees.FindAsync(id);
        if (trainee != null)
        {
            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private void LoadDropdowns(int? selectedStatusId = null)
    {
        ViewData["TraineeStatusId"] = new SelectList(
            _context.TraineeStatuses.AsNoTracking(),
            "TraineeStatusId",
            "StatusName",
            selectedStatusId);
    }

    private async Task<bool> TraineeExists(int id) =>
        await _context.Trainees.AnyAsync(t => t.TraineeId == id);
}
