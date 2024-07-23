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

        [Test]
        public void AddProduct_ShouldThrowException_ProductIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.AddProduct(null));
            Assert.AreEqual("Product is empty (Parameter 'product')", ex.Message);
        }

        [Test]
        public void UpdateProduct_ShouldThrowException_ProductIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.UpdateProduct(null));
            Assert.AreEqual("Product is empty (Parameter 'product')", ex.Message);
        }


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

        [Test]
        public void GetProducts_Should_ReturnCorrectProductList()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            var result = _controller.AddProduct(product);
            var okResult = result.Result as OkObjectResult;
            var firstProduct = okResult.Value as Product;

            product = new Product { Name = "towel", Description = "towel", Price = 200, Quantity = 20, Category = "grocery" };
            result = _controller.AddProduct(product);
            okResult = result.Result as OkObjectResult;
            var secondProduct = okResult.Value as Product;


            var getResult = _controller.GetProducts() as ActionResult<IEnumerable<Product>>;
            okResult = getResult.Result as OkObjectResult;
            var retrievedProduct = okResult.Value as List<Product>;

            Assert.AreEqual(2, retrievedProduct.Count);
            Assert.Contains(firstProduct, retrievedProduct);
            Assert.Contains(secondProduct, retrievedProduct);
        }

        [Test]
        public void GetProductById_Should_ReturnCorrectProduct()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            var result = _controller.AddProduct(product);
            var okResult = result.Result as OkObjectResult;
            var addedProduct = okResult.Value as Product;

             result = _controller.GetProduct(addedProduct.Id) as ActionResult<Product>;
             okResult = result.Result as OkObjectResult;
            var retrievedProduct = okResult.Value as Product;

            Assert.AreEqual(addedProduct, retrievedProduct);
        }

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

        [Test]
        public void DeleteProduct_Should_RemoveProductSuccessfully()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            var result = _controller.AddProduct(product);
            var okResult = result.Result as OkObjectResult;
            var addedProduct = okResult.Value as Product;

            _controller.DeleteProduct(addedProduct.Id);
            var getResult = _controller.GetProducts() as ActionResult<IEnumerable<Product>>;
            okResult = getResult.Result as OkObjectResult;
            var retrievedProducts = okResult.Value as List<Product>;

            Assert.IsFalse(retrievedProducts.Any(p => p.Id == addedProduct.Id));
        }

        [Test]
        public void AddProduct_Should_ThrowException_ForInvalidProduct()
        {
            var invalidProduct = new Product { Name = "", Description = "soap", Price = 10, Quantity = 5, Category = "grocery" };
            var ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product name cannot be empty", ex.Message);



            invalidProduct.Name = "soap";
            invalidProduct.Description = "";
            invalidProduct.Price = 100;
            invalidProduct.Quantity = 10;
            invalidProduct.Category = "grocery";
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product description cannot be empty", ex.Message);

            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = -10;
            invalidProduct.Quantity = 10;
            invalidProduct.Category = "grocery";
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product price must be greater than zero", ex.Message);


            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 10;
            invalidProduct.Quantity = -10;
            invalidProduct.Category = "grocery";
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product quantity must be greater than zero", ex.Message);


            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 10;
            invalidProduct.Quantity = 10;
            invalidProduct.Category = "";
            invalidProduct.Category = "";
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product category cannot be empty", ex.Message);

        }

        [Test]
        public void UpdateProduct_Should_ThrowException_ForInvalidProductId()
        {
            var invalidProduct = new Product { Name = "baby", Description = "Description1", Price = 100, Quantity = 10, Category = "grocery" };
            var addedProduct = _productService.AddProduct(invalidProduct);

            var invalidProductId = 999;
            var ex = Assert.Throws<KeyNotFoundException>(() => _productService.GetProductById(invalidProductId));
            Assert.AreEqual($"Product with ID {invalidProductId} not found", ex.Message);
        }

        [Test]
        public void UpdateProduct_Should_ThrowException_ForInvalidProduct()
        {
            var invalidProduct = new Product { Name = "baby", Description = "Description1", Price = 100, Quantity = 10, Category = "grocery" };
            var addedProduct = _productService.AddProduct(invalidProduct);

            invalidProduct.Id = addedProduct.Id;
            invalidProduct.Name = "";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 100;
            invalidProduct.Quantity = 10;
            invalidProduct.Category = "grocery";
            var ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product name cannot be empty", ex.Message);

            invalidProduct.Name = "soap";
            invalidProduct.Description = "";
            invalidProduct.Price = 100;
            invalidProduct.Quantity = 10;
            invalidProduct.Category = "grocery";
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product description cannot be empty", ex.Message);

            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = -10;
            invalidProduct.Quantity = 10;
            invalidProduct.Category = "grocery";
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product price must be greater than zero", ex.Message);


            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 10;
            invalidProduct.Quantity = -10;
            invalidProduct.Category = "grocery";
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product quantity must be greater than zero", ex.Message);


            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 10;
            invalidProduct.Quantity = 10;
            invalidProduct.Category = "";
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product category cannot be empty", ex.Message);
        }

        [Test]
        public void AddProduct_ShouldThrowException_ForDuplicateProduct()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            _controller.AddProduct(product);

            var duplicateProduct = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, Category = "grocery" };
            var ex = Assert.Throws<ArgumentException>(() => _controller.AddProduct(duplicateProduct));
            Assert.AreEqual("Product already exists", ex.Message);
        }

        [Test]
        public void UpdateProduct_ShouldThrowException_ForNonExistingProduct()
        {
            var product = new Product { Id = 999, Name = "non-existent", Description = "non-existent", Price = 100, Quantity = 10, Category = "grocery" };
            var ex = Assert.Throws<KeyNotFoundException>(() => _controller.UpdateProduct(product));
            Assert.AreEqual("Product with ID 999 not found", ex.Message);
        }

        [Test]
        public void DeleteProduct_ShouldThrowException_ForNonExistingProduct()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() => _controller.DeleteProduct(999));
            Assert.AreEqual("Product with ID 999 not found", ex.Message);
        }

        [Test]
        public void GetProduct_ShouldReturnNotFound_ForInvalidId()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() => _controller.GetProduct(999));
            Assert.AreEqual("Product with ID 999 not found", ex.Message);
        }
    }
}
