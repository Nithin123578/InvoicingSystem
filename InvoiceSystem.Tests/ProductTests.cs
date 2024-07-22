using Application.Interfaces;
using Application.Services;
using Application.Models;

namespace InvoicingSystem.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private IProductService _productService;

        [SetUp]
        public void Setup()
        {
            _productService = new ProductService();
        }

        [Test]
        public void AddProduct_Should_AddProductSuccessfully()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, CategoryId = 1 };
            var addedProduct = _productService.AddProduct(product);

            Assert.AreEqual(1, addedProduct.Id);
            //Assert.AreEqual(1, _productService.GetProducts().Count());
        }

        [Test]
        public void GetProducts_Should_ReturnCorrectProductList()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, CategoryId = 1 };
            var firstProduct = _productService.AddProduct(product);

             product = new Product { Name = "towel", Description = "towel", Price = 200, Quantity = 20, CategoryId = 1 };
            var secondProduct = _productService.AddProduct(product);

            var retrievedProduct = _productService.GetProducts();

            Assert.AreEqual(new List<Product> { firstProduct , secondProduct }, retrievedProduct);
        }

        [Test]
        public void GetProductById_Should_ReturnCorrectProduct()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, CategoryId = 1 };
            var addedProduct = _productService.AddProduct(product);

            var retrievedProduct = _productService.GetProductById(addedProduct.Id);

            Assert.AreEqual(addedProduct, retrievedProduct);
        }

        [Test]
        public void UpdateProduct_Should_UpdateProductSuccessfully()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, CategoryId = 1 };
            var addedProduct = _productService.AddProduct(product);
            addedProduct.Name = "towel";
            addedProduct.Price = 20;
            addedProduct.Quantity = 20; 
            addedProduct.Description = "towel";
            addedProduct.CategoryId = 2;
            _productService.UpdateProduct(addedProduct);
            var updatedProduct = _productService.GetProductById(addedProduct.Id);
            Assert.AreEqual("towel", updatedProduct.Name);
            Assert.AreEqual(20, updatedProduct.Price);
            Assert.AreEqual(20, updatedProduct.Quantity);
            Assert.AreEqual("towel", updatedProduct.Description);
            Assert.AreEqual(2, updatedProduct.CategoryId);
        }

        [Test]
        public void DeleteProduct_Should_RemoveProductSuccessfully()
        {
            var product = new Product { Name = "soap", Description = "soap", Price = 100, Quantity = 10, CategoryId = 1 };
            var addedProduct = _productService.AddProduct(product);

            _productService.DeleteProduct(addedProduct.Id);
            var retrievedProduct = _productService.GetProductById(addedProduct.Id);
            Assert.IsNull(retrievedProduct);
        }

        [Test]
        public void AddProduct_Should_ThrowException_ForInvalidProduct()
        {
            var invalidProduct = new Product { Name = "", Description = "soap", Price = 10, Quantity = 5, CategoryId = 1 };
            var ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product name cannot be empty", ex.Message);



            invalidProduct.Name = "soap";
            invalidProduct.Description = "";
            invalidProduct.Price = 100;
            invalidProduct.Quantity = 10;
            invalidProduct.CategoryId = 10;
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product description cannot be empty", ex.Message);

            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = -10;
            invalidProduct.Quantity = 10;
            invalidProduct.CategoryId = 10;
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product price must be greater than zero", ex.Message);


            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 10;
            invalidProduct.Quantity = -10;
            invalidProduct.CategoryId = 10;
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product quantity must be greater than zero", ex.Message);


            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 10;
            invalidProduct.Quantity = 10;
            invalidProduct.CategoryId = -10;
            ex = Assert.Throws<ArgumentException>(() => _productService.AddProduct(invalidProduct));
            Assert.AreEqual("Product category Id must be greater than zero", ex.Message);

        }

        [Test]
        public void UpdateProduct_Should_ThrowException_ForInvalidProduct()
        {
            var invalidProduct = new Product { Name = "baby", Description = "Description1", Price = 100, Quantity = 10, CategoryId = 1 };
            var addedProduct = _productService.AddProduct(invalidProduct);

            invalidProduct.Id = addedProduct.Id;
            invalidProduct.Name = "";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 100;
            invalidProduct.Quantity = 10;
            invalidProduct.CategoryId = 10;
            var ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product name cannot be empty", ex.Message);

            invalidProduct.Id = addedProduct.Id;
            invalidProduct.Name = "soap";
            invalidProduct.Description = "";
            invalidProduct.Price = 100;
            invalidProduct.Quantity = 10;
            invalidProduct.CategoryId = 10;
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product description cannot be empty", ex.Message);

            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = -10;
            invalidProduct.Quantity = 10;
            invalidProduct.CategoryId = 10;
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product price must be greater than zero", ex.Message);


            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 10;
            invalidProduct.Quantity = -10;
            invalidProduct.CategoryId = 10;
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product quantity must be greater than zero", ex.Message);


            invalidProduct.Name = "soap";
            invalidProduct.Description = "soap";
            invalidProduct.Price = 10;
            invalidProduct.Quantity = 10;
            invalidProduct.CategoryId = -10;
            ex = Assert.Throws<ArgumentException>(() => _productService.UpdateProduct(invalidProduct));
            Assert.AreEqual("Product category Id must be greater than zero", ex.Message);
        }
    }
}
