using Application.Interfaces;
using Application.Models;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly List<Cart> _carts = new List<Cart>();
        private int _nextId = 1;
        public CartService(ICustomerService customerService, IProductService productService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public CartItem AddProductToCart(int customerId, CartItem cartItem)
        {
            ValidateCartItem(cartItem);
            var cart = _carts.FirstOrDefault(c => c.CustomerId == customerId);
            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId, Items = new List<CartItem>() };
                _carts.Add(cart);
            }
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

        public void DeleteCart(int customerId)
        {
            var cart = _carts.FirstOrDefault(p => p.CustomerId == customerId);
            if (cart != null)
            {
                _carts.Remove(cart);
            }
        }

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

        public Cart GetCart(int customerId)
        {
            return _carts.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public Invoice GenerateInvoice(int customerId, string paymentOption)
        {
            ValidatePaymentOption(paymentOption);
            var cart = GetCart(customerId);
            if (cart == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Cart item is empty");
            }

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found.");
            }
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
