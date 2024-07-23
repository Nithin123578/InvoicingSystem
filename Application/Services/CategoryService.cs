using Application.Interfaces;
using Application.Models;
namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly List<Category> _categories = new List<Category>();
        private int _nextId = 1;

        public IEnumerable<Category> GetCategories()
        {
            return _categories;
        }

        public Category GetCategoryById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }

        public Category AddCategory(Category category)
        {
            ValidateCategory(category);
            category.Id = _nextId++;
            _categories.Add(category);
            return category;
        }

        public void UpdateCategory(Category category)
        {
            ValidateCategory(category);
            var existingCategory = _categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
            }
        }

        public void DeleteCategory(int id)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _categories.Remove(category);
            }
        }

        private void ValidateCategory(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category is empty");
            }
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Category name cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(category.Description))
            {
                throw new ArgumentException("Category description cannot be empty");
            }
        }
    }

}
