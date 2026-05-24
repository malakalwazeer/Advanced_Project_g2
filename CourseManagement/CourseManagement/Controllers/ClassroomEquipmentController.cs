using CourseManagement.ViewModels;
using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Controllers;

[Authorize(Roles = "Coordinator")]
public class ClassroomEquipmentController : Controller
{
    private readonly CourseManagementDbContext _context;

    public ClassroomEquipmentController(CourseManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var classroomEquipment = await _context.ClassroomEquipments
            .Include(ce => ce.Classroom)
            .Include(ce => ce.Equipment)
            .OrderBy(ce => ce.Classroom.Location)
            .ThenBy(ce => ce.Equipment.EquipmentName)
            .Select(ce => new ClassroomEqIndexViewModel
            {
                ClassroomId = ce.ClassroomId,
                EquipmentId = ce.EquipmentId,
                LocationName = ce.Classroom.Location,
                EquipmentName = ce.Equipment.EquipmentName,
                Quantity = ce.Quantity
            })
            .ToListAsync();

        return View(classroomEquipment);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateSelectListsAsync();
        return View(new ClassroomEqCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassroomEqCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectListsAsync(model.ClassroomId, model.EquipmentId);
            return View(model);
        }

        var classroomEquipment = new ClassroomEquipment
        {
            ClassroomId = model.ClassroomId,
            EquipmentId = model.EquipmentId,
            Quantity = model.Quantity
        };

        _context.ClassroomEquipments.Add(classroomEquipment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int classroomId, int equipmentId)
    {
        var ce = await _context.ClassroomEquipments
            .FirstOrDefaultAsync(c => c.ClassroomId == classroomId && c.EquipmentId == equipmentId);
        
        if (ce == null) return NotFound();

        return View(new ClassroomEqEditViewModel { ClassroomId = ce.ClassroomId, EquipmentId = ce.EquipmentId, Quantity = ce.Quantity });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ClassroomEqEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ce = await _context.ClassroomEquipments
            .FirstOrDefaultAsync(c => c.ClassroomId == model.ClassroomId && c.EquipmentId == model.EquipmentId);

        if (ce == null) return NotFound();

        ce.Quantity = model.Quantity;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int classroomId, int equipmentId)
    {
        var ce = await _context.ClassroomEquipments
            .Include(c => c.Classroom)
            .Include(c => c.Equipment)
            .FirstOrDefaultAsync(c => c.ClassroomId == classroomId && c.EquipmentId == equipmentId);

        if (ce == null) return NotFound();

        return View(ce);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int classroomId, int equipmentId)
    {
        var ce = await _context.ClassroomEquipments
            .FirstOrDefaultAsync(c => c.ClassroomId == classroomId && c.EquipmentId == equipmentId);

        if (ce != null)
        {
            _context.ClassroomEquipments.Remove(ce);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // Helper that queries classrooms and equipment at once
    private async Task PopulateSelectListsAsync(int? selectedClassroomId = null, int? selectedEquipmentId = null)
    {
        var classrooms = await _context.Classrooms
            .OrderBy(c => c.Location)
            .ToListAsync();

        var equipment = await _context.Equipments
            .OrderBy(e => e.EquipmentName)
            .ToListAsync();

        ViewBag.Classrooms = new SelectList(classrooms, "ClassroomId", "Location", selectedClassroomId);
        ViewBag.Equipment = new SelectList(equipment, "EquipmentId", "EquipmentName", selectedEquipmentId);
    }
}
