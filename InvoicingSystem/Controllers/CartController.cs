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

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("{customerId}/add")]
        public ActionResult<CartItem> AddProductToCart(int customerId, CartItem cartTtem)
        {
            return Ok(_cartService.AddProductToCart(customerId, cartTtem));
        }

        [HttpGet("{customerId}")]
        public IActionResult GetCart(int customerId)
        {
            var cart = _cartService.GetCart(customerId);
            return Ok(cart);
        }

        [HttpPost("{customerId}/invoice")]
        public IActionResult GenerateInvoice(int customerId, string paymentOption)
        {
            var invoice = _cartService.GenerateInvoice(customerId, paymentOption);
            return Ok(invoice);
        }

        [HttpDelete("{customerId}")]
        public IActionResult DeleteCart(int customerId)
        {
            _cartService.DeleteCart(customerId);
            return NoContent();
        }
    }
}
