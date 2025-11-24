using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext _context;

        public CategoriesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId, int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<Category> CreateCategoryAsync(CreateCategoryDto categoryDto, int userId)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                UserId = userId
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> UpdateCategoryAsync(int categoryId, CreateCategoryDto categoryDto, int userId)
        {
            var category = await GetCategoryByIdAsync(categoryId, userId);
            if (category == null) return null;

            category.Name = categoryDto.Name;
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId, int userId)
        {
            var category = await GetCategoryByIdAsync(categoryId, userId);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetCategoryTasksAsync(int categoryId, int userId)
        {
            var category = await GetCategoryByIdAsync(categoryId, userId);
            var tasks = await _context.Tasks
                .Where(t => t.CategoryId == categoryId && t.UserId == userId)
                .ToListAsync();

            return new
            {
                category,
                tasks,
                count = tasks.Count()
            };
        }
    }
}
