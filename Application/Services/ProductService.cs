using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products = new List<Product>();
        private readonly ILogger<ProductService> _logger;
        private int _nextId = 1;

        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<Product> GetProducts()
        {
            try
            {
                return _products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting products.");
                return new List<Product>();
            }
        }

        public Product GetProductById(int id)
        {
            try
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found");
                }
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product by ID.");
                throw;
            }
        }

        public Product AddProduct(Product product)
        {
            try
            {
                ValidateProduct(product);
                product.Id = _nextId++;
                _products.Add(product);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product.");
                throw;
            }
        }

        public void UpdateProduct(Product product)
        {
            try
            {
                ValidateProduct(product);
                var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct == null)
                {
                    throw new KeyNotFoundException($"Product with ID {product.Id} not found");
                }
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Category = product.Category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product.");
                throw;
            }
        }

        public void DeleteProduct(int id)
        {
            try
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found");
                }
                _products.Remove(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product.");
                throw;
            }
        }

        private void ValidateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product is empty");
            }
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("Product name cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(product.Description))
            {
                throw new ArgumentException("Product description cannot be empty");
            }
            if (product.Price <= 0)
            {
                throw new ArgumentException("Product price must be greater than zero");
            }
            if (product.Quantity <= 0)
            {
                throw new ArgumentException("Product quantity must be greater than zero");
            }
            if (string.IsNullOrEmpty(product.Category))
            {
                throw new ArgumentException("Product category cannot be empty");
            }
        }
    }
}
