namespace MunchKing.DTOs
{
    public class AdminOrderDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = default!;
        public string PaymentMode { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string UserEmail { get; set; } = default!;
        public string Username { get; set; } = default!;

        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
