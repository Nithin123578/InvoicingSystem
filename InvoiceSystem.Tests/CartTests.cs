using Application.Interfaces;
using Application.Models;
using Application.Services;
using InvoicingSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvoicingSystem.Tests
{
    [TestFixture]
    public class CartTests
    {
        private CartController _controller;
        private ICartService _cartService;
        private Mock<IProductService> _mockProductService;
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<ILogger<CartService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockProductService = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<CartService>>();
            _cartService = new CartService(_mockCustomerService.Object, _mockProductService.Object, _loggerMock.Object);
            _controller = new CartController(_cartService);
        }

        [Test]
        public void AddItemToCart_Should_AddItemsSuccessfully()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };

            var actionResult = _controller.AddProductToCart(customerId, cartItem);

            var okResult = actionResult.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedCartItem = okResult.Value as CartItem;
            Assert.NotNull(returnedCartItem);

            Assert.AreEqual(cartItem.Name, returnedCartItem.Name);
            Assert.AreEqual(cartItem.Price, returnedCartItem.Price);
            Assert.AreEqual(cartItem.Quantity, returnedCartItem.Quantity);
        }

        [Test]
        public void GeCart_Should_ReturnCorrectCartSList()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            var result = _controller.AddProductToCart(customerId, cartItem);
            var okResult = result.Result as OkObjectResult;
            var firstCart = okResult.Value as CartItem;

            cartItem = new CartItem { Name = "mouse", Price = 100.00m, Quantity = 1, Discount = 9 };
            result = _controller.AddProductToCart(customerId, cartItem);
            okResult = result.Result as OkObjectResult;
            var secondCart = okResult.Value as CartItem;


            var getResult = _controller.GetCart(customerId) as IActionResult;
            okResult = getResult as OkObjectResult;
            var retrievedCarts = okResult.Value as Cart;

            Assert.AreEqual(2, retrievedCarts.Items.Count);
            Assert.Contains(firstCart, retrievedCarts.Items);
            Assert.Contains(secondCart, retrievedCarts.Items);
        }

        [Test]
        public void UpdateItemToCart_Should_UpdateItemsSuccessfully()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            var result = _controller.AddProductToCart(customerId, cartItem);
            var okResult = result.Result as OkObjectResult;
            var addedCart = okResult.Value as CartItem;

            addedCart.Quantity += 1;
            addedCart.Discount = 20;
            result = _controller.AddProductToCart(customerId, addedCart);
            okResult = result.Result as OkObjectResult;
            var UpdatedCart = okResult.Value as CartItem;
            Assert.AreEqual(addedCart.CardId, UpdatedCart.CardId);
            Assert.AreEqual(addedCart.Quantity, UpdatedCart.Quantity);
            Assert.AreEqual(addedCart.Discount, UpdatedCart.Discount);
        }


        [Test]
        public void DeleteCart_Should_RemoveCartSuccessfully()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            var result = _controller.AddProductToCart(customerId, cartItem);
            var okResult = result.Result as OkObjectResult;
            var addedCart = okResult.Value as CartItem;


            _controller.DeleteCart(customerId);
            var getresult = _controller.GetCart(customerId) as IActionResult;
            okResult = getresult as OkObjectResult;
            var getCart = okResult.Value as Cart;
            Assert.Null(getCart);
        }

        [Test]
        public void AddItemToCart_ShouldThrowException_WhenCartItemIsNull()
        {
            var customerId = 1;
            CartItem cartItem = null;

            var ex = Assert.Throws<ArgumentNullException>(() => _controller.AddProductToCart(customerId, cartItem));
            Assert.AreEqual("Cart item is empty (Parameter 'cartItem')", ex.Message);
        }


        [Test]
        public void AddItemToCart_ShouldThrowException_WhenPriceIsNegative()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = -1000.00m, Quantity = 1, Discount = 10 };

            var ex = Assert.Throws<ArgumentException>(() => _controller.AddProductToCart(customerId, cartItem));
            Assert.AreEqual("Price must be non-negative", ex.Message);
        }

        [Test]
        public void AddItemToCart_ShouldThrowException_WhenQuantityIsZeroOrLess()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 0, Discount = 10 };

            var ex = Assert.Throws<ArgumentException>(() => _controller.AddProductToCart(customerId, cartItem));
            Assert.AreEqual("Quantity must be greater than zero", ex.Message);
        }

        [Test]
        public void AddItemToCart_ShouldThrowException_WhenDiscountIsNegative()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = -10 };

            var ex = Assert.Throws<ArgumentException>(() => _controller.AddProductToCart(customerId, cartItem));
            Assert.AreEqual("Discount must be non-negative", ex.Message);
        }

        [Test]
        public void GetCart_ShouldReturnNotFound_WhenCartDoesNotExist()
        {
            var customerId = 1;
            var cart = _cartService.GetCart(customerId);
            Assert.Null(cart);
        }

        [Test]
        public void GenerateInvoice_ShouldReturnInvoice_WhenCartIsValid()
        {
            var customerId = 1;
            var cartItem1 = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            _cartService.AddProductToCart(customerId, cartItem1);
            var cartItem2 = new CartItem { Name = "mouse", Price = 900.00m, Quantity = 1, Discount = 2 };
            _cartService.AddProductToCart(customerId, cartItem2);

            var cartList = new List<CartItem>();
            cartList.Add(cartItem1);
            cartList.Add(cartItem2);

            var subTotal = cartList.Sum(t => (t.Price * t.Quantity) - t.Discount);
            var tax = subTotal * 0.1m; //assuming 10% tax rate
            var total = subTotal + tax;
            var discount = cartList.ToList().Sum(item => item.Discount);

            _mockCustomerService.Setup(service => service.GetCustomerById(customerId)).Returns(new Customer { Id = customerId, Name = "John Doe" });

            var result = _controller.GenerateInvoice(customerId, "CreditCard") as IActionResult;
            var okResult = result as OkObjectResult;
            var invoice = okResult.Value as Invoice;

            Assert.NotNull(invoice);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(customerId, invoice.Customer.Id);
            Assert.AreEqual(cartList.Count, invoice.Items.Count);
            Assert.AreEqual(subTotal, invoice.SubTotal);
            Assert.AreEqual(tax, invoice.Tax);
            Assert.AreEqual(total, invoice.Total);
            Assert.AreEqual(discount, invoice.Discount);
            Assert.AreEqual("CreditCard", invoice.PaymentOption);
        }

        [Test]
        public void GenerateInvoice_ShouldThrowException_WhenCartIsEmpty()
        {
            var customerId = 1;

            var ex = Assert.Throws<InvalidOperationException>(() => _cartService.GenerateInvoice(customerId, "CreditCard"));
            Assert.AreEqual("Cart item is empty", ex.Message);
        }

        [Test]
        public void GenerateInvoice_ShouldThrowException_WhenCustomerNotFound()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            _cartService.AddProductToCart(customerId, cartItem);
            _mockCustomerService.Setup(service => service.GetCustomerById(customerId)).Returns((Customer)null);

            var ex = Assert.Throws<InvalidOperationException>(() => _cartService.GenerateInvoice(customerId, "CreditCard"));
            Assert.AreEqual("Customer not found.", ex.Message);
        }


        [Test]
        public void GenerateInvoice_ShouldThrowException_WhenPaymentOptionIsInvalid()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            _cartService.AddProductToCart(customerId, cartItem);
            _mockCustomerService.Setup(service => service.GetCustomerById(customerId)).Returns(new Customer { Id = customerId, Name = "John Doe" });

            var ex = Assert.Throws<ArgumentException>(() => _cartService.GenerateInvoice(customerId, ""));
            Assert.AreEqual("Payment option cannot be null or empty. (Parameter 'paymentOption')", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => _cartService.GenerateInvoice(customerId, "InvalidOption"));
            Assert.AreEqual("Invalid payment option. (Parameter 'paymentOption')", ex.Message);
        }
    }
}
