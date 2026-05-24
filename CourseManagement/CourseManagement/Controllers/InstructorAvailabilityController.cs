using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class InstructorAvailabilityController : Controller
    {
        private readonly CourseManagementDbContext _context;

        public InstructorAvailabilityController(CourseManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var availabilities = await _context.InstructorAvailabilities
                .Include(a => a.Instructor)
                .AsNoTracking()
                .OrderByDescending(a => a.AvailableDate)
                .ThenBy(a => a.StartTime)
                .Select(a => new AvailabilityIndexViewModel
                {
                    AvailabilityId = a.AvailabilityId,
                    InstructorName = a.Instructor.FullName,
                    AvailableDate = a.AvailableDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsAvailable = a.IsAvailable
                })
                .ToListAsync();

            return View(availabilities);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateInstructorsAsync();
            return View(new AvailabilityCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AvailabilityCreateViewModel model)
        {
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "End Time must be after Start Time.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateInstructorsAsync(model.InstructorId);
                return View(model);
            }

            var availability = new InstructorAvailability
            {
                InstructorId = model.InstructorId,
                AvailableDate = model.AvailableDate,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                IsAvailable = model.IsAvailable
            };

            _context.InstructorAvailabilities.Add(availability);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var availability = await _context.InstructorAvailabilities.FindAsync(id);
            if (availability == null) return NotFound();

            var model = new AvailabilityEditViewModel
            {
                AvailabilityId = availability.AvailabilityId,
                InstructorId = availability.InstructorId,
                AvailableDate = availability.AvailableDate,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                IsAvailable = availability.IsAvailable
            };

            await PopulateInstructorsAsync(model.InstructorId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AvailabilityEditViewModel model)
        {
            if (id != model.AvailabilityId) return NotFound();

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "End Time must be after Start Time.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateInstructorsAsync(model.InstructorId);
                return View(model);
            }

            var availability = await _context.InstructorAvailabilities.FindAsync(id);
            if (availability == null) return NotFound();

            availability.InstructorId = model.InstructorId;
            availability.AvailableDate = model.AvailableDate;
            availability.StartTime = model.StartTime;
            availability.EndTime = model.EndTime;
            availability.IsAvailable = model.IsAvailable;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var availability = await _context.InstructorAvailabilities
                .Include(a => a.Instructor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.AvailabilityId == id);

            if (availability == null) return NotFound();

            var model = new AvailabilityIndexViewModel
            {
                AvailabilityId = availability.AvailabilityId,
                InstructorName = availability.Instructor.FullName,
                AvailableDate = availability.AvailableDate,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                IsAvailable = availability.IsAvailable
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availability = await _context.InstructorAvailabilities.FindAsync(id);
            if (availability != null)
            {
                _context.InstructorAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateInstructorsAsync(int? selectedInstructorId = null)
        {
            var instructors = await _context.Instructors
                .AsNoTracking()
                .OrderBy(i => i.FullName)
                .ToListAsync();

            ViewBag.Instructors = new SelectList(
                instructors,
                nameof(Instructor.InstructorId),
                nameof(Instructor.FullName),
                selectedInstructorId);
        }
    }
}
