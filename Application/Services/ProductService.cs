using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        // In-memory storage for products
        private readonly List<Product> _products = new List<Product>();

        // Logger for logging errors and other information
        private readonly ILogger<ProductService> _logger;

        // Counter for generating unique IDs for products
        private int _nextId = 1;

        // Initializes the service with the logger dependency
        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Retrieves all products
        // Returns a list of products
        public IEnumerable<Product> GetProducts()
        {
            try
            {
                // Returns the list of products
                return _products;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while getting products
                _logger.LogError(ex, "Error occurred while getting products.");
                // Returns an empty list in case of error
                return new List<Product>();
            }
        }

        // Retrieves a specific product by its ID
        // Returns the product or throws an exception if not found
        public Product GetProductById(int id)
        {
            try
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    // Throws an exception if the product with the specified ID is not found
                    throw new KeyNotFoundException($"Product with ID {id} not found");
                }
                // Returns the found product
                return product;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while getting product by ID
                _logger.LogError(ex, "Error occurred while getting product by ID.");
                throw;
            }
        }

        // Adds a new product
        // Returns the created product
        public Product AddProduct(Product product)
        {
            try
            {
                // Validates the product to ensure it has valid properties
                ValidateProduct(product);

                // Checks if the product already exists
                if (_products.Any(p => p.Name == product.Name && p.Description == product.Description && p.Price == product.Price && p.Quantity == product.Quantity && p.Category == product.Category))
                {
                    throw new ArgumentException("Product already exists");
                }

                // Assigns a unique ID to the product and adds it to the list
                product.Id = _nextId++;
                _products.Add(product);

                // Returns the added product
                return product;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while adding the product
                _logger.LogError(ex, "Error occurred while adding product.");
                throw;
            }
        }

        // Updates an existing product
        public void UpdateProduct(Product product)
        {
            try
            {
                // Validates the product to ensure it has valid properties
                ValidateProduct(product);

                // Finds the existing product to update
                var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct == null)
                {
                    throw new KeyNotFoundException($"Product with ID {product.Id} not found");
                }

                // Updates the product details
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Category = product.Category;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while updating the product
                _logger.LogError(ex, "Error occurred while updating product.");
                throw;
            }
        }

        // Deletes a specific product by its ID
        public void DeleteProduct(int id)
        {
            try
            {
                // Finds and removes the product with the specified ID
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found");
                }
                _products.Remove(product);
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while deleting the product
                _logger.LogError(ex, "Error occurred while deleting product.");
                throw;
            }
        }

        // Validates the product to ensure it has valid properties
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
