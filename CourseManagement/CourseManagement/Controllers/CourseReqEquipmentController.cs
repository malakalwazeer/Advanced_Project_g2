using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using CourseManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagement.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CourseReqEquipmentController : Controller
    {
        private readonly CourseManagementDbContext _context;

        public CourseReqEquipmentController(CourseManagementDbContext context)
        {
            _context = context;
        }

        // GET: CourseReqEquipment
        public async Task<IActionResult> Index()
        {
            var requirements = await _context.CourseReqEquipments
                .Include(c => c.Course)
                .Include(c => c.Equipment)
                .Select(c => new CourseReqEqIndexViewModel
                {
                    CourseId = c.CourseId,
                    EquipmentId = c.EquipmentId,
                    CourseName = c.Course.CourseName,
                    EquipmentName = c.Equipment.EquipmentName,
                    Quantity = c.Quantity
                })
                .ToListAsync();

            return View(requirements);
        }

        // GET: CourseReqEquipment/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "EquipmentId", "EquipmentName");
            return View();
        }

        // POST: CourseReqEquipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseReqEqCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if already exists
                var exists = await _context.CourseReqEquipments.AnyAsync(r => r.CourseId == model.CourseId && r.EquipmentId == model.EquipmentId);
                if (exists)
                {
                    ModelState.AddModelError("", "This requirement already exists. Please edit the existing record instead.");
                }
                else
                {
                    var requirement = new CourseReqEquipment
                    {
                        CourseId = model.CourseId,
                        EquipmentId = model.EquipmentId,
                        Quantity = model.Quantity
                    };

                    _context.CourseReqEquipments.Add(requirement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", model.CourseId);
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "EquipmentId", "EquipmentName", model.EquipmentId);
            return View(model);
        }

        // GET: CourseReqEquipment/Edit
        public async Task<IActionResult> Edit(int courseId, int equipmentId)
        {
            var requirement = await _context.CourseReqEquipments
                .Include(r => r.Course)
                .Include(r => r.Equipment)
                .FirstOrDefaultAsync(r => r.CourseId == courseId && r.EquipmentId == equipmentId);

            if (requirement == null) return NotFound();

            var model = new CourseReqEqEditViewModel
            {
                CourseId = requirement.CourseId,
                EquipmentId = requirement.EquipmentId,
                Quantity = requirement.Quantity,
                CourseName = requirement.Course.CourseName,
                EquipmentName = requirement.Equipment.EquipmentName
            };

            return View(model);
        }

        // POST: CourseReqEquipment/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseReqEqEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var requirement = await _context.CourseReqEquipments.FindAsync(model.EquipmentId, model.CourseId);
                if (requirement == null) return NotFound();

                requirement.Quantity = model.Quantity;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: CourseReqEquipment/Delete
        public async Task<IActionResult> Delete(int courseId, int equipmentId)
        {
            var requirement = await _context.CourseReqEquipments
                .Include(c => c.Course)
                .Include(c => c.Equipment)
                .FirstOrDefaultAsync(r => r.CourseId == courseId && r.EquipmentId == equipmentId);

            if (requirement == null) return NotFound();

            var model = new CourseReqEqIndexViewModel
            {
                CourseId = requirement.CourseId,
                EquipmentId = requirement.EquipmentId,
                CourseName = requirement.Course.CourseName,
                EquipmentName = requirement.Equipment.EquipmentName,
                Quantity = requirement.Quantity
            };

            return View(model);
        }

        // POST: CourseReqEquipment/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int courseId, int equipmentId)
        {
            var requirement = await _context.CourseReqEquipments.FindAsync(equipmentId, courseId);
            if (requirement != null)
            {
                _context.CourseReqEquipments.Remove(requirement);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
