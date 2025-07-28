namespace MunchKing.DTOs
{
    public class CheckoutRequest
    {
        public string PaymentMode { get; set; } = default!; 

        public List<CartItem> Items { get; set; } = new();

        public class CartItem
        {
            public int FoodItemId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
