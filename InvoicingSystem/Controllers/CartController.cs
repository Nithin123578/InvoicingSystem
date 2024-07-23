using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvoicingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        // Initializes the controller with the cart service dependency
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Adds a product to the cart for a specified customer
        // Route: POST api/cart/{customerId}/add
        [HttpPost("{customerId}/add")]
        public ActionResult<CartItem> AddProductToCart(int customerId, CartItem cartItem)
        {
            // Calls the service to add the product to the cart
            var addedItem = _cartService.AddProductToCart(customerId, cartItem);
            // Returns the added product with HTTP 200 OK status
            return Ok(addedItem);
        }

        // Retrieves the cart for a specified customer
        // Route: GET api/cart/{customerId}
        [HttpGet("{customerId}")]
        public IActionResult GetCart(int customerId)
        {
            // Fetches the cart details from the service
            var cart = _cartService.GetCart(customerId);
            // Returns the cart with HTTP 200 OK status
            return Ok(cart);
        }

        // Generates an invoice based on the customer's cart and payment option
        // Route: POST api/cart/{customerId}/invoice
        [HttpPost("{customerId}/invoice")]
        public IActionResult GenerateInvoice(int customerId, string paymentOption)
        {
            // Calls the service to generate an invoice
            var invoice = _cartService.GenerateInvoice(customerId, paymentOption);
            // Returns the invoice with HTTP 200 OK status
            return Ok(invoice);
        }

        // Deletes the cart for a specified customer
        // Route: DELETE api/cart/{customerId}
        [HttpDelete("{customerId}")]
        public IActionResult DeleteCart(int customerId)
        {
            // Deletes the cart from the service
            _cartService.DeleteCart(customerId);
            // Returns HTTP 204 No Content status to indicate successful deletion
            return NoContent();
        }
    }
}
