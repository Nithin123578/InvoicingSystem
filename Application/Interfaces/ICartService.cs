using Application.Models;

namespace Application.Interfaces
{
    public interface ICartService
    {
        CartItem AddProductToCart(int customerId, CartItem cartitem);
        Cart GetCart(int customerId);
        Invoice GenerateInvoice(int customerId, string paymentOption);
        void DeleteCart(int customerId);
    }
}
