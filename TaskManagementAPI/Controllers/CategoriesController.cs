using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var userId = GetCurrentUserId();
            var categories = await _categoriesService.GetAllCategoriesAsync(userId);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var userId = GetCurrentUserId();
            var category = await _categoriesService.GetCategoryByIdAsync(id, userId);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategory)
        {
            var userId = GetCurrentUserId();
            var newCategory = await _categoriesService.CreateCategoryAsync(createCategory, userId);

            return CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.Id }, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CreateCategoryDto updateCategory)
        {
            var userId = GetCurrentUserId();
            var category = await _categoriesService.UpdateCategoryAsync(id, updateCategory, userId);

            if(category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = GetCurrentUserId();
            var deleted = await _categoriesService.DeleteCategoryAsync(id, userId);

            if(!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetCategoryTasks(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _categoriesService.GetCategoryTasksAsync(id, userId);

            return Ok(result);
        }
    }
}
