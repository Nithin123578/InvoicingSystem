using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
namespace InvoicingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            return Ok(_categoryService.GetCategories());
        }

        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public ActionResult<Category> AddCategory(Category category)
        {
            var createdCategory = _categoryService.AddCategory(category);
            return Ok(createdCategory);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(Category category)
        {
            _categoryService.UpdateCategory(category);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            _categoryService.DeleteCategory(id);
            return NoContent();
        }

    }
}
