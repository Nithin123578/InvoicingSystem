using Application.Models;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories();
        Category GetCategoryById(int id);
        Category AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int id);
    }

}
