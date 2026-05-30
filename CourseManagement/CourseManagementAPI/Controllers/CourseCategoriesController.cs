using CourseManagementAPI.Data;
using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseCategoriesController : ControllerBase
    {
            private readonly CourseManagementDbContext _context;

            public CourseCategoriesController(CourseManagementDbContext context)
            {
                _context = context;
            }

            [HttpGet]
            [AllowAnonymous]
            public async Task<ActionResult<IEnumerable<CourseCategory>>> GetCourseCategories()
            {
                return await _context.CourseCategories.ToListAsync();
            }

            [HttpGet("{id}")]
            [AllowAnonymous]
            public async Task<ActionResult<CourseCategory>> GetCourseCategory(int id)
            {
                var CourseCategories = await _context.CourseCategories.FindAsync(id);
                if (CourseCategories == null)
                {
                    return NotFound();
                }
                return CourseCategories;
            }

            [HttpPost]
            [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]    
            public async Task<ActionResult<CourseCategory>> CreateCourseCategory(CourseCategory courseCategory)
            {
                _context.CourseCategories.Add(courseCategory);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCourseCategories), new { id = courseCategory.CategoryId }, courseCategory);
            }

            [HttpPut("{id}")]
            [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole},{IdentitySeeder.InstructorRole}")]
            public async Task<ActionResult<CourseCategory>> UpdateCourseCategory(int id, CourseCategory updatedCourseCategory)
            {
                if (id != updatedCourseCategory.CategoryId) return BadRequest();

                var exists = await _context.CourseCategories.AnyAsync(a => a.CategoryId == id);
                if (!exists) return NotFound();

                _context.Entry(updatedCourseCategory).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }

            [HttpDelete("{id}")]
            [Authorize(Roles = $"{IdentitySeeder.TrainingCoordinatorRole}")]
            public async Task<IActionResult> DeleteCourseCategory(int id)
            {
                var courseCategory = await _context.CourseCategories.FindAsync(id);
                if (courseCategory == null) return NotFound();

                _context.CourseCategories.Remove(courseCategory);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
    }

