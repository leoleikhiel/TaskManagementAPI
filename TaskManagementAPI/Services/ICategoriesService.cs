using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface ICategoriesService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(int userId);
        Task<Category?> GetCategoryByIdAsync(int categoryId, int userId);
        Task<Category> CreateCategoryAsync(CreateCategoryDto categoryDto, int userId);
        Task<Category?> UpdateCategoryAsync(int categoryId, CreateCategoryDto categoryDto, int userId);
        Task<bool> DeleteCategoryAsync(int categoryId, int userId);
        Task<object> GetCategoryTasksAsync(int categoryId, int userId);
    }
}
