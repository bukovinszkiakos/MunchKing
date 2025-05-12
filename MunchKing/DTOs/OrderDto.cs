namespace MunchKing.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = default!;
        public string PaymentMode { get; set; } = default!;
        public List<OrderItemDto> Items { get; set; } = new();
    }
}