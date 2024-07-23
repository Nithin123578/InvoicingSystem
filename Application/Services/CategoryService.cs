using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Logging;
namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly List<Category> _categories = new List<Category>();
        private readonly ILogger<CategoryService> _logger;
        private int _nextId = 1;

        public CategoryService(ILogger<CategoryService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<Category> GetCategories()
        {
            try
            {
                return _categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories.");
                return new List<Category>();
            }
        }

        public Category GetCategoryById(int id)
        {
            try
            {
                var category = _categories.FirstOrDefault(c => c.Id == id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                }
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category by ID.");
                throw;
            }
        }

        public Category AddCategory(Category category)
        {
            try
            {
                ValidateCategory(category);
                if (_categories.Any(c => c.Name == category.Name && c.Description == category.Description))
                {
                    throw new ArgumentException("Category already exists");
                }
                category.Id = _nextId++;
                _categories.Add(category);
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding category.");
                throw;
            }
        }

        public void UpdateCategory(Category category)
        {
            try
            {
                ValidateCategory(category);
                var existingCategory = _categories.FirstOrDefault(c => c.Id == category.Id);
                if (existingCategory == null)
                {
                    throw new KeyNotFoundException($"Category with ID {category.Id} not found");
                }
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category.");
                throw;
            }
        }

        public void DeleteCategory(int id)
        {
            try
            {
                var category = _categories.FirstOrDefault(c => c.Id == id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                }
                _categories.Remove(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category.");
                throw;
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
