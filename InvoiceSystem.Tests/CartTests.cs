using Application.Interfaces;
using Application.Models;
using Application.Services;
using InvoicingSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvoicingSystem.Tests
{
    [TestFixture]
    public class CartTests
    {
        private CartController _controller;
        private ICartService _cartService;
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<ILogger<CartService>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _loggerMock = new Mock<ILogger<CartService>>();
            _cartService = new CartService(_mockCustomerService.Object, _loggerMock.Object);
            _controller = new CartController(_cartService);
        }

        /// <summary>
        /// Tests that adding an item to the cart is successful and the item is correctly added.
        /// </summary>
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

        /// <summary>
        /// Tests that retrieving the cart returns the correct list of items.
        /// </summary>
        [Test]
        public void GetCart_Should_ReturnCorrectCartList()
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

        /// <summary>
        /// Tests that updating an item in the cart is successful and the item is correctly updated.
        /// </summary>
        [Test]
        public void UpdateItemInCart_Should_UpdateItemsSuccessfully()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            var result = _controller.AddProductToCart(customerId, cartItem);
            var okResult = result.Result as OkObjectResult;
            var addedCartItem = okResult.Value as CartItem;

            addedCartItem.Quantity += 1;
            addedCartItem.Discount = 20;
            result = _controller.AddProductToCart(customerId, addedCartItem);
            okResult = result.Result as OkObjectResult;
            var updatedCartItem = okResult.Value as CartItem;

            Assert.AreEqual(addedCartItem.Name, updatedCartItem.Name);
            Assert.AreEqual(addedCartItem.Quantity, updatedCartItem.Quantity);
            Assert.AreEqual(addedCartItem.Discount, updatedCartItem.Discount);
        }

        /// <summary>
        /// Tests that removing the cart is successful and the cart is correctly removed.
        /// </summary>
        [Test]
        public void DeleteCart_Should_RemoveCartSuccessfully()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            _controller.AddProductToCart(customerId, cartItem);

            _controller.DeleteCart(customerId);
            var getResult = _controller.GetCart(customerId) as IActionResult;
            var okResult = getResult as OkObjectResult;
            var retrievedCart = okResult.Value as Cart;

            Assert.Null(retrievedCart);
        }

        /// <summary>
        /// Tests that adding a null item to the cart throws an ArgumentNullException.
        /// </summary>
        [Test]
        public void AddItemToCart_ShouldThrowException_WhenCartItemIsNull()
        {
            var customerId = 1;
            CartItem cartItem = null;

            var ex = Assert.Throws<ArgumentNullException>(() => _controller.AddProductToCart(customerId, cartItem));
            Assert.AreEqual("Cart item is empty (Parameter 'cartItem')", ex.Message);
        }

        /// <summary>
        /// Tests that adding an item with a negative price throws an ArgumentException.
        /// </summary>
        [Test]
        public void AddItemToCart_ShouldThrowException_WhenPriceIsNegative()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = -1000.00m, Quantity = 1, Discount = 10 };

            var ex = Assert.Throws<ArgumentException>(() => _controller.AddProductToCart(customerId, cartItem));
            Assert.AreEqual("Price must be non-negative", ex.Message);
        }

        /// <summary>
        /// Tests that adding an item with zero or negative quantity throws an ArgumentException.
        /// </summary>
        [Test]
        public void AddItemToCart_ShouldThrowException_WhenQuantityIsZeroOrLess()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 0, Discount = 10 };

            var ex = Assert.Throws<ArgumentException>(() => _controller.AddProductToCart(customerId, cartItem));
            Assert.AreEqual("Quantity must be greater than zero", ex.Message);
        }

        /// <summary>
        /// Tests that adding an item with a negative discount throws an ArgumentException.
        /// </summary>
        [Test]
        public void AddItemToCart_ShouldThrowException_WhenDiscountIsNegative()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = -10 };

            var ex = Assert.Throws<ArgumentException>(() => _controller.AddProductToCart(customerId, cartItem));
            Assert.AreEqual("Discount must be non-negative", ex.Message);
        }

        /// <summary>
        /// Tests that generating an invoice throws an exception when the cart is empty.
        /// </summary>
        [Test]
        public void GenerateInvoice_ShouldThrowException_WhenCartIsEmpty()
        {
            var customerId = 1;

            var ex = Assert.Throws<InvalidOperationException>(() => _cartService.GenerateInvoice(customerId, "CreditCard"));
            Assert.AreEqual("Cart item is empty", ex.Message);
        }

        /// <summary>
        /// Tests that generating an invoice returns the correct details when the cart is valid.
        /// </summary>
        [Test]
        public void GenerateInvoice_ShouldReturnInvoice_WhenCartIsValid()
        {
            var customerId = 1;
            var cartItem1 = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            var cartItem2 = new CartItem { Name = "Mouse", Price = 900.00m, Quantity = 1, Discount = 2 };

            _cartService.AddProductToCart(customerId, cartItem1);
            _cartService.AddProductToCart(customerId, cartItem2);

            var cartList = new List<CartItem> { cartItem1, cartItem2 };

            var subTotal = cartList.Sum(t => (t.Price * t.Quantity) - t.Discount);
            var tax = subTotal * 0.1m; // Assuming a 10% tax rate
            var total = subTotal + tax;
            var discount = cartList.Sum(item => item.Discount);

            _mockCustomerService.Setup(service => service.GetCustomerById(customerId))
                .Returns(new Customer { Id = customerId, Name = "John Doe" });

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

        /// <summary>
        /// Tests that generating an invoice throws an exception when the customer is not found.
        /// </summary>
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

        /// <summary>
        /// Tests that generating an invoice throws an exception for invalid payment options.
        /// </summary>
        [Test]
        public void GenerateInvoice_ShouldThrowException_WhenPaymentOptionIsInvalid()
        {
            var customerId = 1;
            var cartItem = new CartItem { Name = "Laptop", Price = 1000.00m, Quantity = 1, Discount = 10 };
            _cartService.AddProductToCart(customerId, cartItem);
            _mockCustomerService.Setup(service => service.GetCustomerById(customerId))
                .Returns(new Customer { Id = customerId, Name = "John Doe" });

            var ex = Assert.Throws<ArgumentException>(() => _cartService.GenerateInvoice(customerId, ""));
            Assert.AreEqual("Payment option cannot be null or empty. (Parameter 'paymentOption')", ex.Message);

            ex = Assert.Throws<ArgumentException>(() => _cartService.GenerateInvoice(customerId, "InvalidOption"));
            Assert.AreEqual("Invalid payment option. (Parameter 'paymentOption')", ex.Message);
        }
    }
}
