using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using CourseManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CourseManagement.Controllers
{
    //[Authorize(Roles = "Coordinator")]
    [Authorize(Roles = "TrainingCoordinator")] //malak
    public class EquipmentController : Controller
    {
        private readonly CourseManagementDbContext _context;

        public EquipmentController(CourseManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            var query = _context.Equipments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.ToLower();
                query = query.Where(e =>
                    e.EquipmentName.ToLower().Contains(term) ||
                    (e.Description != null && e.Description.ToLower().Contains(term)));
            }

            var equipment = await query
                .Select(e => new EquipmentIndexViewModel
                {
                    EquipmentId   = e.EquipmentId,
                    EquipmentName = e.EquipmentName,
                    Description   = e.Description
                })
                .ToListAsync();

            ViewBag.SearchString = searchString;

            return View(equipment);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipmentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var equipment = new Equipment
                {
                    EquipmentName = model.EquipmentName,
                    Description = model.Description
                };
                _context.Equipments.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null) return NotFound();

            var model = new EquipmentEditViewModel
            {
                EquipmentId = equipment.EquipmentId,
                EquipmentName = equipment.EquipmentName,
                Description = equipment.Description
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EquipmentEditViewModel model)
        {
            if (id != model.EquipmentId) return NotFound();

            if (ModelState.IsValid)
            {
                var equipment = await _context.Equipments.FindAsync(id);
                if (equipment == null) return NotFound();

                equipment.EquipmentName = model.EquipmentName;
                equipment.Description = model.Description;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null) return NotFound();

            var model = new EquipmentIndexViewModel
            {
                EquipmentId = equipment.EquipmentId,
                EquipmentName = equipment.EquipmentName,
                Description = equipment.Description
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null) return NotFound();

            _context.Equipments.Remove(equipment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
