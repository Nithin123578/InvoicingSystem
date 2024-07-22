using Application.Interfaces;
using Application.Models;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products = new List<Product>();
        private int _nextId = 1;


        public IEnumerable<Product> GetProducts()
        {
            return _products;
        }

        public Product GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public Product AddProduct(Product product)
        {
            ValidateProduct(product);
            product.Id = _nextId ++;
            _products.Add(product);
            return product;
        }

        public void UpdateProduct(Product product)
        {
            ValidateProduct(product);
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Category = product.Category;
            }
        }

        public void DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }

        private void ValidateProduct(Product product)
        {
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
