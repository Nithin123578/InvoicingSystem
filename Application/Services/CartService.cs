using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CartService : ICartService
    {
        // Service for handling customer-related operations
        private readonly ICustomerService _customerService;

        // Service for handling product-related operations
        private readonly IProductService _productService;

        // In-memory storage for carts
        private readonly List<Cart> _carts = new List<Cart>();

        // Logger for logging errors and other information
        private readonly ILogger<CartService> _logger;

        // Counter for generating unique IDs for cart items
        private int _nextId = 1;

        // Initializes the service with dependencies
        public CartService(ICustomerService customerService, IProductService productService, ILogger<CartService> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Adds a product to the cart for a specified customer
        // Returns the added or updated cart item
        public CartItem AddProductToCart(int customerId, CartItem cartItem)
        {
            try
            {
                // Validates the cart item
                ValidateCartItem(cartItem);

                // Finds the cart for the customer
                var cart = _carts.FirstOrDefault(c => c.CustomerId == customerId);

                // Creates a new cart if it doesn't exist
                if (cart == null)
                {
                    cart = new Cart { CustomerId = customerId, Items = new List<CartItem>() };
                    _carts.Add(cart);
                }

                // Updates the existing item quantity or adds a new item to the cart
                var existingItem = cart.Items.FirstOrDefault(item => item.CardId == cartItem.CardId && cartItem.CardId != 0);
                if (existingItem != null)
                {
                    existingItem.Quantity += cartItem.Quantity;
                    existingItem.Discount = existingItem.Discount;
                    existingItem.Total = existingItem.Price * existingItem.Quantity - existingItem.Discount;
                    return existingItem;
                }
                else
                {
                    var newCartItem = new CartItem
                    {
                        CardId = _nextId++,
                        Name = cartItem.Name,
                        Price = cartItem.Price,
                        Quantity = cartItem.Quantity,
                        Discount = cartItem.Discount,
                        Total = cartItem.Price * cartItem.Quantity - cartItem.Discount
                    };
                    cart.Items.Add(newCartItem);
                    return newCartItem;
                }
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while adding the product to the cart
                _logger.LogError(ex, "Error occurred while adding product to cart.");
                throw;
            }
        }

        // Deletes the cart for a specified customer
        public void DeleteCart(int customerId)
        {
            try
            {
                // Finds and removes the cart for the customer
                var cart = _carts.FirstOrDefault(p => p.CustomerId == customerId);
                if (cart != null)
                {
                    _carts.Remove(cart);
                }
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while deleting the cart
                _logger.LogError(ex, "Error occurred while deleting cart.");
                throw;
            }
        }

        // Validates the cart item to ensure it has valid properties
        private void ValidateCartItem(CartItem cartItem)
        {
            if (cartItem == null)
            {
                throw new ArgumentNullException(nameof(cartItem), "Cart item is empty");
            }
            if (cartItem.Price < 0)
            {
                throw new ArgumentException("Price must be non-negative");
            }
            if (cartItem.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero");
            }
            if (cartItem.Discount < 0)
            {
                throw new ArgumentException("Discount must be non-negative");
            }
        }

        // Retrieves the cart for a specified customer
        // Returns the cart or null if not found
        public Cart GetCart(int customerId)
        {
            try
            {
                return _carts.FirstOrDefault(c => c.CustomerId == customerId);
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while retrieving the cart
                _logger.LogError(ex, "Error occurred while getting cart.");
                throw;
            }
        }

        // Generates an invoice based on the customer's cart and payment option
        // Returns the generated invoice
        public Invoice GenerateInvoice(int customerId, string paymentOption)
        {
            try
            {
                // Validates the payment option
                ValidatePaymentOption(paymentOption);

                // Retrieves the cart for the customer
                var cart = GetCart(customerId);
                if (cart == null || !cart.Items.Any())
                {
                    throw new InvalidOperationException("Cart item is empty");
                }

                // Retrieves the customer details
                var customer = _customerService.GetCustomerById(customerId);
                if (customer == null)
                {
                    throw new InvalidOperationException("Customer not found.");
                }

                // Calculates subtotal, tax, and total for the invoice
                var subTotal = cart.Items.Sum(t => t.Total);
                var tax = subTotal * 0.1m; // Assuming 10% tax rate
                var total = subTotal + tax;

                var invoice = new Invoice
                {
                    Customer = customer,
                    Items = cart.Items.ToList(),
                    SubTotal = subTotal,
                    Discount = cart.Items.ToList().Sum(item => item.Discount),
                    Tax = tax,
                    Total = total,
                    PaymentOption = paymentOption
                };

                return invoice;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while generating the invoice
                _logger.LogError(ex, "Error occurred while generating invoice.");
                throw;
            }
        }

        // Validates the payment option to ensure it is valid
        private void ValidatePaymentOption(string paymentOption)
        {
            if (string.IsNullOrWhiteSpace(paymentOption))
            {
                throw new ArgumentException("Payment option cannot be null or empty.", nameof(paymentOption));
            }

            var validPaymentOptions = new List<string> { "CreditCard", "DebitCard", "PayPal", "Cash" };
            if (!validPaymentOptions.Contains(paymentOption))
            {
                throw new ArgumentException("Invalid payment option.", nameof(paymentOption));
            }
        }
    }
}
