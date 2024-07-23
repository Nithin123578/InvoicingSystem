using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        // In-memory storage for categories
        private readonly List<Category> _categories = new List<Category>();

        // Logger for logging errors and other information
        private readonly ILogger<CategoryService> _logger;

        // Counter for generating unique IDs for categories
        private int _nextId = 1;

        // Initializes the service with the logger dependency
        public CategoryService(ILogger<CategoryService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Retrieves all categories
        // Returns a list of categories
        public IEnumerable<Category> GetCategories()
        {
            try
            {
                // Returns the list of categories
                return _categories;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while getting categories
                _logger.LogError(ex, "Error occurred while getting categories.");
                // Returns an empty list in case of error
                return new List<Category>();
            }
        }

        // Retrieves a specific category by its ID
        // Returns the category or throws an exception if not found
        public Category GetCategoryById(int id)
        {
            try
            {
                var category = _categories.FirstOrDefault(c => c.Id == id);
                if (category == null)
                {
                    // Throws an exception if the category with the specified ID is not found
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                }
                // Returns the found category
                return category;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while getting category by ID
                _logger.LogError(ex, "Error occurred while getting category by ID.");
                throw;
            }
        }

        // Adds a new category
        // Returns the created category
        public Category AddCategory(Category category)
        {
            try
            {
                // Validates the category to ensure it has valid properties
                ValidateCategory(category);

                // Checks if the category already exists
                if (_categories.Any(c => c.Name == category.Name && c.Description == category.Description))
                {
                    throw new ArgumentException("Category already exists");
                }

                // Assigns a unique ID to the category and adds it to the list
                category.Id = _nextId++;
                _categories.Add(category);

                // Returns the added category
                return category;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while adding the category
                _logger.LogError(ex, "Error occurred while adding category.");
                throw;
            }
        }

        // Updates an existing category
        public void UpdateCategory(Category category)
        {
            try
            {
                // Validates the category to ensure it has valid properties
                ValidateCategory(category);

                // Finds the existing category to update
                var existingCategory = _categories.FirstOrDefault(c => c.Id == category.Id);
                if (existingCategory == null)
                {
                    throw new KeyNotFoundException($"Category with ID {category.Id} not found");
                }

                // Updates the category details
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while updating the category
                _logger.LogError(ex, "Error occurred while updating category.");
                throw;
            }
        }

        // Deletes a specific category by its ID
        public void DeleteCategory(int id)
        {
            try
            {
                // Finds and removes the category with the specified ID
                var category = _categories.FirstOrDefault(c => c.Id == id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                }
                _categories.Remove(category);
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while deleting the category
                _logger.LogError(ex, "Error occurred while deleting category.");
                throw;
            }
        }

        // Validates the category to ensure it has valid properties
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
