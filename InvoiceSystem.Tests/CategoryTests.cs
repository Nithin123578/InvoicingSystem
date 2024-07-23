using Application.Interfaces;
using Application.Services;
using Application.Models;
using InvoicingSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvoicingSystem.Tests
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private ICategoryService _categoryService;
        private CategoryController _controller;
        private Mock<ILogger<CategoryService>> _loggerMock;
        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _categoryService = new CategoryService(_loggerMock.Object);
            _controller = new CategoryController(_categoryService);
        }

        [Test]
        public void AddCategory_ShouldThrowException_CategoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.AddCategory(null));
            Assert.AreEqual("Category is empty (Parameter 'category')", ex.Message);
        }

        [Test]
        public void UpdateCategory_ShouldThrowException_CategoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.UpdateCategory(null));
            Assert.AreEqual("Category is empty (Parameter 'category')", ex.Message);
        }

        [Test]
        public void AddCategory_Should_AddCategorySuccessfully()
        {
            var category = new Category { Name = "Electronics", Description = "Electronic items" };
            var result = _controller.AddCategory(category);
            var okResult = result.Result as OkObjectResult;
            var addedCategory = okResult.Value as Category;

            Assert.AreEqual(1, addedCategory.Id);
            Assert.AreEqual(category.Name, addedCategory.Name);
        }

        [Test]
        public void GetCategories_Should_ReturnCorrectCategoryList()
        {
            var category1 = new Category { Name = "Electronics", Description = "Electronic items" };
            var result = _controller.AddCategory(category1);
            var okResult = result.Result as OkObjectResult;
            var firstCategory = okResult.Value as Category;

            var category2 = new Category { Name = "Clothing", Description = "Apparel and clothing" };
             result = _controller.AddCategory(category2);
             okResult = result.Result as OkObjectResult;
            var secondCategory = okResult.Value as Category;

            var getResult = _controller.GetCategories() as ActionResult<IEnumerable<Category>>;
            okResult = getResult.Result as OkObjectResult;
            var retrievedCategories = okResult.Value as List<Category>;

            Assert.AreEqual(2, retrievedCategories.Count);
            Assert.Contains(firstCategory, retrievedCategories);
            Assert.Contains(secondCategory, retrievedCategories);
        }

        [Test]
        public void GetCategoryById_Should_ReturnCorrectCategory()
        {
            var category = new Category { Name = "Electronics", Description = "Electronic items" };
            var result = _controller.AddCategory(category);
            var okResult = result.Result as OkObjectResult;
            var addedCategory = okResult.Value as Category;

            result = _controller.GetCategory(addedCategory.Id) as ActionResult<Category>;
            okResult = result.Result as OkObjectResult;
            var retrievedCategory = okResult.Value as Category;
            Assert.AreEqual(addedCategory, retrievedCategory);
        }

        [Test]
        public void UpdateCategory_Should_UpdateCategorySuccessfully()
        {
            var category = new Category { Name = "Electronics", Description = "Electronic items" };
            var result = _controller.AddCategory(category);
            var okResult = result.Result as OkObjectResult;
            var addedCategory = okResult.Value as Category;

            addedCategory.Name = "Clothing";
            addedCategory.Description = "Clothing items";
            _categoryService.UpdateCategory(addedCategory);

            result = _controller.GetCategory(addedCategory.Id) as ActionResult<Category>;
            okResult = result.Result as OkObjectResult;
            var updatedCategory = okResult.Value as Category;

            Assert.AreEqual("Clothing", updatedCategory.Name);
            Assert.AreEqual("Clothing items", updatedCategory.Description);
        }

        [Test]
        public void DeleteCategory_Should_RemoveCategorySuccessfully()
        {
            var category = new Category { Name = "Electronics", Description = "Electronic items" };
            var result = _controller.AddCategory(category);
            var okResult = result.Result as OkObjectResult;
            var addedCategory = okResult.Value as Category;


            _controller.DeleteCategory(addedCategory.Id);
            var getResult = _controller.GetCategories()  as ActionResult<IEnumerable<Category>>;
            okResult = getResult.Result as OkObjectResult;
            var retrievedCategory = okResult.Value as List<Category>;
            Assert.IsFalse(retrievedCategory.Any(p => p.Id == addedCategory.Id));
        }

        [Test]
        public void AddCategory_Should_ThrowException_ForInvalidCategory()
        {
            var invalidCategory = new Category { Name = "", Description = "Clothing items" };
            var ex = Assert.Throws<ArgumentException>(() => _categoryService.AddCategory(invalidCategory));
            Assert.AreEqual("Category name cannot be empty", ex.Message);

            invalidCategory.Name = "Clothing";
            invalidCategory.Description = "";
            ex = Assert.Throws<ArgumentException>(() => _categoryService.AddCategory(invalidCategory));
            Assert.AreEqual("Category description cannot be empty", ex.Message);
        }

        [Test]
        public void UpdateCategory_Should_ThrowException_ForInvalidCategory()
        {
            var validCategory = new Category { Name = "Clothing", Description = "Clothing items" };
            var addedCategory = _categoryService.AddCategory(validCategory);

            validCategory.Id = addedCategory.Id;
            validCategory.Name = "";
            validCategory.Description = "Clothing items";
            var ex = Assert.Throws<ArgumentException>(() => _categoryService.UpdateCategory(validCategory));
            Assert.AreEqual("Category name cannot be empty", ex.Message);

            validCategory.Name = "Clothing";
            validCategory.Description = "";
            ex = Assert.Throws<ArgumentException>(() => _categoryService.UpdateCategory(validCategory));
            Assert.AreEqual("Category description cannot be empty", ex.Message);
        }

        [Test]
        public void UpdateCategory_Should_ThrowException_ForInvalidCategoryId()
        {
            var validCategory = new Category { Name = "Clothing", Description = "Clothing items" };
            var addedCategory = _categoryService.AddCategory(validCategory);

            var invalidCategoryId = 999;
            var ex = Assert.Throws<KeyNotFoundException>(() => _categoryService.GetCategoryById(invalidCategoryId));
            Assert.AreEqual($"Category with ID {invalidCategoryId} not found", ex.Message);
        }
    }
}
