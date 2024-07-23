using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvoicingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        // Service for handling category-related operations
        private readonly ICategoryService _categoryService;

        // Initializes the controller with the category service dependency
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Retrieves a list of all categories
        // Route: GET api/category
        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            // Calls the service method to get all categories and returns them
            return Ok(_categoryService.GetCategories());
        }

        // Retrieves a specific category by its ID
        // Route: GET api/category/{id}
        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                // Returns a 404 Not Found response if the category doesn't exist
                return NotFound();
            }
            // Returns the found category
            return Ok(category);
        }

        // Adds a new category
        // Route: POST api/category
        [HttpPost]
        public ActionResult<Category> AddCategory(Category category)
        {
            var createdCategory = _categoryService.AddCategory(category);
            // Returns the created category with a 200 OK response
            return Ok(createdCategory);
        }

        // Updates an existing category
        // Route: PUT api/category/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(Category category)
        {
            // Calls the service method to update the category
            _categoryService.UpdateCategory(category);
            // Returns a 204 No Content response as there's no content to return
            return NoContent();
        }

        // Deletes a specific category by its ID
        // Route: DELETE api/category/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            // Calls the service method to delete the category
            _categoryService.DeleteCategory(id);
            // Returns a 204 No Content response as there's no content to return
            return NoContent();
        }
    }
}
