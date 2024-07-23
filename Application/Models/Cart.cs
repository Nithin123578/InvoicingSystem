namespace Application.Models
{
    public class CartItem : Product
    {
        public int CardId { get; set; }
        public decimal Discount { get; set; } // Individual product discount
        public decimal Total { get; set; } // Individual product discount
    }

    public class Cart
    {
        public int CustomerId { get; set; }
        public List<CartItem> Items { get; set; }
    }
}
