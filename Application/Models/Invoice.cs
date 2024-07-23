namespace Application.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<CartItem> Items { get; set; }
        public decimal SubTotal { get; set; }
         public decimal Discount { get; set; } // Flat discount on total purchase
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string PaymentOption { get; set; }
    }
}
