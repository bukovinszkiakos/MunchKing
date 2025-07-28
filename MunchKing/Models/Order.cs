using Microsoft.AspNetCore.Identity;
using MunchKing.Enums;

namespace MunchKing.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string PaymentMode { get; set; } = default!;

        public int DisplayOrderNumber { get; set; }


        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public ApplicationUser User { get; set; } = default!;

        public decimal TotalAmount { get; set; }

    }

}
