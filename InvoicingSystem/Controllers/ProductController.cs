using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvoicingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        // Service for handling product-related operations
        private readonly IProductService _productService;

        // Initializes the controller with the product service dependency
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Retrieves a list of all products
        // Route: GET api/product
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            // Calls the service method to get all products and returns them
            return Ok(_productService.GetProducts());
        }

        // Retrieves a specific product by its ID
        // Route: GET api/product/{id}
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                // Returns a 404 Not Found response if the product doesn't exist
                return NotFound();
            }
            // Returns the found product
            return Ok(product);
        }

        // Adds a new product
        // Route: POST api/product
        [HttpPost]
        public ActionResult<Product> AddProduct(Product product)
        {
            var createdProduct = _productService.AddProduct(product);
            // Returns the created product with a 200 OK response
            return Ok(createdProduct);
        }

        // Updates an existing product
        // Route: PUT api/product/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(Product product)
        {
            // Calls the service method to update the product
            _productService.UpdateProduct(product);
            // Returns a 204 No Content response as there's no content to return
            return NoContent();
        }

        // Deletes a specific product by its ID
        // Route: DELETE api/product/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            // Calls the service method to delete the product
            _productService.DeleteProduct(id);
            // Returns a 204 No Content response as there's no content to return
            return NoContent();
        }
    }
}
