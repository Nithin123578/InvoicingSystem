using Application.Interfaces;
using Application.Services;
using Application.Models;
using InvoicingSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging;

namespace InvoicingSystem.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private IProductService _productService;
        private ProductController _controller;
        private Mock<ILogger<ProductService>> _loggerMock;
        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ProductService>>();
            _productService = new ProductService(_loggerMock.Object);
            _controller = new ProductController(_productService);
        }

        /// <summary>
        /// Tests that an exception is thrown when attempting to add a null product.
        /// </summary>
        [Test]
        public void AddProduct_ShouldThrowException_ProductIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.AddProduct(null));
            Assert.AreEqual("Product is empty (Parameter 'product')", ex.Message);
        }

        /// <summary>
        /// Tests that an exception is thrown when attempting to update a null product.
        /// </summary>
        [Test]
        public void UpdateProduct_ShouldThrowException_ProductIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.UpdateProduct(null));
            Assert.AreEqual("Product is empty (Parameter 'product')", ex.Message);
        }

        /// <summary>
        /// Tests that a product is added successfully.
        /// </summary>
        [Test]
        public void AddProduct_Should_AddProductSuccessfully()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };

            var result = _controller.AddProduct(product);
            var okResult = result.Result as OkObjectResult;
            var addedProduct = okResult.Value as Product;

            Assert.AreEqual(1, addedProduct.Id);
            Assert.AreEqual(product.Name, addedProduct.Name);
        }

        /// <summary>
        /// Tests that the correct list of products is returned.
        /// </summary>
        [Test]
        public void GetProducts_Should_ReturnCorrectProductList()
        {
            var product1 = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            _controller.AddProduct(product1);

            var product2 = new Product { Name = "towel", Description = "towel", Price = 200, Quantity = 20, Category = "grocery" };
            _controller.AddProduct(product2);

            var getResult = _controller.GetProducts() as ActionResult<IEnumerable<Product>>;
            var okResult = getResult.Result as OkObjectResult;
            var retrievedProducts = okResult.Value as List<Product>;

            Assert.AreEqual(2, retrievedProducts.Count);
            Assert.Contains(product1, retrievedProducts);
            Assert.Contains(product2, retrievedProducts);
        }

        /// <summary>
        /// Tests that a product can be retrieved by its ID.
        /// </summary>
        [Test]
        public void GetProductById_Should_ReturnCorrectProduct()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            var result = _controller.AddProduct(product);
            var okResult = result.Result as OkObjectResult;
            var addedProduct = okResult.Value as Product;

            var getResult = _controller.GetProduct(addedProduct.Id) as ActionResult<Product>;
            var getOkResult = getResult.Result as OkObjectResult;
            var retrievedProduct = getOkResult.Value as Product;

            Assert.AreEqual(addedProduct, retrievedProduct);
        }

        /// <summary>
        /// Tests that a product is updated successfully.
        /// </summary>
        [Test]
        public void UpdateProduct_Should_UpdateProductSuccessfully()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            var result = _controller.AddProduct(product);
            var okResult = result.Result as OkObjectResult;
            var addedProduct = okResult.Value as Product;

            addedProduct.Name = "towel";
            addedProduct.Price = 20;
            addedProduct.Quantity = 20;
            addedProduct.Description = "towel";
            addedProduct.Category = "grocery";
            _productService.UpdateProduct(addedProduct);

            var updatedProduct = _productService.GetProductById(addedProduct.Id);
            Assert.AreEqual("towel", updatedProduct.Name);
            Assert.AreEqual(20, updatedProduct.Price);
            Assert.AreEqual(20, updatedProduct.Quantity);
            Assert.AreEqual("towel", updatedProduct.Description);
            Assert.AreEqual("grocery", updatedProduct.Category);
        }

        /// <summary>
        /// Tests that a product is deleted successfully.
        /// </summary>
        [Test]
        public void DeleteProduct_Should_RemoveProductSuccessfully()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            var result = _controller.AddProduct(product);
            var okResult = result.Result as OkObjectResult;
            var addedProduct = okResult.Value as Product;

            _controller.DeleteProduct(addedProduct.Id);
            var getResult = _controller.GetProducts() as ActionResult<IEnumerable<Product>>;
            var getOkResult = getResult.Result as OkObjectResult;
            var retrievedProducts = getOkResult.Value as List<Product>;

            Assert.IsFalse(retrievedProducts.Any(p => p.Id == addedProduct.Id));
        }

        /// <summary>
        /// Tests that adding a product with invalid properties throws appropriate exceptions.
        /// </summary>
        [Test]
        public void AddProduct_Should_ThrowException_ForInvalidProduct()
        {
            var invalidProduct = new Product { Name = "", Description = "soap", Price = 10, Quantity = 5, Category = "grocery" };
            var ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product name cannot be empty", ex.Message);

            invalidProduct.Name = "soap";
            invalidProduct.Description = "";
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product description cannot be empty", ex.Message);

            invalidProduct.Description = "soap";
            invalidProduct.Price = -10;
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product price must be greater than zero", ex.Message);

            invalidProduct.Price = 10;
            invalidProduct.Quantity = -10;
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product quantity must be greater than zero", ex.Message);

            invalidProduct.Quantity = 10;
            invalidProduct.Category = "";
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product category cannot be empty", ex.Message);
        }

        /// <summary>
        /// Tests that attempting to update a product with an invalid ID throws an exception.
        /// </summary>
        [Test]
        public void UpdateProduct_Should_ThrowException_ForInvalidProductId()
        {
            var invalidProduct = new Product { Name = "baby", Description = "Description1", Price = 100, Quantity = 10, Category = "grocery" };
            _productService.AddProduct(invalidProduct);

            var invalidProductId = 999;
            var ex = Assert.Throws<KeyNotFoundException>(() => _productService.GetProductById(invalidProductId));
            Assert.AreEqual($"Product with ID {invalidProductId} not found", ex.Message);
        }

        /// <summary>
        /// Tests that updating a product with invalid properties throws appropriate exceptions.
        /// </summary>
        [Test]
        public void UpdateProduct_Should_ThrowException_ForInvalidProduct()
        {
            var invalidProduct = new Product { Name = "baby", Description = "Description1", Price = 100, Quantity = 10, Category = "grocery" };
            var addedProduct = _productService.AddProduct(invalidProduct);

            invalidProduct.Id = addedProduct.Id;
            invalidProduct.Name = "";
            var ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product name cannot be empty", ex.Message);

            invalidProduct.Name = "soap";
            invalidProduct.Description = "";
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product description cannot be empty", ex.Message);

            invalidProduct.Description = "soap";
            invalidProduct.Price = -10;
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product price must be greater than zero", ex.Message);

            invalidProduct.Price = 10;
            invalidProduct.Quantity = -10;
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product quantity must be greater than zero", ex.Message);

            invalidProduct.Quantity = 10;
            invalidProduct.Category = "";
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product category cannot be empty", ex.Message);
        }

        /// <summary>
        /// Tests that attempting to add a duplicate product throws an exception.
        /// </summary>
        [Test]
        public void AddProduct_ShouldThrowException_ForDuplicateProduct()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            _controller.AddProduct(product);

            var duplicateProduct = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            var ex = Assert.Throws<ArgumentException>(() => _controller.AddProduct(duplicateProduct));
            Assert.AreEqual("Product already exists", ex.Message);
        }

        /// <summary>
        /// Tests that attempting to update a non-existing product throws an exception.
        /// </summary>
        [Test]
        public void UpdateProduct_ShouldThrowException_ForNonExistingProduct()
        {
            var product = new Product { Id = 999, Name = "non-existent", Description = "non-existent", Price = 100, Quantity = 10, Category = "grocery" };
            var ex = Assert.Throws<KeyNotFoundException>(() => _controller.UpdateProduct(product));
            Assert.AreEqual("Product with ID 999 not found", ex.Message);
        }

        /// <summary>
        /// Tests that attempting to delete a non-existing product throws an exception.
        /// </summary>
        [Test]
        public void DeleteProduct_ShouldThrowException_ForNonExistingProduct()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() => _controller.DeleteProduct(999));
            Assert.AreEqual("Product with ID 999 not found", ex.Message);
        }

        /// <summary>
        /// Tests that attempting to retrieve a product with an invalid ID returns a not found result.
        /// </summary>
        [Test]
        public void GetProduct_ShouldReturnNotFound_ForInvalidId()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() => _controller.GetProduct(999));
            Assert.AreEqual("Product with ID 999 not found", ex.Message);
        }
    }
}
