using Application.Interfaces;
using Application.Models;
using Application.Services;
using InvoicingSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Tests that an exception is thrown when attempting to add a null category.
        /// </summary>
        [Test]
        public void AddCategory_ShouldThrowException_CategoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.AddCategory(null));
            Assert.AreEqual("Category is empty (Parameter 'category')", ex.Message);
        }

        /// <summary>
        /// Tests that an exception is thrown when attempting to update a null category.
        /// </summary>
        [Test]
        public void UpdateCategory_ShouldThrowException_CategoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.UpdateCategory(null));
            Assert.AreEqual("Category is empty (Parameter 'category')", ex.Message);
        }

        /// <summary>
        /// Tests that a category is added successfully and has the correct properties.
        /// </summary>
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

        /// <summary>
        /// Tests that the correct list of categories is returned.
        /// </summary>
        [Test]
        public void GetCategories_Should_ReturnCorrectCategoryList()
        {
            var category1 = new Category { Name = "Electronics", Description = "Electronic items" };
            var category2 = new Category { Name = "Clothing", Description = "Apparel and clothing" };
            _controller.AddCategory(category1);
            _controller.AddCategory(category2);

            var getResult = _controller.GetCategories() as ActionResult<IEnumerable<Category>>;
            var okResult = getResult.Result as OkObjectResult;
            var retrievedCategories = okResult.Value as List<Category>;

            Assert.AreEqual(2, retrievedCategories.Count);
            Assert.Contains(category1, retrievedCategories);
            Assert.Contains(category2, retrievedCategories);
        }

        /// <summary>
        /// Tests that a category can be retrieved by its ID.
        /// </summary>
        [Test]
        public void GetCategoryById_Should_ReturnCorrectCategory()
        {
            var category = new Category { Name = "Electronics", Description = "Electronic items" };
            var result = _controller.AddCategory(category);
            var okResult = result.Result as OkObjectResult;
            var addedCategory = okResult.Value as Category;

            var getResult = _controller.GetCategory(addedCategory.Id) as ActionResult<Category>;
            okResult = getResult.Result as OkObjectResult;
            var retrievedCategory = okResult.Value as Category;

            Assert.AreEqual(addedCategory, retrievedCategory);
        }

        /// <summary>
        /// Tests that a category is updated successfully with new values.
        /// </summary>
        [Test]
        public void UpdateCategory_Should_UpdateCategorySuccessfully()
        {
            var category = new Category { Name = "Electronics", Description = "Electronic items" };
            var result = _controller.AddCategory(category);
            var okResult = result.Result as OkObjectResult;
            var addedCategory = okResult.Value as Category;

            // Update the category details
            addedCategory.Name = "Clothing";
            addedCategory.Description = "Clothing items";
            _categoryService.UpdateCategory(addedCategory);

            var updatedResult = _controller.GetCategory(addedCategory.Id) as ActionResult<Category>;
            okResult = updatedResult.Result as OkObjectResult;
            var updatedCategory = okResult.Value as Category;

            Assert.AreEqual("Clothing", updatedCategory.Name);
            Assert.AreEqual("Clothing items", updatedCategory.Description);
        }

        /// <summary>
        /// Tests that a category is deleted successfully and no longer exists in the list.
        /// </summary>
        [Test]
        public void DeleteCategory_Should_RemoveCategorySuccessfully()
        {
            var category = new Category { Name = "Electronics", Description = "Electronic items" };
            var result = _controller.AddCategory(category);
            var okResult = result.Result as OkObjectResult;
            var addedCategory = okResult.Value as Category;

            _controller.DeleteCategory(addedCategory.Id);
            var getResult = _controller.GetCategories() as ActionResult<IEnumerable<Category>>;
            okResult = getResult.Result as OkObjectResult;
            var retrievedCategories = okResult.Value as List<Category>;

            Assert.IsFalse(retrievedCategories.Any(c => c.Id == addedCategory.Id));
        }

        /// <summary>
        /// Tests that adding a category with invalid properties throws appropriate exceptions.
        /// </summary>
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

        /// <summary>
        /// Tests that updating a category with invalid properties throws appropriate exceptions.
        /// </summary>
        [Test]
        public void UpdateCategory_Should_ThrowException_ForInvalidCategory()
        {
            var validCategory = new Category { Name = "Clothing", Description = "Clothing items" };
            var addedCategory = _categoryService.AddCategory(validCategory);

            validCategory.Id = addedCategory.Id;
            validCategory.Name = "";
            var ex = Assert.Throws<ArgumentException>(() => _categoryService.UpdateCategory(validCategory));
            Assert.AreEqual("Category name cannot be empty", ex.Message);

            validCategory.Name = "Clothing";
            validCategory.Description = "";
            ex = Assert.Throws<ArgumentException>(() => _categoryService.UpdateCategory(validCategory));
            Assert.AreEqual("Category description cannot be empty", ex.Message);
        }

        /// <summary>
        /// Tests that attempting to update a category with an invalid ID throws an exception.
        /// </summary>
        [Test]
        public void UpdateCategory_Should_ThrowException_ForInvalidCategoryId()
        {
            var validCategory = new Category { Name = "Clothing", Description = "Clothing items" };
            _categoryService.AddCategory(validCategory);

            var invalidCategoryId = 999;
            var ex = Assert.Throws<KeyNotFoundException>(() => _categoryService.GetCategoryById(invalidCategoryId));
            Assert.AreEqual($"Category with ID {invalidCategoryId} not found", ex.Message);
        }

        /// <summary>
        /// Tests that adding a category with a duplicate name throws an exception.
        /// </summary>
        [Test]
        public void AddCategory_ShouldThrowException_ForDuplicateCategory()
        {
            var category = new Category { Name = "Electronics", Description = "Electronic items" };
            _controller.AddCategory(category);

            var duplicateCategory = new Category { Name = "Electronics", Description = "Electronic items" };
            var ex = Assert.Throws<ArgumentException>(() => _controller.AddCategory(duplicateCategory));
            Assert.AreEqual("Category already exists", ex.Message);
        }

        /// <summary>
        /// Tests that retrieving a category with an invalid ID returns a not found result.
        /// </summary>
        [Test]
        public void GetCategoryById_ShouldReturnNotFound_ForInvalidId()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() => _controller.GetCategory(999));
            Assert.AreEqual("Category with ID 999 not found", ex.Message);
        }
    }
}
